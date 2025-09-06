namespace HutongGames.PlayMaker.Actions
{
	public class ActivateWorldRumble : FsmStateAction
	{
		public FsmBool SetActive;

		public override void Reset()
		{
			SetActive = null;
		}

		public override void OnEnter()
		{
			WorldRumbleManager worldRumbleManager = GameCameras.instance.worldRumbleManager;
			if (SetActive.Value)
			{
				worldRumbleManager.AllowRumbles();
			}
			else
			{
				worldRumbleManager.PreventRumbles();
			}
			Finish();
		}
	}
}
