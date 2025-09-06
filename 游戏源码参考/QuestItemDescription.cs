using System;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;

public class QuestItemDescription : MonoBehaviour
{
	[Serializable]
	private struct CounterMaterial
	{
		public Material Material;
	}

	[SerializeField]
	private GameObject[] bottomSection;

	[Space]
	[SerializeField]
	private IconCounter rangeDisplay;

	[SerializeField]
	private QuestItemDescriptionText textDisplayTemplate;

	[SerializeField]
	private Transform neededTextGroup;

	[SerializeField]
	private float neededTextGroupOffsetPerItemY;

	[SerializeField]
	private GameObject rewardGroup;

	[SerializeField]
	private SpriteRenderer rewardIcon;

	[SerializeField]
	[ArrayForEnum(typeof(FullQuestBase.IconTypes))]
	private CounterMaterial[] counterMaterials;

	[Space]
	[SerializeField]
	private GameObject progressBarParent;

	[SerializeField]
	private ImageSlider progressBarSlider;

	[SerializeField]
	private TMP_Text progressBarText;

	[Space]
	[SerializeField]
	private GameObject donateCostGroup;

	[SerializeField]
	private SpriteRenderer donateCostIcon;

	[SerializeField]
	private TMP_Text donateCostText;

	private bool hasAwoken;

	private string initialProgressBarText;

	private Vector3 donateCostIconScale;

	private Vector2 initialRangeIconOffset;

	private List<(FullQuestBase.QuestTarget target, int count)> targetsList;

	private List<QuestItemDescriptionText> spawnedTextDisplays;

