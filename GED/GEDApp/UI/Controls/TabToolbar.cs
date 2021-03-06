using System;
using System.Drawing;
using System.Windows.Forms;

namespace GED.App.UI.Controls
{
   /// <summary>
   /// A toolstrip masquerading as a selection of tabs.
   /// </summary>
   public partial class TabToolStrip : ToolStrip
   {
      #region Delegates

      public delegate void TabToolBarButtonEventHandler(int iIndex);

      #endregion

      #region Events

      public event TabToolBarButtonEventHandler ButtonPressed;

      #endregion

      #region Member Variables

      protected ToolStripButton[] m_aButtons;

      #endregion

      #region Constructors

      public TabToolStrip()
         : this(2)
      {
      }

      public TabToolStrip(int iNumButtons)
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

      public void SetImage(int iButtonIndex, Image oImage)
      {
         m_aButtons[iButtonIndex].Image = oImage;
      }

      public void SetToolTip(int iButtonIndex, String strToolTipText)
      {
         m_aButtons[iButtonIndex].ToolTipText = strToolTipText;
      }

		public void SetNameAndText(int iPageIndex, string strName)
		{
			m_aButtons[iPageIndex].Name = strName;
			m_aButtons[iPageIndex].Text = strName;
		}

      #endregion
	}

   public class TabToolBarRenderer : ToolStripProfessionalRenderer
   {
      public TabToolBarRenderer()
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
