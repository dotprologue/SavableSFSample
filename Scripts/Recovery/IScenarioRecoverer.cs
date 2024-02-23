using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioRecoverer
    {
        UniTask RecoverAsync(ScenarioRecord scenarioRecord, CancellationToken cancellationToken);
    }
}