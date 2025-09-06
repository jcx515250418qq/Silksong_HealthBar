namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public abstract class ListenForMenuButton : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		private bool wasPreviouslyPressed;

		private GameManager gm;

		private InputHandler inputHandler;

		protected abstract Platform.MenuActions MenuAction { get; }

		public override void Reset()
		{
			eventTarget = null;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			wasPreviouslyPressed = Platform.Current.GetMenuAction(inputHandler.inputActions, ignoreAttack: false, isContinuous: true) == Platform.MenuActions.Submit;
		}

		public override void OnUpdate()
		{
			if (gm == null || gm.isPaused || inputHandler == null)
			{
				return;
			}
			HeroActions inputActions = inputHandler.inputActions;
			Platform current = Platform.Current;
			Platform.MenuActions menuAction = MenuAction;
			if (current.GetMenuAction(inputActions, ignoreAttack: false, isContinuous: true) == menuAction)
			{
				if (current.GetMenuAction(inputActions) == menuAction)
				{
					base.Fsm.Event(eventTarget, wasPressed);
				}
				base.Fsm.Event(eventTarget, isPressed);
				wasPreviouslyPressed = true;
			}
			else
			{
				if (wasPreviouslyPressed)
				{
					base.Fsm.Event(eventTarget, wasReleased);
				}
				base.Fsm.Event(eventTarget, isNotPressed);
				wasPreviouslyPressed = false;
			}
		}
	}
}
