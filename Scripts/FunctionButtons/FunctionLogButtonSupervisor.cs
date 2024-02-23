using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class FunctionLogButtonSupervisor : ICancellationInitializable
    {
        private readonly ILogPanelOpener logPanelOpener;
        private readonly IScenarioLocker scenarioLocker;
        private readonly IScenarioLockGetter scenarioLockGetter;
        private readonly IMessageWindow messageWindow;
        private readonly Button logButton;

        public FunctionLogButtonSupervisor(ILogPanelOpener logPanelOpener, IScenarioLocker scenarioLocker, IScenarioLockGetter scenarioLockGetter, IMessageWindow messageWindow, Settings settings)
        {
            this.logPanelOpener = logPanelOpener ?? throw new ArgumentNullException(nameof(logPanelOpener));
            this.scenarioLocker = scenarioLocker ?? throw new ArgumentNullException(nameof(scenarioLocker));
            this.scenarioLockGetter = scenarioLockGetter ?? throw new ArgumentNullException(nameof(scenarioLockGetter));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.LogButton == null)
                throw new ArgumentNullException(nameof(settings));
            logButton = settings.LogButton;
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            logButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .Where(_ => !scenarioLockGetter.IsLocked)
                .ForEachAsync(_ =>
                {
                    scenarioLocker.Lock();
                    logPanelOpener.Open();
                }, cancellationToken: cancellationToken);
            logButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Log), cancellationToken: cancellationToken);
            logButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button LogButton;
        }
    }
}