	private MaterialPropertyBlock block;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref counterMaterials, typeof(FullQuestBase.IconTypes));
	}

	private void Awake()
	{
		OnValidate();
		if ((bool)progressBarText)
		{
			initialProgressBarText = progressBarText.text;
		}
		if ((bool)donateCostIcon)
		{
			donateCostIconScale = donateCostIcon.transform.localScale;
		}
		if ((bool)rangeDisplay)
		{
			initialRangeIconOffset = rangeDisplay.MaxItemOffset;
		}
		hasAwoken = true;
	}

	public void SetDisplay(BasicQuestBase quest)
	{
		if (!hasAwoken)
		{
			Awake();
		}
		bottomSection.SetAllActive(value: false);
		if ((bool)rangeDisplay)
		{
			rangeDisplay.MaxValue = 0;
		}
		if ((bool)rewardGroup)
		{
			rewardGroup.SetActive(value: false);
		}
		if (spawnedTextDisplays != null)
		{
			foreach (QuestItemDescriptionText spawnedTextDisplay in spawnedTextDisplays)
			{
				spawnedTextDisplay.gameObject.SetActive(value: false);
			}
		}
		else if ((bool)textDisplayTemplate)
		{
			textDisplayTemplate.gameObject.SetActive(value: false);
		}
		if ((bool)donateCostGroup)
		{
			donateCostGroup.SetActive(value: false);
		}
		if ((bool)progressBarParent)
		{
			progressBarParent.SetActive(value: false);
		}
		targetsList?.Clear();
		FullQuestBase fullQuest = quest as FullQuestBase;
		if (!fullQuest || fullQuest.IsCompleted)
		{
			return;
		}
		bool flag = false;
		bottomSection.SetAllActive(value: true);
		switch (fullQuest.DescCounterType)
		{
		case FullQuestBase.DescCounterTypes.Icons:
		{
			if (!rangeDisplay)
			{
				break;
			}
			if (targetsList == null)
			{
				targetsList = new List<(FullQuestBase.QuestTarget, int)>(fullQuest.TargetsAndCounters);
			}
			else
			{
				targetsList.AddRange(fullQuest.TargetsAndCounters);
			}
			int num3 = 0;
			int num4 = 0;
			foreach (var targets in targetsList)
			{
				FullQuestBase.QuestTarget item3 = targets.target;
				int item4 = targets.count;
				num4 += fullQuest.GetCollectedCountOverride(item3, item4);
				num3 += item3.Count;
			}
			rangeDisplay.MaxValue = num3;
			rangeDisplay.CurrentValue = num4;
			rangeDisplay.MaxItemOffset = initialRangeIconOffset + fullQuest.CounterIconPadding;
			if (targetsList.Count > 0)
			{
				flag = true;
			}
			if (targetsList.Count <= 1)
			{
				rangeDisplay.SetFilledOverride(null);
			}
			else
			{
				rangeDisplay.SetFilledOverride(delegate(int index)
				{
					int index2 = FindTargetIndex(index);
					(FullQuestBase.QuestTarget, int) tuple = targetsList[index2];
					return tuple.Item2 >= tuple.Item1.Count;
				});
			}
			rangeDisplay.SetItemSprites(delegate(int index)
			{
				int index3 = FindTargetIndex(index);
				return fullQuest.GetCounterSpriteOverride(targetsList[index3].target, index);
			}, fullQuest.CounterIconScale);
			break;
		}
		case FullQuestBase.DescCounterTypes.Text:
		{
			if (!textDisplayTemplate)
			{
				break;
			}
			if (targetsList == null)
			{
				targetsList = new List<(FullQuestBase.QuestTarget, int)>(fullQuest.TargetsAndCounters);
			}
			else
			{
				targetsList.AddRange(fullQuest.TargetsAndCounters);
			}
			if (spawnedTextDisplays == null)
			{
				spawnedTextDisplays = new List<QuestItemDescriptionText>(targetsList.Count) { textDisplayTemplate };
			}
			textDisplayTemplate.ResetDisplay();
			while (spawnedTextDisplays.Count < targetsList.Count)
			{
				spawnedTextDisplays.Add(UnityEngine.Object.Instantiate(textDisplayTemplate, textDisplayTemplate.transform.parent));
			}
			for (int i = 0; i < targetsList.Count; i++)
			{
				(FullQuestBase.QuestTarget, int) tuple2 = targetsList[i];
				QuestItemDescriptionText questItemDescriptionText = spawnedTextDisplays[i];
				questItemDescriptionText.gameObject.SetActive(value: true);
				questItemDescriptionText.SetDisplay(fullQuest, tuple2.Item1, tuple2.Item2);
			}
			if ((bool)neededTextGroup)
			{
				if (targetsList.Count > 0)
				{
					flag = true;
				}
				if (targetsList.Count <= 1)
				{
					neededTextGroup.SetLocalPositionY(0f);
					break;
				}
				float newY = neededTextGroupOffsetPerItemY * (float)(targetsList.Count - 1);
				neededTextGroup.SetLocalPositionY(newY);
			}
			break;
		}
		case FullQuestBase.DescCounterTypes.ProgressBar:
		{
			if (!progressBarParent)
			{
				break;
			}
			int num = 0;
			int num2 = 0;
			foreach (var targetsAndCounter in fullQuest.TargetsAndCounters)
			{
				FullQuestBase.QuestTarget item = targetsAndCounter.target;
				int item2 = targetsAndCounter.count;
				num2 += fullQuest.GetCollectedCountOverride(item, item2);
				num += item.Count;
			}
			if (num > 0)
			{
				flag = true;
				progressBarParent.SetActive(value: true);
				if ((bool)progressBarSlider)
				{
					progressBarSlider.Value = (float)num2 / (float)num;
					progressBarSlider.Color = fullQuest.ProgressBarTint;
				}
				if ((bool)progressBarText)
				{
					progressBarText.text = string.Format(initialProgressBarText, num2, num);
				}
			}
			break;
		}
		case FullQuestBase.DescCounterTypes.Custom:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case FullQuestBase.DescCounterTypes.None:
			break;
		}
		Sprite sprite = fullQuest.RewardIcon;
		if ((bool)rewardGroup)
		{
			rewardGroup.SetActive(sprite);
		}
		if ((bool)rewardIcon)
		{
			rewardIcon.sprite = sprite;
			if ((bool)sprite)
			{
				flag = true;
				CounterMaterial counterMaterial = counterMaterials[(int)fullQuest.RewardIconType];
				rewardIcon.sharedMaterial = counterMaterial.Material;
			}
		}
		if (fullQuest.IsDonateType)
		{
			flag = true;
			if ((bool)donateCostGroup)
			{
				donateCostGroup.SetActive(value: true);
			}
			FullQuestBase.QuestTarget forTarget = fullQuest.Targets.FirstOrDefault();
			if ((bool)donateCostIcon)
			{
				donateCostIcon.sprite = fullQuest.GetCounterSpriteOverride(forTarget, 0);
				donateCostIcon.transform.localScale = donateCostIconScale.MultiplyElements(new Vector3(fullQuest.CounterIconScale, fullQuest.CounterIconScale, 1f));
			}
			if ((bool)donateCostText)
			{
				donateCostText.text = forTarget.Count.ToString();
			}
		}
		if (!flag)
		{
			bottomSection.SetAllActive(value: false);
		}
		int FindTargetIndex(int index)
		{
			int result = 0;
			int num5 = 0;
			for (int j = 0; j < targetsList.Count; j++)
			{
				(FullQuestBase.QuestTarget, int) tuple3 = targetsList[j];
				result = j;
				num5 += tuple3.Item1.Count;
				if (num5 > index)
				{
					break;
				}
			}
			return result;
		}
	}
}
