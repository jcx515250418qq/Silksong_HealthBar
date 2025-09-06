namespace HutongGames.PlayMaker.Actions
{
	public class CanHeroBeGrabbedV2 : FsmStateAction
	{
		public FsmEventTarget eventTarget;

		public FsmEvent canTakeDmgEvent;

		public FsmEvent cannotTakeDmgEvent;

		public FsmBool ignoreParryState;

		public FsmBool ignoreBellBind;

		public FsmBool triggerBellBindEffect;

		public override void Reset()
		{
			eventTarget = null;
			canTakeDmgEvent = null;
			cannotTakeDmgEvent = null;
			ignoreParryState = null;
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance.CanBeGrabbed(ignoreParryState.Value) && (!ignoreBellBind.Value || !instance.WillDoBellBindHit(triggerBellBindEffect.Value)))
			{
				base.Fsm.Event(eventTarget, canTakeDmgEvent);
			}
			else
			{
				base.Fsm.Event(eventTarget, cannotTakeDmgEvent);
			}
			Finish();
		}
	}
}
