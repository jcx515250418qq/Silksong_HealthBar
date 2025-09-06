namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the event that caused the transition to the current state, and stores it in a String Variable.")]
	public class GetLastEvent : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name of the last event in a String Variable.")]
		public FsmString storeEvent;

		public override void Reset()
		{
			storeEvent = null;
		}

		public override void OnEnter()
		{
			storeEvent.Value = ((base.Fsm.LastTransition == null) ? "START" : base.Fsm.LastTransition.EventName);
			Finish();
		}
	}
}
