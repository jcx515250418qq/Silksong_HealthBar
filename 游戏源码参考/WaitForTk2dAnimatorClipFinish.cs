using System;
using UnityEngine;

public class WaitForTk2dAnimatorClipFinish : CustomYieldInstruction
{
	private bool hasEnded;

	private tk2dSpriteAnimator animator;

	private Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int> onFrameChanged;

	private Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> onCompleted;

	private readonly Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> doOnCompletion;

	private tk2dSpriteAnimationClip currentClip;

	public override bool keepWaiting => !hasEnded;

	public WaitForTk2dAnimatorClipFinish(tk2dSpriteAnimator animator, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> onCompletion = null)
	{
		hasEnded = false;
		this.animator = animator;
		doOnCompletion = onCompletion;
		currentClip = animator.CurrentClip;
		if (animator.CurrentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
		{
			onCompleted = OnAnimationCompleted;
			animator.AnimationCompleted = onCompleted;
		}
		else
		{
			onFrameChanged = OnFrameChanged;
			animator.FrameChanged = onFrameChanged;
		}
		animator.AnimationChanged += AnimatorOnAnimationChanged;
	}

	private void AnimatorOnAnimationChanged(tk2dSpriteAnimator tk2dSpriteAnimator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
	{
		Cancel();
	}

	private void OnFrameChanged(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip, int previousFrame, int currentFrame)
	{
		if (currentFrame == clip.frames.Length - 1 || currentFrame <= previousFrame)
		{
			Cancel();
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		Cancel();
		doOnCompletion?.Invoke(anim, clip);
	}

	public void Cancel()
	{
		if (!hasEnded)
		{
			tk2dSpriteAnimator obj = animator;
			obj.FrameChanged = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>)Delegate.Remove(obj.FrameChanged, onFrameChanged);
			onFrameChanged = null;
			tk2dSpriteAnimator obj2 = animator;
			obj2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(obj2.AnimationCompleted, onCompleted);
			onCompleted = null;
			animator.AnimationChanged -= AnimatorOnAnimationChanged;
			hasEnded = true;
		}
	}
}
