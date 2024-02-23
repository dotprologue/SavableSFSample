using ScenarioFlow;

namespace SavableSFSample
{
    public interface IScenarioStatusUpdater
    {
        void StartUpdate(string scenarioScriptPath, ScenarioBook observedScenarioBook);
        void ContinueUpdate(string scenarioScriptPath, ScenarioBook observedScenarioBook);
        void StopUpdate();
    }
}