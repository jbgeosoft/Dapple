using System;
using System.Net;
using System.IO;
using System.Threading;

#if DAPPLE
using WorldWind.Net;
#endif

namespace Geosoft.Dap.Xml
{
	/// <summary>
	/// Delegate for providing download progress feedback
	/// </summary>
	/// <param name="iBytesRead"></param>
	/// <param name="iTotalBytes"></param>
	public delegate void UpdateProgessCallback(int iBytesRead, int iTotalBytes);

	/// <summary>
	/// Summary description for DownloadThread.
	/// </summary>
	internal class DownloadThread
	{
		public event UpdateProgessCallback ProgressCallback;
		public byte[] downloadedData = null;
		public string downloadUrl = "";
		public WebRequest webReq = null;
		public Exception excepted = null;

		public void Download()
		{
			if (ProgressCallback != null)
			{
				try
				{
					WebDownload webDL = new WebDownload();
					if (webReq != null)
						downloadedData = webDL.Download(webReq, ProgressCallback);
					else if (downloadUrl.Length > 0)
						downloadedData = webDL.Download(downloadUrl, ProgressCallback);
				}
				catch (Exception e)
				{
					excepted = e;
				}
			}
		}
	}

	// The RequestState class passes data across async calls.
	internal class DownloadInfo
	{
		const int BufferSize = 1024;
		public byte[] BufferRead;

		public bool useFastBuffers;
		public byte[] dataBufferFast;
		public System.Collections.ArrayList dataBufferSlow;

		public int dataLength;
		public int bytesProcessed;

		public WebRequest Request;
		public Stream ResponseStream;

		public UpdateProgessCallback ProgressCallback;

		public DownloadInfo()
		{
			BufferRead = new byte[BufferSize];
			Request = null;
			dataLength = -1;
			bytesProcessed = 0;

			useFastBuffers = true;
		}
	}

	// ClientGetAsync issues the async request.
	internal class WebDownload
	{
		public ManualResetEvent allDone = new ManualResetEvent(false);
		const int BUFFER_SIZE = 1024;

		public byte[] Download(string url, UpdateProgessCallback progressCB)
		{
			// Get the URI from the command line.
			Uri httpSite = new Uri(url);

			// Create the request object.
			WebRequest req = WebRequest.Create(httpSite);

#if DAPPLE
			req.Proxy = ProxyHelper.DetermineProxyForUrl(
							  url,
							  WorldWind.Net.WebDownload.useWindowsDefaultProxy,
							  WorldWind.Net.WebDownload.useDynamicProxy,
							  WorldWind.Net.WebDownload.proxyUrl,
							  WorldWind.Net.WebDownload.proxyUserName,
							  WorldWind.Net.WebDownload.proxyPassword);
			req.Timeout = WorldWind.Net.WebDownload.DownloadTimeout;
#endif

			return Download(req, progressCB);
		}

		public byte[] Download(WebRequest req, UpdateProgessCallback progressCB)
		{
			// Ensure flag set correctly.			
			allDone.Reset();

			// Create the state object.
			DownloadInfo info = new DownloadInfo();

			// Put the request into the state object so it can be passed around.
			info.Request = req;

			// Assign the callbacks
			info.ProgressCallback += progressCB;

			// Issue the async request.
			IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(ResponseCallback), info);

			// Wait until the ManualResetEvent is set so that the application
			// does not exit until after the callback is called.
			allDone.WaitOne();

			// Pass back the downloaded information.

			if (info.useFastBuffers)
				return info.dataBufferFast;
			else
			{
				byte[] data = new byte[info.dataBufferSlow.Count];
				for (int b = 0; b < info.dataBufferSlow.Count; b++)
					data[b] = (byte)info.dataBufferSlow[b];
				return data;
			}
		}

