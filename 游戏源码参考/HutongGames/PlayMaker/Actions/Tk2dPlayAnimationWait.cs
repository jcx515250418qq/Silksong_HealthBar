using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class Tk2dPlayAnimationWait : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmString ClipName;

		public FsmEvent AnimationCompleteEvent;

		private tk2dSpriteAnimator sprite;

		public override void Reset()
		{
			Target = null;
			ClipName = null;
			AnimationCompleteEvent = null;
		}

		public override void OnEnter()
		{
			GetSprite();
			if (sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
				Finish();
				return;
			}
			IHeroAnimationController component = sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				sprite.Play(component.GetClip(ClipName.Value));
			}
			else
			{
				sprite.Play(ClipName.Value);
			}
			sprite.PlayFromFrame(0);
			tk2dSpriteAnimator tk2dSpriteAnimator = sprite;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate));
		}

		public override void OnExit()
		{
			if (!(sprite == null))
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = sprite;
				tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate));
			}
		}

		private void GetSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
			if (!(ownerDefaultTarget == null))
			{
				sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
			}
		}

		private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{
			if (!(clip.name != ClipName.Value))
			{
				base.Fsm.Event(AnimationCompleteEvent);
				Finish();
			}
		}
	}
}
