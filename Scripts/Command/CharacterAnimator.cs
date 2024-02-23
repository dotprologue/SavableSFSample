using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
    public class CharacterAnimator : ICharacterImageAnimator, IReflectable
    {
        private static readonly float ChangeDuration = 0.2f;

        [CommandMethod("change character's image")]
        [Category("Character")]
        [Description("Change the character's image.")]
        [Snippet("Change the characer {${1:name}}'s image to {${2:image}}.")]
        public void ChangeCharacterImage(CharacterObject characterObject, Sprite sprite)
        {
            characterObject.SpriteRenderer.sprite = sprite;
        }

        [CommandMethod("change character's image async")]
        [Category("Character")]
        [Description("Change the character's image.")]
        [Snippet("Change {${1:name}}'s image to {${2:image}}.")]
        public async UniTask ChangeCharacterImageAsync(CharacterObject characterObject, Sprite sprite, CancellationToken cancellationToken)
        {
            if (characterObject.SpriteRenderer.sprite == null)
            {
                characterObject.SpriteRenderer.TransAlpha(0.0f);
                characterObject.SpriteRenderer.sprite = sprite;
                try
                {
                    await characterObject.SpriteRenderer.TransAlphaAsync(1.0f, ChangeDuration, TransSelector.UpDownQuad, cancellationToken);
                }
                finally
                {
                    characterObject.SpriteRenderer.TransAlpha(1.0f);
                }
            }
            else
            {
                var frontRenderer = GameObject.Instantiate<CharacterObject>(characterObject, characterObject.transform);
                frontRenderer.transform.position = characterObject.transform.position - Vector3.forward;
                characterObject.SpriteRenderer.sprite = sprite;
                try
                {
                    await frontRenderer.SpriteRenderer.TransAlphaAsync(0.0f, ChangeDuration, TransSelector.UpDownQuad, cancellationToken);
                }
                finally
                {
                    GameObject.Destroy(frontRenderer.gameObject);
                }
            }
        }

        [CommandMethod("move character")]
        [Category("Character")]
        [Description("Move the character to the specified position.")]
        [Snippet("Move the character {${1:name}} to {${2:position}}.")]
        public void MoveCharacter(CharacterObject characterObject, Vector3 position)
        {
            characterObject.transform.position = position;
        }

        [CommandMethod("move character async")]
        [Category("Character")]
        [Description("Move the character to the position in the specified seconds.")]
        [Snippet("Move {${1:name}} to the position {${2:position}} in {${3:n}} seconds.")]
        public async UniTask MoveCharacterAsync(CharacterObject characterObject, Vector3 position, float duration, CancellationToken cancellationToken)
        {
            try
            {
                await characterObject.transform.TransLocalPosAsync(position, duration, TransSelector.Linear, cancellationToken);
            }
            finally
            {
                characterObject.transform.localPosition = position;
            }
        }

    }
}
