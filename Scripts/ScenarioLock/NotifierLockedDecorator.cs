using Cysharp.Threading.Tasks;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    //Don't trigger either the next-instruction and the cancellation-instruction while the scenario progression is locked
    public class NotifierLockedDecorator : INextNotifier, ICancellationNotifier
    {
        private readonly INextNotifier nextNotifier;
        private readonly ICancellationNotifier cancellationNotifier;
        private readonly IScenarioLockGetter scenarioLockGetter;

        public NotifierLockedDecorator(INextNotifier nextNotifier, ICancellationNotifier cancellationNotifier, IScenarioLockGetter scenarioLockGetter)
        {
            this.nextNotifier = nextNotifier ?? throw new ArgumentNullException(nameof(nextNotifier));
            this.cancellationNotifier = cancellationNotifier ?? throw new ArgumentNullException(nameof(cancellationNotifier));
            this.scenarioLockGetter = scenarioLockGetter ?? throw new ArgumentNullException(nameof(scenarioLockGetter));
        }

        public async UniTask NotifyNextAsync(CancellationToken cancellationToken)
        {
            do
            {
                await nextNotifier.NotifyNextAsync(cancellationToken);
            }
            while (scenarioLockGetter.IsLocked);
        }

        public async UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
        {
            do
            {
                await cancellationNotifier.NotifyCancellationAsync(cancellationToken);
            }
            while (scenarioLockGetter.IsLocked);
        }
    }
}