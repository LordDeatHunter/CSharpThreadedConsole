# C# - Threaded Console
Multi-threaded Console Input/Output.

# Setup
- Download or clone the repository
- Copy the files inside your project directory
- Create a class that implements `IConsoleContainer`
```cs
namespace ExampleNamespace
{
    public class Example : IConsoleContainer
    {
        public bool IsRunning { get; set; }
        private readonly ConsoleManager _consoleManager;
        public Example()
        {
            _consoleManager = new ConsoleManager(this);
            _consoleManager.AddCommand(ExitCommand.Instance);
            _consoleManager.AddCommand(SayCommand.Instance);
        }
        public void Stop()
        {
            IsRunning = false;
        }
    }
}
```

# Outputting to Conosole
```cs
var outputText = "Example Output!";
var sender = "User1";
var showTimestamp = true;
var messageType = Message.MessageType.Info;
var message = new(outputText, sender, showTimestamp, messageType);
_consoleManager.EnqueueMessage(message);
```

# Creating commands
```cs
namespace ExampleNamespace
{
    public sealed class ExampleCommand : Command
    {
        public static readonly ExampleCommand Instance = new();
        public override HashSet<string> Aliases { get; } = new();
        private ExampleCommand()
        {
            Aliases.Add("hello");
            Aliases.Add("greet");
            Aliases.Add("hi");
        }
        public override void Execute(string[] arguments, ConsoleManager consoleManager)
        {
            arguments = arguments.Skip(1).ToArray();
            var message = arguments.Length == 0 ?
                new Message("Incomplete Command!", "", false, Message.MessageType.Error) :
                new Message("Hello, " + arguments[0] + "!");
            consoleManager.EnqueueMessage(message);
        }
    }
}```
