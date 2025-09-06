public class CheckIsFirstLevelForPlayer : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => GameManager.instance.IsFirstLevelForPlayer;
}
