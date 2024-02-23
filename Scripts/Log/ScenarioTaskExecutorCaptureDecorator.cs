using Cysharp.Threading.Tasks;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class ScenarioTaskExecutorCaptureDecorator : IScenarioTaskExecutor
    {
        private readonly IScenarioTaskExecutor scenarioTaskExecutor;
        private readonly IScenarioCaptureExecutor scenarioCaptureExecutor;
        private readonly ITokenCodeGetter tokenCodeGetter;

        public ScenarioTaskExecutorCaptureDecorator(IScenarioTaskExecutor scenarioTaskExecutor, IScenarioCaptureExecutor scenarioCaptureExecutor, ITokenCodeGetter tokenCodeGetter)
        {
            this.scenarioTaskExecutor = scenarioTaskExecutor ?? throw new ArgumentNullException(nameof(scenarioTaskExecutor));
            this.scenarioCaptureExecutor = scenarioCaptureExecutor ?? throw new ArgumentNullException(nameof(scenarioCaptureExecutor));
            this.tokenCodeGetter = tokenCodeGetter ?? throw new ArgumentNullException(nameof(tokenCodeGetter));
        }

        public async UniTask ExecuteAsync(UniTask scenarioTask, CancellationToken cancellationToken)
        {
            await scenarioTaskExecutor.ExecuteAsync(scenarioTask, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                var tokenCode = tokenCodeGetter.TokenCode;
                if (SpecialTokenCodes.IsPlain(tokenCode) || SpecialTokenCodes.IsFluent(tokenCode))
                {
                    scenarioCaptureExecutor.ExecuteCapture();
                }
            }
        }
    }
}