		private void ResponseCallback(IAsyncResult ar)
		{
			// Get the DownloadInfo object from the async result were
			// we're storing all of the temporary data and the download
			// buffer.
			DownloadInfo info = (DownloadInfo)ar.AsyncState;

			// Get the WebRequest from RequestState.
			WebRequest req = info.Request;

			// Call EndGetResponse, which produces the WebResponse object
			// that came from the request issued above.
			WebResponse resp = req.EndGetResponse(ar);

			// Find the data size from the headers.
			string strContentLength = resp.Headers["Content-Length"];
			if (strContentLength != null)
			{
				info.dataLength = Convert.ToInt32(strContentLength);
				info.dataBufferFast = new byte[info.dataLength];
			}
			else
			{
				info.useFastBuffers = false;
				info.dataBufferSlow = new System.Collections.ArrayList(BUFFER_SIZE);
			}

			//  Start reading data from the response stream.
			Stream ResponseStream = resp.GetResponseStream();

			// Store the response stream in RequestState to read
			// the stream asynchronously.
			info.ResponseStream = ResponseStream;

			//  Pass do.BufferRead to BeginRead.
			IAsyncResult iarRead = ResponseStream.BeginRead(info.BufferRead,
				0,
				BUFFER_SIZE,
				new AsyncCallback(ReadCallBack),
				info);
		}

