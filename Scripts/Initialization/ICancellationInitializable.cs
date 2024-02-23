using System.Threading;

namespace SavableSFSample
{
    public interface ICancellationInitializable
    {
        void InitializeWithCancellation(CancellationToken cancellationToken);
    }
}
