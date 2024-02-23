using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class LogObjectBuilder : ILogObjectBuilder
    {
        private static readonly int DisplayCount = 5;

        private readonly ILogTextGetter logTextGetter;
        private readonly ILogCapturer logCapturer;
        private readonly IRecoveryExecutor recoveryExecutor;
        private readonly GameObject logObjectParent;
        private readonly LogObject logObjectPrefab;
        private readonly Slider logSlider;
        private readonly Button upButton;
        private readonly Button downButton;

        public LogObjectBuilder(ILogTextGetter logTextGetter, ILogCapturer logCapturer, IRecoveryExecutor recoveryExecutor, Settings settings)
        {
            this.logTextGetter = logTextGetter ?? throw new ArgumentNullException(nameof(logTextGetter));
            this.logCapturer = logCapturer ?? throw new ArgumentNullException(nameof(logCapturer));
            this.recoveryExecutor = recoveryExecutor ?? throw new ArgumentNullException(nameof(recoveryExecutor));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            logObjectParent = settings.LogObjectParent ?? throw new ArgumentNullException(nameof(settings.LogObjectParent));
            logObjectPrefab = settings.LogObjectPrefab ?? throw new ArgumentNullException(nameof(settings.LogObjectPrefab));
            logSlider = settings.LogSlider ?? throw new ArgumentNullException(nameof(settings.LogSlider));
            upButton = settings.UpButton ?? throw new ArgumentNullException(nameof(settings.UpButton));
            downButton = settings.DownButton ?? throw new ArgumentNullException(nameof(settings.DownButton));
        }

        public IEnumerable<LogObject> BuildLogObjects()
        {
            var logObjects = Enumerable.Range(0, DisplayCount).Select(_ => GameObject.Instantiate(logObjectPrefab, logObjectParent.transform)).ToArray();

            var cancellationToken = logObjects[0].GetCancellationTokenOnDestroy();
            var logTexts = logTextGetter.LogTexts.ToArray();
            var maxOffset = logTexts.Length > DisplayCount ? logTexts.Length - DisplayCount : 0;
            logSlider.minValue = 0;
            logSlider.maxValue = maxOffset;
            logSlider.value = 0;
            Update();
            //Observe slider
            logSlider.OnValueChangedAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ => Update(), cancellationToken: cancellationToken)
                .Forget();
            //Up button
            upButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    logSlider.value = Mathf.FloorToInt(logSlider.value) + 1;
                    Update();
                }, cancellationToken: cancellationToken).Forget();
            //Down button
            downButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ =>
                {
                    logSlider.value = Mathf.FloorToInt(logSlider.value) - 1;
                    Update();
                }, cancellationToken: cancellationToken).Forget();
            //Set up all jump buttons
            foreach (var (button, index) in logObjects.Select((x, i) => (x.JumpButton, i)))
            {
                button.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                    .ForEachAsync(_ =>
                    {
                        var pickedLogTexts = logTexts.Length > DisplayCount ? logTexts.Skip(logTexts.Length - DisplayCount - Mathf.FloorToInt(logSlider.value)).Take(DisplayCount).ToArray() : logTexts;
                        var logRecord = logCapturer.Capture(pickedLogTexts[index].ScenarioIndex);
                        recoveryExecutor.ExecuteRecovery(logRecord);
                    }, cancellationToken: cancellationToken).Forget();
            }

            return logObjects;

            void Update()
            {
                var pickedLogTexts = logTexts.Length > DisplayCount ? logTexts.Skip(logTexts.Length - DisplayCount - Mathf.FloorToInt(logSlider.value)).Take(DisplayCount).ToArray() : logTexts;
                foreach (var index in Enumerable.Range(0, DisplayCount))
                {
                    var logObject = logObjects[index];
                    if (index < pickedLogTexts.Length)
                    {
                        var logText = pickedLogTexts[index];
                        logObject.JumpButton.gameObject.SetActive(logText.DoesAttachScenario);
                        logObject.CharacterNameText.text = logText.CharacterName;
                        logObject.DialogueLineText.text = logText.DialogueLine;
                    }
                    else
                    {
                        logObject.JumpButton.gameObject.SetActive(false);
                        logObject.CharacterNameText.text = string.Empty;
                        logObject.DialogueLineText.text = string.Empty;
                    }
                }
            }
        }

        [Serializable]
        public class Settings
        {
            public GameObject LogObjectParent;
            public LogObject LogObjectPrefab;
            public Slider LogSlider;
            public Button UpButton;
            public Button DownButton;
        }
    }
}
