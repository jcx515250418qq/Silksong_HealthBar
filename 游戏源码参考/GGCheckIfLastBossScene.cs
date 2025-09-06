using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfLastBossScene : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => BossSequenceController.BossIndex >= BossSequenceController.BossCount;
}
