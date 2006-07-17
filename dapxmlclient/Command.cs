using System;
using System.Collections;
using Geosoft.Dap.Common;
using Geosoft.Dap.Xml;
using Geosoft.Dap.Xml.Common;

namespace Geosoft.Dap
{
   /// <summary>
   /// Issue commands to a dap server
   /// </summary>
   public class Command
   {
      /// <summary>
      /// List of xml versions
      /// </summary>
      public enum Version
      {
         /// <summary></summary>
         GEOSOFT_XML_1_0,

         /// <summary></summary>
         GEOSOFT_XML_1_1
      };

      #region Member Variables
      /// <summary>
      /// The url of the dap server
      /// </summary>
      protected String  m_strUrl;   

      /// <summary>
      /// Geosoft XML parser
      /// </summary>
      protected Parse            m_hParse;

      /// <summary>
      /// Geosoft XML encoder
      /// </summary>
      protected EncodeRequest    m_hEncodeRequest;

      /// <summary>
      /// Control whether to communicate through task or xml over http
      /// </summary>
      protected bool             m_bTask;
      
      /// <summary>
      /// Communication stream
      /// </summary>
      protected Xml.Communication   m_oCommunication;   

      /// <summary>
      /// Create a reader/writer lock
      /// </summary>
      protected System.Threading.ReaderWriterLock  m_oLock = new System.Threading.ReaderWriterLock();
      #endregion

      #region Properties
      /// <summary>
      /// Get or set the url of the dap server
      /// </summary>
      public String Url
      {
         get { return m_strUrl; }
      }

      /// <summary>
      /// Get the Geosoft XML parser
      /// </summary>
      public Parse Parser
      {
         get { return m_hParse; }
      }

      /// <summary>
      /// Get or set the xml version to send to the server
      /// </summary>
      public Version XmlVersion
      {
         get { 
            Version eVersion;
            m_oLock.AcquireReaderLock(0);
            eVersion = m_hEncodeRequest.Version; 
            m_oLock.ReleaseReaderLock();
            return eVersion;
         }
      }

      /// <summary>
      /// Get or set a value indicating whether this request should be encrypted
      /// </summary>
      public bool Secure
      {
         get { 
            bool bSecure;
            m_oLock.AcquireReaderLock(0);
            bSecure = m_oCommunication.Secure; 
            m_oLock.ReleaseReaderLock();
            return bSecure;
         }
      }

      /// <summary>
      /// Get/set the user name
      /// </summary>
      public string UserName
      {
         get { 
            string str;
            m_oLock.AcquireReaderLock(0);
            str = m_hEncodeRequest.UserName; 
            m_oLock.ReleaseReaderLock();
            return str;
         }
      }

      /// <summary>
      /// Get/set the user's password
      /// </summary>
      public string Password
      { 
         get 
         { 
            string str;
            m_oLock.AcquireReaderLock(0);
            str = m_hEncodeRequest.Password; 
            m_oLock.ReleaseReaderLock();
            return str;
         }
      }
      #endregion

      #region Constructor
      /// <summary>
      /// Default constructor
      /// </summary>
      public Command()
      {
         m_hParse = new Parse();
         m_hEncodeRequest = new EncodeRequest(Version.GEOSOFT_XML_1_0);
         m_oCommunication = new Communication(false, false);
      }    

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="szUrl"></param>
      /// <param name="bTask"></param>
      public Command(String szUrl, bool bTask)
      {
         m_strUrl = szUrl;
         m_hParse = new Parse(szUrl);
         m_hEncodeRequest = new EncodeRequest(Version.GEOSOFT_XML_1_0);
         m_oCommunication = new Communication(bTask, false);
      }         

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="szUrl"></param>
      /// <param name="bTask"></param>
      /// <param name="eVersion"></param>
      public Command(String szUrl, bool bTask, Version eVersion)
      {
         m_strUrl = szUrl;
         m_hParse = new Parse(szUrl);
         m_hEncodeRequest = new EncodeRequest(eVersion);
         m_oCommunication = new Communication(bTask, false);
      }

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="szUrl"></param>
      /// <param name="bTask"></param>
      /// <param name="eVersion"></param>
      /// <param name="bSecure"></param>
      /// <param name="strPassword"></param>
      /// <param name="strUserName"></param>
      public Command(String szUrl, bool bTask, Version eVersion, bool bSecure, string strUserName, string strPassword)
      {
         m_strUrl = szUrl;
         m_hParse = new Parse(szUrl);
         m_hEncodeRequest = new EncodeRequest(eVersion, strUserName, strPassword);
         m_oCommunication = new Communication(bTask, bSecure);
      }      
      #endregion

      #region Member Functions
      /// <summary>
      /// Change the version of this server
      /// </summary>
      /// <param name="eVersion"></param>
      public void ChangeVersion(Version eVersion)
      {
         m_oLock.AcquireWriterLock(0);
         m_hEncodeRequest.Version = eVersion;
         m_oLock.ReleaseWriterLock();
      }

