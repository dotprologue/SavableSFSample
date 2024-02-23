using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace SavableSFSample
{
    public interface IAssetStorage
    {
        UniTask LoadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object;
        UniTask UnloadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object;
        IEnumerable<string> GetPathAll<T>() where T : UnityEngine.Object;
    }
}