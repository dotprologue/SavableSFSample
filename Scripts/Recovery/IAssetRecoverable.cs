using Cysharp.Threading.Tasks;
using System.Threading;

namespace SavableSFSample
{
    public interface IAssetRecoverable
    {
        string Capture(IPrimitiveSerializer primitiveSerializer);
        UniTask RecoverAsync(string input, IPrimitiveDeserializer primitiveDeserializer, CancellationToken cancellationToken);
    }
}
