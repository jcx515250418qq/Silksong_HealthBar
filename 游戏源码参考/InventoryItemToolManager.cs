using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemToolManager : InventoryItemListManager<InventoryItemTool, ToolItem>, IInventoryPaneAvailabilityProvider
{
	public enum EquipStates
	{
		None = 0,
		PlaceTool = 1,
		SelectTool = 2,
		SwitchCrest = 3
	}

	public enum CanChangeEquipsTypes
	{
		Regular = 0,
		Reload = 1,
		Transform = 2
	}

	public Action<bool> OnToolRefresh;

	[SerializeField]
	private SpriteRenderer displayIcon;

	[SerializeField]
	private InventoryItemGrid toolList;

	[SerializeField]
	private InventoryToolCrestList crestList;

	[SerializeField]
	private InventoryFloatingToolSlots extraSlots;

	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private NestedFadeGroupSpriteRenderer[] listSectionHeaders;

	[SerializeField]
	private float disabledListSectionOpacity = 0.5f;

	[Space]
	[SerializeField]
	private LayoutGroup promptLayout;

	[SerializeField]
	private NestedFadeGroupBase equipPrompt;

	[SerializeField]
	private TMP_Text equipPromptText;

	[SerializeField]
	private LocalisedString equipText;

	[SerializeField]
	private LocalisedString unequipText;

	[SerializeField]
	private LocalisedString equipSkillText;

	[SerializeField]
	private LocalisedString unequipSkillText;

	[SerializeField]
	private NestedFadeGroupBase changeCrestPrompt;

	[SerializeField]
	private NestedFadeGroupBase selectCrestPrompt;

	[SerializeField]
	private GameObject cancelPrompt;

	[SerializeField]
	private NestedFadeGroupBase reloadPrompt;

	[SerializeField]
	private NestedFadeGroupBase customTogglePrompt;

	[SerializeField]
	private TMP_Text customTogglePromptText;

	[SerializeField]
	[ArrayForEnum(typeof(CurrencyType))]
	private GameObject[] reloadCurrencyCounters;

	[SerializeField]
	private GameObject boolToggleParent;

	[SerializeField]
	private GameObject boolToggleFill;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase toolGroup;

	[SerializeField]
	private NestedFadeGroupBase crestGroup;

	[SerializeField]
	private float groupFadeTime = 0.1f;

	private Coroutine groupFadeRoutine;

	[SerializeField]
	private InventoryItemToolTween tweenTool;

	[SerializeField]
	private NestedFadeGroupBase toolEquipMsg;

	[SerializeField]
	private TMP_Text toolEquipMsgText;

	[SerializeField]
	private LocalisedString toolEquipMsgTool;

	[SerializeField]
	private LocalisedString toolEquipMsgSkill;

	[SerializeField]
	private LocalisedString reloadMsg;

	[SerializeField]
	private LocalisedString transformMsg;

	[SerializeField]
	private NestedFadeGroupBase crestEquipMsg;

	[SerializeField]
	private float toolMsgFadeInTime;

	[SerializeField]
	private float toolMsgFadeOutTime;

	[SerializeField]
	private NestedFadeGroupBase cursedEquipMsg;

	[SerializeField]
	private TMP_Text cursedEquipMsgText;

	[SerializeField]
	private LocalisedString cursedEquipMsgTool;

	[SerializeField]
	private LocalisedString cursedEquipMsgSkill;

	[SerializeField]
	private LocalisedString cursedEquipMsgCrest;

	[Space]
	[SerializeField]
	private TMP_Text toolAmountText;

	[SerializeField]
	private LocalisedString toolUsePromptText;

	[SerializeField]
	private InventoryItemComboButtonPromptDisplay comboButtonPromptDisplay;

	[SerializeField]
	private float buttonPromptExtraDescOffset;

	[SerializeField]
	private Vector2 buttonPromptCurrencyAltPos;

	[SerializeField]
	private Transform currencyParent;

	[SerializeField]
	private Vector2 currencyPromptAltPos;

	[Space]
	[SerializeField]
	private InventoryItemSelectableButtonEvent changeCrestButton;

	[SerializeField]
	private GameObject crestButtonNormalDisplay;

	[SerializeField]
	private GameObject crestButtonLockedDisplay;

	[SerializeField]
	private SetTextMeshProGameText changeCrestButtonText;

	[SerializeField]
	private LocalisedString changeCrestText;

	[SerializeField]
	private LocalisedString viewCrestsText;

	[SerializeField]
	private GameObject descriptionIconGroup;

	[Space]
	[SerializeField]
	private CollectableItem slotUnlockItem;

	[SerializeField]
	private CrestSocketUnlockInventoryDescription slotUnlockDescExtra;

	[SerializeField]
	private InventoryItemCollectable slotUnlockItemDisplay;

	[Space]
	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	[Space]
	[SerializeField]
	private TMP_Text completionText;

	private string initialToolAmountText;

	private double hideEquipMessageAllowedTime;

	private bool showReloadPrompt;

	private bool showCustomTogglePrompt;

	private bool showEquipPrompt;

	private int currentToolCount;

	private Vector2? buttonPromptInitialPos;

	private Vector2? currencyParentInitialPos;

	private InventoryPane pane;

	private InventoryPaneList paneList;

	private EquipStates equipState;

	private InventoryItemSelectable selectedBeforePickup;

	private bool refreshCurrentSelected;

	public InventoryItemTool HoveringTool { get; private set; }

	public EquipStates EquipState
	{
		get
		{
			return equipState;
		}
		private set
		{
			equipState = value;
			UpdateButtonPrompts();
			paneList.CanSwitchPanes = value != EquipStates.SwitchCrest;
			paneList.InSubMenu = value != EquipStates.None;
		}
	}

	public bool ShowingToolMsg { get; private set; }

	public bool ShowingCrestMsg { get; private set; }

	public bool ShowingCursedMsg { get; private set; }

	public bool IsHoldingTool => PickedUpTool != null;

	public ToolItem PickedUpTool { get; private set; }

	public InventoryToolCrestSlot SelectedSlot { get; private set; }

	public CollectableItem SlotUnlockItem => slotUnlockItem;

	public bool CanUnlockSlot => slotUnlockItem.CollectedAmount > 0;

	public InventoryItemCollectable SlotUnlockItemDisplay => slotUnlockItemDisplay;

	public CrestSocketUnlockInventoryDescription SocketUnlockInventoryDescription => slotUnlockDescExtra;

	public bool IsHeroCursed => Gameplay.CursedCrest.IsEquipped;

	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize(ref listSectionHeaders, typeof(ToolItemType));
		ArrayForEnumAttribute.EnsureArraySize(ref reloadCurrencyCounters, typeof(CurrencyType));
	}

	protected override void Awake()
	{
		pane = GetComponent<InventoryPane>();
		base.Awake();
		OnValidate();
		if ((bool)toolAmountText)
		{
			initialToolAmountText = toolAmountText.text;
		}
		paneList = GetComponentInParent<InventoryPaneList>();
		if ((bool)pane)
		{
			pane.OnPaneEnd += delegate
			{
				if ((bool)tweenTool)
				{
					tweenTool.Cancel();
				}
				HideEquipMsgsInstant();
				EndSwitchingCrest();
				EquipState = EquipStates.None;
				if ((bool)crestGroup)
				{
					crestGroup.AlphaSelf = 0f;
				}
				if ((bool)toolGroup)
				{
					toolGroup.AlphaSelf = 1f;
				}
				PickedUpTool = null;
				selectedBeforePickup = null;
				SelectedSlot = null;
				GetSelectables(null).ForEach(delegate(InventoryItemTool tool)
				{
					tool.ItemData.HasBeenSeen = true;
				});
			};
			pane.OnPaneStart += UpdateTextDisplays;
		}
		UpdateTextDisplays();
		SetToolUsePrompt(null, showHold: false, 0f);
	}

	public override void InstantScroll()
	{
		if ((bool)base.CurrentSelected)
		{
			if (base.CurrentSelected.transform.IsChildOf(base.ItemList.transform))
			{
				base.ItemList.ScrollTo(base.CurrentSelected, isInstant: true);
			}
			return;
		}
		ToolItem unlockedTool = ToolItemManager.UnlockedTool;
		InventoryItemSelectable startSelectable = GetStartSelectable();
		ToolItemManager.UnlockedTool = unlockedTool;
		if (startSelectable != null && startSelectable.transform.IsChildOf(base.ItemList.transform))
		{
			base.ItemList.ScrollTo(startSelectable, isInstant: true);
		}
	}

	private void Start()
	{
		EquipState = EquipStates.None;
		if ((bool)toolEquipMsg)
		{
			toolEquipMsg.gameObject.SetActive(value: true);
			toolEquipMsg.AlphaSelf = 0f;
		}
		if ((bool)crestEquipMsg)
		{
			crestEquipMsg.gameObject.SetActive(value: true);
			crestEquipMsg.AlphaSelf = 0f;
		}
		if ((bool)cursedEquipMsg)
		{
			cursedEquipMsg.gameObject.SetActive(value: true);
			cursedEquipMsg.AlphaSelf = 0f;
		}
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if ((bool)displayIcon)
		{
			displayIcon.gameObject.SetActive(value: false);
		}
		HideEquipMsgs(force: true);
		SetToolUsePrompt(null, showHold: false, 0f);
		if ((bool)toolAmountText)
		{
			toolAmountText.gameObject.SetActive(value: false);
		}
		descriptionIconGroup.SetActive(value: true);
		slotUnlockDescExtra.gameObject.SetActive(value: false);
		showEquipPrompt = false;
		showReloadPrompt = false;
		showCustomTogglePrompt = false;
		UpdateButtonPrompts();
		reloadCurrencyCounters.SetAllActive(value: false);
		currencyParent.gameObject.SetActive(value: false);
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		ToolItem toolItem = null;
		bool flag = true;
		InventoryItemTool inventoryItemTool = selectable as InventoryItemTool;
		if (inventoryItemTool != null)
		{
			toolItem = inventoryItemTool.ItemData;
			flag = CrestHasSlot(toolItem.Type);
		}
		InventoryItemToolBase inventoryItemToolBase = selectable as InventoryItemToolBase;
		InventoryToolCrestSlot inventoryToolCrestSlot = selectable as InventoryToolCrestSlot;
		Sprite sprite;
		Color color;
		if (inventoryItemToolBase != null)
		{
			sprite = inventoryItemToolBase.Sprite;
			color = inventoryItemToolBase.SpriteTint;
			if (inventoryToolCrestSlot == null || !inventoryToolCrestSlot.IsLocked)
			{
				if ((bool)displayIcon)
				{
					displayIcon.gameObject.SetActive(value: true);
					displayIcon.sprite = sprite;
					displayIcon.color = color;
				}
				showEquipPrompt = true;
			}
		}
		else
		{
			sprite = null;
			color = Color.white;
		}
		if (inventoryToolCrestSlot != null)
		{
			if (inventoryToolCrestSlot.IsLocked)
			{
				if (CanUnlockSlot)
				{
					slotUnlockDescExtra.SetSlotSprite(sprite, color);
					slotUnlockDescExtra.gameObject.SetActive(value: true);
					slotUnlockItemDisplay.Item = slotUnlockItem;
				}
			}
			else
			{
				if ((bool)inventoryToolCrestSlot.EquippedItem)
				{
					toolItem = inventoryToolCrestSlot.EquippedItem;
				}
				flag = ToolListHasType(inventoryToolCrestSlot.Type);
			}
		}
		bool flag2 = CanChangeEquips();
		bool isHeroCursed = IsHeroCursed;
		if ((bool)toolItem)
		{
			if (toolItem.IsUnlockedNotHidden)
			{
				if ((bool)toolAmountText && toolItem.DisplayAmountText)
				{
					ToolItemsData.Data toolData = PlayerData.instance.GetToolData(toolItem.name);
					int toolStorageAmount = ToolItemManager.GetToolStorageAmount(toolItem);
					toolAmountText.text = string.Format(initialToolAmountText, toolData.AmountLeft, toolStorageAmount);
					toolAmountText.gameObject.SetActive(value: true);
				}
				if (toolItem.DisplayTogglePrompt)
				{
					showCustomTogglePrompt = true;
					if ((bool)customTogglePromptText)
					{
						customTogglePromptText.text = toolItem.CustomToggleText;
					}
				}
				if (toolItem.ReplenishUsage == ToolItem.ReplenishUsages.OneForOne)
				{
					showReloadPrompt = true;
					if ((bool)reloadPrompt)
					{
						reloadPrompt.AlphaSelf = ((toolItem.CanReload() && flag2) ? 1f : disabledListSectionOpacity);
					}
					if (toolItem.ReplenishResource != ToolItem.ReplenishResources.None)
					{
						currencyParent.gameObject.SetActive(value: true);
						GameObject gameObject = reloadCurrencyCounters[(int)toolItem.ReplenishResource];
						if ((bool)gameObject)
						{
							gameObject.SetActive(value: true);
						}
					}
				}
				if ((!showReloadPrompt || !showCustomTogglePrompt) && !toolItem.HideUsePrompt)
				{
					SetToolUsePrompt(ToolItemManager.GetAttackToolBinding(toolItem), toolItem.ShowPromptHold, toolItem.ExtraDescriptionSection ? buttonPromptExtraDescOffset : 0f);
				}
				if ((bool)customTogglePrompt)
				{
					if (showCustomTogglePrompt)
					{
						customTogglePrompt.AlphaSelf = ((flag2 && !isHeroCursed) ? 1f : disabledListSectionOpacity);
					}
					else
					{
						customTogglePrompt.AlphaSelf = 1f;
					}
				}
				if (toolItem.HasCustomAction)
				{
					comboButtonPromptDisplay.Show(toolItem.CustomButtonCombo);
				}
			}
			else
			{
				showEquipPrompt = false;
			}
		}
		if (inventoryItemTool != null || inventoryToolCrestSlot != null)
		{
			if ((bool)equipPrompt)
			{
				equipPrompt.AlphaSelf = ((flag && flag2 && !isHeroCursed) ? 1f : disabledListSectionOpacity);
			}
			if ((bool)equipPromptText)
			{
				ToolItemType toolItemType = ((inventoryItemTool != null) ? inventoryItemTool.ToolType : inventoryToolCrestSlot.Type);
				if (toolItem != null && toolItem.IsEquipped)
				{
					equipPromptText.text = ((toolItemType == ToolItemType.Skill) ? unequipSkillText : unequipText);
				}
				else
				{
					equipPromptText.text = ((toolItemType == ToolItemType.Skill) ? equipSkillText : equipText);
				}
			}
		}
		else if (selectable is InventoryItemSelectableButtonEvent)
		{
			if ((bool)equipPrompt)
			{
				equipPrompt.AlphaSelf = 1f;
			}
			if ((bool)equipPromptText)
			{
				equipPromptText.text = (flag2 ? changeCrestText : viewCrestsText);
			}
		}
		UpdateButtonPrompts();
	}

	public bool TryPickupOrPlaceTool(ToolItem tool)
	{
		PickedUpTool = tool;
		if (!tool)
		{
			return false;
		}
		IEnumerable<InventoryToolCrestSlot> enumerable = null;
		IEnumerable<InventoryToolCrestSlot> enumerable2 = null;
		IEnumerable<InventoryToolCrestSlot> enumerable3 = null;
		if ((bool)crestList)
		{
			enumerable2 = crestList.GetSlots();
			if (GetAvailableSlotCount(enumerable2, tool.Type, checkEmpty: true) > 0)
			{
				enumerable = enumerable2;
			}
		}
		if (enumerable == null && (bool)extraSlots)
		{
			enumerable3 = extraSlots.GetSlots();
			if (GetAvailableSlotCount(enumerable3, tool.Type, checkEmpty: true) > 0)
			{
				enumerable = enumerable3;
			}
		}
		if (enumerable == null)
		{
			if (GetAvailableSlotCount(enumerable2, tool.Type, checkEmpty: false) > 0)
			{
				enumerable = enumerable2;
			}
			else if (GetAvailableSlotCount(enumerable3, tool.Type, checkEmpty: false) > 0)
			{
				enumerable = enumerable3;
			}
		}
		if (enumerable != null)
		{
			InventoryToolCrestSlot availableSlot = GetAvailableSlot(enumerable, tool.Type);
			if ((bool)availableSlot)
			{
				EquipState = EquipStates.PlaceTool;
				selectedBeforePickup = base.CurrentSelected;
				if (availableSlot.Type.IsAttackType())
				{
					if (GetAvailableSlotCount(enumerable, availableSlot.Type, checkEmpty: false) == 1)
					{
						PlaceTool(availableSlot, isManual: true);
					}
					else
					{
						PlayMoveSound();
						SetSelected(availableSlot, null);
					}
				}
				else if (GetAvailableSlotCount(enumerable, availableSlot.Type, checkEmpty: true) > 0)
				{
					PlaceTool(availableSlot, isManual: true);
				}
				else
				{
					int availableSlotCount = GetAvailableSlotCount(enumerable2, availableSlot.Type, checkEmpty: false);
					int availableSlotCount2 = GetAvailableSlotCount(enumerable3, availableSlot.Type, checkEmpty: false);
					if (availableSlotCount + availableSlotCount2 == 1)
					{
						PlaceTool(availableSlot, isManual: true);
					}
					else
					{
						PlayMoveSound();
						SetSelected(availableSlot, null);
					}
				}
				RefreshTools();
				return true;
			}
		}
		PickedUpTool = null;
		return false;
	}

	public void PlaceTool(InventoryToolCrestSlot slot, bool isManual)
	{
		if ((bool)slot && PickedUpTool.Type != slot.Type)
		{
			return;
		}
		ToolItem pickedUpTool = PickedUpTool;
		PickedUpTool = null;
		EquipState = EquipStates.None;
		if (isManual)
		{
			slot.SetEquipped(pickedUpTool, isManual: true, refreshTools: true);
		}
		if ((bool)selectedBeforePickup)
		{
			if (isManual)
			{
				slot.PreOpenSlot();
			}
			if ((bool)tweenTool && (bool)slot)
			{
				tweenTool.DoPlace(selectedBeforePickup.transform.position, slot.transform.position, pickedUpTool, Selected);
			}
			else
			{
				Selected();
			}
		}
		void Selected()
		{
			SetSelected(selectedBeforePickup, null);
			selectedBeforePickup = null;
		}
	}

	public InventoryToolCrestSlot GetAvailableSlot(IEnumerable<InventoryToolCrestSlot> slots, ToolItemType toolType)
	{
		InventoryToolCrestSlot inventoryToolCrestSlot = null;
		foreach (InventoryToolCrestSlot slot in slots)
		{
			if (!slot.IsLocked && slot.Type == toolType)
			{
				if (!inventoryToolCrestSlot)
				{
					inventoryToolCrestSlot = slot;
				}
				if (!slot.EquippedItem)
				{
					return slot;
				}
			}
		}
		return inventoryToolCrestSlot;
	}

	private static int GetAvailableSlotCount(IEnumerable<InventoryToolCrestSlot> slots, ToolItemType? toolType, bool checkEmpty)
	{
		return slots.Count((InventoryToolCrestSlot slot) => !slot.IsLocked && (!toolType.HasValue || slot.Type == toolType) && (!checkEmpty || slot.EquippedItem == null));
	}

	public static bool IsToolEquipped(ToolItem toolItem)
	{
		return ToolItemManager.IsToolEquipped(toolItem, ToolEquippedReadSource.Hud);
	}

	public bool CrestHasSlot(ToolItemType type)
	{
		if ((bool)crestList && crestList.CrestHasSlot(type))
		{
			return true;
		}
		if ((bool)extraSlots && GetAvailableSlotCount(extraSlots.GetSlots(), type, checkEmpty: false) > 0)
		{
			return true;
		}
		return false;
	}

	public bool CrestHasAnySlots()
	{
		if ((bool)crestList && crestList.CrestHasAnySlots())
		{
			return true;
		}
		if (IsHeroCursed)
		{
			return false;
		}
		if ((bool)extraSlots && GetAvailableSlotCount(extraSlots.GetSlots(), null, checkEmpty: false) > 0)
		{
			return true;
		}
		return false;
	}

	public bool ToolListHasType(ToolItemType type)
	{
		if ((bool)toolList)
		{
			foreach (InventoryItemTool listItem in toolList.GetListItems<InventoryItemTool>())
			{
				if (listItem.ToolType == type)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UnequipTool(ToolItem toolItem, InventoryToolCrestSlot slot)
	{
		if (!toolItem)
		{
			return;
		}
		ToolItemManager.UnequipTool(toolItem);
		if (!slot && (bool)crestList)
		{
			slot = crestList.GetEquippedToolSlot(toolItem);
		}
		if (!slot && (bool)extraSlots)
		{
			slot = extraSlots.GetEquippedToolSlot(toolItem);
		}
		Vector3? vector = null;
		Vector3? vector2 = null;
		if ((bool)slot)
		{
			vector = slot.transform.position;
			slot.SetEquipped(null, isManual: true, refreshTools: false);
		}
		if ((bool)toolList)
		{
			InventoryItemTool inventoryItemTool = toolList.GetListItems((InventoryItemTool t) => t.ItemData == toolItem).FirstOrDefault();
			if (inventoryItemTool != null)
			{
				toolList.ScrollTo(inventoryItemTool, isInstant: true);
				vector2 = inventoryItemTool.transform.position;
			}
		}
		if (vector.HasValue && vector2.HasValue && (bool)tweenTool)
		{
			tweenTool.DoReturn(vector.Value, vector2.Value, toolItem, RefreshTools);
		}
		else
		{
			RefreshTools();
		}
	}

	public void RefreshTools()
	{
		RefreshTools(isInstant: false, updateCrest: true);
	}

	public void RefreshTools(bool isInstant, bool updateCrest)
	{
		for (int i = 0; i < listSectionHeaders.Length; i++)
		{
			Color color = listSectionHeaders[i].Color;
			if ((bool)SelectedSlot && i != (int)SelectedSlot.Type)
			{
				color.a = disabledListSectionOpacity;
			}
			else
			{
				color.a = 1f;
			}
			listSectionHeaders[i].Color = color;
		}
		bool isHidden = crestList.CurrentCrest.IsHidden;
		if ((bool)crestButtonLockedDisplay)
		{
			crestButtonLockedDisplay.SetActive(isHidden);
		}
		if ((bool)crestButtonNormalDisplay)
		{
			crestButtonNormalDisplay.SetActive(!isHidden);
		}
		if (updateCrest)
		{
			InventoryToolCrest currentCrest = crestList.CurrentCrest;
			if ((bool)currentCrest)
			{
				currentCrest.UpdateListDisplay(isInstant);
			}
			InventoryFloatingToolSlots inventoryFloatingToolSlots = extraSlots;
			EquipStates equipStates = EquipState;
			inventoryFloatingToolSlots.SetInEquipMode(equipStates == EquipStates.PlaceTool || equipStates == EquipStates.SelectTool);
		}
		OnToolRefresh?.Invoke(isInstant);
		if (refreshCurrentSelected)
		{
			if (base.CurrentSelected == null || !base.CurrentSelected.gameObject.activeInHierarchy)
			{
				SetSelected(SelectedActionType.LeftMost, justDisplay: true);
			}
			refreshCurrentSelected = false;
		}
	}

	public void OnAppliedCrest()
	{
		refreshCurrentSelected = true;
	}

	public void StartSelection(InventoryToolCrestSlot slot)
	{
		if (!(toolList == null))
		{
			List<InventoryItemTool> listItems = toolList.GetListItems((InventoryItemTool toolItem) => toolItem.ToolType == slot.Type);
			List<InventoryItemTool> list = listItems.Where((InventoryItemTool toolItem) => !IsToolEquipped(toolItem.ItemData)).ToList();
			InventoryItemTool inventoryItemTool = null;
			if (list.Count > 0)
			{
				inventoryItemTool = list[0];
			}
			else if (listItems.Count > 0)
			{
				inventoryItemTool = listItems[0];
			}
			if (!(inventoryItemTool == null))
			{
				SelectedSlot = slot;
				EquipState = EquipStates.SelectTool;
				PlayMoveSound();
				SetSelected(inventoryItemTool, null);
				RefreshTools();
			}
		}
	}

	public void EndSelection(InventoryItemTool tool)
	{
		if (!SelectedSlot)
		{
			return;
		}
		if ((bool)tool && (bool)tool.ItemData && SelectedSlot.Type == tool.ToolType)
		{
			if ((bool)tweenTool)
			{
				SelectedSlot.SetEquipped(tool.ItemData, isManual: true, refreshTools: true);
				tweenTool.DoPlace(tool.transform.position, SelectedSlot.transform.position, tool.ItemData, SelectionEnd);
				return;
			}
			SelectedSlot.SetEquipped(tool.ItemData, isManual: true, refreshTools: true);
		}
		SelectionEnd();
		void SelectionEnd()
		{
			PlayMoveSound();
			SetSelected(SelectedSlot, null);
			SelectedSlot = null;
			EquipState = EquipStates.None;
			RefreshTools();
		}
	}

	public bool BeginSwitchingCrest()
	{
		if (EquipState != 0)
		{
			return false;
		}
		EquipState = EquipStates.SwitchCrest;
		HideEquipMsgs(force: true);
		RefreshTools();
		return true;
	}

	public void PaneMovePrevented()
	{
		if (equipState == EquipStates.SwitchCrest)
		{
			crestList.PaneMovePrevented();
		}
	}

	public bool EndSwitchingCrest()
	{
		if (EquipState != EquipStates.SwitchCrest)
		{
			return false;
		}
		EquipState = EquipStates.None;
		HideCrestEquipMsg(force: true);
		return true;
	}

	public float FadeToolGroup(bool fadeIn)
	{
		if (!toolGroup)
		{
			return 0f;
		}
		float num = toolGroup.FadeTo(fadeIn ? 1 : 0, groupFadeTime, null, isRealtime: true);
		if (groupFadeRoutine != null)
		{
			StopCoroutine(groupFadeRoutine);
		}
		if (fadeIn)
		{
			groupFadeRoutine = this.StartTimerRoutine(0f, num, null, null, delegate
			{
				if ((bool)cursor)
				{
					cursor.Activate();
				}
				if (pane.IsPaneActive)
				{
					SelectedActionType select = SelectedActionType.Previous;
					InventoryToolCrestSlot inventoryToolCrestSlot = base.CurrentSelected as InventoryToolCrestSlot;
					if (inventoryToolCrestSlot != null && !crestList.CurrentCrest.HasSlot(inventoryToolCrestSlot))
					{
						select = SelectedActionType.LeftMost;
					}
					SetProxyActive(value: true, select);
				}
			}, isRealtime: true);
		}
		else
		{
			if ((bool)cursor)
			{
				cursor.Deactivate();
			}
			if (pane.IsPaneActive)
			{
				SetProxyActive(value: false);
			}
		}
		return num;
	}

	public float FadeCrestGroup(bool fadeIn)
	{
		if ((bool)crestGroup)
		{
			return crestGroup.FadeTo(fadeIn ? 1 : 0, groupFadeTime, null, isRealtime: true);
		}
		return 0f;
	}

	public Color GetToolTypeColor(ToolItemType type)
	{
		return UI.GetToolTypeColor(type);
	}

	public bool CanChangeEquips()
	{
		if (!GameManager.instance.playerData.atBench)
		{
			return CheatManager.CanChangeEquipsAnywhere;
		}
		return true;
	}

	public bool CanChangeEquips(ToolItemType promptToolType, CanChangeEquipsTypes changeType)
	{
		if (changeType == CanChangeEquipsTypes.Regular && IsHeroCursed)
		{
			if (ShowingCursedMsg)
			{
				HideCursedMsg();
			}
			else
			{
				ShowCursedMsg(isCrestEquip: false, promptToolType);
			}
			return false;
		}
		if (CanChangeEquips())
		{
			return true;
		}
		if (ShowingToolMsg)
		{
			HideToolEquipMsg();
		}
		else
		{
			ShowToolEquipMsg(promptToolType, changeType);
		}
		return false;
	}

	public void ShowToolEquipMsg(ToolItemType type, CanChangeEquipsTypes changeType)
	{
		if ((bool)toolEquipMsg && !ShowingToolMsg)
		{
			if ((bool)toolEquipMsgText)
			{
				TMP_Text tMP_Text = toolEquipMsgText;
				tMP_Text.text = changeType switch
				{
					CanChangeEquipsTypes.Reload => reloadMsg, 
					CanChangeEquipsTypes.Transform => transformMsg, 
					_ => (type == ToolItemType.Skill) ? toolEquipMsgSkill : toolEquipMsgTool, 
				};
			}
			toolEquipMsg.FadeTo(1f, toolMsgFadeInTime, null, isRealtime: true);
			ShowingToolMsg = true;
			paneList.InSubMenu = true;
			hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)toolMsgFadeInTime;
			failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
	}

	public void HideToolEquipMsg(bool force = false)
	{
		if ((bool)toolEquipMsg && ShowingToolMsg && (force || !(Time.unscaledTimeAsDouble < hideEquipMessageAllowedTime)))
		{
			toolEquipMsg.FadeTo(0f, toolMsgFadeOutTime, null, isRealtime: true);
			ShowingToolMsg = false;
			paneList.InSubMenu = false;
		}
	}

	public void HideToolEquipMsgInstant()
	{
		if ((bool)toolEquipMsg && ShowingToolMsg)
		{
			toolEquipMsg.FadeTo(0f, 0f, null, isRealtime: true);
			ShowingToolMsg = false;
			paneList.InSubMenu = false;
		}
	}

	public void HideEquipMsgs(bool force = false)
	{
		HideToolEquipMsg(force);
		HideCrestEquipMsg(force);
		HideCursedMsg(force);
	}

	public void HideEquipMsgsInstant()
	{
		HideToolEquipMsgInstant();
		HideCrestEquipMsgInstant();
		HideCursedMsgInstant();
	}

	public void ShowCrestEquipMsg()
	{
		ShowingCrestMsg = ShowBasicEquipMsg(crestEquipMsg, ShowingCrestMsg);
	}

	public void HideCrestEquipMsg(bool force = false)
	{
		ShowingCrestMsg = HideBasicEquipMsg(crestEquipMsg, toolMsgFadeOutTime, ShowingCrestMsg, force);
	}

	public void HideCrestEquipMsgInstant()
	{
		ShowingCrestMsg = HideBasicEquipMsg(crestEquipMsg, 0f, ShowingCrestMsg, force: true);
	}

	public void ShowCursedMsg(bool isCrestEquip, ToolItemType toolType)
	{
		if ((bool)cursedEquipMsgText)
		{
			if (isCrestEquip)
			{
				cursedEquipMsgText.text = cursedEquipMsgCrest;
			}
			else if (toolType == ToolItemType.Skill)
			{
				cursedEquipMsgText.text = cursedEquipMsgSkill;
			}
			else
			{
				cursedEquipMsgText.text = cursedEquipMsgTool;
			}
		}
		ShowingCursedMsg = ShowBasicEquipMsg(cursedEquipMsg, ShowingCursedMsg);
	}

	public void HideCursedMsg(bool force = false)
	{
		ShowingCursedMsg = HideBasicEquipMsg(cursedEquipMsg, toolMsgFadeOutTime, ShowingCursedMsg, force);
	}

	public void HideCursedMsgInstant()
	{
		ShowingCursedMsg = HideBasicEquipMsg(cursedEquipMsg, 0f, ShowingCursedMsg, force: true);
	}

	private bool ShowBasicEquipMsg(NestedFadeGroupBase msgGroup, bool showingBool)
	{
		if (!msgGroup || showingBool)
		{
			return showingBool;
		}
		msgGroup.FadeTo(1f, toolMsgFadeInTime, null, isRealtime: true);
		paneList.InSubMenu = true;
		hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)toolMsgFadeInTime;
		failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		return true;
	}

	private bool HideBasicEquipMsg(NestedFadeGroupBase msgGroup, float fadeTime, bool showingBool, bool force)
	{
		if (!msgGroup || !showingBool)
		{
			return showingBool;
		}
		if (!force && Time.unscaledTimeAsDouble < hideEquipMessageAllowedTime)
		{
			return true;
		}
		msgGroup.FadeTo(0f, fadeTime, null, isRealtime: true);
		paneList.InSubMenu = false;
		return false;
	}

	protected override List<ToolItem> GetItems()
	{
		if (!PlayerData.instance.ConstructedFarsight)
		{
			List<ToolItem> list = ToolItemManager.GetUnlockedTools().ToList();
			currentToolCount = list.Count;
			return list;
		}
		currentToolCount = 0;
		List<ToolItem> list2 = ToolItemManager.GetAllTools().ToList();
		for (int num = list2.Count - 1; num >= 0; num--)
		{
			ToolItem toolItem = list2[num];
			if (toolItem.IsUnlockedNotHidden)
			{
				currentToolCount++;
			}
			else if (!toolItem.IsCounted)
			{
				list2.RemoveAt(num);
			}
			else
			{
				SavedItem countKey = toolItem.CountKey;
				foreach (ToolItem item in list2)
				{
					if (!(item == toolItem) && item.CountKey == countKey)
					{
						list2.RemoveAt(num);
						break;
					}
				}
			}
		}
		return list2;
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemTool> selectableItems, List<ToolItem> items)
	{
		for (int i = 0; i < selectableItems.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(value: true);
			selectableItems[i].SetData(items[i]);
		}
		int[] array = typeof(ToolItemType).GetValuesWithOrder().ToArray();
		List<InventoryItemGrid.GridSection> list = new List<InventoryItemGrid.GridSection>(array.Length);
		int[] array2 = array;
		foreach (int i2 in array2)
		{
			list.Add(new InventoryItemGrid.GridSection
			{
				Header = listSectionHeaders[i2].transform,
				Items = selectableItems.Where((InventoryItemTool item) => item.ToolType == (ToolItemType)i2).Cast<InventoryItemSelectableDirectional>().ToList()
			});
		}
		return list;
	}

	protected override void OnItemListSetup()
	{
		if ((bool)completionText)
		{
			if (PlayerData.instance.ConstructedFarsight)
			{
				completionText.gameObject.SetActive(value: true);
				int count = ToolItemManager.GetCount(ToolItemManager.GetAllTools(), null);
				int num = Mathf.Min(currentToolCount, count);
				completionText.text = $"{num} / {count}";
			}
			else
			{
				completionText.gameObject.SetActive(value: false);
			}
		}
	}

	public bool IsAvailable()
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		if (ToolItemManager.GetAllCrests().Count((ToolCrest crest) => crest.IsVisible) > 1)
		{
			return true;
		}
		if (GetItems().Count <= 0)
		{
			return false;
		}
		foreach (ToolCrest allCrest in ToolItemManager.GetAllCrests())
		{
			if (allCrest.IsVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void SetToolUsePrompt(AttackToolBinding? binding, bool showHold, float offsetY)
	{
		if (comboButtonPromptDisplay == null)
		{
			return;
		}
		Vector2 valueOrDefault = currencyParentInitialPos.GetValueOrDefault();
		if (!currencyParentInitialPos.HasValue)
		{
			valueOrDefault = currencyParent.transform.localPosition;
			currencyParentInitialPos = valueOrDefault;
		}
		if (!binding.HasValue)
		{
			comboButtonPromptDisplay.Hide();
			currencyParent.transform.SetLocalPosition2D(currencyParentInitialPos.Value);
			return;
		}
		currencyParent.transform.SetLocalPosition2D(currencyPromptAltPos);
		Vector3 localPosition = comboButtonPromptDisplay.transform.localPosition;
		valueOrDefault = buttonPromptInitialPos.GetValueOrDefault();
		if (!buttonPromptInitialPos.HasValue)
		{
			valueOrDefault = localPosition;
			buttonPromptInitialPos = valueOrDefault;
		}
		if (currencyParent.gameObject.activeSelf)
		{
			localPosition = buttonPromptCurrencyAltPos;
		}
		else
		{
			localPosition = buttonPromptInitialPos.Value;
			localPosition.y += offsetY;
		}
		comboButtonPromptDisplay.transform.localPosition = localPosition;
		comboButtonPromptDisplay.Show(new InventoryItemComboButtonPromptDisplay.Display
		{
			ActionButton = HeroActionButton.QUICK_CAST,
			DirectionModifier = binding.Value,
			PromptText = toolUsePromptText,
			ShowHold = showHold
		});
	}

	private void UpdateTextDisplays()
	{
		LocalisedString localisedString = (CanChangeEquips() ? changeCrestText : viewCrestsText);
		if ((bool)changeCrestButton)
		{
			changeCrestButton.InteractionText = localisedString;
		}
		if ((bool)changeCrestButtonText)
		{
			changeCrestButtonText.Text = localisedString;
		}
	}

	protected override InventoryItemSelectable GetStartSelectable()
	{
		InventoryItemTool inventoryItemTool = GetSelectables(null).FirstOrDefault((InventoryItemTool tool) => ToolItemManager.UnlockedTool == tool.ItemData);
		ToolItemManager.UnlockedTool = null;
		if ((bool)inventoryItemTool)
		{
			return inventoryItemTool;
		}
		return base.GetStartSelectable();
	}

	private void UpdateButtonPrompts()
	{
		bool active = showEquipPrompt && equipState != EquipStates.SwitchCrest;
		bool active2 = equipState == EquipStates.None;
		bool active3 = equipState == EquipStates.SwitchCrest;
		bool active4 = equipState != EquipStates.None;
		if ((bool)equipPrompt)
		{
			equipPrompt.gameObject.SetActive(active);
		}
		if ((bool)changeCrestPrompt)
		{
			changeCrestPrompt.AlphaSelf = (IsHeroCursed ? disabledListSectionOpacity : 1f);
			changeCrestPrompt.gameObject.SetActive(active2);
		}
		if ((bool)selectCrestPrompt)
		{
			selectCrestPrompt.gameObject.SetActive(active3);
			selectCrestPrompt.AlphaSelf = (CanChangeEquips() ? 1f : disabledListSectionOpacity);
		}
		if ((bool)cancelPrompt)
		{
			cancelPrompt.SetActive(active4);
		}
		if ((bool)reloadPrompt)
		{
			reloadPrompt.gameObject.SetActive(showReloadPrompt && equipState != EquipStates.SwitchCrest);
		}
		if ((bool)customTogglePrompt)
		{
			customTogglePrompt.gameObject.SetActive(showCustomTogglePrompt && equipState != EquipStates.SwitchCrest);
		}
		if ((bool)boolToggleParent)
		{
			boolToggleParent.SetActive(showReloadPrompt && equipState != EquipStates.SwitchCrest);
		}
		if ((bool)boolToggleFill)
		{
			boolToggleFill.SetActive(value: false);
		}
		if ((bool)promptLayout)
		{
			promptLayout.ForceUpdateLayoutNoCanvas();
		}
	}

	public void SetHoveringTool(InventoryItemTool tool, bool refreshTools)
	{
		HoveringTool = tool;
		if (refreshTools)
		{
			RefreshTools();
		}
	}

	public override bool MoveSelection(SelectionDirection direction)
	{
		bool flag = base.MoveSelection(direction);
		if (!flag)
		{
			EquipStates equipStates = equipState;
			if (equipStates == EquipStates.PlaceTool || equipStates == EquipStates.SelectTool)
			{
				return true;
			}
		}
		return flag;
	}
}
