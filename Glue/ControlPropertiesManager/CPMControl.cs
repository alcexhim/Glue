using System;
using System.Collections.Generic;
using System.Text;

namespace Glue.ControlPropertiesManager
{
	public class CPMControl
	{
		public class CPMControlCollection
			: System.Collections.ObjectModel.Collection<CPMControl>
		{
		}

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
	}
}
