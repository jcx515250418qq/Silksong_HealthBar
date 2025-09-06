using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForJumpV2 : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmBool isPressedBool;

		public FsmBool queueBool;

		public FsmBool activeBool;

		public bool stateEntryOnly;

		public float delayBeforeActive;

		private GameManager gm;

		private InputHandler inputHandler;

		private float delayTimer;

		public override void Reset()
		{
			eventTarget = null;
			activeBool = new FsmBool
			{
				UseVariable = true
			};
			queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			delayTimer = delayBeforeActive;
			if (delayBeforeActive == 0f)
			{
				CheckInput();
			}
			if (stateEntryOnly)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (delayTimer > 0f)
			{
				delayTimer -= Time.deltaTime;
			}
			else
			{
				CheckInput();
			}
		}

		private void CheckInput()
		{
			if (gm.isPaused || (!activeBool.IsNone && !activeBool.Value))
			{
				return;
			}
			if (inputHandler.inputActions.Jump.WasPressed)
			{
				base.Fsm.Event(wasPressed);
				if (!queueBool.IsNone)
				{
					queueBool.Value = true;
				}
			}
			if (inputHandler.inputActions.Jump.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.Jump.IsPressed)
			{
				base.Fsm.Event(isPressed);
				isPressedBool.Value = true;
			}
			if (!inputHandler.inputActions.Jump.IsPressed)
			{
				base.Fsm.Event(isNotPressed);
				isPressedBool.Value = false;
				if (!queueBool.IsNone)
				{
					queueBool.Value = false;
				}
			}
		}
	}
}
