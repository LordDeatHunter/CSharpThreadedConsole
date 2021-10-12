using System;

namespace ThreadedConsole
{

    public class Message
    {
        private readonly Func<string> _text;
        private readonly bool _showTime;
        private readonly MessageType _messageType;
        private readonly string _origin;

        public Message(string text, string origin = "", bool showTime = true, MessageType messageType = MessageType.Info) :
            this(() => text, origin, showTime, messageType) { }

        public Message(Func<string> text, string origin = "", bool showTime = true, MessageType messageType = MessageType.Info)
        {
            _text = text;
            _origin = origin;
            _showTime = showTime;
            _messageType = messageType;
        }

        public string GetText() => _text.Invoke();

        private static string GetTimeFormatted()
        {
            return "[" + GetTime() + "] ";
        }
        private static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public enum MessageType
        {
            Info,
            Error,
            Warn,
            Success,
            Send
        }

        public int PrintLine()
        {
            var message = GetText();
            var outputLength = message.Length;
            var prefix = GetPrefix();
            if (prefix != "")
            {
                outputLength += prefix.Length;
                Console.Write(prefix);
            }
            Console.ForegroundColor = _messageType switch
            {
                MessageType.Error => ConsoleColor.Red,
                MessageType.Warn => ConsoleColor.Yellow,
                MessageType.Success => ConsoleColor.Green,
                MessageType.Send => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };
            Console.WriteLine(message + new string(' ', Console.BufferWidth - outputLength - 1));
            Console.ResetColor();
            return outputLength;
        }

        private string GetPrefix()
        {
            return _showTime switch
            {
                true when _origin != "" => "[" + GetTime() + " | " + _origin + "]: ",
                true when _origin == "" => GetTimeFormatted(),
                false when _origin != "" => GetOriginFormatted(),
                _ => ""
            };
        }

        private string GetOriginFormatted()
        {
            return "[" + _origin + "]: ";
        }
    }
}