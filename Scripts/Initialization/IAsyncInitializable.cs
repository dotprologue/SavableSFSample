using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IAsyncInitializable
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);
    }
}
