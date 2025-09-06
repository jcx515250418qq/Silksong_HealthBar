using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class InventoryToolCrestSlot : InventoryItemToolBase
{
	private const float DISABLED_SLOT_OPACITY = 0.5f;

	private const float WRONG_SLOT_OPACITY = 0.3f;

	private const float SLOT_FADE_DURATION = 0.1f;

	private const float LOCKED_SLOT_SCALE = 0.8f;

	public static readonly Color InvalidItemColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	[Header("Tool Crest Slot")]
	[SerializeField]
	private Sprite slotTypeSprite;

	[SerializeField]
	private NestedFadeGroupBase slotTypeGroup;

	[SerializeField]
	private NestedFadeGroupBase slotTypeIconGroup;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotTypeIcon;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotTypeIconFilled;

	[SerializeField]
	private Animator slotFilledAnimator;

	[SerializeField]
	private SpriteRenderer itemIcon;

	[SerializeField]
	private SpriteMask itemIconMask;

	[SerializeField]
	private Animator slotAnimator;

	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] attackAnimatorControllers;

	[SerializeField]
	private TextMeshPro amountText;

	[Space]
	[SerializeField]
	private AnimationCurve unlockReadyColourPulseCurve;

	[SerializeField]
	private float unlockReadyColorPulseDuration;

	[SerializeField]
	private float unlockHoldDuration;

	[SerializeField]
	private MinMaxFloat unlockHoldShakeMagnitude;

	[SerializeField]
	private PlayParticleEffects unlockHoldParticles;

	[SerializeField]
	private PassColour unlockBurstEffectPrefab;

	[Header("Vibrations")]
	[SerializeField]
	private VibrationDataAsset unlockRumble;

	[SerializeField]
	private VibrationDataAsset unlockShake;

	private bool isPreOpened;

	private bool wasVisible;

	[NonSerialized]
	private InventoryItemToolManager manager;

	private InventoryToolCrestList crestList;

	private int previousAnimId;

	private bool isPulsingColour;

	private Color pulseColourA;

	private Color pulseColourB;

	private float pulseColourTimeElapsed;

	private Transform unlockHoldShakeTransform;

	private Vector3 unlockHoldInitialPosition;

	private Coroutine unlockHoldRoutine;

	private Action onUnlockHoldEnd;

	private Func<ToolCrestsData.SlotData> getSavedDataOverride;

	private Action<ToolCrestsData.SlotData> setSavedDataOverride;

	private bool isSelected;

	private bool wasSelected;

	private PassColour spawnedUnlockBurstEffect;

	private VibrationEmission consumeRumbleEmission;

	private int queuedAnimId;

	private int queuedSmallAnimId;

	private static readonly int _equipAnim = Animator.StringToHash("Equip");

	private static readonly int _unequipAnim = Animator.StringToHash("Unequip");

	private static readonly int _fullAnim = Animator.StringToHash("Full");

	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	private static readonly int _lockedAnim = Animator.StringToHash("Locked");

	private static readonly int _unlockReadyIdleAnim = Animator.StringToHash("Unlock Ready Idle");

	private static readonly int _unlockReadySelectedAnim = Animator.StringToHash("Unlock Ready Selected");

	private static readonly int _filledAnim = Animator.StringToHash("Filled");

	private static readonly int _flashAmountProp = Shader.PropertyToID("_FlashAmount");

	private static readonly int _flashColorProp = Shader.PropertyToID("_FlashColor");

	private MaterialPropertyBlock block;

	private ToolCrest.SlotInfo slotInfo;

	private ToolItem itemData;

	private int lastUpdate;

	private InventoryItemToolManager.EquipStates lastEquipState;

	private bool lastSelectState;

	private MaterialPropertyBlock Block => block ?? (block = new MaterialPropertyBlock());

	public InventoryToolCrest Crest { get; private set; }

	public int SlotIndex { get; private set; }

	public override string DisplayName
	{
		get
		{
			if (!EquippedItem)
			{
				return string.Empty;
			}
			return EquippedItem.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!EquippedItem)
			{
				return string.Empty;
			}
			return EquippedItem.Description;
		}
	}

	public override Sprite Sprite
	{
		get
		{
			if (!EquippedItem)
			{
				return slotTypeSprite;
			}
			return EquippedItem.InventorySpriteBase;
		}
	}

	private Sprite ItemSprite
	{
		get
		{
			if (!EquippedItem)
			{
				return null;
			}
			return EquippedItem.GetInventorySprite((EquippedItem.PoisonDamageTicks > 0 && IsToolEquipped(Gameplay.PoisonPouchTool)) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default);
		}
	}

	public override Color SpriteTint
	{
		get
		{
			if ((bool)EquippedItem && (bool)itemIcon)
			{
				return Color.white;
			}
			if (IsLocked && (!manager || !manager.CanUnlockSlot))
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			if ((bool)slotTypeIcon)
			{
				return slotTypeIcon.Color;
			}
			return Color.white;
		}
	}

	public override Color? CursorColor
	{
		get
		{
			if (!manager)
			{
				return base.CursorColor;
			}
			if (isPulsingColour)
			{
				return pulseColourA;
			}
			if (IsLocked)
			{
				return base.CursorColor;
			}
			return manager.GetToolTypeColor(SlotInfo.Type);
		}
	}

	public float ItemFlashAmount
	{
		get
		{
			if ((bool)itemIcon)
			{
				return itemIcon.sharedMaterial.GetFloat(_flashAmountProp);
			}
			return 0f;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if ((bool)itemIcon)
			{
				MaterialPropertyBlock materialPropertyBlock = Block;
				materialPropertyBlock.Clear();
				itemIcon.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(_flashAmountProp, value);
				itemIcon.SetPropertyBlock(materialPropertyBlock);
			}
			if ((bool)slotTypeIconGroup)
			{
				slotTypeIconGroup.AlphaSelf = 1f - value;
			}
			if ((bool)slotTypeIconFilled)
			{
				slotTypeIconFilled.AlphaSelf = (EquippedItem ? 0f : value);
			}
		}
	}

	public ToolItem EquippedItem { get; private set; }

	public override ToolItem ItemData => EquippedItem;

	public ToolItemType Type => SlotInfo.Type;

	public bool IsLocked
	{
		get
		{
			if (Crest == null)
			{
				return false;
			}
			if (Crest.CrestData == null)
			{
				return false;
			}
			if (slotInfo.IsLocked)
			{
				return !SaveData.IsUnlocked;
			}
			return false;
		}
	}

	public ToolCrestsData.SlotData SaveData
	{
		get
		{
			if (getSavedDataOverride != null)
			{
				return getSavedDataOverride();
			}
			List<ToolCrestsData.SlotData> slots = PlayerData.instance.ToolEquips.GetData(Crest.name).Slots;
			if (slots == null || SlotIndex >= slots.Count)
			{
				return default(ToolCrestsData.SlotData);
			}
			return slots[SlotIndex];
		}
		private set
		{
			if (setSavedDataOverride != null)
			{
				setSavedDataOverride(value);
				return;
			}
			PlayerData instance = PlayerData.instance;
			ToolCrestsData.Data data = instance.ToolEquips.GetData(Crest.name);
			List<ToolCrestsData.SlotData> list = data.Slots;
			if (list == null)
			{
				list = (data.Slots = new List<ToolCrestsData.SlotData>());
				instance.ToolEquips.SetData(Crest.name, data);
			}
			while (list.Count < SlotIndex + 1)
			{
				list.Add(default(ToolCrestsData.SlotData));
			}
			list[SlotIndex] = value;
		}
	}

	public ToolCrest.SlotInfo SlotInfo
	{
		get
		{
			return slotInfo;
		}
		set
		{
			slotInfo = value;
			GetComponentsIfNeeded();
			if ((bool)itemIcon)
			{
				MaterialPropertyBlock materialPropertyBlock = Block;
				materialPropertyBlock.Clear();
				itemIcon.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetColor(_flashColorProp, manager.GetToolTypeColor(slotInfo.Type));
				itemIcon.SetPropertyBlock(materialPropertyBlock);
			}
			if (IsLocked && !spawnedUnlockBurstEffect && (bool)unlockBurstEffectPrefab)
			{
				PassColour passColour = UnityEngine.Object.Instantiate(unlockBurstEffectPrefab, base.transform);
				passColour.gameObject.SetActive(value: false);
				passColour.transform.localPosition = Vector3.zero;
				spawnedUnlockBurstEffect = passColour;
			}
			if ((bool)spawnedUnlockBurstEffect)
			{
				spawnedUnlockBurstEffect.SetColour(manager.GetToolTypeColor(Type));
			}
			if ((bool)slotAnimator && slotInfo.Type.IsAttackType())
			{
				slotAnimator.runtimeAnimatorController = attackAnimatorControllers[(int)slotInfo.AttackBinding];
			}
		}
	}

	protected override bool IsSeen
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	protected override bool IsAutoNavSelectable => wasVisible;

	public event Action OnSetEquipSaved;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)itemIcon)
		{
			itemIcon.sprite = null;
		}
		if ((bool)itemIconMask)
		{
			itemIconMask.sprite = null;
		}
		GetComponentsIfNeeded();
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if (!componentInParent)
		{
			return;
		}
		componentInParent.OnPaneEnd += delegate
		{
			if ((bool)spawnedUnlockBurstEffect)
			{
				spawnedUnlockBurstEffect.gameObject.SetActive(value: false);
			}
		};
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize(ref attackAnimatorControllers, typeof(AttackToolBinding));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		previousAnimId = -1;
		if (queuedAnimId != 0)
		{
			PlayAnim(queuedAnimId);
		}
		if (queuedSmallAnimId != 0)
		{
			PlayAnimSmall(queuedSmallAnimId);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		queuedAnimId = 0;
		queuedSmallAnimId = 0;
	}

	public void SetIsVisible(bool isVisible)
	{
		wasVisible = isVisible;
		EvaluateAutoNav();
	}

	protected override void Update()
	{
		base.Update();
		if (isPulsingColour)
		{
			pulseColourTimeElapsed += Time.unscaledDeltaTime;
			if (pulseColourTimeElapsed > unlockReadyColorPulseDuration)
			{
				pulseColourTimeElapsed %= unlockReadyColorPulseDuration;
			}
			float t = unlockReadyColourPulseCurve.Evaluate(pulseColourTimeElapsed / unlockReadyColorPulseDuration);
			Color color = Color.Lerp(pulseColourA, pulseColourB, t);
			float groupAlpha = Mathf.Lerp(0.5f, 1f, t);
			SetSlotColour(color, groupAlpha, fadeAlpha: false);
			if (isSelected)
			{
				UpdateDisplay();
			}
		}
	}

	private void GetComponentsIfNeeded()
	{
		if (!manager)
		{
			manager = GetComponentInParent<InventoryItemToolManager>();
			if ((bool)manager)
			{
				InventoryItemToolManager inventoryItemToolManager = manager;
				inventoryItemToolManager.OnToolRefresh = (Action<bool>)Delegate.Combine(inventoryItemToolManager.OnToolRefresh, new Action<bool>(UpdateSlotDisplay));
			}
			else
			{
				Debug.LogWarningFormat(this, "Tool Slot \"{0}\" couldn't find parent manager!", base.gameObject.name);
			}
		}
		if (!Crest)
		{
			Crest = GetComponentInParent<InventoryToolCrest>();
		}
		if (!crestList)
		{
			crestList = GetComponentInParent<InventoryToolCrestList>();
		}
	}

	public void SetCrestInfo(InventoryToolCrest crest, int slotIndex, Func<ToolCrestsData.SlotData> getSavedDataOverrideFunc = null, Action<ToolCrestsData.SlotData> setSavedDataOverrideAction = null)
	{
		Crest = crest;
		SlotIndex = slotIndex;
		getSavedDataOverride = getSavedDataOverrideFunc;
		setSavedDataOverride = setSavedDataOverrideAction;
	}

	public void PreOpenSlot()
	{
		if (!EquippedItem)
		{
			PlayAnim(_equipAnim);
		}
		isPreOpened = true;
	}

	public void SetEquipped(ToolItem toolItem, bool isManual, bool refreshTools)
	{
		GetComponentsIfNeeded();
		bool num = EquippedItem != toolItem;
		EquippedItem = toolItem;
		if (num)
		{
			ItemDataUpdated();
		}
		if (isManual)
		{
			if (this.OnSetEquipSaved != null)
			{
				this.OnSetEquipSaved();
			}
			if ((bool)slotAnimator)
			{
				if ((bool)toolItem)
				{
					if (!isPreOpened)
					{
						PlayAnim(_equipAnim);
					}
				}
				else
				{
					PlayAnim(_unequipAnim);
				}
				isPreOpened = false;
			}
			if (refreshTools)
			{
				manager.RefreshTools();
			}
		}
		else
		{
			PlaySlotStateAnims(IsLocked, manager.CanUnlockSlot, force: true);
		}
		RefreshIcon();
		UpdateDisplay();
	}

	public override bool Submit()
	{
		GetComponentsIfNeeded();
		if (!manager)
		{
			return false;
		}
		if (IsLocked)
		{
			if ((bool)EquippedItem)
			{
				manager.UnequipTool(EquippedItem, this);
				return true;
			}
			if (manager.CanUnlockSlot)
			{
				unlockHoldRoutine = StartCoroutine(UnlockHoldRoutine());
				UpdateSlotDisplay(isInstant: false);
				return true;
			}
			return false;
		}
		if (!manager.CanChangeEquips(Type, InventoryItemToolManager.CanChangeEquipsTypes.Regular))
		{
			return false;
		}
		if (manager.EquipState == InventoryItemToolManager.EquipStates.PlaceTool)
		{
			DoPlace();
			return true;
		}
		return base.Submit();
	}

	public override bool SubmitReleased()
	{
		if (TryCancelUnlockHold())
		{
			return true;
		}
		return base.SubmitReleased();
	}

	protected override bool DoPress()
	{
		switch (manager.EquipState)
		{
		case InventoryItemToolManager.EquipStates.None:
			if ((bool)EquippedItem)
			{
				manager.UnequipTool(EquippedItem, this);
			}
			else
			{
				manager.StartSelection(this);
			}
			return true;
		case InventoryItemToolManager.EquipStates.PlaceTool:
			DoPlace();
			return true;
		default:
			return false;
		}
	}

	private void DoPlace()
	{
		if (manager.IsHoldingTool)
		{
			manager.PlaceTool(this, isManual: true);
		}
	}

	public override bool Cancel()
	{
		GetComponentsIfNeeded();
		if (!manager)
		{
			return base.Cancel();
		}
		if (manager.ShowingToolMsg)
		{
			manager.HideToolEquipMsg();
			return false;
		}
		if (manager.IsHoldingTool)
		{
			manager.PlayMoveSound();
			manager.PlaceTool(null, isManual: false);
			return true;
		}
		return base.Cancel();
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		GetNextSelectableAndSlot(direction, out var nextSelectable, out var nextSlot);
		if ((bool)manager && manager.EquipState == InventoryItemToolManager.EquipStates.PlaceTool)
		{
			if (!nextSlot)
			{
				return GetSlotFromAutoNavGroup(direction, Type);
			}
			if (IsSlotInvalid(Type, nextSlot))
			{
				nextSlot = nextSlot.GetNextSlotOfType(direction, Type);
			}
			if (nextSlot == null)
			{
				nextSelectable = GetNextFallbackSelectable(direction);
				nextSlot = nextSelectable as InventoryToolCrestSlot;
				if (nextSlot == null)
				{
					return GetSlotFromAutoNavGroup(direction, Type);
				}
				if (nextSlot.Type != Type)
				{
					nextSlot = nextSlot.GetNextSlotOfType(direction, Type);
				}
			}
			if (!nextSlot)
			{
				return GetSlotFromAutoNavGroup(direction, Type);
			}
			return nextSlot;
		}
		return nextSelectable;
	}

	private InventoryItemSelectable GetSlotFromAutoNavGroup(InventoryItemManager.SelectionDirection direction, ToolItemType type)
	{
		return GetSelectableFromAutoNavGroup(direction, (InventoryToolCrestSlot slot) => !IsSlotInvalid(type, slot));
	}

	private InventoryToolCrestSlot GetNextSlotOfType(InventoryItemManager.SelectionDirection direction, ToolItemType type)
	{
		GetNextSelectableAndSlot(direction, out var _, out var nextSlot);
		if ((bool)nextSlot && IsSlotInvalid(type, nextSlot))
		{
			return nextSlot.GetNextSlotOfType(direction, type);
		}
		return nextSlot;
	}

	private void GetNextSelectableAndSlot(InventoryItemManager.SelectionDirection direction, out InventoryItemSelectable nextSelectable, out InventoryToolCrestSlot nextSlot)
	{
		nextSelectable = base.GetNextSelectable(direction);
		nextSlot = (nextSelectable ? (nextSelectable.Get(direction) as InventoryToolCrestSlot) : null);
	}

	private bool IsSlotInvalid(ToolItemType type, InventoryToolCrestSlot nextSlot)
	{
		if (nextSlot.Type == type)
		{
			if (nextSlot.IsLocked)
			{
				return !manager.CanUnlockSlot;
			}
			return false;
		}
		return true;
	}

	private void UpdateSlotDisplay(bool isInstant)
	{
		int frameCount = Time.frameCount;
		if (lastUpdate == frameCount && lastEquipState == manager.EquipState && isSelected == lastSelectState)
		{
			return;
		}
		lastEquipState = manager.EquipState;
		lastSelectState = isSelected;
		lastUpdate = frameCount;
		Color toolTypeColor = manager.GetToolTypeColor(Type);
		Color.RGBToHSV(toolTypeColor, out var H, out var S, out var V);
		Color color = Color.HSVToRGB(H, S * 0.4f, V);
		Color color2 = Color.HSVToRGB(H, 0f, V);
		bool flag;
		bool flag2;
		bool flag3;
		bool fadeAlpha;
		if ((bool)Crest)
		{
			flag = crestList.CurrentCrest == Crest;
			flag2 = IsLocked;
			flag3 = flag2 && manager.CanUnlockSlot;
			fadeAlpha = crestList.IsSetupComplete;
		}
		else
		{
			flag = true;
			flag2 = false;
			flag3 = false;
			fadeAlpha = true;
		}
		PlaySlotStateAnims(flag2, flag3, force: false);
		if (wasSelected)
		{
			_ = manager.EquipState == InventoryItemToolManager.EquipStates.SwitchCrest;
		}
		else
			_ = 0;
		bool flag4 = (wasSelected = isSelected && (wasSelected || manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest));
		float groupAlpha;
		Color color3;
		if (flag2)
		{
			if (flag3 && flag4)
			{
				groupAlpha = 1f;
			}
			else
			{
				InventoryItemToolManager.EquipStates equipState = manager.EquipState;
				groupAlpha = ((equipState == InventoryItemToolManager.EquipStates.PlaceTool || equipState == InventoryItemToolManager.EquipStates.SelectTool) ? 0.3f : 0.5f);
			}
			color3 = Color.white;
		}
		else if (manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest && flag)
		{
			groupAlpha = 1f;
			color3 = Color.white;
			switch (manager.EquipState)
			{
			case InventoryItemToolManager.EquipStates.PlaceTool:
				if ((bool)manager.PickedUpTool && manager.PickedUpTool.Type != Type)
				{
					groupAlpha = 0.3f;
					color3 = InvalidItemColor;
				}
				break;
			case InventoryItemToolManager.EquipStates.SelectTool:
				if ((bool)manager.SelectedSlot && manager.SelectedSlot != this)
				{
					groupAlpha = 0.3f;
					color3 = InvalidItemColor;
				}
				break;
			case InventoryItemToolManager.EquipStates.None:
				if ((bool)manager.HoveringTool && manager.HoveringTool.ToolType != Type)
				{
					groupAlpha = 0.3f;
				}
				break;
			}
		}
		else
		{
			groupAlpha = 1f;
			color3 = Color.white;
		}
		if (unlockHoldRoutine != null)
		{
			isPulsingColour = false;
			SetSlotColour(toolTypeColor, 1f, fadeAlpha);
		}
		else if (flag3 && flag4)
		{
			if (!isPulsingColour)
			{
				isPulsingColour = true;
				pulseColourA = color;
				pulseColourB = toolTypeColor;
				pulseColourTimeElapsed = 0f;
				SetSlotColour(pulseColourA, 0.5f, fadeAlpha);
			}
		}
		else
		{
			isPulsingColour = false;
			SetSlotColour(flag2 ? color2 : toolTypeColor, groupAlpha, fadeAlpha);
		}
		if ((bool)amountText)
		{
			if (flag && (bool)EquippedItem && EquippedItem.DisplayAmountText)
			{
				ToolItemsData.Data toolData = PlayerData.instance.GetToolData(EquippedItem.name);
				amountText.text = toolData.AmountLeft.ToString();
				amountText.color = color3;
				amountText.gameObject.SetActive(value: true);
			}
			else
			{
				amountText.gameObject.SetActive(value: false);
			}
		}
		(slotTypeIconGroup ? slotTypeIconGroup.transform : base.transform).localScale = (flag2 ? new Vector3(0.8f, 0.8f, 1f) : Vector3.one);
		if ((bool)itemIcon)
		{
			itemIcon.color = UpdateGetIconColour(itemIcon, color3, !isInstant);
		}
		RefreshIcon();
		UpdateDisplay();
	}

	protected override bool IsToolEquipped(ToolItem toolItem)
	{
		InventoryToolCrest crest = Crest;
		if (!crest)
		{
			return false;
		}
		if (!crestList.IsSwitchingCrests && crest == crestList.CurrentCrest)
		{
			return toolItem.IsEquippedHud;
		}
		return crest.GetEquippedToolSlot(toolItem);
	}

	private void RefreshIcon()
	{
		Sprite itemSprite = ItemSprite;
		if ((bool)itemIcon)
		{
			itemIcon.sprite = itemSprite;
		}
		if ((bool)itemIconMask)
		{
			itemIconMask.sprite = itemSprite;
		}
	}

	private void PlaySlotStateAnims(bool isLocked, bool isUnlockReady, bool force)
	{
		if (isLocked)
		{
			if (isUnlockReady)
			{
				PlayAnim((isSelected && manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest) ? _unlockReadySelectedAnim : _unlockReadyIdleAnim);
				PlayAnimSmall(_lockedAnim);
			}
			else
			{
				PlayAnim(_lockedAnim);
				PlayAnimSmall(_lockedAnim);
			}
			return;
		}
		if ((bool)EquippedItem)
		{
			if (force || previousAnimId != _equipAnim)
			{
				PlayAnim(_fullAnim);
			}
		}
		else if (force || previousAnimId != _unequipAnim)
		{
			PlayAnim(_emptyAnim);
		}
		PlayAnimSmall(_filledAnim);
	}

	private void PlayAnim(int animId)
	{
		if ((bool)slotAnimator && slotAnimator.isActiveAndEnabled)
		{
			slotAnimator.Play(animId);
		}
		else
		{
			queuedAnimId = animId;
		}
		previousAnimId = animId;
	}

	private void PlayAnimSmall(int animId)
	{
		if ((bool)slotFilledAnimator && slotFilledAnimator.isActiveAndEnabled)
		{
			slotFilledAnimator.Play(animId);
		}
		else
		{
			queuedSmallAnimId = animId;
		}
	}

	private void SetSlotColour(Color color, float groupAlpha, bool fadeAlpha)
	{
		if ((bool)slotTypeIcon)
		{
			slotTypeIcon.Color = color;
		}
		if ((bool)slotTypeIconFilled)
		{
			slotTypeIconFilled.BaseColor = color;
		}
		if ((bool)slotTypeGroup)
		{
			if (fadeAlpha)
			{
				slotTypeGroup.FadeTo(groupAlpha, 0.1f, null, isRealtime: true);
			}
			else
			{
				slotTypeGroup.AlphaSelf = groupAlpha;
			}
		}
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		if (!isSelected)
		{
			isSelected = true;
			UpdateSlotDisplay(isInstant: false);
		}
		base.Select(direction);
	}

	public override void Deselect()
	{
		if (isSelected)
		{
			isSelected = false;
			UpdateSlotDisplay(isInstant: false);
		}
		base.Deselect();
	}

	private IEnumerator UnlockHoldRoutine()
	{
		unlockHoldShakeTransform = (slotTypeGroup ? slotTypeGroup.transform : base.transform);
		unlockHoldInitialPosition = unlockHoldShakeTransform.localPosition;
		onUnlockHoldEnd = delegate
		{
			crestList.IsBlocked = false;
			unlockHoldShakeTransform.localPosition = unlockHoldInitialPosition;
			if ((bool)unlockHoldParticles)
			{
				unlockHoldParticles.StopParticleSystems();
			}
		};
		crestList.IsBlocked = true;
		if ((bool)unlockHoldParticles)
		{
			unlockHoldParticles.PlayParticleSystems();
		}
		InventoryItemCollectable unlockItem = manager.SlotUnlockItemDisplay;
		CrestSocketUnlockInventoryDescription unlockDesc = manager.SocketUnlockInventoryDescription;
		unlockDesc.StartConsume();
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f / 60f);
		double beforeWaitTime;
		for (float elapsed = 0f; elapsed < unlockHoldDuration; elapsed += (float)(Time.unscaledTimeAsDouble - beforeWaitTime))
		{
			float num = elapsed / unlockHoldDuration;
			unlockHoldShakeTransform.localPosition = unlockHoldInitialPosition + (Vector3)UnityEngine.Random.insideUnitCircle * unlockHoldShakeMagnitude.GetLerpedValue(num);
			unlockItem.SetConsumeShakeAmount(num, 1f);
			unlockDesc.SetConsumeShakeAmount(num);
			UpdateUnlockRumble(num);
			beforeWaitTime = Time.unscaledTimeAsDouble;
			yield return wait;
		}
		unlockItem.SetConsumeShakeAmount(0f, 1f);
		unlockDesc.SetConsumeShakeAmount(0f);
		unlockDesc.ConsumeCompleted();
		onUnlockHoldEnd();
		onUnlockHoldEnd = null;
		unlockHoldRoutine = null;
		ToolCrestsData.SlotData saveData = SaveData;
		saveData.IsUnlocked = true;
		SaveData = saveData;
		manager.SlotUnlockItem.Take(1, showCounter: false);
		if ((bool)spawnedUnlockBurstEffect)
		{
			spawnedUnlockBurstEffect.gameObject.SetActive(value: false);
			spawnedUnlockBurstEffect.gameObject.SetActive(value: true);
		}
		unlockItem.PlayConsumeEffect();
		StopUnlockRumble();
		PlayFinalShake();
		UpdateSlotDisplay(isInstant: false);
		manager.SetDisplay(this);
		manager.RefreshTools();
	}

	private bool TryCancelUnlockHold()
	{
		if (unlockHoldRoutine == null)
		{
			return false;
		}
		StopCoroutine(unlockHoldRoutine);
		unlockHoldRoutine = null;
		InventoryItemCollectable slotUnlockItemDisplay = manager.SlotUnlockItemDisplay;
		slotUnlockItemDisplay.SetConsumeShakeAmount(0f, 1f);
		slotUnlockItemDisplay.StopConsumeRumble();
		UpdateSlotDisplay(isInstant: false);
		onUnlockHoldEnd();
		onUnlockHoldEnd = null;
		manager.SocketUnlockInventoryDescription.CancelConsume();
		StopUnlockRumble();
		return true;
	}

	private void UpdateUnlockRumble(float strength)
	{
		if (consumeRumbleEmission == null)
		{
			consumeRumbleEmission = VibrationManager.PlayVibrationClipOneShot(unlockRumble, null, isLooping: true, "", isRealtime: true);
		}
		consumeRumbleEmission?.SetStrength(strength);
	}

	public void StopUnlockRumble()
	{
		consumeRumbleEmission?.Stop();
		consumeRumbleEmission = null;
	}

	private void PlayFinalShake()
	{
		VibrationManager.PlayVibrationClipOneShot(unlockShake, null, isLooping: false, "", isRealtime: true);
	}
}
