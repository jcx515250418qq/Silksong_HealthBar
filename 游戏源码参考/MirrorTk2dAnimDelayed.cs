using UnityEngine;

public class MirrorTk2dAnimDelayed : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator mirrorAnimator;

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private float delay;

	private tk2dSpriteAnimationClip previouslyPlaying;

	private float delayLeft;

	private void OnDisable()
	{
		delayLeft = 0f;
		previouslyPlaying = null;
	}

	private void LateUpdate()
	{
		if (delayLeft > 0f)
		{
			delayLeft -= Time.deltaTime;
			if (delayLeft <= 0f)
			{
				animator.Play(previouslyPlaying);
			}
		}
		if ((previouslyPlaying == null || !mirrorAnimator.IsPlaying(previouslyPlaying)) && mirrorAnimator.Playing)
		{
			delayLeft = delay;
			previouslyPlaying = mirrorAnimator.CurrentClip;
			if (delayLeft <= 0f)
			{
				animator.Play(previouslyPlaying);
			}
		}
	}
}
