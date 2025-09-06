using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class tk2dPlayAnimationConditional : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString AnimName;

		public FsmBool Condition;

		public bool EveryFrame;

		private tk2dSpriteAnimator animator;

		private IHeroAnimationController heroAnim;

		public override void Reset()
		{
			Target = null;
			AnimName = null;
			Condition = null;
			EveryFrame = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				animator = safe.GetComponent<tk2dSpriteAnimator>();
				heroAnim = safe.GetComponent<IHeroAnimationController>();
			}
			if (!animator)
			{
				Finish();
				return;
			}
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			if (Condition.Value)
			{
				if (heroAnim != null)
				{
					animator.Play(heroAnim.GetClip(AnimName.Value));
				}
				else
				{
					animator.Play(AnimName.Value);
				}
			}
		}
	}
}
