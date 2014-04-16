using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Glue
{
    public struct GlueScriptEntryPointInformation
    {
        private string mvarEntryPoint;
        public string EntryPoint { get { return mvarEntryPoint; } set { mvarEntryPoint = value; } }

        private string mvarFileName;
        public string FileName { get { return mvarFileName; } set { mvarFileName = value; } }
    }

    [ProvideProperty("OnWindowOpened", typeof(System.Windows.Forms.Form))]
    public class GlueExtender : Component, IExtenderProvider
    {
        private Dictionary<System.Windows.Forms.Form, GlueScriptEntryPointInformation> mvarOnWindowOpened = new Dictionary<System.Windows.Forms.Form,GlueScriptEntryPointInformation>();
        public void SetOnWindowOpened(System.Windows.Forms.Form component, GlueScriptEntryPointInformation value)
        {
            if (mvarOnWindowOpened.ContainsKey(component))
            {
                mvarOnWindowOpened[component] = value;
            }
            else
            {
                mvarOnWindowOpened.Add(component, value);
            }
        }
        public GlueScriptEntryPointInformation GetOnWindowOpened(System.Windows.Forms.Form form)
        {
            if (mvarOnWindowOpened.ContainsKey(form))
            {
                return mvarOnWindowOpened[form];
            }
            return default(GlueScriptEntryPointInformation);
        }

        public bool CanExtend(object extendee)
        {
            return (extendee is System.Windows.Forms.Form || extendee is System.Windows.Forms.Control);
        }
    }
}
