using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Altova.Types;
using ConfigurationWizard;
using Dapple.CustomControls;
using Dapple.LayerGeneration;
using Dapple.Properties;
using dappleview;
using DM.SharedMemory;
using Geosoft.GX.DAPGetData;
using Microsoft.Win32;
using MontajRemote;
using Utility;
using WorldWind;
using WorldWind.Net;
using WorldWind.PluginEngine;
using WorldWind.Renderable;
using NewServerTree;

namespace Dapple
{
	internal partial class MainForm : MainApplication
	{
		#region Win32 DLLImports

		[DllImport("User32.dll")]
		private static extern UInt32 RegisterWindowMessageW(String strMessage);

		private struct RECT
		{
			internal int left;
			internal int top;
			internal int right;
			internal int bottom;

			public static implicit operator Rectangle(RECT rect)
			{
				return new Rectangle(rect.left, rect.top, rect.right - rect.left,
				rect.bottom - rect.top);
			}
		}

		private const int DCX_WINDOW = 0x00000001;
		private const int DCX_CACHE = 0x00000002;
		private const int DCX_LOCKWINDOWUPDATE = 0x00000400;
		private const int SRCCOPY = 0x00CC0020;
		private const int CAPTUREBLT = 0x40000000;
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int
		flags);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern bool BitBlt(
		IntPtr hdcDest,
		int nXDest,
		int nYDest,
		int nWidth,
		int nHeight,
		IntPtr hdcSrc,
		int nXSrc,
		int nYSrc,
		int dwRop
		);

		#endregion

		#region Delegates

		/// <summary>
		/// Called when this control selects a layer to tell others to load Metadata for it.
		/// </summary>
		/// <param name="?"></param>
		internal delegate void ViewMetadataHandler(IBuilder oBuilder);

		#endregion

		#region Constants

		internal const int MAX_MRU_TERMS = 8;
		internal const string ViewExt = ".dapple";
		internal const string FavouriteServerExt = ".dapple_serverlist";
		internal const string LastView = "lastview" + ViewExt;
		internal const string DefaultView = "default" + ViewExt;
		internal const string ViewFileDescr = "Dapple View";
		internal const string LinkFileDescr = "Dapple Link";
		internal const string WebsiteUrl = "http://dapple.geosoft.com/";
		internal const string VersionFile = "version.txt";
		internal const string LicenseWebsiteUrl = "http://dapple.geosoft.com/license.asp";
		internal const string CreditsWebsiteUrl = "http://dapple.geosoft.com/credits.asp";
		internal const string ReleaseNotesWebsiteUrl = "http://dapple.geosoft.com/releasenotes.asp";
		internal const string WebsiteHelpUrl = "http://dapple.geosoft.com/help/";
		internal const string WebsiteForumsHelpUrl = "https://dappleforums.geosoft.com/";
		internal const string WMSWebsiteHelpUrl = "http://dapple.geosoft.com/help/wms.asp";
		internal const string DAPWebsiteHelpUrl = "http://dapple.geosoft.com/help/dap.asp";
		internal const string NEW_SERVER_GATEWAY = "AddNewServer.aspx";
		internal const string SEARCH_XML_GATEWAY = "SearchInterfaceXML.aspx";
		internal const string NO_SEARCH = "--- Enter keyword(s) ---";
		internal static readonly UInt32 OpenViewMessage = RegisterWindowMessageW("Dapple.OpenViewMessage");
		internal static readonly string UserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DappleData");

		#endregion

		#region Private Members

		private Splash splashScreen;
		private DappleModel m_oModel;
		private ServerList c_oServerList;
		private JanaTab cServerViewsTab;
		private DappleSearchList c_oDappleSearch;
		private static WorldWindow c_oWorldWindow;

		internal static WorldWindow WorldWindowSingleton
		{
			get { return c_oWorldWindow; }
		}

		private NASA.Plugins.ScaleBarLegend scalebarPlugin;

		private Murris.Plugins.Compass compassPlugin;
		private Murris.Plugins.GlobalClouds cloudsPlugin;
		private Murris.Plugins.SkyGradient skyPlugin;

		private Stars3D.Plugin.Stars3D starsPlugin;
		private ThreeDconnexion.Plugin.TDxWWInput threeDConnPlugin;

		private RenderableObjectList placeNames;

		private string openView = "";
		private string m_strOpenGeoTiffFile = "";
		private string m_strOpenGeoTiffName = "";
		private bool m_blOpenGeoTiffTmp = false;
		private string m_strOpenKMLFile = "";
		private string m_strOpenKMLName = "";
		private bool m_blOpenKMLTmp = false;
		private bool m_blSelectPersonalDAP = false;

		private string lastView = "";
		private string metaviewerDir = "";

		private MetadataDisplayThread m_oMetadataDisplay;

		private static ImageList s_oImageList = new ImageList();
		private static RemoteInterface s_oMontajRemoteInterface;
		private static Dapple.Extract.Options.Client.ClientType s_eClientType;
		private static GeographicBoundingBox s_oOMMapExtentWGS84;
		private static GeographicBoundingBox s_oOMMapExtentNative;
		private static string s_strAoiCoordinateSystem;
		private static string s_strOpenMapFileName = string.Empty;
		private Dictionary<String, GeographicBoundingBox> m_oCountryAOIs;

		private string m_szLastSearchString = String.Empty;
		private GeographicBoundingBox m_oLastSearchROI = null;
		#endregion

		#region Properties

		internal void SetSelectPersonalDAPOnStartup(bool blValue)
		{
			m_blSelectPersonalDAP = blValue;
		}

		private String SearchKeyword
		{
			get
			{
				String result = c_tbSearchKeywords.Text.Trim();
				if (result.Equals(NO_SEARCH)) result = String.Empty;

				return result;
			}
		}

		private static bool IsRunningAsDapClient
		{
			get { return s_oMontajRemoteInterface != null; }
		}

		internal static bool IsRunningAsMapinfoDapClient
		{
			get { return IsRunningAsDapClient && s_eClientType == Dapple.Extract.Options.Client.ClientType.MapInfo; }
		}

		internal static ImageList DataTypeImageList
		{
			get { return s_oImageList; }
		}

		public const string DisabledServerIconKey = "disserver";
		public const string OfflineServerIconKey = "offline";
		public const string EnabledServerIconKey = "enserver";
		public const string LiveMapsIconKey = "live";
		public const string DapDatabaseIconKey = "dap_database";
		public const string DapDocumentIconKey = "dap_document";
		public const string DapMapIconKey = "dap_map";
		public const string DapGridIconKey = "dap_grid";
		public const string DapPictureIconKey = "dap_picture";
		public const string DapPointIconKey = "dap_point";
		public const string DapSpfIconKey = "dap_spf";
		public const string DapVoxelIconKey = "dap_voxel";
		public const string DapArcGisIconKey = "dap_arcgis";
		public const string LayerIconKey = "layer";
		public const string ErrorIconKey = "error";
		public const string ArcImsIconKey = "arcims";
		public const string GeorefImageIconKey = "georef_image";
		public const string WmsIconKey = "wms";
		public const string KmlIconKey = "kml";
		public const string BlueMarbleIconKey = "blue_marble";
		public const string DapIconKey = "dap";
		public const string TileIconKey = "tile";
		public const string FolderIconKey = "folder";
		public const string DesktopCatalogerIconKey = "desktopcataloger";

		internal static RemoteInterface MontajInterface
		{
			get
			{
				return s_oMontajRemoteInterface;
			}
		}

		/// <summary>
		/// Get the client
		/// </summary>
		internal static Dapple.Extract.Options.Client.ClientType Client
		{
			get { return s_eClientType; }
		}

		/// <summary>
		/// Get the open map area of interest
		/// </summary>
		internal static GeographicBoundingBox MapAoi
		{
			get
			{
				return s_oOMMapExtentNative == null ? null : s_oOMMapExtentNative.Clone() as GeographicBoundingBox;
			}
		}

		/// <summary>
		/// Get the open map coordinate system
		/// </summary>
		internal static string MapAoiCoordinateSystem
		{
			get { return s_strAoiCoordinateSystem; }
		}

		/// <summary>
		/// Get the name of the open map
		/// </summary>
		internal static string MapFileName
		{
			get { return s_strOpenMapFileName; }
		}

		#region Blue Marble

