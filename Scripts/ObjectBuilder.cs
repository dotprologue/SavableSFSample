using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow;
using ScenarioFlow.Scripts;
using ScenarioFlow.Tasks;
using System;
using System.Linq;
using UnityEngine;

namespace SavableSFSample
{
	public class ObjectBuilder : MonoBehaviour
    {
        [SerializeField]
        BackgroundAnimator.Settings backgroundAnimatorSettings;
        [SerializeField]
        ButtonNotifier.Settings buttonNotifierSettings;
        [SerializeField]
        CharacterProvider.Settings characterProviderSettings;
        [SerializeField]
        DialogueWriter.Settings dialogueWriterSettings;
        [SerializeField]
        FunctionAutoButtonSupervisor.Settings functionAutoButtonSupervisorSettings;
        [SerializeField]
        FunctionSkipButtonSupervisor.Settings functionSkipButtonSupervisorSettings;
        [SerializeField]
        FunctionSaveButtonSupervisor.Settings functionSaveButtonSupervisorSettings;
        [SerializeField]
        FunctionLoadButtonSupervisor.Settings functionLoadButtonSupervisorSettings;
        [SerializeField]
        FunctionLogButtonSupervisor.Settings functionLogButtonSupervisorSettings;
        [SerializeField]
        FunctionViewButtonSupervisor.Settings functionViewButtonSupervisorSettings;
        [SerializeField]
        FunctionLockButtonSupervisor.Settings functionLockButtonSupervisorSettings;
        [SerializeField]
        KeyNotifier.Settings keyNotifierSettings;
        [SerializeField]
        LogObjectBuilder.Settings logObjectBuilderSettings;
        [SerializeField]
        LogPanel.Settings logPanelSettings;
        [SerializeField]
        MessageWindow.Settings messageWindowSettings;
        [SerializeField]
        PlayerSelectionPresenter.Settings playerSelectionPresenterSettings;
        [SerializeField]
        SaveLoadPanel.Settings saveLoadPanelSettings;
        [SerializeField]
        SaveLoadPanelExitButtonSupervisor.Settings saveLoadPanelExitButtonSupervisorSettings;
        [SerializeField]
        SaveLoadPanelHomeButtonSupervisor.Settings saveLoadPanelHomeButtonSupervisorSettings;
        [SerializeField]
        ScenarioCamera.Settings scenarioCameraSettings;
        [SerializeField]
        ScenarioLoader.Settings scenarioLoaderSettings;
        [SerializeField]
        ScenarioStarter.Settings scenarioStarterSettings;
        [SerializeField]
        SceneReloader.Settings sceneReloaderSettings;
        [SerializeField]
        SceneTransitionAnimator.Settings sceneTransitionAnimatorSettings;
        [SerializeField]
        StorySelectionExitButtonSupervisor.Settings storySelectionExitButtonSupervisorSettings;
        [SerializeField]
        StorySelectionLoadButtonSupervisor.Settings storySelectionLoadButtonSupervisorSettings;

        [SerializeField]
        GameObject storySelectionPanel;
        [SerializeField]
        ScenarioSourceMediator scenarioSourceMediatorPrefab;

        [SerializeField]
        bool debugMode;
        [SerializeField]
        ScenarioScript scenarioScript;

