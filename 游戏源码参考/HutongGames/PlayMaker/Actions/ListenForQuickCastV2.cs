using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickCastV2 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		public FsmEvent WasPressed;

		public FsmEvent WasReleased;

		public FsmEvent IsPressed;

		public FsmEvent IsNotPressed;

		public FsmBool queueBool;

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
			queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			timer = DelayBeforeActive.Value;
		}

		public override void OnUpdate()
		{
			if (gm.isPaused)
			{
				return;
			}
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				if (!IsActive.Value && !IsActive.IsNone)
				{
					return;
				}
				if (inputHandler.inputActions.QuickCast.WasPressed)
				{
					base.Fsm.Event(WasPressed);
					if (!queueBool.IsNone)
					{
						queueBool.Value = true;
					}
				}
				if (inputHandler.inputActions.QuickCast.WasReleased)
				{
					base.Fsm.Event(WasReleased);
				}
				if (inputHandler.inputActions.QuickCast.IsPressed)
				{
					base.Fsm.Event(IsPressed);
				}
				if (!inputHandler.inputActions.QuickCast.IsPressed)
				{
					base.Fsm.Event(IsNotPressed);
					if (!queueBool.IsNone)
					{
						queueBool.Value = false;
					}
				}
			}
		}
	}
}
