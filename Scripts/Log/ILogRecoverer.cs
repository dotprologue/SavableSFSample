namespace SavableSFSample
{
    public interface ILogRecoverer
    {
        ScenarioRecord Recover(LogRecord logRecord);
    }
}