using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIsBossRushMode : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => GameManager.instance.playerData.bossRushMode;
}
