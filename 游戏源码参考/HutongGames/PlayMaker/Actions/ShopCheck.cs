using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ShopCheck : FSMUtility.CheckFsmStateAction
	{
		public FsmOwnerDefault Target;

		public override bool IsTrue
		{
			get
			{
				GameObject gameObject = Target.GetSafe(this);
				if (!gameObject)
				{
					return false;
				}
				ShopOwnerBase component = gameObject.GetComponent<ShopOwnerBase>();
				if ((bool)component)
				{
					gameObject = component.ShopObject;
				}
				ShopMenuStock component2 = gameObject.GetComponent<ShopMenuStock>();
				if (!component2)
				{
					return false;
				}
				return CheckShop(component2);
			}
		}

		public override void Reset()
		{
			base.Reset();
			Target = null;
		}

		protected abstract bool CheckShop(ShopMenuStock shop);
	}
}
