namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in LateUpdate, after the Update loop.")]
	public class LateUpdateEvent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Event to send in LateUpdate.")]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			sendEvent = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnEnter()
		{
		}

		public override void OnLateUpdate()
		{
			Finish();
			base.Fsm.Event(sendEvent);
		}
	}
}
