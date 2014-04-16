using System;
namespace Glue
{
	public class Toolbar
	{

		public Toolbar()
		{
		}
		public Toolbar(string text)
		{
			mvarName = text;
			mvarText = text;
		}
		public Toolbar(string name, string text)
		{
			mvarName = name;
			mvarText = text;
		}

		private string mvarName = null;
		/// <summary>
		/// The name of this toolbar. If the toolbar with this name does not
		/// exist, it will be created.
		/// </summary>
		public string Name
		{
			get { return mvarName; }
			set { mvarName = value; }
		}
		
		private string mvarText = String.Empty;
		/// <summary>
		/// The text to display as the label of this toolbar, if labels are
		/// displayed.
		/// </summary>
		public string Text
		{
			get { return mvarText; }
			set { mvarText = value; }
		}
		
		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		/// <summary>
		/// The commands available on this toolbar.
		/// </summary>
		public Command.CommandCollection Commands
		{
			get { return mvarCommands; }
		}
		
		public class ToolbarCollection
			: System.Collections.ObjectModel.Collection<Toolbar>
		{
			public Toolbar Add (string name)
			{
				return Add (name, name);
			}
			public Toolbar Add (string name, string text)
			{
				Toolbar toolbar = new Toolbar ();
				toolbar.Name = name;
				toolbar.Text = text;
				base.Add (toolbar);
				return toolbar;
			}
		}
	}
}

