using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dapple.Properties;

namespace Dapple.CustomControls
{
   /// <summary>
   /// A toolstrip masquerading as a selection of tabs.
   /// </summary>
   internal partial class TabToolStrip : ToolStrip
   {
      #region Delegates

      internal delegate void TabToolbarButtonDelegate(int iIndex);

      #endregion

      #region Events

      internal event TabToolbarButtonDelegate ButtonPressed;

      #endregion

      #region Member Variables

      protected ToolStripButton[] m_aButtons;

      #endregion

      #region Constructors

      internal TabToolStrip()
         : this(2)
      {
      }

      internal TabToolStrip(int iNumButtons)
      {
         if (iNumButtons < 1) throw new ArgumentException("Need to specify at least one button");

         InitializeComponent();

         this.SuspendLayout();

			using (Bitmap oPlaceholderGraphic = new Bitmap(16, 16))
			{
				using (Graphics g = Graphics.FromImage(oPlaceholderGraphic))
				{
					g.FillRectangle(Brushes.Gainsboro, new Rectangle(0, 0, 16, 16));
					g.DrawLine(Pens.Red, new Point(0, 0), new Point(15, 15));

					m_aButtons = new ToolStripButton[iNumButtons];

					for (int count = 0; count < iNumButtons; count++)
					{
						m_aButtons[count] = new ToolStripButton();
						m_aButtons[count].Tag = count;
						m_aButtons[count].Image = oPlaceholderGraphic;
						m_aButtons[count].Click += new EventHandler(ButtonClick);
						m_aButtons[count].DisplayStyle = ToolStripItemDisplayStyle.Image;
						this.Items.Add(m_aButtons[count]);
					}
					m_aButtons[0].Checked = true;

					this.Renderer = new TabToolBarRenderer();
					this.GripStyle = ToolStripGripStyle.Hidden;
					this.Dock = DockStyle.Bottom;

					this.ResumeLayout();
				}
			}
      }

      #endregion

      #region Event Handlers

      void ButtonClick(object sender, EventArgs e)
      {
         foreach (ToolStripButton oButton in m_aButtons)
         {
            oButton.Checked = false;
         }
         ((ToolStripButton)sender).Checked = true;

         if (ButtonPressed != null && sender is ToolStripButton)
         {
            ButtonPressed((int)((ToolStripButton)sender).Tag);
         }
      }

      #endregion

      #region Public Members

      internal void SetImage(int iButtonIndex, Image oImage)
      {
         m_aButtons[iButtonIndex].Image = oImage;
      }

      internal void SetToolTip(int iButtonIndex, String szToolTipText)
      {
         m_aButtons[iButtonIndex].ToolTipText = szToolTipText;
      }

		internal void SetNameAndText(int iPageIndex, string szName)
		{
			m_aButtons[iPageIndex].Name = szName;
			m_aButtons[iPageIndex].Text = szName;
		}

      #endregion
	}

   internal class TabToolBarRenderer : ToolStripProfessionalRenderer
   {
      internal TabToolBarRenderer()
         : base()
      {
         this.RoundedEdges = false;
      }

      protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
      {
         e.Graphics.DrawLine(SystemPens.WindowFrame, new Point(0, 0), new Point(e.AffectedBounds.Width, 0));
      }

      protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
      {
         //Don't
         //e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.AffectedBounds);
      }
   }
}