		private RenderableObject GetBMNG()
		{
			for (int i = 0; i < c_oWorldWindow.CurrentWorld.RenderableObjects.Count; i++)
			{
				if (((RenderableObject)c_oWorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i]).Name == "4 - The Blue Marble")
					return c_oWorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i] as RenderableObject;
			}
			return null;
		}

		private RenderableObject GetActiveBMNG()
		{
			RenderableObject roBMNG = GetBMNG();
			if (roBMNG != null && roBMNG.IsOn)
				return GetActiveBMNG(roBMNG);
			else
				return null;
		}

		private RenderableObjectList GetActiveBMNG(RenderableObject roBMNG)
		{
			if (roBMNG is RenderableObjectList)
			{
				if ((roBMNG as RenderableObjectList).ChildObjects.Count == 2 && roBMNG.isInitialized)
					return roBMNG as RenderableObjectList;
				for (int i = 0; i < (roBMNG as RenderableObjectList).Count; i++)
				{
					RenderableObject ro = (RenderableObject)(roBMNG as RenderableObjectList).ChildObjects[i];
					if (ro is RenderableObjectList)
					{
						if ((ro as RenderableObjectList).ChildObjects.Count != 2)
						{
							for (int j = 0; j < (ro as RenderableObjectList).Count; j++)
							{
								RenderableObjectList roRet = GetActiveBMNG((RenderableObject)(ro as RenderableObjectList).ChildObjects[j]);
								if (roRet != null)
									return roRet;
							}
						}
						else if (ro.isInitialized)
							return ro as RenderableObjectList;
					}
				}
			}
			return null;
		}

		#endregion

		#endregion

		#region Constructor

		internal MainForm(string strView,
			string strGeoTiff, string strGeotiffName, bool bGeotiffTmp,
			string strKMLFile, string strKMLName, bool blKMLTmp, 
			string strLastView, Dapple.Extract.Options.Client.ClientType eClientType, RemoteInterface oMRI, GeographicBoundingBox oAoi, string strAoiCoordinateSystem, string strMapFileName)
		{
			if (String.Compare(Path.GetExtension(strView), ViewExt, true) == 0 && File.Exists(strView))
				this.openView = strView;

			m_strOpenGeoTiffFile = strGeoTiff;
			m_strOpenGeoTiffName = strGeotiffName;
			m_blOpenGeoTiffTmp = bGeotiffTmp;

			m_strOpenKMLFile = strKMLFile;
			m_strOpenKMLName = strKMLName;
			m_blOpenKMLTmp = blKMLTmp;

			this.lastView = strLastView;
			s_oMontajRemoteInterface = oMRI;

			// Establish the version number string used for user display,
			// such as the Splash and Help->About screens.
			// To change the Application.ProductVersion make the
			// changes in \WorldWind\AssemblyInfo.cs
			// For alpha/beta versions, include " alphaN" or " betaN"
			// at the end of the format string.
			Version ver = new Version(Application.ProductVersion);
			Release = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
			if (ver.Build % 2 != 0)
				Release += " (BETA)";

			// Name the main thread.
			System.Threading.Thread.CurrentThread.Name = ThreadNames.EventDispatch;

			// Copy/Update any configuration files and other files if needed now
			CurrentSettingsDirectory = Path.Combine(UserPath, "Config");
			Directory.CreateDirectory(CurrentSettingsDirectory);
			Settings.CachePath = Path.Combine(UserPath, "Cache");
			Directory.CreateDirectory(Settings.CachePath);
			this.metaviewerDir = Path.Combine(UserPath, "Metadata");
			Directory.CreateDirectory(this.metaviewerDir);
			string[] cfgFiles = Directory.GetFiles(Path.Combine(DirectoryPath, "Config"), "*.xml");
			foreach (string strCfgFile in cfgFiles)
			{
				string strUserCfg = Path.Combine(CurrentSettingsDirectory, Path.GetFileName(strCfgFile));
				if (!File.Exists(strUserCfg))
					File.Copy(strCfgFile, strUserCfg);
			}
			string[] metaFiles = Directory.GetFiles(Path.Combine(Path.Combine(DirectoryPath, "Data"), "MetaViewer"), "*.*");
			foreach (string strMetaFile in metaFiles)
			{
				string strUserMeta = Path.Combine(this.metaviewerDir, Path.GetFileName(strMetaFile));
				File.Copy(strMetaFile, strUserMeta, true);
			}

			// --- Set up a new user's favorites list and home view ---

			/*if (!File.Exists(Path.Combine(CurrentSettingsDirectory, "user.dapple_serverlist")))
			{
				File.Copy(Path.Combine(Path.Combine(DirectoryPath, "Data"), "default.dapple_serverlist"), Path.Combine(CurrentSettingsDirectory, "user.dapple_serverlist"));
			}*/
			HomeView.CreateDefault();

			InitSettings();

			if (Settings.NewCachePath.Length > 0)
			{
				try
				{
					// We want to make sure the new cache path is writable
					Directory.CreateDirectory(Settings.NewCachePath);
					if (Directory.Exists(Settings.CachePath))
						Utility.FileSystem.DeleteFolderGUI(this, Settings.CachePath, "Deleting Existing Cache");
					Settings.CachePath = Settings.NewCachePath;
				}
				catch
				{
				}
				Settings.NewCachePath = "";
			}


			if (Settings.ConfigurationWizardAtStartup)
			{
				Wizard frm = new Wizard(Settings);
				frm.ShowDialog(this);
				Settings.ConfigurationWizardAtStartup = false;
			}

			if (Settings.ConfigurationWizardAtStartup)
			{
				// If the settings file doesn't exist, then we are using the
				// default settings, and the default is to show the Configuration
				// Wizard at startup. We only want that to happen the first time
				// World Wind is started, so change the setting to false(the user
				// can change it to true if they want).
				if (!File.Exists(Settings.FileName))
				{
					Settings.ConfigurationWizardAtStartup = false;
				}
				ConfigurationWizard.Wizard wizard = new ConfigurationWizard.Wizard(Settings);
				wizard.TopMost = true;
				wizard.ShowInTaskbar = true;
				wizard.ShowDialog();
				// TODO: should settings be saved now, in case of program crashes,
				//	   and so that XML file on disk matches in-memory settings?
			}

			//#if !DEBUG
			using (this.splashScreen = new Splash())
			{
				this.splashScreen.Owner = this;
				this.splashScreen.Show();

				Application.DoEvents();
				//#endif

				// --- setup the list of images used for the different datatypes ---

				s_oImageList.ColorDepth = ColorDepth.Depth32Bit;
				s_oImageList.ImageSize = new Size(16, 16);
				s_oImageList.TransparentColor = Color.Transparent;

				s_oImageList.Images.Add(EnabledServerIconKey, Resources.enserver);
				s_oImageList.Images.Add(DisabledServerIconKey, Resources.disserver);
				s_oImageList.Images.Add(OfflineServerIconKey, Resources.offline);
				s_oImageList.Images.Add(DapIconKey, Resources.dap);
				s_oImageList.Images.Add(DapDatabaseIconKey, Resources.dap_database);
				s_oImageList.Images.Add(DapDocumentIconKey, Resources.dap_document);
				s_oImageList.Images.Add(DapGridIconKey, Resources.dap_grid);
				s_oImageList.Images.Add(DapMapIconKey, Resources.dap_map);
				s_oImageList.Images.Add(DapPictureIconKey, Resources.dap_picture);
				s_oImageList.Images.Add(DapPointIconKey, Resources.dap_point);
				s_oImageList.Images.Add(DapSpfIconKey, Resources.dap_spf);
				s_oImageList.Images.Add(DapVoxelIconKey, Resources.dap_voxel);
				s_oImageList.Images.Add(FolderIconKey, Resources.folder);
				s_oImageList.Images.Add(DapArcGisIconKey, global::Dapple.Properties.Resources.dap_arcgis);
				s_oImageList.Images.Add(KmlIconKey, Resources.kml);
				s_oImageList.Images.Add(ErrorIconKey, global::Dapple.Properties.Resources.error);
				s_oImageList.Images.Add(LayerIconKey, global::Dapple.Properties.Resources.layer);
				s_oImageList.Images.Add(LiveMapsIconKey, global::Dapple.Properties.Resources.live);
				s_oImageList.Images.Add(TileIconKey, global::Dapple.Properties.Resources.tile);
				s_oImageList.Images.Add(GeorefImageIconKey, global::Dapple.Properties.Resources.georef_image);
				s_oImageList.Images.Add(WmsIconKey, Resources.wms);
				s_oImageList.Images.Add(ArcImsIconKey, global::Dapple.Properties.Resources.arcims);
				s_oImageList.Images.Add(BlueMarbleIconKey, Dapple.Properties.Resources.blue_marble);
				s_oImageList.Images.Add(DesktopCatalogerIconKey, Dapple.Properties.Resources.dcat);

				c_oWorldWindow = new WorldWindow();
#if !DEBUG
				Utility.AbortUtility.ProgramAborting += new MethodInvoker(c_oWorldWindow.KillD3DAndWorkerThread);
#endif
				c_oWorldWindow.AllowDrop = true;
				c_oWorldWindow.DragOver += new DragEventHandler(c_oWorldWindow_DragOver);
				c_oWorldWindow.DragDrop += new DragEventHandler(c_oWorldWindow_DragDrop);
				c_oWorldWindow.Resize += new EventHandler(c_oWorldWindow_Resize);
				InitializeComponent();
				this.SuspendLayout();
				c_oLayerList.ImageList = s_oImageList;

/*#if DEBUG
				// --- Make the server tree HOOGE ---
				this.splitContainerMain.SplitterDistance = 400;
				this.splitContainerLeftMain.SplitterDistance = 400;
#endif*/

				this.Icon = new System.Drawing.Icon(@"app.ico");
				DappleToolStripRenderer oTSR = new DappleToolStripRenderer();
				c_tsSearch.Renderer = oTSR;
				c_tsLayers.Renderer = oTSR;
				c_tsOverview.Renderer = oTSR;
				c_tsMetadata.Renderer = oTSR;

				c_tsNavigation.Renderer = new BorderlessToolStripRenderer();

				// set Upper and Lower limits for Cache size control, in bytes
				long CacheUpperLimit = (long)Settings.CacheSizeGigaBytes * 1024L * 1024L * 1024L;
				long CacheLowerLimit = (long)Settings.CacheSizeGigaBytes * 768L * 1024L * 1024L;	//75% of upper limit

				try
				{
					Directory.CreateDirectory(Settings.CachePath);
				}
				catch
				{
					// We get here when people used a cache drive that since dissappeared (e.g. USB flash)
					// Revert to default cache directory in this case

					Settings.CachePath = Path.Combine(UserPath, "Cache");
					Directory.CreateDirectory(Settings.CachePath);
				}

				//Set up the cache
				c_oWorldWindow.Cache = new Cache(
					Settings.CachePath,
					CacheLowerLimit,
					CacheUpperLimit,
					Settings.CacheCleanupInterval,
					Settings.TotalRunTime);

				#region Plugin + World Init.

				WorldWind.Terrain.TerrainTileService terrainTileService = new WorldWind.Terrain.TerrainTileService("http://worldwind25.arc.nasa.gov/wwelevation/wwelevation.aspx", "srtm30pluszip", 20, 150, "bil", 12, Path.Combine(Settings.CachePath, "Earth\\TerrainAccessor\\SRTM"), TimeSpan.FromMinutes(30), "Int16");
				WorldWind.Terrain.TerrainAccessor terrainAccessor = new WorldWind.Terrain.NltTerrainAccessor("SRTM", -180, -90, 180, 90, terrainTileService, null);

				WorldWind.World world = new WorldWind.World("Earth",
					new Point3d(0, 0, 0), Quaternion4d.RotationYawPitchRoll(0, 0, 0),
					(float)6378137,
					System.IO.Path.Combine(c_oWorldWindow.Cache.CacheDirectory, "Earth"),
					terrainAccessor);

				c_oWorldWindow.CurrentWorld = world;
				c_oWorldWindow.DrawArgs.WorldCamera.CameraChanged += new EventHandler(c_oWorldWindow_CameraChanged);

				string strPluginsDir = Path.Combine(DirectoryPath, "Plugins");

				this.scalebarPlugin = new NASA.Plugins.ScaleBarLegend();
				this.scalebarPlugin.PluginLoad(this, strPluginsDir);
				this.scalebarPlugin.IsVisible = World.Settings.ShowScaleBar;

				this.starsPlugin = new Stars3D.Plugin.Stars3D();
				this.starsPlugin.PluginLoad(this, Path.Combine(strPluginsDir, "Stars3D"));

				this.compassPlugin = new Murris.Plugins.Compass();
				this.compassPlugin.PluginLoad(this, Path.Combine(strPluginsDir, "Compass"));

				String szGlobalCloudsCacheDir = Path.Combine(Settings.CachePath, @"Plugins\GlobalClouds");
				Directory.CreateDirectory(szGlobalCloudsCacheDir);
				String szGlobalCloudsPluginDir = Path.Combine(CurrentSettingsDirectory, @"Plugins\GlobalClouds");
				Directory.CreateDirectory(szGlobalCloudsPluginDir);

				if (!File.Exists(Path.Combine(szGlobalCloudsPluginDir, Murris.Plugins.GlobalCloudsLayer.serverListFileName)))
				{
					File.Copy(Path.Combine(Path.Combine(strPluginsDir, "GlobalClouds"), Murris.Plugins.GlobalCloudsLayer.serverListFileName), Path.Combine(szGlobalCloudsPluginDir, Murris.Plugins.GlobalCloudsLayer.serverListFileName));
				}

				this.cloudsPlugin = new Murris.Plugins.GlobalClouds(szGlobalCloudsCacheDir);
				this.cloudsPlugin.PluginLoad(this, szGlobalCloudsPluginDir);

				this.skyPlugin = new Murris.Plugins.SkyGradient();
				this.skyPlugin.PluginLoad(this, Path.Combine(strPluginsDir, "SkyGradient"));

				this.threeDConnPlugin = new ThreeDconnexion.Plugin.TDxWWInput();
				this.threeDConnPlugin.PluginLoad(this, Path.Combine(strPluginsDir, "3DConnexion"));

				ThreadPool.QueueUserWorkItem(LoadPlacenames);

				c_scWorldMetadata.Panel1.Controls.Add(c_oWorldWindow);
				c_oWorldWindow.Dock = DockStyle.Fill;

				#endregion

				float[] verticalExaggerationMultipliers = { 0.0f, 1.0f, 1.5f, 2.0f, 3.0f, 5.0f, 7.0f, 10.0f };
				foreach (float multiplier in verticalExaggerationMultipliers)
				{
					ToolStripMenuItem curItem = new ToolStripMenuItem(multiplier.ToString("f1", System.Threading.Thread.CurrentThread.CurrentCulture) + "x", null, new EventHandler(menuItemVerticalExaggerationChange));
					c_miVertExaggeration.DropDownItems.Add(curItem);
					curItem.CheckOnClick = true;
					if (Math.Abs(multiplier - World.Settings.VerticalExaggeration) < 0.1f)
						curItem.Checked = true;
				}

				this.c_miShowCompass.Checked = World.Settings.ShowCompass;
				this.c_miShowDLProgress.Checked = World.Settings.ShowDownloadIndicator;
				this.c_miShowCrosshair.Checked = World.Settings.ShowCrosshairs;
				this.c_miShowInfoOverlay.Checked = World.Settings.ShowPosition;
				this.c_miShowGridlines.Checked = World.Settings.ShowLatLonLines;
				this.c_miShowGlobalClouds.Checked = World.Settings.ShowClouds;
				if (World.Settings.EnableSunShading)
				{
					if (!World.Settings.SunSynchedWithTime)
						this.c_miSunshadingEnabled.Checked = true;
					else
						this.c_miSunshadingSync.Checked = true;
				}
				else
					this.c_miSunshadingDisabled.Checked = true;
				this.c_miShowAtmoScatter.Checked = World.Settings.EnableAtmosphericScattering;

				this.c_miAskLastViewAtStartup.Checked = Settings.AskLastViewAtStartup;
				if (!Settings.AskLastViewAtStartup)
					this.c_miOpenLastViewAtStartup.Checked = Settings.LastViewAtStartup;


				#region OverviewPanel

				// Fix: earlier versions of Dapple set the DataPath as an absolute reference, so if Dapple was uninstalled, OMapple could not find
				// the file for the overview control.  To fix this, switch the variable to a relative reference if the absolute one doesn't resolve.
				// Dapple will still work; the relative reference will be from whatever directory Dapple is being run.
				if (!Directory.Exists(Settings.DataPath)) Settings.DataPath = "Data";

				#endregion


				c_oWorldWindow.MouseEnter += new EventHandler(this.c_oWorldWindow_MouseEnter);
				c_oWorldWindow.MouseLeave += new EventHandler(this.c_oWorldWindow_MouseLeave);
				c_oOverview.AOISelected += new Overview.AOISelectedDelegate(c_oOverview_AOISelected);

				#region Search view setup

				this.c_oServerList = new ServerList();
				m_oModel = new DappleModel(c_oLayerList);
				m_oModel.SelectedNodeChanged += new EventHandler(m_oModel_SelectedNodeChanged);
				c_oLayerList.Attach(m_oModel);
				NewServerTree.View.ServerTree newServerTree = new NewServerTree.View.ServerTree();
				newServerTree.Attach(m_oModel);
				c_oServerList.Attach(m_oModel);
				c_oLayerList.LayerSelectionChanged += new EventHandler(c_oLayerList_LayerSelectionChanged);

				m_oMetadataDisplay = new MetadataDisplayThread(this);
				m_oMetadataDisplay.AddBuilder(null);
				c_oServerList.LayerList = c_oLayerList;
				c_oLayerList.GoTo += new LayerList.GoToHandler(this.GoTo);

				c_oLayerList.ViewMetadata += new ViewMetadataHandler(m_oMetadataDisplay.AddBuilder);
				c_oServerList.ViewMetadata += new ViewMetadataHandler(m_oMetadataDisplay.AddBuilder);
				c_oServerList.LayerSelectionChanged += new EventHandler(c_oServerList_LayerSelectionChanged);

				this.cServerViewsTab = new JanaTab();
				this.cServerViewsTab.SetImage(0, Resources.tab_tree);
				this.cServerViewsTab.SetImage(1, Resources.tab_list);
				this.cServerViewsTab.SetToolTip(0, "Server tree view");
				this.cServerViewsTab.SetToolTip(1, "Server list view");
				this.cServerViewsTab.SetNameAndText(0, "TreeView");
				this.cServerViewsTab.SetNameAndText(1, "ListView");
				this.cServerViewsTab.SetPage(0, newServerTree);
				this.cServerViewsTab.SetPage(1, this.c_oServerList);
				cServerViewsTab.PageChanged += new JanaTab.PageChangedDelegate(ServerPageChanged);

				c_oDappleSearch = new DappleSearchList();
				c_oDappleSearch.LayerSelectionChanged += new EventHandler(c_oDappleSearch_LayerSelectionChanged);
				c_oDappleSearch.Attach(m_oModel, c_oLayerList);

				c_tcSearchViews.TabPages[0].Controls.Add(cServerViewsTab);
				cServerViewsTab.Dock = DockStyle.Fill;
				c_tcSearchViews.TabPages[1].Controls.Add(c_oDappleSearch);
				c_oDappleSearch.Dock = DockStyle.Fill;

				c_oLayerList.SetBaseLayer(new BlueMarbleBuilder());

				this.ResumeLayout(false);

				#endregion

				this.PerformLayout();

				while (!this.splashScreen.IsDone)
					System.Threading.Thread.Sleep(50);

				// Force initial render to avoid showing random contents of frame buffer to user.
				c_oWorldWindow.Render();
				WorldWindow.Focus();

				#region OM Forked Process configuration

				if (IsRunningAsDapClient)
				{
					c_oLayerList.OMFeaturesEnabled = true;
					this.MinimizeBox = false;

					if (oAoi != null && !string.IsNullOrEmpty(strAoiCoordinateSystem))
					{
						s_oOMMapExtentNative = oAoi;
						s_strAoiCoordinateSystem = strAoiCoordinateSystem;
						s_strOpenMapFileName = strMapFileName;

						s_oOMMapExtentWGS84 = s_oOMMapExtentNative.Clone() as GeographicBoundingBox;
						s_oMontajRemoteInterface.ProjectBoundingRectangle(strAoiCoordinateSystem, ref s_oOMMapExtentWGS84.West, ref s_oOMMapExtentWGS84.South, ref s_oOMMapExtentWGS84.East, ref s_oOMMapExtentWGS84.North, Dapple.Extract.Resolution.WGS_84);
					}
					s_eClientType = eClientType;

					c_miLastView.Enabled = false;
					c_miLastView.Visible = false;
					c_miDappleHelp.Visible = false;
					c_miDappleHelp.Enabled = false;
					toolStripSeparator10.Visible = false;
					c_miOpenImage.Visible = false;
					c_miOpenImage.Enabled = false;
					c_miOpenKeyhole.Visible = false;
					c_miOpenKeyhole.Enabled = false;

					// Hide and disable the file menu
					c_miFile.Visible = false;
					c_miFile.Enabled = false;
					c_miOpenSavedView.Visible = false;
					c_miOpenSavedView.Enabled = false;
					c_miOpenHomeView.Visible = false;
					c_miOpenHomeView.Enabled = false;
					c_miSetHomeView.Visible = false;
					c_miSetHomeView.Enabled = false;
					c_miSaveView.Visible = false;
					c_miSaveView.Enabled = false;
					c_miSendViewTo.Visible = false;
					c_miSendViewTo.Enabled = false;
					c_miOpenKeyhole.Visible = false;
					c_miOpenKeyhole.Enabled = false;

					// Show the OM help menu
					c_miGetDatahelp.Enabled = true;
					c_miGetDatahelp.Visible = true;

					// Don't let the user check for updates.  EVER.
					c_miCheckForUpdates.Visible = false;
					c_miCheckForUpdates.Enabled = false;
				}
				else
				{
					c_miExtractLayers.Visible = false;
					c_miExtractLayers.Enabled = false;
				}

				#endregion

				loadCountryList();
				populateAoiComboBox();
				LoadMRUList();
				CenterNavigationToolStrip();
				//#if !DEBUG

				c_tbSearchKeywords.Text = NO_SEARCH;
			}
			//#endif
		}

		void m_oModel_SelectedNodeChanged(object sender, EventArgs e)
		{
			if (m_oModel.SelectedNode is LayerModelNode)
			{
#pragma warning disable 618
				m_oMetadataDisplay.AddBuilder((m_oModel.SelectedNode as LayerModelNode).ConvertToLayerBuilder());
#pragma warning restore 618
			}

			populateAoiComboBox();
			CmdSetupToolsMenu();
			CmdSetupServersMenu();
		}

		#endregion

		#region Updates

		bool m_bUpdateFromMenu;
		delegate void NotifyUpdateDelegate(string strVersion);

		private void NotifyUpdate(string strVersion)
		{
			UpdateDialog dlg = new UpdateDialog(strVersion);

			if (dlg.ShowDialog(this) == DialogResult.Yes)
				MainForm.BrowseTo(MainForm.WebsiteUrl);
		}

		private void NotifyUpdateNotAvailable()
		{
			Program.ShowMessageBox(
				"Your version of Dapple is up-to-date.",
				"Check For Updates",
				MessageBoxButtons.OK,
				MessageBoxDefaultButton.Button1,
				MessageBoxIcon.Information);
		}

		private void UpdateDownloadComplete(WebDownload downloadInfo)
		{
			// First compare the file to the one we have
			bool bUpdate = false;
			string strVersion;
			string strTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				string[] tokens;
				int iHaveVer1, iHaveVer2, iHaveVer3;
				int iCurVer1, iCurVer2, iCurVer3;
				downloadInfo.Verify();
				downloadInfo.SaveMemoryDownloadToFile(strTemp);

				using (StreamReader sr = new StreamReader(Path.Combine(DirectoryPath, VersionFile)))
				{
					strVersion = sr.ReadLine();
					tokens = strVersion.Split('.');
					iHaveVer1 = Convert.ToInt32(tokens[0], CultureInfo.InvariantCulture);
					iHaveVer2 = Convert.ToInt32(tokens[1], CultureInfo.InvariantCulture);
					iHaveVer3 = Convert.ToInt32(tokens[2], CultureInfo.InvariantCulture);
				}

				using (StreamReader sr = new StreamReader(strTemp))
				{
					strVersion = sr.ReadLine();
					tokens = strVersion.Split('.');
					iCurVer1 = Convert.ToInt32(tokens[0], CultureInfo.InvariantCulture);
					iCurVer2 = Convert.ToInt32(tokens[1], CultureInfo.InvariantCulture);
					iCurVer3 = Convert.ToInt32(tokens[2], CultureInfo.InvariantCulture);
				}

				if (iCurVer1 > iHaveVer1 || (iCurVer1 == iHaveVer1 && iCurVer2 > iHaveVer2) ||
					(iCurVer1 == iHaveVer1 && iCurVer2 == iHaveVer2 && iCurVer3 > iHaveVer3))
				{
					this.BeginInvoke(new NotifyUpdateDelegate(NotifyUpdate), new object[] { strVersion });
					bUpdate = true;
				}

				File.Delete(strTemp);
			}
			catch (System.Net.WebException)
			{
			}
			catch
			{
			}
			finally
			{
				if (!bUpdate && m_bUpdateFromMenu)
					this.BeginInvoke(new MethodInvoker(NotifyUpdateNotAvailable));
			}
		}

		private void CheckForUpdates(bool bFromMenu)
		{
			m_bUpdateFromMenu = bFromMenu;
			WebDownload download = new WebDownload(WebsiteUrl + VersionFile);
			download.DownloadType = DownloadType.Unspecified;
			download.CompleteCallback += new DownloadCompleteHandler(UpdateDownloadComplete);
			download.BackgroundDownloadMemory();
		}

		#endregion

		#region Download Progress

		private class ActiveDownload
		{
			internal LayerBuilder builder; // null indicates Blue Marble
			internal bool bOn;
			internal bool bRead;
		}

		bool m_bDownloadUpdating;
		List<ActiveDownload> m_downloadList = new List<ActiveDownload>();
		int m_iTotal;
		int m_iPos;
		bool m_bDownloading;

		private delegate void UpdateDownloadIndicatorsDelegate(bool bDownloading, int iPos, int iTotal, List<ActiveDownload> newList);
		private void UpdateDownloadIndicators(bool bDownloading, int iPos, int iTotal, List<ActiveDownload> newList)
		{
			// --- Version 2.0.0 was getting into this method somehow even after the main form was disposed.
			// --- Check for it and return if we're not in a state to update anything.
			if (IsHandleCreated == false || IsDisposed == true) return;

			// --- This always happens in main thread (but protect it anyway) ---
			if (!m_bDownloadUpdating)
			{
				m_bDownloadUpdating = true;
				m_downloadList = newList;
				m_bDownloading = bDownloading;
				m_iPos = iPos;
				m_iTotal = iTotal;
				if (m_bDownloading)
				{
					// Add new or update information for previous downloads
					for (int i = 0; i < newList.Count; i++)
					{
						int iFound = -1;
						for (int j = 0; j < m_downloadList.Count; j++)
						{
							if (newList[i].builder == m_downloadList[j].builder)
							{
								iFound = j;
								break;
							}
						}
						if (iFound != -1)
						{
							if (m_downloadList[iFound].bRead)
							{
								// This for simple flashing led animation
								newList[i].bOn = !m_downloadList[iFound].bOn;
								newList[i].bRead = false;
							}
						}
					}
					m_downloadList = newList;
				}
				else
					m_downloadList.Clear();

				if (!m_bDownloading)
				{
					if (this.toolStripProgressBar.Visible)
						this.toolStripProgressBar.Value = 100;
					this.toolStripProgressBar.Visible = false;
					this.toolStripStatusLabel1.Visible = false;
					this.toolStripStatusSpin1.Visible = false;
					this.toolStripStatusLabel2.Visible = false;
					this.toolStripStatusSpin2.Visible = false;
					this.toolStripStatusLabel3.Visible = false;
					this.toolStripStatusSpin3.Visible = false;
					this.toolStripStatusLabel4.Visible = false;
					this.toolStripStatusSpin4.Visible = false;
					this.toolStripStatusLabel5.Visible = false;
					this.toolStripStatusSpin5.Visible = false;
					this.toolStripStatusLabel6.Visible = false;
					this.toolStripStatusSpin6.Visible = false;
				}
				else
				{
					this.toolStripProgressBar.Visible = true;
					this.toolStripProgressBar.Value = m_iTotal > 0 ? Math.Max(5, Math.Min(100 * m_iPos / m_iTotal, 100)) : 0;
					this.toolStripProgressBar.ToolTipText = "";// (m_iPos / 1024).ToString() + "KB from an estimated " + (m_iTotal / 1024).ToString() + "KB completed.";
					if (m_downloadList.Count >= 6)
					{
						this.toolStripStatusLabel6.Text = "";
						if (m_downloadList[5].builder == null)
						{
							this.toolStripStatusLabel6.ToolTipText = "Base Image";
							this.toolStripStatusLabel6.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel6.ToolTipText = m_downloadList[5].builder.Title;
							this.toolStripStatusLabel6.Image = s_oImageList.Images[m_downloadList[5].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel6.Visible = true;
						this.toolStripStatusSpin6.Text = "";
						this.toolStripStatusSpin6.Image = m_downloadList[5].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[5].iSpin);
						this.toolStripStatusSpin6.Visible = true;
						m_downloadList[5].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel6.Visible = false;
						this.toolStripStatusSpin6.Visible = false;
					}

					if (m_downloadList.Count >= 5)
					{
						this.toolStripStatusLabel5.Text = "";
						if (m_downloadList[4].builder == null)
						{
							this.toolStripStatusLabel5.ToolTipText = "Base Image";
							this.toolStripStatusLabel5.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel5.ToolTipText = m_downloadList[4].builder.Title;
							this.toolStripStatusLabel5.Image = s_oImageList.Images[m_downloadList[4].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel5.Visible = true;
						this.toolStripStatusSpin5.Text = "";
						this.toolStripStatusSpin5.Image = m_downloadList[4].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[4].iSpin);
						this.toolStripStatusSpin5.Visible = true;
						m_downloadList[4].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel5.Visible = false;
						this.toolStripStatusSpin5.Visible = false;
					}

					if (m_downloadList.Count >= 4)
					{
						this.toolStripStatusLabel4.Text = "";
						if (m_downloadList[3].builder == null)
						{
							this.toolStripStatusLabel4.ToolTipText = "Base Image";
							this.toolStripStatusLabel4.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel4.ToolTipText = m_downloadList[3].builder.Title;
							this.toolStripStatusLabel4.Image = s_oImageList.Images[m_downloadList[3].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel4.Visible = true;
						this.toolStripStatusSpin4.Text = "";
						this.toolStripStatusSpin4.Image = m_downloadList[3].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[3].iSpin);
						this.toolStripStatusSpin4.Visible = true;
						m_downloadList[3].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel4.Visible = false;
						this.toolStripStatusSpin4.Visible = false;
					}

					if (m_downloadList.Count >= 3)
					{
						this.toolStripStatusLabel3.Text = "";
						if (m_downloadList[2].builder == null)
						{
							this.toolStripStatusLabel3.ToolTipText = "Base Image";
							this.toolStripStatusLabel3.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel3.ToolTipText = m_downloadList[2].builder.Title;
							this.toolStripStatusLabel3.Image = s_oImageList.Images[m_downloadList[2].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel3.Visible = true;
						this.toolStripStatusSpin3.Text = "";
						this.toolStripStatusSpin3.Image = m_downloadList[2].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[2].iSpin);
						this.toolStripStatusSpin3.Visible = true;
						m_downloadList[2].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel3.Visible = false;
						this.toolStripStatusSpin3.Visible = false;
					}

					if (m_downloadList.Count >= 2)
					{
						this.toolStripStatusLabel2.Text = "";
						if (m_downloadList[1].builder == null)
						{
							this.toolStripStatusLabel2.ToolTipText = "Base Image";
							this.toolStripStatusLabel2.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel2.ToolTipText = m_downloadList[1].builder.Title;
							this.toolStripStatusLabel2.Image = s_oImageList.Images[m_downloadList[1].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel2.Visible = true;
						this.toolStripStatusSpin2.Text = "";
						this.toolStripStatusSpin2.Image = m_downloadList[1].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[1].iSpin);
						this.toolStripStatusSpin2.Visible = true;
						m_downloadList[1].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel2.Visible = false;
						this.toolStripStatusSpin2.Visible = false;
					}

					if (m_downloadList.Count >= 1)
					{
						this.toolStripStatusLabel1.Text = "";
						if (m_downloadList[0].builder == null)
						{
							this.toolStripStatusLabel1.ToolTipText = "Base Image";
							this.toolStripStatusLabel1.Image = global::Dapple.Properties.Resources.marble_icon.ToBitmap();
						}
						else
						{
							this.toolStripStatusLabel1.ToolTipText = m_downloadList[0].builder.Title;
							this.toolStripStatusLabel1.Image = s_oImageList.Images[m_downloadList[0].builder.ServerTypeIconKey];
						}
						this.toolStripStatusLabel1.Visible = true;
						this.toolStripStatusSpin1.Text = "";
						this.toolStripStatusSpin1.Image = m_downloadList[0].bOn ? global::Dapple.Properties.Resources.led_on : global::Dapple.Properties.Resources.led_off; //GetSpinImage(m_downloadList[0].iSpin);
						this.toolStripStatusSpin1.Visible = true;
						m_downloadList[0].bRead = true;
					}
					else
					{
						this.toolStripStatusLabel1.Visible = false;
						this.toolStripStatusSpin1.Visible = false;
					}
				}
				m_bDownloadUpdating = false;
			}
		}

		#endregion
	
		#region MainApplication Implementation

		/// <summary>
		/// MainApplication's System.Windows.Forms.Form
		/// </summary>
		public override System.Windows.Forms.Form Form
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// MainApplication's globe window
		/// </summary>
		public override WorldWindow WorldWindow
		{
			get
			{
				return c_oWorldWindow;
			}
		}

		/// <summary>
		/// The splash screen dialog.
		/// </summary>
		public override WorldWind.Splash SplashScreen
		{
			get
			{
				return splashScreen;
			}
		}

		#endregion

		#region Metadata displayer

		internal delegate void StringParamDelegate(String szString);
		internal delegate void LoadMetadataDelegate(IBuilder oBuilder);

		private void DisplayMetadataMessage(String szMessage)
		{
			if (InvokeRequired)
				this.Invoke(new StringParamDelegate(DisplayMetadataMessage), new Object[] { szMessage });
			else
			{
				c_wbMetadata.Visible = false;
				c_lMetadata.Text = szMessage;
			}
		}

		private void DisplayMetadataDocument(String strFilename)
		{
			c_wbMetadata.Visible = true;
			Uri metaUri = new Uri(strFilename);
			if (!metaUri.Equals(c_wbMetadata.Url))
			{
				// --- Delete the file we were pointing to before ---
				if (c_wbMetadata.Url != null && c_wbMetadata.Url.Scheme.Equals("file"))
				{
					try
					{
						File.Delete(c_wbMetadata.Url.LocalPath);
					}
					catch (IOException)
					{
						// --- Not a big deal if we can't delete a temp. file, Dapple
						// --- empties the folder each time it starts up anyway
					}
				}
				c_wbMetadata.Url = metaUri;
			}
		}
		
		private void LoadMetadata(IBuilder oBuilder)
		{
			if (InvokeRequired)
				this.Invoke(new LoadMetadataDelegate(LoadMetadata), new Object[] { oBuilder });
			else
			{
				try
				{
					XmlDocument oDoc = new XmlDocument();
					oDoc.AppendChild(oDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
					XmlNode oNode = null;
					string strStyleSheet = null;

					if ((oBuilder is AsyncBuilder) && (oBuilder as AsyncBuilder).LoadingErrorOccurred)
					{
						DisplayMetadataMessage("The selected object failed to load.");
						return;
					}
					else if (oBuilder.SupportsMetaData)
					{
						oNode = oBuilder.GetMetaData(oDoc);
						if (oNode == null)
						{
							DisplayMetadataMessage("You do not have permission to view the metadata for this data layer.");
							return;
						}
						strStyleSheet = oBuilder.StyleSheetName;
						DisplayMetadataMessage("Loading metadata for layer " + oBuilder.Title + "...");
					}
					else if (!String.IsNullOrEmpty(oBuilder.MetadataDisplayMessage))
					{
						DisplayMetadataMessage(oBuilder.MetadataDisplayMessage);
						return;
					}
					else
					{
						DisplayMetadataMessage("Metadata for the selected object is unsupported.");
						return;
					}

					if (oNode is XmlDocument)
					{
						oDoc = oNode as XmlDocument;
					}
					else if (oNode is XmlElement)
					{
						oDoc.AppendChild(oNode);
					}
					if (strStyleSheet != null)
					{
						XmlNode oRef = oDoc.CreateProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + Path.Combine(this.metaviewerDir, strStyleSheet) + "'");
						oDoc.InsertBefore(oRef, oDoc.DocumentElement);
					}

					string filePath = Path.Combine(this.metaviewerDir, Path.GetRandomFileName());
					filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".xml");
					oDoc.Save(filePath);
					DisplayMetadataDocument(filePath);
				}
				catch (Exception e)
				{
					DisplayMetadataMessage("An error occurred while accessing metadata: " + e.Message);
				}
			}
		}

		/// <summary>
		/// Synchronization class which handles displaying the metadata for a layer.  Prevents loading a layer
		/// multiple times, and supresses multiple loads when more than one layer change occurs in a short
		/// time period.
		/// </summary>
		class MetadataDisplayThread
		{
			private Object LOCK = new Object();
			private ManualResetEvent m_oSignaller = new ManualResetEvent(false);
			private List<IBuilder> m_hLayerToLoad = new List<IBuilder>();
			private Thread m_hThread;
			private MainForm m_hOwner;

			internal MetadataDisplayThread(MainForm hOwner)
			{
				m_hOwner = hOwner;

				m_hThread = new Thread(new ThreadStart(ThreadMain));
				m_hThread.IsBackground = true;
				m_hThread.Start();
			}

			internal void AddBuilder(IBuilder oBuilder)
			{
				lock (LOCK)
				{
					m_hLayerToLoad.Add(oBuilder);
					m_oSignaller.Set();
				}
			}

			internal void Abort()
			{
				m_hThread.Abort();
			}

			private void ThreadMain()
			{
				IBuilder oCurrentBuilder = null;
				IBuilder oLastBuilder = null;

				while (true)
				{
					m_oSignaller.WaitOne();

					lock (LOCK)
					{
						oCurrentBuilder = m_hLayerToLoad[m_hLayerToLoad.Count - 1];
						m_hLayerToLoad.Clear();
						m_oSignaller.Reset();
					}

					if (oCurrentBuilder == null)
					{
						m_hOwner.DisplayMetadataMessage("Select a dataset in the server tree or server list to view its associated metadata.");
						oLastBuilder = oCurrentBuilder;
					}
					else
					{
						if (oCurrentBuilder is AsyncBuilder)
							((AsyncBuilder)oCurrentBuilder).WaitUntilLoaded();

						if (!oCurrentBuilder.Equals(oLastBuilder))
						{
							m_hOwner.LoadMetadata(oCurrentBuilder);
							oLastBuilder = oCurrentBuilder;
						}
					}
				}
			}
		}

		#endregion

		#region Menu Item Event Handlers

		private void c_miCheckForUpdates_Click(object sender, EventArgs e)
		{
			CheckForUpdates(true);
		}

		private void c_miOpenImage_Click(object sender, EventArgs e)
		{
			string strLastFolderCfg = Path.Combine(Path.Combine(UserPath, Settings.ConfigPath), "opengeotif.cfg");

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "GeoTIFF Files|*.tif;*.tiff";
			openFileDialog.Title = "Open GeoTIFF File in Current View...";
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			openFileDialog.RestoreDirectory = true;
			if (File.Exists(strLastFolderCfg))
			{
				try
				{
					using (StreamReader sr = new StreamReader(strLastFolderCfg))
					{
						string strDir = sr.ReadLine();
						if (Directory.Exists(strDir))
							openFileDialog.InitialDirectory = strDir;
					}
				}
				catch
				{
				}
			}

			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				AddGeoTiff(openFileDialog.FileName, "", false, true);
				try
				{
					using (StreamWriter sw = new StreamWriter(strLastFolderCfg))
					{
						sw.WriteLine(Path.GetDirectoryName(openFileDialog.FileName));
					}
				}
				catch
				{
				}
			}
		}

		private void c_miAddDAPServer_Click(object sender, EventArgs e)
		{
			AddDAPServer();
		}

		private void c_miAddWMSServer_Click(object sender, EventArgs e)
		{
			AddWMSServer();
		}

		private void c_miAddArcIMSServer_Click(object sender, EventArgs e)
		{
			AddArcIMSServer();
		}

		private void c_miAskLastViewAtStartup_Click(object sender, EventArgs e)
		{
			c_miOpenLastViewAtStartup.Checked = false;
			Settings.AskLastViewAtStartup = c_miAskLastViewAtStartup.Checked;
		}

		private void c_miOpenLastViewAtStartup_Click(object sender, EventArgs e)
		{
			Settings.AskLastViewAtStartup = false;
			this.c_miAskLastViewAtStartup.Checked = false;
			Settings.LastViewAtStartup = c_miOpenLastViewAtStartup.Checked;
		}

		private void c_miAdvancedSettings_Click(object sender, EventArgs e)
		{
			Wizard wiz = new Wizard(Settings);
			wiz.ShowDialog(this);
		}

		/// <summary>
		/// Handler for a change in the selection of the vertical exaggeration from the main menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemVerticalExaggerationChange(object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem item in c_miVertExaggeration.DropDownItems)
			{
				if (item != sender)
				{
					item.Checked = false;
				}
				else
				{
					World.Settings.VerticalExaggeration = Convert.ToSingle(item.Text.Replace("x", string.Empty), CultureInfo.InvariantCulture);
				}
			}
			c_oWorldWindow.Invalidate();
		}

		private void c_miShowCompass_Click(object sender, EventArgs e)
		{
			World.Settings.ShowCompass = c_miShowCompass.Checked;
			this.compassPlugin.Layer.IsOn = World.Settings.ShowCompass;
		}

		private void c_miShowDLProgress_Click(object sender, EventArgs e)
		{
			World.Settings.ShowDownloadIndicator = c_miShowDLProgress.Checked;
		}

		private void c_miShowCrosshair_Click(object sender, EventArgs e)
		{
			World.Settings.ShowCrosshairs = c_miShowCrosshair.Checked;
		}

		private void c_miOpenSavedView_Click(object sender, EventArgs e)
		{
			ViewOpenDialog dlgtest = new ViewOpenDialog(Path.Combine(UserPath, Settings.ConfigPath));
			DialogResult res = dlgtest.ShowDialog(this);
			if (dlgtest.ViewFile != null)
			{
				if (res == DialogResult.OK)
					OpenView(dlgtest.ViewFile, true, true);
			}
		}

		private void c_miOpenHomeView_Click(object sender, EventArgs e)
		{
			CmdLoadHomeView();
		}

		private void c_miShowGridLines_Click(object sender, EventArgs e)
		{
			World.Settings.ShowLatLonLines = c_miShowGridlines.Checked;
			foreach (RenderableObject oRO in c_oWorldWindow.CurrentWorld.RenderableObjects.ChildObjects)
			{
				if (oRO.Name == "1 - Grid Lines")
				{
					oRO.IsOn = c_miShowGridlines.Checked;
					break;
				}
			}
		}

		private void c_miShowInfoOverlay_Click(object sender, EventArgs e)
		{
			World.Settings.ShowPosition = c_miShowInfoOverlay.Checked;
			c_oWorldWindow.Invalidate();
		}

		private void c_miSaveView_Click(object sender, EventArgs e)
		{
			string tempViewFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ViewExt);
			string tempFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ".jpg");
			SaveCurrentView(tempViewFile, tempFile, "");
			Image img = Image.FromFile(tempFile);
			SaveViewForm form = new SaveViewForm(Path.Combine(UserPath, Settings.ConfigPath), img);

			if (form.ShowDialog(this) == DialogResult.OK)
			{
				if (File.Exists(form.OutputPath))
					File.Delete(form.OutputPath);

				XmlDocument oDoc = new XmlDocument();
				oDoc.Load(tempViewFile);
				XmlNode oRoot = oDoc.DocumentElement;
				XmlNode oNode = oDoc.CreateElement("notes");
				oNode.InnerText = form.Notes;
				oRoot.AppendChild(oNode);
				oDoc.Save(form.OutputPath);
			}

			img.Dispose();
			if (File.Exists(tempFile)) File.Delete(tempFile);
			if (File.Exists(tempViewFile)) File.Delete(tempViewFile);
		}

		private void c_miSendViewTo_Click(object sender, EventArgs e)
		{
			string tempBodyFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ".txt");
			string tempJpgFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ".jpg");
			string tempViewFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ViewExt);
			string strMailApp = Path.Combine(Path.Combine(DirectoryPath, "System"), "mailer.exe");

			SaveCurrentView(tempViewFile, tempJpgFile, "");


			using (StreamWriter sw = new StreamWriter(tempBodyFile))
			{
				sw.WriteLine();
				sw.WriteLine();
				sw.WriteLine("Get Dapple to view the attachment: " + WebsiteUrl + ".");
			}

			try
			{
				ProcessStartInfo psi = new ProcessStartInfo(strMailApp);
				psi.UseShellExecute = false;
				psi.CreateNoWindow = true;
				psi.Arguments = String.Format(CultureInfo.InvariantCulture,
					" \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" \"{6}\" \"{7}\"",
					ViewFileDescr, "", "",
					tempViewFile, ViewFileDescr + ViewExt,
					tempJpgFile, ViewFileDescr + ".jpg", tempBodyFile);
				using (Process p = Process.Start(psi))
				{
					// Let the screen draw so it doesn't look damaged.
					Application.DoEvents();
					p.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				Program.ShowMessageBox(
					"An unexpected error occurred sending the view:\n" + ex.Message,
					"Send View To",
					MessageBoxButtons.OK,
					MessageBoxDefaultButton.Button1,
					MessageBoxIcon.Error);
			}

			File.Delete(tempBodyFile);
			File.Delete(tempJpgFile);
			File.Delete(tempViewFile);
		}

		private void c_miShowPlaceNames_Click(object sender, EventArgs e)
		{
			if (this.placeNames == null) return;

			World.Settings.ShowPlacenames = !World.Settings.ShowPlacenames;
			this.c_miShowPlaceNames.Checked = World.Settings.ShowPlacenames;
			this.placeNames.IsOn = World.Settings.ShowPlacenames;
		}

		private void c_miShowScaleBar_Click(object sender, EventArgs e)
		{
			this.scalebarPlugin.IsVisible = !this.scalebarPlugin.IsVisible;
			World.Settings.ShowScaleBar = this.scalebarPlugin.IsVisible;
			this.c_miShowScaleBar.Checked = this.scalebarPlugin.IsVisible;
		}

		private void c_miSunshadingEnabled_Click(object sender, EventArgs e)
		{
			World.Settings.EnableSunShading = true;
			World.Settings.SunSynchedWithTime = false;
			this.c_miSunshadingEnabled.Checked = true;
			this.c_miSunshadingSync.Checked = false;
			this.c_miSunshadingDisabled.Checked = false;
		}

		private void c_miSunshadingSync_Click(object sender, EventArgs e)
		{
			World.Settings.EnableSunShading = true;
			World.Settings.SunSynchedWithTime = true;
			this.c_miSunshadingEnabled.Checked = false;
			this.c_miSunshadingSync.Checked = true;
			this.c_miSunshadingDisabled.Checked = false;
		}

		private void c_miSunshadingDisabled_Click(object sender, EventArgs e)
		{
			World.Settings.EnableSunShading = false;
			World.Settings.SunSynchedWithTime = false;
			this.c_miSunshadingEnabled.Checked = false;
			this.c_miSunshadingSync.Checked = false;
			this.c_miSunshadingDisabled.Checked = true;
		}

		private void c_miShowAtmoScatter_Click(object sender, EventArgs e)
		{
			World.Settings.EnableAtmosphericScattering = !World.Settings.EnableAtmosphericScattering;
			this.c_miShowAtmoScatter.Checked = World.Settings.EnableAtmosphericScattering;
		}

		private void c_miShowGlobalClouds_Click(object sender, EventArgs e)
		{
			World.Settings.ShowClouds = !World.Settings.ShowClouds;
			this.c_miShowGlobalClouds.Checked = World.Settings.ShowClouds;
			this.cloudsPlugin.layer.IsOn = World.Settings.ShowClouds;
		}

		private void c_miExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void c_miView_DropDownOpening(object sender, EventArgs e)
		{
			this.c_miShowScaleBar.Checked = this.scalebarPlugin.IsVisible;
		}

		private void c_miHelpAbout_Click(object sender, EventArgs e)
		{
			AboutDialog dlg = new AboutDialog();
			dlg.ShowDialog(this);
		}

		private void c_miHelpHomepage_Click(object sender, EventArgs e)
		{
			MainForm.BrowseTo(MainForm.WebsiteUrl);
		}

		private void c_miHelpForums_Click(object sender, EventArgs e)
		{
			MainForm.BrowseTo(MainForm.WebsiteForumsHelpUrl);
		}

		private void c_miHelpWebDocs_Click(object sender, EventArgs e)
		{
			MainForm.BrowseTo(MainForm.WebsiteHelpUrl);
		}

		private void c_miAddLayer_Click(object sender, EventArgs e)
		{
			AddDatasetAction();
		}

		private void c_miExtractLayers_Click(object sender, EventArgs e)
		{
			c_oLayerList.CmdExtractVisibleLayers();
		}

		private void c_miSearch_Click(object sender, EventArgs e)
		{
			doSearch();
		}

		private void c_miViewProperties_Click(object sender, EventArgs e)
		{
			m_oModel.SelectedServer.ViewProperties();
		}

		private void c_miTakeSnapshot_Click(object sender, EventArgs e)
		{
			c_oLayerList.CmdTakeSnapshot();
		}

		private void c_miRemoveLayer_Click(object sender, EventArgs e)
		{
			c_oLayerList.CmdRemoveSelectedLayers();
		}

		private void c_miRefreshServer_Click(object sender, EventArgs e)
		{
			m_oModel.SelectedServer.RefreshServer();
		}

		private void c_miRemoveServer_Click(object sender, EventArgs e)
		{
			m_oModel.RemoveServer(m_oModel.SelectedServer, true);
		}

		private void c_miSetHomeView_Click(object sender, EventArgs e)
		{
			CmdSaveHomeView();
		}

		private void c_oDappleSearch_LayerSelectionChanged(object sender, EventArgs e)
		{
			CmdSetupToolsMenu();
		}

		private void c_oLayerList_LayerSelectionChanged(object sender, EventArgs e)
		{
			c_miRemoveLayer.Enabled = c_oLayerList.RemoveAllowed;
		}

		private void c_tcSearchViews_SelectedIndexChanged(object sender, EventArgs e)
		{
			CmdSetupServersMenu();
			CmdSetupToolsMenu();
		}

		private void c_miGetDataHelp_Click(object sender, EventArgs e)
		{
			CmdDisplayOMHelp();
		}

		private void c_miSetFavouriteServer_Click(object sender, EventArgs e)
		{
			m_oModel.SetFavouriteServer(m_oModel.SelectedServer, true);
		}

		private void c_miToggleServerStatus_Click(object sender, EventArgs e)
		{
			m_oModel.ToggleServer(m_oModel.SelectedServer, true);
		}

		private void c_miOpenKeyhole_Click(object sender, EventArgs e)
		{
			String strHistoryFilename = Path.Combine(Path.Combine(UserPath, Settings.ConfigPath), "keyholehistory.cfg");

			String strInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			try
			{
				if (File.Exists(strHistoryFilename))
				{
					String[] oContents = File.ReadAllLines(strHistoryFilename);
					if (Directory.Exists(oContents[0]))
					{
						strInitialDirectory = oContents[0];
					}
				}
			}
			catch (Exception)
			{
				// --- Problem reading last directory, just use My Documents ---
				strInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			OpenFileDialog bob = new OpenFileDialog();
			bob.Filter = "KML/KMZ Files|*.kml;*.kmz";
			bob.FilterIndex = 1;
			bob.InitialDirectory = strInitialDirectory;
			bob.Title = "Select Keyhole File to Open...";
			bob.Multiselect = false;
			bob.RestoreDirectory = true;

			if (bob.ShowDialog() == DialogResult.OK && File.Exists(bob.FileName))
			{
				try
				{
					File.WriteAllText(strHistoryFilename, Path.GetDirectoryName(bob.FileName));
				}
				catch (Exception)
				{
					// --- Problem saving last directory, just ignore it ---
				}

				try
				{
					Dapple.KML.KMLLayerBuilder oBuilder = new Dapple.KML.KMLLayerBuilder(bob.FileName, WorldWindowSingleton, null);
					AddLayerBuilder(oBuilder);
					oBuilder.GoToLookAt(c_oWorldWindow);
				}
				catch (ArgumentException ex)
				{
					Program.ShowMessageBox("An error occurred while loading file '" + Path.GetFileName(bob.FileName) + "':" + Environment.NewLine + Environment.NewLine + ex.Message, "Open KML File", MessageBoxButtons.OK, MessageBoxDefaultButton.Button1, MessageBoxIcon.Error);
				}
			}
		}

		private void c_miServers_DropDownOpening(object sender, EventArgs e)
		{
			CmdSetupServersMenu();
		}

		private void c_miTools_DropDownOpening(object sender, EventArgs e)
		{
			CmdSetupToolsMenu();
		}

		#endregion

		#region MainForm Event Handlers

		private void MainForm_Shown(object sender, EventArgs e)
		{
			if (IsRunningAsDapClient)
			{
				try
				{
					s_oMontajRemoteInterface.StartConnection();
				}
				catch (System.Runtime.Remoting.RemotingException)
				{
					c_oLayerList.OMFeaturesEnabled = false;
					c_miGetDatahelp.Enabled = false;
					c_miExtractLayers.Enabled = false;

					Program.ShowMessageBox(
						"Dapple has experienced an error attempting to connect to " + EnumUtils.GetDescription(s_eClientType) + ". Restarting the application or your computer may fix this problem.",
						"Dapple Startup",
						MessageBoxButtons.OK,
						MessageBoxDefaultButton.Button1,
						MessageBoxIcon.Error);
				}
			}

			// Render once to not just show the atmosphere at startup (looks better) ---
			c_oWorldWindow.SafeRender();


			try
			{
				// --- Draw the screen, so it doesn't look damaged ---
				UseWaitCursor = true;
				Application.DoEvents();

				if (this.openView.Length > 0)
					OpenView(this.openView, m_strOpenGeoTiffFile.Length == 0 && m_strOpenKMLFile.Length == 0, true);
				else if (!IsRunningAsDapClient && File.Exists(Path.Combine(Path.Combine(UserPath, Settings.ConfigPath), LastView)))
				{
					if (Settings.AskLastViewAtStartup)
					{
						Utils.MessageBoxExLib.MessageBoxEx msgBox = Utils.MessageBoxExLib.MessageBoxExManager.CreateMessageBox(null);
						msgBox.AllowSaveResponse = true;
						msgBox.SaveResponseText = "Don't ask me again";
						msgBox.Caption = this.Text;
						msgBox.Icon = Utils.MessageBoxExLib.MessageBoxExIcon.Question;
						msgBox.AddButtons(MessageBoxButtons.YesNo);
						msgBox.Text = "Would you like to open your last View?";
						msgBox.Font = this.Font;
						Settings.LastViewAtStartup = msgBox.Show(this) == Utils.MessageBoxExLib.MessageBoxExResult.Yes;
						if (msgBox.SaveResponse)
							Settings.AskLastViewAtStartup = false;
					}

					if (Settings.LastViewAtStartup)
						OpenView(Path.Combine(Path.Combine(UserPath, Settings.ConfigPath), LastView), true, true);
					else
						CmdLoadHomeView();
				}
				else
				{
					CmdLoadHomeView();
				}

				if (m_blSelectPersonalDAP && m_oModel.PersonalDapServer != null)
				{
					m_oModel.SelectedNode = m_oModel.PersonalDapServer;
				}

				if (this.m_strOpenGeoTiffFile.Length > 0)
					AddGeoTiff(this.m_strOpenGeoTiffFile, this.m_strOpenGeoTiffName, this.m_blOpenGeoTiffTmp, true);
				if (this.m_strOpenKMLFile.Length > 0)
					AddKML(m_strOpenKMLFile, m_strOpenKMLName, m_blOpenKMLTmp, true, s_oOMMapExtentWGS84);


				// Check for updates daily
				if (IsRunningAsDapClient == false && Settings.UpdateCheckDate.Date != System.DateTime.Now.Date)
					CheckForUpdates(false);
				Settings.UpdateCheckDate = System.DateTime.Now;

				foreach (RenderableObject oRO in c_oWorldWindow.CurrentWorld.RenderableObjects.ChildObjects)
				{
					if (oRO.Name == "1 - Grid Lines")
					{
						oRO.IsOn = World.Settings.ShowLatLonLines;
						break;
					}
				}

				if (s_oOMMapExtentWGS84 != null)
				{
					doSearch(String.Empty, s_oOMMapExtentWGS84);
					GoTo(s_oOMMapExtentWGS84, false);
				}

				c_oOverview.StartRenderTimer();
			}
			finally
			{
				UseWaitCursor = false;
			}
		}

		bool m_bSizing = false;
		private void MainForm_ResizeBegin(object sender, EventArgs e)
		{
			m_bSizing = true;
			c_oWorldWindow.Visible = false;
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			m_bSizing = false;
			c_oWorldWindow.Visible = true;
			c_oWorldWindow.SafeRender();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			c_oWorldWindow.Visible = false;
		}

		private void MainForm_SizeChanged(object sender, EventArgs e)
		{
			if (!m_bSizing)
			{
				c_oWorldWindow.Visible = true;
				c_oWorldWindow.SafeRender();
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

			c_oWorldWindow.IsRenderDisabled = false;

			this.toolStripProgressBar.Visible = false;
			this.toolStripStatusLabel1.Visible = false;
			this.toolStripStatusLabel1.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel2.Visible = false;
			this.toolStripStatusLabel2.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel3.Visible = false;
			this.toolStripStatusLabel3.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel4.Visible = false;
			this.toolStripStatusLabel4.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel5.Visible = false;
			this.toolStripStatusLabel5.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel6.Visible = false;
			this.toolStripStatusLabel6.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin1.Visible = false;
			this.toolStripStatusSpin1.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin2.Visible = false;
			this.toolStripStatusSpin2.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin3.Visible = false;
			this.toolStripStatusSpin3.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin4.Visible = false;
			this.toolStripStatusSpin4.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin5.Visible = false;
			this.toolStripStatusSpin5.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusSpin6.Visible = false;
			this.toolStripStatusSpin6.Alignment = ToolStripItemAlignment.Right;
			c_oWorldWindow.Updated += c_oWorldWindow_Updated;
		}

		private void MainForm_Closing(object sender, CancelEventArgs e)
		{
			Program.FocusOnCaller();

			// Turn off the metadata display thread and background search thread
			m_oMetadataDisplay.Abort();

			this.threeDConnPlugin.Unload();

			// Turning off the layers will set this
			bool bSaveGridLineState = World.Settings.ShowLatLonLines;

			this.WindowState = FormWindowState.Minimized;

			SaveLastView();
			//tvServers.SaveFavoritesList(Path.Combine(Path.Combine(UserPath, "Config"), "user.dapple_serverlist"));

			World.Settings.ShowLatLonLines = bSaveGridLineState;

			// Register the cache location to make it easy for uninstall to clear the cache for at least the current user
			try
			{
				RegistryKey keySW = Registry.CurrentUser.CreateSubKey("Software");
				RegistryKey keyDapple = keySW.CreateSubKey("Dapple");
				keyDapple.SetValue("CachePathForUninstall", Settings.CachePath);
			}
			catch
			{
			}

			FinalizeSettings();

			// Don't force-dispose the WorldWindow (or, really, anything), just let .NET free it up for us.
			// Should kill the background worker thread though.
			c_oWorldWindow.KillWorkerThread();

			// --- Delete all the temporary XML files the metadata viewer has been pumping out ---

			foreach (String bob in Directory.GetFiles(metaviewerDir, "*.xml"))
			{
				try
				{
					File.Delete(bob);
				}
				catch (System.IO.IOException)
				{
					// Couldn't delete a temp file?  Not the end of the world.
				}
			}

			SaveMRUList();
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			c_oWorldWindow.IsRenderDisabled = true;
		}

		private void MainForm_Activated(object sender, EventArgs e)
		{
			c_oWorldWindow.IsRenderDisabled = false;
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (m.Msg == OpenViewMessage)
			{
				try
				{
					Segment s = new Segment("Dapple.OpenView", SharedMemoryCreationFlag.Attach, 0);

					string[] strData = (string[])s.GetData();

					string strView = strData[0];
					string strGeoTiff = strData[1];
					string strGeoTiffName = strData[2];
					bool bGeotiffTmp = strData[3] == "YES";
					this.lastView = strData[4];
					bool bKmlTmp = strData[5] == "YES";
					string strKmlName = strData[6];
					string strKmlFile = strData[7];

					if (strView.Length > 0)
						OpenView(strView, strGeoTiff.Length == 0, true);
					if (strGeoTiff.Length > 0)
						AddGeoTiff(strGeoTiff, strGeoTiffName, bGeotiffTmp, true);
					if (strKmlName.Length > 0 && strKmlFile.Length > 0)
						AddKML(strKmlFile, strKmlName, bKmlTmp, false, null);
				}
				catch
				{
				}
			}
			base.WndProc(ref m);
		}

		#endregion

		#region World Window Event Handlers

		private void c_oWorldWindow_MouseLeave(object sender, EventArgs e)
		{
			c_scMain.Panel1.Select();
		}

		private void c_oWorldWindow_MouseEnter(object sender, EventArgs e)
		{
			c_oWorldWindow.Select();
		}

		private void c_oWorldWindow_Updated(object sender, EventArgs e)
		{
			int iBuilderPos, iBuilderTotal;
			// Do the work in the update thread and just invoke to update the GUI


			int iPos = 0;
			int iTotal = 0;
			bool bDownloading = false;
			List<ActiveDownload> currentList = new List<ActiveDownload>();
			RenderableObject roBMNG = GetActiveBMNG();

			if (roBMNG != null && roBMNG.IsOn && ((QuadTileSet)((RenderableObjectList)roBMNG).ChildObjects[1]).bIsDownloading(out iBuilderPos, out iBuilderTotal))
			{
				bDownloading = true;
				iPos += iBuilderPos;
				iTotal += iBuilderTotal;
				ActiveDownload dl = new ActiveDownload();
				dl.builder = null;
				dl.bOn = true;
				dl.bRead = false;
				currentList.Add(dl);
			}

			foreach (LayerBuilder oBuilder in c_oLayerList.AllLayers)
			{
				if (oBuilder.bIsDownloading(out iBuilderPos, out iBuilderTotal))
				{
					bDownloading = true;
					iPos += iBuilderPos;
					iTotal += iBuilderTotal;
					ActiveDownload dl = new ActiveDownload();
					dl.builder = oBuilder;
					dl.bOn = true;
					dl.bRead = false;
					currentList.Add(dl);
				}
			}

			// In rare cases, the WorldWindow's background worker thread might start sending back updates before the
			// MainForm's window handle has been created.  Don't let it, or you'll cascade the system into failure.
			if (this.IsHandleCreated)
			{
				this.BeginInvoke(new UpdateDownloadIndicatorsDelegate(UpdateDownloadIndicators), new object[] { bDownloading, iPos, iTotal, currentList });
			}
		}

		private void c_oWorldWindow_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(List<LayerBuilder>)))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void c_oWorldWindow_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(List<LayerBuilder>)))
			{
				List<LayerBuilder> oDropData = e.Data.GetData(typeof(List<LayerBuilder>)) as List<LayerBuilder>;
				c_oLayerList.AddLayers(oDropData);
			}
		}

		void c_oWorldWindow_CameraChanged(object sender, EventArgs e)
		{
			if (m_oLastSearchROI != null)
			{
				SetSearchable(!GeographicBoundingBox.FromQuad(c_oWorldWindow.CurrentAreaOfInterest).Equals(m_oLastSearchROI));
			}
		}

		void c_oWorldWindow_Resize(object sender, EventArgs e)
		{
			c_oOverview.Invalidate();
		}

		#endregion

		#region Other Event Handlers

		#region Nav Strip Buttons

		enum NavMode
		{
			None,
			ZoomIn,
			ZoomOut,
			RotateLeft,
			RotateRight,
			TiltUp,
			TiltDown
		}

		private NavMode eNavMode = NavMode.None;
		private bool bNavTimer = false;

		private void c_bResetTilt_Click(object sender, EventArgs e)
		{
			c_oWorldWindow.DrawArgs.WorldCamera.SetPosition(
					 c_oWorldWindow.Latitude,
					 c_oWorldWindow.Longitude,
					  c_oWorldWindow.DrawArgs.WorldCamera.Heading.Degrees,
					  c_oWorldWindow.DrawArgs.WorldCamera.Altitude,
					  0);

		}

		private void c_bResetRotation_Click(object sender, EventArgs e)
		{
			c_oWorldWindow.DrawArgs.WorldCamera.SetPosition(
					 c_oWorldWindow.Latitude,
					 c_oWorldWindow.Longitude,
					  0,
					  c_oWorldWindow.DrawArgs.WorldCamera.Altitude,
					  c_oWorldWindow.DrawArgs.WorldCamera.Tilt.Degrees);
		}

		private void c_bResetCamera_Click(object sender, EventArgs e)
		{
			c_oWorldWindow.DrawArgs.WorldCamera.Reset();
		}

		private void timerNavigation_Tick(object sender, EventArgs e)
		{
			this.bNavTimer = true;
			switch (this.eNavMode)
			{
				case NavMode.ZoomIn:
					c_oWorldWindow.DrawArgs.WorldCamera.Zoom(0.2f);
					return;
				case NavMode.ZoomOut:
					c_oWorldWindow.DrawArgs.WorldCamera.Zoom(-0.2f);
					return;
				case NavMode.RotateLeft:
					Angle rotateClockwise = Angle.FromRadians(-0.01f);
					c_oWorldWindow.DrawArgs.WorldCamera.Heading += rotateClockwise;
					c_oWorldWindow.DrawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateClockwise);
					return;
				case NavMode.RotateRight:
					Angle rotateCounterclockwise = Angle.FromRadians(0.01f);
					c_oWorldWindow.DrawArgs.WorldCamera.Heading += rotateCounterclockwise;
					c_oWorldWindow.DrawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateCounterclockwise);
					return;
				case NavMode.TiltUp:
					c_oWorldWindow.DrawArgs.WorldCamera.Tilt += Angle.FromDegrees(-1.0f);
					return;
				case NavMode.TiltDown:
					c_oWorldWindow.DrawArgs.WorldCamera.Tilt += Angle.FromDegrees(1.0f);
					return;
				default:
					return;
			}
		}

		private void toolStripNavButton_MouseRemoveCapture(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.timerNavigation.Enabled = false;
			this.eNavMode = NavMode.None;
		}

		private void c_bRotateRight_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.RotateLeft;
		}

		private void c_bRotateLeft_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.RotateRight;
		}

		private void c_bTiltDown_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.TiltDown;
		}

		private void c_bTiltUp_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.TiltUp;
		}

		private void c_bZoomOut_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.ZoomOut;
		}

		private void c_bZoomIn_MouseDown(object sender, MouseEventArgs e)
		{
			this.timerNavigation.Enabled = true;
			this.bNavTimer = false;
			this.eNavMode = NavMode.ZoomIn;
		}

		private void c_bZoomIn_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				c_oWorldWindow.DrawArgs.WorldCamera.Zoom(2.0f);
			}
			else
				this.bNavTimer = false;
		}

		private void c_bZoomOut_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				c_oWorldWindow.DrawArgs.WorldCamera.Zoom(-2.0f);
			}
			else
				this.bNavTimer = false;
		}

		private void c_bRotateRight_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				Angle rotateClockwise = Angle.FromRadians(-0.2f);
				c_oWorldWindow.DrawArgs.WorldCamera.Heading += rotateClockwise;
				c_oWorldWindow.DrawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateClockwise);
			}
			else
				this.bNavTimer = false;
		}

		private void c_bRotateLeft_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				Angle rotateCounterclockwise = Angle.FromRadians(0.2f);
				c_oWorldWindow.DrawArgs.WorldCamera.Heading += rotateCounterclockwise;
				c_oWorldWindow.DrawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateCounterclockwise);
			}
			else
				this.bNavTimer = false;
		}

		private void c_bTiltUp_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				c_oWorldWindow.DrawArgs.WorldCamera.Tilt += Angle.FromDegrees(-10.0f);
			}
			else
				this.bNavTimer = false;
		}

		private void c_bTiltDown_Click(object sender, EventArgs e)
		{
			this.timerNavigation.Enabled = false;
			if (!this.bNavTimer)
			{
				c_oWorldWindow.DrawArgs.WorldCamera.Tilt += Angle.FromDegrees(10.0f);
			}
			else
				this.bNavTimer = false;
		}

		#endregion

		private void ServerPageChanged(int iIndex)
		{
			CmdSetupToolsMenu();
			CmdSetupServersMenu();
		}

		private void c_scWorldMetadata_Panel1_Resize(object sender, EventArgs e)
		{
			CenterNavigationToolStrip();
		}

		private void c_oOverview_AOISelected(object sender, GeographicBoundingBox bounds)
		{
			GoTo(bounds, false);
		}

		private void c_tbSearchKeywords_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar.Equals((char)13))
			{
				doSearch();
			}
		}

		private void c_bSearch_Click(object sender, EventArgs e)
		{
			doSearch();
		}

		private bool m_blSupressSearchSelectedIndexChanged = false;
		private void c_tbSearchKeywords_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_blSupressSearchSelectedIndexChanged) return;
			doSearch();
		}

		#region Globe visibility disabling

		private void c_scMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			c_oWorldWindow.Visible = false;
		}

		private void c_scMain_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!m_bSizing)
			{
				c_oWorldWindow.Visible = true;
				c_oWorldWindow.SafeRender();
			}
		}

		private void c_scWorldMetadata_SplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			c_oWorldWindow.Visible = false;
		}

		private void c_scWorldMetadata_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!m_bSizing)
			{
				c_oWorldWindow.Visible = true;
				c_oWorldWindow.SafeRender();
			}
		}

		#endregion

		private void c_oServerList_LayerSelectionChanged(object sender, EventArgs e)
		{
			CmdSetupToolsMenu();
		}

		private void c_tbSearchKeywords_Enter(object sender, EventArgs e)
		{
			if (c_tbSearchKeywords.Text.Equals(NO_SEARCH))
			{
				c_tbSearchKeywords.Text = String.Empty;
			}
			c_tbSearchKeywords.ForeColor = SystemColors.ControlText;
		}

		private void c_tbSearchKeywords_Leave(object sender, EventArgs e)
		{
			if (c_tbSearchKeywords.Text.Equals(String.Empty))
			{
				c_tbSearchKeywords.Text = NO_SEARCH;
				c_tbSearchKeywords.ForeColor = SystemColors.GrayText;
			}
		}

		private void c_bClearSearch_Click(object sender, EventArgs e)
		{
			clearSearch();
		}

		private void c_tbSearchKeywords_TextUpdate(object sender, EventArgs e)
		{
			if (!SearchKeyword.Equals(m_szLastSearchString))
			{
				SetSearchable(true);
			}
		}

		#endregion

		#region Commands

		#region ImageList Queries

		/// <summary>
		/// Returns imagelist index from key name
		/// </summary>
		/// <param name="strKey"></param>
		/// <returns></returns>
		internal static int ImageListIndex(string strKey)
		{
			return s_oImageList.Images.IndexOfKey(strKey);
		}

		/// <summary>
		/// Returns imagelist index from dap dataset type
		/// </summary>
		/// <param name="strType"></param>
		/// <returns></returns>
		internal static int ImageIndex(string strType)
		{
			switch (strType.ToLower())
			{
				case "database":
					return ImageListIndex(DapDatabaseIconKey);
				case "document":
					return ImageListIndex(DapDocumentIconKey);
				case "generic":
					return ImageListIndex(DapMapIconKey);
				case "grid":
					return ImageListIndex(DapGridIconKey);
				case "gridsection":
					return ImageListIndex(DapGridIconKey);
				case "map":
					return ImageListIndex(DapMapIconKey);
				case "picture":
					return ImageListIndex(DapPictureIconKey);
				case "picturesection":
					return ImageListIndex(DapPictureIconKey);
				case "point":
					return ImageListIndex(DapPointIconKey);
				case "spf":
					return ImageListIndex(DapSpfIconKey);
				case "voxel":
					return ImageListIndex(DapVoxelIconKey);
				case "imageserver":
					return ImageListIndex(ArcImsIconKey);
				case "arcgis":
					return ImageListIndex(DapArcGisIconKey);
				default:
					return ImageListIndex(DapIconKey);
			}
		}

		#endregion

		#region AOIs

		private void loadCountryList()
		{
			if (m_oCountryAOIs == null)
			{
				m_oCountryAOIs = new Dictionary<string, GeographicBoundingBox>();
				String[] straCountries = Resources.aoi_region.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				for (int count = 1; count < straCountries.Length - 1; count++)
				{
					String[] data = straCountries[count].Split(new char[] { ',' });
					double minX, minY, maxX, maxY;
					if (!Double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out minX)) continue;
					if (!Double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out minY)) continue;
					if (!Double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out maxX)) continue;
					if (!Double.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out maxY)) continue;

					m_oCountryAOIs.Add(data[0], new GeographicBoundingBox(maxY, minY, minX, maxX));
				}
			}
		}

		private void populateAoiComboBox()
		{
			// --- Create list of AoIs to assign to the drop-down list ---

			List<KeyValuePair<String, GeographicBoundingBox>> oAois = new List<KeyValuePair<string, GeographicBoundingBox>>();


			// --- Get the currently active DAP server if there is one ---

			Server oSelectedDapServer = null;
			if (m_oModel.SelectedServer != null && m_oModel.SelectedServer is DapServerModelNode)
			{
				oSelectedDapServer = (m_oModel.SelectedServer as DapServerModelNode).Server;
			}
			bool blViableDapServer = oSelectedDapServer != null && oSelectedDapServer.Enabled && oSelectedDapServer.Status == Server.ServerStatus.OnLine;


			// --- Add header line ---

			oAois.Add(new KeyValuePair<String, GeographicBoundingBox>("--- Select a specific region ---", null));


			// --- Add server extent if available ---

			if (blViableDapServer)
			{
				oAois.Add(new KeyValuePair<String, GeographicBoundingBox>("Server extent", new GeographicBoundingBox(oSelectedDapServer.ServerExtents.MaxY, oSelectedDapServer.ServerExtents.MinY, oSelectedDapServer.ServerExtents.MinX, oSelectedDapServer.ServerExtents.MaxX)));
			}


			// --- If we're FDWD, add the original map extents ---

			if (IsRunningAsDapClient && s_oOMMapExtentWGS84 != null)
			{
				oAois.Add(new KeyValuePair<String, GeographicBoundingBox>("Original map extent", s_oOMMapExtentWGS84));
			}


			// --- Add divider ---

			oAois.Add(new KeyValuePair<String, GeographicBoundingBox>("-----------------------------", null));


			// --- Add list of AoIs:
			// --- if we've selected an enabled, online DAP server, get its AoI list
			// --- otherwise, add the countries of the world AoI list

			if (blViableDapServer)
			{
				ArrayList aAOIs = oSelectedDapServer.ServerConfiguration.GetAreaList();
				foreach (String strAOI in aAOIs)
				{
					double minX, minY, maxX, maxY;
					String strCoord;
					oSelectedDapServer.ServerConfiguration.GetBoundingBox(strAOI, out maxX, out maxY, out minX, out minY, out strCoord);
					if (strCoord.Equals("WGS 84"))
					{
						GeographicBoundingBox oBox = new GeographicBoundingBox(maxY, minY, minX, maxX);
						oAois.Add(new KeyValuePair<String, GeographicBoundingBox>(strAOI, oBox));
					}
				}
			}
			else
			{
				foreach (KeyValuePair<String, GeographicBoundingBox> country in m_oCountryAOIs)
				{
					oAois.Add(country);
				}
			}

			c_oOverview.SetAOIList(oAois);
		}

		#endregion

		#region Placename Loading

		private void LoadPlacenames(Object oParams)
		{
			this.placeNames = ConfigurationLoader.getRenderableFromLayerFile(Path.Combine(CurrentSettingsDirectory, "^Placenames.xml"), this.WorldWindow.CurrentWorld, this.WorldWindow.Cache, true, null);
			try
			{
				if (!this.IsDisposed)
					Invoke(new MethodInvoker(LoadPlacenamesCallback));
			}
			catch (ObjectDisposedException)
			{
				// --- The user closed the form before the placenames were loaded.  Ignore, since we're shutting down anyway. ---
			}
		}

		private void LoadPlacenamesCallback()
		{
			if (this.placeNames != null)
			{
				this.placeNames.IsOn = World.Settings.ShowPlacenames;
				this.placeNames.RenderPriority = RenderPriority.Placenames;
				c_oWorldWindow.CurrentWorld.RenderableObjects.Add(this.placeNames);
				this.c_miShowPlaceNames.Enabled = true;
			}
			this.c_miShowPlaceNames.Checked = World.Settings.ShowPlacenames;
		}

		#endregion

		#region Open/Save View Methods

		private void SaveLastView()
		{
			// --- Don't save views when we're running inside OM ---
			if (IsRunningAsDapClient) return;

			string tempFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ".jpg");
			if (this.lastView.Length == 0)
				SaveCurrentView(Path.Combine(UserPath, Path.Combine(Settings.ConfigPath, LastView)), tempFile, string.Empty);
			else
				SaveCurrentView(this.lastView, tempFile, string.Empty);
			File.Delete(tempFile);
		}

		/// <summary>
		/// Saves the current view to an xml file, 
		/// this requires that worldWindow was created in the same thread as the caller
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="notes"></param>
		private void SaveCurrentView(string fileName, string picFileName, string notes)
		{
			DappleView view = new DappleView();

			// blue marble
			RenderableObject roBMNG = GetBMNG();
			if (roBMNG != null)
				view.View.Addshowbluemarble(new SchemaBoolean(roBMNG.IsOn));

			WorldWind.Camera.MomentumCamera camera = c_oWorldWindow.DrawArgs.WorldCamera as WorldWind.Camera.MomentumCamera;

			//stop the camera
			camera.SetPosition(camera.Latitude.Degrees, camera.Longitude.Degrees, camera.Heading.Degrees, camera.Altitude, camera.Tilt.Degrees);

			//store the servers
			m_oModel.SaveToView(view);

			// store the current layers
			if (c_oLayerList.AllLayers.Count > 0)
			{
				activelayersType lyrs = view.View.Newactivelayers();
				foreach (LayerBuilder container in c_oLayerList.AllLayers)
				{
					if (!container.Temporary)
					{
						datasetType dataset = lyrs.Newdataset();
						dataset.Addname(new SchemaString(container.Title));
						opacityType op = dataset.Newopacity();
						op.Value = container.Opacity;
						dataset.Addopacity(op);
						dataset.Adduri(new SchemaString(container.GetURI()));
						dataset.Addinvisible(new SchemaBoolean(!container.Visible));
						lyrs.Adddataset(dataset);
					}
				}
				view.View.Addactivelayers(lyrs);
			}

			// store the camera information
			cameraorientationType cameraorient = view.View.Newcameraorientation();
			cameraorient.Addlat(new SchemaDouble(camera.Latitude.Degrees));
			cameraorient.Addlon(new SchemaDouble(camera.Longitude.Degrees));
			cameraorient.Addaltitude(new SchemaDouble(camera.Altitude));
			cameraorient.Addheading(new SchemaDouble(camera.Heading.Degrees));
			cameraorient.Addtilt(new SchemaDouble(camera.Tilt.Degrees));
			view.View.Addcameraorientation(cameraorient);

			if (notes.Length > 0)
				view.View.Addnotes(new SchemaString(notes));

			// Save screen capture (The regular WorldWind method crashes some systems, use interop)
			//this.worldWindow.SaveScreenshot(picFileName);
			//this.worldWindow.Render();

			using (Image img = TakeSnapshot(c_oWorldWindow.Handle))
				img.Save(picFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

			FileStream fs = new FileStream(picFileName, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			byte[] buffer = new byte[fs.Length];
			br.Read(buffer, 0, Convert.ToInt32(fs.Length));
			br.Close();
			fs.Close();
			view.View.Addpreview(new SchemaBase64Binary(buffer));

			view.Save(fileName);
		}

		private static Image TakeSnapshot(IntPtr handle)
		{
			Bitmap bmp;
			RECT tempRect;
			GetWindowRect(handle, out tempRect);
			Rectangle windowRect = tempRect;
			IntPtr formDC = GetDCEx(handle, IntPtr.Zero, DCX_CACHE | DCX_WINDOW);

			using (Graphics grfx = Graphics.FromHdc(formDC))
				bmp = new Bitmap(windowRect.Width, windowRect.Height, grfx);

			using (Graphics grfx = Graphics.FromImage(bmp))
			{
				IntPtr bmpDC = grfx.GetHdc();

				BitBlt(bmpDC, 0, 0, bmp.Width, bmp.Height, formDC, 0, 0, CAPTUREBLT |
				SRCCOPY);
				grfx.ReleaseHdc(bmpDC);
				ReleaseDC(handle, formDC);
			}
			return bmp;
		}
		private bool OpenView(string filename, bool bGoto, bool bLoadLayers)
		{
			bool bOldView = false;

			if (File.Exists(filename))
			{
				DappleView view = new DappleView(filename);
				
				if (bGoto && view.View.Hascameraorientation())
				{
					cameraorientationType orient = view.View.cameraorientation;
					c_oWorldWindow.DrawArgs.WorldCamera.SetPosition(orient.lat.Value, orient.lon.Value, orient.heading.Value, orient.altitude.Value, orient.tilt.Value);
				}

				m_oModel.LoadFromView(view);
				if (bLoadLayers && view.View.Hasactivelayers())
				{
					c_oLayerList.CmdLoadFromView(view, m_oModel);
				}
			}

			if (bOldView)
				MessageBox.Show(this, "The view " + filename + " contained some layers from an earlier version\nwhich could not be retrieved. We apologize for the inconvenience.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return true;
		}

		#endregion

		#region Add Layers

		private void AddGeoTiff(string strGeoTiff, string strGeoTiffName, bool bTmp, bool bGoto)
		{
			LayerBuilder builder = new GeorefImageLayerBuilder(strGeoTiffName, strGeoTiff, bTmp, c_oWorldWindow, null);

			Cursor = Cursors.WaitCursor;
			if (builder.GetLayer() != null)
			{
				Cursor = Cursors.Default;

				// If the file is already there remove it 
				c_oLayerList.CmdRemoveGeoTiff(strGeoTiff);

				// If there is already a layer by that name find unique name
				if (strGeoTiffName.Length > 0)
				{
					int iCount = 0;
					string strNewName = strGeoTiffName;
					bool bExist = true;
					while (bExist)
					{
						bExist = false;
						foreach (LayerBuilder container in c_oLayerList.AllLayers)
						{
							if (container.Title == strNewName)
							{
								bExist = true;
								break;
							}
						}

						if (bExist)
						{
							iCount++;
							strNewName = strGeoTiffName + "_" + iCount.ToString(CultureInfo.InvariantCulture);
						}
					}
					strGeoTiffName = strNewName;
				}

				if (bTmp) builder.Opacity = 128;
				else builder.Opacity = 255;
				builder.Visible = true;
				builder.Temporary = bTmp;

				c_oLayerList.AddLayer(builder);

				if (bGoto)
					GoTo(builder, false);
			}
			else
			{
				Cursor = Cursors.Default;
				string strMessage = "Error adding the file: '" + strGeoTiff + "'.\nOnly WGS 84 geographic images can be displayed at this time.";
				string strGeoInfo = GeorefImageLayerBuilder.GetGeorefInfoFromGeotif(strGeoTiff);
				if (strGeoInfo.Length > 0)
				{
					strMessage += "\nThis image is:\n\n" + strGeoInfo;
				}

				Program.ShowMessageBox(
					strMessage,
					"Open GeoTIFF Image",
					MessageBoxButtons.OK,
					MessageBoxDefaultButton.Button1,
					MessageBoxIcon.Error);
			}
		}

		private void AddKML(String strKMLFile, String strKMLName, bool blTemporary, bool blGoTo, GeographicBoundingBox oBounds)
		{
			try
			{
				this.UseWaitCursor = true;

				Dapple.KML.KMLLayerBuilder oBuilder = new Dapple.KML.KMLLayerBuilder(strKMLFile, strKMLName, WorldWindowSingleton, null, oBounds);
				oBuilder.Temporary = blTemporary;
				c_oLayerList.AddLayer(oBuilder);
				oBuilder.GoToLookAt(WorldWindowSingleton);
			}
			catch (Exception ex)
			{
				Program.ShowMessageBox(
					"An error occurred while trying to add Keyhole file " + strKMLFile + ":" + Environment.NewLine + Environment.NewLine + ex.Message.ToString(),
					"Open KML File",
					MessageBoxButtons.OK,
					MessageBoxDefaultButton.Button1,
					MessageBoxIcon.Error
					);
			}
			finally
			{
				this.UseWaitCursor = false;
			}
		}

		internal void AddLayerBuilder(LayerBuilder oLayer)
		{
			c_oLayerList.AddLayer(oLayer);

			SaveLastView();
		}

		#endregion

		#region Go To

		void GoTo(LayerBuilder builder)
		{
			GoTo(builder.Extents, false);
		}

		void GoTo(LayerBuilder builder, bool blImmediate)
		{
			GoTo(builder.Extents, blImmediate);
		}

		void GoTo(GeographicBoundingBox extents, bool blImmediate)
		{
			c_oWorldWindow.GoToBoundingBox(extents, blImmediate);
		}

		#endregion

		#region Add Servers

		internal void AddDAPServer()
		{
			m_oModel.AddDAPServer();
		}

		internal void AddWMSServer()
		{
			m_oModel.AddWMSServer();
		}

		internal void AddArcIMSServer()
		{
			m_oModel.AddArcIMSServer();
		}

		#endregion

		/// <summary>
		/// Try to open url in web browser
		/// </summary>
		/// <param name="url">The url to open in browser</param>
		internal static void BrowseTo(string url)
		{
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch
			{
				try
				{
					System.Diagnostics.Process.Start("IExplore.exe", url);
				}
				catch
				{
				}
			}
		}

		private void AddDatasetAction()
		{
			if (c_tcSearchViews.SelectedIndex == 0)
			{
				if (cServerViewsTab.SelectedIndex == 0)
				{
					m_oModel.ViewedDatasets.Add(m_oModel.SelectedNode as LayerModelNode);
				}
				else if (cServerViewsTab.SelectedIndex == 1)
				{
					c_oLayerList.AddLayers(c_oServerList.SelectedLayers);
				}
			}
			else if (c_tcSearchViews.SelectedIndex == 1)
			{
				c_oDappleSearch.CmdAddSelected();
			}
		}

		internal void CmdSaveHomeView()
		{
			SaveCurrentView(HomeView.FullPath, Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), ".jpg"), String.Empty);
		}

		internal void CmdLoadHomeView()
		{
			OpenView(HomeView.FullPath, true, true);
		}

		private void CmdDisplayOMHelp()
		{
			try
			{
				MontajInterface.DisplayHelp();
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
				Program.ShowMessageBox(
					"Connection to " + Utility.EnumUtils.GetDescription(MainForm.Client) + " lost, unable to display help.",
					"Extract Layers",
					MessageBoxButtons.OK,
					MessageBoxDefaultButton.Button1,
					MessageBoxIcon.Error);
				c_miGetDatahelp.Enabled = false;
			}
		}

		private void SaveMRUList()
		{
			StreamWriter oOutput = null;
			try
			{
				oOutput = new StreamWriter(Path.Combine(CurrentSettingsDirectory, "MRU.txt"), false);

				foreach (String szMRU in c_tbSearchKeywords.Items)
				{
					oOutput.WriteLine(szMRU);
				}
			}
			catch (IOException)
			{
				// Do nothing, if a minor bug borks the MRU list, it's not the end of the world.
			}
			finally
			{
				if (oOutput != null) oOutput.Close();
			}
		}

		private void LoadMRUList()
		{
			if (!File.Exists(Path.Combine(CurrentSettingsDirectory, "MRU.txt"))) return;

			try
			{
				String[] aMRUs = File.ReadAllLines(Path.Combine(CurrentSettingsDirectory, "MRU.txt"));

				for (int count = 0; count < aMRUs.Length && count < MAX_MRU_TERMS; count++)
				{
					c_tbSearchKeywords.Items.Add(aMRUs[count]);
				}
			}
			catch (IOException)
			{
				// Do nothing, if we can't read the MRU list, it's not the end of the world.
			}
		}

		private void SetSearchable(bool blValue)
		{
			c_miSearch.Enabled = blValue;
			c_bSearch.Enabled = blValue;
		}

		private void SetSearchClearable(bool blValue)
		{
			c_bClearSearch.Enabled = blValue;
		}

		private void CenterNavigationToolStrip()
		{
			Point newLocation = new Point((c_tsNavigation.Parent.Width - c_tsNavigation.Width) / 2, c_tsNavigation.Parent.Height - c_tsNavigation.Height);
			c_tsNavigation.Location = newLocation;
		}

		/// <summary>
		/// Do a search programmatically.
		/// </summary>
		/// <remarks>
		/// Doesn't update MRU list or supress repeated searches.
		/// </remarks>
		/// <param name="szKeywords"></param>
		/// <param name="oAoI"></param>
		private void doSearch(String szKeywords, GeographicBoundingBox oAoI)
		{
			SetSearchable(false);
			SetSearchClearable(true);

			m_oLastSearchROI = oAoI;
			m_szLastSearchString = szKeywords;

			applySearchCriteria();
		}

		private void doSearch()
		{
			// --- Cancel if the search parameters are unchanged ---
			GeographicBoundingBox oCurrSearchROI = GeographicBoundingBox.FromQuad(c_oWorldWindow.CurrentAreaOfInterest);
			String szCurrSearchString = SearchKeyword;
			if (oCurrSearchROI.Equals(m_oLastSearchROI) && szCurrSearchString.Equals(m_szLastSearchString)) return;

			// --- Reorder the MRU list.  Supress index changed is important because removing the current MRU will raise the event again ---
			m_blSupressSearchSelectedIndexChanged = true;
			c_tbSearchKeywords.SuspendLayout();
			if (!SearchKeyword.Equals(String.Empty))
			{
				c_tbSearchKeywords.Items.Remove(szCurrSearchString);

				while (c_tbSearchKeywords.Items.Count >= MAX_MRU_TERMS)
				{
					c_tbSearchKeywords.Items.RemoveAt(c_tbSearchKeywords.Items.Count - 1);
				}
				c_tbSearchKeywords.Items.Insert(0, szCurrSearchString);
				c_tbSearchKeywords.Text = szCurrSearchString;
			}
			c_tbSearchKeywords.ResumeLayout();
			m_blSupressSearchSelectedIndexChanged = false;

			// --- Mop up and move out ---

			SetSearchable(false);
			SetSearchClearable(true);

			m_oLastSearchROI = GeographicBoundingBox.FromQuad(c_oWorldWindow.CurrentAreaOfInterest);
			m_szLastSearchString = SearchKeyword;

			applySearchCriteria();
		}

		private void clearSearch()
		{
			SetSearchable(true);
			SetSearchClearable(false);

			m_oLastSearchROI = null;
			m_szLastSearchString = String.Empty;

			c_tbSearchKeywords.Text = NO_SEARCH;
			c_tbSearchKeywords.ForeColor = SystemColors.GrayText;

			applySearchCriteria();
		}

		private void applySearchCriteria()
		{
			this.UseWaitCursor = true;
			Application.DoEvents();
			m_oModel.SetSearchFilter(m_szLastSearchString, m_oLastSearchROI);
			this.c_oServerList.setSearchCriteria(m_szLastSearchString, m_oLastSearchROI);
			this.c_oDappleSearch.SetSearchParameters(m_szLastSearchString, m_oLastSearchROI);
			this.UseWaitCursor = false;
			Application.DoEvents();
		}

		private void CmdSetupServersMenu()
		{
			bool blServerSelected = m_oModel.SelectedServer != null;
			blServerSelected &= c_tcSearchViews.SelectedIndex == 0;
			blServerSelected &= cServerViewsTab.SelectedIndex == 0;

			c_miViewProperties.Enabled = blServerSelected;
			c_miRefreshServer.Enabled = blServerSelected;
			c_miRemoveServer.Enabled = blServerSelected;
			c_miSetFavouriteServer.Enabled = blServerSelected && !m_oModel.SelectedServer.Favourite;

			if (blServerSelected == false)
			{
				c_miToggleServerStatus.Text = "Disable";
				c_miToggleServerStatus.Image = Resources.disserver;
				c_miToggleServerStatus.Enabled = false;
			}
			else
			{
				bool blServerEnabled = m_oModel.SelectedServer.Enabled;

				if (blServerEnabled)
				{
					c_miToggleServerStatus.Text = "Disable";
					c_miToggleServerStatus.Image = Resources.disserver;
				}
				else
				{
					c_miToggleServerStatus.Text = "Enable";
					c_miToggleServerStatus.Image = Resources.enserver;
				}
				c_miToggleServerStatus.Enabled = true;
			}
		}

		private void CmdSetupToolsMenu()
		{
			if (c_tcSearchViews.SelectedIndex == 0)
			{
				if (cServerViewsTab.SelectedIndex == 0)
				{
					// --- Active is server tree ---
					c_miAddLayer.Enabled = m_oModel.SelectedNode is LayerModelNode;
				}
				else if (cServerViewsTab.SelectedIndex == 1)
				{
					// --- Active is server list ---
					c_miAddLayer.Enabled = c_oServerList.SelectedLayers.Count > 0;
				} 
			}
			else if (c_tcSearchViews.SelectedIndex == 1)
			{
				// --- Active is dapple search list ---
				c_miAddLayer.Enabled = c_oDappleSearch.HasLayersSelected;
			}

			c_miRemoveLayer.Enabled = c_oLayerList.RemoveAllowed;
		}

		internal void CmdShowServerTree()
		{
			c_tcSearchViews.SelectedIndex = 0;
		}

		#endregion
	}
}