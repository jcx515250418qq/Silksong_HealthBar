using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForTaunt : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		public FsmEvent wasPressed;

		public FsmEvent wasReleased;

		public FsmEvent isPressed;

		public FsmEvent isNotPressed;

		public FsmFloat delayBeforeActive;

		private GameManager gm;

		private InputHandler inputHandler;

		private float timer;

		public override void Reset()
		{
			eventTarget = null;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			timer = delayBeforeActive.Value;
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
				return;
			}
			if (inputHandler.inputActions.Taunt.WasPressed)
			{
				base.Fsm.Event(wasPressed);
			}
			if (inputHandler.inputActions.Taunt.WasReleased)
			{
				base.Fsm.Event(wasReleased);
			}
			if (inputHandler.inputActions.Taunt.IsPressed)
			{
				base.Fsm.Event(isPressed);
			}
			if (!inputHandler.inputActions.Taunt.IsPressed)
			{
				base.Fsm.Event(isNotPressed);
			}
		}
	}
}
