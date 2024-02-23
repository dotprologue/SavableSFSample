using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class ButtonNotifier : INextNotifier, ICancellationNotifier
    {
        //Buttons to be observed
        private readonly IEnumerable<Button> buttons;

        public ButtonNotifier(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.Buttons == null)
                throw new ArgumentNullException(nameof(settings.Buttons));
            if (settings.Buttons.Count() == 0)
                throw new ArgumentException("No button to be observed was passed.");

            buttons = settings.Buttons;
        }

        //Trigger the next-instruction when any button is clicked
        public UniTask NotifyNextAsync(CancellationToken cancellationToken)
        {
            return WaitAnyButtonClicked(cancellationToken);
        }

        //Trigger the cancellation-instruction when any button is clicked
        public UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
        {
            return WaitAnyButtonClicked(cancellationToken);
        }

        //Wait until any button is clicked
        private UniTask WaitAnyButtonClicked(CancellationToken cancellationToken)
        {
            return UniTask.WhenAny(buttons.Select(button => button.
            OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)));
        }

        [Serializable]
        public class Settings
        {
            //Buttons to be observed
            public Button[] Buttons;
        }
    }
}
