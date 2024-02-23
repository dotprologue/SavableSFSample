namespace SavableSFSample
{
    public interface IAssetDeserializer
    {
        T Deserialize<T>(string input) where T : UnityEngine.Object;
    }
}