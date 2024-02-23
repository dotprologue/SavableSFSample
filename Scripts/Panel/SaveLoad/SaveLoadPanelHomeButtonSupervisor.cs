using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class SaveLoadPanelHomeButtonSupervisor : ICancellationInitializable
    {
        private readonly ISceneReloader sceneReloader;
        private readonly IMessageWindow messageWindow;
        private readonly Button homeButton;

        public SaveLoadPanelHomeButtonSupervisor(ISceneReloader sceneReloader, IMessageWindow messageWindow, Settings settings)
        {
            this.sceneReloader = sceneReloader ?? throw new ArgumentNullException(nameof(sceneReloader));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            homeButton = settings.HomeButton ?? throw new ArgumentNullException(nameof(settings.HomeButton));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            homeButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    sceneReloader.ReloadScene();
                }, cancellationToken: cancellationToken);
            homeButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Home), cancellationToken: cancellationToken);
            homeButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button HomeButton;
        }
    }
}
