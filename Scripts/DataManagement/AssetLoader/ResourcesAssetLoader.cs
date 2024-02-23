using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class ResourcesAssetLoader : IAssetLoader
    {
        public async UniTask<T> LoadAssetAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            var result = await Resources.LoadAsync<T>(assetPath).ToUniTask(cancellationToken: cancellationToken);
            if (result == null)
            {
                throw new ArgumentException($"No asset exists at '{assetPath}'.");
            }
            return (T)result;
        }

        public async UniTask<IEnumerable<T>> LoadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            var result = await UniTask.FromResult(Resources.LoadAll<T>(assetPath));
            if (result.Length == 0)
            {
                throw new ArgumentException($"No asset exists at '{assetPath}'.");
            }
            return result;
        }

        public async UniTask UnloadAssetAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            await Resources.UnloadUnusedAssets().ToUniTask(cancellationToken: cancellationToken);
        }

        public async UniTask UnloadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            await Resources.UnloadUnusedAssets().ToUniTask(cancellationToken: cancellationToken);
        }
    }
}