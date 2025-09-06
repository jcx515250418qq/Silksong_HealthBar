namespace HutongGames.PlayMaker.Actions
{
	public class GetShopObject : FSMUtility.GetComponentFsmStateAction<ShopOwnerBase>
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		public override void Reset()
		{
			base.Reset();
			StoreObject = null;
		}

		protected override void DoAction(ShopOwnerBase owner)
		{
			StoreObject.Value = owner.ShopObject;
		}
	}
}
