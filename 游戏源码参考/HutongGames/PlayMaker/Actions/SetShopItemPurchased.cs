using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetShopItemPurchased : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmBool IsWaitingBool;

		public override void Reset()
		{
			Target = null;
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
					}, 0);
				}
			}
			Finish();
		}
	}
}
