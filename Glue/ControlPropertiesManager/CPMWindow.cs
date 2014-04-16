using System;
using System.Collections.Generic;
using System.Text;

namespace Glue.ControlPropertiesManager
{
	public class CPMWindow
	{
		private string mvarName = String.Empty;
		public string Name
		{
			get { return mvarName; }
			set { mvarName = value; }
		}

		private Dictionary<string, string> mvarProperties = new Dictionary<string, string>();
		public Dictionary<string, string> Properties
		{
			get { return mvarProperties; }
		}

		private CPMControl.CPMControlCollection mvarControls = new CPMControl.CPMControlCollection();
		public CPMControl.CPMControlCollection Controls
		{
			get { return mvarControls; }
		}

		public class CPMWindowCollection
			: System.Collections.ObjectModel.Collection<CPMWindow>
		{
			public CPMWindow this[string Name]
			{
				get
				{
					foreach (CPMWindow wnd in this)
					{
						if (wnd.Name == Name)
						{
							return wnd;
						}
					}
					return null;
				}
			}
		}
	}
}
