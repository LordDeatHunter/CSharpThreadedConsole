using System;
using System.Collections.Generic;
using System.Threading;
using ThreadedConsole.Commands;

namespace ThreadedConsole
{
    public class ConsoleManager
    {
        private static readonly object ScreenLock = new();
        private string _currentInput = "";
        private readonly Queue<Message> _messageQueue = new();
        private readonly CommandManager _commandManager;
        private int _currentCursorTop;
        private int CursorTop
        {
            get => Math.Min(Console.BufferHeight - 1, _currentCursorTop);
            set => _currentCursorTop = Math.Min(value, Console.BufferHeight - 1);
        }
        private bool _updateInput;
        private readonly Thread _ioThread;
        private readonly Thread _printQueueThread;
        private const string Prompt = "> ";
        private readonly List<string> _history = new();
        private int _historyIndex;
        private readonly IConsoleContainer _consoleContainer;
        private bool _isRunning;

        public ConsoleManager(IConsoleContainer container)
        {
            _commandManager = new CommandManager(this);
            _consoleContainer = container;
            _updateInput = true;
            _history.Add("");
            _ioThread = new Thread(IOThread);
            _printQueueThread = new Thread(PrintQueue);
        }

        public void Start()
        {
            if (_isRunning)
            {
                EnqueueMessage(new Message("ConsoleManager is already started", "ConsoleManager", false, Message.MessageType.Error));
                return;
            }
            _isRunning = true;
            //Console.SetBufferSize(Console.BufferWidth, 3000);
            _currentCursorTop = Console.CursorTop;
            _ioThread.Start();
            _printQueueThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _consoleContainer.Stop();
        }

        private void IOThread()
        {
            while (_consoleContainer.IsRunning)
            {
                lock (ScreenLock)
                {
                    if (Console.KeyAvailable)
                    {
                        ProcessInput(Console.ReadKey(true));
                    }
                    
                    if (_updateInput)
                    {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(0, CursorTop);
                        Console.Write(Prompt + _currentInput + new string(' ',
                            Console.BufferWidth - _currentInput.Length - Prompt.Length - 1));
                        _updateInput = false;
                    }

                    Console.CursorVisible = true;
                    Console.SetCursorPosition(Math.Min(Console.BufferWidth, _currentInput.Length + Prompt.Length),
                        CursorTop);
                }
            }
        }

        private void PrintQueue()
        {
            while (_consoleContainer.IsRunning || _messageQueue.Count != 0)
            {
                lock (ScreenLock)
                {
                    if (_messageQueue.Count == 0)
                    {
                        continue;
                    }

                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, CursorTop);
                    var messageLength = _messageQueue.Dequeue().PrintLine();
                    CursorTop += (int) Math.Ceiling(messageLength / (float) Console.BufferWidth);
                    _updateInput = true;
                }
            }
            Console.CursorVisible = true;
        }

        private void ProcessInput(ConsoleKeyInfo key)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                {
                    EnqueueMessage(new Message("> " + _currentInput, "", false, Message.MessageType.Send));
                    _history.Add("");
                    _historyIndex = _history.Count - 1;
                    _commandManager.Execute(_currentInput);
                    _currentInput = "";
                    break;
                }
                case ConsoleKey.Backspace:
                {
                    if (_currentInput.Length > 0)
                        _currentInput = _currentInput.Remove(_currentInput.Length - 1);
                    break;
                }
                case ConsoleKey.Tab:
                {
                    //TODO: Implement Auto-complete
                    break;
                }
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                {
                    //TODO: Implement moving through input text
                    break;
                }
                case ConsoleKey.UpArrow:
                {
                    _historyIndex = Math.Max(0, _historyIndex - 1);
                    _currentInput = _history[_historyIndex];
                    break;
                }
                case ConsoleKey.DownArrow:
                {
                    _historyIndex = Math.Min(_history.Count - 1, _historyIndex + 1);
                    _currentInput = _history[_historyIndex];
                    break;
                }
                default:
                {
                    _currentInput += key.KeyChar;
                    _history[^1] = _currentInput;
                    break;
                }
            }
            _updateInput = true;
        }

        public void EnqueueMessage(Message message)
        {
            lock (ScreenLock)
            {
                _messageQueue.Enqueue(message);
            }
        }

        public void AddCommand(Command command)
        {
            _commandManager.AddCommand(command);
        }
    }
}