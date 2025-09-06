namespace HutongGames.PlayMaker.Actions
{
	public class HeroSetWandererCrestState : FsmStateAction
	{
		public FsmBool QueuedNextHitCritical;

		public FsmBool CriticalHitsLocked;

		public override void Reset()
		{
			QueuedNextHitCritical = new FsmBool
			{
				UseVariable = true
			};
			CriticalHitsLocked = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			HeroController.WandererCrestStateInfo wandererState = instance.WandererState;
			if (!QueuedNextHitCritical.IsNone)
			{
				wandererState.QueuedNextHitCritical = QueuedNextHitCritical.Value;
			}
			if (!CriticalHitsLocked.IsNone)
			{
				wandererState.CriticalHitsLocked = CriticalHitsLocked.Value;
			}
			instance.WandererState = wandererState;
			Finish();
		}
	}
}
