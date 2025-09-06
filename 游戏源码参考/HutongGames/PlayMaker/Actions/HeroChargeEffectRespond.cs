namespace HutongGames.PlayMaker.Actions
{
	public class HeroChargeEffectRespond : FsmStateAction
	{
		public FsmEvent ChargeBurstEvent;

		public FsmEvent ChargeEndEvent;

		private HeroChargeEffects charge;

		public override void Reset()
		{
			ChargeBurstEvent = null;
			ChargeEndEvent = null;
		}

		public override void OnEnter()
		{
			charge = ManagerSingleton<HeroChargeEffects>.Instance;
			charge.ChargeBurst += OnChargeBurst;
			charge.ChargeEnd += OnChargeEnd;
		}

		private void OnChargeBurst()
		{
			base.Fsm.Event(ChargeBurstEvent);
		}

		private void OnChargeEnd()
		{
			base.Fsm.Event(ChargeEndEvent);
			Finish();
		}

		public override void OnExit()
		{
			charge.ChargeBurst -= OnChargeBurst;
			charge.ChargeEnd -= OnChargeEnd;
		}
	}
}
