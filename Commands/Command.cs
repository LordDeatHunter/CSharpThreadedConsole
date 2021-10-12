using System.Collections.Generic;

namespace ThreadedConsole.Commands
{
    public abstract class Command
    {
        public abstract void Execute(IEnumerable<string> arguments, ConsoleManager consoleManager);
        public abstract HashSet<string> Aliases { get; }
    }
    
}