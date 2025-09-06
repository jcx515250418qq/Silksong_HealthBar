using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundNail : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => BossSequenceController.BoundNail;
}
