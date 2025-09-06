using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class tk2dPlayAnimAfterPreviousCompleteV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString AnimName;

		public FsmBool RandomFrame;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidPlay;

		private tk2dSpriteAnimator animator;

		public override void Reset()
		{
			Target = null;
			AnimName = null;
			RandomFrame = null;
			StoreDidPlay = null;
		}

		public override void OnEnter()
		{
			StoreDidPlay.Value = false;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				animator = safe.GetComponent<tk2dSpriteAnimator>();
				if ((bool)animator)
				{
					tk2dSpriteAnimator tk2dSpriteAnimator = animator;
					tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
					return;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			ClearEvent(ref animator);
		}

		private void ClearEvent(ref tk2dSpriteAnimator animator)
		{
			if ((bool)animator)
			{
				tk2dSpriteAnimator obj = animator;
				obj.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(obj.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
				animator = null;
			}
		}

		private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
		{
			if (!string.IsNullOrEmpty(AnimName.Value))
			{
				tk2dSpriteAnimationClip clipByName = animator.GetClipByName(AnimName.Value);
				if (RandomFrame.Value)
				{
					animator.PlayFromFrame(clipByName, UnityEngine.Random.Range(0, clip.frames.Length));
				}
				else
				{
					animator.Play(clipByName);
				}
				StoreDidPlay.Value = true;
				ClearEvent(ref animator);
				Finish();
			}
		}
	}
}
