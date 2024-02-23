namespace SavableSFSample
{
    public interface IRecoveryExecutor
    {
        void ExecuteRecovery(LogRecord recoveryLogRecord);
    }
}