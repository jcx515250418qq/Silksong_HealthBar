namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when the user hits any Key or Mouse Button.")]
	public class AnyKey : FsmStateAction
	{
		[Tooltip("Where to send the event")]
		public FsmEventTarget eventTarget;

		[RequiredField]
		[Tooltip("Event to send when any Key or Mouse Button is pressed.")]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			eventTarget = null;
			sendEvent = null;
		}

		public override void OnUpdate()
		{
			if (ActionHelpers.AnyKeyDown())
			{
				base.Fsm.Event(eventTarget, sendEvent);
			}
		}
	}
}