        private async UniTaskVoid Start()
        {
            var cancellationToken = this.GetCancellationTokenOnDestroy();

            var scenarioSourceMediator = GameObject.Find(nameof(ScenarioSourceMediator))?.GetComponent<ScenarioSourceMediator>();
            if (scenarioSourceMediator == null)
            {
                scenarioSourceMediator = Instantiate(scenarioSourceMediatorPrefab);
                scenarioSourceMediator.name = nameof(ScenarioSourceMediator);
                DontDestroyOnLoad(scenarioSourceMediator.gameObject);
            }
            var sceneReloader = new SceneReloader(sceneReloaderSettings);


            var jsonPrimitiveSerializer = new JsonPrimitiveSerializer();
            var staticPrimitiveMemory = new StaticPrimitiveMemory();
            var resourcesAssetLoader = new ResourcesAssetLoader();
            var assetStorage = new AssetStorage(resourcesAssetLoader);

            var scenarioStatusHolder = new ScenarioStatusHolder();

            var scenarioLockHolder = new ScenarioLockHolder();
            var logStorage = new LogStorage();

            var backgroundAnimator = new BackgroundAnimator(backgroundAnimatorSettings);
            var characterProvider = new CharacterProvider(characterProviderSettings);
            var characterAnimator = new CharacterAnimator();
            var dialogueWriter = new DialogueWriter(characterAnimator, logStorage, dialogueWriterSettings);
            var sceneTransitionAnimator = new SceneTransitionAnimator(sceneTransitionAnimatorSettings);

            var spriteProvider = new SpriteProvider(assetStorage, assetStorage);

            var scenarioRecoverable = new ScenarioRecoverable(
                new IRecoverable[]
                {
                    backgroundAnimator,
                    characterProvider,
                    dialogueWriter,
                    scenarioStatusHolder,
                    sceneTransitionAnimator,
                },
                new IAssetRecoverable[]
                {
                    spriteProvider,
                });

            var scenarioCapturer = new ScenarioCapturer(scenarioRecoverable, jsonPrimitiveSerializer, assetStorage);
            var scenarioCaptuererEventPublisherDecorator = new ScenarioCapturerEventPublisherDecorator(scenarioCapturer);
            var scenarioCaptureExecutor = new ScenarioCaptureExecutor(scenarioCaptuererEventPublisherDecorator, logStorage);
            var scenarioRecoverer = new ScenarioRecoverer(scenarioRecoverable, jsonPrimitiveSerializer, assetStorage);

            var buttonNotifier = new ButtonNotifier(buttonNotifierSettings);
            var keyNotifier = new KeyNotifier(keyNotifierSettings);
            var autoNextNotifier = new AutoNextNotifier();
            var compositeAnyNextNotifier = new CompositeAnyNextNotifier(new INextNotifier[]
            {
                autoNextNotifier,
                buttonNotifier,
                keyNotifier,
            });
            var compositeAnyCancellationNotifier = new CompositeAnyCancellationNotifier(new ICancellationNotifier[]
            {
                buttonNotifier,
                keyNotifier,
            });
            var notifierLockedDecorator = new NotifierLockedDecorator(compositeAnyNextNotifier, compositeAnyCancellationNotifier, scenarioLockHolder);

            var scenarioTaskExecutor = new ScenarioTaskExecutor(notifierLockedDecorator, notifierLockedDecorator);
            var scenarioTaskExecutorLockedDecorator = new ScenarioTaskExecutorLockedDecorator(scenarioTaskExecutor, scenarioTaskExecutor, scenarioLockHolder);
            var scenarioTaskExecutorCaptureDecorator = new ScenarioTaskExecutorCaptureDecorator(scenarioTaskExecutorLockedDecorator, scenarioCaptureExecutor, scenarioTaskExecutor);
            var scenarioBookReader = new ScenarioBookReader(scenarioTaskExecutorCaptureDecorator);

            var scenarioBookPublisher = new ScenarioBookPublisher(new IReflectable[]
            {
                backgroundAnimator,
                new BranchMaker(scenarioBookReader),
                new CancellationTokenDecoder(scenarioTaskExecutor),
                characterAnimator,
                characterProvider,
                new DelayMaker(),
                dialogueWriter,
                new PrimitiveDecoder(),
                new PlayerSelectionPresenter(scenarioBookReader, logStorage, playerSelectionPresenterSettings),
                sceneReloader,
                sceneTransitionAnimator,
                spriteProvider,
                new VectorDecoder(),
            });

            autoNextNotifier.Duration = 2.0f;
            autoNextNotifier.IsActive = false;
            scenarioTaskExecutor.Duration = 0.01f;

            var autoSwitch = new AutoSwitch(autoNextNotifier);
            var skipSwitch = new SkipSwitch(scenarioTaskExecutor);
            var autoSkipSwitchExclusiveDecorator = new AutoSkipSwitchExclusiveDecorator(autoSwitch, skipSwitch);
            var autoSkipSwitchLockedDecorator = new AutoSkipSwitchLockedDecorator(autoSkipSwitchExclusiveDecorator, autoSkipSwitchExclusiveDecorator, scenarioLockHolder);
            var scenarioLockerAutoSkipBreakDecorator = new ScenarioLockerAutoSkipBreakDecorator(scenarioLockHolder, autoSwitch, skipSwitch);

            var scenarioStarter = new ScenarioStarter(scenarioSourceMediator, sceneReloader, scenarioStarterSettings);
            var scenarioStatusUpdater = new ScenarioStatusUpdater(scenarioStatusHolder, scenarioCaptuererEventPublisherDecorator, scenarioCaptureExecutor);
            var continuableScenarioScriptPlayer = new ContinuableScenarioScriptPlayer(
                new ScenarioBookLoader(scenarioStarter, scenarioBookPublisher),
                scenarioBookReader,
                scenarioStatusUpdater);

            var messageWindow = new MessageWindow(messageWindowSettings);

            var recoveryExecutor = new RecoveryExecutor(scenarioSourceMediator, sceneReloader);

            var scenarioSaver = new ScenarioSaver(staticPrimitiveMemory, dialogueWriter, new ScenarioCamera(scenarioCameraSettings), logStorage);
            var scenarioLoader = new ScenarioLoader(staticPrimitiveMemory, recoveryExecutor, messageWindow, scenarioLoaderSettings);


            var saveLoadPanel = new SaveLoadPanel(scenarioSaver, scenarioLoader, scenarioLockHolder, messageWindow, saveLoadPanelSettings);
            var logObjectBuilder = new LogObjectBuilder(logStorage, logStorage, recoveryExecutor, logObjectBuilderSettings);
            var logPanel = new LogPanel(logObjectBuilder, scenarioLockHolder, messageWindow, logPanelSettings);

            var cancellationInitializables = new ICancellationInitializable[]
            {
                new FunctionAutoButtonSupervisor(autoSkipSwitchLockedDecorator, messageWindow, functionAutoButtonSupervisorSettings),
                new FunctionSkipButtonSupervisor(autoSkipSwitchLockedDecorator, messageWindow, functionSkipButtonSupervisorSettings),
                new FunctionSaveButtonSupervisor(saveLoadPanel, scenarioLockHolder, messageWindow, functionSaveButtonSupervisorSettings),
                new FunctionLoadButtonSupervisor(saveLoadPanel, scenarioLockHolder, messageWindow, functionLoadButtonSupervisorSettings),
                new FunctionLogButtonSupervisor(logPanel, scenarioLockerAutoSkipBreakDecorator, scenarioLockHolder, messageWindow, functionLogButtonSupervisorSettings),
                new FunctionViewButtonSupervisor(scenarioLockHolder, scenarioLockHolder, messageWindow, functionViewButtonSupervisorSettings),
                new FunctionLockButtonSupervisor(messageWindow, functionLockButtonSupervisorSettings),
                new SaveLoadPanelExitButtonSupervisor(messageWindow, saveLoadPanelExitButtonSupervisorSettings),
                new SaveLoadPanelHomeButtonSupervisor(sceneReloader, messageWindow, saveLoadPanelHomeButtonSupervisorSettings),
                scenarioStarter,
                new StorySelectionExitButtonSupervisor(messageWindow, storySelectionExitButtonSupervisorSettings),
                new StorySelectionLoadButtonSupervisor(saveLoadPanel, messageWindow, storySelectionLoadButtonSupervisorSettings),
            };

            var asyncInitializables = new IAsyncInitializable[]
            {
                saveLoadPanel,
            };

            var disposables = new IDisposable[]
            {
                scenarioTaskExecutor,
                messageWindow,
            };

            foreach (var disposable in disposables)
            {
                disposable.AddTo(cancellationToken);
            }

            await UniTask.WhenAll(asyncInitializables.Select(x => x.InitializeAsync(cancellationToken)));
            foreach (var cancellationInitializable in cancellationInitializables)
            {
                cancellationInitializable.InitializeWithCancellation(cancellationToken);
            }

            if (scenarioSourceMediator.TryAcceptNewScenario(out var scenarioScriptPath))
            {
                storySelectionPanel.SetActive(false);
                await continuableScenarioScriptPlayer.StartAsync(scenarioScriptPath, cancellationToken);
                sceneReloader.ReloadScene();
            }
            else if (scenarioSourceMediator.TryAcceptRecovery(out var recoveryLogRecord))
            {
                var scenarioRecord = logStorage.Recover(recoveryLogRecord);
                await scenarioRecoverer.RecoverAsync(scenarioRecord, cancellationToken);
                storySelectionPanel.SetActive(false);
                sceneReloader.StartSceneAsync(cancellationToken).Forget();
                await continuableScenarioScriptPlayer.ContinueAsync(scenarioStatusHolder.ScenarioScriptPath, scenarioStatusHolder.RestartIndex, cancellationToken);
                sceneReloader.ReloadScene();
            }
            else if (debugMode)
            {
                scenarioSourceMediator.RequestNewScenario(scenarioScript.name);
                sceneReloader.ReloadScene();
            }
            else
            {
                await sceneReloader.StartSceneAsync(cancellationToken);
            }
        }
    }
}