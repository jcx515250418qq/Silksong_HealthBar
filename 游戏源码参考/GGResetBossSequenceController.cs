using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGResetBossSequenceController : FsmStateAction
{
	public override void OnEnter()
	{
		BossSequenceController.Reset();
		Finish();
	}
}
