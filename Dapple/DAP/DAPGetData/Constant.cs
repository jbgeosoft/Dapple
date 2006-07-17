using System;

namespace Geosoft.GX.DAPGetData
{
   /// <summary>
   /// Summary description for Constant.
   /// </summary>
   public class Constant
   {
      public Constant()
      {
      }

      /// <summary>
      /// Reproject the src bounding box into the destination coordinate system
      /// </summary>
      /// <param name="hSrcBB"></param>
      /// <param name="hDestCS"></param>
      /// <returns></returns>
      static public bool Reproject(Geosoft.Dap.Common.BoundingBox  hSrcBB, Geosoft.Dap.Common.CoordinateSystem hDestCS)
      {
#if !DAPPLE
         Geosoft.GXNet.CIPJ   hSrcIPJ = null; 
         Geosoft.GXNet.CIPJ   hDestIPJ = null;
         Geosoft.GXNet.CPJ    hPJ = null;

         double               dMinX;
         double               dMinY;
         double               dMaxX;
         double               dMaxY;

         String               strProjectionName = String.Empty;
         String               strDatum = String.Empty;
         String               strProjection = String.Empty;
         String               strUnits = String.Empty;
         String               strLocalDatum = String.Empty;

         // --- All dummies therefore will be dummies in new coordinate sytem as well ---

         if (hSrcBB.MinX == Geosoft.GXNet.Constant.rDUMMY || 
            hSrcBB.MinY == Geosoft.GXNet.Constant.rDUMMY || 
            hSrcBB.MaxX == Geosoft.GXNet.Constant.rDUMMY || 
            hSrcBB.MaxY == Geosoft.GXNet.Constant.rDUMMY)
         {
            return true;
         }


         try
         {
            hSrcIPJ = Geosoft.GXNet.CIPJ.Create();
            hDestIPJ = Geosoft.GXNet.CIPJ.Create();

            hSrcIPJ.SetGXF("", hSrcBB.CoordinateSystem.Datum, hSrcBB.CoordinateSystem.Method, hSrcBB.CoordinateSystem.Units, hSrcBB.CoordinateSystem.LocalDatum);         
            hDestIPJ.SetGXF("", hDestCS.Datum, hDestCS.Method, hDestCS.Units, hDestCS.LocalDatum);       
         

            // --- check to see if this is a valid coordinate system for the given coordinates ---

            hPJ = Geosoft.GXNet.CPJ.CreateIPJ(hSrcIPJ, hDestIPJ);
            dMinX = hSrcBB.MinX;
            dMinY = hSrcBB.MinY;
            dMaxX = hSrcBB.MaxX;
            dMaxY = hSrcBB.MaxY;

            hPJ.ProjectBoundingRectangle(ref dMinX, ref dMinY, ref dMaxX, ref dMaxY);

            if (dMinX == Geosoft.GXNet.Constant.rDUMMY || 
               dMinY == Geosoft.GXNet.Constant.rDUMMY || 
               dMaxX == Geosoft.GXNet.Constant.rDUMMY || 
               dMaxY == Geosoft.GXNet.Constant.rDUMMY)
            {
               return false; // --- unable to convert to the desired coordinate system ---            
            }
        
         
            // --- copy new projection into src bounding box ---

            hDestIPJ.IGetName(Geosoft.GXNet.Constant.IPJ_NAME_DATUM, ref strDatum);
            hDestIPJ.IGetName(Geosoft.GXNet.Constant.IPJ_NAME_METHOD, ref strProjection);
            hDestIPJ.IGetName(Geosoft.GXNet.Constant.IPJ_NAME_LDATUM, ref strLocalDatum);
            hDestIPJ.IGetName(Geosoft.GXNet.Constant.IPJ_NAME_UNIT_FULL, ref strUnits);

            hSrcBB.CoordinateSystem.Projection = hDestCS.Projection;
            hSrcBB.CoordinateSystem.Datum = hDestCS.Datum;
            hSrcBB.CoordinateSystem.Method = hDestCS.Method;
            hSrcBB.CoordinateSystem.Units =hDestCS.Units;
            hSrcBB.CoordinateSystem.LocalDatum = hDestCS.LocalDatum;
            hSrcBB.MinX = dMinX;
            hSrcBB.MinY = dMinY;
            hSrcBB.MaxX = dMaxX;
            hSrcBB.MaxY = dMaxY;         

            return true;
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("Reproject - " + e.Message);
         }
         finally
         {
            if (hPJ != null) hPJ.Dispose();
            if (hSrcIPJ != null) hSrcIPJ.Dispose();
            if (hDestIPJ != null) hDestIPJ.Dispose();
         }
#endif
         return false;         
      }

#if !DAPPLE
      /// <summary>
      /// Create a coordinate system from an ipj and a bounding box
      /// </summary>
      /// <param name="dMinX"></param>
      /// <param name="dMinY"></param>
      /// <param name="dMaxX"></param>
      /// <param name="dMaxY"></param>
      /// <param name="hIPJ"></param>
      /// <returns></returns>
      static public Geosoft.Dap.Common.BoundingBox SetCoordinateSystem(double dMinX, double dMinY, double dMaxX, double dMaxY, Geosoft.GXNet.CIPJ hIPJ)
      {
         Geosoft.Dap.Common.BoundingBox   hBoundingBox = null;
         string                           strProjectionName = string.Empty;
         string                           strDatum = string.Empty;
         string                           strProjection = string.Empty;
         string                           strUnits = string.Empty;
         string                           strLocalDatum = string.Empty;

         hIPJ.IGetGXF(ref strProjectionName, ref strDatum, ref strProjection, ref strUnits, ref strLocalDatum);                  
                  
         hBoundingBox = new Geosoft.Dap.Common.BoundingBox(dMaxX, dMaxY, dMinX, dMinY);
         hBoundingBox.CoordinateSystem = new Geosoft.Dap.Common.CoordinateSystem();
         hBoundingBox.CoordinateSystem.Projection = strProjectionName;
         hBoundingBox.CoordinateSystem.Datum = strDatum;
         hBoundingBox.CoordinateSystem.Method = strProjection;
         hBoundingBox.CoordinateSystem.Units = strUnits;
         hBoundingBox.CoordinateSystem.LocalDatum = strLocalDatum;                 
         return hBoundingBox;
      }
#endif

