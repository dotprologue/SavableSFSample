using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavableSFSample
{
    [Serializable]
    public class ScenarioPack
    {
        [SerializeField]
        private LogRecord logRecord;
        [SerializeField]
        private string characterName;
        [SerializeField]
        private string dialogueLine;
        [SerializeField]
        private byte[] textureBytes;

        public ScenarioPack(LogRecord logRecord, string characterName, string dialogueLine, IEnumerable<byte> textureBytes)
        {
            this.logRecord = logRecord ?? throw new ArgumentNullException(nameof(logRecord));
            this.characterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
            this.dialogueLine = dialogueLine ?? throw new ArgumentNullException(nameof(dialogueLine));
            if (textureBytes == null)
                throw new ArgumentNullException(nameof(textureBytes));
            this.textureBytes = textureBytes.ToArray();
        }

        public LogRecord LogRecord => logRecord;
        public string CharacterName => characterName;
        public string DialogueLine => dialogueLine;
        public IEnumerable<byte> TextureBytes => textureBytes;
    }
}