		private void ReadCallBack(IAsyncResult asyncResult)
		{
			// Get the DownloadInfo object from AsyncResult.
			DownloadInfo info = (DownloadInfo)asyncResult.AsyncState;

			// Retrieve the ResponseStream that was set in RespCallback.
			Stream responseStream = info.ResponseStream;

			// Read info.BufferRead to verify that it contains data.
			int bytesRead = responseStream.EndRead(asyncResult);
			if (bytesRead > 0)
			{
				if (info.useFastBuffers)
				{
					System.Array.Copy(info.BufferRead, 0,
						info.dataBufferFast, info.bytesProcessed,
						bytesRead);
				}
				else
				{
					for (int b = 0; b < bytesRead; b++)
						info.dataBufferSlow.Add(info.BufferRead[b]);
				}
				info.bytesProcessed += bytesRead;

				// If a registered progress-callback, inform it of our
				// download progress so far.
				if (info.ProgressCallback != null)
					info.ProgressCallback(info.bytesProcessed, info.dataLength);

				// Continue reading data until responseStream.EndRead returns �1.
				IAsyncResult ar = responseStream.BeginRead(
					info.BufferRead, 0, BUFFER_SIZE,
					new AsyncCallback(ReadCallBack), info);
			}
			else
			{
				responseStream.Close();
				allDone.Set();
			}
			return;
		}
	}

	/// <summary>
	/// Transmit and recieve xml over an http connection
	/// </summary>
	public class Communication
	{
		#region Constants
		const int BUFFER_SIZE = 2048;
		const int DEFAULT_TIMEOUT = 15000;
		#endregion

		#region Member Variables
		private bool m_bTask;
		private bool m_bSecure;
		private int m_iTimeout;
		private bool m_bReadFromCan = false;
		private bool m_bWriteToCan = false;
		private System.Collections.Generic.Dictionary<int, System.IO.MemoryStream> m_oResponseCan = null;
		#endregion

		#region Properties
		/// <summary>
		/// Get/Set wether to send this encypted or not
		/// </summary>
		public bool Secure
		{
			get { return m_bSecure; }
			set { m_bSecure = value; }
		}

		/// <summary>
		/// Get/Set wether to send this through task
		/// </summary>
		public bool Task
		{
			get { return m_bTask; }
			set { m_bTask = value; }
		}

		/// <summary>
		/// Get/Set the can used for canned responses.
		/// </summary>
		public System.Collections.Generic.Dictionary<int, System.IO.MemoryStream> ResponseCan
		{
			get { return m_oResponseCan; }
			set { m_oResponseCan = value; }
		}

		/// <summary>
		/// Get/Set whether to read http responses from the can instead of from the internet.
		/// </summary>
		public bool ReadFromCan
		{
			get { return m_bReadFromCan; }
			set
			{
				if (m_bWriteToCan && value)
					throw new InvalidOperationException("Cannot enable reading from the can while writing to the can is enabled.");
				if (m_oResponseCan == null)
					throw new InvalidOperationException("Cannot enable reading from the can before the can is initialized.  Assign a non-null value to ResponseCan first.");
				m_bReadFromCan = value;
			}
		}

		/// <summary>
		/// Get/Set whether to save http responses from the internet to the response can.
		/// </summary>
		public bool WritetoCan
		{
			get { return m_bWriteToCan; }
			set
			{
				if (m_bReadFromCan && value)
					throw new InvalidOperationException("Cannot enable writing to the can while reading from the can is enabled.");
				if (m_oResponseCan == null)
					throw new InvalidOperationException("Cannot enable writing to the can before the can is initialized.  Assign a non-null value to ResponseCan first.");
				m_bWriteToCan = value;
			}
		}

		/// <summary>
		/// The number of milliseconds before a connection times out.
		/// </summary>
		public int Timeout
		{
			get { return m_iTimeout; }
			set { m_iTimeout = value; }
		}

		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="bTask"></param>
		/// <param name="bSecure"></param>
		public Communication(bool bTask, bool bSecure)
			: this(bTask, bSecure, DEFAULT_TIMEOUT)
		{
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="bTask"></param>
		/// <param name="bSecure"></param>
		/// <param name="iTimeout"></param>
		public Communication(bool bTask, bool bSecure, int iTimeout)
		{
			m_bTask = bTask;
			m_bSecure = bSecure;
			m_iTimeout = iTimeout;
		}

		/// <summary>
		/// Send an xml request on the wire, get the response and parse it into an XML document
		/// </summary>
		/// <param name="hRequestDocument"></param>
		/// <param name="szUrl"></param>
		/// <param name="progressCallBack">Progress handler (may be null)</param>
		/// <returns></returns>
		public System.Xml.XmlDocument Send(string szUrl, System.Xml.XmlDocument hRequestDocument, UpdateProgessCallback progressCallBack)
		{
			System.Xml.XmlDocument result = null;
#if !DAPPLE
			if (m_bTask)
			{
				result = SendTask(szUrl, hRequestDocument);
			}
			else
#endif
			{
				result = SendHttp(szUrl, hRequestDocument, progressCallBack);
			}

			CheckForError(result);

			return result;
		}

		/// <summary>
		/// Check a response XML document to see if it contains any error messages.
		/// </summary>
		/// <param name="oDoc"></param>
		private void CheckForError(System.Xml.XmlDocument oDoc)
		{
			// --- search for an error ---

			System.Xml.XmlNodeList hNodeList = oDoc.SelectNodes("//" + Geosoft.Dap.Xml.Common.Constant.Tag.ERROR_TAG);
			if (hNodeList.Count >= 1)
			{
				System.Xml.XmlElement hElement = hNodeList[0] as System.Xml.XmlElement;

				throw new DapException(hElement.InnerText);
			}
		}

		/// <summary>
		/// Send an xml request on the wire, get the response and parse it into an XML reader
		/// </summary>
		/// <param name="hRequestDocument"></param>
		/// <param name="szUrl"></param>
		/// <param name="strResponseFile"></param>
		/// <param name="progressCallBack">Progress handler (may be null)</param>
		/// <returns></returns>
		public System.Xml.XmlReader SendEx(string szUrl, System.Xml.XmlDocument hRequestDocument, string strResponseFile, UpdateProgessCallback progressCallBack)
		{
#if !DAPPLE
			if (m_bTask)
			{
				return SendTaskEx(szUrl, hRequestDocument, strResponseFile);
			}
#endif
			return SendHttpEx(szUrl, hRequestDocument, progressCallBack);
		}

#if !DAPPLE
		/// <summary>
		/// Send an xml request through task
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="oRequestDocument"></param>
		/// <param name="strResponseFile"></param>
		/// <returns></returns>
		protected void SendTaskInternal(string strUrl, System.Xml.XmlDocument oRequestDocument, string strResponseFile)
		{
			Geosoft.GXNet.CDAP hDAP = null;

			try
			{
				string strXML;

				// --- remove the /ois.dll?geosoft_xml from the url ---

				int iIndex = strUrl.LastIndexOf("/");
				strUrl = strUrl.Substring(0, iIndex);

				hDAP = Geosoft.GXNet.CDAP.Create(strUrl, "Send XML Request");
				strXML = oRequestDocument.InnerXml;

				if (hDAP.sExecuteGeosoftXML(strXML, strXML.Length, strResponseFile) != 0)
				{
					int iNumErrors = Geosoft.GXNet.CSYS.iNumErrorsAP();

					String[] errStrings = new String[iNumErrors];
					for (int count = 0; count < iNumErrors; count++)
					{
						Geosoft.GXNet.CSYS.IGetErrorMessageAP(count, ref errStrings[count]);
					}
					Geosoft.GXNet.CSYS.iClearErrAP();

					throw new DapException(errStrings[0]);
				}
			}
			finally
			{
				if (hDAP != null) hDAP.Dispose();
			}
		}

		/// <summary>
		/// Send an xml request through task
		/// </summary>
		/// <param name="szUrl"></param>
		/// <param name="hRequestDocument"></param>
		/// <returns></returns>
		protected System.Xml.XmlDocument SendTask(string szUrl, System.Xml.XmlDocument hRequestDocument)
		{
			string strXMLResponseFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			System.Xml.XmlDocument hResponseDocument = null;

			try
			{
				SendTaskInternal(szUrl, hRequestDocument, strXMLResponseFile);

				hResponseDocument = new System.Xml.XmlDocument();
				hResponseDocument.Load(strXMLResponseFile);
			}
			finally
			{
				System.IO.File.Delete(strXMLResponseFile);
			}
			return hResponseDocument;
		}

		/// <summary>
		/// Send an xml request through task
		/// </summary>
		/// <param name="szUrl"></param>
		/// <param name="hRequestDocument"></param>
		/// <param name="strResponseFile"></param>
		/// <returns></returns>
		protected System.Xml.XmlReader SendTaskEx(string szUrl, System.Xml.XmlDocument hRequestDocument, string strResponseFile)
		{
			System.Xml.XmlReader oResponse = null;

			SendTaskInternal(szUrl, hRequestDocument, strResponseFile);
			oResponse = System.Xml.XmlReader.Create(strResponseFile);

			return oResponse;
		}

#endif
		/// <summary>
		/// Send an xml request on the wire, get the response and parse it into an XML document
		/// </summary>
		/// <param name="oRequestDocument"></param>
		/// <param name="szUrl"></param>
		/// <param name="progressCallBack"></param>
		/// <returns></returns>
		protected System.Xml.XmlReader SendHttpInternal(string szUrl, System.Xml.XmlDocument oRequestDocument, UpdateProgessCallback progressCallBack)
		{
			System.Xml.XmlReaderSettings oSettings = new System.Xml.XmlReaderSettings();
			oSettings.IgnoreWhitespace = true;
			int iCanKey = 0;
			if (m_bReadFromCan || m_bWriteToCan)
				iCanKey = szUrl.GetHashCode() ^ oRequestDocument.OuterXml.GetHashCode();

			// Get responses from the can if available
			if (m_bReadFromCan)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine("Loading can entry " + iCanKey.ToString("x"));
#endif
				if (m_oResponseCan.ContainsKey(iCanKey))
				{
					MemoryStream oBackingStore = new MemoryStream();
					lock (((System.Collections.ICollection)m_oResponseCan).SyncRoot)
					{
						MemoryStream oCannedStore = m_oResponseCan[iCanKey];
						oCannedStore.WriteTo(oBackingStore);
						oCannedStore.Position = 0;
						oBackingStore.Position = 0;
					}

					return System.Xml.XmlReader.Create(oBackingStore, oSettings);
				}
				else
				{
					throw new WebException("Response can does not contain this datum", WebExceptionStatus.ConnectFailure);
				}
			}

			byte[] byte1;

			System.IO.StringWriter hRequest = null;
			System.IO.Stream hRequestStream = null;
			System.Xml.XmlReader oResponseXmlStream = null;

			System.Net.HttpWebRequest cHttpWReq = null;
			System.Net.HttpWebResponse cHttpWResp = null;

			try
			{
				// --- Initialize all the required streams to write out the xml request ---

				hRequest = new System.IO.StringWriter();


				// --- Create a HTTP Request to the Dap Server and HTTP Response Objects---

				cHttpWReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(szUrl);
				cHttpWReq.Pipelined = false;

#if DAPPLE
				cHttpWReq.Proxy = ProxyHelper.DetermineProxyForUrl(
								  szUrl,
								  WorldWind.Net.WebDownload.useWindowsDefaultProxy,
								  WorldWind.Net.WebDownload.useDynamicProxy,
								  WorldWind.Net.WebDownload.proxyUrl,
								  WorldWind.Net.WebDownload.proxyUserName,
								  WorldWind.Net.WebDownload.proxyPassword);
#endif



				// --- Encode the document into ascii ---

				System.Text.UTF8Encoding hRequestEncoding = new System.Text.UTF8Encoding();

				oRequestDocument.Save(hRequest);
				byte1 = hRequestEncoding.GetBytes(hRequest.GetStringBuilder().ToString());


				// --- Setup the HTTP Request ---

				cHttpWReq.Method = "POST";
#if DAPPLE
				if (WorldWind.Net.WebDownload.useProto == WorldWind.Net.WebDownload.HttpProtoVersion.HTTP1_1)
					cHttpWReq.ProtocolVersion = HttpVersion.Version11;
				else
					cHttpWReq.ProtocolVersion = HttpVersion.Version10;
#else
				cHttpWReq.ProtocolVersion = HttpVersion.Version11;
#endif
				cHttpWReq.KeepAlive = false;
				cHttpWReq.ContentType = "application/x-www-form-urlencoded";
				cHttpWReq.ContentLength = byte1.Length;
				cHttpWReq.Timeout = m_iTimeout;

				// --- Serialize the XML document onto the wire ---

				hRequestStream = cHttpWReq.GetRequestStream();
				hRequestStream.Write(byte1, 0, byte1.Length);
				hRequestStream.Close();

				// --- Turn off connection keep-alives. ---

				cHttpWReq.KeepAlive = false;

				if (progressCallBack == null)
				{
					// --- Get the response ---

					cHttpWResp = (System.Net.HttpWebResponse)cHttpWReq.GetResponse();

					if (m_bWriteToCan)
					{
#if DEBUG
						System.Diagnostics.Debug.WriteLine("Saving can entry " + iCanKey.ToString("x"));
#endif
						MemoryStream oCannedData = bufferResponseStream(cHttpWResp.GetResponseStream());
						MemoryStream oResponseBackingStore = new MemoryStream();
						oCannedData.WriteTo(oResponseBackingStore);
						oCannedData.Position = 0;
						oResponseBackingStore.Position = 0;
						oResponseXmlStream = System.Xml.XmlReader.Create(oResponseBackingStore, oSettings);

						m_oResponseCan[iCanKey] = oCannedData;
					}
					else
					{
						oResponseXmlStream = System.Xml.XmlReader.Create(cHttpWResp.GetResponseStream(), oSettings);
					}
				}
				else
				{
					// Create the download thread (class) and populate with filename and callbacks
					DownloadThread dl = new DownloadThread();
					dl.ProgressCallback += progressCallBack;
					dl.webReq = cHttpWReq;
					// Start the download thread....
					System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(dl.Download));
					t.Start();
					t.Join();

					if (dl.excepted != null)
						throw dl.excepted;

					MemoryStream memStrm = new MemoryStream(dl.downloadedData);

					if (m_bWriteToCan)
					{
#if DEBUG
						System.Diagnostics.Debug.WriteLine("Saving can entry " + iCanKey.ToString("x"));
#endif
						MemoryStream oCannedData = new MemoryStream();
						memStrm.WriteTo(oCannedData);
						oCannedData.Position = 0;
						memStrm.Position = 0;
						oResponseXmlStream = System.Xml.XmlReader.Create(memStrm, oSettings);

						lock (((System.Collections.ICollection)m_oResponseCan).SyncRoot)
						{
#if DEBUG
							if (m_oResponseCan.ContainsKey(iCanKey))
								System.Diagnostics.Debug.WriteLine("Duplicate can key detected");
#endif
							m_oResponseCan[iCanKey] = oCannedData;
						}
					}
					else
					{
						oResponseXmlStream = System.Xml.XmlReader.Create(memStrm, oSettings);
					}
				}
			}
			catch (Exception e)
			{
				oResponseXmlStream = null;
				throw e;
			}
			return oResponseXmlStream;
		}

		/// <summary>
		/// Buffer a stream to a MemoryStream.
		/// </summary>
		/// <param name="oToBuffer"></param>
		/// <returns></returns>
		public static MemoryStream bufferResponseStream(Stream oToBuffer)
		{
			MemoryStream result = new MemoryStream();
			int bufferSize = 4096;
			byte[] buffer = new byte[bufferSize];
			int bytesRead;
			while ((bytesRead = oToBuffer.Read(buffer, 0, bufferSize)) != 0)
			{
				result.Write(buffer, 0, bytesRead);
			}
			result.Position = 0;
			return result;
		}

		/// <summary>
		/// Send an xml request on the wire, get the response and parse it into an XML document
		/// </summary>
		/// <param name="hRequestDocument"></param>
		/// <param name="szUrl"></param>
		/// <param name="progressCallBack"></param>
		/// <returns></returns>
		protected System.Xml.XmlDocument SendHttp(string szUrl, System.Xml.XmlDocument hRequestDocument, UpdateProgessCallback progressCallBack)
		{
			System.Xml.XmlDocument hResponseDocument = null;
			System.Xml.XmlReader oResponse = null;

			try
			{

				// --- send the http request ---

				oResponse = SendHttpInternal(szUrl, hRequestDocument, progressCallBack);


				// --- Load Response into XML Document ---

				hResponseDocument = new System.Xml.XmlDocument();
				hResponseDocument.Load(oResponse);
			}
			catch (System.Xml.XmlException)
			{
				hResponseDocument = null;
				throw new DapException("Error reading server response");
			}
			catch (Exception e)
			{
				hResponseDocument = null;
				throw e;
			}
			finally
			{
				if (oResponse != null) oResponse.Close();
			}
			return hResponseDocument;
		}

		/// <summary>
		/// Send an xml request onto the wire and return a pointer to the actual response stream. Used when you have a long response
		/// that you do not want to load into an XML document and store all in memory. 
		/// Note: The response stream is not closed. That is left up to the caller to dispose of
		/// </summary>
		/// <param name="hRequestDocument"></param>
		/// <param name="szUrl"></param>
		/// <param name="progressCallBack"></param>
		/// <returns></returns>
		public System.Xml.XmlReader SendHttpEx(string szUrl, System.Xml.XmlDocument hRequestDocument, UpdateProgessCallback progressCallBack)
		{
			System.Xml.XmlReader oResponse = null;

			try
			{

				// --- send the http request ---

				oResponse = SendHttpInternal(szUrl, hRequestDocument, progressCallBack);
			}
			catch (Exception e)
			{
				oResponse = null;
				throw e;
			}
			return oResponse;
		}
	}
}
