namespace SavableSFSample
{
    public interface IPrimitiveSerializer
    {
        string Serialize<T>(T input) where T : class;
    }
}