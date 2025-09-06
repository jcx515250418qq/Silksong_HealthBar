using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public abstract class QuestItemManager : InventoryItemListManager<InventoryItemQuest, BasicQuestBase>
{
	[SerializeField]
	private NestedFadeGroup descriptionGroup;

	[SerializeField]
	private TextMeshPro typeText;

	[SerializeField]
	private TextMeshPro locationText;

	[SerializeField]
	private QuestItemDescription questItemDescription;

	private float fontSize;

	private float paraSpacing;

	protected override void Awake()
	{
		base.Awake();
		fontSize = descriptionText.fontSize;
		paraSpacing = descriptionText.paragraphSpacing;
		ResetExtraDisplay();
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		ResetExtraDisplay();
	}

	private void ResetExtraDisplay()
	{
		if ((bool)descriptionGroup)
		{
			descriptionGroup.AlphaSelf = 0f;
		}
		if ((bool)questItemDescription)
		{
			questItemDescription.SetDisplay(null);
		}
		descriptionText.fontSize = fontSize;
		descriptionText.paragraphSpacing = paraSpacing;
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		InventoryItemQuest inventoryItemQuest = selectable as InventoryItemQuest;
		if ((bool)inventoryItemQuest)
		{
			BasicQuestBase quest = inventoryItemQuest.Quest;
			if (!quest)
			{
				return;
			}
			if (quest is FullQuestBase fullQuestBase)
			{
				if (fullQuestBase.OverrideFontSize.IsEnabled)
				{
					descriptionText.fontSize = fullQuestBase.OverrideFontSize.Value;
				}
				if (fullQuestBase.OverrideParagraphSpacing.IsEnabled)
				{
					descriptionText.paragraphSpacing = fullQuestBase.OverrideParagraphSpacing.Value;
				}
			}
			if ((bool)descriptionGroup)
			{
				descriptionGroup.AlphaSelf = 1f;
			}
			if ((bool)typeText)
			{
				if ((bool)quest.QuestType)
				{
					typeText.text = quest.QuestType.DisplayName;
					typeText.color = quest.QuestType.TextColor;
				}
				else
				{
					typeText.text = "NO TYPE ASSIGNED";
					typeText.color = Color.magenta;
				}
			}
			if ((bool)questItemDescription)
			{
				questItemDescription.SetDisplay(quest);
			}
			if ((bool)locationText)
			{
				string location = quest.Location;
				locationText.text = location;
				locationText.gameObject.SetActive(!string.IsNullOrWhiteSpace(location));
			}
		}
		else if ((bool)descriptionGroup)
		{
			descriptionGroup.AlphaSelf = 1f;
		}
	}

	public override bool MoveSelectionPage(SelectionDirection direction)
	{
		return false;
	}
}
