using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public interface IScenarioCamera
    {
        UniTask<Texture2D> GetTextureAsync(CancellationToken cancellationToken);
    }
}
