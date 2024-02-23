using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioScriptPlayer
    {
        UniTask StartAsync(string scenarioScriptPath, CancellationToken cancellationToken);
    }
}
