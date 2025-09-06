using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectableItemManager : ManagerSingleton<CollectableItemManager>
{
	private delegate void ItemAffectingDelegate(ref CollectableItemsData.Data data);

	[SerializeField]
	private CollectableItemList masterList;

	[SerializeField]
	private CollectableItem invalidTemplate;

	private CollectableItem collectedItem;

	private static bool previousHiddenMode;

	private static readonly ObjectCache<List<CollectableItem>> _collectedItemCache = new ObjectCache<List<CollectableItem>>();

	public static CollectableItem CollectedItem
	{
		get
		{
			return ManagerSingleton<CollectableItemManager>.Instance.collectedItem;
		}
		set
		{
			if (value != null)
			{
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.None;
			}
			ManagerSingleton<CollectableItemManager>.Instance.collectedItem = value;
		}
	}

	public static int Version { get; private set; }

	public static void IncrementVersion()
	{
		Version++;
	}

	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<CollectableItemManager>.UnsafeInstance == this)
		{
			Version++;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (ManagerSingleton<CollectableItemManager>.UnsafeInstance == this)
		{
			Version++;
		}
	}

	public static void AddItem(CollectableItem item, int amount = 1)
	{
		if ((bool)ManagerSingleton<CollectableItemManager>.Instance)
		{
			ManagerSingleton<CollectableItemManager>.Instance.InternalAddItem(item, amount);
		}
	}

	public static void RemoveItem(CollectableItem item, int amount = 1)
	{
		if ((bool)ManagerSingleton<CollectableItemManager>.Instance)
		{
			ManagerSingleton<CollectableItemManager>.Instance.InternalRemoveItem(item, amount);
		}
	}

	public static void ClearStatic()
	{
		_collectedItemCache.UpdateCache(null, -1);
	}

	public static List<CollectableItem> GetCollectedItems()
	{
		bool flag = IsInHiddenMode();
		if (!_collectedItemCache.ShouldUpdate(Version))
		{
			return _collectedItemCache.Value;
		}
		if (!ManagerSingleton<CollectableItemManager>.Instance)
		{
			_collectedItemCache.UpdateCache(new List<CollectableItem>(), Version);
		}
		else if (flag)
		{
			_collectedItemCache.UpdateCache(ManagerSingleton<CollectableItemManager>.Instance.InternalGetCollectedItems((CollectableItem item) => item.IsVisibleWithBareInventory || item.SaveData.AmountWhileHidden > 0), Version);
		}
		else
		{
			_collectedItemCache.UpdateCache(ManagerSingleton<CollectableItemManager>.Instance.InternalGetCollectedItems(null), Version);
		}
		return _collectedItemCache.Value;
	}

	public static bool IsInHiddenMode()
	{
		HeroController instance = HeroController.instance;
		bool flag = (bool)instance && (bool)instance.Config && instance.Config.ForceBareInventory;
		if (previousHiddenMode != flag)
		{
			previousHiddenMode = flag;
			Version++;
		}
		return flag;
	}

	public static void ApplyHiddenItems()
	{
		CollectableItemsData collectables = GameManager.instance.playerData.Collectables;
		foreach (string validName in collectables.GetValidNames())
		{
			CollectableItemsData.Data data = collectables.GetData(validName);
			if (data.AmountWhileHidden > 0)
			{
				data.Amount += data.AmountWhileHidden;
				data.AmountWhileHidden = 0;
				collectables.SetData(validName, data);
			}
		}
		IncrementVersion();
	}

	private void InternalAddItem(CollectableItem item, int amount)
	{
		if (!IsItemInMasterList(item) || amount <= 0)
		{
			return;
		}
		if (IsInHiddenMode())
		{
			AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
			{
				data.AmountWhileHidden += amount;
			});
		}
		else
		{
			AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
			{
				data.Amount += amount;
			});
		}
	}

	private void InternalRemoveItem(CollectableItem item, int amount)
	{
		if (!IsItemInMasterList(item) || amount <= 0)
		{
			return;
		}
		if (IsInHiddenMode())
		{
			AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
			{
				data.AmountWhileHidden -= amount;
				if (data.AmountWhileHidden < 0)
				{
					data.AmountWhileHidden = 0;
				}
			});
			return;
		}
		AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
		{
			data.Amount -= amount;
			if (data.Amount < 0)
			{
				data.Amount = 0;
			}
		});
	}

	private List<CollectableItem> InternalGetCollectedItems(Func<CollectableItem, bool> predicate)
	{
		if (!masterList)
		{
			return new List<CollectableItem>();
		}
		if (!Application.isPlaying)
		{
			return masterList.ToList();
		}
		List<CollectableItem> list = (from itemName in GameManager.instance.playerData.Collectables.GetValidNames()
			where !IsItemInMasterList(itemName)
			select itemName).Select(GetInvalidItem).ToList();
		ICollection<CollectableItem> collection;
		if (predicate == null)
		{
			collection = GetAllCollectables();
		}
		else
		{
			ICollection<CollectableItem> collection2 = GetAllCollectables().Where(predicate).ToList();
			collection = collection2;
		}
		ICollection<CollectableItem> collection3 = collection;
		List<CollectableItem> list2 = new List<CollectableItem>(collection3.Count + list.Count);
		list2.AddRange(collection3.Where((CollectableItem item) => item.IsVisible));
		list2.AddRange(list);
		foreach (CollectableItem item in list2)
		{
			item.ReportPreviouslyCollected();
		}
		return list2;
	}

	public ICollection<CollectableItem> GetAllCollectables()
	{
		return masterList;
	}

	public static CollectableItem GetItemByName(string itemName)
	{
		return ManagerSingleton<CollectableItemManager>.Instance.masterList.GetByName(itemName);
	}

	private void AffectItemData(string itemName, ItemAffectingDelegate affector)
	{
		CollectableItemsData collectables = GameManager.instance.playerData.Collectables;
		CollectableItemsData.Data data = collectables.GetData(itemName);
		affector(ref data);
		collectables.SetData(itemName, data);
		IncrementVersion();
	}

	private bool IsItemInMasterList(CollectableItem item)
	{
		return IsItemInMasterList(item.name);
	}

	private bool IsItemInMasterList(string itemName)
	{
		if (!masterList)
		{
			Debug.LogError("Collectable Item masterList is not assigned!");
			return false;
		}
		foreach (CollectableItem master in masterList)
		{
			if (master.name.Equals(itemName))
			{
				return true;
			}
		}
		Debug.LogError($"Collectable Item: {itemName} not found in master list!");
		return false;
	}

	private CollectableItem GetInvalidItem(string itemName)
	{
		CollectableItem obj = (invalidTemplate ? UnityEngine.Object.Instantiate(invalidTemplate) : ScriptableObject.CreateInstance<CollectableItemBasic>());
		obj.name = itemName;
		return obj;
	}
}
