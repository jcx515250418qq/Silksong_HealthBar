using UnityEngine;

public class HeroAnimationControllerProxy : MonoBehaviour, IHeroAnimationController
{
	public tk2dSpriteAnimationClip GetClip(string clipName)
	{
		if (HeroController.instance == null)
		{
			return null;
		}
		return HeroController.instance.GetComponent<HeroAnimationController>().GetClip(clipName);
	}
}
