namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSuperdashV3 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget Target;

		public FsmEvent WasPressed;

		public FsmEvent WasReleased;

		public FsmEvent IsPressed;

		public FsmEvent IsNotPressed;

		public FsmBool isPressedBool;

		public FsmBool ActiveBool;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			Target = null;
			WasPressed = null;
			WasReleased = null;
			IsPressed = null;
			IsNotPressed = null;
			ActiveBool = new FsmBool
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
			if (gm.isPaused || (!ActiveBool.IsNone && !ActiveBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.SuperDash.WasPressed)
			{
				base.Fsm.Event(WasPressed);
			}
			if (inputHandler.inputActions.SuperDash.WasReleased)
			{
				base.Fsm.Event(WasReleased);
			}
			if (inputHandler.inputActions.SuperDash.IsPressed)
			{
				base.Fsm.Event(IsPressed);
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = true;
				}
			}
			if (!inputHandler.inputActions.SuperDash.IsPressed)
			{
				base.Fsm.Event(IsNotPressed);
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = false;
				}
			}
		}
	}
}
