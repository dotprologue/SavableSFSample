using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public static class SpriteRendererTransitionExtensions
    {
        public static UniTask TransAlphaAsync(this SpriteRenderer spriteRenderer, float endAlpha, float duration, Func<float, float> selector, CancellationToken cancellationToken)
        {
            return TransitionEnumerable.TransitionRange(spriteRenderer.color.a, endAlpha, duration, selector, cancellationToken)
                .ForEachAsync(alpha =>
                {
                    spriteRenderer.TransAlpha(alpha);
                }, cancellationToken: cancellationToken);
        }

        public static void TransAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
    }
}
