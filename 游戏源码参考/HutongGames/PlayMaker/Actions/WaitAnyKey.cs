namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Waits until any key is pressed then action finishes. Similar to AnyKey action but can be used in Action Sequences.")]
	public class WaitAnyKey : FsmStateAction
	{
		[Tooltip("Where to send the optional event")]
		public FsmEventTarget eventTarget;

		[Tooltip("Optional event to send when any Key or Mouse Button is pressed.")]
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
				Finish();
			}
		}
	}
}
