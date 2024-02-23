using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Threading;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class SceneTransitionAnimator : IReflectable, IRecoverable
    {
        private readonly Image curtainImage;

        public SceneTransitionAnimator(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            curtainImage = settings.CurtainImage ?? throw new ArgumentNullException(nameof(settings.CurtainImage));
        }

        [CommandMethod("enter scene transition async")]
        [Category("Scene Transition")]
        [Description("Close the curtain to prepare the next scene in the specified seconds.")]
        [Snippet("Close the curtain in {${1:n}} seconds.")]
        public async UniTask EnterSceneTransitionAsync(float duration, CancellationToken cancellationToken)
        {
            try
            {
                await curtainImage.TransAlphaAsync(1.0f, duration, TransSelector.Linear, cancellationToken);
            }
            finally
            {
                curtainImage.TransAlpha(1.0f);
            }
        }

        [CommandMethod("exit scene transition async")]
        [Category("SceneTransition")]
        [Description("Open the curtain to show the next scene in the specified seconds.")]
        [Snippet("Open the curtain in {${2:n}} seconds.")]
        public async UniTask ExitSceneTransitionAsync(float duration, CancellationToken cancellationToken)
        {
            try
            {
                await curtainImage.TransAlphaAsync(0.0f, duration, TransSelector.Linear, cancellationToken);
            }
            finally
            {
                curtainImage.TransAlpha(0.0f);
            }
        }

        public string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var record = new Record
            {
                ImageAlpha = curtainImage.color.a,
            };
            return primitiveSerializer.Serialize(record);
        }

        public void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            var record = primitiveDeserializer.Deserialize<Record>(input);
            curtainImage.TransAlpha(record.ImageAlpha);
        }

        [Serializable]
        public class Record
        {
            public float ImageAlpha;
        }

        [Serializable]
        public class Settings
        {
            public Image CurtainImage;
        }
    }
}
