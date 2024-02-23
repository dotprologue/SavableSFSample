namespace SavableSFSample
{
    public interface IAutoActivator
    {
        float Duration { get; set; }
        bool IsActive { get; set; }
    }
}