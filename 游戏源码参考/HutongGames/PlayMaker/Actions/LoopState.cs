namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Note("Put this action at the end of a State to loop through all actions in a state the specified number of times.")]
	[Tooltip("Loops through the state the specified number of times then sends the Finish Event.")]
	public class LoopState : FsmStateAction
	{
		[RequiredField]
		[Tooltip("How many times to loop through the state.")]
		public FsmInt loops;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current loop count. Starts at 0. Useful for iterating through arrays.")]
		public FsmInt storeCurrentLoop;

		[Tooltip("Event to send when the loops have finished.")]
		public FsmEvent finishEvent;

		private int loopedCount;

		public override void OnEnter()
		{
			storeCurrentLoop.Value = loopedCount;
			loopedCount++;
			if (loopedCount >= loops.Value)
			{
				loopedCount = 0;
				base.Fsm.Event(finishEvent);
				Finish();
			}
			else
			{
				base.Fsm.SwitchState(base.Fsm.ActiveState);
			}
		}
	}
}
