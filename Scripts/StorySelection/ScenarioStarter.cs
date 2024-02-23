using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioStarter : IScenarioScriptLoader, ICancellationInitializable
    {
        private readonly INewScenarioRequester newScenarioRequester;
        private readonly ISceneReloader sceneReloader;
        private readonly Dictionary<string, ScenarioSelectionButton> scenarioSelectionButtonDictionary;

        public ScenarioStarter(INewScenarioRequester newScenarioRequester, ISceneReloader sceneReloader, Settings settings)
        {
            this.newScenarioRequester = newScenarioRequester ?? throw new ArgumentNullException(nameof(newScenarioRequester));
            this.sceneReloader = sceneReloader ?? throw new ArgumentNullException(nameof(sceneReloader));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            var scenarioSelectionButtons = settings.ScenarioSelectionButtonParent.GetComponentsInChildren<ScenarioSelectionButton>();
            scenarioSelectionButtonDictionary = scenarioSelectionButtons
                .ToDictionary(scenarioButton => scenarioButton.ScenarioScript.name, scenarioButton => scenarioButton);
            var firstSelectionButton = scenarioSelectionButtons.First();
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            UniTask.Void(async token =>
			{
				var (_, path) = await UniTask.WhenAny(
				scenarioSelectionButtonDictionary.Values
				.Select(selectionButton => UniTask.Create(async () =>
				{
					await selectionButton.Button.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken).FirstOrDefaultAsync(cancellationToken: cancellationToken);
					return selectionButton.ScenarioScript.name;
				})));
                if (!token.IsCancellationRequested)
                {
                    newScenarioRequester.RequestNewScenario(path);
                    sceneReloader.ReloadScene();
                }
			}, cancellationToken: cancellationToken);
        }

        public async UniTask<ScenarioScript> LoadAsync(string scenarioScriptPath, CancellationToken cancellationToken)
        {
            if (scenarioSelectionButtonDictionary.ContainsKey(scenarioScriptPath))
            {
                return await UniTask.FromResult(scenarioSelectionButtonDictionary[scenarioScriptPath].ScenarioScript);
            }
            else
            {
                throw new ArgumentException($"Scenario script '{scenarioScriptPath}' does not exist.");
            }
        }

        public UniTask UnloadAsync(string scenarioScriptPath, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        [Serializable]
        public class Settings
        {
            public GameObject ScenarioSelectionButtonParent;
        }
    }
}
