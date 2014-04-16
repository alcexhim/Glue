using System;
using System.Collections.Generic;

using System.Text;

namespace Glue
{
	public class Plugin : IComparable<Plugin>
	{
		private List<Guid> mvarApplicationIDs = new List<Guid>();
		/// <summary>
		/// Gets a list of IDs of all the applications that this Plugin is compatible with.
		/// </summary>
		public List<Guid> ApplicationIDs
		{
			get { return mvarApplicationIDs; }
		}

		private int mvarPriority = 0;
		public virtual int Priority { get { return mvarPriority; } }


		protected internal virtual bool CompatibleWithAnyApplication
		{
			get { return false; }
		}

		private bool mvarEnabled = true;
		public bool Enabled
		{
			get { return mvarEnabled; }
			internal set { mvarEnabled = value; }
		}

		protected void SendPluginEvent(string EventName)
		{
			SendPluginEvent(EventName, new System.Collections.Generic.KeyValuePair<string, object>[0]);
		}
		protected void SendPluginEvent(string EventName, params System.Collections.Generic.KeyValuePair<string, object>[] EventArguments)
		{
			PluginEventEventArgs e = new PluginEventEventArgs(this, EventName, EventArguments);
			Common.Events.SendPluginEvent(e);
		}

		protected object GetHostApplicationProperty(string PropertyName)
		{
			return null;
		}

		private Toolbar.ToolbarCollection mvarToolbars = new Toolbar.ToolbarCollection();
		/// <summary>
		/// Gets a collection of toolbars that this Plugin will create.
		/// </summary>
		public Toolbar.ToolbarCollection Toolbars
		{
			get { return mvarToolbars; }
		}

		private MenuBar mvarMenuBar = new MenuBar();
		/// <summary>
		/// Gets the MenuBar that this Plugin can modify.
		/// </summary>
		public MenuBar MenuBar
		{
			get { return mvarMenuBar; }
		}

        private ContextMenuCollection mvarContextMenus = new ContextMenuCollection();
        public ContextMenuCollection ContextMenus { get { return mvarContextMenus; } }

		protected internal virtual void OnApplicationEvent(ApplicationEventEventArgs e)
		{
		}

		#region IComparable<Plugin> Members

		public int CompareTo(Plugin other)
		{
			return other.Priority.CompareTo(mvarPriority);
		}

		#endregion
	}
}
