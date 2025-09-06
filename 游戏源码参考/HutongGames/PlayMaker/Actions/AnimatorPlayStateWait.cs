using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Plays an animator state and waits until it's completion. Will never finish if state or animator can not be found.")]
	public class AnimatorPlayStateWait : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmString stateName;

		public FsmEvent finishEvent;

		private Animator animator;

		private bool hasWaited;

		private double? resumeTime;

		private int stateHash;

		public override void Reset()
		{
			target = null;
			stateName = null;
			finishEvent = null;
		}

		public override void OnEnter()
		{
			hasWaited = false;
			resumeTime = null;
			GameObject safe = target.GetSafe(this);
			if ((bool)safe)
			{
				animator = safe.GetComponent<Animator>();
				if ((bool)animator && !string.IsNullOrWhiteSpace(stateName.Value))
				{
					animator.Play(stateName.Value, 0, 0f);
				}
			}
		}

		public override void OnUpdate()
		{
			if (hasWaited)
			{
				if ((bool)animator && !resumeTime.HasValue)
				{
					AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					resumeTime = (double)currentAnimatorStateInfo.length + Time.timeAsDouble;
					stateHash = currentAnimatorStateInfo.shortNameHash;
				}
				if ((resumeTime.HasValue && Time.timeAsDouble >= resumeTime) || animator.GetCurrentAnimatorStateInfo(0).shortNameHash != stateHash)
				{
					base.Fsm.Event(finishEvent);
					Finish();
				}
			}
			else
			{
				hasWaited = true;
			}
		}
	}
}
