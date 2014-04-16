using System;
using System.Collections.Generic;
using System.Text;

namespace Glue.ControlPropertiesManager
{
	public class CPMAssembly
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
		private CPMWindow.CPMWindowCollection mvarWindows = new CPMWindow.CPMWindowCollection();
		public CPMWindow.CPMWindowCollection Windows
		{
			get { return mvarWindows; }
		}

		public class CPMAssemblyCollection
			: System.Collections.ObjectModel.Collection<CPMAssembly>
		{
			public CPMAssembly this[string Name]
			{
				get
				{
					foreach (CPMAssembly asm in this)
					{
						if (asm.Name == Name)
						{
							return asm;
						}
					}
					return null;
				}
			}
		}
	}
}
