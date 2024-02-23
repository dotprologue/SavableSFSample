namespace SavableSFSample
{
    public interface IPrimitiveDeserializer
    {
        T Deserialize<T>(string input) where T : class;
    }
}