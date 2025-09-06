using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class MenuAchievementsList : MonoBehaviour, IRecyclableScrollRectDataSource
{
	[SerializeField]
	private RecyclableScrollRect scrollRect;

	private IReadOnlyList<Achievement> achievements;

	public void PreInit()
	{
		GameManager instance = GameManager.instance;
		achievements = instance.achievementHandler.AchievementsList.Achievements;
		scrollRect.DataSource = this;
		scrollRect.PreInit();
	}

	private void OnEnable()
	{
		if (achievements == null)
		{
			Debug.LogError("ERROR: Can't load achievements if they haven't been fetched yet!");
		}
		else
		{
			scrollRect.ReloadData();
		}
	}

	public int GetItemCount()
	{
		return achievements.Count;
	}

	public void SetCell(ICell cell, int index)
	{
		((MenuAchievement)cell).ConfigureCell(achievements[index], index);
	}
}
