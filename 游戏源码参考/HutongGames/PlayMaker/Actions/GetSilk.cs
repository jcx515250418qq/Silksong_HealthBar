namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetSilk : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;

		public override void Reset()
		{
			StoreAmount = null;
		}

		public override void OnEnter()
		{
			StoreAmount.Value = PlayerData.instance.silk;
			Finish();
		}
	}
}
