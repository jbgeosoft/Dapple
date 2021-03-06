using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows.Forms;
using DM.SharedMemory;
using Utility;
using WorldWind;

namespace Dapple
{
   static class Program
	{
		internal static bool g_blTestingMode = false;
		internal static int g_iCallerProcID = -1;

		/// <summary>
      /// The main entry point for the application.
      /// Mutex code fragments taken from http://www.c-sharpcorner.com/FAQ/Create1InstanceAppSC.asp
      /// </summary>
      [STAThread]
      static void Main(string[] args)
		{
#if !DEBUG
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif
			WorldWindow.VideoMemoryExhausted += ReportVideoMemoryExhaustion;

#if !DEBUG
			bool aborting = false;
#endif

#if DEBUG
			System.Text.StringBuilder oTemp = new System.Text.StringBuilder();
         foreach (String arg in args)
         {
            oTemp.Append(arg);
            oTemp.Append(Environment.NewLine);
         }
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				ShowMessageBox(
					"Dapple is being invoked with the following command-line parameters:" + Environment.NewLine + oTemp.ToString() + Environment.NewLine + Environment.NewLine + "Attach debugger if desired, then press OK to continue.",
					"Dapple Startup",
					MessageBoxButtons.OK,
					MessageBoxDefaultButton.Button1,
					MessageBoxIcon.Information);
			}
#endif

			MontajRemote.RemoteInterface oRemoteInterface = null;
			IpcChannel oClientChannel = null;

			// Command line parsing
			CommandLineArguments cmdl = new CommandLineArguments(args);

