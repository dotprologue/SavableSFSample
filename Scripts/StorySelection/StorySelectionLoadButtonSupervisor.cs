using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class StorySelectionLoadButtonSupervisor : ICancellationInitializable
    {
        private readonly ILoadOnlyPanelOpener loadOnlyPanelOpener;
        private readonly IMessageWindow messageWindow;
        private readonly Button loadButton;

        public StorySelectionLoadButtonSupervisor(ILoadOnlyPanelOpener loadOnlyPanelOpener, IMessageWindow messageWindow, Settings settings)
        {
            this.loadOnlyPanelOpener = loadOnlyPanelOpener ?? throw new ArgumentNullException(nameof(loadOnlyPanelOpener));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            loadButton = settings.LoadButton ?? throw new ArgumentNullException(nameof(settings.LoadButton));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            loadButton
                .OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    loadOnlyPanelOpener.OpenLoadOnlyPanel();
                }, cancellationToken: cancellationToken);
            loadButton
                .GetAsyncPointerEnterTrigger()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Load), cancellationToken: cancellationToken);
            loadButton
                .GetAsyncPointerExitTrigger()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button LoadButton;
        }
    }
}
