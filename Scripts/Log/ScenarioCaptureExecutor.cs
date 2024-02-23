using System;

namespace SavableSFSample
{
    public class ScenarioCaptureExecutor : IScenarioCaptureExecutor
    {
        private readonly IScenarioCapturer scenarioCapturer;
        private readonly IScenarioRecordStacker scenarioRecordStacker;

        public ScenarioCaptureExecutor(IScenarioCapturer scenarioCapturer, IScenarioRecordStacker scenarioRecordStacker)
        {
            this.scenarioCapturer = scenarioCapturer ?? throw new ArgumentNullException(nameof(scenarioCapturer));
            this.scenarioRecordStacker = scenarioRecordStacker ?? throw new ArgumentNullException(nameof(scenarioRecordStacker));
        }

        public void ExecuteCapture()
        {
            var scenarioRecord = scenarioCapturer.Capture();
            scenarioRecordStacker.StackScenario(scenarioRecord);
        }
    }
}