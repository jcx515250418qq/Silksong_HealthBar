using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemSelectableDirectional : InventoryItemSelectable
{
	[Serializable]
	public class SelectableList
	{
		public List<InventoryItemSelectable> Selectables;
	}

	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	public InventoryItemSelectable[] Selectables;

	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	[FormerlySerializedAs("fallbackSelectables")]
	public SelectableList[] FallbackSelectables;

	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private SelectableList[] nextPages = new SelectableList[0];

	private InventoryAutoNavGroup autoNavGroup;

	protected virtual bool IsAutoNavSelectable => true;

	public InventoryItemGrid Grid { get; set; }

	public int GridSectionIndex { get; set; }

	public int GridItemIndex { get; set; }

	protected virtual void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref Selectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize(ref FallbackSelectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize(ref nextPages, typeof(InventoryItemManager.SelectionDirection));
	}

	protected virtual void Awake()
	{
		OnValidate();
	}

	protected virtual void OnEnable()
	{
		EvaluateAutoNav();
	}

	protected virtual void Start()
	{
	}

	protected virtual void OnDisable()
	{
		if ((bool)autoNavGroup)
		{
			autoNavGroup.Deregister(this);
		}
	}

	protected void EvaluateAutoNav()
	{
		if (!autoNavGroup)
		{
			autoNavGroup = GetComponentInParent<InventoryAutoNavGroup>();
		}
		if ((bool)autoNavGroup)
		{
			if (IsAutoNavSelectable)
			{
				autoNavGroup.Register(this);
			}
			else
			{
				autoNavGroup.Deregister(this);
			}
		}
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (!base.gameObject.activeSelf && direction.HasValue)
		{
			return GetNextSelectable(direction.Value);
		}
		return base.Get(direction);
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return GetNextSelectable(direction, allowAutoNavOnFirst: true);
	}

	protected InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction, bool allowAutoNavOnFirst)
	{
		InventoryItemSelectable inventoryItemSelectable = Selectables[(int)direction];
		bool flag = !inventoryItemSelectable;
		if (inventoryItemSelectable == null || !inventoryItemSelectable.gameObject.activeInHierarchy)
		{
			inventoryItemSelectable = GetNextFallbackSelectable(direction);
		}
		if (inventoryItemSelectable == null && (!flag || allowAutoNavOnFirst))
		{
			inventoryItemSelectable = GetSelectableFromAutoNavGroup<InventoryItemSelectable>(direction);
			if (inventoryItemSelectable == this)
			{
				inventoryItemSelectable = null;
			}
		}
		if (inventoryItemSelectable == null && (bool)base.transform.parent)
		{
			IInventorySelectionParent componentInParent = base.transform.parent.GetComponentInParent<IInventorySelectionParent>();
			if (componentInParent != null)
			{
				inventoryItemSelectable = componentInParent.GetNextSelectable(this, direction);
			}
		}
		return inventoryItemSelectable;
	}

	protected InventoryItemSelectable GetNextFallbackSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return FallbackSelectables[(int)direction].Selectables.FirstOrDefault((InventoryItemSelectable fallback) => fallback != null && fallback.gameObject.activeInHierarchy);
	}

	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		SelectableList selectableList = nextPages[(int)direction];
		if (selectableList != null)
		{
			foreach (InventoryItemSelectable selectable in selectableList.Selectables)
			{
				if (!(selectable == null) && selectable.isActiveAndEnabled)
				{
					return selectable;
				}
			}
		}
		return base.GetNextSelectablePage(currentSelected, direction);
	}

	protected InventoryItemSelectable GetSelectableFromAutoNavGroup<T>(InventoryItemManager.SelectionDirection direction, Func<T, bool> predicate = null) where T : InventoryItemSelectable
	{
		if (!autoNavGroup)
		{
			return this;
		}
		return autoNavGroup.GetNextSelectable(this, direction, predicate);
	}
}
