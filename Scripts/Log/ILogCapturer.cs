namespace SavableSFSample
{
    public interface ILogCapturer
    {
        int ScenarioCount { get; }
        LogRecord Capture(int scenarioIndex);
    }
}