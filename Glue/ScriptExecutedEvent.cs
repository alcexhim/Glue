using System;
namespace Glue
{
	public delegate void ScriptExecutedEventHandler(object sender, ScriptExecutedEventArgs e);
	
	public class ScriptExecutedEventArgs : EventArgs
	{
		private Flame.Script mvarScript = null;
		/// <summary>
		/// The script to execute.
		/// </summary>
		public Flame.Script Script
		{
			get { return mvarScript; }
		}
		
		public ScriptExecutedEventArgs(Flame.Script script)
		{
			mvarScript = script;
		}
	}
}

