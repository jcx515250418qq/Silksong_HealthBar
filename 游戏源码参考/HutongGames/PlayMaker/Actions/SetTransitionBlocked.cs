using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetTransitionBlocked : FsmStateAction
	{
		public FsmFloat SetTime;

		[RequiredField]
		public FsmBool SetIsBlocked;

		public override void Reset()
		{
			SetTime = null;
			SetIsBlocked = null;
		}

		public override void OnEnter()
		{
			if (SetTime.Value <= 0f)
			{
				Set();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		public override void OnExit()
		{
			if (!base.Finished)
			{
				Set();
			}
		}

		private void DoAction()
		{
			if (Time.time >= SetTime.Value)
			{
				Set();
			}
		}

		private void Set()
		{
			TransitionPoint.IsTransitionBlocked = SetIsBlocked.Value;
			Finish();
		}
	}
}
