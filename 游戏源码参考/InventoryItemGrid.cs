using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class InventoryItemGrid : InventoryItemSelectable, IInventorySelectionParent
{
	[Serializable]
	private class SelectableList
	{
		public InventoryItemSelectable[] Selectables;
	}

	public class GridSection
	{
		public Transform Header;

		public bool HideHeaderIfNoneBefore;

		public List<InventoryItemSelectableDirectional> Items = new List<InventoryItemSelectableDirectional>();
	}

	public enum ListSetupTypes
	{
		Auto = 0,
		[UsedImplicitly]
		Custom = 1
	}

	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private SelectableList[] selectables;

	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private SelectableList[] nextPages;

	[Space]
	public int RowSplit = 4;

	public Vector2 GridOffset;

	public Vector2 ItemOffset;

	[SerializeField]
	private Transform attachToBottom;

	[SerializeField]
	private Vector2 bottomOffset;

	[Space]
	public float SectionPadding = 2.5f;

	public float SectionHeaderHeight = 1f;

	[Space]
	[SerializeField]
	private ListSetupTypes listSetupType;

	[SerializeField]
	private ScrollView scrollView;

	private List<GridSection> collections;

	private bool hasDoneInitialScroll;

	private InventoryItemManager itemManager;

	private InventoryItemSelectable queuedScroll;

	public ListSetupTypes ListSetupType => listSetupType;

	public override string DisplayName => string.Empty;

	public override string Description => string.Empty;

	protected void Awake()
	{
		itemManager = GetComponentInParent<InventoryItemManager>();
		for (int i = 0; i < selectables.Length; i++)
		{
			SelectableList obj = selectables[i];
			obj.Selectables = obj.Selectables.Where((InventoryItemSelectable o) => o != null).ToArray();
		}
	}

	protected void Start()
	{
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneStart += Setup;
		}
		OnValidate();
		List<GridSection> list = collections;
		if (list != null && list.Count > 0 && collections[0].Items.Count > 0)
		{
			ScrollEventHandler(collections[0].Items[0]);
		}
	}

	protected void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref selectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize(ref nextPages, typeof(InventoryItemManager.SelectionDirection));
		Setup();
	}

	[ContextMenu("Refresh Preview")]
	public void Setup()
	{
		if (listSetupType == ListSetupTypes.Auto)
		{
			List<InventoryItemSelectableDirectional> items = new List<InventoryItemSelectableDirectional>(GetComponentsInChildren<InventoryItemSelectableDirectional>()).Where((InventoryItemSelectableDirectional item) => item.gameObject.activeSelf).ToList();
			List<GridSection> newCollections = new List<GridSection>
			{
				new GridSection
				{
					Header = null,
					Items = items
				}
			};
			Setup(newCollections);
		}
	}

	public void Setup(List<GridSection> newCollections)
	{
		for (int i = 0; i < newCollections.Count; i++)
		{
			GridSection gridSection = newCollections[i];
			if (!gridSection.Header)
			{
				continue;
			}
			bool flag = i > 0 && newCollections[i - 1].Items.Count > 0;
			bool flag2 = false;
			foreach (InventoryItemSelectableDirectional item in gridSection.Items)
			{
				if (item.gameObject.activeSelf)
				{
					flag2 = true;
					break;
				}
			}
			bool active = flag2 && (!gridSection.HideHeaderIfNoneBefore || flag);
			gridSection.Header.gameObject.SetActive(active);
		}
		SetupScrollEvents(subscribe: false);
		collections = (newCollections = newCollections.Where((GridSection collection) => collection.Items.Count > 0).ToList());
		SetupScrollEvents(subscribe: true);
		for (int j = 0; j < newCollections.Count; j++)
		{
			GridSection gridSection2 = newCollections[j];
			GridSection gridSection3 = ((j > 0) ? newCollections[j - 1] : null);
			GridSection gridSection4 = ((j < newCollections.Count - 1) ? newCollections[j + 1] : null);
			List<InventoryItemSelectableDirectional> upOverrides = null;
			List<InventoryItemSelectableDirectional> downOverrides = null;
			if (gridSection3 != null)
			{
				int num = gridSection3.Items.Count % RowSplit;
				if (num == 0)
				{
					num = RowSplit;
				}
				upOverrides = gridSection3.Items.Skip(gridSection3.Items.Count - num).ToList();
			}
			if (gridSection4 != null)
			{
				downOverrides = gridSection4.Items.Take(RowSplit).ToList();
			}
			LinkGridSelectables(gridSection2.Items, downOverrides, upOverrides);
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional = gridSection3?.Items.LastOrDefault();
			float num2 = ((inventoryItemSelectableDirectional != null) ? (inventoryItemSelectableDirectional.transform.localPosition.y - GridOffset.y + ItemOffset.y) : 0f);
			PositionGridItems(gridSection2.Items, num2 - SectionPadding);
			if ((bool)newCollections[j].Header)
			{
				newCollections[j].Header.SetLocalPositionY(newCollections[j].Items[0].transform.localPosition.y + SectionHeaderHeight);
			}
			for (int k = 0; k < gridSection2.Items.Count; k++)
			{
				InventoryItemSelectableDirectional inventoryItemSelectableDirectional2 = gridSection2.Items[k];
				inventoryItemSelectableDirectional2.Grid = this;
				inventoryItemSelectableDirectional2.GridSectionIndex = j;
				inventoryItemSelectableDirectional2.GridItemIndex = k;
			}
		}
		if ((bool)scrollView)
		{
			scrollView.FullUpdate();
			if (!hasDoneInitialScroll && !newCollections.IsNullOrEmpty() && !newCollections[0].Items.IsNullOrEmpty())
			{
				if (queuedScroll != null)
				{
					ScrollTo(queuedScroll, isInstant: true);
				}
				else
				{
					ScrollTo(newCollections[0].Items[0].transform.position, isInstant: true);
				}
				hasDoneInitialScroll = true;
			}
			else if (queuedScroll != null)
			{
				ScrollTo(queuedScroll, isInstant: true);
			}
		}
		if (!attachToBottom)
		{
			return;
		}
		attachToBottom.SetPosition2D(base.transform.position);
		if (newCollections.Count != 0)
		{
			List<GridSection> list = newCollections;
			GridSection gridSection5 = list[list.Count - 1];
			if (gridSection5.Items.Count != 0)
			{
				List<InventoryItemSelectableDirectional> items = gridSection5.Items;
				InventoryItemSelectableDirectional inventoryItemSelectableDirectional3 = items[items.Count - 1];
				Extensions.SetPosition2D(position: new Vector2(base.transform.position.x, inventoryItemSelectableDirectional3.transform.position.y) + bottomOffset, t: attachToBottom);
			}
		}
	}

	private void LinkGridSelectables(List<InventoryItemSelectableDirectional> childItems, List<InventoryItemSelectableDirectional> downOverrides, List<InventoryItemSelectableDirectional> upOverrides)
	{
		int rowSplit = RowSplit;
		int num = Mathf.CeilToInt((float)childItems.Count / (float)RowSplit) - 1;
		for (int i = 0; i < childItems.Count; i++)
		{
			int num2 = i % rowSplit;
			int num3 = i / rowSplit;
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional = childItems[i];
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional2 = ((upOverrides != null && upOverrides.Count > 0) ? upOverrides[Mathf.Min(num2, upOverrides.Count - 1)] : null);
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional3 = ((downOverrides != null && downOverrides.Count > 0) ? downOverrides[Mathf.Min(num2, downOverrides.Count - 1)] : null);
			if (inventoryItemSelectableDirectional == null)
			{
				continue;
			}
			inventoryItemSelectableDirectional.Selectables[2] = childItems.GetBy2DIndexes(rowSplit, num2 - 1, num3);
			inventoryItemSelectableDirectional.Selectables[3] = childItems.GetBy2DIndexes(rowSplit, num2 + 1, num3);
			inventoryItemSelectableDirectional.Selectables[0] = childItems.GetBy2DIndexes(rowSplit, num2, num3 - 1, inventoryItemSelectableDirectional2 ? inventoryItemSelectableDirectional2 : null);
			inventoryItemSelectableDirectional.Selectables[1] = childItems.GetBy2DIndexes(rowSplit, num2, num3 + 1);
			if (inventoryItemSelectableDirectional.Selectables[1] == null)
			{
				if (num3 < num)
				{
					inventoryItemSelectableDirectional.Selectables[1] = childItems[childItems.Count - 1];
				}
				else
				{
					inventoryItemSelectableDirectional.Selectables[1] = (inventoryItemSelectableDirectional3 ? inventoryItemSelectableDirectional3 : null);
				}
			}
			if (inventoryItemSelectableDirectional.Selectables[3] == null && num2 < rowSplit - 1 && num3 > 0)
			{
				inventoryItemSelectableDirectional.Selectables[3] = childItems.GetBy2DIndexes(rowSplit, Mathf.Min(num2 + 1, rowSplit - 1), num3 - 1);
			}
		}
	}

	private void PositionGridItems(List<InventoryItemSelectableDirectional> childItems, float yOffset)
	{
		int rowSplit = RowSplit;
		for (int i = 0; i < childItems.Count; i++)
		{
			int num = i % rowSplit;
			int num2 = i / rowSplit;
			childItems[i].transform.SetLocalPosition2D(GridOffset + new Vector2(ItemOffset.x * (float)num, ItemOffset.y * (float)num2 + yOffset));
		}
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (collections == null || collections.Count == 0)
		{
			return null;
		}
		GridSection gridSection = collections[0];
		if (gridSection != null && gridSection.Items.Count > 0)
		{
			if (direction.HasValue)
			{
				if ((bool)itemManager && (bool)itemManager.CurrentSelected)
				{
					IEnumerable<InventoryItemSelectableDirectional> items = collections.SelectMany((GridSection col) => col.Items);
					InventoryItemSelectableDirectional closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis(direction.Value, itemManager.CurrentSelected, items);
					if ((bool)closestOnAxis)
					{
						return closestOnAxis.Get(direction);
					}
				}
				InventoryItemManager.SelectionDirection value = direction.Value;
				if (value != InventoryItemManager.SelectionDirection.Left)
				{
					_ = 3;
					return gridSection.Items[0].Get(direction);
				}
				return gridSection.Items[Mathf.Min(collections[0].Items.Count, RowSplit) - 1].Get(direction);
			}
			return gridSection.Items[0].Get(direction);
		}
		return null;
	}

	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectableDirectional inventoryItemSelectableDirectional = currentSelected as InventoryItemSelectableDirectional;
		if (!inventoryItemSelectableDirectional)
		{
			return base.GetNextSelectablePage(currentSelected, direction);
		}
		for (int i = 0; i < collections.Count; i++)
		{
			GridSection gridSection = collections[i];
			int num = gridSection.Items.IndexOf(inventoryItemSelectableDirectional);
			if (num < 0)
			{
				continue;
			}
			switch (direction)
			{
			case InventoryItemManager.SelectionDirection.Down:
				if (i < collections.Count - 1)
				{
					GridSection gridSection3 = collections[i + 1];
					if (gridSection3.Items.Count > 0)
					{
						return gridSection3.Items[0];
					}
				}
				if (gridSection.Items.Count > 0)
				{
					List<InventoryItemSelectableDirectional> items = gridSection.Items;
					return items[items.Count - 1];
				}
				break;
			case InventoryItemManager.SelectionDirection.Up:
				if (num > 0)
				{
					return gridSection.Items[0];
				}
				if (i > 0)
				{
					GridSection gridSection2 = collections[i - 1];
					if (gridSection2.Items.Count > 0)
					{
						return gridSection2.Items[0];
					}
				}
				break;
			}
		}
		SelectableList selectableList = nextPages[(int)direction];
		if (selectableList != null)
		{
			InventoryItemSelectable[] array = selectableList.Selectables;
			foreach (InventoryItemSelectable inventoryItemSelectable in array)
			{
				if (!(inventoryItemSelectable == null) && inventoryItemSelectable.isActiveAndEnabled)
				{
					return inventoryItemSelectable;
				}
			}
		}
		return base.GetNextSelectablePage(currentSelected, direction);
	}

	public List<T> GetListItems<T>(Func<T, bool> evaluation = null) where T : InventoryItemSelectable
	{
		if (evaluation == null)
		{
			evaluation = (T _) => true;
		}
		return (from T selectable in collections.SelectMany((GridSection section) => section.Items)
			where evaluation(selectable)
			select selectable).ToList();
	}

	private void SetupScrollEvents(bool subscribe)
	{
		if (collections == null || collections.Count == 0)
		{
			return;
		}
		foreach (GridSection collection in collections)
		{
			foreach (InventoryItemSelectableDirectional item in collection.Items)
			{
				if (subscribe)
				{
					item.OnSelected += ScrollEventHandler;
				}
				else
				{
					item.OnSelected -= ScrollEventHandler;
				}
			}
		}
	}

	private void ScrollEventHandler(InventoryItemSelectable item)
	{
		ScrollTo(item);
	}

	public void ScrollTo(InventoryItemSelectable item, bool isInstant = false)
	{
		if (!(item == null))
		{
			if (!hasDoneInitialScroll || (isInstant && !base.gameObject.activeInHierarchy))
			{
				queuedScroll = item;
			}
			else
			{
				ScrollTo(item.transform.position, isInstant);
			}
		}
	}

	private void ScrollTo(Vector2 worldPos, bool isInstant)
	{
		if ((bool)scrollView)
		{
			Vector2 localPosition = scrollView.transform.InverseTransformPoint(worldPos);
			scrollView.ScrollTo(localPosition, isInstant);
			queuedScroll = null;
		}
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this;
	}

	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		if (!direction.HasValue)
		{
			return null;
		}
		Vector2 vector = source.transform.position;
		Vector2 vector2;
		switch (direction.Value)
		{
		case InventoryItemManager.SelectionDirection.Up:
		case InventoryItemManager.SelectionDirection.Down:
			vector2 = new Vector2(2f, 1f);
			break;
		case InventoryItemManager.SelectionDirection.Left:
		case InventoryItemManager.SelectionDirection.Right:
			vector2 = new Vector2(1f, 2f);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		Vector2 vector3 = vector2;
		InventoryItemSelectable result = null;
		float num = float.MaxValue;
		InventoryItemSelectable[] array = selectables[(int)direction.Value].Selectables;
		foreach (InventoryItemSelectable inventoryItemSelectable in array)
		{
			Vector2 vector4 = (Vector2)inventoryItemSelectable.transform.position - vector;
			float magnitude = (vector4 * vector3).magnitude;
			if (!(magnitude > num))
			{
				num = magnitude;
				result = inventoryItemSelectable;
			}
		}
		return result;
	}

	public InventoryItemSelectable GetItemOrFallback(int sectionIndex, int itemIndex)
	{
		if (collections.Count == 0)
		{
			return null;
		}
		bool flag = false;
		if (sectionIndex >= collections.Count)
		{
			sectionIndex = collections.Count - 1;
			flag = true;
		}
		GridSection gridSection = collections[sectionIndex];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		if (flag || itemIndex >= gridSection.Items.Count)
		{
			itemIndex = gridSection.Items.Count - 1;
		}
		return gridSection.Items[itemIndex];
	}

	public InventoryItemSelectable GetFirst()
	{
		if (collections.Count == 0)
		{
			return null;
		}
		GridSection gridSection = collections[0];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		return gridSection.Items[0];
	}

	public InventoryItemSelectable GetLast()
	{
		if (collections.Count == 0)
		{
			return null;
		}
		List<GridSection> list = collections;
		GridSection gridSection = list[list.Count - 1];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		List<InventoryItemSelectableDirectional> items = gridSection.Items;
		return items[items.Count - 1];
	}

	public static void LinkVertical(InventoryItemSelectableDirectional top, InventoryItemGrid bottom)
	{
		if ((bool)top)
		{
			top.Selectables[1] = bottom;
		}
		if ((bool)bottom)
		{
			bottom.selectables[0] = new SelectableList
			{
				Selectables = new InventoryItemSelectable[1] { top }
			};
		}
	}
}
