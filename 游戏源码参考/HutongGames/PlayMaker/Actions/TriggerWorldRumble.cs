namespace HutongGames.PlayMaker.Actions
{
	public class TriggerWorldRumble : FsmStateAction
	{
		public override void OnEnter()
		{
			WorldRumbleManager worldRumbleManager = GameCameras.instance.worldRumbleManager;
			if ((bool)worldRumbleManager)
			{
				worldRumbleManager.ForceRumble();
			}
			Finish();
		}
	}
}
