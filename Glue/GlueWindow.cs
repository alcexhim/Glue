using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Glue
{
	/// <summary>
	/// A window that automatically inherits Glue capabilities, such as sending notifications when the
	/// window's size changes or the window is opened/closed.
	/// </summary>
	public partial class GlueWindow : Form
	{
		public GlueWindow()
		{
			InitializeComponent();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			Glue.ApplicationEventEventArgs ee = new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.WindowShown,
				new KeyValuePair<string, object>("Window", this));

			Glue.Common.Methods.SendApplicationEvent(ee);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			Glue.ApplicationEventEventArgs ee = new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.WindowClosing,
				new KeyValuePair<string, object>("Window", this));

			Glue.Common.Methods.SendApplicationEvent(ee);

			if (ee.CancelEvent)
			{
				e.Cancel = true;
			}
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Glue.ApplicationEventEventArgs ee = new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.WindowClosing,
				new KeyValuePair<string, object>("Window", this));

			Glue.Common.Methods.SendApplicationEvent(ee);
		}

		private bool mvarTextChanging = false;

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			if (mvarTextChanging) return;

			string WindowTitle = this.Text;
			Glue.ApplicationEventEventArgs ee = new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.WindowCaptionUpdating,
				new KeyValuePair<string, object>("Text", WindowTitle));

			Glue.Common.Methods.SendApplicationEvent(ee);

			WindowTitle = ee.EventArguments["Text"].ToString();
			
			mvarTextChanging = true;
			this.Text = WindowTitle;
			mvarTextChanging = false;
		}
	}
}