      /// <summary>
      /// Check to see if this is a valid bounding box
      /// </summary>
      /// <param name="dCoordinate"></param>
      /// <param name="dMin"></param>
      /// <param name="dMax"></param>
      /// <returns></returns>
      static public bool IsValidBoundingBox(Geosoft.Dap.Common.BoundingBox oBox)
      {
         if (oBox.MinX < oBox.MaxX && oBox.MinY < oBox.MaxY)
            return true;
         return false;
      }      

      /// <summary>
      /// Strip any geosoft qualifiers
      /// </summary>
      /// <param name="strUrl"></param>
      /// <returns></returns>
      static public string StripQualifiers(string strPath)
      {
         int iIndex = strPath.LastIndexOf("(");
         int iDotIndex = strPath.LastIndexOf(".");

         if (iIndex == -1 || iIndex < iDotIndex) return strPath;

         string strRet = strPath.Substring(0, iIndex);

         return strRet;
      }

      /// <summary>
      /// Strip any geosoft endings off of the url
      /// </summary>
      /// <param name="strUrl"></param>
      /// <returns></returns>
      static protected string StripGeosoftEnding(string strUrl)
      {
         if (strUrl.EndsWith("/DDS") || strUrl.EndsWith("/DCS") || strUrl.EndsWith("/DFS") || strUrl.EndsWith("/DMS"))
         {
            strUrl = strUrl.Substring(0, strUrl.Length - 4);
         } 
         else if (strUrl.EndsWith("/DDS/") || strUrl.EndsWith("/DCS/") || strUrl.EndsWith("/DFS/") || strUrl.EndsWith("/DMS/"))
         {
            strUrl = strUrl.Substring(0, strUrl.Length - 5);
         }
         return strUrl;
      }

      /// <summary>
      /// Make the url to communicate with the dap server
      /// </summary>
      /// <param name="szUrl"></param>
      /// <returns></returns>
      static public string MakeUrl(string szUrl)
      {
         szUrl = StripGeosoftEnding(szUrl);
         return szUrl + "/DAP"; 
      }
            
      /// <summary>
      /// Format the coordinate to only 2 decimal places
      /// </summary>
      /// <param name="d"></param>
      /// <returns></returns>
      static public string FormatCoordinate(double d)
      {
         return d.ToString("f2");
      }

#if !DAPPLE
      /// <summary>
      /// Set the height and width of this dialog in the ap meta settings
      /// </summary>
      /// <param name="iWidth"></param>
      /// <param name="iHeight"></param>
      static public void SetSizeInSettingsMeta(Int32 iWidth, Int32 iHeight)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iHeightAttribute;
         Int32                iWidthAttribute;
                  
         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iHeightAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData Height");
            iWidthAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData Width");