			try
			{
				bool blSelectPersonalDAP = false;
				bool bAbort = false;
				string strView = "", strGeoTiff = "", strGeoTiffName = "", strLastView = "", strMapFileName = string.Empty;
				bool bGeotiffTmp = false;
				String strKMLFile = String.Empty, strKMLName = String.Empty;
				bool blKMLTmp = false;

				GeographicBoundingBox oAoi = null;
				string strAoiCoordinateSystem = string.Empty;
				Dapple.Extract.Options.Client.ClientType eClientType = Dapple.Extract.Options.Client.ClientType.None;

				if (cmdl["h"] != null)
				{
					PrintUsage();
					return;
				}

				SetDCAvailability(cmdl);

				if (cmdl["callerprocid"] != null)
				{
					g_iCallerProcID = Int32.Parse(cmdl["callerprocid"], CultureInfo.InvariantCulture);
				}

				if (cmdl[0] != null)
				{
					if (String.Compare(cmdl[0], "ABORT") == 0 && cmdl[1] != null)
						bAbort = true;
					else
					{
						strView = Path.GetFullPath(cmdl[0]);
						if (String.Compare(Path.GetExtension(strView), MainForm.ViewExt, true) != 0 || !File.Exists(strView))
						{
							PrintUsage();
							return;
						}
					}
				}

				if (cmdl["geotiff"] != null)
				{
					strGeoTiff = Path.GetFullPath(cmdl["geotiff"]);
					if (!(String.Compare(Path.GetExtension(strGeoTiff), ".tiff", true) == 0 || String.Compare(Path.GetExtension(strGeoTiff), ".tif", true) == 0) || !File.Exists(strGeoTiff))
					{
						PrintUsage();
						return;
					}
				}

#if !DEBUG
				if (cmdl["noaborttool"] != null)
				{
					g_blTestingMode = true;
				}
#endif
				if (cmdl["personaldap"] != null)
				{
					blSelectPersonalDAP = true;
				}

				if (cmdl["geotifftmp"] != null)
				{
					string strGeoTiffTmpVar = cmdl["geotifftmp"];
					int iIndex = strGeoTiffTmpVar.IndexOf(":");
					if (iIndex == -1)
					{
						PrintUsage();
						return;
					}

					strGeoTiff = Path.GetFullPath(strGeoTiffTmpVar.Substring(iIndex + 1));
					strGeoTiffName = strGeoTiffTmpVar.Substring(0, iIndex);
					bGeotiffTmp = true;
					if (strGeoTiffName.Length == 0 || !(String.Compare(Path.GetExtension(strGeoTiff), ".tiff", true) == 0 || String.Compare(Path.GetExtension(strGeoTiff), ".tif", true) == 0) || !File.Exists(strGeoTiff))
					{
						PrintUsage();
						return;
					}
				}

				if (cmdl["kmltmp"] != null)
				{
					string strKMLTmpVar = cmdl["kmltmp"];
					int iIndex = strKMLTmpVar.IndexOf(":");
					if (iIndex == -1)
					{
						PrintUsage();
						return;
					}

					strKMLFile = Path.GetFullPath(strKMLTmpVar.Substring(iIndex + 1));
					strKMLName = strKMLTmpVar.Substring(0, iIndex);
					blKMLTmp = true;
					if (strKMLName.Length == 0 || !(String.Compare(Path.GetExtension(strKMLFile), ".kmz", true) == 0 || String.Compare(Path.GetExtension(strKMLFile), ".kml", true) == 0) || !File.Exists(strKMLFile))
					{
						PrintUsage();
						return;
					}
				}

				if (cmdl["exitview"] != null)
					strLastView = Path.GetFullPath(cmdl["exitview"]);

				if (cmdl["montajport"] != null)
				{
					int iMontajPort = int.Parse(cmdl["montajport"], NumberStyles.Any, CultureInfo.InvariantCulture);

					if (cmdl["dummyserver"] != null)
					{
						oClientChannel = new IpcChannel(String.Format(CultureInfo.InvariantCulture, "localhost:{0}", iMontajPort));
						ChannelServices.RegisterChannel(oClientChannel, true);
						RemotingConfiguration.RegisterWellKnownServiceType(typeof(MontajRemote.RemoteInterface), "MontajRemote", System.Runtime.Remoting.WellKnownObjectMode.Singleton);
					}
					else
					{
						oClientChannel = new IpcChannel();
						ChannelServices.RegisterChannel(oClientChannel, true);
					}

					oRemoteInterface = (MontajRemote.RemoteInterface)Activator.GetObject(typeof(MontajRemote.RemoteInterface), String.Format(CultureInfo.InvariantCulture, "ipc://localhost:{0}/MontajRemote", iMontajPort));
				}

				if (cmdl["aoi"] != null)
				{
					String[] strValues = cmdl["aoi"].Split(new char[] { ',' });
					if (strValues.Length != 4)
					{
						ShowMessageBox(
							"The -aoi command line argument has incorrect number of components.",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}
					double dMinX = 180, dMinY = 90, dMaxX = -180, dMaxY = -90;

					bool bAoiArgument = double.TryParse(strValues[0], out dMinX);

					if (bAoiArgument)
						bAoiArgument = double.TryParse(strValues[1], out dMinY);

					if (bAoiArgument)
						bAoiArgument = double.TryParse(strValues[2], out dMaxX);

					if (bAoiArgument)
						bAoiArgument = double.TryParse(strValues[3], out dMaxY);

					if (bAoiArgument)
					{
						oAoi = new GeographicBoundingBox(dMaxY, dMinY, dMinX, dMaxX);
					}
					else
					{
						ShowMessageBox(
							"The -aoi command line argument has incorrectly-formatted component(s).",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}

					if (oAoi.North < oAoi.South || oAoi.East < oAoi.West)
					{
						ShowMessageBox(
							"The -aoi command line argument specifies an incorrect bounding box.",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}

					if (cmdl["aoi_cs"] != null)
					{
						strAoiCoordinateSystem = cmdl["aoi_cs"];
					}

					if (string.IsNullOrEmpty(strAoiCoordinateSystem))
					{
						ShowMessageBox(
							"The -aoi_cs command line parameter must be present when using -aoi parameter.",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}

					if (cmdl["filename_map"] != null)
					{
						strMapFileName = cmdl["filename_map"];
					}
				}

				if (cmdl["client"] != null)
				{
					try
					{
						eClientType = (Dapple.Extract.Options.Client.ClientType)Enum.Parse(eClientType.GetType(), cmdl["client"], true);
					}
					catch
					{
						ShowMessageBox(
							"The -client command line is invalid.",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}
				}


				// From now on in own path please and free the console
				Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				if (bAbort)
				{
					string strErrors = File.ReadAllText(args[1]);
#if !DEBUG
					aborting = true;
#endif
					ErrorDisplay errorDialog = new ErrorDisplay();
					errorDialog.errorMessages(strErrors);
					Application.Run(errorDialog);
				}
				else
				{
					if (GetSystemMetrics(SM_REMOTESESSION) != 0)
					{
						ShowMessageBox(
							"Dapple cannot be run over a remote connection.",
							"Dapple Startup",
							MessageBoxButtons.OK,
							MessageBoxDefaultButton.Button1,
							MessageBoxIcon.Error);
						return;
					}

					Process instance = RunningInstance();

					if (instance == null)
					{
						try
						{
							MainForm oForm = new MainForm(strView, strGeoTiff, strGeoTiffName, bGeotiffTmp, strKMLFile, strKMLName, blKMLTmp, strLastView, eClientType, oRemoteInterface, oAoi, strAoiCoordinateSystem, strMapFileName);
							oForm.SetSelectPersonalDAPOnStartup(blSelectPersonalDAP);
							Application.Run(oForm);
						}
						catch (Microsoft.DirectX.DirectXException)
						{
							if (g_blTestingMode == true)
							{
								throw;
							}
							else
							{
								ShowMessageBox(
									"Dapple was unable to locate a compatible graphics adapter. Make sure you are running the latest version of DirectX.",
									"Dapple Startup",
									MessageBoxButtons.OK,
									MessageBoxDefaultButton.Button1,
									MessageBoxIcon.Error);
								return;
							}
						}
						catch (System.Runtime.Remoting.RemotingException)
						{
							if (g_blTestingMode == true)
							{
								throw;
							}
							else
							{
								ShowMessageBox(
									"Dapple has experienced an error attempting to connect to " + EnumUtils.GetDescription(eClientType) + ". Restarting the application or your computer may fix this problem.",
									"Dapple Startup",
									MessageBoxButtons.OK,
									MessageBoxDefaultButton.Button1,
									MessageBoxIcon.Error);
							}
						}
						catch (System.ComponentModel.Win32Exception)
						{
							if (g_blTestingMode == true)
							{
								throw;
							}
							else
							{
								ShowMessageBox(
									"Dapple has encountered an internal Win32 error during startup." + Environment.NewLine +
									"If this error persists, and you have a Logitech webcam installed, you may be able to resolve the error" + Environment.NewLine +
									"by disabling the 'Process Monitor' and 'LvSrvLauncher' services on your computer.",
									"Dapple Startup",
									MessageBoxButtons.OK,
									MessageBoxDefaultButton.Button1,
									MessageBoxIcon.Error);
							}
						}
					}
					else
					{
						HandleRunningInstance(instance);
						if (strView.Length > 0 || strGeoTiff.Length > 0 || (strKMLName.Length > 0 && strKMLFile.Length > 0))
						{
							try
							{
								using (Segment s = new Segment("Dapple.OpenView", SharedMemoryCreationFlag.Create, 10000))
								{
									string[] strData = new string[8];
									strData[0] = strView;
									strData[1] = strGeoTiff;
									strData[2] = strGeoTiffName;
									strData[3] = bGeotiffTmp ? "YES" : "NO";
									strData[4] = strLastView;
									strData[5] = blKMLTmp ? "YES" : "NO";
									strData[6] = strKMLName;
									strData[7] = strKMLFile;

									s.SetData(strData);
									SendMessage(instance.MainWindowHandle, MainForm.OpenViewMessage, IntPtr.Zero, IntPtr.Zero);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
#if !DEBUG
			catch (Exception caught)
			{
				if (g_blTestingMode)
				{
					throw;
				}
				else
				{
					if (!aborting)
						Utility.AbortUtility.Abort(caught, Thread.CurrentThread);
				}
			}
#endif
			finally
			{
				if (oRemoteInterface != null)
				{
					try
					{
						oRemoteInterface.EndConnection();
					}
					catch (System.Runtime.Remoting.RemotingException) { } // Ignore these, they most likely mean that OM was closed before Dapple was.
				}
				if (oClientChannel != null)
				{
					try
					{
						ChannelServices.UnregisterChannel(oClientChannel);
					}
					catch (System.Runtime.Remoting.RemotingException) { } // Ignore these, they most likely mean that OM was closed before Dapple was.
				}
			}
		}

		private static void SetDCAvailability(CommandLineArguments cmdl)
		{
			if (cmdl["unlicensed"] != null)
			{
				NewServerTree.PersonalDapServerModelNode.DisableDesktopCataloger();
			}
		}

		internal static void FocusOnCaller()
		{
			if (g_iCallerProcID != -1)
			{
				try
				{
					Process caller = Process.GetProcessById(g_iCallerProcID);
					SetForegroundWindow(caller.MainWindowHandle);
				}
				catch (ArgumentException)
				{
					// --- Invalid process.  Just don't switch ---
				}
			}
		}

      internal static void PrintUsage()
      {
         ShowMessageBox("Dapple command line usage:\n" +
				"\n" +
				"Dapple -h -geotiff=file -exitview=view view\n" +
				"\n" +
				"-h\t\tthis help\n" +
				"-geotiff=file\tpath to a geotiff in WGS84 to be loaded in the current or start-up view\n" +
				"–geotifftmp=name:tmpfilename Layer name and path to a temporary geotiff filename\n" +
				"                             (will be deleted on Dapple exit) in WGS84 to be loaded\n" +
				"                             in current or start-up view.\n" + 
				"-exitview=view\tpath to a Dapple view file in which to place the last view\n" +
				"view\t\tpath to a Dapple view file to load as start-up view\n" +
				"\n",
				"Dapple Command Line Usage",
				MessageBoxButtons.OK,
				MessageBoxDefaultButton.Button1,
				MessageBoxIcon.Information);
      }

      internal static Process RunningInstance()
      {
         Process current = Process.GetCurrentProcess();
         Process[] processes = Process.GetProcessesByName(current.ProcessName);

         //Loop through the running processes in with the same name
         foreach (Process process in processes)
         {
            //Ignore the current process
            if (process.Id != current.Id)
            {
					if (current.ProcessName.Contains("vshost"))
					{
						if (MessageBox.Show("Another instance of Dapple is running, but this is a Visual Studio hosting process. Ignore other process and run anyway?", "Dapple Process Conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							return null;
						}
						else
						{
							return process;
						}
					}

               //Make sure that the process is running from the exe file.
               //if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") ==
               //current.MainModule.FileName)
               //{
               //Return the other process instance.
               return process;
               //}
            }
         }
         return null;
      }

      internal static void HandleRunningInstance(Process instance)
      {
         //Make sure the window is not minimized or maximized
         if (IsIconic(instance.MainWindowHandle))
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);

         //Set the real intance to foreground window
         SetForegroundWindow(instance.MainWindowHandle);
      }

		/// <summary>
		/// Occurs when an un-trapped thread exception is thrown in UI event handlers.
		/// </summary>
		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			CrashAndBurn(e.Exception);
		}

		/// <summary>
		/// Occurs when an un-trapped thread exception is thrown in ThreadPool threads.
		/// </summary>
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			// --- Don't report these, as they occur naturally in the .NET runtime.
			if (ex is ThreadAbortException || ex is AppDomainUnloadedException) return;
			CrashAndBurn(ex);
		}

		/// <summary>
		/// Log the exception, and if this isn't a debug build, display an abort that users can send in.
		/// </summary>
		/// <param name="e"></param>
		private static void CrashAndBurn(Exception e)
		{
			Log.Write(e);
#if !DEBUG
			Utility.AbortUtility.Abort(e, Thread.CurrentThread);
#endif
		}

      [DllImport("User32.dll")]
      private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

      [DllImport("User32.dll")]
      private static extern bool IsIconic(IntPtr hWnd);

      [DllImport("User32.dll")]
      private static extern UInt32 SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

      [DllImport("User32.dll")]
      private static extern bool SetForegroundWindow(IntPtr hWnd);

      private const int WS_SHOWNORMAL = 1;

		[DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
		private static extern int GetSystemMetrics(int nIndex);

		private const int SM_REMOTESESSION = 0x1000;

		/// <summary>
		/// Shows a MessageBox.
		/// </summary>
		/// <param name="strTitle"></param>
		/// <param name="strMessage"></param>
		/// <param name="eButtons"></param>
		/// <param name="eIcon"></param>
		/// <param name="eDefBuffon"></param>
		/// <returns></returns>
		internal static DialogResult ShowMessageBox(String strMessage, String strCaption, MessageBoxButtons eButtons, MessageBoxDefaultButton eDefBuffon, MessageBoxIcon eIcon)
		{
			return MessageBox.Show(strMessage, strCaption, eButtons, eIcon, eDefBuffon);
		}

		internal static void ReportVideoMemoryExhaustion(object sender, EventArgs e)
		{
			ShowMessageBox(
				"Dapple has run out of video memory and must close. To increase the amount of video memory available to Dapple, close any other applications that have 3D displays open, or upgrade your video card.",
				"Video Memory Exhausted",
				MessageBoxButtons.OK,
				MessageBoxDefaultButton.Button1,
				MessageBoxIcon.Error);

			System.Environment.Exit(-1);
		}
   }
}
