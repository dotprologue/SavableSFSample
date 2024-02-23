using System;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioCover
    {
        public Texture2D Texture { get; private set; }
        public string TitleText { get; private set; }
        public string SubText { get; private set; }

        public ScenarioCover(Texture2D texture, string titleText, string subText)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            TitleText = titleText ?? throw new ArgumentNullException(nameof(titleText));
            SubText = subText ?? throw new ArgumentNullException(nameof(subText));
        }
    }
}