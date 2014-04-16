using System;
using System.Collections.Generic;

using System.Text;

namespace Glue
{
	public delegate void ApplicationEventEventHandler(object sender, ApplicationEventEventArgs e);
	public class ApplicationEventEventArgs
	{
		private bool mvarCancelApplication = false;
		public bool CancelApplication
		{
			get { return mvarCancelApplication; }
			set { mvarCancelApplication = value; }
		}

		internal Plugin mvarPlugin = null;
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

		public ApplicationEventEventArgs(string eventName)
		{
			mvarEventName = eventName;
		}
		public ApplicationEventEventArgs(string eventName, params KeyValuePair<string, object>[] eventArguments)
		{
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

		private bool mvarCancelPlugins = false;
		public bool CancelPlugins { get { return mvarCancelPlugins; } set { mvarCancelPlugins = value; } }

		private bool mvarCancelEvent = false;
		public bool CancelEvent { get { return mvarCancelEvent; } set { mvarCancelEvent = value; } }
	}
}
