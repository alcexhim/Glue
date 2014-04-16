using System;
using System.Collections.Generic;
using System.Text;

namespace Glue.ControlPropertiesManager
{
	public class CPMLanguage
	{
		public class CPMLanguageCollection
			: System.Collections.ObjectModel.Collection<CPMLanguage>
		{
			public CPMLanguage Add(int lcid, string name)
			{
				if (Contains(lcid))
				{
					throw new ArgumentException(Common.Methods.GetResourceString("Argument_AddingDuplicate__", lcid, lcid));
				}

				CPMLanguage lang = new CPMLanguage();
				lang.ID = lcid;
				lang.Name = name;
				base.Add(lang);
				return lang;
			}

			public bool Contains(int lcid)
			{
				return (this[lcid] != null);
			}
			public bool Remove(int lcid)
			{
				CPMLanguage lang = this[lcid];
				if (lang != null)
				{
					base.Remove(lang);
					return true;
				}
				return false;
			}

			public CPMLanguage this[int lcid]
			{
				get
				{
					foreach (CPMLanguage lang in this)
					{
						if (lang.ID == lcid)
						{
							return lang;
						}
					}
					return null;
				}
			}

			public CPMLanguage Update(int lcid, string name)
			{
				CPMLanguage lang = this[lcid];
				if (lang == null)
				{
					return Add(lcid, name);
				}
				return lang;
			}
		}

		private int mvarID = -1;
		public int ID
		{
			get { return mvarID; }
			private set { mvarID = value; }
		}

		private string mvarName = String.Empty;
		public string Name
		{
			get { return mvarName; }
			private set { mvarName = value; }
		}

		private CPMAssembly.CPMAssemblyCollection mvarAssemblies = new CPMAssembly.CPMAssemblyCollection();
		public CPMAssembly.CPMAssemblyCollection Assemblies
		{
			get { return mvarAssemblies; }
		}

		private static CPMLanguage mvarCurrentLanguage = null;
		public static CPMLanguage CurrentLanguage
		{
			get { return mvarCurrentLanguage; }
			set { mvarCurrentLanguage = value; }
		}

		private static CPMLanguage mvarDefaultLanguage = null;
		public static CPMLanguage DefaultLanguage
		{
			get { return mvarDefaultLanguage; }
			set { mvarDefaultLanguage = value; }
		}

		private static CPMLanguageCollection mvarLanguages = new CPMLanguageCollection();
		public static CPMLanguageCollection Languages
		{
			get
			{
				return mvarLanguages;
			}
		}
	}
}
