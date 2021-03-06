using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dapple.Extract
{
	internal partial class OverwriteDialog : Form
	{
		internal OverwriteDialog(String szMessage, Form hOwner)
		{
			InitializeComponent();
			label1.Text = szMessage;
			this.Owner = hOwner;
		}

		internal OverwriteDialog() : this("The specified file already exists! Overwrite?", null)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
		}
	}
}