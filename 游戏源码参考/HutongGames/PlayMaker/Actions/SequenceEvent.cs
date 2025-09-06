namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends the next event on the state each time the state is entered.")]
	public class SequenceEvent : FsmStateAction
	{
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay before sending the event in seconds.")]
		public FsmFloat delay;

		[UIHint(UIHint.Variable)]
		[Tooltip("Assign a variable to control reset. Set it to True to reset the sequence. Value is set to False after resetting.")]
		public FsmBool reset;

		private DelayedEvent delayedEvent;

		private int eventIndex;

		public override void Reset()
		{
			delay = null;
		}

		public override void OnEnter()
		{
			if (reset.Value)
			{
				eventIndex = 0;
				reset.Value = false;
			}
			int num = base.State.Transitions.Length;
			if (num > 0)
			{
				FsmEvent fsmEvent = base.State.Transitions[eventIndex].FsmEvent;
				if (delay.Value < 0.001f)
				{
					base.Fsm.Event(fsmEvent);
					Finish();
				}
				else
				{
					delayedEvent = base.Fsm.DelayedEvent(fsmEvent, delay.Value);
				}
				eventIndex++;
				if (eventIndex == num)
				{
					eventIndex = 0;
				}
			}
		}

		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(delayedEvent))
			{
				Finish();
			}
		}
	}
}
