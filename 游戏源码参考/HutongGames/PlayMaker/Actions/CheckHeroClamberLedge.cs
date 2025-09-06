namespace HutongGames.PlayMaker.Actions
{
	public class CheckHeroClamberLedge : FSMUtility.CheckFsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreY;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreCollider;

		public override bool IsTrue
		{
			get
			{
				if (HeroController.instance.CheckClamberLedge(out var y, out var clamberedCollider))
				{
					StoreY.Value = y;
					StoreCollider.Value = (clamberedCollider ? clamberedCollider.gameObject : null);
					return true;
				}
				StoreY.Value = 0f;
				StoreCollider.Value = null;
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			StoreY = null;
			StoreCollider = null;
		}
	}
}
