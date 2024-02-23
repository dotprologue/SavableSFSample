using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class KeyNotifier : INextNotifier, ICancellationNotifier
    {
        //Keys to be observed
        private readonly IEnumerable<KeyCode> keyCodes;

        public KeyNotifier(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.KeyCodes == null)
                throw new ArgumentNullException(nameof(settings.KeyCodes));
            if (settings.KeyCodes.Count() == 0)
                throw new ArgumentException("No KeyCode to be observed was passed.");

            keyCodes = settings.KeyCodes;
        }

        //Trigger the next-instruction when any key is pressed
        public UniTask NotifyNextAsync(CancellationToken cancellationToken)
        {
            return WaitForAnyKeyDownAsync(cancellationToken);
        }

        //Trigger the cancellation-instruction when any key is pressed
        public UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
        {
            return WaitForAnyKeyDownAsync(cancellationToken);
        }

        //Wait until any key is pressed
        private UniTask WaitForAnyKeyDownAsync(CancellationToken cancellationToken)
        {
            return UniTaskAsyncEnumerable.EveryUpdate()
                .SelectMany(_ => keyCodes.ToUniTaskAsyncEnumerable().Select(code => Input.GetKeyDown(code)))
                .Where(x => x)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            //Keys to be observed
            public KeyCode[] KeyCodes;
        }
    }
}
