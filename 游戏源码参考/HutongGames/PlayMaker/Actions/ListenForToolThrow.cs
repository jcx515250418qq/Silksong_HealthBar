namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForToolThrow : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		public FsmEvent WasPressed;

		public FsmEvent WasReleased;

		public FsmEvent IsPressed;

		public FsmEvent IsNotPressed;

		public FsmBool RequireToolToThrow;

		public FsmBool IsActive;

		private HeroController hc;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			EventTarget = null;
			WasPressed = null;
			WasReleased = null;
			IsPressed = null;
			IsNotPressed = null;
			RequireToolToThrow = true;
			IsActive = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			hc = HeroController.instance;
		}

		public override void OnUpdate()
		{
			if (!hc.IsPaused() && (IsActive.IsNone || IsActive.Value))
			{
				if (inputHandler.inputActions.QuickCast.WasPressed && CanDo(reportFailure: true))
				{
					base.Fsm.Event(WasPressed);
				}
				if (inputHandler.inputActions.QuickCast.WasReleased)
				{
					base.Fsm.Event(WasReleased);
				}
				if (inputHandler.inputActions.QuickCast.IsPressed && CanDo(reportFailure: false))
				{
					base.Fsm.Event(IsPressed);
				}
				if (!inputHandler.inputActions.QuickCast.IsPressed)
				{
					base.Fsm.Event(IsNotPressed);
				}
			}
		}

		private bool CanDo(bool reportFailure)
		{
			if (RequireToolToThrow.Value)
			{
				return HeroController.instance.GetWillThrowTool(reportFailure);
			}
			return true;
		}
	}
}
