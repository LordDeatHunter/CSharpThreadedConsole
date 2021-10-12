using System.Collections.Generic;

namespace ThreadedConsole.Commands
{
    public abstract class Command
    {
        public abstract void Execute(string[] strings, ConsoleManager consoleManager);
        public abstract HashSet<string> Aliases { get; }
    }
    
}