namespace HutongGames.PlayMaker.Actions
{
	public class TakeCurrency : FsmStateAction
	{
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		public FsmInt Amount;

		public override void Reset()
		{
			CurrencyType = null;
			Amount = null;
		}

		public override void OnEnter()
		{
			if (!CurrencyType.IsNone && Amount.Value > 0)
			{
				CurrencyManager.TakeCurrency(Amount.Value, (CurrencyType)(object)CurrencyType.Value);
			}
			Finish();
		}
	}
}
