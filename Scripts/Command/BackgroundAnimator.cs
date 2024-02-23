using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class BackgroundAnimator : IReflectable, IRecoverable
    {
        private readonly SpriteRenderer backgroundRenderer;

        public BackgroundAnimator(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.BackgroundRenderer == null)
                throw new ArgumentNullException(nameof(settings.BackgroundRenderer));

            backgroundRenderer = settings.BackgroundRenderer ?? throw new ArgumentNullException(nameof(settings.BackgroundRenderer));
        }

        [CommandMethod("change background")]
        [Category("Background")]
        [Description("Change the background image.")]
        [Snippet("Change the background image to {${1:name}}.")]
        public void ChangeBackground(Sprite sprite)
        {
            backgroundRenderer.sprite = sprite;
        }

		[CommandMethod("change background async")]
		[Category("Background")]
		[Description("Change the background image.")]
		[Snippet("Change the background image to {${1:name}}.")]
		public async UniTask ChangeBackgroundAsync(Sprite sprite, CancellationToken cancellationToken)
        {
            var frontRenderer = GameObject.Instantiate<SpriteRenderer>(backgroundRenderer, backgroundRenderer.transform);
            frontRenderer.transform.position = backgroundRenderer.transform.position - Vector3.forward;
            backgroundRenderer.sprite = sprite;
            try
            {
                await frontRenderer.TransAlphaAsync(0.0f, 3.0f, TransSelector.Linear, cancellationToken);
            }
            finally
            {
                GameObject.Destroy(frontRenderer.gameObject);
            }
        }

        public string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var record = new Record
            {
                SpriteCode = backgroundRenderer.sprite == null ? string.Empty : assetSerializer.Serialize(backgroundRenderer.sprite)
            };
            return primitiveSerializer.Serialize(record);
        }

        public void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            var data = primitiveDeserializer.Deserialize<Record>(input);
            backgroundRenderer.sprite = data.SpriteCode == string.Empty ? null : assetDeserializer.Deserialize<Sprite>(data.SpriteCode);
        }

        [Serializable]
        public class Record
        {
            public string SpriteCode;
        }

        [Serializable]
        public class Settings
        {
            public SpriteRenderer BackgroundRenderer;
        }
    }
}