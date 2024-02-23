namespace SavableSFSample
{
    //Manage the lock of the scenario progression
    //For example, the lock is enabled when the player entered the menu
    public class ScenarioLockHolder : IScenarioLocker, IScenarioUnlocker, IScenarioLockGetter
    {
        public bool IsLocked { get; private set; }

        public void Lock()
        {
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }
    }
}