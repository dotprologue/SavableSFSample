namespace SavableSFSample
{
    public interface IMessageWindow
    {
        void ShowMessage(string message);
        void ShowVolatileMessage(string message);
        void ClearMessage();
    }
}