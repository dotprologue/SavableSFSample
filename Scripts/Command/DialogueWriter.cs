using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace SavableSFSample
{
    public class DialogueWriter : IDialogueTextGetter, IReflectable, IRecoverable
    {
		private readonly ICharacterImageAnimator characterImageAnimator;
		private readonly ILogStacker logStacker;
        private readonly TextMeshProUGUI characterNameText;
        private readonly TextMeshProUGUI dialogueLineText;
        private readonly GameObject dialogueBox;
        private readonly float characterInterval;
        private readonly float characterFadeDuration;

        public string CharacterName => characterNameText.text;
        public string DialogueLine => dialogueLineText.text;

        public DialogueWriter(ICharacterImageAnimator characterImageAnimator, ILogStacker logStacker, Settings settings)
        {
            this.characterImageAnimator = characterImageAnimator ?? throw new ArgumentNullException(nameof(characterImageAnimator));
            this.logStacker = logStacker ?? throw new ArgumentNullException(nameof(logStacker));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.CharacterInterval < 0)
                throw new ArgumentOutOfRangeException(nameof(settings.CharacterInterval));
            if (settings.CharacterFadeDuration < 0)
                throw new ArgumentOutOfRangeException(nameof(settings.CharacterFadeDuration));

            characterNameText = settings.CharacterNameText ?? throw new ArgumentNullException(nameof(settings.CharacterNameText));
            dialogueLineText = settings.DialogueLineText ?? throw new ArgumentNullException(nameof(settings.DialogueLineText));
            dialogueBox = settings.DialogueBox ?? throw new ArgumentNullException(nameof(settings.DialogueBox));
            dialogueBox.SetActive(false);
            characterInterval = settings.CharacterInterval;
            characterFadeDuration = settings.CharacterFadeDuration;
            characterNameText.text = string.Empty;
            dialogueLineText.text = string.Empty;
        }

        [CommandMethod("write dialogue async")]
        [Category("Dialogue")]
        [Description("Write dialogue line on the dialogue box.")]
        [Snippet("{${1:Character}}")]
        [Snippet("{${2:Line}}")]
		public async UniTask WriteDialogueAsync(string characterName, string dialogueLine, CancellationToken cancellationToken)
		{
            logStacker.StackLog(characterName, dialogueLine, true);
			dialogueBox.SetActive(true);
			characterNameText.text = characterName;
			dialogueLineText.alpha = 0.0f;
			dialogueLineText.text = dialogueLine;

			var textInfo = dialogueLineText.textInfo;
			dialogueLineText.ForceMeshUpdate();
			try
			{
				var vertexIndexList = new List<int>();
				await UniTask.WhenAll(
					Enumerable.Range(0, textInfo.characterCount)
					.Select(async characterIndex =>
					{
						var characterInfo = dialogueLineText.textInfo.characterInfo[characterIndex];
						var characterColor = new Color(characterInfo.color.r, characterInfo.color.g, characterInfo.color.b, 0);
						var materialIndex = characterInfo.materialReferenceIndex;
						var vertexIndex = characterInfo.vertexIndex;
						var colors32 = textInfo.meshInfo[materialIndex].colors32;
						await UniTask.Delay(TimeSpan.FromSeconds(characterInterval * characterIndex));
						if (vertexIndexList.Contains(vertexIndex))
						{
							return;
						}
						else
						{
							vertexIndexList.Add(vertexIndex);
						}
						await TransitionEnumerable.TransitionRange(0.0f, 1.0f, characterFadeDuration, TransSelector.Linear, cancellationToken)
						.ForEachAsync(alpha =>
						{
							var newAlpha = (byte)Mathf.Floor(alpha * 255);
							foreach (var index in Enumerable.Range(vertexIndex, 4))
							{
								colors32[index].a = newAlpha;
							}
							dialogueLineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
						}, cancellationToken: cancellationToken);
					}));
			}
			finally
			{
				dialogueLineText.alpha = 1.0f;
			}
		}

		[CommandMethod("write dialogue with image async")]
		[Category("Dialogue")]
		[Description("Write a dialogue line on the dialogue box, and change a character's image.")]
		[Description("Specify the target character with the character name.")]
		[Snippet("{${1:Character}}")]
		[Snippet("{${2:Line}}")]
		[Snippet("Change the image to {${3:Image}}.")]
		[DialogueSnippet("Change the image to {${1:Image}}.")]
		public async UniTask WriteDialogueWithImageAsync(CharacterObject characterObject, string dialougeLine, Sprite sprite, CancellationToken cancellationToken)
		{
			try
			{
				await characterImageAnimator.ChangeCharacterImageAsync(characterObject, sprite, cancellationToken);
			}
			finally
			{
				await WriteDialogueAsync(characterObject.name, dialougeLine, cancellationToken);
			}
		}


		[CommandMethod("hide dialogue box")]
        [Category("Dialogue")]
        [Description("Hide the dialogue texts and the background image.")]
        [Description("The dialogue texts are erased.")]
        [Snippet("Hide the dialogue box.")]
        public void HideDialogueBox()
        {
			characterNameText.text = string.Empty;
			dialogueLineText.text = string.Empty;
			dialogueBox.SetActive(false);
        }

        public string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var record = new Record
            {
                CharacterName = characterNameText.text,
                DialogueLine = dialogueLineText.text,
                IsDialogueBoxActive = dialogueBox.activeSelf,
            };
            return primitiveSerializer.Serialize(record);
        }

        public void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            var record = primitiveDeserializer.Deserialize<Record>(input);
            characterNameText.text = record.CharacterName;
            dialogueLineText.text = record.DialogueLine;
            dialogueBox.SetActive(record.IsDialogueBoxActive);
        }

        [Serializable]
        public class Settings
        {
            public TextMeshProUGUI CharacterNameText;
            public TextMeshProUGUI DialogueLineText;
            public GameObject DialogueBox;
            public float CharacterInterval = 0.05f;
            public float CharacterFadeDuration = 0.1f;
        }

        [Serializable]
        public class Record
        {
            public string CharacterName;
            public string DialogueLine;
            public bool IsDialogueBoxActive;
        }
    }
}
