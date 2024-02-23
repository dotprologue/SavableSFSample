using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class FunctionSaveButtonSupervisor : ICancellationInitializable
    {
        private readonly ISavePanelOpener savePanelOpener;
        private readonly IScenarioLocker scenarioLocker;
        private readonly IMessageWindow messageWindow;
        private readonly Button saveButton;

        public FunctionSaveButtonSupervisor(ISavePanelOpener savePanelOpener, IScenarioLocker scenarioLocker, IMessageWindow messageWindow, Settings settings)
        {
            this.savePanelOpener = savePanelOpener ?? throw new ArgumentNullException(nameof(savePanelOpener));
            this.scenarioLocker = scenarioLocker ?? throw new ArgumentNullException(nameof(scenarioLocker));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            saveButton = settings.SaveButton ?? throw new ArgumentNullException(nameof(saveButton));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            saveButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    scenarioLocker.Lock();
                    savePanelOpener.OpenSavePanel();
                }, cancellationToken: cancellationToken);
            saveButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Save), cancellationToken: cancellationToken);
            saveButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public Button SaveButton;
        }
    }
}
