using ScenarioFlow;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class CancellationTokenDecoder : IReflectable
    {
        private readonly ICancellationTokenDecoder cancellationTokenDecoder;

        public CancellationTokenDecoder(ICancellationTokenDecoder cancellationTokenDecoder)
        {
            this.cancellationTokenDecoder = cancellationTokenDecoder ?? throw new ArgumentNullException(nameof(cancellationTokenDecoder));
        }

        [DecoderMethod]
        public CancellationToken Decode(string tokenCode)
        {
            return cancellationTokenDecoder.Decode(tokenCode);
        }
    }
}