      /// <summary>
      /// Change login information for this server
      /// </summary>
      /// <param name="strUserName"></param>
      /// <param name="strPassword"></param>
      public void ChangeLogin(string strUserName, string strPassword)
      {
         m_oLock.AcquireWriterLock(0);
         m_hEncodeRequest.UserName = strUserName;
         m_hEncodeRequest.Password = strPassword;
         m_oLock.ReleaseWriterLock();
      }

      /// <summary>
      /// Change whether we send these packets as secure or not
      /// </summary>
      /// <param name="bSecure"></param>
      public void ChangeSecureConnection(bool bSecure)
      {
         m_oLock.AcquireWriterLock(0);
         m_oCommunication.Secure = bSecure;
         m_oLock.ReleaseWriterLock();
      }

      /// <summary>
      /// Authenticate the user in GeosoftXML format from the dap server
      /// </summary>
      /// <returns>True/False depending if the user is authenticated or not</returns>
      public bool AuthenticateUser(string strUserName, string strPassword, UpdateProgessCallback progressCallBack)
      {
         bool                    bAuthenticated = false;
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.AuthenticateUser(null, strUserName, strPassword);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
            bAuthenticated = m_hParse.AuthenticateUser(hResponseDocument);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return bAuthenticated;
      }

      /// <summary>
      /// Get the configuration meta in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetConfiguration(UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.Configuration(null);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the configuration meta in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="progressCallBack"></param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetServerConfiguration(string strPassword, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.ServerConfiguration(null, strPassword);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Update the configuration meta in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="oConfiguration">The configuration xml to append to this request</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument UpdateServerConfiguration(string strPassword, System.Xml.XmlNode oConfiguration, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.UpdateServerConfiguration(null, strPassword, oConfiguration);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }


      /// <summary>
      /// Get the capabilities structure from the dap server
      /// </summary>
      /// <param name="hCapabilities">The capabilities of the dap server</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCapabilities(out Capabilities hCapabilities, UpdateProgessCallback progressCallBack)
      {
         System.Xml.XmlDocument  hResponse = null;

         hCapabilities = null;

         hResponse = GetCapabilities(progressCallBack);         
         hCapabilities = new Capabilities(hResponse);         
      }

      /// <summary>
      /// Get the capabilities in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The capabilities response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetCapabilities(UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CAPABILITIES);

            hRequestDocument = m_hEncodeRequest.Capabilities( null );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the list of properties
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list of properties in GeosoftXML</returns>
      public System.Xml.XmlDocument GetProperties(UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.PROPERTIES);

            hRequestDocument = m_hEncodeRequest.Properties( null );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the list of properties
      /// </summary>
      /// <param name="hArray">The list of properties</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetProperties(out System.Collections.SortedList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument  hResponse = null;

         hResponse = GetProperties(progressCallBack);     
         m_hParse.Properties(hResponse, out hArray);         
      }

