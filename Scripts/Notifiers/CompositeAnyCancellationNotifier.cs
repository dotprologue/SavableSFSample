using Cysharp.Threading.Tasks;
using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SavableSFSample
{
    public class CompositeAnyCancellationNotifier : ICancellationNotifier
    {
        private readonly IEnumerable<ICancellationNotifier> cancellationNotifiers;

        public CompositeAnyCancellationNotifier(IEnumerable<ICancellationNotifier> cancellationNotifiers)
        {
            if (cancellationNotifiers == null)
                throw new ArgumentNullException(nameof(cancellationNotifiers));
            if (cancellationNotifiers.Count() == 0)
                throw new ArgumentException("No cancellation-notifier was passed.");

            this.cancellationNotifiers = cancellationNotifiers;
        }

        //Trigger the cancellation-instruction when any task of cancellation-notifier finishes
        public UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
        {
            return UniTask.WhenAny(cancellationNotifiers.Select(notifier => notifier.NotifyCancellationAsync(cancellationToken)));
        }

    }
}
