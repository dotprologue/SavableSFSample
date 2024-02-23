namespace SavableSFSample
{
    public interface IRecoveryStatusReader
    {
        string ScenarioScriptId { get; }
        int StartIndex { get; }
    }
}
