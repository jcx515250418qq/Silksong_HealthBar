namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Checks if object has an active non bouncer.")]
	public sealed class HasActiveNonBouncer : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NonBouncer))]
		public FsmOwnerDefault Target;

		public FsmBool requireActive;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		public override void Reset()
		{
			Target = null;
			requireActive = true;
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter()
		{
			NonBouncer safe = Target.GetSafe<NonBouncer>(this);
			if (safe != null && (safe.active || !requireActive.Value))
			{
				base.Fsm.Event(trueEvent);
				Finish();
			}
			else
			{
				base.Fsm.Event(falseEvent);
				Finish();
			}
		}
	}
}
