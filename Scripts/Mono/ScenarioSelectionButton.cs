using ScenarioFlow.Scripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SavableSFSample
{
    [RequireComponent(typeof(Button))]
    public class ScenarioSelectionButton : MonoBehaviour
    {
        [SerializeField]
        private ScenarioScript scenarioScript;
        [SerializeField]
        private string title;

        public Button Button { get; private set; }
        public ScenarioScript ScenarioScript => scenarioScript;
        public string Title => title;

        private void Awake()
        {
            Button = GetComponent<Button>();
            GetComponentInChildren<TextMeshProUGUI>().text = title;
        }
    }
}