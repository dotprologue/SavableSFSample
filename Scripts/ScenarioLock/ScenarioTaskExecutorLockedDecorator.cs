using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class ScenarioTaskExecutorLockedDecorator : IScenarioTaskExecutor
    {
        private readonly IScenarioTaskExecutor scenarioTaskExecutor;
        private readonly ITokenCodeGetter tokenCodeGetter;
        private readonly IScenarioLockGetter scenarioLockGetter;

        public ScenarioTaskExecutorLockedDecorator(IScenarioTaskExecutor scenarioTaskExecutor, ITokenCodeGetter tokenCodeGetter, IScenarioLockGetter scenarioLockGetter)
        {
            this.scenarioTaskExecutor = scenarioTaskExecutor ?? throw new ArgumentNullException(nameof(scenarioTaskExecutor));
            this.tokenCodeGetter = tokenCodeGetter ?? throw new ArgumentNullException(nameof(tokenCodeGetter));
            this.scenarioLockGetter = scenarioLockGetter ?? throw new ArgumentNullException(nameof(scenarioLockGetter));
        }

        //Stop the scenario progression when it is locked except when the execution method of the task is neither the serial nor the parallel
        public async UniTask ExecuteAsync(UniTask scenarioTask, CancellationToken cancellationToken)
        {
            await scenarioTaskExecutor.ExecuteAsync(scenarioTask, cancellationToken);
            if (scenarioLockGetter.IsLocked && (SpecialTokenCodes.IsStandard(tokenCodeGetter.TokenCode) || SpecialTokenCodes.IsFluent(tokenCodeGetter.TokenCode)))
            {
                //Wait until the scenario progression is unlocked
                await UniTaskAsyncEnumerable.EveryValueChanged(scenarioLockGetter, x => x.IsLocked)
                    .Where(x => !x)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            }
        }
    }
}