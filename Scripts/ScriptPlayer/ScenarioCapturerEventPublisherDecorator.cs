using System;

namespace SavableSFSample
{
    public class ScenarioCapturerEventPublisherDecorator : IScenarioCapturer, IBeforeScenarioCaptureEventPublisher
    {
        private readonly IScenarioCapturer scenarioCapturer;

        public event Action BeforeScenarioCapture;

        public ScenarioCapturerEventPublisherDecorator(IScenarioCapturer scenarioCapturer)
        {
            this.scenarioCapturer = scenarioCapturer ?? throw new ArgumentNullException(nameof(scenarioCapturer));
        }

        public ScenarioRecord Capture()
        {
            BeforeScenarioCapture?.Invoke();
            return scenarioCapturer.Capture();
        }
    }
}
