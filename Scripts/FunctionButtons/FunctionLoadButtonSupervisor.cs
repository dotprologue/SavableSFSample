using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class FunctionLoadButtonSupervisor : ICancellationInitializable
    {
        private readonly ILoadPanelOpener loadPanelOpener;
        private readonly IScenarioLocker scenarioLocker;
        private readonly IMessageWindow messageWindow;
        private readonly Button loadButton;

        public FunctionLoadButtonSupervisor(ILoadPanelOpener loadPanelOpener, IScenarioLocker scenarioLocker, IMessageWindow messageWindow, Settings settings)
        {
            this.loadPanelOpener = loadPanelOpener ?? throw new ArgumentNullException(nameof(loadPanelOpener));
            this.scenarioLocker = scenarioLocker ?? throw new ArgumentNullException(nameof(scenarioLocker));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            loadButton = settings.LoadButton ?? throw new ArgumentNullException(nameof(loadButton));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            loadButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    scenarioLocker.Lock();
                    loadPanelOpener.OpenLoadPanel();
                }, cancellationToken);
            loadButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Load), cancellationToken: cancellationToken);
            loadButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);

        }

        [Serializable]
        public class Settings
        {
            public Button LoadButton;
        }
    }
}