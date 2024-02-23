using System;

namespace SavableSFSample
{
    //When the scenario progression is locked, turn off both the auto mode and the skip mode
    public class ScenarioLockerAutoSkipBreakDecorator : IScenarioLocker
    {
        private IScenarioLocker scenarioLocker;
        private IAutoSwitch autoSwitch;
        private ISkipSwitch skipSwitch;

        public ScenarioLockerAutoSkipBreakDecorator(IScenarioLocker scenarioLocker, IAutoSwitch autoSwitch, ISkipSwitch skipSwitch)
        {
            this.scenarioLocker = scenarioLocker ?? throw new ArgumentNullException(nameof(scenarioLocker));
            this.autoSwitch = autoSwitch ?? throw new ArgumentNullException(nameof(autoSwitch));
            this.skipSwitch = skipSwitch ?? throw new ArgumentNullException(nameof(skipSwitch));
        }

        public void Lock()
        {
            scenarioLocker.Lock();
            autoSwitch.SwitchAuto(false);
            skipSwitch.SwitchSkip(false);
        }
    }
}
