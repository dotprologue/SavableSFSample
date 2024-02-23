using Cysharp.Threading.Tasks;
using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SavableSFSample
{
    public class CompositeAnyNextNotifier : INextNotifier
    {
        private readonly IEnumerable<INextNotifier> nextNotifiers;

        public CompositeAnyNextNotifier(IEnumerable<INextNotifier> nextNotifiers)
        {
            if (nextNotifiers == null)
                throw new ArgumentNullException(nameof(nextNotifiers));
            if (nextNotifiers.Count() == 0)
            {
                throw new ArgumentException("No next-notifier was passed.");
            }
            this.nextNotifiers = nextNotifiers;
        }

        //Trigger the next-instruction when any task of next-notifier finishes
        public UniTask NotifyNextAsync(CancellationToken cancellationToken)
        {
            return UniTask.WhenAny(nextNotifiers.Select(notifier => notifier.NotifyNextAsync(cancellationToken)));
        }
    }
}
