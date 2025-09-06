namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for menu actions, and safely disambiguates jump/submit/attack/cancel/cast.")]
	public class ListenForMenuActions : FsmStateAction
	{
		public FsmEventTarget eventTarget;

		public FsmEvent submitPressed;

		public FsmEvent cancelPressed;

		public FsmBool ignoreAttack;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			eventTarget = null;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			if (gm == null)
			{
				LogError("Cannot listen for buttons without game manager.");
				return;
			}
			inputHandler = gm.inputHandler;
			if (inputHandler == null)
			{
				LogError("Cannot listen for buttons without input handler.");
			}
		}

		public override void OnUpdate()
		{
			if (!(gm == null) && !gm.isPaused && !(inputHandler == null))
			{
				HeroActions inputActions = inputHandler.inputActions;
				switch (Platform.Current.GetMenuAction(inputActions, ignoreAttack.Value))
				{
				case Platform.MenuActions.Submit:
					base.Fsm.Event(eventTarget, submitPressed);
					break;
				case Platform.MenuActions.Cancel:
					base.Fsm.Event(eventTarget, cancelPressed);
					break;
				}
			}
		}
	}
}
