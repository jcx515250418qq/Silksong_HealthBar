using InControl;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashV3 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		public FsmBool IsActive;

		public FsmFloat DelayBeforeActive;

		public FsmEvent WasPressed;

		public FsmEvent WasReleased;

		public FsmEvent IsPressed;

		public FsmEvent IsNotPressed;

		[UIHint(UIHint.Variable)]
		public FsmBool SetIsPressed;

		private GameManager gm;

		private InputHandler inputHandler;

		private float timer;

		public override void Reset()
		{
			EventTarget = null;
			IsActive = true;
			DelayBeforeActive = null;
			WasPressed = null;
			WasReleased = null;
			IsPressed = null;
			IsNotPressed = null;
			SetIsPressed = null;
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
			SetIsPressed.Value = false;
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
				PlayerAction dash = inputHandler.inputActions.Dash;
				if (dash.WasPressed)
				{
					base.Fsm.Event(EventTarget, WasPressed);
				}
				if (dash.WasReleased)
				{
					base.Fsm.Event(EventTarget, WasReleased);
				}
				if (dash.IsPressed)
				{
					base.Fsm.Event(EventTarget, IsPressed);
				}
				if (!dash.IsPressed)
				{
					base.Fsm.Event(EventTarget, IsNotPressed);
				}
				SetIsPressed.Value = dash.IsPressed;
			}
		}
	}
}
