using System;
using GlobalEnums;
using PolyAndCode.UI;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class MenuAchievement : MonoBehaviour, ICell
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Sprite hiddenIconSprite;

	[SerializeField]
	private Text title;

	[SerializeField]
	private Text text;

	private Achievement achievement;

	private GameManager gm;

	private void OnEnable()
	{
		gm = GameManager.instance;
		if ((bool)gm)
		{
			gm.RefreshLanguageText += Refresh;
		}
		Refresh();
	}

	private void OnDisable()
	{
		if (!(gm == null))
		{
			gm.RefreshLanguageText -= Refresh;
			gm = null;
		}
	}

	public void ConfigureCell(Achievement ach, int cellIndex)
	{
		achievement = ach;
		Refresh();
	}

	public void Refresh()
	{
		if (!gm || achievement == null)
		{
			return;
		}
		try
		{
			if (gm.IsAchievementAwarded(achievement.PlatformKey))
			{
				icon.sprite = achievement.Icon;
				icon.color = Color.white;
				title.text = Language.Get(achievement.TitleCell, "Achievements");
				text.text = Language.Get(achievement.DescriptionCell, "Achievements");
			}
			else if (achievement.Type == AchievementType.Normal)
			{
				icon.sprite = achievement.Icon;
				icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				title.text = Language.Get(achievement.TitleCell, "Achievements");
				text.text = Language.Get(achievement.DescriptionCell, "Achievements");
			}
			else
			{
				icon.sprite = hiddenIconSprite;
				icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				title.text = Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
				text.text = Language.Get((Application.platform == RuntimePlatform.Switch) ? "HIDDEN_ACHIEVEMENT_ALT" : "HIDDEN_ACHIEVEMENT", "Achievements");
			}
		}
		catch (Exception)
		{
			if (achievement.Type == AchievementType.Normal)
			{
				icon.sprite = achievement.Icon;
				icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				title.text = Language.Get(achievement.TitleCell, "Achievements");
				text.text = Language.Get(achievement.DescriptionCell, "Achievements");
			}
			else
			{
				icon.sprite = hiddenIconSprite;
				title.text = Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
				text.text = Language.Get("HIDDEN_ACHIEVEMENT", "Achievements");
			}
		}
	}
}
