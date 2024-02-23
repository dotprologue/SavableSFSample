using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class SceneReloader : ISceneStarter, ISceneReloader, IReflectable
    {
        private Image loadingScreenImage;
        private float sceneFadeDuration;

        public SceneReloader(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.LoadingScreenImage == null)
                throw new ArgumentNullException(nameof(settings.LoadingScreenImage));
            if (sceneFadeDuration < 0)
                throw new ArgumentOutOfRangeException(nameof(sceneFadeDuration));
            loadingScreenImage = settings.LoadingScreenImage;
            sceneFadeDuration = settings.SceneFadeDuration;
        }

        [CommandMethod("start scene async")]
        [Category("Scene")]
        [Description("Exit the loading screen.")]
        [Snippet("Exit the loading screen.")]
        public async UniTask StartSceneAsync(CancellationToken cancellationToken)
        {
            try
            {
                await loadingScreenImage.TransAlphaAsync(0.0f, sceneFadeDuration, TransSelector.UpDownQuad, cancellationToken);
            }
            finally
            {
                loadingScreenImage.TransAlpha(0.0f);
                loadingScreenImage.gameObject.SetActive(false);
            }
        }

        public void ReloadScene()
        {
            UniTask.Void(async () =>
            {
                loadingScreenImage.gameObject.SetActive(true);
                await loadingScreenImage.TransAlphaAsync(1.0f, sceneFadeDuration, TransSelector.UpDownQuad, loadingScreenImage.GetCancellationTokenOnDestroy());
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        [Serializable]
        public class Settings
        {
            public Image LoadingScreenImage;
            public float SceneFadeDuration;
        }
    }
}
