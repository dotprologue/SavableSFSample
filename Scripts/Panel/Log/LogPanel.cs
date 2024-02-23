using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class LogPanel : ILogPanelOpener
    {
        private readonly ILogObjectBuilder logObjectBuilder;
        private readonly IScenarioUnlocker scenarioUnlocker;
        private readonly IMessageWindow messageWindow;
        private readonly GameObject logPanelObject;
        private readonly Button closeButton;

        public LogPanel(ILogObjectBuilder logObjectBuilder, IScenarioUnlocker scenarioUnlocker, IMessageWindow messageWindow, Settings settings)
        {
            this.logObjectBuilder = logObjectBuilder ?? throw new ArgumentNullException(nameof(logObjectBuilder));
            this.scenarioUnlocker = scenarioUnlocker ?? throw new ArgumentNullException(nameof(scenarioUnlocker));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            logPanelObject = settings.LogPanelObject ?? throw new ArgumentNullException(nameof(settings.LogPanelObject));
            logPanelObject.SetActive(false);
            logPanelObject.transform.position = Vector3.zero;
            closeButton = settings.CloseButton ?? throw new ArgumentNullException(nameof(settings.CloseButton));
        }

        public void Open()
        {
            logPanelObject.SetActive(true);
            var logObjects = logObjectBuilder.BuildLogObjects().ToArray();
            var cancellationToken = logObjects[0].GetCancellationTokenOnDestroy();

            foreach (var logObject in logObjects)
            {
                logObject.JumpButton
                    .GetAsyncPointerEnterTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ => messageWindow.ShowMessage(Message.Jump), cancellationToken: cancellationToken);
                logObject.JumpButton
                    .GetAsyncPointerExitTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken);
            }

            //Close panel
            UniTask.Void(async token =>
            {
                closeButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ShowMessage(Message.Close), cancellationToken: token).Forget();
                closeButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: cancellationToken).Forget();
                //Wait until the close button is clicked
                await closeButton.OnClickAsAsyncEnumerable(token)
                .FirstOrDefaultAsync(cancellationToken: token);
                messageWindow.ClearMessage();
                logPanelObject.SetActive(false);
                foreach (var logObject in logObjects)
                {
                    GameObject.Destroy(logObject.gameObject);
                }
                scenarioUnlocker.Unlock();
            }, cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public GameObject LogPanelObject;
            public Button CloseButton;
        }
    }
}
