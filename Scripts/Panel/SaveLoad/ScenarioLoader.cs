using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioLoader : IScenarioLoader
    {
        private readonly IPrimitiveLoader primitiveLoader;
        private readonly IRecoveryExecutor recoveryExecutor;
        private readonly IMessageWindow messageWindow;
        private readonly int maxScenarioPackCount;

        public ScenarioLoader(IPrimitiveLoader primitiveLoader, IRecoveryExecutor recoveryExecutor, IMessageWindow messageWindow, Settings settings)
        {
            this.primitiveLoader = primitiveLoader ?? throw new ArgumentNullException(nameof(primitiveLoader));
            this.recoveryExecutor = recoveryExecutor ?? throw new ArgumentNullException(nameof(recoveryExecutor));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.MaxScenarioPackCount < 0)
                throw new ArgumentOutOfRangeException(nameof(settings.MaxScenarioPackCount));
            maxScenarioPackCount = settings.MaxScenarioPackCount;
        }

        public async UniTask LoadScenarioAsync(int scenarioNumber, CancellationToken cancellationToken)
        {
            if (!primitiveLoader.Exist(DataPath.ScenarioDataPath(scenarioNumber)))
            {
                messageWindow.ShowVolatileMessage($"The data {scenarioNumber + 1} does not exist");
                return;
            }
            try
            {
                var scenarioPack = await primitiveLoader.LoadAsync<ScenarioPack>(DataPath.ScenarioDataPath(scenarioNumber), cancellationToken);
                recoveryExecutor.ExecuteRecovery(scenarioPack.LogRecord);
            }
            catch (Exception)
            {
                messageWindow?.ShowVolatileMessage($"Failed to load the data {scenarioNumber + 1}");
            }
        }

        public async UniTask<IReadOnlyDictionary<int, ScenarioCover>> GetScenarioCoversAsync(CancellationToken cancellationToken)
        {
            return await UniTaskAsyncEnumerable.Range(0, maxScenarioPackCount)
                .Where(index => primitiveLoader.Exist(DataPath.ScenarioDataPath(index)))
                .ToDictionaryAwaitWithCancellationAsync((index, token) => UniTask.FromResult(index), async (index, token) =>
                {
                    var scenarioPack = await primitiveLoader.LoadAsync<ScenarioPack>(DataPath.ScenarioDataPath(index), token);
                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(scenarioPack.TextureBytes.ToArray());
                    return new ScenarioCover(texture, scenarioPack.CharacterName, scenarioPack.DialogueLine);
                }, cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public int MaxScenarioPackCount;
        }
    }
}
