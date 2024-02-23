using Cysharp.Threading.Tasks;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class ContinuableScenarioScriptPlayer : IScenarioScriptPlayer, IContinuedScenarioScriptPlayer
    {
        private readonly IScenarioBookLoader scenarioBookLoader;
        private readonly IScenarioBookReader scenarioBookReader;
        private readonly IScenarioStatusUpdater scenarioStatusUpdater;

        public ContinuableScenarioScriptPlayer(IScenarioBookLoader scenarioBookLoader, IScenarioBookReader scenarioBookReader, IScenarioStatusUpdater scenarioStatusUpdater)
        {
            this.scenarioBookLoader = scenarioBookLoader ?? throw new ArgumentNullException(nameof(scenarioBookLoader));
            this.scenarioBookReader = scenarioBookReader ?? throw new ArgumentNullException(nameof(ScenarioBookReader));
            this.scenarioStatusUpdater = scenarioStatusUpdater ?? throw new ArgumentNullException(nameof(scenarioStatusUpdater));
        }

        public async UniTask StartAsync(string scenarioScriptPath, CancellationToken cancellationToken)
        {
            var scenarioBook = await scenarioBookLoader.LoadScenarioBookAsync(scenarioScriptPath, cancellationToken);
            scenarioStatusUpdater.StartUpdate(scenarioScriptPath, scenarioBook);
            try
            {
                await scenarioBookReader.ReadAsync(scenarioBook, cancellationToken);
            }
            finally
            {
                scenarioStatusUpdater.StopUpdate();
            }
        }

        public async UniTask ContinueAsync(string scenarioScriptPath, int restartIndex, CancellationToken cancellationToken)
        {
            var scenarioBook = await scenarioBookLoader.LoadScenarioBookAsync(scenarioScriptPath, cancellationToken);
            if (restartIndex < 0 || scenarioBook.Length <= restartIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(restartIndex));
            }
            scenarioBook.OpenTo(restartIndex);
            scenarioStatusUpdater.ContinueUpdate(scenarioScriptPath, scenarioBook);
            try
            {
                await scenarioBookReader.ReadAsync(scenarioBook, cancellationToken);
            }
            finally
            {
                scenarioStatusUpdater.StopUpdate();
            }
        }
    }
}