using JetBrains.Annotations;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Relinquishes hero control, gracefully ending current actions (e.g., sprinting)")]
	[UsedImplicitly]
	public class HeroRelinquishControlDynamic : FsmStateAction
	{
		private HeroController hc;

		public override void OnEnter()
		{
			hc = HeroController.instance;
			if (hc.cState.isSprinting)
			{
				hc.sprintFSM.SendEvent("SKID END");
				return;
			}
			hc.RelinquishControl();
			Finish();
		}

		public override void OnUpdate()
		{
			if (!hc.controlReqlinquished)
			{
				hc.RelinquishControl();
				Finish();
			}
		}

		public override void OnExit()
		{
			if (!hc.controlReqlinquished)
			{
				hc.RelinquishControl();
			}
		}
	}
}
