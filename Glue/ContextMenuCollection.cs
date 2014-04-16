using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glue
{
    public class ContextMenuCollection
    {
        private Dictionary<string, Command> mvarItems = new Dictionary<string, Command>();
        public Command this[string name]
        {
            get
            {
                if (!mvarItems.ContainsKey(name))
                {
                    Command cmd = new Command();
                    cmd.Name = name;

                    mvarItems.Add(name, cmd);
                }
                return mvarItems[name];
            }
        }
    }
}
