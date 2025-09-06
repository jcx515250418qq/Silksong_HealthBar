namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByNameOnExit : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[RequiredField]
		public FsmString sendEvent;

		public override void Reset()
		{
			eventTarget = null;
			sendEvent = null;
		}

		public override void OnExit()
		{
			base.Fsm.Event(eventTarget, sendEvent.Value);
		}
	}
}
