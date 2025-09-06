using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Quest Display)")]
public class CollectableItemQuestDisplay : CollectableItem
{
	private enum CountMappings
	{
		Direct = 0,
		Percentage = 1
	}

	[Serializable]
	private struct ItemState
	{
		public LocalisedString DisplayName;

		public LocalisedString Description;

		public Sprite Icon;
	}

	[Space]
	[SerializeField]
	private Quest quest;

	[SerializeField]
	private CountMappings countMapping;

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString pickupDisplayName;

	[SerializeField]
	private ItemState[] states;

	[SerializeField]
	private bool displayCollectedAmount;

	public override bool DisplayAmount => displayCollectedAmount;

	public override int CollectedAmount
	{
		get
		{
			if ((bool)quest && quest.IsAccepted && !quest.IsCompleted)
			{
				if (!displayCollectedAmount)
				{
					return 1;
				}
				return quest.Counters.FirstOrDefault();
			}
			return 0;
		}
	}

	public override string GetDisplayName(ReadSource readSource)
	{
		if ((readSource == ReadSource.GetPopup || readSource == ReadSource.TakePopup) && !pickupDisplayName.IsEmpty)
		{
			return pickupDisplayName;
		}
		return GetCurrentState().DisplayName;
	}

	public override string GetDescription(ReadSource readSource)
	{
		return GetCurrentState().Description;
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		return GetCurrentState().Icon;
	}

	private ItemState GetCurrentState()
	{
		if (!quest)
		{
			return default(ItemState);
		}
		(FullQuestBase.QuestTarget target, int count) tuple = quest.TargetsAndCounters.FirstOrDefault();
		int num = tuple.count;
		int count = tuple.target.Count;
		int num2 = states.Length - 1;
		if (countMapping == CountMappings.Percentage)
		{
			float num3 = (float)num2 * ((float)num / (float)count);
			num = ((num3 > 0f && num3 < 1f) ? 1 : Mathf.FloorToInt(num3));
		}
		if (num > num2)
		{
			num = num2;
		}
		return states[num];
	}

	public override void SetupExtraDescription(GameObject obj)
	{
		if ((bool)quest)
		{
			QuestItemDescription component = obj.GetComponent<QuestItemDescription>();
			if ((bool)component)
			{
				component.SetDisplay(quest);
			}
		}
	}

	public override bool IsAtMax()
	{
		if (!quest)
		{
			return base.IsAtMax();
		}
		(FullQuestBase.QuestTarget, int) tuple = quest.TargetsAndCounters.FirstOrDefault();
		return tuple.Item2 >= tuple.Item1.Count;
	}

	protected override void AddAmount(int amount)
	{
		if ((bool)quest)
		{
			for (int i = 0; i < amount; i++)
			{
				quest.IncrementQuestCounter();
			}
		}
	}
}
