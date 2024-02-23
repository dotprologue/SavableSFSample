using UnityEngine;

namespace SavableSFSample
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterObject : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
