using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroWallJumpBrollyCheck : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool IsFacingRight;

		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		public override void Reset()
		{
			Target = null;
			IsFacingRight = null;
			TrueEvent = null;
			FalseEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if ((bool)component)
				{
					bool flag = component.IsFacingNearWall(IsFacingRight.Value, component.WALLJUMP_BROLLY_RAY_LENGTH, Color.green);
					base.Fsm.Event(flag ? TrueEvent : FalseEvent);
				}
			}
			Finish();
		}
	}
}
