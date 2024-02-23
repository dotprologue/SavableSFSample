using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IPrimitiveSaver
    {
        UniTask SaveAsync<T>(string path, T value, CancellationToken cancellationToken) where T : class;

        bool Exist(string path);
    }
}