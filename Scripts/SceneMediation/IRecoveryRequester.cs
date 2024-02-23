namespace SavableSFSample
{
    public interface IRecoveryRequester
    {
        void RequestRecovery(LogRecord recoveryLogRecord);
    }
}
