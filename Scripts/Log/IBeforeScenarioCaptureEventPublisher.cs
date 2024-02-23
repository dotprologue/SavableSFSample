using System;

namespace SavableSFSample
{
    public interface IBeforeScenarioCaptureEventPublisher
    {
        event Action BeforeScenarioCapture;
    }
}