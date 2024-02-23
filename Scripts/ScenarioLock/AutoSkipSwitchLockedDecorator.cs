using System;

namespace SavableSFSample
{
    //Don't turn on either the auto mode and the skip mode when the scenario progression is locked
    public class AutoSkipSwitchLockedDecorator : IAutoSwitch, ISkipSwitch
    {
        private IAutoSwitch autoSwitch;
        private ISkipSwitch skipSwitch;
        private IScenarioLockGetter scenarioLockGetter;

        public bool IsAutoActive => autoSwitch.IsAutoActive;
        public bool IsSkipActive => skipSwitch.IsSkipActive;

        public AutoSkipSwitchLockedDecorator(IAutoSwitch autoSwitch, ISkipSwitch skipSwitch, IScenarioLockGetter scenarioLockGetter)
        {
            this.autoSwitch = autoSwitch ?? throw new ArgumentNullException(nameof(autoSwitch));
            this.skipSwitch = skipSwitch ?? throw new ArgumentNullException(nameof(skipSwitch));
            this.scenarioLockGetter = scenarioLockGetter ?? throw new ArgumentNullException(nameof(scenarioLockGetter));
        }

        public void SwitchAuto(bool isActive)
        {
            autoSwitch.SwitchAuto(isActive && !scenarioLockGetter.IsLocked);
        }

        public void SwitchSkip(bool isActive)
        {
            skipSwitch.SwitchSkip(isActive && !scenarioLockGetter.IsLocked);
        }

    }
}