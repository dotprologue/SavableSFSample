using ScenarioFlow;
using ScenarioFlow.Scripts;

namespace SavableSFSample
{
    public interface IRecoveryStatusRecordStarter
    {
        void Start(ScenarioScript scenarioScript, ScenarioBook scenarioBook);
    }
}