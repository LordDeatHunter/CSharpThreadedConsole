using System.Collections.Generic;

namespace ThreadedConsole.Commands
{
    public sealed class ExitCommand : Command
    {
        public static readonly ExitCommand Instance = new();
        public override HashSet<string> Aliases { get; } = new();

        private ExitCommand()
        {
            Aliases.Add("quit");
            Aliases.Add("exit");
            Aliases.Add("stop");
            Aliases.Add("leave");
        }
        
        public override void Execute(IEnumerable<string> arguments, ConsoleManager consoleManager)
        {
            consoleManager.EnqueueMessage(new Message("Stopping server..."));
            consoleManager.Stop();
        }
    }
    
}