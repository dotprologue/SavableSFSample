using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class SaveLoadPanel : ISavePanelOpener, ILoadPanelOpener, ILoadOnlyPanelOpener, IAsyncInitializable
    {
        private readonly IScenarioSaver scenarioSaver;
        private readonly IScenarioLoader scenarioLoader;
        private readonly IScenarioUnlocker scenarioUnlocker;
        private readonly IMessageWindow messageWindow;
        private readonly GameObject saveLoadPanelObject;
        private readonly GameObject scenarioDataWindowParent;
        private readonly TextMeshProUGUI titleText;
        private readonly Button saveModeButton;
        private readonly Button loadModeButton;
        private readonly Button closeButton;
        private readonly Button homeButton;
        private readonly Button exitButton;
        private readonly Sprite buttonSpriteOnEnable;
        private readonly Sprite buttonSpriteOnDisable;
        private readonly Sprite buttonSpriteOnInactivate;

        private PanelMode panelMode = PanelMode.Closed;

        public SaveLoadPanel(IScenarioSaver scenarioSaver, IScenarioLoader scenarioLoader, IScenarioUnlocker scenarioUnlocker, IMessageWindow messageWindow, Settings settings)
        {
            this.scenarioSaver = scenarioSaver ?? throw new ArgumentNullException(nameof(scenarioSaver));
            this.scenarioLoader = scenarioLoader ?? throw new ArgumentNullException(nameof(scenarioLoader));
            this.scenarioUnlocker = scenarioUnlocker ?? throw new ArgumentNullException(nameof(scenarioUnlocker));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            saveLoadPanelObject = settings.SaveLoadPanelObject ?? throw new ArgumentNullException(nameof(settings.SaveLoadPanelObject));
            saveLoadPanelObject.transform.position = Vector3.zero;
            saveLoadPanelObject.SetActive(false);
            scenarioDataWindowParent = settings.ScenarioDataWindowParent ?? throw new ArgumentNullException(nameof(settings.ScenarioDataWindowParent));
            titleText = settings.TitleText ?? throw new ArgumentNullException(nameof(settings.TitleText));
            saveModeButton = settings.SaveModeButton ?? throw new ArgumentNullException(nameof(settings.SaveModeButton));
            loadModeButton = settings.LoadModeButton ?? throw new ArgumentNullException(nameof(settings.LoadModeButton));
            closeButton = settings.CloseButton ?? throw new ArgumentNullException(nameof(closeButton));
            homeButton = settings.HomeButton ?? throw new ArgumentNullException(nameof(homeButton));
            exitButton = settings.ExitButton ?? throw new ArgumentNullException(nameof(exitButton));
            buttonSpriteOnEnable = settings.ButtonSpriteOnEnable ?? throw new ArgumentNullException(nameof(settings.ButtonSpriteOnEnable));
            buttonSpriteOnDisable = settings.ButtonSpriteOnDisable ?? throw new ArgumentNullException(nameof(settings.ButtonSpriteOnDisable));
            buttonSpriteOnInactivate = settings.ButtonSpriteOnInactivate ?? throw new ArgumentNullException(nameof(settings.ButtonSpriteOnInactivate));
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            var scenarioCoverDictionary = await scenarioLoader.GetScenarioCoversAsync(cancellationToken);
            var scenarioDataWindows = scenarioDataWindowParent.GetComponentsInChildren<ScenarioDataWindow>().ToArray();
            foreach (var index in Enumerable.Range(0, scenarioDataWindows.Length))
            {
                if (scenarioCoverDictionary.ContainsKey(index))
                {
                    scenarioDataWindows[index].SetCover(scenarioCoverDictionary[index]);
                }
                else
                {
                    scenarioDataWindows[index].ClearWindow();
                }
            }
        }

        public void OpenSavePanel()
        {
            Activate();
            ShowSavePanel();
            saveLoadPanelObject.SetActive(true);
        }

        public void OpenLoadPanel()
        {
            Activate();
            ShowLoadPanel();
            saveLoadPanelObject.SetActive(true);
        }

        public void OpenLoadOnlyPanel()
        {
            Activate();
            titleText.text = "Load";
            saveModeButton.GetComponent<Image>().sprite = buttonSpriteOnInactivate;
            loadModeButton.GetComponent<Image>().sprite = buttonSpriteOnEnable;
            homeButton.GetComponent<Image>().sprite = buttonSpriteOnInactivate;
            exitButton.GetComponent<Image>().sprite = buttonSpriteOnInactivate;
            homeButton.interactable = false;
            exitButton.interactable = false;
            panelMode = PanelMode.LoadOnly;
            saveLoadPanelObject.SetActive(true);
        }

        private void Activate()
        {
            UniTask.Void(async () =>
            {
                var tokenOnDestroy = closeButton.GetCancellationTokenOnDestroy();
                var tokenSourceOnClose = new CancellationTokenSource();
                var tokenOnClose = tokenSourceOnClose.Token;
                try
                {
                    //Scenario data windows
                    var scenarioDataWindows = scenarioDataWindowParent.GetComponentsInChildren<ScenarioDataWindow>().ToArray();
                    foreach (var (scenarioDataWindow, index) in scenarioDataWindows.Select((w, i) => (w, i)))
                    {
                        scenarioDataWindow.OnClickAsAsyncEnumerable(tokenOnClose)
                        .ForEachAwaitWithCancellationAsync(async (_, token) =>
                        {
                            if (panelMode == PanelMode.Save)
                            {
                                var scenarioCover = await scenarioSaver.SaveScenarioAsync(index, cancellationToken: token);
                                scenarioDataWindow.SetCover(scenarioCover);
                            }
                            else if (panelMode == PanelMode.Load || panelMode == PanelMode.LoadOnly)
                            {
                                await scenarioLoader.LoadScenarioAsync(index, cancellationToken: token);
                            }
                        }, cancellationToken: tokenOnClose).Forget();
                    }
                    //Save mode button
                    saveModeButton.OnClickAsAsyncEnumerable(cancellationToken: tokenOnClose)
                        .Where(_ => panelMode == PanelMode.Load)
                        .ForEachAsync(_ =>
                        {
                            ShowSavePanel();
                        }, cancellationToken: tokenOnClose).Forget();
                    saveModeButton.GetAsyncPointerEnterTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ =>
                    {
                        if (panelMode == PanelMode.Load)
                        {
                            saveModeButton.GetComponent<Image>().sprite = buttonSpriteOnEnable;
                        }
                        messageWindow.ShowMessage(Message.Save);
                    }, cancellationToken: tokenOnClose).Forget();
                    saveModeButton.GetAsyncPointerExitTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ =>
                    {
                        if (panelMode == PanelMode.Load)
                        {
                            saveModeButton.GetComponent<Image>().sprite = buttonSpriteOnDisable;
                        }
                        messageWindow.ClearMessage();
                    }, cancellationToken: tokenOnClose).Forget();
                    //Load mode button
                    loadModeButton.OnClickAsAsyncEnumerable(cancellationToken: tokenOnClose)
                        .Where(_ => panelMode == PanelMode.Save)
                        .ForEachAsync(_ =>
                        {
                            ShowLoadPanel();
                        }, cancellationToken: tokenOnClose).Forget();
                    loadModeButton.GetAsyncPointerEnterTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ =>
                    {
                        if (panelMode == PanelMode.Save)
                        {
                            loadModeButton.GetComponent<Image>().sprite = buttonSpriteOnEnable;
                        }
                        messageWindow.ShowMessage(Message.Load);
                    }, cancellationToken: tokenOnClose).Forget();
                    loadModeButton.GetAsyncPointerExitTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ =>
                    {
                        if (panelMode == PanelMode.Save)
                        {
                            loadModeButton.GetComponent<Image>().sprite = buttonSpriteOnDisable;
                        }
                        messageWindow.ClearMessage();
                    }, cancellationToken: tokenOnClose).Forget();
                    //Close Button
                    closeButton
                    .GetAsyncPointerEnterTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ => messageWindow.ShowMessage(Message.Close), cancellationToken: tokenOnClose).Forget();
                    closeButton
                    .GetAsyncPointerExitTrigger()
                    .AsUniTaskAsyncEnumerable()
                    .ForEachAsync(_ => messageWindow.ClearMessage(), cancellationToken: tokenOnClose).Forget();
                    //Wait until the close button is clicked
                    await closeButton.OnClickAsAsyncEnumerable(cancellationToken: tokenOnDestroy)
                    .FirstOrDefaultAsync(cancellationToken: tokenOnDestroy);
                    messageWindow.ClearMessage();
                    //Close the panel
                    saveLoadPanelObject.SetActive(false);
                    if (panelMode == PanelMode.Save || panelMode == PanelMode.Load)
                    {
                        scenarioUnlocker.Unlock();
                    }
                    panelMode = PanelMode.Closed;
                }
                finally
                {
                    tokenSourceOnClose?.Cancel();
                    tokenSourceOnClose?.Dispose();
                }
            });
        }

        private void ShowSavePanel()
        {
            titleText.text = "Save";
            saveModeButton.GetComponent<Image>().sprite = buttonSpriteOnEnable;
            loadModeButton.GetComponent<Image>().sprite = buttonSpriteOnDisable;
            homeButton.interactable = true;
            exitButton.interactable = true;
            panelMode = PanelMode.Save;
        }

        private void ShowLoadPanel()
        {
            titleText.text = "Load";
            saveModeButton.GetComponent<Image>().sprite = buttonSpriteOnDisable;
            loadModeButton.GetComponent<Image>().sprite = buttonSpriteOnEnable;
            homeButton.interactable = true;
            exitButton.interactable = true;
            panelMode = PanelMode.Load;
        }

        [Serializable]
        public class Settings
        {
            public GameObject SaveLoadPanelObject;
            public GameObject ScenarioDataWindowParent;
            public TextMeshProUGUI TitleText;
            public Button SaveModeButton;
            public Button LoadModeButton;
            public Button CloseButton;
            public Button HomeButton;
            public Button ExitButton;
            public Sprite ButtonSpriteOnEnable;
            public Sprite ButtonSpriteOnDisable;
            public Sprite ButtonSpriteOnInactivate;
        }

        private enum PanelMode
        {
            Closed,
            Save,
            Load,
            LoadOnly,
        }
    }
}