      /// <summary>
      /// Get a list of all the datasets stored by the dap server
      /// Note: No hierarchy information is retained
      /// </summary>
      /// <param name="szPath">A path to filter the result set by. Eg. /folder1/folder2/@datasetname</param>
      /// <param name="iStartIndex">The index of the first dataset to return</param>
      /// <param name="iMaxResults">The maximum number of datasets to return</param>
      /// <param name="szKeywords">Filter results based on these keywords. NOTE: Each keyword is seperated by a space</param>
      /// <param name="hBoundingBox">Filter results based on this bounding box</param>
      /// <param name="iLevels">Number of levels to recurse down</param>
      /// <param name="hDataSets">The list of datasets</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCatalog(string szPath, int iStartIndex, int iMaxResults, string szKeywords, BoundingBox hBoundingBox, int iLevels, out System.Collections.ArrayList hDataSets, UpdateProgessCallback progressCallBack)
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CATALOG);

            hRequestDocument = m_hEncodeRequest.Catalog( null, false, szPath, iStartIndex, iMaxResults, szKeywords, hBoundingBox, iLevels);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack); 
 
            m_hParse.Catalog(hResponseDocument, out hDataSets);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
      }

      /// <summary>
      /// Get a list of the datasets stored by the dap server
      /// Note: No hierarchy information is retained
      /// </summary>
      /// <param name="szPath">A path to filter the result set by. Eg. /folder1/folder2/@datasetname</param>
      /// <param name="hDataSets">The list of datasets</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCatalog(string szPath, out System.Collections.ArrayList hDataSets, UpdateProgessCallback progressCallBack)
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CATALOG);

            hRequestDocument = m_hEncodeRequest.Catalog( null, false, szPath, 0, 0, null, null, 0);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack); 

            m_hParse.Catalog(hResponseDocument, out hDataSets);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
      }

      /// <summary>
      /// Get a list of all the datasets stored by the dap server
      /// Note: No hierarchy information is retained
      /// </summary>
      /// <param name="hDataSets">The list of datasets</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCatalog(out System.Collections.ArrayList hDataSets, UpdateProgessCallback progressCallBack)
      {
         System.Xml.XmlDocument  hResponse = null;

         hResponse = GetCatalog(progressCallBack);         
         m_hParse.Catalog(hResponse, out hDataSets);         
      }      

      /// <summary>
      /// Get the catalog in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The catalog response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetCatalog(UpdateProgessCallback progressCallBack) 
      {
         return GetCatalog(null, 0, 0, null, null, 0, progressCallBack);         
      }

      /// <summary>
      /// Get the catalog in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="szPath">A path to filter the result set by. Eg. /folder1/folder2/@datasetname</param>
      /// <param name="iStartIndex">The index of the first dataset to return</param>
      /// <param name="iMaxResults">The maximum number of datasets to return</param>
      /// <param name="szKeywords">Filter results based on these keywords. NOTE: Each keyword is seperated by a space</param>
      /// <param name="hBoundingBox">Filter results based on this bounding box</param>
      /// <param name="iLevel">Number of levels to recurse down</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The catalog response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetCatalog(string szPath, int iStartIndex, int iMaxResults, string szKeywords, BoundingBox hBoundingBox, int iLevel, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CATALOG);

            hRequestDocument = m_hEncodeRequest.Catalog( null, false, szPath, iStartIndex, iMaxResults, szKeywords, hBoundingBox, iLevel);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Inform the dap server that it must refresh its catalog
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>True/False</returns>
      public bool RefreshCatalog(string strPassword, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         bool                    bRet = false;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CATALOG);
            hRequestDocument = m_hEncodeRequest.RefreshCatalog(null, strPassword);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
            bRet = m_hParse.RefreshCatalog(hResponseDocument);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return bRet;
      }

      /// <summary>
      /// Get the number of datasets that match this query
      /// </summary>
      /// <param name="szPath">A path to filter the result set by. Eg. /folder1/folder2/@datasetname</param>
      /// <param name="iStartIndex">The index of the first dataset to return</param>
      /// <param name="iMaxResults">The maximum number of datasets to return</param>
      /// <param name="szKeywords">Filter results based on these keywords. NOTE: Each keyword is seperated by a space</param>
      /// <param name="hBoundingBox">Filter results based on this bounding box</param>
      /// <param name="iLevels">Number of levels to recurse down</param>
      /// <param name="iNumDataSets">The number of datasets</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetDataSetCount( string szPath, int iStartIndex, int iMaxResults, string szKeywords, BoundingBox hBoundingBox, int iLevels, out Int32 iNumDataSets, UpdateProgessCallback progressCallBack)
      {
         System.Xml.XmlDocument  hResponseDocument;
         
         iNumDataSets = 0;

         hResponseDocument = GetDataSetCount(szPath, iStartIndex, iMaxResults, szKeywords, hBoundingBox, iLevels, progressCallBack);  
         m_hParse.DataSetCount(hResponseDocument, out iNumDataSets);         
      }

      /// <summary>
      /// Get the number of datasets that match this query
      /// </summary>
      /// <param name="szPath">A path to filter the result set by. Eg. /folder1/folder2/@datasetname</param>
      /// <param name="iStartIndex">The index of the first dataset to return</param>
      /// <param name="iMaxResults">The maximum number of datasets to return</param>
      /// <param name="szKeywords">Filter results based on these keywords. NOTE: Each keyword is seperated by a space</param>
      /// <param name="hBoundingBox">Filter results based on this bounding box</param>
      /// <param name="iLevel">Number of levels to recurse down</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The catalog response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetDataSetCount(string szPath, int iStartIndex, int iMaxResults, string szKeywords, BoundingBox hBoundingBox, int iLevel, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CATALOG);

            hRequestDocument = m_hEncodeRequest.Catalog( null, true, szPath, iStartIndex, iMaxResults, szKeywords, hBoundingBox, iLevel );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the catalog edition in GeosoftXML format from the dap server
      /// </summary>
      /// <returns>The catalog edition response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetCatalogEdition(UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.CATALOG_EDITION);

            hRequestDocument = m_hEncodeRequest.CatalogEdition( null );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the catalog edition from the dap server
      /// </summary>
      /// <param name="strConfigurationEdition">The edition of the server</param>
      /// <param name="strEdition">The edition of the catalog</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCatalogEdition(out string strConfigurationEdition, out string strEdition, UpdateProgessCallback progressCallBack) 
      {         
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetCatalogEdition(progressCallBack);      
 
         m_hParse.CatalogEdition(hResponseDocument, out strConfigurationEdition, out strEdition);
      }

      /// <summary>
      /// Get the catalog edition from the dap server
      /// </summary>
      /// <param name="strEdition">The edition of the catalog</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetCatalogEdition(out string strEdition, UpdateProgessCallback progressCallBack) 
      {         
         string strConfigurationEdition;

         GetCatalogEdition(out strConfigurationEdition, out strEdition, progressCallBack);
      }

      /// <summary>
      /// Get the list of keywords in GeosoftXML format from the dap server
      /// </summary>
      /// <returns>The keywords response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetKeywords(UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.KEYWORDS);

            hRequestDocument = m_hEncodeRequest.Keywords( null );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the list of keywords from the dap server
      /// </summary>
      /// <param name="hKeywords">The list of keywords</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetKeywords(out ArrayList hKeywords, UpdateProgessCallback progressCallBack) 
      {         
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetKeywords(progressCallBack);      
 
         m_hParse.Keywords(hResponseDocument, out hKeywords);
      }

      /// <summary>
      /// Get the dataset edition in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="hDataSet">The dataset to get the edition of</param>
      /// <returns>The dataset edition response in GeosoftXML</returns>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public System.Xml.XmlDocument GetDataSetEdition(DataSet hDataSet, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         
         // --- check to make sure that the dataset is located on the server this command is connected to ---

         if (hDataSet.Url != Url)
            throw new DapException("The dataset is not located on this server.");

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.DATASET_EDITION);

            hRequestDocument = m_hEncodeRequest.DatasetEdition( null, hDataSet.Name );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the dataset edition from the dap server
      /// </summary>
      /// <param name="hDataSet">The dataset to get the edition of</param>
      /// <param name="szEdition">The edition of the dataset</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetDataSetEdition(DataSet hDataSet, out String szEdition, UpdateProgessCallback progressCallBack) 
      {         
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetDataSetEdition(hDataSet, progressCallBack);      
 
         m_hParse.DataSetEdition( hResponseDocument, out szEdition );
      }

      /// <summary>
      /// Get meta information for a particular dataset
      /// </summary>
      /// <param name="hDataSet">The dataset to retrieve meta information for</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The meta response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetMetaData(DataSet hDataSet, UpdateProgessCallback progressCallBack) 
      {
         // --- check to make sure that the dataset is located on the server this command is connected to ---

         if (hDataSet.Url != Url)
            throw new DapException("The dataset is not located on this server.");

         return GetMetaData( hDataSet.Name, progressCallBack);
      }

      /// <summary>
      /// Get meta information for a particular dataset
      /// </summary>
      /// <param name="hItem">The item element within a catalog response</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The meta response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetMetaData(System.Xml.XmlNode hItem, UpdateProgessCallback progressCallBack) 
      {
         if (hItem == null || hItem.Name != Constant.Tag.ITEM_TAG) throw new DapException("Invalid item element");


         // --- get the dataset name ---

         System.Xml.XmlNode hAttr = hItem.Attributes.GetNamedItem("name");
         if (hAttr == null) throw new DapException("Missing name attribute in item element");

         return GetMetaData( hAttr.Value, progressCallBack);         
      }

      /// <summary>
      /// Get the meta information for a paricular dataset
      /// </summary>
      /// <param name="szDataSet">The unique name of the dataset</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The meta response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetMetaData(string szDataSet, UpdateProgessCallback progressCallBack)
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.META);

            hRequestDocument = m_hEncodeRequest.Metadata( null, szDataSet );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the image for a particular dataset from the catalog item element
      /// </summary>
      /// <param name="hItem">The item element within a catalog response</param>
      /// <param name="hFormat">The format to retrieve the image in</param>
      /// <param name="hResolution">The resolution to retrieve the image at</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The image response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetImage(System.Xml.XmlNode hItem, Format hFormat, Resolution hResolution, UpdateProgessCallback progressCallBack) 
      {
         DataSet           hDataSet;
         BoundingBox       hBoundingBox;
         

         // --- parse the data set element ---

         m_hParse.DataSet(hItem, out hDataSet);

         
         // --- parse the bounding box element ---
         
         m_hParse.BoundingBox(hItem.FirstChild, out hBoundingBox);

         hDataSet.Boundary = hBoundingBox;

         return GetImage(hDataSet, hFormat, hResolution, progressCallBack);
         
      }
      
      /// <summary>
      /// Get an image from a dataset
      /// </summary>
      /// <param name="hDataSet">The dataset to get an image for</param>
      /// <param name="hFormat">The format to retrieve the image in</param>
      /// <param name="hResolution">The resolution to retrieve the image at</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The image response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetImage(DataSet hDataSet, Format hFormat, Resolution hResolution, UpdateProgessCallback progressCallBack) 
      {                  
         // --- check to make sure that the dataset is located on the server this command is connected to ---

         if (hDataSet.Url != Url)
            throw new DapException("The dataset is not located on this server.");


         System.Collections.ArrayList  hArrayList = new System.Collections.ArrayList();
         hArrayList.Add( hDataSet.Name );         
         return GetImage(hFormat, hDataSet.Boundary, hResolution, hArrayList, progressCallBack);
      }
      
      /// <summary>
      /// Get an image from a list of dataset server names
      /// </summary>
      /// <param name="hFormat">The format to retrieve the image in</param>
      /// <param name="hBoundingBox">The bounding box of the image</param>
      /// <param name="hResolution">The resolution to retrive the image at</param>
      /// <param name="hArrayList">The list of dataset server name</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The image response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetImage(Format hFormat, BoundingBox hBoundingBox, Resolution hResolution, System.Collections.ArrayList hArrayList, UpdateProgessCallback progressCallBack) 
      {
         return GetImage(hFormat, hBoundingBox, hResolution, false, false, hArrayList, progressCallBack);
      }

      /// <summary>
      /// Get an image from a list of dataset server names
      /// </summary>
      /// <param name="hFormat">The format to retrieve the image in</param>
      /// <param name="hBoundingBox">The bounding box of the image</param>
      /// <param name="hResolution">The resolution to retrive the image at</param>
      /// <param name="bBaseMap">Draw the base map</param>
      /// <param name="bIndexMap">Draw the index map</param>
      /// <param name="hArrayList">The list of dataset server names</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The image response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetImage(Format hFormat, BoundingBox hBoundingBox, Resolution hResolution, bool bBaseMap, bool bIndexMap, System.Collections.ArrayList hArrayList, UpdateProgessCallback progressCallBack) 
      {                  
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;

         try 
         {            
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.IMAGE);

            hRequestDocument = m_hEncodeRequest.Image( null, hFormat, hBoundingBox, hResolution, bBaseMap, bIndexMap, hArrayList);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get an image from a list of Geosoft.Dap.Command.DataSet classes
      /// </summary>
      /// <param name="hFormat">The format to retrieve the image in</param>
      /// <param name="hBoundingBox">The bounding box of the image</param>
      /// <param name="hResolution">The resolution to retrive the image at</param>
      /// <param name="bBaseMap">Draw the base map</param>
      /// <param name="bIndexMap">Draw the index map</param>
      /// <param name="hArrayList">The list of Geosoft.Dap.Command.DataSet classes</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The image response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetImageEx(Format hFormat, BoundingBox hBoundingBox, Resolution hResolution, bool bBaseMap, bool bIndexMap, System.Collections.ArrayList hArrayList, UpdateProgessCallback progressCallBack) 
      {                  
         ArrayList   hList = new ArrayList();
         
         foreach (DataSet hDataSet in hArrayList)
         {
            if (hDataSet.Url == Url)
               hList.Add(hDataSet.Name);
         }

         return GetImage(hFormat, hBoundingBox, hResolution, bBaseMap, bIndexMap, hList, progressCallBack);
      }

      /// <summary>
      /// Get the default resolution to extract this data set at
      /// </summary>
      /// <param name="szType">The dataset type</param>
      /// <param name="hBoundingBox">The bounding box</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The default resolution response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetDefaultResolution(string szType, BoundingBox hBoundingBox, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.DEFAULT_RESOLUTION);

            hRequestDocument = m_hEncodeRequest.DefaultResolution(null, szType, hBoundingBox );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the default resolution to extract this data set at
      /// </summary>
      /// <param name="szType">The dataset type</param>
      /// <param name="hBoundingBox">The bounding box</param>
      /// <param name="szResolution">The default resolution</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetDefaultResolution(string szType, BoundingBox hBoundingBox, out String szResolution, UpdateProgessCallback progressCallBack) 
      {         
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetDefaultResolution(szType, hBoundingBox, progressCallBack);

         m_hParse.DefaultResolution( hResponseDocument, out szResolution );  
      }

      /// <summary>
      /// Get the default resolution to extract this data set at
      /// </summary>
      /// <param name="hDataSet">The dataset</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The default resolution response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetDefaultResolution(DataSet hDataSet, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetDefaultResolution(hDataSet.Type, hDataSet.Boundary, progressCallBack);

         return hResponseDocument;
      }

      /// <summary>
      /// Get the default resolution to extract this data set at
      /// </summary>
      /// <param name="hDataSet">The dataset</param>
      /// <param name="szResolution">The default resolution</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetDefaultResolution(DataSet hDataSet, out String szResolution, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument  hResponseDocument;

         hResponseDocument = GetDefaultResolution(hDataSet, progressCallBack);

         m_hParse.DefaultResolution( hResponseDocument, out szResolution );         
      }

      /// <summary>
      /// Get the list of supported datums
      /// </summary>
      /// <returns>The list of supported datums response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetSupportedDatums(UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.DATUM, null, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported datums
      /// </summary>
      /// <param name="hArray">The list of supported datums</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedDatums(out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.DATUM, null, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported projections
      /// </summary>
      /// <returns>The list of supported projections response in GeosoftXML</returns>
      /// <param name="szDatum">Datum String</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public System.Xml.XmlDocument GetSupportedProjections(string szDatum, UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.PROJECTION, szDatum, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported projections
      /// </summary>
      /// <param name="szDatum">The datum to filter the projections by. NOTE: can be null</param>
      /// <param name="hArray">The list of supported projections</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedProjections(string szDatum, out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.PROJECTION, szDatum, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported units
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list of supported units response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetSupportedUnits(UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.UNITS, null, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported units
      /// </summary>
      /// <param name="hArray">The list of supported units</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedUnits(out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.UNITS, null, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <returns>The list of supported local datum descriptions response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetSupportedLocalDatumDescriptions(UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_DESCRIPTION, null, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="szDatum">The datum to filter the results</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list of supported local datum descriptions response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetSupportedLocalDatumDescriptions(string szDatum, UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_DESCRIPTION, szDatum, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="hArray">The list of supported local datum descriptions</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedLocalDatumDescriptions(out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_DESCRIPTION, null, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="szDatum">The datum to filter the results</param>
      /// <param name="hArray">The list of supported local datum descriptions</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedLocalDatumDescriptions(string szDatum, out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_DESCRIPTION, szDatum, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="szDatum">The datum to filter the results</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list of supported local datum names in GesoftXML</returns>
      public System.Xml.XmlDocument GetSupportedLocalDatumNames(string szDatum, UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_NAME, szDatum, progressCallBack);
      }      

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list of supported local datum names in GesoftXML</returns>
      public System.Xml.XmlDocument GetSupportedLocalDatumNames(UpdateProgessCallback progressCallBack) 
      {
         return GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_NAME, null, progressCallBack);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="hArray">The list of supported local datum names</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedLocalDatumNames(out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_NAME, null, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported local datums
      /// </summary>
      /// <param name="szDatum">The datum to filter the results</param>
      /// <param name="hArray">The list of supported local datum names</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void GetSupportedLocalDatumNames(string szDatum, out System.Collections.ArrayList hArray, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument hDoc = GetSupportedCoordinateSystems(CoordinateSystem.Types.LOCAL_DATUM_NAME, szDatum, progressCallBack);
         m_hParse.SupportedCoordinateSystem(hDoc, out hArray);
      }

      /// <summary>
      /// Get the list of supported coordinate systems
      /// </summary>
      /// <param name="eType">The list type. Must be one of DATUM, PROJECTION, UNITS, LOCAL_DATUM_NAME or LOCAL_DATUM_DESCRIPTION</param>
      /// <param name="szDatum">The datum to filter the results</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The list response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetSupportedCoordinateSystems(CoordinateSystem.Types eType, string szDatum, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.SUPPORTED_COORDINATE_SYSTEMS);

            hRequestDocument = m_hEncodeRequest.CoordinateSystemList( null, CoordinateSystem.TYPES[Convert.ToInt32(eType)], szDatum );
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Begin the extraction process for a particular dataset
      /// </summary>
      /// <param name="hDataSet">The dataset to extract</param>
      /// <param name="hBox">The region to extract</param>
      /// <param name="bNative">Download in native coordinate system or that of bounding box</param>
      /// <param name="szKey">The extraction key</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void Extract(ExtractDataSet hDataSet, BoundingBox hBox, bool bNative, out string szKey, UpdateProgessCallback progressCallBack) 
      {                  
         System.Xml.XmlDocument        hResponseDocument;
         
         hResponseDocument = Extract(hDataSet, hBox, bNative, progressCallBack);
         m_hParse.ExtractKey(hResponseDocument, out szKey);         
      }

      /// <summary>
      /// Begin the extraction process for a particular dataset
      /// </summary>
      /// <param name="hDataSet">The dataset to extract</param>
      /// <param name="hBox">The region to extract</param>
      /// <param name="bNative">Download in native coordinate system or that of bounding box</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The extract response in GeosoftXML</returns>
      public System.Xml.XmlDocument Extract(ExtractDataSet hDataSet, BoundingBox hBox, bool bNative, UpdateProgessCallback progressCallBack) 
      {                 
         ArrayList         hList = new ArrayList();

         hList.Add(hDataSet);
         return Extract(hList, hBox, bNative, progressCallBack);
      }

      /// <summary>
      /// Begin the extraction process for a collection of datasets
      /// </summary>
      /// <param name="hDataSetList">The list of datasets to extract</param>
      /// <param name="hBox">The projection to extract the datasets to</param>
      /// <param name="bNative">Save datasets in native coordinate system or that of the bounding box</param>
      /// <param name="szKey">The extraction key</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void Extract(ArrayList hDataSetList, BoundingBox hBox, bool bNative, out string szKey, UpdateProgessCallback progressCallBack) 
      {                  
         System.Xml.XmlDocument        hResponseDocument;

         hResponseDocument = Extract(hDataSetList, hBox, bNative, progressCallBack);
         m_hParse.ExtractKey(hResponseDocument, out szKey);         
      }

      /// <summary>
      /// Begin the extraction process for a particular dataset
      /// </summary>
      /// <param name="hDataSetList">The list of datasets to extract</param>
      /// <param name="hBox">The projection to extract the datasets to</param>
      /// <param name="bNative">Save datasets in native coordinate system or that of the bounding box</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The extract response in GeosoftXML</returns>
      public System.Xml.XmlDocument Extract(ArrayList hDataSetList, BoundingBox hBox, bool bNative, UpdateProgessCallback progressCallBack) 
      {                  
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.EXTRACT);

            hRequestDocument = m_hEncodeRequest.Extract(null, hDataSetList, hBox, bNative);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }
      
      /// <summary>
      /// Get the extract progress from the dap server
      /// </summary>
      /// <param name="szKey">The extraction key</param>
      /// <param name="eStatus">The status of the extraction</param>
      /// <param name="iProgress">The percent complete</param>
      /// <param name="szStatus">The current task</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void ExtractProgress(string szKey, out Constant.ExtractStatus eStatus, out Int32 iProgress, out string szStatus, UpdateProgessCallback progressCallBack) 
      {                  
         System.Xml.XmlDocument        hResponseDocument;

         hResponseDocument = ExtractProgress(szKey, progressCallBack);
         m_hParse.ExtractProgress(hResponseDocument, out eStatus, out iProgress, out szStatus);
      }

      /// <summary>
      /// Get the extract progress from the dap server
      /// </summary>
      /// <param name="szKey">The extraction key</param>
      /// <param name="eStatus">The status of the extraction</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns></returns>
      public void ExtractProgress(string szKey, out Constant.ExtractStatus eStatus, UpdateProgessCallback progressCallBack) 
      {                  
         System.Xml.XmlDocument        hResponseDocument;
         Int32                         iProgress;
         string                        szStatus;

         hResponseDocument = ExtractProgress(szKey, progressCallBack);
         m_hParse.ExtractProgress(hResponseDocument, out eStatus, out iProgress, out szStatus);
      }         

      /// <summary>
      /// Get the extract progress from the dap server
      /// </summary>
      /// <param name="szKey">The extraction key</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The extract progress response in GeosoftXML</returns>
      public System.Xml.XmlDocument ExtractProgress(string szKey, UpdateProgessCallback progressCallBack) 
      {                  
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;         

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.EXTRACT);

            hRequestDocument = m_hEncodeRequest.ExtractProgress(null, szKey);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the extracted data
      /// </summary>
      /// <param name="szKey">The extraction key</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The extact data response in GeosoftXML</returns>
      public System.Xml.XmlDocument ExtractData(string szKey, UpdateProgessCallback progressCallBack) 
      {                  
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;         

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.EXTRACT);

            hRequestDocument = m_hEncodeRequest.ExtractData(null, szKey);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Translate a series of coordinates into a new projection
      /// </summary>
      /// <param name="hICS">The input coordinate system</param>
      /// <param name="hOCS">The output coordinate system</param>
      /// <param name="hItems">The list of points to translate</param>
      /// <param name="hOutItems">The translate points</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void TranslateCoordinates(CoordinateSystem hICS,
                                       CoordinateSystem hOCS,
                                       System.Collections.ArrayList hItems,
                                       out System.Collections.ArrayList hOutItems, UpdateProgessCallback progressCallBack) 
      {                  
         System.Xml.XmlDocument        hResponseDocument;
         
         hResponseDocument = TranslateCoordinates(hICS, hOCS, hItems, progressCallBack);
         m_hParse.TranslateCoordinates(hResponseDocument, out hOutItems);
      }  

      /// <summary>
      /// Translate a series of coordinates to the new projection
      /// </summary>      
      /// <param name="hInputCoordinateSystem">The input coordinate system</param>
      /// <param name="hOutputCoordinateSystem">The output coordinate system</param>
      /// <param name="hItems">The list of points to translate</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The translated coordinates response in GeosoftXML</returns>
      public System.Xml.XmlDocument TranslateCoordinates(CoordinateSystem hInputCoordinateSystem,
                                                         CoordinateSystem hOutputCoordinateSystem,
                                                         System.Collections.ArrayList hItems, UpdateProgessCallback progressCallBack)
      {
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;         

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.TRANSLATE);

            hRequestDocument = m_hEncodeRequest.TranslateCoordinates(null, hInputCoordinateSystem, hOutputCoordinateSystem, hItems);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Translate a bounding box into a new projection
      /// </summary>
      /// <param name="hBoundingBox">The bounding box to translate</param>
      /// <param name="hOCS">The output coordinate system</param>
      /// <param name="dResolution">The extraction resolution</param>
      /// <param name="hOutBoundingBox">The translated bounding box</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      public void TranslateBoundingBox(BoundingBox hBoundingBox, CoordinateSystem hOCS, ref Double dResolution, out BoundingBox hOutBoundingBox, UpdateProgessCallback progressCallBack)         
      {                  
         System.Xml.XmlDocument        hResponseDocument;

         hResponseDocument = TranslateBoundingBox(hBoundingBox, dResolution, hOCS, progressCallBack);
         m_hParse.TranslateBoundingBox(hResponseDocument, out hOutBoundingBox, out dResolution);
      }  

      /// <summary>
      /// Translate a bounding box into a new projection
      /// </summary>
      /// <param name="hBoundingBox">The bounding box to translate</param>
      /// <param name="hOCS">The output coordinate system</param>
      /// <param name="hOutBoundingBox">The translated bounding box</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The translated bounding box response in GeosoftXML</returns>
      public void TranslateBoundingBox(BoundingBox hBoundingBox, CoordinateSystem hOCS, out BoundingBox hOutBoundingBox, UpdateProgessCallback progressCallBack)
      {
         System.Xml.XmlDocument        hResponseDocument;       
         Double                        dResolution;

         hResponseDocument = TranslateBoundingBox(hBoundingBox, 0, hOCS, progressCallBack);
         m_hParse.TranslateBoundingBox(hResponseDocument, out hOutBoundingBox, out dResolution);         
      }

      /// <summary>
      /// Translate a bounding box into a new projection
      /// </summary>
      /// <param name="hBoundingBox">The bounding box to translate</param>
      /// <param name="hOCS">The output coordinate system</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The translated bounding box response in GeosoftXML</returns>
      public System.Xml.XmlDocument TranslateBoundingBox(BoundingBox hBoundingBox, CoordinateSystem hOCS, UpdateProgessCallback progressCallBack)
      {
         System.Xml.XmlDocument        hResponseDocument;         

         hResponseDocument = TranslateBoundingBox(hBoundingBox, 0, hOCS, progressCallBack);
         return hResponseDocument;
      }

      /// <summary>
      /// Translate a bounding box into a new projection
      /// </summary>
      /// <param name="hBoundingBox">The bounding box to translate</param>      
      /// <param name="dResolution">The extraction resolution</param>
      /// <param name="hOCS">The output coordinate system</param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The translated bounding box in GeosoftXML</returns>
      public System.Xml.XmlDocument TranslateBoundingBox(BoundingBox hBoundingBox, Double dResolution, CoordinateSystem hOCS, UpdateProgessCallback progressCallBack)
      {     
         string                        szUrl;
         System.Xml.XmlDocument        hRequestDocument;
         System.Xml.XmlDocument        hResponseDocument;         

         try 
         {
            m_oLock.AcquireReaderLock(0);
            szUrl = CreateUrl(Constant.Request.TRANSLATE);

            hRequestDocument = m_hEncodeRequest.TranslateBoundingBox(null,hBoundingBox,hOCS, dResolution);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// Get the log in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetLog(string strPassword, UpdateProgessCallback progressCallBack) 
      {
         return GetLog(strPassword, DateTime.Today, progressCallBack);
      }

      /// <summary>
      /// Get the log in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="oDate"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument GetLog(string strPassword, DateTime oDate, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.GetLog(null, strPassword, oDate);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// clear the log in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument ClearLog(string strPassword, UpdateProgessCallback progressCallBack) 
      {
         return ClearLog(strPassword, DateTime.Today, progressCallBack);
      }

      /// <summary>
      /// clear the log in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="oDate"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument ClearLog(string strPassword, DateTime oDate, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.ClearLog(null, strPassword, oDate);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }

      /// <summary>
      /// list the logs in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="oLogs"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public void ListLogs(string strPassword, out ArrayList oLogs, UpdateProgessCallback progressCallBack) 
      {
         System.Xml.XmlDocument oDoc;

         oDoc = ListLogs(strPassword, progressCallBack);
         m_hParse.ListLogs(oDoc, out oLogs);
      }

      /// <summary>
      /// list the logs in GeosoftXML format from the dap server
      /// </summary>
      /// <param name="strPassword"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>The configuration response in GeosoftXML</returns>
      public System.Xml.XmlDocument ListLogs(string strPassword, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CONFIGURATION);
            hRequestDocument = m_hEncodeRequest.ListLogs(null, strPassword);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return hResponseDocument;
      }
      
      /// <summary>
      /// Create the url, base of the xml version and whether it is secure or not
      /// </summary>
      /// <param name="eRequest"></param>
      /// <returns></returns>
      public string CreateUrl(Constant.Request eRequest)
      {
         m_oLock.AcquireReaderLock(0);

         string strUrl = m_strUrl;

         switch (m_hEncodeRequest.Version)
         {
            case Version.GEOSOFT_XML_1_0:
               strUrl += Constant.ServerV1[Convert.ToInt32(eRequest)];
               break;
            case Version.GEOSOFT_XML_1_1:
               strUrl += Constant.ServerV1_1[Convert.ToInt32(eRequest)];
               break;
         }

         if (m_oCommunication.Secure && eRequest != Constant.Request.CONFIGURATION)
            strUrl += Constant.RequestXmlSecure;
         else 
            strUrl += Constant.RequestXmlNormal;

         m_oLock.ReleaseReaderLock();

         return strUrl;
      }

      /// <summary>
      /// Open up a client state object on the server
      /// </summary>
      /// <param name="strKey"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>True/False</returns>
      public bool CreateClientState(out string strKey, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CATALOG);
            hRequestDocument = m_hEncodeRequest.CreateClientState(null);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);
            strKey = m_hParse.CreateClientState(hResponseDocument);
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return true;
      }

      /// <summary>
      /// Destroy a client state object on the server
      /// </summary>
      /// <param name="strKey"></param>
      /// <param name="progressCallBack">Progress handler (may be null)</param>
      /// <returns>True/False</returns>
      public bool DestroyClientState(string strKey, UpdateProgessCallback progressCallBack) 
      {
         string                  szUrl;
         System.Xml.XmlDocument  hRequestDocument;
         System.Xml.XmlDocument  hResponseDocument;
         
         try 
         {
            m_oLock.AcquireReaderLock(0);

            szUrl = CreateUrl(Constant.Request.CATALOG);
            hRequestDocument = m_hEncodeRequest.DestroyClientState(null, strKey);
            hResponseDocument = m_oCommunication.Send(szUrl, hRequestDocument, progressCallBack);  
         }
         finally
         {
            m_oLock.ReleaseReaderLock();
         }
         return true;
      }
      #endregion
   }
}