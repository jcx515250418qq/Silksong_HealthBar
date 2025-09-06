using System.Collections.Generic;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryItemCollectableManager : InventoryItemListManager<InventoryItemCollectable, CollectableItem>
{
	[SerializeField]
	private Transform consumableHeader;

	[SerializeField]
	private Transform relicHeader;

	[SerializeField]
	private NestedFadeGroupBase memoryUseMsg;

	[SerializeField]
	private float msgFadeInTime;

	[SerializeField]
	private float msgFadeOutTime;

	[SerializeField]
	private InventoryCollectableItemSelectionHelper selectableHelper;

	private double hideEquipMessageAllowedTime;

	private InventoryPane pane;

	private InventoryPaneList paneList;

	public bool ShowingMemoryUseMsg { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		pane = GetComponent<InventoryPane>();
		paneList = GetComponentInParent<InventoryPaneList>();
		if ((bool)pane)
		{
			pane.OnPaneEnd += HideMemoryUseMsgInstant;
		}
		HideMemoryUseMsgInstant();
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemCollectable> selectableItems, List<CollectableItem> items)
	{
		for (int i = 0; i < selectableItems.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(value: true);
			selectableItems[i].Item = items[i];
		}
		if ((bool)relicHeader)
		{
			relicHeader.gameObject.SetActive(value: false);
		}
		if ((bool)consumableHeader)
		{
			consumableHeader.gameObject.SetActive(value: false);
		}
		return (from item in selectableItems
			group item by new
			{
				IsConsumable = item.Item.IsConsumable()
			} into @group
			orderby @group.Key.IsConsumable
			select new InventoryItemGrid.GridSection
			{
				Items = @group.Cast<InventoryItemSelectableDirectional>().ToList(),
				Header = (@group.Key.IsConsumable ? consumableHeader : null),
				HideHeaderIfNoneBefore = true
			}).ToList();
	}

	protected override List<CollectableItem> GetItems()
	{
		return CollectableItemManager.GetCollectedItems();
	}

	protected override InventoryItemSelectable GetStartSelectable()
	{
		if (selectableHelper != null && selectableHelper.TryGetSelectable(out var selectable))
		{
			CollectableItemManager.CollectedItem = null;
			return selectable;
		}
		InventoryItemCollectable inventoryItemCollectable = GetSelectables(null).FirstOrDefault((InventoryItemCollectable item) => CollectableItemManager.CollectedItem == item.Item);
		CollectableItemManager.CollectedItem = null;
		if ((bool)inventoryItemCollectable)
		{
			return inventoryItemCollectable;
		}
		return base.GetStartSelectable();
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		HideMemoryUseMsg(force: true);
	}

	public void ShowMemoryUseMsg()
	{
		if ((bool)memoryUseMsg && !ShowingMemoryUseMsg)
		{
			memoryUseMsg.AlphaSelf = 0f;
			memoryUseMsg.gameObject.SetActive(value: true);
			memoryUseMsg.FadeTo(1f, msgFadeInTime, null, isRealtime: true);
			ShowingMemoryUseMsg = true;
			paneList.InSubMenu = true;
			hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)msgFadeInTime;
		}
	}

	public void HideMemoryUseMsg(bool force = false)
	{
		if ((bool)memoryUseMsg && ShowingMemoryUseMsg && (force || !(Time.unscaledTimeAsDouble < hideEquipMessageAllowedTime)))
		{
			memoryUseMsg.FadeTo(0f, msgFadeOutTime, null, isRealtime: true);
			ShowingMemoryUseMsg = false;
			paneList.InSubMenu = false;
		}
	}

	public void HideMemoryUseMsgInstant()
	{
		if ((bool)memoryUseMsg && ShowingMemoryUseMsg)
		{
			memoryUseMsg.AlphaSelf = 0f;
			memoryUseMsg.gameObject.SetActive(value: false);
			ShowingMemoryUseMsg = false;
			paneList.InSubMenu = false;
		}
	}
}
