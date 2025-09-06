using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashReleased : FsmStateAction
	{
		public FsmBool storeDashWasReleased;

		public float timeBeforeActive;

		private float timer;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			storeDashWasReleased = null;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			storeDashWasReleased.Value = false;
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer >= timeBeforeActive)
			{
				CheckInput();
			}
			else
			{
				timer += Time.deltaTime;
			}
		}

		private void CheckInput()
		{
			if (!gm.isPaused && !inputHandler.inputActions.Dash.IsPressed)
			{
				storeDashWasReleased.Value = true;
			}
		}
	}
}
