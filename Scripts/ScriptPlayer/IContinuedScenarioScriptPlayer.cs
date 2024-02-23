using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IContinuedScenarioScriptPlayer
    {
        UniTask ContinueAsync(string scenarioScriptPath, int restartIndex, CancellationToken cancellationToken);
    }
}
