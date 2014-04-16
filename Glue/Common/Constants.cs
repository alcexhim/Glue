using System;
using System.Collections.Generic;
using System.Text;

namespace Glue.Common
{
	public static class Constants
	{
		public static class EventNames
		{
			public const string ApplicationStart = "ApplicationStart";
			public const string ApplicationStop = "ApplicationStop";

			public const string BeforeOpenFileDialog = "BeforeOpenFileDialog";
			public const string AfterOpenFileDialog = "AfterOpenFileDialog";

			public const string BeforeSaveFileDialog = "BeforeSaveFileDialog";
			public const string AfterSaveFileDialog = "AfterSaveFileDialog";

			public const string BeforeOpenFile = "BeforeOpenFile";
			public const string AfterOpenFile = "AfterOpenFile";

			public const string BeforeSaveFile = "BeforeSaveFile";
			public const string AfterSaveFile = "AfterSaveFile";

			public const string WindowCaptionUpdating = "WindowCaptionUpdating";

			public const string WindowClosing = "WindowClosing";
			public const string WindowClosed = "WindowClosed";

			public const string WindowShown = "AboutDialogShown";
		}
	}
}
