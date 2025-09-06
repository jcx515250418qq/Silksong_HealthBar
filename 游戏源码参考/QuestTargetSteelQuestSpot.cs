using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Steel Quest Spot")]
public class QuestTargetSteelQuestSpot : QuestTargetCounter
{
	[Serializable]
	private struct SpotInfo
	{
		public string SceneName;
	}

	[SerializeField]
	private SpotInfo[] spotInfos;

	[Space]
	[SerializeField]
	private Sprite icon;

	public override bool CanGetMore()
	{
		SteelSoulQuestSpot.Spot spot = GetSpot();
		if (spot == null)
		{
			return true;
		}
		return !spot.IsSeen;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		SteelSoulQuestSpot.Spot[] steelQuestSpots = PlayerData.instance.SteelQuestSpots;
		if (steelQuestSpots == null)
		{
			return 0;
		}
		int num = 0;
		SteelSoulQuestSpot.Spot[] array = steelQuestSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsSeen)
			{
				num++;
			}
		}
		return num;
	}

	public override string GetPopupName()
	{
		return string.Empty;
	}

	public override Sprite GetPopupIcon()
	{
		return icon;
	}

	private static SteelSoulQuestSpot.Spot GetSpot()
	{
		PlayerData instance = PlayerData.instance;
		SteelSoulQuestSpot.Spot[] steelQuestSpots = instance.SteelQuestSpots;
		if (steelQuestSpots == null)
		{
			return null;
		}
		string sceneNameString = GameManager.instance.GetSceneNameString();
		for (int i = 0; i < steelQuestSpots.Length; i++)
		{
			if (!(steelQuestSpots[i].SceneName != sceneNameString))
			{
				return instance.SteelQuestSpots[i];
			}
		}
		return null;
	}
}
