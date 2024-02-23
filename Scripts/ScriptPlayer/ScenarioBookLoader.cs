using Cysharp.Threading.Tasks;
using ScenarioFlow;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class ScenarioBookLoader : IScenarioBookLoader
    {
        private readonly IScenarioScriptLoader scenarioScriptLoader;
        private readonly IScenarioBookPublisher scenarioBookPublisher;

        public ScenarioBookLoader(IScenarioScriptLoader scenarioScriptLoader, IScenarioBookPublisher scenarioBookPublisher)
        {
            this.scenarioScriptLoader = scenarioScriptLoader ?? throw new ArgumentNullException(nameof(scenarioScriptLoader));
            this.scenarioBookPublisher = scenarioBookPublisher ?? throw new ArgumentNullException(nameof(scenarioBookPublisher));
        }

        public async UniTask<ScenarioBook> LoadScenarioBookAsync(string scenarioScriptPath, CancellationToken cancellationToken)
        {
            var scenarioScript = await scenarioScriptLoader.LoadAsync(scenarioScriptPath, cancellationToken);
            return scenarioBookPublisher.Publish(scenarioScript);
        }

        public UniTask UnloadScenarioBookAsync(string scenarioScriptPath, CancellationToken cancellationToken)
        {
            return scenarioScriptLoader.UnloadAsync(scenarioScriptPath, cancellationToken);
        }
    }
}
