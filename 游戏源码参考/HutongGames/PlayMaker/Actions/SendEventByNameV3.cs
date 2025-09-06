namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByNameV3 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		[Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
		public bool everyFrame;

		[Tooltip("Event will only be sent if this bool is true - note that the bool must be true on state entry for a delayed event to be sent!")]
		public FsmBool activeBool;

		private DelayedEvent delayedEvent;

		public override void Reset()
		{
			eventTarget = null;
			sendEvent = null;
			delay = null;
			everyFrame = false;
			activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (!activeBool.Value || activeBool.IsNone)
			{
				Finish();
			}
			else if (delay.Value < 0.001f)
			{
				base.Fsm.Event(eventTarget, sendEvent.Value);
				if (!everyFrame)
				{
					Finish();
				}
			}
			else
			{
				delayedEvent = base.Fsm.DelayedEvent(eventTarget, FsmEvent.GetFsmEvent(sendEvent.Value), delay.Value);
			}
		}

		public override void OnUpdate()
		{
			if (!everyFrame)
			{
				if (DelayedEvent.WasSent(delayedEvent))
				{
					Finish();
				}
			}
			else
			{
				base.Fsm.Event(eventTarget, sendEvent.Value);
			}
		}
	}
}
