namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDreamNail : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

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
			if (!gm.isPaused && (activeBool.Value || activeBool.IsNone) && !inputHandler.ForceDreamNailRePress)
			{
				if (inputHandler.inputActions.DreamNail.WasPressed)
				{
					base.Fsm.Event(wasPressed);
				}
				if (inputHandler.inputActions.DreamNail.WasReleased)
				{
					base.Fsm.Event(wasReleased);
				}
				if (inputHandler.inputActions.DreamNail.IsPressed)
				{
					base.Fsm.Event(isPressed);
				}
				if (!inputHandler.inputActions.DreamNail.IsPressed)
				{
					base.Fsm.Event(isNotPressed);
				}
			}
		}
	}
}
