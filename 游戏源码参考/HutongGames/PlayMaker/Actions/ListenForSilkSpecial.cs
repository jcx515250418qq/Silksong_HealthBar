using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSilkSpecial : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmBool isPressedBool;

		public FsmFloat pressedTimer;

		public FsmBool activeBool;

		public bool stateEntryOnly;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			eventTarget = null;
			isPressedBool = new FsmBool
			{
				UseVariable = true
			};
			pressedTimer = new FsmFloat
			{
				UseVariable = true
			};
			activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			CheckForInput();
			if (stateEntryOnly)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			CheckForInput();
		}

		public void CheckForInput()
		{
			if (gm.isPaused || (!activeBool.IsNone && !activeBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.QuickCast.WasPressed)
			{
				base.Fsm.Event(wasPressed);
			}
			if (inputHandler.inputActions.QuickCast.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.QuickCast.IsPressed)
			{
				base.Fsm.Event(isPressed);
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = true;
				}
				if (!pressedTimer.IsNone)
				{
					pressedTimer.Value += Time.deltaTime;
				}
			}
			if (!inputHandler.inputActions.QuickCast.IsPressed)
			{
				base.Fsm.Event(isNotPressed);
				if (!isPressedBool.IsNone)
				{
					isPressedBool.Value = false;
				}
				if (!pressedTimer.IsNone && pressedTimer.Value != 0f)
				{
					pressedTimer.Value = 0f;
				}
			}
		}
	}
}
