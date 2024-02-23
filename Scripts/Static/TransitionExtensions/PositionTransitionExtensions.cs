using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public static class PositionTransitionExtensions
    {
        public async static UniTask TransLocalPosAsync(this Transform transform, Vector3 endLocalPos, float duration, Func<float, float> selector, CancellationToken cancellationToken)
        {
            if (duration == 0.0f)
            {
                transform.localPosition = endLocalPos;
                return;
            }
            var startLocalPos = transform.localPosition;
            var diff = endLocalPos - startLocalPos;
            await TransitionEnumerable.TransitionRange(0.0f, 1.0f, duration, selector, cancellationToken)
                .ForEachAsync(rate => transform.localPosition = startLocalPos + diff * rate, cancellationToken: cancellationToken);
        }
    }
}
