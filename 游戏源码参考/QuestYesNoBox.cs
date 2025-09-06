using System;
using System.Linq;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

public class QuestYesNoBox : YesNoBox
{
	[Space]
	[SerializeField]
	private InventoryItemQuest questDisplay;

	[SerializeField]
	private GameObject deliverGroup;

	[SerializeField]
	private SpriteRenderer deliverIcon;

	[SerializeField]
	private TMP_Text rewardAmountText;

	[Header("Dynamic Height")]
	[SerializeField]
	private TextMeshPro questNameText;

	[SerializeField]
	private LayoutElement layoutElement;

	[SerializeField]
	private float baseHeight = 2.3f;

	[SerializeField]
	private float heightPerLine = 0.6f;

	private static QuestYesNoBox _instance;

	protected override void Awake()
	{
		base.Awake();
		if (!_instance)
		{
			_instance = this;
		}
		pane.OnPaneStart += base.OnAppearing;
		pane.PaneOpenedAnimEnd += base.OnAppeared;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void UpdateHeight()
	{
		if ((bool)questNameText && (bool)layoutElement)
		{
			string text = questNameText.text;
			TMP_TextInfo textInfo = questNameText.GetTextInfo(text);
			float minHeight = baseHeight + heightPerLine * (float)textInfo.lineCount;
			layoutElement.minHeight = minHeight;
		}
	}

	public static void Open(Action yes, Action no, bool returnHud, FullQuestBase quest, bool beginQuest)
	{
		if (!_instance || !quest)
		{
			return;
		}
		_instance.questDisplay.SetQuest(quest, isInCompletedSection: false);
		_instance.UpdateHeight();
		if (quest.Targets.Count == 1)
		{
			DeliveryQuestItem deliveryQuestItem = quest.Targets.Select((FullQuestBase.QuestTarget target) => target.Counter).OfType<DeliveryQuestItem>().FirstOrDefault();
			if ((bool)deliveryQuestItem)
			{
				_instance.rewardAmountText.text = quest.RewardCount.ToString();
				_instance.deliverIcon.sprite = deliveryQuestItem.GetIcon(CollectableItem.ReadSource.GetPopup);
				_instance.deliverGroup.SetActive(value: true);
			}
			else
			{
				_instance.deliverGroup.SetActive(value: false);
			}
		}
		else
		{
			_instance.deliverGroup.SetActive(value: false);
		}
		_instance.InternalOpen(beginQuest ? new Action(NewYes) : yes, no, returnHud);
		void NewYes()
		{
			quest.BeginQuest(yes);
		}
	}

	public static void ForceClose()
	{
		if ((bool)_instance)
		{
			_instance.DoEnd();
		}
	}
}
