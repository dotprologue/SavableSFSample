namespace SavableSFSample
{
    public interface IAutoSwitch
    {
        bool IsAutoActive { get; }
        void SwitchAuto(bool isActive);
    }
}