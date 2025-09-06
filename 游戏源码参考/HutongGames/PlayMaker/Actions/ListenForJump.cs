namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForJump : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmBool queueBool;

		public FsmBool activeBool;

		public bool stateEntryOnly;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			eventTarget = null;
			activeBool = new FsmBool
			{
				UseVariable = true
			};
			queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			CheckInput();
			if (stateEntryOnly)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			CheckInput();
		}

		private void CheckInput()
		{
			if (gm.isPaused || (!activeBool.IsNone && !activeBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.Jump.WasPressed)
			{
				base.Fsm.Event(wasPressed);
				if (!queueBool.IsNone)
				{
					queueBool.Value = true;
				}
			}
			if (inputHandler.inputActions.Jump.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.Jump.IsPressed)
			{
				base.Fsm.Event(isPressed);
			}
			if (!inputHandler.inputActions.Jump.IsPressed)
			{
				base.Fsm.Event(isNotPressed);
				if (!queueBool.IsNone)
				{
					queueBool.Value = false;
				}
			}
		}
	}
}
