using System;
using UnityEngine;

namespace SavableSFSample
{
    [Serializable]
    public class LogText
    {
        [SerializeField]
        private int scenarioIndex;
        [SerializeField]
        private bool doesAttachScenario;
        [SerializeField]
        private string characterName;
        [SerializeField]
        private string dialogueLine;

        public int ScenarioIndex => scenarioIndex;
        public bool DoesAttachScenario => doesAttachScenario;
        public string CharacterName => characterName;
        public string DialogueLine => dialogueLine;

        public LogText(int scenarioIndex, bool doesAttachScenario, string characterName, string dialogueLine)
        {
            this.scenarioIndex = scenarioIndex;
            this.doesAttachScenario = doesAttachScenario;
            this.characterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
            this.dialogueLine = dialogueLine ?? throw new ArgumentNullException(nameof(dialogueLine));
        }
    }
}
