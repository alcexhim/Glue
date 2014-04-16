using System;
namespace Glue
{
	public class Environment
	{
		public event ScriptExecutedEventHandler ScriptExecuted;
		
		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		public Command.CommandCollection Commands
		{
			get { return mvarCommands; }
		}
		
		public void Execute (Flame.Script script)
		{
			if (ScriptExecuted != null)
			{
				ScriptExecuted (this, new ScriptExecutedEventArgs (script));
			}
		}
		public void Execute (Command cmd)
		{
			cmd.Activate ();
		}
	}
}

