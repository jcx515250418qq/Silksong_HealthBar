using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class FsmStateActionAnimatorBase : ComponentAction<Animator>
	{
		public enum AnimatorFrameUpdateSelector
		{
			OnUpdate = 0,
			OnAnimatorMove = 1,
			OnAnimatorIK = 2
		}

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Select when to perform the action, during OnUpdate, OnAnimatorMove, OnAnimatorIK")]
		public AnimatorFrameUpdateSelector everyFrameOption;

		protected int IklayerIndex;

		public abstract void OnActionUpdate();

		public override void Reset()
		{
			everyFrame = false;
			everyFrameOption = AnimatorFrameUpdateSelector.OnUpdate;
		}

		public override void OnPreprocess()
		{
			if (everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorMove)
			{
				base.Fsm.HandleAnimatorMove = true;
			}
			if (everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				base.Fsm.HandleAnimatorIK = true;
			}
		}

		public override void OnUpdate()
		{
			if (everyFrameOption == AnimatorFrameUpdateSelector.OnUpdate)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void DoAnimatorMove()
		{
			if (everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorMove)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void DoAnimatorIK(int layerIndex)
		{
			IklayerIndex = layerIndex;
			if (everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}
	}
}
