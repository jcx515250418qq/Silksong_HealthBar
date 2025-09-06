using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossSequence : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => BossSequenceController.IsInSequence;
}
