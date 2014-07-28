using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using UniversalEditor;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.Accessors;

namespace Glue.Common
{
	public static class Methods
	{

		public static void SendApplicationEvent(Glue.ApplicationEventEventArgs e)
		{
			Plugin[] plugins = GetAvailablePlugins();
			foreach (Plugin plugin in plugins)
			{
				if (!plugin.Enabled) continue;

				e.mvarPlugin = plugin;

                plugin.OnApplicationEvent(e);
                Type type = plugin.GetType();

				Events.SendApplicationEvent(e);

				if (e.CancelPlugins) return;
			}
		}

		private static Plugin[] mvarAvailablePlugins = null;
		public static Plugin[] GetAvailablePlugins()
		{
			if (mvarAvailablePlugins == null)
			{
				List<Plugin> plugins = new List<Plugin>();

				Assembly[] assemblies = Mirror.Methods.GetAvailableAssemblies();
				foreach (Assembly asm in assemblies)
				{
					try
					{
						foreach (Type t in asm.GetTypes())
						{
							if (!(!t.IsSubclassOf(typeof(Plugin)) || t.IsAbstract))
							{
								try
								{
                                    Plugin plug = (Plugin)asm.CreateInstance(t.FullName);
                                    bool found = false;
                                    foreach (Plugin plu1 in plugins)
                                    {
                                        if (plu1.GetType().FullName == plug.GetType().FullName)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        plugins.Add(plug);
                                    }
								}
								catch (TargetInvocationException ex)
								{
									string message = ex.Message;
									if (ex.InnerException != null)
									{
										message = ex.InnerException.Message;
									}
									Console.WriteLine("glue: cannot load plugin " + t.FullName + ": " + message);
								}
							}
						}
					}
					catch (ReflectionTypeLoadException ex)
					{
					}
				}

				plugins.Sort();
				mvarAvailablePlugins = plugins.ToArray();
			}
			return mvarAvailablePlugins;
		}

		public static void LoadControlProperties(string FileName, params string[] XMLPath)
		{
			Console.WriteLine("glue: loading control properties from file \"" + FileName + "\"");

			MarkupObjectModel mom = new MarkupObjectModel();

			Document.Load(mom, new XMLDataFormat(), new FileAccessor(FileName), true);

			MarkupTagElement el = (mom.FindElement(XMLPath) as MarkupTagElement);
			if (el == null)
			{
				Console.WriteLine("glue: invalid Control Properties Schema document");
				return;
			}

			ControlPropertiesManager.CPMLanguage.Languages.Update(-1, "Default Language");

			foreach (MarkupElement el1 in el.Elements)
			{
				if (el1 is MarkupTagElement)
				{
					MarkupTagElement tag = (el1 as MarkupTagElement);
					if (el1.Name == "Language")
					{
						if (tag.Attributes["ID"] == null)
						{
							Console.WriteLine("glue: not loading language without id");
							continue;
						}

						int languageID = Int32.Parse(tag.Attributes["ID"].Value);
						string languageName = String.Empty;
						try
						{
							languageName = System.Globalization.CultureInfo.GetCultureInfo(languageID).DisplayName;
							Console.WriteLine("glue: application supports language \"" + languageName + "\"");
						}
						catch (ArgumentException)
						{
							Console.WriteLine("glue: system does not support language " + languageID.ToString());
							continue;
						}

						ControlPropertiesManager.CPMLanguage lang = ControlPropertiesManager.CPMLanguage.Languages.Update(languageID, languageName);
						LoadLanguage(lang, tag);

						if (tag.Attributes["IsDefault"] != null)
						{
							if (tag.Attributes["IsDefault"].Value == "true")
							{
								ControlPropertiesManager.CPMLanguage.DefaultLanguage = lang;
							}
						}
					}
					else if (el1.Name == "Assembly")
					{
						LoadAssembly(ControlPropertiesManager.CPMLanguage.Languages[-1], tag);
					}
				}
			}

			ControlPropertiesManager.CPMLanguage lango = ControlPropertiesManager.CPMLanguage.Languages[System.Globalization.CultureInfo.CurrentCulture.LCID];
			if (lango != null)
			{
				ControlPropertiesManager.CPMLanguage.CurrentLanguage = lango;
			}
			else
			{
				ControlPropertiesManager.CPMLanguage.CurrentLanguage = ControlPropertiesManager.CPMLanguage.DefaultLanguage;
			}
		}

