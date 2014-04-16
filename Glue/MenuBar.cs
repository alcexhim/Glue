using System;
namespace Glue
{
	public class MenuBar
	{
		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		public Command.CommandCollection Commands
		{
			get { return mvarCommands; }
		}
	}
}

