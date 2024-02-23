namespace SavableSFSample
{
    public interface ILogStacker
    {
        void StackLog(string characterName, string dialogueLine, bool doesAttachScenario);
    }
}