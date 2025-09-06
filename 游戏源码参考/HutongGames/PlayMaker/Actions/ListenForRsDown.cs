namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForRsDown : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmBool isPressedBool;

		public FsmBool activeBool;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			eventTarget = null;
			wasPressed = null;
			wasReleased = null;
			isPressed = null;
			isNotPressed = null;
			isPressedBool = new FsmBool
			{
				UseVariable = true
			};
			activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
		}

		public override void OnUpdate()
		{
			if (gm.isPaused || (!activeBool.IsNone && !activeBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.RsDown.WasPressed)
			{
				base.Fsm.Event(wasPressed);
			}
			if (inputHandler.inputActions.RsDown.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.RsDown.IsPressed)
			{
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = true;
				}
				base.Fsm.Event(isPressed);
			}
			if (!inputHandler.inputActions.RsDown.IsPressed)
			{
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = false;
				}
				base.Fsm.Event(isNotPressed);
			}
		}
	}
}
