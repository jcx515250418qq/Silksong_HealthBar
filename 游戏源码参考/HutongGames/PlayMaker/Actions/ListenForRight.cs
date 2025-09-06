using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForRight : FsmStateAction
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

		public bool ignoreIfLeftPressed;

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
			if (gm.isPaused || (ignoreIfLeftPressed && inputHandler.inputActions.Left.IsPressed) || (!activeBool.IsNone && !activeBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.Right.WasPressed)
			{
				base.Fsm.Event(wasPressed);
			}
			if (inputHandler.inputActions.Right.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.Right.IsPressed)
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
			if (!inputHandler.inputActions.Right.IsPressed)
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
