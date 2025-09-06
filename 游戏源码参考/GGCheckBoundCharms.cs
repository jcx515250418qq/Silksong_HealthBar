using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundCharms : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => BossSequenceController.BoundCharms;
}
