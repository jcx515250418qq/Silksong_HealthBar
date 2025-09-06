namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetSilkV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;

		public bool EveryFrame;

		private PlayerData playerData;

		public override void Reset()
		{
			StoreAmount = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			playerData = PlayerData.instance;
			StoreAmount.Value = playerData.silk;
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			StoreAmount.Value = playerData.silk;
		}
	}
}
