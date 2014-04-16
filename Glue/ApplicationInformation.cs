using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glue
{
	public static class ApplicationInformation
	{
		private static Guid mvarApplicationID = Guid.Empty;
		public static Guid ApplicationID { get { return mvarApplicationID; } set { mvarApplicationID = value; } }

        private static string mvarApplicationTitle = String.Empty;
        public static string ApplicationTitle { get { return mvarApplicationTitle; } set { mvarApplicationTitle = value; } }

        private static string mvarApplicationDataPath = String.Empty;
        public static string ApplicationDataPath { get; set; }
    }
}
