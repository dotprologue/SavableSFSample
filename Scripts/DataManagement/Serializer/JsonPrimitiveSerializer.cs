using UnityEngine;

namespace SavableSFSample
{
    public class JsonPrimitiveSerializer : IPrimitiveSerializer, IPrimitiveDeserializer
    {
        public string Serialize<T>(T input) where T : class
        {
            return JsonUtility.ToJson(input);
        }

        public T Deserialize<T>(string input) where T : class
        {
            return JsonUtility.FromJson<T>(input);
        }
    }
}