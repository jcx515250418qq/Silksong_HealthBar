using System;
using System.Linq;
using TMProOld;
using UnityEngine;

public class QuestIconDisplay : MonoBehaviour
{
	public enum IconSizes
	{
		Large = 0,
		Small = 1
	}

	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private IconSizes iconSize;

	[SerializeField]
	private SpriteRenderer[] glows;

	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private TMP_Text typeText;

	[Space]
	[SerializeField]
	private SpriteRenderer targetCounterIcon;

	[SerializeField]
	private TMP_Text targetCountText;

	private bool hasWoken;

	private Vector3 counterIconScale;

	private void Awake()
	{
		if (!hasWoken)
		{
			hasWoken = true;
			if ((bool)targetCounterIcon)
			{
				counterIconScale = targetCounterIcon.transform.localScale;
			}
		}
	}

	public void SetQuest(FullQuestBase quest)
	{
		Awake();
		QuestType questType = quest.QuestType;
		switch (iconSize)
		{
		case IconSizes.Large:
		{
			if ((bool)icon)
			{
				icon.sprite = questType.LargeIcon;
			}
			SpriteRenderer[] array = glows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = questType.LargeIconGlow;
			}
			break;
		}
		case IconSizes.Small:
		{
			if ((bool)icon)
			{
				icon.sprite = questType.Icon;
			}
			SpriteRenderer[] array = glows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = questType.CanCompleteIcon;
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		if ((bool)nameText)
		{
			nameText.text = quest.DisplayName;
		}
		if ((bool)typeText)
		{
			typeText.text = questType.DisplayName;
			typeText.color = questType.TextColor;
		}
		if ((bool)targetCounterIcon)
		{
			targetCounterIcon.sprite = quest.GetCounterSpriteOverride(quest.Targets.FirstOrDefault(), 0);
			targetCounterIcon.transform.localScale = counterIconScale.MultiplyElements(new Vector3(quest.CounterIconScale, quest.CounterIconScale, 1f));
		}
		if ((bool)targetCountText)
		{
			int count = quest.Targets.FirstOrDefault().Count;
			targetCountText.text = count.ToString();
		}
	}
}
