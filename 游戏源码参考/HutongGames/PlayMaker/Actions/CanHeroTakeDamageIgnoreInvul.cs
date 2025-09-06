namespace HutongGames.PlayMaker.Actions
{
	public class CanHeroTakeDamageIgnoreInvul : FsmStateAction
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
			if (HeroController.instance.CanTakeDamageIgnoreInvul())
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
