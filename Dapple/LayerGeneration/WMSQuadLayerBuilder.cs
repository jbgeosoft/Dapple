using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WorldWind;
using WorldWind.Renderable;
using System.Xml;
using WorldWind.Net.Wms;

namespace Dapple.LayerGeneration
{
   public class WMSQuadLayerBuilder : ImageBuilder
   {
      #region Private Members

      public WMSLayer m_wmsLayer;
      QuadTileSet m_oQuadTileSet;

      //Image Accessor
      private int m_iLevels = 15;
      private int m_intTextureSizePixels = 256;
      private string m_strCacheRoot;
      WMSLayerAccessor m_oWMSLayerAccessor = null;

      //WMS Layer Accessor
      string m_strServerUrl = string.Empty;
      string m_strLayerName = string.Empty;
      string m_strVersion = string.Empty;
      string m_strStyle = string.Empty;

      //QuadTileLayer
      int distAboveSurface = 0;
      bool terrainMapped = false;
      GeographicBoundingBox m_hBoundary = new GeographicBoundingBox(0, 0, 0, 0);
      ImageAccessor m_oImageAccessor = null;
      private decimal m_decLevelZeroTileSizeDegrees = 0;

      bool IsOn = true;
      bool m_blnIsChanged = true;
      #endregion

      #region Static
      public static readonly string URLProtocolName = "gxwms://";

      public static readonly string TypeName = "WMSQuadLayer";

      public static readonly string CacheSubDir = "WMS Image Cache";

      public static string GetServerFileNameFromUrl(string strurl)
      {
         string serverfile = strurl;
         int iQuery = serverfile.IndexOf("?");
         if (iQuery != -1)
            serverfile = serverfile.Substring(0, iQuery);

         int iUrl = strurl.IndexOf("//") + 2;
         if (iUrl == -1)
            iUrl = strurl.IndexOf("\\") + 2;
         if (iUrl != -1)
            serverfile = serverfile.Substring(iUrl);
         foreach (Char ch in Path.GetInvalidFileNameChars())
            serverfile = serverfile.Replace(ch.ToString(), "_");
         return serverfile;
      }

      private static void ParseURI(string uri, ref string strCapURL, ref string strLayer, ref int pixelsize)
      {
         strCapURL = uri.Replace(URLProtocolName, "http://");
         int iIndex = strCapURL.LastIndexOf("&");
         if (iIndex != -1)
         {
            pixelsize = Convert.ToInt32(strCapURL.Substring(iIndex).Replace("&pixelsize=", ""));
            strCapURL = strCapURL.Substring(0, iIndex);
         }
         else
            return;
         iIndex = strCapURL.LastIndexOf("&");
         if (iIndex != -1)
         {
            strLayer = strCapURL.Substring(iIndex).Replace("&layer=", "");
            strCapURL = strCapURL.Substring(0, iIndex).Trim();
         }
         else
            return;
         WMSCatalogBuilder.TrimCapabilitiesURL(ref strCapURL);
      }

      public static string ServerURLFromURI(string uri)
      {
         string strServer = "";
         string strLayer = "";
         int pixelsize = 1024;
         try
         {
            ParseURI(uri, ref strServer, ref strLayer, ref pixelsize);
         }
         catch
         {
         }
         return strServer;
      }

      public static WMSQuadLayerBuilder GetBuilderFromURI(string uri, WMSCatalogBuilder provider, WorldWindow worldWindow, WMSServerBuilder wmsserver)
      {
         string strServer = "";
         string strLayer = "";
         int pixelsize = 1024;
         try
         {
            ParseURI(uri, ref strServer, ref strLayer, ref pixelsize);

            WMSList oServer = provider.FindServer(strServer);
            foreach (WMSLayer layer in oServer.Layers)
            {
               WMSLayer result = FindLayer(strLayer, layer);
               if (result != null)
               {
                  WMSQuadLayerBuilder zoomBuilder = wmsserver.FindLayerBuilder(result);
                  if (zoomBuilder != null)
                  {
                     zoomBuilder.ImagePixelSize = pixelsize;
                     return zoomBuilder;
                  }
               }
            }
         }
         catch
         {
         }
         return null;
      }