		private static void LoadLanguage(ControlPropertiesManager.CPMLanguage language, MarkupTagElement tag)
		{
			foreach (MarkupElement el in tag.Elements)
			{
				if (el is MarkupTagElement)
				{
					if (el.Name == "Assembly")
					{
						LoadAssembly(language, (el as MarkupTagElement));
					}
				}
			}
		}

		private static void LoadAssembly(ControlPropertiesManager.CPMLanguage language, MarkupTagElement tag)
		{
			ControlPropertiesManager.CPMAssembly asm = new ControlPropertiesManager.CPMAssembly();
			foreach (MarkupAttribute att in tag.Attributes)
			{
				if (att.Name == "ID")
				{
					asm.Name = att.Value;
				}
				else if (asm.Properties.ContainsKey(att.Name))
				{
					asm.Properties[att.Name] = att.Value;
				}
				else
				{
					asm.Properties.Add(att.Name, att.Value);
				}
			}

			foreach (MarkupElement el in tag.Elements)
			{
				if (el is MarkupTagElement)
				{
					if (el.Name == "Window")
					{
						LoadWindow(asm, (el as MarkupTagElement));
					}
				}
			}
			language.Assemblies.Add(asm);
		}

		private static void LoadWindow(ControlPropertiesManager.CPMAssembly asm, MarkupTagElement tag)
		{
			ControlPropertiesManager.CPMWindow wnd = new ControlPropertiesManager.CPMWindow();
			foreach (MarkupAttribute att in tag.Attributes)
			{
				if (att.Name == "ID")
				{
					wnd.Name = att.Value;
				}
				else if (wnd.Properties.ContainsKey(att.Name))
				{
					wnd.Properties[att.Name] = att.Value;
				}
				else
				{
					wnd.Properties.Add(att.Name, att.Value);
				}
			}
			foreach (MarkupElement el in tag.Elements)
			{
				if (el is MarkupTagElement)
				{
					if (el.Name == "Control")
					{
						LoadControl(wnd, (el as MarkupTagElement));
					}
				}
			}
			asm.Windows.Add(wnd);
		}
		private static void LoadControl(ControlPropertiesManager.CPMWindow wnd, MarkupTagElement tag)
		{
			ControlPropertiesManager.CPMControl ctl = new ControlPropertiesManager.CPMControl();
			foreach (MarkupAttribute att in tag.Attributes)
			{
				if (att.Name == "ID")
				{
					ctl.Name = att.Value;
				}
				else if (ctl.Properties.ContainsKey(att.Name))
				{
					ctl.Properties[att.Name] = att.Value;
				}
				else
				{
					ctl.Properties.Add(att.Name, att.Value);
				}
			}
			wnd.Controls.Add(ctl);
		}

