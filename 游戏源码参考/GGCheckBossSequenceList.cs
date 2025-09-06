using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBossSequenceList : FSMUtility.CheckFsmStateAction
{
	[ObjectType(typeof(BossSequence))]
	public FsmObject tierList;

	public override bool IsTrue => BossSequenceController.CheckIfSequence((BossSequence)tierList.Value);

	public override void Reset()
	{
		tierList = null;
		base.Reset();
	}
}
