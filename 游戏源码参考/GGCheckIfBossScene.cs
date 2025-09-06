using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossScene : FsmStateAction
{
	public FsmEvent bossSceneEvent;

	public FsmEvent regularSceneEvent;

	public override void Reset()
	{
		bossSceneEvent = null;
		regularSceneEvent = null;
	}

	public override void OnEnter()
	{
		if (BossSceneController.IsBossScene)
		{
			base.Fsm.Event(bossSceneEvent);
		}
		else
		{
			base.Fsm.Event(regularSceneEvent);
		}
		Finish();
	}
}
