using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryItemMateriumManager : InventoryItemListManager<InventoryItemMaterium, MateriumItem>
{
	[Space]
	[SerializeField]
	private SpriteRenderer displayIcon;

	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase component = GetComponent<InventoryPaneBase>();
		if ((bool)component)
		{
			component.OnPaneStart += MateriumItemManager.CheckAchievements;
		}
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if ((bool)displayIcon)
		{
			displayIcon.sprite = null;
		}
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		InventoryItemMaterium inventoryItemMaterium = selectable as InventoryItemMaterium;
		if (inventoryItemMaterium == null)
		{
			base.SetDisplay(selectable);
		}
		else if (inventoryItemMaterium.ItemData.IsCollected)
		{
			base.SetDisplay(selectable);
			if ((bool)displayIcon)
			{
				displayIcon.sprite = inventoryItemMaterium.Sprite;
			}
		}
		else
		{
			SetDisplay(selectable.gameObject);
		}
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemMaterium> selectableItems, List<MateriumItem> items)
	{
		for (int i = 0; i < selectableItems.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(value: true);
			selectableItems[i].ItemData = items[i];
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList()
			}
		};
	}

	protected override List<MateriumItem> GetItems()
	{
		return ManagerSingleton<MateriumItemManager>.Instance.MasterList.Where((MateriumItem record) => record.IsRequiredForCompletion || record.IsCollected).ToList();
	}
}
