using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IPrimitiveLoader
    {
        UniTask<T> LoadAsync<T>(string path, CancellationToken cancellationToken) where T : class;

        bool Exist(string path);
    }
}
