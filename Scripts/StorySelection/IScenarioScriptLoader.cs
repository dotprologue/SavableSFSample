using Cysharp.Threading.Tasks;
using ScenarioFlow.Scripts;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioScriptLoader
    {
        UniTask<ScenarioScript> LoadAsync(string scenarioScriptPath, CancellationToken cancellationToken);
        UniTask UnloadAsync(string scenarioScriptPath, CancellationToken cancellationToken);
    }
}
