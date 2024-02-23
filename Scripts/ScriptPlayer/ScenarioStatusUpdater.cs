using ScenarioFlow;
using System;

namespace SavableSFSample
{
    public class ScenarioStatusUpdater : IScenarioStatusUpdater
    {
        private readonly IScenarioStatusSetter scenarioStatusSetter;
        private readonly IBeforeScenarioCaptureEventPublisher beforeScenarioCaptureEventPublisher;
        private readonly IScenarioCaptureExecutor scenarioCaptureExecutor;

        private Action beforeCaptureAction;
        private ScenarioBook observedScenarioBook;

        public ScenarioStatusUpdater(IScenarioStatusSetter scenarioStatusSetter, IBeforeScenarioCaptureEventPublisher beforeScenarioCaptureEventPublisher, IScenarioCaptureExecutor scenarioCaptureExecutor)
        {
            this.scenarioStatusSetter = scenarioStatusSetter ?? throw new ArgumentNullException(nameof(scenarioStatusSetter));
            this.beforeScenarioCaptureEventPublisher = beforeScenarioCaptureEventPublisher ?? throw new ArgumentNullException(nameof(beforeScenarioCaptureEventPublisher));
            this.scenarioCaptureExecutor = scenarioCaptureExecutor ?? throw new ArgumentNullException(nameof(scenarioCaptureExecutor));
        }

        public void StartUpdate(string scenarioScriptPath, ScenarioBook observedScenarioBook)
        {
            if (observedScenarioBook == null)
                throw new ArgumentNullException(nameof(observedScenarioBook));
            StopUpdate();
            this.observedScenarioBook = observedScenarioBook;
            scenarioStatusSetter.ScenarioScriptPath = scenarioScriptPath;
            scenarioStatusSetter.RestartIndex = 0;
            //First capture
            scenarioCaptureExecutor.ExecuteCapture();
            //Update status every time the capture is executed
            beforeCaptureAction = UpdateStatus;
            beforeScenarioCaptureEventPublisher.BeforeScenarioCapture += beforeCaptureAction;
        }

        public void ContinueUpdate(string scenarioScriptPath, ScenarioBook observedScenarioBook)
        {
            if (observedScenarioBook == null)
                throw new ArgumentNullException(nameof(observedScenarioBook));
            StopUpdate();
            this.observedScenarioBook = observedScenarioBook;
            scenarioStatusSetter.ScenarioScriptPath = scenarioScriptPath;
            scenarioStatusSetter.RestartIndex = observedScenarioBook.CurrentIndex;
            beforeCaptureAction = UpdateStatus;
            beforeScenarioCaptureEventPublisher.BeforeScenarioCapture += beforeCaptureAction;
        }

        public void StopUpdate()
        {
            if (observedScenarioBook != null)
            {
                beforeScenarioCaptureEventPublisher.BeforeScenarioCapture -= beforeCaptureAction;
                beforeCaptureAction = null;
                observedScenarioBook = null;
            }
        }

        private void UpdateStatus()
        {
            scenarioStatusSetter.RestartIndex = observedScenarioBook.CurrentIndex + 1;
        }
    }
}
