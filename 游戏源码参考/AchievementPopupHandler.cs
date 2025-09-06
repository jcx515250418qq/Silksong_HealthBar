using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

public class AchievementPopupHandler : MonoBehaviour
{
	public static AchievementPopupHandler Instance;

	public AchievementPopup template;

	private List<AchievementPopup> popups = new List<AchievementPopup>();

	public bool reverseOrder;

	private AchievementHandler achievements;

	private AchievementPopup lastPopup;

	private void Awake()
	{
		Instance = this;
		if ((bool)template)
		{
			popups.Add(template);
			template.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		achievements = GameManager.instance.achievementHandler;
	}

	public void Setup(AchievementHandler handler)
	{
		achievements = handler;
		if (!Platform.Current.HasNativeAchievementsDialog)
		{
			achievements.AwardAchievementEvent += HandleAchievementEvent;
		}
	}

	private void OnDestroy()
	{
		if (!Platform.Current.HasNativeAchievementsDialog)
		{
			achievements.AwardAchievementEvent -= HandleAchievementEvent;
		}
	}

	private void HandleAchievementEvent(string key)
	{
		Achievement achievement = achievements.AchievementsList.FindAchievement(key);
		Sprite icon = achievement.Icon;
		string text = Language.Get(achievement.TitleCell, "Achievements");
		string description = Language.Get(achievement.DescriptionCell, "Achievements");
		AchievementPopup pooledPopup = GetPooledPopup();
		if ((bool)pooledPopup)
		{
			pooledPopup.Setup(icon, text, description);
			lastPopup = pooledPopup;
			pooledPopup.OnFinish += DisableAll;
		}
		else
		{
			Debug.LogError("Could not get achievement popup!");
		}
	}

	private AchievementPopup GetPooledPopup()
	{
		AchievementPopup achievementPopup = null;
		foreach (AchievementPopup popup in popups)
		{
			if (!popup.gameObject.activeSelf)
			{
				achievementPopup = popup;
				break;
			}
		}
		if (achievementPopup == null && (bool)template)
		{
			achievementPopup = Object.Instantiate(template.gameObject, template.transform.parent).GetComponent<AchievementPopup>();
			popups.Add(achievementPopup);
		}
		if (reverseOrder)
		{
			achievementPopup.transform.SetAsFirstSibling();
		}
		else
		{
			achievementPopup.transform.SetAsLastSibling();
		}
		achievementPopup.gameObject.SetActive(value: true);
		return achievementPopup;
	}

	private void DisableAll(AchievementPopup sender)
	{
		sender.OnFinish -= DisableAll;
		if (!(sender == lastPopup))
		{
			return;
		}
		foreach (AchievementPopup popup in popups)
		{
			popup.gameObject.SetActive(value: false);
		}
	}
}
