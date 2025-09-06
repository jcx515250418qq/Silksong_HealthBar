using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class InventoryToolCrest : InventoryItemSelectableDirectional
{
	private static readonly Vector3 _slotScaleCurrent = new Vector3(1f, 1f, 1f);

	private static readonly Vector3 _slotScaleOther = new Vector3(0.7f, 0.7f, 1f);

	public static readonly Color DeselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Space]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[Space]
	[SerializeField]
	private NestedFadeGroupSpriteRenderer crestSprite;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer crestSilhouette;

	[SerializeField]
	private float lerpTime = 0.2f;

	[SerializeField]
	private float fadeTime = 0.2f;

	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private Vector2 deselectedScale = new Vector2(0.5f, 0.5f);

	private Vector2 defaultScale;

	[Space]
	[SerializeField]
	private SpriteRenderer crestGlowSprite;

	[SerializeField]
	private Animator crestSubmitAnimator;

	[SerializeField]
	private AudioEvent crestSubmitAudio;

	[Space]
	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private InventoryToolCrestSlot[] templateSlots;

	[SerializeField]
	private GameObject newIndicator;

	private Dictionary<ToolItemType, List<InventoryToolCrestSlot>> spawnedSlots;

	private Dictionary<ToolItemType, Queue<InventoryToolCrestSlot>> spawnedSlotsRemaining;

	private readonly List<ToolCrest.SlotInfo> activeSlotsData = new List<ToolCrest.SlotInfo>();

	private readonly List<InventoryToolCrestSlot> activeSlots = new List<InventoryToolCrestSlot>();

	private Coroutine transitionRoutine;

	private bool wasCurrentCrest;

	private Vector3 newIndicatorInitialScale;

	private bool isNew;

	private GameObject activeDisplayObject;

	private readonly Dictionary<GameObject, GameObject> spawnedDisplayObjects = new Dictionary<GameObject, GameObject>();

	private InventoryToolCrestList crestList;

	private InventoryItemToolManager manager;

	private Coroutine equipRoutine;

	private static readonly int _inertAnim = Animator.StringToHash("Inert");

	private static readonly int _burstAnim = Animator.StringToHash("Burst");

	private static ToolItemType[] TOOL_TYPES = (ToolItemType[])Enum.GetValues(typeof(ToolItemType));

	public override string DisplayName => displayName;

	public override string Description => description;

	public bool IsUnlocked
	{
		get
		{
			if ((bool)CrestData)
			{
				return CrestData.IsVisible;
			}
			return false;
		}
	}

	public bool IsHidden
	{
		get
		{
			if ((bool)CrestData)
			{
				return CrestData.IsHidden;
			}
			return false;
		}
	}

	public ToolCrest CrestData { get; private set; }

	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize(ref templateSlots, typeof(ToolItemType));
	}

	protected override void Awake()
	{
		base.Awake();
		manager = GetComponentInParent<InventoryItemToolManager>();
		if ((bool)newIndicator)
		{
			newIndicatorInitialScale = newIndicator.transform.localScale;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (equipRoutine != null)
		{
			StopCoroutine(equipRoutine);
		}
	}

	public void Setup(ToolCrest newCrestData)
	{
		CrestData = newCrestData;
		base.gameObject.name = (newCrestData ? newCrestData.name : "Spare Crest");
		if ((bool)crestSubmitAnimator && crestSubmitAnimator.isActiveAndEnabled)
		{
			crestSubmitAnimator.Play(_inertAnim);
		}
		if ((bool)newCrestData)
		{
			displayName = newCrestData.DisplayName;
			description = newCrestData.Description;
			if ((bool)crestSprite)
			{
				crestSprite.Sprite = newCrestData.CrestSprite;
			}
			if ((bool)crestSilhouette)
			{
				crestSilhouette.Sprite = newCrestData.CrestSilhouette;
			}
			if ((bool)crestGlowSprite)
			{
				crestGlowSprite.sprite = newCrestData.CrestGlow;
			}
			GameObject displayPrefab = newCrestData.DisplayPrefab;
			if ((bool)displayPrefab)
			{
				GameObject gameObject;
				if (spawnedDisplayObjects.TryGetValue(displayPrefab, out var value))
				{
					gameObject = value;
				}
				else
				{
					gameObject = UnityEngine.Object.Instantiate(displayPrefab, base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					spawnedDisplayObjects[displayPrefab] = gameObject;
				}
				if ((bool)activeDisplayObject && activeDisplayObject != gameObject)
				{
					activeDisplayObject.SetActive(value: false);
				}
				gameObject.SetActive(value: true);
				activeDisplayObject = gameObject;
			}
			ToolItemType[] tOOL_TYPES;
			if (spawnedSlots == null)
			{
				spawnedSlots = new Dictionary<ToolItemType, List<InventoryToolCrestSlot>>();
				tOOL_TYPES = TOOL_TYPES;
				foreach (ToolItemType key in tOOL_TYPES)
				{
					spawnedSlots[key] = new List<InventoryToolCrestSlot>();
				}
			}
			if (spawnedSlotsRemaining == null)
			{
				spawnedSlotsRemaining = new Dictionary<ToolItemType, Queue<InventoryToolCrestSlot>>();
				tOOL_TYPES = TOOL_TYPES;
				foreach (ToolItemType key2 in tOOL_TYPES)
				{
					spawnedSlotsRemaining[key2] = new Queue<InventoryToolCrestSlot>();
				}
			}
			tOOL_TYPES = TOOL_TYPES;
			foreach (ToolItemType type in tOOL_TYPES)
			{
				int num = newCrestData.Slots.Count((ToolCrest.SlotInfo slotData) => slotData.Type == type);
				int count = spawnedSlots[type].Count;
				int num2 = num - count;
				InventoryToolCrestSlot inventoryToolCrestSlot = templateSlots[(int)type];
				inventoryToolCrestSlot.gameObject.SetActive(value: false);
				while (num2 > 0)
				{
					InventoryToolCrestSlot inventoryToolCrestSlot2 = UnityEngine.Object.Instantiate(inventoryToolCrestSlot, inventoryToolCrestSlot.transform.parent);
					spawnedSlots[type].Add(inventoryToolCrestSlot2);
					inventoryToolCrestSlot2.OnSetEquipSaved += SaveEquips;
					num2--;
				}
				spawnedSlotsRemaining[type].Clear();
				foreach (InventoryToolCrestSlot item2 in spawnedSlots[type])
				{
					item2.gameObject.SetActive(value: false);
					spawnedSlotsRemaining[type].Enqueue(item2);
				}
			}
			activeSlots.Clear();
			activeSlotsData.Clear();
			for (int j = 0; j < newCrestData.Slots.Length; j++)
			{
				ToolCrest.SlotInfo item = newCrestData.Slots[j];
				InventoryToolCrestSlot inventoryToolCrestSlot3 = spawnedSlotsRemaining[item.Type].Dequeue();
				inventoryToolCrestSlot3.gameObject.SetActive(value: true);
				inventoryToolCrestSlot3.SetCrestInfo(this, j);
				inventoryToolCrestSlot3.transform.SetLocalPosition2D(item.Position);
				activeSlots.Add(inventoryToolCrestSlot3);
				activeSlotsData.Add(item);
			}
		}
		for (int k = 0; k < activeSlots.Count; k++)
		{
			InventoryToolCrestSlot inventoryToolCrestSlot4 = activeSlots[k];
			ToolCrest.SlotInfo slotInfo = activeSlotsData[k];
			inventoryToolCrestSlot4.Selectables[0] = GetActiveSlot(slotInfo.NavUpIndex);
			inventoryToolCrestSlot4.Selectables[1] = GetActiveSlot(slotInfo.NavDownIndex);
			inventoryToolCrestSlot4.Selectables[2] = GetActiveSlot(slotInfo.NavLeftIndex);
			inventoryToolCrestSlot4.Selectables[3] = GetActiveSlot(slotInfo.NavRightIndex);
			SetListSlotIndex(inventoryToolCrestSlot4.FallbackSelectables[0].Selectables, slotInfo.NavUpFallbackIndex);
			SetListSlotIndex(inventoryToolCrestSlot4.FallbackSelectables[1].Selectables, slotInfo.NavDownFallbackIndex);
			SetListSlotIndex(inventoryToolCrestSlot4.FallbackSelectables[2].Selectables, slotInfo.NavLeftFallbackIndex);
			SetListSlotIndex(inventoryToolCrestSlot4.FallbackSelectables[3].Selectables, slotInfo.NavRightFallbackIndex);
			inventoryToolCrestSlot4.SlotInfo = slotInfo;
		}
		foreach (InventoryToolCrestSlot activeSlot in activeSlots)
		{
			InventoryItemManager.PropagateSelectables(this, activeSlot);
		}
		isNew = !newCrestData.IsHidden && newCrestData.SaveData.DisplayNewIndicator;
	}

	private InventoryToolCrestSlot GetActiveSlot(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= activeSlots.Count)
		{
			Debug.LogError("Crest slot index out of range!", this);
			return null;
		}
		return activeSlots[index];
	}

	private void SetListSlotIndex(List<InventoryItemSelectable> selectables, int slotIndex)
	{
		selectables.Clear();
		if (slotIndex >= 0)
		{
			if (slotIndex >= activeSlots.Count)
			{
				Debug.LogError("Crest slot index out of range!", this);
			}
			else
			{
				selectables.Add(activeSlots[slotIndex]);
			}
		}
	}

	public void GetEquippedForSlots()
	{
		List<ToolItem> equippedToolsForCrest = ToolItemManager.GetEquippedToolsForCrest(base.gameObject.name);
		if (equippedToolsForCrest != null)
		{
			for (int i = 0; i < Mathf.Min(equippedToolsForCrest.Count, activeSlots.Count); i++)
			{
				activeSlots[i].SetEquipped(equippedToolsForCrest[i], isManual: false, refreshTools: false);
			}
		}
	}

	private void SaveEquips()
	{
		ToolItemManager.SetEquippedTools(base.gameObject.name, activeSlots.Select((InventoryToolCrestSlot slot) => (!slot.EquippedItem) ? null : slot.EquippedItem.name).ToList());
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (activeSlots.Count <= 0)
		{
			return null;
		}
		if (!direction.HasValue)
		{
			return activeSlots[0].Get(null);
		}
		if ((bool)manager && (bool)manager.CurrentSelected)
		{
			InventoryToolCrestSlot closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis(direction.Value, manager.CurrentSelected, activeSlots);
			if ((bool)closestOnAxis)
			{
				return closestOnAxis.Get(direction);
			}
		}
		return activeSlots[0].Get(direction);
	}

	public bool HasSlot(ToolItemType type)
	{
		foreach (InventoryToolCrestSlot activeSlot in activeSlots)
		{
			if (activeSlot.Type == type && !activeSlot.IsLocked)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnySlots()
	{
		foreach (InventoryToolCrestSlot activeSlot in activeSlots)
		{
			if (!activeSlot.IsLocked)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasSlot(InventoryToolCrestSlot otherSlot)
	{
		foreach (InventoryToolCrestSlot activeSlot in activeSlots)
		{
			if (activeSlot == otherSlot)
			{
				return true;
			}
		}
		return false;
	}

	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem toolItem)
	{
		return activeSlots.FirstOrDefault((InventoryToolCrestSlot slot) => slot.EquippedItem == toolItem);
	}

	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		return activeSlots;
	}

	public float Show(bool value, bool isInstant)
	{
		if (!fadeGroup)
		{
			fadeGroup = GetComponent<NestedFadeGroupBase>();
		}
		float result = (isInstant ? 0f : fadeTime);
		if (!fadeGroup)
		{
			return result;
		}
		return fadeGroup.FadeTo(value ? 1 : 0, result, null, isRealtime: true);
	}

	public void UpdateListDisplay(bool isInstant = false)
	{
		if (!crestList)
		{
			crestList = GetComponentInParent<InventoryToolCrestList>();
			defaultScale = base.transform.localScale;
		}
		bool flag = crestList.CurrentCrest == this;
		if (!IsUnlocked)
		{
			if (flag)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: true);
				}
			}
			else if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if ((bool)crestList)
		{
			Color newColor;
			switch (manager.EquipState)
			{
			case InventoryItemToolManager.EquipStates.None:
				newColor = DeselectedColor;
				break;
			case InventoryItemToolManager.EquipStates.SwitchCrest:
				newColor = ((crestList.CurrentCrest == this) ? Color.white : DeselectedColor);
				break;
			case InventoryItemToolManager.EquipStates.PlaceTool:
			case InventoryItemToolManager.EquipStates.SelectTool:
				newColor = InventoryToolCrestSlot.InvalidItemColor;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			Vector3 newScale = ((crestList.CurrentCrest == this) ? defaultScale : deselectedScale);
			newScale.z = 1f;
			if (transitionRoutine != null)
			{
				StopCoroutine(transitionRoutine);
			}
			if (!isInstant)
			{
				transitionRoutine = StartCoroutine(TransitionDisplayState(newColor, newScale, flag, isInstant: false));
			}
			else
			{
				TransitionDisplayState(newColor, newScale, flag, isInstant: true).MoveNext();
			}
		}
		if (isNew)
		{
			if (flag)
			{
				if (crestList.IsSwitchingCrests)
				{
					ToolCrestsData.Data saveData = CrestData.SaveData;
					saveData.DisplayNewIndicator = false;
					CrestData.SaveData = saveData;
					isNew = false;
					if ((bool)newIndicator && newIndicator.activeSelf)
					{
						newIndicator.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, 0f, dontTrack: false, isRealtime: true);
					}
				}
				else if ((bool)newIndicator)
				{
					newIndicator.SetActive(value: false);
				}
			}
			else if ((bool)newIndicator)
			{
				newIndicator.SetActive(value: true);
				newIndicator.transform.localScale = newIndicatorInitialScale;
			}
		}
		else if ((bool)newIndicator)
		{
			newIndicator.SetActive(value: false);
		}
	}

	private IEnumerator TransitionDisplayState(Color newColor, Vector3 newScale, bool isCurrentCrest, bool isInstant)
	{
		Color startColor = (crestSprite ? crestSprite.Color : Color.white);
		Vector3 startScale = base.transform.localScale;
		float oldFlashAmount = ((!wasCurrentCrest) ? 1 : 0);
		float newFlashAmount = ((!isCurrentCrest) ? 1 : 0);
		Vector3 slotScaleStart = (wasCurrentCrest ? _slotScaleCurrent : _slotScaleOther);
		Vector3 slotScaleEnd = (isCurrentCrest ? _slotScaleCurrent : _slotScaleOther);
		wasCurrentCrest = isCurrentCrest;
		if (!isInstant)
		{
			for (float elapsed = 0f; elapsed < lerpTime; elapsed += Time.unscaledDeltaTime)
			{
				SetLerpedValues(elapsed / lerpTime);
				yield return null;
			}
		}
		SetLerpedValues(1f);
		void SetLerpedValues(float time)
		{
			float num = Mathf.Lerp(oldFlashAmount, newFlashAmount, time);
			float a = 1f - num;
			if ((bool)crestSprite)
			{
				Color color = Color.Lerp(startColor, newColor, time);
				color.a = a;
				crestSprite.Color = color;
			}
			if ((bool)crestSilhouette)
			{
				crestSilhouette.AlphaSelf = num;
			}
			base.transform.localScale = Vector3.Lerp(startScale, newScale, time);
			foreach (InventoryToolCrestSlot activeSlot in activeSlots)
			{
				activeSlot.ItemFlashAmount = num;
				activeSlot.transform.localScale = Vector3.Lerp(slotScaleStart, slotScaleEnd, time);
			}
		}
	}

	public void DoEquip(Action onEquip)
	{
		if (!crestSubmitAnimator || !crestSubmitAnimator.isActiveAndEnabled)
		{
			onEquip();
		}
		else
		{
			equipRoutine = StartCoroutine(DoEquipAnim(onEquip));
		}
	}

	private IEnumerator DoEquipAnim(Action onEquip)
	{
		crestSubmitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		crestSubmitAnimator.Play(_burstAnim, 0, 0f);
		yield return null;
		if (crestSubmitAnimator.updateMode == AnimatorUpdateMode.UnscaledTime)
		{
			yield return new WaitForSecondsRealtime(crestSubmitAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		else
		{
			yield return new WaitForSeconds(crestSubmitAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		onEquip();
	}
}
