using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
	public interface ICharacterImageAnimator
	{
		UniTask ChangeCharacterImageAsync(CharacterObject characterObject, Sprite sprite, CancellationToken cancellationToken);
	}
}