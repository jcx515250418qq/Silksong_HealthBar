using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class tk2dPlayAnimationOptionalReset : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString AnimName;

		public FsmBool ResetFrame;

		public bool EveryFrame;

		private tk2dSpriteAnimator animator;

		private IHeroAnimationController heroAnim;

		public override void Reset()
		{
			Target = null;
			AnimName = null;
			ResetFrame = null;
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
			tk2dSpriteAnimationClip clip = ((heroAnim != null) ? heroAnim.GetClip(AnimName.Value) : animator.GetClipByName(AnimName.Value));
			if (ResetFrame.Value)
			{
				animator.PlayFromFrame(clip, 0);
			}
			else
			{
				animator.Play(clip);
			}
		}
	}
}
