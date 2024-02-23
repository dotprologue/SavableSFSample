namespace SavableSFSample
{
    public interface IRecoverable
    {
        string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer);

        void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer);
    }
}
