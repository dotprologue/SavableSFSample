namespace SavableSFSample
{
    public interface ISkipSwitch
    {
        bool IsSkipActive { get; }
        void SwitchSkip(bool isActive);
    }
}