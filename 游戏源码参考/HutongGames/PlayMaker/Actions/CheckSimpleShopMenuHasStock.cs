namespace HutongGames.PlayMaker.Actions
{
	public class CheckSimpleShopMenuHasStock : FSMUtility.CheckFsmStateAction
	{
		[CheckForComponent(typeof(SimpleShopMenuOwner))]
		public FsmOwnerDefault Target;

		public override bool IsTrue => Target.GetSafe(this).GetComponent<SimpleShopMenuOwner>().HasStockLeft();

		public override void Reset()
		{
			base.Reset();
			Target = null;
		}
	}
}
