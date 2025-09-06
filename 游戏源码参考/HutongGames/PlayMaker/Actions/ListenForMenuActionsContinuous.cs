namespace HutongGames.PlayMaker.Actions
{
	public class ListenForMenuActionsContinuous : FsmStateAction
	{
		public FsmEventTarget EventTarget;

		public FsmEvent SubmitPressed;

		public FsmEvent CancelPressed;

		public FsmEvent NonePressed;

		public FsmBool IgnoreAttack;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			EventTarget = null;
			SubmitPressed = null;
			CancelPressed = null;
			NonePressed = null;
			IgnoreAttack = null;
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
				switch (Platform.Current.GetMenuAction(inputActions, IgnoreAttack.Value, isContinuous: true))
				{
				case Platform.MenuActions.Submit:
					base.Fsm.Event(EventTarget, SubmitPressed);
					break;
				case Platform.MenuActions.Cancel:
					base.Fsm.Event(EventTarget, CancelPressed);
					break;
				default:
					base.Fsm.Event(EventTarget, NonePressed);
					break;
				}
			}
		}
	}
}
