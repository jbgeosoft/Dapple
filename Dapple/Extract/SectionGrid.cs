using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Dapple.Extract
{
   /// <summary>
   /// Set the download options for the grid
   /// </summary>
   internal partial class SectionGrid : DownloadOptions
   {
      #region Constants
      private readonly string GRD_EXT = ".grd";      
      #endregion

      /// <summary>
      /// Control where the resolution can be changed
      /// </summary>
      internal override bool ResolutionEnabled
      {
         set { oResolution.Enabled = value; }
      }

		internal override bool OpenInMap
		{
			get { return ((Options.SectionGrid.DisplayOptions)cbDisplayOptions.SelectedIndex) != Options.SectionGrid.DisplayOptions.DoNotDisplay; }
		}

		internal override ErrorProvider ErrorProvider
		{
			get
			{
				return base.ErrorProvider;
			}
			set
			{
				base.ErrorProvider = value;
				oResolution.ErrorProvider = value;
			}
		}

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="oDAPbuilder"></param>
      internal SectionGrid(Dapple.LayerGeneration.DAPQuadLayerBuilder oDAPbuilder)
         : base(oDAPbuilder)
      {
         InitializeComponent();
         tbFilename.Text = System.IO.Path.ChangeExtension(oDAPbuilder.Title, GRD_EXT);

         cbDisplayOptions.DataSource = Options.SectionGrid.DisplayOptionStrings;
         cbDisplayOptions.SelectedIndex = 0;         

         oResolution.SetDownloadOptions(this);
         SetDefaultResolution();
      }

      /// <summary>
      /// Set the default resolution
      /// </summary>
      internal override void SetDefaultResolution()
      {
         double dXOrigin, dYOrigin, dXCellSize, dYCellSize;
         int iSizeX, iSizeY;

         string strCoordinateSystem = m_strLayerProjection;
         MainForm.MontajInterface.GetGridInfo(m_oDAPLayer.ServerURL, m_oDAPLayer.DatasetName, out dXOrigin, out dYOrigin, out iSizeX, out iSizeY, out dXCellSize, out dYCellSize);

         oResolution.Setup(false, strCoordinateSystem, dXOrigin, dYOrigin, iSizeX, iSizeY, dXCellSize, dYCellSize);         
      }

      internal override void SetNativeResolution()
      {
         oResolution.SetNativeResolution();
      }

      /// <summary>
      /// Write out settings for the Grid dataset
      /// </summary>
      /// <param name="oDatasetElement"></param>
      /// <param name="strDestFolder"></param>
      /// <param name="bDefaultResolution"></param>
      /// <returns></returns>
		internal override ExtractSaveResult Save(System.Xml.XmlElement oDatasetElement, string strDestFolder, DownloadSettings.DownloadCoordinateSystem eCS)
      {
         // --- cannot reproject section data --- 
			ExtractSaveResult result = base.Save(oDatasetElement, strDestFolder, DownloadSettings.DownloadCoordinateSystem.Native);

         System.Xml.XmlAttribute oPathAttr = oDatasetElement.OwnerDocument.CreateAttribute("file");
         oPathAttr.Value = System.IO.Path.Combine(strDestFolder, System.IO.Path.ChangeExtension(Utility.FileSystem.SanitizeFilename(tbFilename.Text), GRD_EXT));
         oDatasetElement.Attributes.Append(oPathAttr);

         System.Xml.XmlAttribute oResolutionAttr = oDatasetElement.OwnerDocument.CreateAttribute("resolution");
			oResolutionAttr.Value = oResolution.ResolutionValueSpecific(eCS).ToString(CultureInfo.InvariantCulture);         
         oDatasetElement.Attributes.Append(oResolutionAttr);

         System.Xml.XmlElement oDisplayElement = oDatasetElement.OwnerDocument.CreateElement("display_options");
         Options.SectionGrid.DisplayOptions eDisplayOption = (Options.SectionGrid.DisplayOptions)cbDisplayOptions.SelectedIndex;
         oDisplayElement.InnerText = eDisplayOption.ToString();
         oDatasetElement.AppendChild(oDisplayElement);

			return result;
      }

		internal override DownloadOptions.DuplicateFileCheckResult CheckForDuplicateFiles(String szExtractDirectory, Form hExtractForm)
		{
			String szFilename = System.IO.Path.Combine(szExtractDirectory, System.IO.Path.ChangeExtension(tbFilename.Text, GRD_EXT));
			if (System.IO.File.Exists(szFilename))
			{
				return QueryOverwriteFile("The file \"" + szFilename + "\" already exists.  Overwrite?", hExtractForm);
			}
			else
			{
				return DuplicateFileCheckResult.Yes;
			}
		}

		private void tbFilename_Validating(object sender, CancelEventArgs e)
		{
			if (String.IsNullOrEmpty(tbFilename.Text))
			{
				m_oErrorProvider.SetError(tbFilename, "Field cannot be empty.");
				e.Cancel = true;
			}
			else
			{
				m_oErrorProvider.SetError(tbFilename, String.Empty);
			}
		}
   }
}
