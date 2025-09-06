using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroTurnToFace : FsmStateAction
	{
		private const string TURN_ANIM_NAME = "TurnWalk";

		public FsmOwnerDefault Target;

		private tk2dSpriteAnimator heroAnimator;

		private HeroController hc;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			hc = HeroController.instance;
			Transform transform = hc.transform;
			float num = safe.transform.position.x - transform.position.x;
			float x = transform.localScale.x;
			bool flag = false;
			if (x > 0f)
			{
				if (num > 0f)
				{
					flag = true;
				}
			}
			else if (num < 0f)
			{
				flag = true;
			}
			if (!flag)
			{
				Finish();
				return;
			}
			hc.StopAnimationControl();
			heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
			heroAnimator.Play("TurnWalk");
			hc.FlipSprite();
			tk2dSpriteAnimator tk2dSpriteAnimator = heroAnimator;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnHeroAnimationCompleted));
		}

		public override void OnExit()
		{
			if ((bool)heroAnimator)
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = heroAnimator;
				tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnHeroAnimationCompleted));
				heroAnimator = null;
			}
		}

		private void OnHeroAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
		{
			if (clip.name != "TurnWalk")
			{
				Debug.LogErrorFormat(base.Owner, "Wrong animation finished! Expected: {0}, Was: {1}", "TurnWalk", clip.name);
				return;
			}
			hc.StartAnimationControl();
			if (hc.hero_state == ActorStates.no_input)
			{
				hc.AnimCtrl.PlayIdle();
			}
			Finish();
			tk2dSpriteAnimator tk2dSpriteAnimator = heroAnimator;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnHeroAnimationCompleted));
			heroAnimator = null;
		}
	}
}
