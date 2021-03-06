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
   /// Set the hyxz options
   /// </summary>
   internal partial class HyperXYZ : DownloadOptions
   {
      #region Constants
      private readonly string DATABASE_EXT = ".gdb";
		private readonly string SHP_EXT = ".shp";
		private readonly string TAB_EXT = ".tab";
      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="oDAPbuilder"></param>
      internal HyperXYZ(Dapple.LayerGeneration.DAPQuadLayerBuilder oDAPbuilder)
         : base(oDAPbuilder)
      {
         InitializeComponent();
			tbFilename.Text = System.IO.Path.ChangeExtension(oDAPbuilder.Title, ExtensionForHXYZ);
      }

		internal override bool OpenInMap
		{
			get { return true; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			c_lArcMapNote.Visible = (MainForm.Client == Options.Client.ClientType.ArcMAP);
		}

      /// <summary>
      /// Write out settings for the HyperXYZ dataset
      /// </summary>
      /// <param name="oDatasetElement"></param>
      /// <param name="strDestFolder"></param>
      /// <param name="bDefaultResolution"></param>
      /// <returns></returns>
		internal override ExtractSaveResult Save(System.Xml.XmlElement oDatasetElement, string strDestFolder, DownloadSettings.DownloadCoordinateSystem eCS)
      {
			// --- Always download point data in its native projection when in ArcMap ---
			if (MainForm.Client == Options.Client.ClientType.ArcMAP)
				eCS = DownloadSettings.DownloadCoordinateSystem.Native;

         ExtractSaveResult result = base.Save(oDatasetElement, strDestFolder, eCS);

         System.Xml.XmlAttribute oPathAttr = oDatasetElement.OwnerDocument.CreateAttribute("file");
			oPathAttr.Value = System.IO.Path.Combine(strDestFolder, System.IO.Path.ChangeExtension(Utility.FileSystem.SanitizeFilename(tbFilename.Text), ExtensionForHXYZ));

         oDatasetElement.Attributes.Append(oPathAttr);

			return result;
      }

		internal override DownloadOptions.DuplicateFileCheckResult CheckForDuplicateFiles(String szExtractDirectory, Form hExtractForm)
		{
			String szFilename = System.IO.Path.Combine(szExtractDirectory, System.IO.Path.ChangeExtension(tbFilename.Text, ExtensionForHXYZ));
			if (System.IO.File.Exists(szFilename))
			{
				return QueryOverwriteFile("The file \"" + szFilename + "\" already exists.  Overwrite?", hExtractForm);
			}
			else
			{
				return DuplicateFileCheckResult.Yes;
			}
		}

		private String ExtensionForHXYZ
		{
			get
			{
				switch (MainForm.Client)
				{
					case Options.Client.ClientType.ArcMAP:
						return SHP_EXT;
					case Options.Client.ClientType.MapInfo:
						return TAB_EXT;
					case Options.Client.ClientType.OasisMontaj:
						return DATABASE_EXT;
					default:
						throw new InvalidOperationException("Unprogrammed client type " + MainForm.Client.ToString());
				}
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
