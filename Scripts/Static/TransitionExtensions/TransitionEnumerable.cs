using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public static class TransitionEnumerable
    {
        public static IUniTaskAsyncEnumerable<float> TransitionRange(float start, float end, float duration, Func<float, float> selector, CancellationToken cancellationToken)
        {
            if (selector(0.0f) != 0.0f || selector(1.0f) != 1.0f)
            {
                throw new ArgumentException("The selector must return 0 for the input 0 and return 1 for the input 1.");
            }
            if (duration < 0.0f)
            {
                throw new ArgumentOutOfRangeException("Duration must not be negative number.");
            }
            if (duration == 0.0f)
            {
                return UniTask.FromResult(end).ToUniTaskAsyncEnumerable();
            }
            var velocity = 1 / duration;
            var range = end - start;
            var doesFinish = false;
            //[0, 1]
            var progress = 0.0f;
            return UniTaskAsyncEnumerable.EveryUpdate()
                .TakeUntilCanceled(cancellationToken)
                .TakeWhile(_ => !doesFinish)
                .Select(_ =>
                {
                    var result = start + selector(progress) * range;
                    progress += velocity * Time.deltaTime;
                    if (1.0f <= progress)
                    {
                        doesFinish = true;
                        return end;
                    }
                    else
                    {
                        return result;
                    }
                });
        }
    }
}
