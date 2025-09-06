using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class AchievementListener : MonoBehaviour
{
	private GameManager gm;

	public SpriteRenderer icon;

	public TextMeshPro title;

	public TextMeshPro text;

	public PlayMakerFSM fsmToSendEvent;

	public string eventName;

	private void OnEnable()
	{
		if (!gm)
		{
			gm = GameManager.instance;
		}
		gm.achievementHandler.AwardAchievementEvent += CaptureAchievementEvent;
	}

	private void OnDisable()
	{
		gm.achievementHandler.AwardAchievementEvent -= CaptureAchievementEvent;
	}

	private void CaptureAchievementEvent(string achievementKey)
	{
		Achievement achievement = gm.achievementHandler.AchievementsList.FindAchievement(achievementKey);
		icon.sprite = achievement.Icon;
		title.text = Language.Get(achievement.TitleCell, "Achievements");
		text.text = Language.Get(achievement.DescriptionCell, "Achievements");
		if ((bool)fsmToSendEvent && !string.IsNullOrEmpty(eventName))
		{
			fsmToSendEvent.SendEvent(eventName);
		}
	}
}
