namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	public class QueueJump : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmBool queueBool;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			queueBool = null;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			queueBool.Value = false;
			CheckInput();
		}

		public override void OnUpdate()
		{
			CheckInput();
		}

		private void CheckInput()
		{
			if (!gm.isPaused)
			{
				if (inputHandler.inputActions.Jump.WasPressed)
				{
					queueBool.Value = true;
				}
				if (!inputHandler.inputActions.Jump.IsPressed)
				{
					queueBool.Value = false;
				}
			}
		}
	}
}
