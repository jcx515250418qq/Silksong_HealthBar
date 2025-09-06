using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CountShopItems : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(ShopItem.TypeFlags))]
		public FsmEnum MatchFlag;

		public FsmBool PurchasedOnly;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;

		public override void Reset()
		{
			Target = null;
			MatchFlag = null;
			PurchasedOnly = null;
			StoreCount = null;
		}

		public override void OnEnter()
		{
			StoreCount.Value = 0;
			DoAction();
			Finish();
		}

		private void DoAction()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			ShopOwnerBase component = safe.GetComponent<ShopOwnerBase>();
			IEnumerable<ShopItem> stock;
			if ((bool)component)
			{
				stock = component.Stock;
			}
			else
			{
				ShopMenuStock component2 = safe.GetComponent<ShopMenuStock>();
				if (!component2)
				{
					return;
				}
				stock = component2.EnumerateStock();
			}
			ShopItem.TypeFlags typeFlags = (ShopItem.TypeFlags)(object)MatchFlag.Value;
			int value = CountShopItemsFromStock(stock, typeFlags, PurchasedOnly.Value);
			StoreCount.Value = value;
		}

		public static int CountShopItemsFromStock(IEnumerable<ShopItem> stock, ShopItem.TypeFlags typeFlags, bool purchasedOnly)
		{
			if (stock == null)
			{
				return 0;
			}
			int num = 0;
			foreach (ShopItem item in stock)
			{
				if ((bool)item && (!purchasedOnly || item.IsPurchased) && (typeFlags == ShopItem.TypeFlags.None || (typeFlags & item.GetTypeFlags()) != 0))
				{
					num++;
				}
			}
			return num;
		}
	}
}
