using System;

namespace SavableSFSample
{
    //Return [0, 1] for [0, 1]
    public static class TransSelector
    {
        public static readonly Func<float, float> Linear = x => x;
        public static readonly Func<float, float> UpQuad = x => MathF.Pow(x, 2);
        public static readonly Func<float, float> DownQuad = x => -MathF.Pow(x - 1, 2) + 1;
        public static readonly Func<float, float> UpDownQuad = x => x < 0.5f ? MathF.Pow(x, 2) + 0.5f * x : -MathF.Pow(x, 2) + 2.5f * x - 0.5f;
    }
}
