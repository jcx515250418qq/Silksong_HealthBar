using System;
using UnityEngine;

public class tk2dAnimationTriggerFlipScale : MonoBehaviour
{
	[SerializeField]
	private string triggerInfoContains;

	[SerializeField]
	private bool flipX;

	[SerializeField]
	private bool flipY;

	[SerializeField]
	private bool resetOnComplete;

	private bool hasTriggered;

	private Vector3 initialScale;

	private tk2dSpriteAnimator animator;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	private void OnEnable()
	{
		initialScale = base.transform.localScale;
		tk2dSpriteAnimator obj = animator;
		obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		tk2dSpriteAnimator obj2 = animator;
		obj2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
	}

	private void OnDisable()
	{
		tk2dSpriteAnimator obj = animator;
		obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		tk2dSpriteAnimator obj2 = animator;
		obj2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(obj2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIndex)
	{
		string eventInfo = clip.GetFrame(frameIndex).eventInfo;
		if (string.IsNullOrEmpty(eventInfo) || string.IsNullOrEmpty(triggerInfoContains) || eventInfo.Contains(triggerInfoContains))
		{
			Vector3 localScale = base.transform.localScale;
			if (flipX)
			{
				localScale.x *= -1f;
			}
			if (flipY)
			{
				localScale.y *= -1f;
			}
			base.transform.localScale = localScale;
			hasTriggered = true;
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (hasTriggered)
		{
			hasTriggered = false;
			if (resetOnComplete)
			{
				base.transform.localScale = initialScale;
			}
		}
	}
}
