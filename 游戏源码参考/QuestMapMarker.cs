using System;
using GlobalEnums;
using UnityEngine;

public class QuestMapMarker : MapMarkerArrow, GameMapPinLayout.ILayoutHook
{
	private enum IsVisibleConditions
	{
		Active = 0,
		CanComplete = 1,
		ActiveSteelSoulSpot = 2
	}

	[SerializeField]
	private MapZone quickMapZone;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("collectableItem", false, false, false)]
	private BasicQuestBase quest;

	[SerializeField]
	[Conditional("collectableItem", false, false, false)]
	private IsVisibleConditions isVisibleCondition;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingTargetSceneName", true, true, false)]
	private string targetSceneName;

	[SerializeField]
	private CollectableItem collectableItem;

	private bool IsUsingTargetSceneName()
	{
		return isVisibleCondition == IsVisibleConditions.ActiveSteelSoulSpot;
	}

	protected override bool IsActive(bool isQuickMap, MapZone currentMapZone)
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		if (isQuickMap && quickMapZone != 0 && currentMapZone != quickMapZone)
		{
			return false;
		}
		if ((bool)collectableItem)
		{
			return collectableItem.CollectedAmount > 0;
		}
		if (quest == null)
		{
			return false;
		}
		if (!quest.IsMapMarkerVisible)
		{
			return false;
		}
		switch (isVisibleCondition)
		{
		case IsVisibleConditions.CanComplete:
			return !(quest is IQuestWithCompletion questWithCompletion) || questWithCompletion.CanComplete;
		case IsVisibleConditions.Active:
			return true;
		case IsVisibleConditions.ActiveSteelSoulSpot:
		{
			SteelSoulQuestSpot.Spot[] steelQuestSpots = PlayerData.instance.SteelQuestSpots;
			if (steelQuestSpots == null)
			{
				Debug.LogError("Steel Quest Spots array is null!", this);
				return false;
			}
			SteelSoulQuestSpot.Spot[] array = steelQuestSpots;
			foreach (SteelSoulQuestSpot.Spot spot in array)
			{
				if (spot == null)
				{
					Debug.LogError("Quest spot is null!", this);
				}
				else if (!spot.IsSeen && spot.SceneName == targetSceneName)
				{
					return true;
				}
			}
			return false;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void LayoutFinished()
	{
		SetPosition(base.transform.localPosition);
	}
}
