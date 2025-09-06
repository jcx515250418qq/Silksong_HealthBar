namespace HutongGames.PlayMaker.Actions
{
	public class CurrencyCounterMethod : FsmStateAction
	{
		public enum Methods
		{
			ToZero = 0,
			Take = 1,
			Add = 2,
			Show = 3,
			Hide = 4
		}

		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		[ObjectType(typeof(Methods))]
		public FsmEnum Method;

		[HideIf("NotRequiresAmount")]
		public FsmInt Amount;

		public bool NotRequiresAmount()
		{
			if (Method.IsNone)
			{
				return true;
			}
			Methods methods = (Methods)(object)Method.Value;
			if ((uint)(methods - 1) <= 1u)
			{
				return false;
			}
			return true;
		}

		public override void Reset()
		{
			CurrencyType = null;
			Method = Methods.ToZero;
			Amount = null;
		}

		public override void OnEnter()
		{
			if (!CurrencyType.IsNone && !Method.IsNone)
			{
				Methods methods = (Methods)(object)Method.Value;
				CurrencyType type = (CurrencyType)(object)CurrencyType.Value;
				switch (methods)
				{
				case Methods.ToZero:
					CurrencyCounter.ToZero(type);
					break;
				case Methods.Take:
					CurrencyCounter.Take(Amount.Value, type);
					break;
				case Methods.Add:
					CurrencyCounter.Add(Amount.Value, type);
					break;
				case Methods.Show:
					CurrencyCounter.Show(type);
					break;
				case Methods.Hide:
					CurrencyCounter.Hide(type);
					break;
				}
			}
			Finish();
		}
	}
}
