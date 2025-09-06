namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Forward an event received by this FSM to another target.")]
	public class ForwardEvent : FsmStateAction
	{
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		[Tooltip("The events to forward.")]
		public FsmEvent[] eventsToForward;

		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;

		public override void Reset()
		{
			forwardTo = new FsmEventTarget
			{
				target = FsmEventTarget.EventTarget.FSMComponent
			};
			eventsToForward = null;
			eatEvents = true;
		}

		public override void Awake()
		{
			base.HandlesOnEvent = true;
		}

		public override bool Event(FsmEvent fsmEvent)
		{
			if (eventsToForward != null)
			{
				FsmEvent[] array = eventsToForward;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == fsmEvent)
					{
						base.Fsm.Event(forwardTo, fsmEvent);
						return eatEvents;
					}
				}
			}
			return false;
		}
	}
}
