using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine.UI;
using Cysharp.Threading.Tasks.Triggers;

namespace SavableSFSample
{
    public class StorySelectionExitButtonSupervisor : ICancellationInitializable
    {
        private readonly IMessageWindow messageWindow;
        private readonly Button exitButton;

        public StorySelectionExitButtonSupervisor(IMessageWindow messageWindow, Settings settings)
        {
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            exitButton = settings.ExitButton ?? throw new ArgumentNullException(nameof(settings.ExitButton));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            exitButton
                .OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                UnityEngine.Application.Quit();
#endif
                }, cancellationToken: cancellationToken);
            exitButton
                .GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Exit));
            exitButton
                .GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button ExitButton;
        }
    }
}