      public static WMSLayer FindLayer(string layerName, WMSLayer list)
      {
         foreach (WMSLayer layer in list.ChildLayers)
         {
            if (layer.ChildLayers != null && layer.ChildLayers.Length > 0)
            {
               WMSLayer result = FindLayer(layerName, layer);
               if (result != null)
               {
                  return result;
               }
            }
            if (layer.Name == layerName)
            {
               return layer;
            }
         }
         return null;
      }

      #endregion

      public WMSQuadLayerBuilder(WMSLayer layer, int height, bool isTerrainMapped, GeographicBoundingBox boundary,
         WMSLayerAccessor wmsLayerAccessor, bool isOn, World world, string cacheDirectory, IBuilder parent)
      {
         m_wmsLayer = layer;
         m_strName = layer.Title;
         distAboveSurface = height;
         terrainMapped = isTerrainMapped;
         m_hBoundary = boundary;

         // Determine the needed levels (function of tile size and resolution, for which we just use ~5 meters because it is not available with WMS)
         double dRes = 5.0 / 100000.0;
         if (dRes > 0)
         {
            decimal dTileSize = LevelZeroTileSize;
            m_iLevels = 1;
            while ((double)dTileSize / Convert.ToDouble(m_intTextureSizePixels) > dRes / 4.0)
            {
               m_iLevels++;
               dTileSize /= 2;
            }
         }

         IsOn = isOn;
         m_strCacheRoot = cacheDirectory;
         m_oWorld = world;
         m_Parent = parent;

         m_oWMSLayerAccessor = wmsLayerAccessor;
         m_strLayerName = m_oWMSLayerAccessor.WMSLayerName;
         m_strServerUrl = m_oWMSLayerAccessor.ServerGetMapUrl;
         m_strVersion = m_oWMSLayerAccessor.Version;
         m_strStyle = m_oWMSLayerAccessor.WMSLayerStyle;
      }

      public override string ServiceType
      {
         get
         {
            return "WMS Layer";
         }
      }

      public override bool SupportsMetaData
      {
         get
         {
            return false;
         }
      }

      public override XmlNode GetMetaData(XmlDocument oDoc)
      {
         return null;
      }

      public override bool SupportsLegend
      {
         get
         {
            return m_wmsLayer.HasLegend;
         }
      }

      public override string[] GetLegendURLs()
      {
         if (m_wmsLayer.HasLegend)
         {
            foreach (WMSLayerStyle style in m_wmsLayer.Styles)
            {
               if (style.legendURL != null && style.legendURL.Length > 0)
               {
                  int i = 0;
                  string[] strArr = new string[style.legendURL.Length];
                  foreach (WMSLayerStyleLegendURL legURL in style.legendURL)
                  {
                     strArr[i] = legURL.ToString();
                     i++;
                  }
                  return strArr;
               }
            }
         }
         return null;
      }

      public override RenderableObject GetLayer()
      {
         return GetQuadLayer();
      }

