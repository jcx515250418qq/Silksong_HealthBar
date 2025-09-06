using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

public abstract class SimpleShopMenuOwner : MonoBehaviour
{
	[SerializeField]
	private SimpleShopMenu shopMenuPrefab;

	[SerializeField]
	private LocalisedString shopTitle;

	[SerializeField]
	private LocalisedString purchaseText = new LocalisedString("UI", "CTRL_BUY");

	private static SimpleShopMenu _spawnedMenu;

	public string ShopTitle => shopTitle;

	public string PurchaseText => purchaseText;

	public virtual bool ClosePaneOnPurchase => true;

	public event Action ClosedNoPurchase;

	public event Action ClosedPurchase;

	protected virtual void Start()
	{
		if (!_spawnedMenu && (bool)shopMenuPrefab)
		{
			_spawnedMenu = UnityEngine.Object.Instantiate(shopMenuPrefab);
		}
	}

	public bool OpenMenu()
	{
		List<ISimpleShopItem> items = GetItems();
		if (items == null || items.Count == 0)
		{
			return false;
		}
		if ((bool)_spawnedMenu)
		{
			_spawnedMenu.SetStock(this, items);
			_spawnedMenu.Activate();
			return true;
		}
		return false;
	}

	public void RefreshStock()
	{
		_spawnedMenu.SetStock(this, GetItems());
	}

	public bool HasStockLeft()
	{
		List<ISimpleShopItem> items = GetItems();
		if (items != null)
		{
			return items.Count > 0;
		}
		return false;
	}

	public void ClosedMenu(bool didPurchase, int purchaseIndex)
	{
		if (didPurchase)
		{
			if (purchaseIndex >= 0)
			{
				OnPurchasedItem(purchaseIndex);
			}
			this.ClosedPurchase?.Invoke();
		}
		else
		{
			this.ClosedNoPurchase?.Invoke();
		}
	}

	public void PurchaseNoClose(int purchaseIndex)
	{
		if (purchaseIndex >= 0)
		{
			OnPurchasedItem(purchaseIndex);
		}
	}

	protected abstract List<ISimpleShopItem> GetItems();

	protected abstract void OnPurchasedItem(int itemIndex);
}
