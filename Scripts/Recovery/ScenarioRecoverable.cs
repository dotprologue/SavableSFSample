using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SavableSFSample
{
    public class ScenarioRecoverable : IScenarioRecoverable
    {
        private readonly Dictionary<string, IRecoverable> recoverableDictionary;
        private readonly Dictionary<string, IAssetRecoverable> assetRecoverableDictionary;

        public ScenarioRecoverable(IEnumerable<IRecoverable> recoverables, IEnumerable<IAssetRecoverable> assetRecoverables)
        {
            if (recoverables == null)
                throw new ArgumentNullException(nameof(recoverables));
            if (assetRecoverables == null)
                throw new ArgumentNullException(nameof(assetRecoverables));

            var Ids = recoverables.Select(r => r.GetType().FullName).Concat(assetRecoverables.Select(r => r.GetType().FullName));
            if (Ids.Count() != Ids.Distinct().Count())
                throw new ArgumentException("Type duplication was detected. Each type must be unique.");
            recoverableDictionary = recoverables.ToDictionary(r => r.GetType().FullName, r => r);
            assetRecoverableDictionary = assetRecoverables.ToDictionary(r => r.GetType().FullName, r => r);
        }

        public ScenarioRecord Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var assetRecoverableRecords = assetRecoverableDictionary.Select(pair => new ScenarioRecord.Fragment(pair.Key, pair.Value.Capture(primitiveSerializer)));
            var recoverableRecords = recoverableDictionary.Select(pair => new ScenarioRecord.Fragment(pair.Key, pair.Value.Capture(primitiveSerializer, assetSerializer)));
            return new ScenarioRecord(Enumerable.Concat(recoverableRecords, assetRecoverableRecords));
        }

        public async UniTask RecoverAsync(ScenarioRecord scenarioRecord, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer, CancellationToken cancellationToken)
        {
            //Recover asset recoverables at first
            await UniTask.WhenAll(
                scenarioRecord.Fragments
                .Where(fragment => assetRecoverableDictionary
                .ContainsKey(fragment.Id))
                .Select(fragment => assetRecoverableDictionary[fragment.Id].RecoverAsync(fragment.Value, primitiveDeserializer, cancellationToken)));
            //Recover recoverables
            foreach (var fragment in scenarioRecord.Fragments.Where(fragment => recoverableDictionary.ContainsKey(fragment.Id)))
            {
                recoverableDictionary[fragment.Id].Recover(fragment.Value, primitiveDeserializer, assetDeserializer);
            }
        }
    }
}
