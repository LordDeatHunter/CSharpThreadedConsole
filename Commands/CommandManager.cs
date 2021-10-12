using System.Collections.Generic;

namespace ThreadedConsole.Commands
{
    public class CommandManager
    {

        private readonly Dictionary<string, Command> _commandDictionary = new();
        private readonly ConsoleManager _consoleManager;

        public CommandManager(ConsoleManager consoleManager)
        {
            _consoleManager = consoleManager;
        }

        public void AddCommand(Command command)
        {
            foreach (var alias in command.Aliases)
            {
                _commandDictionary.Add(alias, command);
            }
        }

        public void Execute(string command)
        {
            var arguments = command.Split(" ");
            if (arguments.Length == 0)
            {
                return;
            }
            if (_commandDictionary.ContainsKey(arguments[0]))
            {
                _commandDictionary[arguments[0]].Execute(arguments, _consoleManager);
            }
            else
            {
                _consoleManager.EnqueueMessage(new Message("Invalid Command.", "", false, Message.MessageType.Error));
            }
        }
        
    }
    
}