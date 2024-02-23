using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    [RequireComponent(typeof(Button))]
    public class BranchSelectionButton : MonoBehaviour
    {
        private Button button;
        private Image image;
        private TextMeshProUGUI textMeshProUGUI;

        public TextMeshProUGUI Text => textMeshProUGUI;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

        }

        public void SetInteractable(bool isActive)
        {
            button.interactable = isActive;
        }

        public void SetAlpha(float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            textMeshProUGUI.alpha = alpha;
        }

        public IUniTaskAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(CancellationToken cancellationToken)
        {
            return button.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken);
        }
    }
}