            hMeta.SetAttribInt(iDapClass, iHeightAttribute, iHeight);
            hMeta.SetAttribInt(iDapClass, iWidthAttribute, iWidth);

            Geosoft.GXNet.CSYS.SetSettingsMETA(hMeta);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("SetSizeInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }

      /// <summary>
      /// Get the size of the dialog from the meta settings
      /// </summary>
      /// <param name="iWidth"></param>
      /// <param name="iHeight"></param>
      static public void GetSizeInSettingsMeta(out Int32 iWidth, out Int32 iHeight)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iHeightAttribute;
         Int32                iWidthAttribute;
            
         iHeight = Geosoft.GXNet.Constant.iDUMMY;
         iWidth = Geosoft.GXNet.Constant.iDUMMY;

         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iHeightAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData Height");
            iWidthAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData Width");

            hMeta.GetAttribInt(iDapClass, iHeightAttribute, ref iHeight);
            hMeta.GetAttribInt(iDapClass, iWidthAttribute, ref iWidth);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("GetSizeInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }

      /// <summary>
      /// Set the current result view of this dialog in the ap meta settings
      /// </summary>
      /// <param name="bView"></param>
      static public void SetResultViewInSettingsMeta(bool bView)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iResultViewAttribute;
                  
         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iResultViewAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData ResultView");
            
            Int32 iView = Convert.ToInt32(bView);
            hMeta.SetAttribBool(iDapClass, iResultViewAttribute, iView);

            Geosoft.GXNet.CSYS.SetSettingsMETA(hMeta);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("SetResultViewInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }

      /// <summary>
      /// Get the result view of the dialog from the meta settings
      /// </summary>
      /// <param name="bView"></param>
      static public void GetResultViewInSettingsMeta(out bool bView)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iResultViewAttribute;
         Int32                iView = 0;
                     
         bView = false;

         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iResultViewAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/GetDapData ResultView");

            hMeta.GetAttribBool(iDapClass, iResultViewAttribute, ref iView);

            bView = Convert.ToBoolean(iView);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("GetResultsViewInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }


      /// <summary>
      /// Set the current selected dap server in the ap meta settings
      /// </summary>
      /// <param name="bView"></param>
      static public void SetSelectedServerInSettingsMeta(string szUrl)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iUrlAttribute;
                  
         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iUrlAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/Dap Server");
            
            hMeta.SetAttribString(iDapClass, iUrlAttribute, szUrl);

            Geosoft.GXNet.CSYS.SetSettingsMETA(hMeta);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("SetSelectedServerInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }

      /// <summary>
      /// Get the selected server from the meta settings
      /// </summary>
      /// <param name="bView"></param>
      static public void GetSelectedServerInSettingsMeta(out string szUrl)
      {
         Geosoft.GXNet.CMETA  hMeta = null;
         Int32                iDapClass;
         Int32                iUrlAttribute;
                     
         szUrl = string.Empty;

         try
         {            
            hMeta = Geosoft.GXNet.CMETA.Create();
            Geosoft.GXNet.CSYS.GetSettingsMETA(hMeta);

            iDapClass = hMeta.ResolveUMN("CLASS:/Geosoft/Core/AppSettings/Dap Settings");
            iUrlAttribute = hMeta.ResolveUMN("ATTRIB:/Geosoft/Core/AppSettings/Dap Settings/Dap Server");
            
            hMeta.IGetAttribString(iDapClass, iUrlAttribute, ref szUrl);
         } 
         catch (Exception e)
         {
            GetDapError.Instance.Write("GetSelectedServerInSettingsMeta - " + e.Message);
         }
         finally
         {
            if (hMeta != null)
            {
               try
               {
                  hMeta.Dispose();
               } 
               catch {}
            }
         }
      }
#endif

      public class Xml
      {
         public class Tag
         {
            public static string Geosoft_Xml = "geosoft_xml";
            public static string Server = "server";
         }

         public class Attr
         {
            public static string Name = "name";
            public static string Url = "url";
            public static string Status = "status";
            public static string Major_Version = "major_version";
            public static string Minor_Version = "minor_version";
            public static string CacheVersion = "cache_version";
         }
      }

   }
}
