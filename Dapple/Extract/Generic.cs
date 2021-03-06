using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Dapple.Extract
{
   /// <summary>
   /// Set the options for download acquire datasets
   /// </summary>
   internal partial class Generic : DownloadOptions
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="oDAPbuilder"></param>
      internal Generic(Dapple.LayerGeneration.DAPQuadLayerBuilder oDAPbuilder)
         : base(oDAPbuilder)
      {
         InitializeComponent();
      }

		internal override bool OpenInMap
		{
			get { return true; }
		}

      /// <summary>
      /// Write out settings for the acquire dataset
      /// </summary>
      /// <param name="oDatasetElement"></param>
      /// <param name="strDestFolder"></param>
      /// <param name="bDefaultResolution"></param>
      /// <returns></returns>
		internal override ExtractSaveResult Save(System.Xml.XmlElement oDatasetElement, string strDestFolder, DownloadSettings.DownloadCoordinateSystem eCS)
      {
         return base.Save(oDatasetElement, strDestFolder, eCS);         
      }
   }
}