		/// <summary>
		/// Applies the control properties for the specified form and the assembly with the specified name.
		/// </summary>
		/// <param name="parent">The form on which to apply the control properties.</param>
		/// <param name="assemblyName">The assembly for which to load the control properties.</param>
		public static void ApplyControlProperties(Form parent, string assemblyName)
		{
			ControlPropertiesManager.CPMLanguage lang = ControlPropertiesManager.CPMLanguage.CurrentLanguage;
			if (lang == null) return;

			ControlPropertiesManager.CPMAssembly asm = lang.Assemblies[assemblyName];

			if (asm != null)
			{
				foreach (ControlPropertiesManager.CPMControl ctl in asm.Windows[parent.Name].Controls)
				{
					Type parentType = parent.GetType();
					FieldInfo fi = parentType.GetField(ctl.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

					if (fi == null) continue;

					object obj = fi.GetValue(parent);
					Type type = obj.GetType();

					foreach (KeyValuePair<string, string> p in ctl.Properties)
					{
						try
						{
							type.GetProperty(p.Key).SetValue(obj, p.Value, null);
						}
						catch (KeyNotFoundException)
						{
							Console.WriteLine("glue: could not apply control property \"" + p.Key + "\" to control \"" + ctl.Name + "\"");
						}
					}
				}
			}
		}


		[System.Security.SecurityCritical]
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
		private static extern string GetResourceFromDefault(string key);

		public static string GetResourceString(string key, params object[] values)
		{
			string resourceFromDefault = GetResourceFromDefault(key);
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, resourceFromDefault, values);
		}

		private static System.Collections.Generic.Dictionary<string, object> mvarGlobalVariables = new Dictionary<string, object>();
		public static object GetGlobalVariable(string name)
		{
			return GetGlobalVariable(name, null);
		}
		public static object GetGlobalVariable(string name, object defaultValue)
		{
			if (mvarGlobalVariables.ContainsKey(name))
			{
				return mvarGlobalVariables[name];
			}
			return defaultValue;
		}
		public static void SetGlobalVariable(string name, object value)
		{
			if (mvarGlobalVariables.ContainsKey(name))
			{
				mvarGlobalVariables[name] = value;
				return;
			}
			mvarGlobalVariables.Add(name, value);
		}

        public static void InitializeCustomizableMenuItems(MenuStrip menuStrip)
        {
            Plugin[] plugins = GetAvailablePlugins();
            foreach (Plugin plugin in plugins)
            {
                ToolStripMenuItem tsmiParent = null;
                foreach (Command cmd in plugin.MenuBar.Commands)
                {
                    if (tsmiParent == null)
                    {
                        if (menuStrip.Items.ContainsKey(cmd.Name))
                        {
                            tsmiParent = (menuStrip.Items[cmd.Name] as ToolStripMenuItem);
                        }
                        else
                        {
                            tsmiParent = new ToolStripMenuItem();
                            tsmiParent.Name = cmd.Name;
                            tsmiParent.Text = cmd.Text;
                            tsmiParent.Image = cmd.Image;
                            tsmiParent.Checked = cmd.Toggled;
                            tsmiParent.Click += tsmi_Click;
                            menuStrip.Items.Add(tsmiParent);
                        }
                    }
                    else
                    {
                        if (tsmiParent.DropDownItems.ContainsKey(cmd.Name))
                        {
                            tsmiParent = (tsmiParent.DropDownItems[cmd.Name] as ToolStripMenuItem);
                        }
                        else
                        {
                            ToolStripMenuItem tsmi = new ToolStripMenuItem();
                            tsmi.Name = cmd.Name;
                            tsmi.Text = cmd.Text;
                            tsmi.Image = cmd.Image;
                            tsmi.Checked = cmd.Toggled;
                            tsmi.Tag = cmd;
                            tsmiParent.DropDownItems.Add(tsmi);

                            tsmiParent = tsmi;
                        }
                    }
                    RecursiveLoadMenuItem(tsmiParent, cmd);
                }
            }
        }

