using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioSaver
    {
        UniTask<ScenarioCover> SaveScenarioAsync(int scenarioNumber, CancellationToken cancellationToken);
    }
}
