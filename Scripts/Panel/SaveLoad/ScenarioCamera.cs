using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioCamera : IScenarioCamera
    {
        private readonly Camera camera;

        public ScenarioCamera(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            camera = settings.Camera ?? throw new ArgumentNullException(nameof(settings.Camera));
        }

        public UniTask<Texture2D> GetTextureAsync(CancellationToken cancellationToken)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                return UniTask.FromResult(texture);
            }
            finally
            {
                camera.targetTexture = null;
                RenderTexture.active = null;
            }
        }

        [Serializable]
        public class Settings
        {
            public Camera Camera;
        }
    }
}
