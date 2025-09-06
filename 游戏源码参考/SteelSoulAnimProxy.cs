using GlobalEnums;
using UnityEngine;

[RequireComponent(typeof(tk2dSpriteAnimator))]
public class SteelSoulAnimProxy : MonoBehaviour, IHeroAnimationController
{
	[SerializeField]
	private tk2dSpriteAnimation steelSoulAnims;

	private tk2dSpriteAnimator animator;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	public tk2dSpriteAnimationClip GetClip(string clipName)
	{
		if (PlayerData.instance.permadeathMode == PermadeathModes.On && (bool)steelSoulAnims)
		{
			tk2dSpriteAnimationClip clipByName = steelSoulAnims.GetClipByName(clipName);
			if (clipByName != null)
			{
				return clipByName;
			}
		}
		tk2dSpriteAnimationClip clipByName2 = animator.Library.GetClipByName(clipName);
		if (clipByName2 != null)
		{
			return clipByName2;
		}
		return null;
	}
}
