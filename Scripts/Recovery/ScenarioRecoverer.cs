using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class ScenarioRecoverer : IScenarioRecoverer
    {
        private readonly IScenarioRecoverable scenarioRecoverable;
        private readonly IPrimitiveDeserializer primitiveDeserializer;
        private readonly IAssetDeserializer assetDeserializer;

        public ScenarioRecoverer(IScenarioRecoverable scenarioRecoverable, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            this.scenarioRecoverable = scenarioRecoverable ?? throw new ArgumentNullException(nameof(scenarioRecoverable));
            this.primitiveDeserializer = primitiveDeserializer ?? throw new ArgumentNullException(nameof(primitiveDeserializer));
            this.assetDeserializer = assetDeserializer ?? throw new ArgumentNullException(nameof(assetDeserializer));
        }

        public async UniTask RecoverAsync(ScenarioRecord scenarioRecord, CancellationToken cancellationToken)
        {
            await scenarioRecoverable.RecoverAsync(scenarioRecord, primitiveDeserializer, assetDeserializer, cancellationToken);
        }
    }
}
