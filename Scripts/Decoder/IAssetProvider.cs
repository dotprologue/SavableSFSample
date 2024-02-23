namespace SavableSFSample
{
    public interface IAssetProvider
    {
        T GetAsset<T>(string assetName) where T : UnityEngine.Object;
    }
}