using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class LogObject : MonoBehaviour
    {
        [SerializeField]
        private Button jumpButton;
        [SerializeField]
        private TextMeshProUGUI characterNameText;
        [SerializeField]
        private TextMeshProUGUI dialogueLineText;

        public Button JumpButton => jumpButton;
        public TextMeshProUGUI CharacterNameText => characterNameText;
        public TextMeshProUGUI DialogueLineText => dialogueLineText;
    }
}
