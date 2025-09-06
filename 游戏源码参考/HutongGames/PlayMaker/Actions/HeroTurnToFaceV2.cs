using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroTurnToFaceV2 : FsmStateAction
	{
		private const string TURN_ANIM_NAME = "TurnWalk";

		public FsmOwnerDefault Target;

		public FsmEvent didNotTurnEvent;

		public FsmEvent didTurnEvent;

		public FsmEvent turnFinishedEvent;

		private tk2dSpriteAnimator heroAnimator;

		private HeroController hc;

		public override void Reset()
		{
			Target = null;
			didNotTurnEvent = null;
			didTurnEvent = null;
			turnFinishedEvent = null;
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
				base.Fsm.Event(didNotTurnEvent);
				Finish();
				return;
			}
			hc.StopAnimationControl();
			heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
			heroAnimator.Play("TurnWalk");
			hc.FlipSprite();
			heroAnimator.AnimationCompletedEvent += OnHeroAnimationCompleted;
			heroAnimator.AnimationChanged += OnAnimationChanged;
			base.Fsm.Event(didTurnEvent);
		}

		private void OnAnimationChanged(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
		{
			if (newclip.name != "TurnWalk")
			{
				base.Fsm.Event(turnFinishedEvent);
				hc.StartAnimationControl();
				Finish();
				heroAnimator.AnimationCompletedEvent -= OnHeroAnimationCompleted;
				heroAnimator.AnimationChanged -= OnAnimationChanged;
				heroAnimator = null;
			}
		}

		public override void OnExit()
		{
			if ((bool)heroAnimator)
			{
				heroAnimator.AnimationCompletedEvent -= OnHeroAnimationCompleted;
				heroAnimator.AnimationChanged -= OnAnimationChanged;
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
			base.Fsm.Event(turnFinishedEvent);
			hc.StartAnimationControl();
			if (hc.hero_state == ActorStates.no_input)
			{
				hc.AnimCtrl.PlayIdle();
			}
			Finish();
			heroAnimator.AnimationCompletedEvent -= OnHeroAnimationCompleted;
			heroAnimator.AnimationChanged -= OnAnimationChanged;
			heroAnimator = null;
		}
	}
}
