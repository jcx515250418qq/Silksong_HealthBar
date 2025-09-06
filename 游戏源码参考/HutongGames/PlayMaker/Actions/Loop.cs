namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends the Loop Event when the action runs. It loops the specified number of times then sends the Finish Event. ")]
	public class Loop : FsmStateAction
	{
		[RequiredField]
		[Tooltip("How many times to loop.")]
		public FsmInt loops;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current loop count. Starts at 0. Useful for iterating through arrays.")]
		public FsmInt storeCurrentLoop;

		[Tooltip("Event that starts a loop.")]
		public FsmEvent loopEvent;

		[Tooltip("Event to send when the loops have finished.")]
		public FsmEvent finishEvent;

		private int loopedCount;

		public override void OnEnter()
		{
			storeCurrentLoop.Value = loopedCount;
			loopedCount++;
			if (loopedCount > loops.Value)
			{
				loopedCount = 0;
				base.Fsm.Event(finishEvent);
				Finish();
			}
			else
			{
				base.Fsm.Event(loopEvent);
			}
		}
	}
}