        public static void InitializeCustomizableMenuItems(ContextMenuStrip menuStrip)
        {
            Plugin[] plugins = GetAvailablePlugins();
            string name = menuStrip.Name;

            Control parent = menuStrip.Parent;
            while (parent != null)
            {
                name = parent.Name + "." + name;
                parent = parent.Parent;
            }

            foreach (Plugin plugin in plugins)
            {
                ToolStripMenuItem tsmiParent = null;
                if (plugin.ContextMenus[name] == null) continue;

                foreach (Command cmd in plugin.ContextMenus[name].Commands)
                {
                    if (tsmiParent == null)
                    {
                        if (menuStrip.Items.ContainsKey(cmd.Name))
                        {
                            tsmiParent = (menuStrip.Items[cmd.Name] as ToolStripMenuItem);
                        }
                        else
                        {
                            tsmiParent = new ToolStripMenuItem();
                            tsmiParent.Name = cmd.Name;
                            tsmiParent.Text = cmd.Text;
                            tsmiParent.Image = cmd.Image;
                            tsmiParent.Checked = cmd.Toggled;
                            tsmiParent.Click += tsmi_Click;

                            if (!cmd.UseDefaultPosition)
                            {
                                if (cmd.Position > 0)
                                {
                                    menuStrip.Items.Insert(cmd.Position, tsmiParent);
                                }
                                else
                                {
                                    menuStrip.Items.Insert(menuStrip.Items.Count + cmd.Position + 1, tsmiParent);
                                }
                            }
                            else
                            {
                                menuStrip.Items.Add(tsmiParent);
                            }
                            menuStrip.Items.Add(tsmiParent);
                        }
                    }
                    else
                    {
                        if (tsmiParent.DropDownItems.ContainsKey(cmd.Name))
                        {
                            tsmiParent = (tsmiParent.DropDownItems[cmd.Name] as ToolStripMenuItem);
                        }
                        else
                        {
                            ToolStripMenuItem tsmi = new ToolStripMenuItem();
                            tsmi.Name = cmd.Name;
                            tsmi.Text = cmd.Text;
                            tsmi.Image = cmd.Image;
                            tsmi.Checked = cmd.Toggled;
                            tsmi.Tag = cmd;

                            if (!cmd.UseDefaultPosition)
                            {
                                if (cmd.Position > 0)
                                {
                                    tsmiParent.DropDownItems.Insert(cmd.Position, tsmi);
                                }
                                else
                                {
                                    tsmiParent.DropDownItems.Insert(tsmiParent.DropDownItems.Count + cmd.Position + 1, tsmi);
                                }
                            }
                            else
                            {
                                tsmiParent.DropDownItems.Add(tsmi);
                            }
                            tsmiParent = tsmi;
                        }
                    }
                    RecursiveLoadMenuItem(tsmiParent, cmd);
                }
            }
        }

        private static void tsmi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
            if (tsmi == null) return;

            Command cmd = (tsmi.Tag as Command);
            if (cmd == null) return;

            cmd.Activate();
        }

        private static void RecursiveLoadMenuItem(ToolStripMenuItem tsmiParent, Command cmd)
        {
            foreach (Command cmd1 in cmd.Commands)
            {
                if (cmd1 is Command.CommandSeparator)
                {
                    ToolStripSeparator tss = new ToolStripSeparator();
                    if (!cmd1.UseDefaultPosition)
                    {
                        if (cmd1.Position > 0)
                        {
                            tsmiParent.DropDownItems.Insert(cmd1.Position, tss);
                        }
                        else
                        {
                            tsmiParent.DropDownItems.Insert(tsmiParent.DropDownItems.Count + cmd1.Position + 1, tss);
                        }
                    }
                    else
                    {
                        tsmiParent.DropDownItems.Add(tss);
                    }
                }
                else
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Name = cmd1.Name;
                    tsmi.Text = cmd1.Text;
                    tsmi.Image = cmd1.Image;
                    tsmi.Tag = cmd1;
                    tsmi.Click += tsmi_Click;
                    tsmi.Checked = cmd1.Toggled;

                    if (!cmd1.UseDefaultPosition)
                    {
                        if (cmd1.Position > 0)
                        {
                            tsmiParent.DropDownItems.Insert(cmd1.Position, tsmi);
                        }
                        else
                        {
                            tsmiParent.DropDownItems.Insert(tsmiParent.DropDownItems.Count + cmd1.Position + 1, tsmi);
                        }
                    }
                    else
                    {
                        tsmiParent.DropDownItems.Add(tsmi);
                    }

                    RecursiveLoadMenuItem(tsmi, cmd1);
                }
            }
        }
    }
}
