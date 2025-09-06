using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetShopItemPurchasedV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmInt SubItemIndex;

		[UIHint(UIHint.Variable)]
		public FsmBool IsWaitingBool;

		public override void Reset()
		{
			Target = null;
			SubItemIndex = null;
			IsWaitingBool = null;
		}

		public override void OnEnter()
		{
			IsWaitingBool.Value = false;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				ShopItemStats component = safe.GetComponent<ShopItemStats>();
				if (component != null)
				{
					IsWaitingBool.Value = true;
					component.SetPurchased(delegate
					{
						IsWaitingBool.Value = false;
						GameCameras.instance.HUDIn();
					}, SubItemIndex.Value);
				}
			}
			Finish();
		}
	}
}
