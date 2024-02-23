using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioLoader
    {
        UniTask LoadScenarioAsync(int scenarioNumber, CancellationToken cancellationToken);

        UniTask<IReadOnlyDictionary<int, ScenarioCover>> GetScenarioCoversAsync(CancellationToken cancellationToken);
    }
}
