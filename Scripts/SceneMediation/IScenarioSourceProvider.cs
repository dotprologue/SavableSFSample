namespace SavableSFSample
{
    public interface IScenarioSourceProvider
    {
        bool TryAcceptNewScenario(out string scenarioScriptPath);
        bool TryAcceptRecovery(out LogRecord recoveryLogRecord);
    }
}
