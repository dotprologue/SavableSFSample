using Cysharp.Threading.Tasks;
using ScenarioFlow;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioBookLoader
    {
        UniTask<ScenarioBook> LoadScenarioBookAsync(string scenarioScriptPath, CancellationToken cancellationToken);
        UniTask UnloadScenarioBookAsync(string scenarioScriptPath, CancellationToken cancellationToken);
    }
}
