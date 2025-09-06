using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class LookAnimNpcForceFacing : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public FsmBool ForceFacing;

		[HideIf("HideFacing")]
		public FsmBool FaceLeft;

		public FsmBool WaitUntilFinished;

		public FsmEvent FinishedTurning;

		private int notTurningCount;

		private LookAnimNPC lookAnimNpc;

		private bool hasLookNpc;

		protected override bool AutoFinish => false;

		public bool HideFacing()
		{
			return !ForceFacing.Value;
		}

		public override void Reset()
		{
			base.Reset();
			ForceFacing = null;
			FaceLeft = null;
			WaitUntilFinished = null;
			FinishedTurning = null;
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			hasLookNpc = lookAnim != null;
			lookAnimNpc = lookAnim;
			if (ForceFacing.Value)
			{
				lookAnim.ForceTurn(FaceLeft.Value);
			}
			else
			{
				lookAnim.UnlockTurn();
			}
			notTurningCount = 0;
			if (!WaitUntilFinished.Value)
			{
				Finish();
			}
		}

		protected override void DoActionNoComponent(GameObject target)
		{
			FinishTurn();
		}

		public override void OnUpdate()
		{
			if (!hasLookNpc)
			{
				FinishTurn();
				return;
			}
			if (!lookAnimNpc.IsTurning)
			{
				notTurningCount++;
			}
			else
			{
				notTurningCount = 0;
			}
			if (notTurningCount > 1)
			{
				FinishTurn();
			}
		}

		private void FinishTurn()
		{
			base.Fsm.Event(FinishedTurning);
			Finish();
		}
	}
}
