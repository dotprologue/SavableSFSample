using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioSaver : IScenarioSaver
    {
        private readonly IPrimitiveSaver primitiveSaver;
        private readonly IDialogueTextGetter dialogueTextGetter;
        private readonly IScenarioCamera scenarioCamera;
        private readonly ILogCapturer logCapturer;

        public ScenarioSaver(IPrimitiveSaver primitiveSaver, IDialogueTextGetter dialogueTextGetter, IScenarioCamera scenarioCamera, ILogCapturer logCapturer)
        {
            this.primitiveSaver = primitiveSaver ?? throw new ArgumentNullException(nameof(primitiveSaver));
            this.dialogueTextGetter = dialogueTextGetter ?? throw new ArgumentNullException(nameof(dialogueTextGetter));
            this.scenarioCamera = scenarioCamera ?? throw new ArgumentNullException(nameof(scenarioCamera));
            this.logCapturer = logCapturer ?? throw new ArgumentNullException(nameof(logCapturer));
        }


        public async UniTask<ScenarioCover> SaveScenarioAsync(int scenarioNumber, CancellationToken cancellationToken)
        {
            var characterName = dialogueTextGetter.CharacterName;
            var dialogueLine = dialogueTextGetter.DialogueLine;
            var logRecord = logCapturer.Capture(logCapturer.ScenarioCount - 1);
            var texture = await scenarioCamera.GetTextureAsync(cancellationToken);
            await primitiveSaver.SaveAsync(DataPath.ScenarioDataPath(scenarioNumber), new ScenarioPack(logRecord, characterName, dialogueLine, texture.EncodeToPNG()), cancellationToken);
            return new ScenarioCover(texture, characterName, dialogueLine);
        }
    }
}
