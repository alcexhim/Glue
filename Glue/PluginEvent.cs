using System;
using System.Collections.Generic;

using System.Text;

namespace Glue
{
	public delegate void PluginEventEventHandler(object sender, PluginEventEventArgs e);
	public class PluginEventEventArgs : System.ComponentModel.CancelEventArgs
	{
		private Plugin mvarPlugin = null;
		public Plugin Plugin
		{
			get { return mvarPlugin; }
		}

		private string mvarEventName = String.Empty;
		public string EventName
		{
			get { return mvarEventName; }
		}

		private System.Collections.Generic.Dictionary<string, object> mvarEventArguments = new System.Collections.Generic.Dictionary<string, object>();
		public System.Collections.Generic.Dictionary<string, object> EventArguments
		{
			get { return mvarEventArguments; }
		}

		public PluginEventEventArgs(Plugin plugin, string eventName)
		{
			mvarPlugin = plugin;
			mvarEventName = eventName;
		}
		public PluginEventEventArgs(Plugin plugin, string eventName, params KeyValuePair<string, object>[] eventArguments)
		{
			mvarPlugin = plugin;
			mvarEventName = eventName;
			foreach (KeyValuePair<string, object> kvp in eventArguments)
			{
				if (!mvarEventArguments.ContainsKey(kvp.Key))
				{
					mvarEventArguments.Add(kvp.Key, kvp.Value);
				}
				else
				{
					mvarEventArguments[kvp.Key] = kvp.Value;
				}
			}
		}
	}
}
