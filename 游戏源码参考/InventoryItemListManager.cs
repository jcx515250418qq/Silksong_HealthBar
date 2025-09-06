using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InventoryItemListManager<TSelectable, TItem> : InventoryItemManager where TSelectable : InventoryItemSelectable
{
	public interface IMoveNextButton
	{
		bool WillSubmitMoveNext { get; }
	}

	protected delegate List<InventoryItemGrid.GridSection> GetGridSectionsDelegate(List<TSelectable> selectableItems, List<TItem> items);

	[SerializeField]
	[FormerlySerializedAs("questList")]
	private InventoryItemGrid itemList;

	[SerializeField]
	private TSelectable templateItem;

	private bool isSetup;

	protected InventoryItemGrid ItemList => itemList;

	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase component = GetComponent<InventoryPaneBase>();
		if ((bool)component)
		{
			component.OnPaneStart += UpdateList;
		}
		UpdateList();
	}

	[ContextMenu("Preview List")]
	public void UpdateList()
	{
		if ((bool)itemList)
		{
			List<TItem> items = GetItems();
			SetupGrid(itemList, templateItem, items, GetGridSections);
			OnItemListSetup();
		}
	}

	public override void InstantScroll()
	{
		if ((bool)base.CurrentSelected)
		{
			itemList.ScrollTo(base.CurrentSelected, isInstant: true);
		}
	}

	protected void SetupGrid(InventoryItemGrid grid, TSelectable currentTemplate, List<TItem> items, GetGridSectionsDelegate getGridSections)
	{
		List<TSelectable> list = new List<TSelectable>(grid.transform.childCount);
		foreach (Transform item2 in grid.transform)
		{
			TSelectable component = item2.GetComponent<TSelectable>();
			if ((bool)component)
			{
				list.Add(component);
			}
		}
		list.Remove(currentTemplate);
		currentTemplate.gameObject.SetActive(value: false);
		if (!Application.isPlaying || !isSetup)
		{
			isSetup = true;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				UnityEngine.Object.DestroyImmediate(list[num].gameObject);
			}
			list.Clear();
		}
		int i;
		for (i = items.Count - list.Count; i > 0; i--)
		{
			TSelectable item = UnityEngine.Object.Instantiate(currentTemplate, grid.transform);
			list.Add(item);
			OnItemInstantiated(item);
		}
		for (; i < 0; i++)
		{
			TSelectable val = list.Last();
			val.gameObject.SetActive(value: false);
			list.Remove(val);
		}
		grid.Setup(getGridSections(list, items));
	}

	protected List<TSelectable> GetSelectables(Func<TSelectable, bool> predicate)
	{
		if (!itemList)
		{
			return null;
		}
		return itemList.GetListItems(predicate);
	}

	protected abstract List<TItem> GetItems();

	protected abstract List<InventoryItemGrid.GridSection> GetGridSections(List<TSelectable> selectableItems, List<TItem> items);

	protected virtual void OnItemInstantiated(TSelectable item)
	{
	}

	protected virtual void OnItemListSetup()
	{
	}

	public override bool SubmitButtonSelected()
	{
		if (base.IsActionsBlocked)
		{
			return false;
		}
		InventoryItemSelectable currentSelected = base.CurrentSelected;
		if (!(currentSelected is IMoveNextButton { WillSubmitMoveNext: not false }))
		{
			return base.SubmitButtonSelected();
		}
		InventoryItemSelectable nextSelectable = currentSelected.GetNextSelectable(SelectionDirection.Right);
		if (nextSelectable is IMoveNextButton && nextSelectable.GetNextSelectable(SelectionDirection.Down) != currentSelected)
		{
			SetSelected(nextSelectable, null);
			PlayMoveSound();
			return true;
		}
		InventoryItemSelectable inventoryItemSelectable = currentSelected;
		for (int i = 0; i < 10; i++)
		{
			if (!(inventoryItemSelectable.GetNextSelectable(SelectionDirection.Left) is IMoveNextButton moveNextButton2))
			{
				break;
			}
			if (!(moveNextButton2 is InventoryItemSelectable inventoryItemSelectable2))
			{
				break;
			}
			inventoryItemSelectable = inventoryItemSelectable2;
		}
		if (inventoryItemSelectable.GetNextSelectable(SelectionDirection.Down) is IMoveNextButton moveNextButton3 && moveNextButton3 is InventoryItemSelectable selectable)
		{
			SetSelected(selectable, null);
			PlayMoveSound();
			return true;
		}
		TSelectable val = GetSelectables(null).FirstOrDefault();
		if ((bool)val && val != currentSelected)
		{
			SetSelected(val, null);
			PlayMoveSound();
			return true;
		}
		return base.SubmitButtonSelected();
	}
}
