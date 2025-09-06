namespace HutongGames.PlayMaker.Actions
{
	public class AddUsingSilk : FsmStateAction
	{
		public FsmInt Amount;

		[ObjectType(typeof(SilkSpool.SilkUsingFlags))]
		public FsmEnum UsingType;

		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;

		public override void Reset()
		{
			Amount = 1;
			UsingType = SilkSpool.SilkUsingFlags.Normal;
			DidAddTracker = null;
		}

		public override void OnEnter()
		{
			DidAddTracker.Value = SilkSpool.Instance.AddUsing((SilkSpool.SilkUsingFlags)(object)UsingType.Value, Amount.Value);
			Finish();
		}
	}
}
