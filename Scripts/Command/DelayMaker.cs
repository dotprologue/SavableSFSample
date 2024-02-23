using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Threading;

namespace SavableSFSample
{
    public class DelayMaker : IReflectable
    {
        [CommandMethod("delay seconds async")]
        [Category("Delay")]
        [Description("Wait for the specified seconds.")]
        [Snippet("Wait for {${1:n}} seconds.")]
        public async UniTask DelaySecondsAsync(float seconds, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: cancellationToken);
        }
    }
}
