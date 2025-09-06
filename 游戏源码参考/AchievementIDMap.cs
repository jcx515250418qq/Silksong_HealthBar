using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AchievementIDMap", menuName = "Hollow Knight/Achievement ID Map", order = 1900)]
public class AchievementIDMap : ScriptableObject
{
	[Serializable]
	public class AchievementIDPair
	{
		[SerializeField]
		[FormerlySerializedAs("achievementId")]
		private string internalId;

		[SerializeField]
		[FormerlySerializedAs("trophyId")]
		private int serviceId;

		[SerializeField]
		private bool useCustomEvent;

		[SerializeField]
		private CustomEvent customEvent;

		public string InternalId => internalId;

		public int ServiceId => serviceId;

		public bool UseCustomEvent => useCustomEvent;

		public CustomEvent CustomEvent => customEvent;
	}

	[Serializable]
	public struct CustomEvent
	{
		public string statName;
	}

	[NamedArray("GetElementName")]
	[SerializeField]
	private AchievementIDPair[] pairs;

	private Dictionary<string, int> serviceIdsByInternalId;

	private Dictionary<string, AchievementIDPair> infoById;

	public int Count
	{
		get
		{
			if (pairs != null)
			{
				return pairs.Length;
			}
			return 0;
		}
	}

	public int? GetServiceIdForInternalId(string internalId)
	{
		if (serviceIdsByInternalId == null)
		{
			serviceIdsByInternalId = new Dictionary<string, int>();
			for (int i = 0; i < pairs.Length; i++)
			{
				AchievementIDPair achievementIDPair = pairs[i];
				serviceIdsByInternalId.Add(achievementIDPair.InternalId, achievementIDPair.ServiceId);
			}
		}
		if (!serviceIdsByInternalId.TryGetValue(internalId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool TryGetAchievementInformation(string internalId, out AchievementIDPair info)
	{
		if (infoById == null)
		{
			infoById = new Dictionary<string, AchievementIDPair>();
			for (int i = 0; i < pairs.Length; i++)
			{
				AchievementIDPair achievementIDPair = pairs[i];
				infoById.Add(achievementIDPair.InternalId, achievementIDPair);
			}
		}
		return infoById.TryGetValue(internalId, out info);
	}

	private string GetElementName(int index)
	{
		try
		{
			AchievementIDPair achievementIDPair = pairs[index];
			return string.Format("{0} : {1}{2}", achievementIDPair.ServiceId, achievementIDPair.InternalId, achievementIDPair.UseCustomEvent ? (" - " + achievementIDPair.CustomEvent.statName) : string.Empty);
		}
		catch (Exception)
		{
		}
		return string.Empty;
	}
}
