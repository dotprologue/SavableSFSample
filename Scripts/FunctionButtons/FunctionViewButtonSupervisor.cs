using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks.Triggers;

namespace SavableSFSample
{
    public class FunctionViewButtonSupervisor : ICancellationInitializable
    {
        private readonly IScenarioLocker scenarioLocker;
        private readonly IScenarioUnlocker scenarioUnlocker;
        private readonly IMessageWindow messageWindow;
        private readonly Button viewButton;
        private readonly Button closeButton;
        private readonly GameObject[] HiddenObjects;

        public FunctionViewButtonSupervisor(IScenarioLocker scenarioLocker, IScenarioUnlocker scenarioUnlocker, IMessageWindow messageWindow, Settings settings)
        {
            this.scenarioLocker = scenarioLocker ?? throw new ArgumentNullException(nameof(scenarioLocker));
            this.scenarioUnlocker = scenarioUnlocker ?? throw new ArgumentNullException(nameof(scenarioUnlocker));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            viewButton = settings.ViewButton ?? throw new ArgumentNullException(nameof(settings.ViewButton));
            closeButton = settings.CloseButton ?? throw new ArgumentNullException(nameof(settings.CloseButton));
            closeButton.gameObject.SetActive(false);
            HiddenObjects = settings.HiddenObjects ?? throw new ArgumentNullException(nameof(settings.HiddenObjects));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            viewButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAwaitWithCancellationAsync(async (_, token) =>
                {
                    scenarioLocker.Lock();
                    closeButton.gameObject.SetActive(true);
                    foreach (var obj in HiddenObjects)
                    {
                        obj.SetActive(false);
                        messageWindow.ClearMessage();
                    }
                    await closeButton
                    .OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                    foreach (var obj in HiddenObjects)
                    {
                        obj.SetActive(true);
                    }
                    closeButton.gameObject.SetActive(false);
                    scenarioUnlocker.Unlock();
                }, cancellationToken: cancellationToken);
            viewButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.View), cancellationToken: cancellationToken);
            viewButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button ViewButton;
            public Button CloseButton;
            public GameObject[] HiddenObjects;
        }
    }
}
