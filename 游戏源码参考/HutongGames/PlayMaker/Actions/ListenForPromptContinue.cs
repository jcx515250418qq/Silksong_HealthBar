namespace HutongGames.PlayMaker.Actions
{
	public class ListenForPromptContinue : FsmStateAction
	{
		public FsmEventTarget EventTarget;

		public FsmEvent ContinuePressed;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			EventTarget = null;
			ContinuePressed = null;
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
			if (!(gm == null) && !gm.isPaused && !(inputHandler == null) && inputHandler.WasSkipButtonPressed)
			{
				base.Fsm.Event(ContinuePressed);
			}
		}
	}
}
