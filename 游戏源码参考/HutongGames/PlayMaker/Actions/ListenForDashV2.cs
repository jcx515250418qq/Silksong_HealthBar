using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashV2 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		public FsmEvent WasPressed;

		public FsmEvent WasReleased;

		public FsmEvent IsPressed;

		public FsmEvent IsNotPressed;

		public FsmFloat DelayBeforeActive;

		public FsmBool IsActive;

		private GameManager gm;

		private InputHandler inputHandler;

		private float timer;

		public override void Reset()
		{
			EventTarget = null;
			WasPressed = null;
			WasReleased = null;
			IsPressed = null;
			IsNotPressed = null;
			DelayBeforeActive = null;
			IsActive = true;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			timer = DelayBeforeActive.Value;
			CheckInput();
		}

		public override void OnUpdate()
		{
			CheckInput();
		}

		private void CheckInput()
		{
			if (gm.isPaused)
			{
				return;
			}
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else if (IsActive.Value || IsActive.IsNone)
			{
				if (inputHandler.inputActions.Dash.WasPressed)
				{
					base.Fsm.Event(WasPressed);
				}
				if (inputHandler.inputActions.Dash.WasReleased)
				{
					base.Fsm.Event(WasReleased);
				}
				if (inputHandler.inputActions.Dash.IsPressed)
				{
					base.Fsm.Event(IsPressed);
				}
				if (!inputHandler.inputActions.Dash.IsPressed)
				{
					base.Fsm.Event(IsNotPressed);
				}
			}
		}
	}
}
