namespace HutongGames.PlayMaker.Actions
{
	public class CanHeroBeGrabbed : FsmStateAction
	{
		public FsmEventTarget eventTarget;

		public FsmEvent canTakeDmgEvent;

		public FsmEvent cannotTakeDmgEvent;

		public override void Reset()
		{
			eventTarget = null;
			canTakeDmgEvent = null;
			cannotTakeDmgEvent = null;
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance.CanBeGrabbed() && !instance.WillDoBellBindHit(sendEvent: true))
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
