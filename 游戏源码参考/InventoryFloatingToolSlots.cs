using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryFloatingToolSlots : MonoBehaviour
{
	[Serializable]
	private class Slot
	{
		public InventoryToolCrestSlot SlotObject;

		public ToolItemType Type;

		public string Id;

		public GameObject CursedSlot;
	}

	[Serializable]
	private class Config
	{
		public Slot[] Slots;

		public GameObject[] Brackets;

		public Vector2 PositionOffset;

		public PlayerDataTest Condition;
	}

	[SerializeField]
	private Config[] configs;

	[SerializeField]
	private NestedFadeGroupBase bracketsFader;

	private Config currentConfig;

	private Vector2 initialPos;

	private void Awake()
	{
		initialPos = base.transform.localPosition;
		InventoryPane componentInParent = GetComponentInParent<InventoryPane>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPrePaneStart += Evaluate;
		}
		Evaluate();
	}

	private void Evaluate()
	{
		currentConfig = null;
		Config[] array = configs;
		Slot[] slots;
		foreach (Config config in array)
		{
			slots = config.Slots;
			for (int j = 0; j < slots.Length; j++)
			{
				slots[j].SlotObject.OnSetEquipSaved -= SaveEquips;
			}
			if (config.Condition.IsFulfilled)
			{
				currentConfig = config;
			}
		}
		bool isEquipped = Gameplay.CursedCrest.IsEquipped;
		if (currentConfig == null || isEquipped)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		base.transform.SetLocalPosition2D(initialPos + currentConfig.PositionOffset.MultiplyElements((Vector2)base.transform.localScale));
		array = configs;
		GameObject[] brackets;
		foreach (Config config2 in array)
		{
			brackets = config2.Brackets;
			for (int j = 0; j < brackets.Length; j++)
			{
				brackets[j].SetActive(value: false);
			}
			slots = config2.Slots;
			foreach (Slot obj in slots)
			{
				obj.SlotObject.SetIsVisible(isVisible: false);
				obj.SlotObject.gameObject.SetActive(value: false);
				obj.CursedSlot.SetActive(value: false);
			}
		}
		brackets = currentConfig.Brackets;
		for (int i = 0; i < brackets.Length; i++)
		{
			brackets[i].SetActive(value: true);
		}
		slots = currentConfig.Slots;
		foreach (Slot slot in slots)
		{
			slot.SlotObject.gameObject.SetActive(value: true);
			slot.SlotObject.SetIsVisible(isVisible: true);
			slot.SlotObject.ItemFlashAmount = 0f;
			slot.SlotObject.SlotInfo = new ToolCrest.SlotInfo
			{
				Type = slot.Type
			};
			slot.SlotObject.SetCrestInfo(null, -1, () => PlayerData.instance.ExtraToolEquips.GetData(slot.Id), delegate(ToolCrestsData.SlotData data)
			{
				PlayerData.instance.ExtraToolEquips.SetData(slot.Id, data);
			});
			string equippedTool = PlayerData.instance.ExtraToolEquips.GetData(slot.Id).EquippedTool;
			ToolItem toolItem = (string.IsNullOrEmpty(equippedTool) ? null : ToolItemManager.GetToolByName(equippedTool));
			slot.SlotObject.SetEquipped(toolItem, isManual: false, refreshTools: false);
			slot.SlotObject.OnSetEquipSaved += SaveEquips;
		}
	}

	private void SaveEquips()
	{
		if (currentConfig != null)
		{
			Slot[] slots = currentConfig.Slots;
			foreach (Slot slot in slots)
			{
				ToolItemManager.SetExtraEquippedTool(slot.Id, slot.SlotObject.EquippedItem);
			}
		}
	}

	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem toolItem)
	{
		if (currentConfig == null || !toolItem)
		{
			return null;
		}
		return currentConfig.Slots.Select((Slot slot) => slot.SlotObject).FirstOrDefault((InventoryToolCrestSlot slot) => slot.EquippedItem == toolItem);
	}

	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		if (currentConfig == null)
		{
			return Enumerable.Empty<InventoryToolCrestSlot>();
		}
		return currentConfig.Slots.Select((Slot slot) => slot.SlotObject);
	}

	public void SetInEquipMode(bool value)
	{
		bracketsFader.FadeTo(value ? InventoryToolCrestSlot.InvalidItemColor.r : InventoryToolCrest.DeselectedColor.r, 0.15f, null, isRealtime: true);
	}
}
