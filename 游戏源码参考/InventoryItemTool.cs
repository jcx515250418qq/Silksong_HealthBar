using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryItemTool : InventoryItemToolBase
{
	[Header("Tool")]
	[SerializeField]
	private ToolItem itemData;

	[SerializeField]
	private SpriteRenderer itemIcon;

	[SerializeField]
	private GameObject emptyNotch;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer selectedIndicator;

	[Space]
	[SerializeField]
	private Animator slotAnimator;

	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private RuntimeAnimatorController[] slotAnimatorControllers;

	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] attackAnimatorControllers;

	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] skillAnimatorControllers;

	private bool isAnimatorEquipped;

	[NonSerialized]
	private InventoryItemToolManager manager;

	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	private static readonly int _fullAnim = Animator.StringToHash("Full");

	private static readonly int _equipAnim = Animator.StringToHash("Equip");

	private static readonly int _unequipAnim = Animator.StringToHash("Unequip");

	public ToolItemType ToolType => itemData.Type;

	public override ToolItem ItemData => itemData;

	public override string DisplayName
	{
		get
		{
			if (!itemData.IsUnlockedNotHidden)
			{
				return string.Empty;
			}
			return itemData.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!itemData.IsUnlockedNotHidden)
			{
				return string.Empty;
			}
			return itemData.Description;
		}
	}

	public override Sprite Sprite
	{
		get
		{
			if (!itemData.IsUnlockedNotHidden)
			{
				return null;
			}
			return itemData.InventorySpriteBase;
		}
	}

	public override Color? CursorColor
	{
		get
		{
			if ((bool)itemData && (bool)manager)
			{
				return manager.GetToolTypeColor(itemData.Type);
			}
			return null;
		}
	}

	protected override bool IsSeen
	{
		get
		{
			if (!itemData || !itemData.IsUnlockedNotHidden)
			{
				return true;
			}
			if (itemData is IToolExtraNew { HasExtraNew: not false })
			{
				return false;
			}
			return itemData.HasBeenSeen;
		}
		set
		{
			if ((bool)itemData && itemData.IsUnlockedNotHidden)
			{
				itemData.HasBeenSeen = value;
				if (value && itemData is IToolExtraNew { HasExtraNew: not false } toolExtraNew)
				{
					toolExtraNew.SetExtraSeen();
				}
			}
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize(ref slotAnimatorControllers, typeof(ToolItemType));
		ArrayForEnumAttribute.EnsureArraySize(ref attackAnimatorControllers, typeof(AttackToolBinding));
		ArrayForEnumAttribute.EnsureArraySize(ref skillAnimatorControllers, typeof(AttackToolBinding));
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponentsIfNeeded();
	}

	private void GetComponentsIfNeeded()
	{
		if (!manager)
		{
			manager = GetComponentInParent<InventoryItemToolManager>();
		}
	}

	public void SetData(ToolItem newItemData)
	{
		GetComponentsIfNeeded();
		if ((bool)manager)
		{
			InventoryItemToolManager inventoryItemToolManager = manager;
			inventoryItemToolManager.OnToolRefresh = (Action<bool>)Delegate.Remove(inventoryItemToolManager.OnToolRefresh, new Action<bool>(UpdateEquippedDisplay));
			if ((bool)newItemData)
			{
				InventoryItemToolManager inventoryItemToolManager2 = manager;
				inventoryItemToolManager2.OnToolRefresh = (Action<bool>)Delegate.Combine(inventoryItemToolManager2.OnToolRefresh, new Action<bool>(UpdateEquippedDisplay));
			}
		}
		bool num = itemData != newItemData;
		itemData = newItemData;
		if (num)
		{
			ItemDataUpdated();
		}
		base.gameObject.name = (newItemData ? newItemData.name : "null");
		RefreshIcon();
		if ((bool)newItemData && (bool)slotAnimator)
		{
			RuntimeAnimatorController runtimeAnimatorController = slotAnimatorControllers[(int)newItemData.Type];
			slotAnimator.runtimeAnimatorController = runtimeAnimatorController;
			UpdateAttackSlotAnimator();
			slotAnimator.Play(isAnimatorEquipped ? _fullAnim : _emptyAnim);
		}
		UpdateDisplay();
	}

	protected override bool IsToolEquipped(ToolItem toolItem)
	{
		return toolItem.IsEquippedHud;
	}

	private void RefreshIcon()
	{
		if ((bool)itemData && itemData.IsUnlockedNotHidden)
		{
			if ((bool)itemIcon)
			{
				itemIcon.sprite = itemData.InventorySpriteBase;
			}
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: false);
			}
		}
		else
		{
			if ((bool)itemIcon)
			{
				itemIcon.sprite = null;
			}
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: true);
			}
		}
	}

	public override bool Submit()
	{
		if (!manager || !manager.CanChangeEquips(itemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Regular))
		{
			return false;
		}
		if (manager.EquipState == InventoryItemToolManager.EquipStates.SelectTool)
		{
			if (!itemData.IsUnlockedNotHidden)
			{
				return false;
			}
			DoPick();
			return true;
		}
		return base.Submit();
	}

	protected override bool DoPress()
	{
		switch (manager.EquipState)
		{
		case InventoryItemToolManager.EquipStates.None:
			if (InventoryItemToolManager.IsToolEquipped(itemData))
			{
				manager.UnequipTool(itemData, null);
			}
			else
			{
				if (!itemData.IsUnlockedNotHidden)
				{
					return false;
				}
				if (!manager.TryPickupOrPlaceTool(itemData))
				{
					ReportFailure();
				}
			}
			return true;
		case InventoryItemToolManager.EquipStates.SelectTool:
			if (!itemData.IsUnlockedNotHidden)
			{
				return false;
			}
			DoPick();
			return true;
		default:
			return false;
		}
	}

	private void DoPick()
	{
		if (!InventoryItemToolManager.IsToolEquipped(itemData))
		{
			manager.EndSelection(this);
		}
	}

	public override bool Cancel()
	{
		if (manager.ShowingToolMsg)
		{
			manager.HideToolEquipMsg();
		}
		else if (manager.ShowingCursedMsg)
		{
			manager.HideCursedMsg();
		}
		else if ((bool)manager && (bool)manager.SelectedSlot)
		{
			manager.EndSelection(null);
		}
		return base.Cancel();
	}

	private void UpdateEquippedDisplay(bool isInstant)
	{
		GetComponentsIfNeeded();
		Color color = ((!(manager.PickedUpTool == itemData) && (!manager.SelectedSlot || manager.SelectedSlot.Type == itemData.Type)) ? Color.white : InventoryToolCrestSlot.InvalidItemColor);
		if ((bool)itemIcon)
		{
			itemIcon.color = color;
		}
		if (InventoryItemToolManager.IsToolEquipped(itemData))
		{
			if ((bool)selectedIndicator)
			{
				selectedIndicator.Color = manager.GetToolTypeColor(itemData.Type).MultiplyElements(color);
			}
			if ((bool)slotAnimator && (!isAnimatorEquipped || isInstant))
			{
				UpdateAttackSlotAnimator();
				if (isInstant)
				{
					slotAnimator.Play(_fullAnim, 0, 1f);
				}
				else
				{
					slotAnimator.Play(_equipAnim);
				}
				isAnimatorEquipped = true;
			}
		}
		else if ((bool)slotAnimator && (isAnimatorEquipped || isInstant))
		{
			if (isInstant)
			{
				slotAnimator.Play(_emptyAnim, 0, 1f);
			}
			else
			{
				slotAnimator.Play(_unequipAnim);
			}
			isAnimatorEquipped = false;
		}
		RefreshIcon();
		UpdateDisplay();
	}

	private void UpdateAttackSlotAnimator()
	{
		if (itemData.Type.IsAttackType())
		{
			AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(itemData);
			if (attackToolBinding.HasValue)
			{
				slotAnimator.runtimeAnimatorController = ((itemData.Type == ToolItemType.Skill) ? skillAnimatorControllers[(int)attackToolBinding.Value] : attackAnimatorControllers[(int)attackToolBinding.Value]);
			}
		}
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectable nextSelectable = base.GetNextSelectable(direction);
		InventoryItemTool inventoryItemTool = nextSelectable as InventoryItemTool;
		if ((bool)manager && manager.EquipState == InventoryItemToolManager.EquipStates.SelectTool && (!nextSelectable || inventoryItemTool == null || inventoryItemTool.ToolType != manager.SelectedSlot.Type))
		{
			return this;
		}
		return nextSelectable;
	}

	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		if (manager.EquipState != InventoryItemToolManager.EquipStates.SelectTool)
		{
			return base.GetNextSelectablePage(currentSelected, direction);
		}
		return null;
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		manager.SetHoveringTool(this, refreshTools: true);
	}

	public override void Deselect()
	{
		base.Deselect();
		if (manager.HoveringTool == this)
		{
			bool flag = (bool)manager.NextSelected && manager.NextSelected is InventoryItemTool;
			manager.SetHoveringTool(null, !flag);
		}
	}
}
