using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavableSFSample
{
    public class CharacterProvider : IReflectable, IRecoverable
    {
        private readonly GameObject characterObjectParent;
        private readonly CharacterObject characterObjectPrefab;
        private readonly Dictionary<string, CharacterObject> characterDictionary = new Dictionary<string, CharacterObject>();

        public CharacterProvider(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (settings.CharacterObjectParent == null)
                throw new ArgumentNullException(nameof(settings.CharacterObjectParent));
            if (settings.CharacterObjectPrefab == null)
                throw new ArgumentNullException(nameof(settings.CharacterObjectPrefab));

            characterObjectParent = settings.CharacterObjectParent;
            characterObjectPrefab = settings.CharacterObjectPrefab;
        }

        [CommandMethod("add character")]
        [Category("Character")]
        [Description("Add a new character to the scene.")]
        [Snippet("Add a new character {${1:name}}.")]
        public void AddCharacter(string name)
        {
            //Make sure that a character with the same name doesn't exist
            if (characterDictionary.ContainsKey(name))
            {
                throw new ArgumentException($"Character '{name}' exists already.");
            }
            //Create a new character
            var newObject = GameObject.Instantiate(characterObjectPrefab.gameObject, characterObjectParent.transform);
            newObject.name = name;
            characterDictionary.Add(name, newObject.GetComponent<CharacterObject>());
        }

		[CommandMethod("remove character")]
		[Category("Character")]
		[Description("Remove a character in the scene.")]
		[Snippet("Remove the character {${1:name}}.")]
		public void RemoveCharacter(string name)
        {
            //Make sure that the character exists
            if (!characterDictionary.ContainsKey(name))
            {
                throw new ArgumentException($"Character '{name}' does not exist.");
            }
            //Destroy the character
            GameObject.Destroy(characterDictionary[name].gameObject);
            characterDictionary.Remove(name);
        }

		[CommandMethod("remove all characters")]
		[Category("Character")]
		[Description("Remove all characters in the scene.")]
		[Snippet("Remove all the characters.")]
		public void RemoveCharacterAll()
        {
            foreach (var name in characterDictionary.Keys.ToArray())
            {
                RemoveCharacter(name);
            }
        }

		[DecoderMethod]
		[Description("A decoder for the 'CharacterObject' type.")]
		[Description("Returns a character object with the character name in the scene.")]
		public CharacterObject GetCharacterObject(string name)
        {
            //Make sure that the character exists
            if (!characterDictionary.ContainsKey(name))
            {
                throw new ArgumentException($"Character '{name}' does not exist.");
            }
            return characterDictionary[name];
        }

        public string Capture(IPrimitiveSerializer primitiveSerializer, IAssetSerializer assetSerializer)
        {
            var fragments = characterDictionary.Values.Select(character =>
            {
                var sprite = character.GetComponent<SpriteRenderer>().sprite;
                var spriteCode = sprite == null ? string.Empty : assetSerializer.Serialize(character.GetComponent<SpriteRenderer>().sprite);
                return new CharacterFragment
                {
                    Name = character.name,
                    SpriteCode = spriteCode,
                    Position = character.transform.position
                };
            });
            var record = new Record
            {
                CharacterFragments = fragments.ToArray()
            };
            return primitiveSerializer.Serialize(record);
        }

        public void Recover(string input, IPrimitiveDeserializer primitiveDeserializer, IAssetDeserializer assetDeserializer)
        {
            //Destroy all characters at first
            RemoveCharacterAll();
            //Recover
            var record = primitiveDeserializer.Deserialize<Record>(input);
            foreach (var fragment in record.CharacterFragments)
            {
                AddCharacter(fragment.Name);
                var characterObject = characterDictionary[fragment.Name];
                var sprite = fragment.SpriteCode == string.Empty ? null : assetDeserializer.Deserialize<Sprite>(fragment.SpriteCode);
                characterObject.GetComponent<SpriteRenderer>().sprite = sprite;
                characterObject.transform.position = fragment.Position;
            }
        }

        [Serializable]
        public class Record
        {
            public CharacterFragment[] CharacterFragments;
        }

        [Serializable]
        public class CharacterFragment
        {
            public string Name;
            public string SpriteCode;
            public Vector3 Position;
        }

        [Serializable]
        public class Settings
        {
            public GameObject CharacterObjectParent;
            public CharacterObject CharacterObjectPrefab;
        }
    }
}
