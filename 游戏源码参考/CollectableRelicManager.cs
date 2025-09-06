using System.Collections.Generic;
using UnityEngine;

public class CollectableRelicManager : ManagerSingleton<CollectableRelicManager>
{
	[SerializeField]
	private CollectableRelicList masterList;

	public static CollectableRelicsData.Data GetRelicData(CollectableRelic relic)
	{
		return PlayerData.instance.Relics.GetData(relic.name);
	}

	public static void SetRelicData(CollectableRelic relic, CollectableRelicsData.Data data)
	{
		PlayerData.instance.Relics.SetData(relic.name, data);
		CollectableItemManager.IncrementVersion();
	}

	public static CollectableRelic GetRelic(string relicName)
	{
		return ManagerSingleton<CollectableRelicManager>.Instance.masterList.GetByName(relicName);
	}

	public static IReadOnlyList<CollectableRelic> GetAllRelics()
	{
		return ManagerSingleton<CollectableRelicManager>.Instance.masterList;
	}
}
