using System;
using System.Collections.Generic;
using System.Text;
using WorldWind.Renderable;
using WorldWind;
using Geosoft.GX.DAPGetData;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using Dapple.DAP;

namespace Dapple.LayerGeneration
{
   class DAPBrowserMapBuilder : LayerBuilder
   {
      #region Static

		public static readonly string URISchemeName = "gxdapbm";

		public static readonly string CacheSubDir = "DAPImages";

		#endregion

      #region Member variables

      private QuadTileSet m_layer;
		private Server m_oServer;

      public int m_iHeight;
      public int m_iTextureSizePixels;

      #endregion

      public DAPBrowserMapBuilder(WorldWindow worldWindow, Server server, IBuilder parent)
			:
		   this(worldWindow,  server, parent, 0, 256)
		{
		}

		public DAPBrowserMapBuilder(WorldWindow worldWindow, Server server, IBuilder parent, int height, int size)
         :base("Browser map for " + server.Name, worldWindow, parent)
		{
			m_oServer = server;

			m_iHeight = height;
			m_iTextureSizePixels = size;
      }

      #region ImageBuilder Implementations

      public override GeographicBoundingBox Extents
      {
         get
         {
            return new GeographicBoundingBox(
               m_oServer.ServerExtents.MaxY,
               m_oServer.ServerExtents.MinY,
               m_oServer.ServerExtents.MinX,
               m_oServer.ServerExtents.MaxX
            );
         }
      }

      public override byte Opacity
      {
         get
         {
            if (m_layer != null)
               return m_layer.Opacity;
            return m_bOpacity;
         }
         set
         {
            bool bChanged = false;
            if (m_bOpacity != value)
            {
               m_bOpacity = value;
               bChanged = true;
            }
            if (m_layer != null && m_layer.Opacity != value)
            {
               m_layer.Opacity = value;
               bChanged = true;
            }
            if (bChanged)
               SendBuilderChanged(BuilderChangeType.OpacityChanged);
         }
      }

      public override bool Visible
      {
         get
         {
            if (m_layer != null)
               return m_layer.IsOn;
            return m_IsOn;
         }
         set
         {
            bool bChanged = false;
            if (m_IsOn != value)
            {
               m_IsOn = value;
               bChanged = true;
            }
            if (m_layer != null && m_layer.IsOn != value)
            {
               m_layer.IsOn = value;
               bChanged = true;
            }

            if (bChanged)
               SendBuilderChanged(BuilderChangeType.VisibilityChanged);
         }
      }

      [System.ComponentModel.Browsable(false)]
      public override bool IsChanged
      {
         get { return m_layer == null; }
      }

      public override string ServerTypeIconKey
      {
         get { return "dap"; }
      }

      public override string DisplayIconKey
      {
         get { return "dap"; }
      }

      public override bool bIsDownloading(out int iBytesRead, out int iTotalBytes)
      {
         if (m_layer != null)
            return m_layer.bIsDownloading(out iBytesRead, out iTotalBytes);
         else
         {
            iBytesRead = 0;
            iTotalBytes = 0;
            return false;
         }
      }
      
      public override RenderableObject GetLayer()
      {
         return GetQuadTileSet();
      }

      public override string GetURI()
      {
         NameValueCollection queryColl = new NameValueCollection();

         string strHost = "";
         string strPath = "";
         Utility.URI.ParseURI("http", m_oServer.Url, ref strHost, ref strPath);

         return Utility.URI.CreateURI(URISchemeName, strHost, strPath, queryColl);
      }

      public override string GetCachePath()
      {
         return Path.Combine(Path.Combine(Path.Combine(m_strCacheRoot, CacheSubDir), m_oServer.Url.Replace("http://", "")), "BrowserMap");
      }

      protected override void CleanUpLayer(bool bFinal)
      {
         if (m_layer != null)
            m_layer.Dispose();
         m_layer = null;
      }

      public override object CloneSpecific()
      {
         return new DAPBrowserMapBuilder(m_oWorldWindow, m_oServer, m_Parent, m_iHeight, m_iTextureSizePixels);
      }

      public override bool Equals(object obj)
      {
         if (!(obj is DAPBrowserMapBuilder)) return false;
         DAPBrowserMapBuilder castObj = obj as DAPBrowserMapBuilder;

         // -- Equal if they are the browser map for the same server --
         return this.m_oServer.Url.Equals(castObj.m_oServer.Url);
      }

      #endregion

      #region ImageBuilder Overrides

      public override bool SupportsMetaData
      {
         get
         {
            return false;
         }
      }

      #endregion

      #region Private Members

      private QuadTileSet GetQuadTileSet()
      {
         if (m_layer == null)
         {

            ImageStore[] imageStores = new ImageStore[1];
            imageStores[0] = new DAPImageStore(String.Empty, m_oServer);
            imageStores[0].DataDirectory = null;
            imageStores[0].LevelZeroTileSizeDegrees = 90;
            imageStores[0].LevelCount = 5;
            imageStores[0].ImageExtension = ".png";
            imageStores[0].CacheDirectory = GetCachePath();
            imageStores[0].TextureFormat = World.Settings.TextureFormat;
            imageStores[0].TextureSizePixels = 512;

            m_layer = new QuadTileSet(this.Name, m_oWorldWindow.CurrentWorld, 0, m_oServer.ServerExtents.MaxY, m_oServer.ServerExtents.MinY, m_oServer.ServerExtents.MinX, m_oServer.ServerExtents.MaxX, true, imageStores);
            m_layer.AlwaysRenderBaseTiles = true;
            m_layer.IsOn = m_IsOn;
            m_layer.Opacity = m_bOpacity;
         }
         return m_layer;
      }

      #endregion

   }
}