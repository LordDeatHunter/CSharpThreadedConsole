using System.Collections.Generic;
using System.Linq;

namespace ThreadedConsole.Commands
{
    public sealed class SayCommand : Command
    {
        public static readonly SayCommand Instance = new();
        public override HashSet<string> Aliases { get; } = new();

        private SayCommand()
        {
            Aliases.Add("say");
        }

        public override void Execute(string[] arguments, ConsoleManager consoleManager)
        {
            var output = string.Join(' ', arguments.Skip(1));
            consoleManager.EnqueueMessage(
                output == ""
                    ? new Message("Incomplete Command!", "", false, Message.MessageType.Error)
                    : new Message(output, "Server")
            );
        }
    }
}