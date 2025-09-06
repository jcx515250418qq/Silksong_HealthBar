namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsAnyCurrencyCounterRolling : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => CurrencyCounterBase.IsAnyCounterRolling;
	}
}
