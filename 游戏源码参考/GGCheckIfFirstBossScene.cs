using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfFirstBossScene : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => BossSequenceController.BossIndex < 1;
}
