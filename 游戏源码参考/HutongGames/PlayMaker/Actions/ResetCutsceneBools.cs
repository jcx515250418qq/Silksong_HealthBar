namespace HutongGames.PlayMaker.Actions
{
	public class ResetCutsceneBools : FsmStateAction
	{
		public override void OnEnter()
		{
			PlayerData.instance.ResetCutsceneBools();
			Finish();
		}
	}
}
