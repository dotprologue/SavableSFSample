namespace SavableSFSample
{
    public interface IScenarioStatusSetter
    {
        string ScenarioScriptPath { set; }
        int RestartIndex { set; }
    }
}