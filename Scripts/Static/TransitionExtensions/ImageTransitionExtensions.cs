using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public static class ImageTransitionExtensions
    {
        public static async UniTask TransAlphaAsync(this Image image, float endAlpha, float duration, Func<float, float> selector, CancellationToken cancellationToken)
        {
            await TransitionEnumerable.TransitionRange(image.color.a, endAlpha, duration, selector, cancellationToken)
                .ForEachAsync(alpha =>
                {
                    image.TransAlpha(alpha);
                }, cancellationToken: cancellationToken);
        }

        public static void TransAlpha(this Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}