using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class LookAnimNpcGetFacingLeft : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public FsmEvent FacingLeft;

		public FsmEvent FacingRight;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public FsmBool EveryFrame;

		private int notTurningCount;

		private LookAnimNPC lookAnimNpc;

		private bool hasLookNpc;

		protected override bool AutoFinish => false;

		public override void Reset()
		{
			base.Reset();
			StoreValue = null;
			EveryFrame = null;
			FacingLeft = null;
			FacingRight = null;
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			hasLookNpc = lookAnim != null;
			lookAnimNpc = lookAnim;
			SendEvent();
			if (!EveryFrame.Value)
			{
				Finish();
			}
		}

		protected override void DoActionNoComponent(GameObject target)
		{
			Finish();
		}

		public override void OnUpdate()
		{
			if (!hasLookNpc)
			{
				Finish();
			}
			else
			{
				SendEvent();
			}
		}

		private void SendEvent()
		{
			if (hasLookNpc)
			{
				if (StoreValue != null)
				{
					StoreValue.Value = lookAnimNpc.WasFacingLeft;
				}
				if (lookAnimNpc.WasFacingLeft)
				{
					base.Fsm.Event(FacingLeft);
				}
				else
				{
					base.Fsm.Event(FacingRight);
				}
			}
		}
	}
}
