using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class ScenarioDataWindow : MonoBehaviour
    {
        [SerializeField]
        private Button windowButton;
        [SerializeField]
        private Image windowImage;
        [SerializeField]
        private TextMeshProUGUI characterNameText;
        [SerializeField]
        private TextMeshProUGUI dialogueLineText;

        public void SetCover(ScenarioCover scenarioCover)
        {
            characterNameText.text = scenarioCover.TitleText;
            dialogueLineText.text = scenarioCover.SubText;
            var texture = scenarioCover.Texture;
            windowImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            windowImage.TransAlpha(1.0f);
        }

        public void ClearWindow()
        {
            characterNameText.text = string.Empty;
            dialogueLineText.text = string.Empty;
            windowImage.TransAlpha(0.0f);
            windowImage.sprite = null;
        }

        public IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(CancellationToken cancellationToken)
        {
            return windowButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken);
        }
    }
}