      public QuadTileSet GetQuadLayer()
      {
         if (m_blnIsChanged)
         {
            string strExt = ".png";
            string strCachePath = Path.Combine(GetCachePath(), LevelZeroTileSize.ToString());
            System.IO.Directory.CreateDirectory(strCachePath);

            if (string.Compare(m_oWMSLayerAccessor.ImageFormat, "image/jpeg", true, System.Globalization.CultureInfo.InvariantCulture) == 0 ||
               String.Compare(m_oWMSLayerAccessor.ImageFormat, "image/jpg", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
               strExt = ".jpg";

            m_oImageAccessor = new ImageAccessor(strCachePath,
                m_intTextureSizePixels,
                LevelZeroTileSize,
                m_iLevels,
                strExt,
                strCachePath, m_oWMSLayerAccessor);

            m_oQuadTileSet = new QuadTileSet(m_strName,
                m_hBoundary,
                m_oWorld,
                distAboveSurface,
                (terrainMapped ? m_oWorld.TerrainAccessor : null),
                m_oImageAccessor, Opacity, true);
            m_oQuadTileSet.IsOn = m_IsOn;
            m_blnIsChanged = false;
         }
         return m_oQuadTileSet;
      }

      #region IBuilder Members

      public override byte Opacity
      {
         get
         {
            if (m_oQuadTileSet != null)
               return m_oQuadTileSet.Opacity;
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
            if (m_oQuadTileSet != null && m_oQuadTileSet.Opacity != value)
            {
               m_oQuadTileSet.Opacity = value;
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
            if (m_oQuadTileSet != null)
               return m_oQuadTileSet.IsOn;
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
            if (m_oQuadTileSet != null && m_oQuadTileSet.IsOn != value)
            {
               m_oQuadTileSet.IsOn = value;
               bChanged = true;
            }

            if (bChanged)
               SendBuilderChanged(BuilderChangeType.VisibilityChanged);
         }
      }

      public override string Type
      {
         get { return WMSQuadLayerBuilder.TypeName; }
      }

      public override bool IsChanged
      {
         get { return m_blnIsChanged; }
      }

      public override string LogoKey
      {
         get { return "wms"; }
      }

      public override bool bIsDownloading(out int iBytesRead, out int iTotalBytes)
      {
         if (m_oQuadTileSet != null)
            return m_oQuadTileSet.bIsDownloading(out iBytesRead, out iTotalBytes);
         else
         {
            iBytesRead = 0;
            iTotalBytes = 0;
            return false;
         }
      }
      
      public string Style
      {
         get
         {
            return m_strStyle;
         }
         set
         {
            m_strStyle = value;
            m_blnIsChanged = true;
         }
      }

      #endregion

      public decimal LevelZeroTileSize
      {
         get
         {
            if (m_decLevelZeroTileSizeDegrees == 0)
               // Round to ceiling of four decimals (>~ 10 meter resolution)
               m_decLevelZeroTileSizeDegrees = Math.Min(30, Math.Ceiling(10000 * (decimal)Math.Max(m_hBoundary.North - m_hBoundary.South, m_hBoundary.West - m_hBoundary.East)) / 10000);
            return m_decLevelZeroTileSizeDegrees;
         }
      }
      

      public int Levels
      {
         get { return m_iLevels; }
      }

      #region ImageBuilder Members

      public override GeographicBoundingBox Extents
      {
         get { return m_hBoundary; }
      }

      public override int ImagePixelSize
      {
         get { return m_intTextureSizePixels; }
         set
         {
            if (m_intTextureSizePixels != value)
            {
               m_blnIsChanged = true;
               m_intTextureSizePixels = value;
            }
         }
      }

      #endregion

      public override string GetCachePath()
      {
         string serverfile = GetServerFileNameFromUrl(m_wmsLayer.ParentWMSList.ServerGetMapUrl);
         return Path.Combine(Path.Combine(Path.Combine(m_strCacheRoot, CacheSubDir), serverfile), Utility.StringHash.GetBase64HashForPath(m_wmsLayer.Name));
      }

      public override string GetURI()
      {
         return (m_wmsLayer.ParentWMSList.ServerGetCapabilitiesUrl + "&layer=" + m_wmsLayer.Name + "&pixelsize=" + m_intTextureSizePixels.ToString()).Replace("http://", URLProtocolName);
      }

      public override object Clone()
      {
         return new WMSQuadLayerBuilder(m_wmsLayer, distAboveSurface, terrainMapped, m_hBoundary,
            m_oWMSLayerAccessor, IsOn, m_oWorld, m_strCacheRoot, m_Parent);
      }

      protected override void CleanUpLayer()
      {
         m_oQuadTileSet.Dispose();
         m_oImageAccessor.Dispose();
         m_oImageAccessor = null;
         m_oQuadTileSet = null;
         m_blnIsChanged = true;
      }
   }
}