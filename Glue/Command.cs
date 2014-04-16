using System;
using System.Collections.Generic;

using System.Text;

namespace Glue
{
	public class Command
	{
		public event EventHandler Activated;

		private CommandToggleMode mvarToggleMode = CommandToggleMode.None;
		public CommandToggleMode ToggleMode
		{
			get { return mvarToggleMode; }
			set { mvarToggleMode = value; }
		}
		private bool mvarToggled = false;
		public bool Toggled
		{
			get { return mvarToggled; }
			set { mvarToggled = value; }
		}

		public enum CommandToggleMode
		{
			None = 0,
			Radio = 1,
			Check = 2
		}
		public class CommandSeparator : Command
		{
		}
		public static class PredefinedCommands
		{
			private static string mvarFileMenu = "mnuFile";
			public static string FileMenu
			{
				get { return mvarFileMenu; }
			}
			private static string mvarEditMenu = "mnuEdit";
			public static string EditMenu
			{
				get { return mvarEditMenu; }
			}
			private static string mvarViewMenu = "mnuView";
			public static string ViewMenu
			{
				get { return mvarViewMenu; }
			}
			private static string mvarToolsMenu = "mnuTools";
			public static string ToolsMenu
			{
				get { return mvarToolsMenu; }
			}
			private static string mvarWindowMenu = "mnuWindow";
			public static string WindowMenu
			{
				get { return mvarWindowMenu; }
			}
			private static string mvarHelpMenu = "mnuHelp";
			public static string HelpMenu
			{
				get { return mvarHelpMenu; }
			}
		}
		
		private string mvarName = null;
		/// <summary>
		/// The programmatic identifier of this command. 
		/// </summary>
		public string Name
		{
			get { return mvarName; }
			set { mvarName = value; }
		}

		private string mvarText = String.Empty;
		/// <summary>
		/// The text displayed as the label of this command. 
		/// </summary>
		public string Text
		{
			get { return mvarText; }
			set { mvarText = value; }
		}
		
		private System.Drawing.Image mvarImage = null;
		public System.Drawing.Image Image
		{
			get { return mvarImage; }
			set { mvarImage = value; }
		}

		private bool mvarUseDefaultPosition = true;
		public bool UseDefaultPosition
		{
			get { return mvarUseDefaultPosition; }
			set { mvarUseDefaultPosition = value; }
		}

		private int mvarPosition = -1;
		public int Position
		{
			get { return mvarPosition; }
			set { mvarPosition = value; mvarUseDefaultPosition = false; }
		}
		
		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		public Command.CommandCollection Commands
		{
			get { return mvarCommands; }
			set { mvarCommands = value; }
		}
		
		public Command ()
		{
			mvarName = null;
			mvarText = String.Empty;
		}
		public Command (string text)
		{
			mvarName = text;
			mvarText = text;
		}
		public Command (string name, string text)
		{
			mvarName = name;
			mvarText = text;
		}
		public Command(string name, string text, System.Drawing.Image image)
		{
			mvarName = name;
			mvarText = text;
			mvarImage = image;
		}
		public Command(string name, string text, System.Drawing.Image image, int position)
		{
			mvarName = name;
			mvarText = text;
			mvarImage = image;
			Position = position;
		}
		public Command(string name, string text, System.Drawing.Image image, int position, EventHandler activated)
		{
			mvarName = name;
			mvarText = text;
			mvarImage = image;
			Position = position;
			
			if (activated != null)
			{
				Activated += activated;
			}
		}

		public void Activate ()
		{
			if (Activated != null)
				Activated (this, EventArgs.Empty);
		}
		
		public class CommandCollection
			: System.Collections.ObjectModel.Collection<Command>
		{
			public Command AddButton (string name)
			{
				return Add(name, name);
			}
			public Command Add(string name, EventHandler activated)
			{
				return Add(name, name, null, -1, activated);
			}
			public Command Add (string name, string text)
			{
				return Add(name, text, null, -1, null);
			}
			public Command Add(string name, string text, EventHandler activated)
			{
				return Add(name, text, null, -1, activated);
			}
            public Command Add(string name, string text, EventHandler activated, int position)
            {
                return Add(name, text, null, position, activated);
            }
			public Command Add(string name, string text, System.Drawing.Image image)
			{
				return Add(name, text, image, -1);
			}
			public Command Add(string name, string text, System.Drawing.Image image, EventHandler activated)
			{
				return Add(name, text, image, -1, activated);
			}
			public Command Add(string name, string text, System.Drawing.Image image, int position)
			{
				return Add(name, text, image, position, null);
			}
			public Command Add(string name, string text, System.Drawing.Image image, int position, EventHandler activated)
			{
				Command cmd = new Command (name, text, image, position, activated);
				base.Add (cmd);
				return cmd;
			}

			public void Add(Command item, int position)
			{
				item.Position = position;
				base.Add(item);
			}

			public Command this[string name]
			{
				get
				{
					foreach (Command cmd in this)
					{
						if (cmd.Name == name)
						{
							return cmd;
						}
					}
					return AddButton(name);
				}
			}

			public bool Contains(string name)
			{
				foreach (Command cmd in this)
				{
					if (cmd.Name == name)
					{
						return true;
					}
				}
				return false;
			}

			public bool Remove(string name)
			{
				foreach (Command cmd in this)
				{
					if (cmd.Name == name)
					{
						base.Remove(cmd);
						return true;
					}
				}
				return false;
			}

			public CommandSeparator AddSeparator()
			{
				CommandSeparator sep = new CommandSeparator();
				base.Add(sep);
				return sep;
			}
			public CommandSeparator AddSeparator(int position)
			{
				CommandSeparator sep = new CommandSeparator();
				sep.Position = position;
				base.Add(sep);
				return sep;
			}
			public CommandSeparator AddSeparator(string name)
			{
				CommandSeparator sep = new CommandSeparator();
				sep.Name = name;
				base.Add(sep);
				return sep;
			}
			public CommandSeparator AddSeparator(string name, int position)
			{
				CommandSeparator sep = new CommandSeparator();
				sep.Name = name;
				sep.Position = position;
				base.Add(sep);
				return sep;
			}
		}
	}
}
