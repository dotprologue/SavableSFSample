namespace SavableSFSample
{
    public interface IScenarioStatusGetter
    {
        string ScenarioScriptPath { get; }
        int RestartIndex { get; }
    }
}