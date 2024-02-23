using System;

namespace SavableSFSample
{
    public class ScenarioStatusHolder : IScenarioStatusGetter, IScenarioStatusSetter, IRecoverable
    {
        public string ScenarioScriptPath { get; set; } = null;
        public int RestartIndex { get; set; } = -1;

        public string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var record = new Record
            {
                Path = ScenarioScriptPath,
                Index = RestartIndex
            };
            return primitiveSerializer.Serialize(record);
        }

        public void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            var record = primitiveDeserializer.Deserialize<Record>(input);
            ScenarioScriptPath = record.Path;
            RestartIndex = record.Index;
        }

        [Serializable]
        public class Record
        {
            public string Path;
            public int Index;
        }
    }
}
