using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemMainQuest : InventoryItemQuest, IInventorySelectionParent
{
	[Space]
	[SerializeField]
	private InventoryItemQuest subQuestItemTemplate;

	[SerializeField]
	private GameObject subQuestsParent;

	[SerializeField]
	private GridLayoutGroup subQuestsLayout;

	[SerializeField]
	private LayoutGroup layoutGroup;

	private List<InventoryItemQuest> spawnedSubQuestItems;

	public event Action<InventoryItemSelectable> OnSubSelected;

	public override void SetQuest(BasicQuestBase newQuest, bool isInCompletedSection)
	{
		base.SetQuest(newQuest, isInCompletedSection);
		if (!newQuest)
		{
			return;
		}
		MainQuest mainQuest = newQuest as MainQuest;
		if (mainQuest == null)
		{
			Debug.LogErrorFormat(this, "{0} is not a MainQuest. InventoryItemMainQuest should only be used with MainQuests!", newQuest.name);
			return;
		}
		if (mainQuest.IsAnyAltTargetsComplete)
		{
			SetSubQuestsActive(value: false);
			return;
		}
		IReadOnlyList<SubQuest> subQuests = mainQuest.SubQuests;
		if (subQuests != null && subQuests.Count > 0)
		{
			subQuestItemTemplate.gameObject.SetActive(value: false);
			if (spawnedSubQuestItems == null)
			{
				spawnedSubQuestItems = new List<InventoryItemQuest>(subQuests.Count);
			}
			for (int num = subQuests.Count - spawnedSubQuestItems.Count; num > 0; num--)
			{
				InventoryItemQuest inventoryItemQuest = UnityEngine.Object.Instantiate(subQuestItemTemplate, subQuestItemTemplate.transform.parent);
				spawnedSubQuestItems.Add(inventoryItemQuest);
				inventoryItemQuest.OnSelected += SubSelected;
			}
			subQuestsLayout.constraintCount = ((subQuests.Count == 4) ? 2 : 3);
			int num2 = Mathf.CeilToInt((float)subQuests.Count / (float)subQuestsLayout.constraintCount);
			for (int i = 0; i < subQuests.Count; i++)
			{
				InventoryItemQuest inventoryItemQuest2 = spawnedSubQuestItems[i];
				inventoryItemQuest2.gameObject.SetActive(value: true);
				inventoryItemQuest2.SetQuest(subQuests[i].GetCurrent(), isInCompletedSection);
				inventoryItemQuest2.RegisterTextForUpdate();
				int num3 = i / subQuestsLayout.constraintCount;
				int index = i % subQuestsLayout.constraintCount;
				inventoryItemQuest2.Selectables[2] = ((i > 0) ? spawnedSubQuestItems[i - 1] : null);
				inventoryItemQuest2.Selectables[3] = ((i < spawnedSubQuestItems.Count - 1) ? spawnedSubQuestItems[i + 1] : null);
				if (num3 == 0)
				{
					inventoryItemQuest2.Selectables[0] = this;
				}
				else
				{
					inventoryItemQuest2.Selectables[0] = spawnedSubQuestItems[index];
				}
				if (num3 >= num2 - 1)
				{
					inventoryItemQuest2.Selectables[1] = null;
					continue;
				}
				int index2 = i + subQuestsLayout.constraintCount;
				inventoryItemQuest2.Selectables[1] = spawnedSubQuestItems[index2];
			}
			for (int j = subQuests.Count; j < spawnedSubQuestItems.Count; j++)
			{
				spawnedSubQuestItems[j].gameObject.SetActive(value: false);
			}
			SetSubQuestsActive(value: true);
		}
		else
		{
			SetSubQuestsActive(value: false);
		}
	}

	private void SetSubQuestsActive(bool value)
	{
		subQuestsParent.SetActive(value);
		layoutGroup.ForceUpdateLayoutNoCanvas();
	}

	private void SubSelected(InventoryItemSelectable selectable)
	{
		if (this.OnSubSelected != null)
		{
			this.OnSubSelected(selectable);
		}
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (direction == InventoryItemManager.SelectionDirection.Up && spawnedSubQuestItems != null)
		{
			foreach (InventoryItemQuest spawnedSubQuestItem in spawnedSubQuestItems)
			{
				if (Manager.CurrentSelected == spawnedSubQuestItem)
				{
					return base.Get(direction);
				}
			}
			InventoryItemQuest closestSub = GetClosestSub(fromRoot: false);
			if ((bool)closestSub)
			{
				return closestSub;
			}
		}
		return base.Get(direction);
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		if (direction == InventoryItemManager.SelectionDirection.Down && spawnedSubQuestItems != null)
		{
			InventoryItemQuest closestSub = GetClosestSub(fromRoot: true);
			if ((bool)closestSub)
			{
				return closestSub;
			}
		}
		return base.GetNextSelectable(direction);
	}

	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		if (direction == InventoryItemManager.SelectionDirection.Down)
		{
			return base.GetNextSelectable(direction.Value);
		}
		return null;
	}

	private InventoryItemQuest GetClosestSub(bool fromRoot)
	{
		if (spawnedSubQuestItems.Count == 0)
		{
			return null;
		}
		if (fromRoot && subQuestsLayout.constraintCount == 2)
		{
			return spawnedSubQuestItems[0];
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		InventoryItemQuest result = null;
		foreach (InventoryItemQuest spawnedSubQuestItem in spawnedSubQuestItems)
		{
			if (!spawnedSubQuestItem.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3 position = spawnedSubQuestItem.transform.position;
			if (!(position.y > num2))
			{
				num2 = position.y;
				float num3 = Mathf.Abs(position.x - base.transform.position.x);
				if (!(num3 > num))
				{
					num = num3;
					result = spawnedSubQuestItem;
				}
			}
		}
		return result;
	}
}
