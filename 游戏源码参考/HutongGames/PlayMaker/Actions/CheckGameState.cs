using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class CheckGameState : FsmStateAction
	{
		public FsmEvent playing;

		public FsmEvent otherwise;

		public bool everyFrame;

		public override void Reset()
		{
			playing = null;
			otherwise = null;
		}

		public override void OnEnter()
		{
			Check();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void Check()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				base.Fsm.Event(otherwise);
				LogError("Cannot CheckGameState() before the game manager is loaded.");
			}
			else if (unsafeInstance.GameState == GameState.PLAYING)
			{
				base.Fsm.Event(playing);
			}
			else
			{
				base.Fsm.Event(otherwise);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			Check();
		}
	}
}
