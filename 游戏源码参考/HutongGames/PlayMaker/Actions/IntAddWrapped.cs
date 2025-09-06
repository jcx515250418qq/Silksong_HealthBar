namespace HutongGames.PlayMaker.Actions
{
	public class IntAddWrapped : FsmStateAction
	{
		public FsmInt Value;

		public FsmInt AddValue;

		public FsmInt Min;

		public FsmInt Max;

		public FsmBool InclusiveMax;

		public FsmInt StoreResult;

		public override void Reset()
		{
			Value = null;
			AddValue = null;
			Min = null;
			Max = null;
			InclusiveMax = null;
			StoreResult = null;
		}

		public override void OnEnter()
		{
			int value = Value.Value;
			int value2 = AddValue.Value;
			value += value2;
			int value3 = Min.Value;
			int num = Max.Value;
			if (InclusiveMax.Value)
			{
				num++;
			}
			value = ((value >= value3) ? (value3 + (value - value3) % (num - value3)) : (num - (value3 - value) % (num - value3)));
			StoreResult.Value = value;
			Finish();
		}
	}
}
