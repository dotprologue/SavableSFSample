using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface ISceneStarter
    {
        UniTask StartSceneAsync(CancellationToken cancellationToken);
    }
}
