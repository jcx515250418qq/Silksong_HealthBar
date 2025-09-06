using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementsList : ScriptableObject
{
	[NamedArray("GetElementName")]
	[SerializeField]
	private List<Achievement> achievements;

	public IReadOnlyList<Achievement> Achievements => achievements;

	public Achievement FindAchievement(string key)
	{
		foreach (Achievement achievement in achievements)
		{
			if (string.Equals(achievement.PlatformKey, key))
			{
				return achievement;
			}
		}
		return null;
	}

	public bool AchievementExists(string key)
	{
		foreach (Achievement achievement in achievements)
		{
			if (string.Equals(achievement.PlatformKey, key))
			{
				return true;
			}
		}
		return false;
	}

	private string GetElementName(int index)
	{
		try
		{
			Achievement achievement = achievements[index];
			return $"{index + 1}: {achievement.PlatformKey} : ({achievement.Type})";
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return $"Element {index}";
	}
}
