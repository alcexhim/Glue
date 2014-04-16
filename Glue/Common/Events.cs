using System;
using System.Collections.Generic;

using System.Text;

namespace Glue.Common
{
	public static class Events
	{
		public static event Glue.ApplicationEventEventHandler ApplicationEvent;
		internal static void SendApplicationEvent(Glue.ApplicationEventEventArgs e)
		{
			if (ApplicationEvent != null)
			{
				ApplicationEvent(e.Plugin, e);
			}
		}
		public static event Glue.PluginEventEventHandler PluginEvent;
		internal static void SendPluginEvent(Glue.PluginEventEventArgs e)
		{
			if (PluginEvent != null)
			{
				PluginEvent(e.Plugin, e);
			}
		}
	}
}
