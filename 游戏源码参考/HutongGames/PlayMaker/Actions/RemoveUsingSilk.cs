namespace HutongGames.PlayMaker.Actions
{
	public class RemoveUsingSilk : FsmStateAction
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
			if (DidAddTracker.Value)
			{
				SilkSpool.Instance.RemoveUsing((SilkSpool.SilkUsingFlags)(object)UsingType.Value, Amount.Value);
				DidAddTracker.Value = false;
			}
			Finish();
		}
	}
}
