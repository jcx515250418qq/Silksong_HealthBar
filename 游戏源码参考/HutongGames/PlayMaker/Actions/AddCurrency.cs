namespace HutongGames.PlayMaker.Actions
{
	public class AddCurrency : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		public FsmInt Amount;

		public override void Reset()
		{
			Target = null;
			CurrencyType = null;
			Amount = null;
		}

		public override void OnEnter()
		{
			if (!CurrencyType.IsNone && Amount.Value > 0)
			{
				CurrencyManager.AddCurrency(Amount.Value, (CurrencyType)(object)CurrencyType.Value);
			}
			Finish();
		}
	}
}
