using System;

namespace SavableSFSample
{
    public class ScenarioCapturer : IScenarioCapturer
    {
        private readonly IScenarioRecoverable scenarioRecoverable;
        private readonly IPrimitiveSerializer primitiveSerializer;
        private readonly IAssetSerializer assetSerializer;

        public ScenarioCapturer(IScenarioRecoverable scenarioRecoverable, IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            this.scenarioRecoverable = scenarioRecoverable ?? throw new ArgumentNullException(nameof(scenarioRecoverable));
            this.primitiveSerializer = primitiveSerializer ?? throw new ArgumentNullException(nameof(primitiveSerializer));
            this.assetSerializer = assetSerializer ?? throw new ArgumentNullException(nameof(assetSerializer));
        }

        public ScenarioRecord Capture()
        {
            return scenarioRecoverable.Capture(primitiveSerializer, assetSerializer);
        }
    }
}
