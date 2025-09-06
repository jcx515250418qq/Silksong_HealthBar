using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AnimatorPauseAtTime : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString Anim;

		public FsmFloat Time;

		public override void Reset()
		{
			Target = null;
			Anim = null;
			Time = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			Animator animator = (safe ? safe.GetComponent<Animator>() : null);
			if (animator != null)
			{
				animator.Play(Anim.Name, 0, Time.Value);
				animator.enabled = false;
				animator.Update(0f);
			}
			Finish();
		}
	}
}
