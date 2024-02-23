using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IScenarioRecoverable
    {
        ScenarioRecord Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer);
        UniTask RecoverAsync(ScenarioRecord scenarioRecord, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer, CancellationToken cancellationToken);
    }
}
