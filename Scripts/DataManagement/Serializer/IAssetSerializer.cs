namespace SavableSFSample
{
    public interface IAssetSerializer
    {
        string Serialize<T>(T input) where T : UnityEngine.Object;
    }
}