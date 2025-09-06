using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Type")]
public class QuestType : ScriptableObject
{
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString displayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite icon;

	[SerializeField]
	private Sprite canCompleteIcon;

	[SerializeField]
	private Color textColor;

	[Header("Fullscreen Prompt")]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite largeIcon;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite largeIconGlow;

	[Header("Properties")]
	[SerializeField]
	private bool isDonateType;

	[SerializeField]
	[PlayerDataField(typeof(List<string>), false)]
	private string removeQuestFromListOnComplete;

	public string DisplayName
	{
		get
		{
			if (!displayName.IsEmpty)
			{
				return displayName;
			}
			return string.Empty;
		}
	}

	public Sprite Icon => icon;

	public Sprite CanCompleteIcon => canCompleteIcon;

	public Color TextColor => textColor;

	public Sprite LargeIcon => largeIcon;

	public Sprite LargeIconGlow => largeIconGlow;

	public bool IsDonateType => isDonateType;

	public static QuestType Create(LocalisedString displayName, Sprite icon, Color textColor, Sprite largeIcon, Sprite largeIconGlow, Sprite iconGlow = null)
	{
		QuestType questType = ScriptableObject.CreateInstance<QuestType>();
		questType.name = displayName;
		questType.displayName = displayName;
		questType.icon = icon;
		questType.canCompleteIcon = (iconGlow ? iconGlow : icon);
		questType.textColor = textColor;
		questType.largeIcon = largeIcon;
		questType.largeIconGlow = largeIconGlow;
		return questType;
	}

	public void OnQuestCompleted(FullQuestBase quest)
	{
		if (!string.IsNullOrEmpty(removeQuestFromListOnComplete))
		{
			PlayerData.instance.GetVariable<List<string>>(removeQuestFromListOnComplete)?.Remove(quest.name);
		}
	}
}
