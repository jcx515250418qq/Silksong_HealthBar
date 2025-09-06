using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class EndGGBossScene : FsmStateAction
{
	public override void OnEnter()
	{
		if ((bool)BossSceneController.Instance)
		{
			BossSceneController.Instance.EndBossScene();
		}
		Finish();
	}
}
