using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickMapV2 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmBool queueBool;

		public FsmFloat DelayBeforeActive;

		public FsmBool IsActive;

		private GameManager gm;

		private InputHandler inputHandler;

		private float timer;

		public override void Reset()
		{
			eventTarget = null;
			wasPressed = null;
			wasReleased = null;
			isPressed = null;
			isNotPressed = null;
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
				if (inputHandler.inputActions.QuickMap.WasPressed)
				{
					base.Fsm.Event(wasPressed);
					if (!queueBool.IsNone)
					{
						queueBool.Value = true;
					}
				}
				if (inputHandler.inputActions.QuickMap.WasReleased)
				{
					base.Fsm.Event(wasReleased);
				}
				if (inputHandler.inputActions.QuickMap.IsPressed)
				{
					base.Fsm.Event(isPressed);
				}
				if (!inputHandler.inputActions.QuickMap.IsPressed)
				{
					base.Fsm.Event(isNotPressed);
					if (!queueBool.IsNone)
					{
						queueBool.Value = false;
					}
				}
			}
		}
	}
}
