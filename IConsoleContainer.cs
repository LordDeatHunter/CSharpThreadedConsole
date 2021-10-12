namespace ThreadedConsole
{
    public interface IConsoleContainer
    {
        public bool IsRunning { get; set; }
        public void Stop();
    }
}