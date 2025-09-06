using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class Tk2dPlayAnimationWaitV2 : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmString ClipName;

		public FsmEvent AnimationCompleteEvent;

		private tk2dSpriteAnimator sprite;

		private bool isValid;

		private bool hasExpectedClip;

		private tk2dSpriteAnimationClip expectedClip;

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
				isValid = false;
				return;
			}
			isValid = true;
			IHeroAnimationController component = sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				expectedClip = component.GetClip(ClipName.Value);
				sprite.Play(expectedClip);
			}
			else
			{
				expectedClip = sprite.GetClipByName(ClipName.Value);
				sprite.Play(expectedClip);
			}
			hasExpectedClip = expectedClip != null;
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

		public override void OnUpdate()
		{
			if (!hasExpectedClip)
			{
				Finish();
			}
			else if (sprite.CurrentClip != expectedClip)
			{
				base.Fsm.Event(AnimationCompleteEvent);
				Finish();
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
