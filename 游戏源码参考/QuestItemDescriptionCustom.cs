using System.Collections.Generic;
using UnityEngine;

public class QuestItemDescriptionCustom : MonoBehaviour
{
	[SerializeField]
	private FullQuestBase quest;

	[Space]
	[SerializeField]
	private IconCounter rangeDisplay;

	[SerializeField]
	private bool hideMax;

	private List<(FullQuestBase.QuestTarget target, int count)> targetsList;

	private void OnEnable()
	{
		if (targetsList == null)
		{
			targetsList = new List<(FullQuestBase.QuestTarget, int)>(quest.TargetsAndCounters);
		}
		else
		{
			targetsList.AddRange(quest.TargetsAndCounters);
		}
		int num = 0;
		int num2 = 0;
		foreach (var targets in targetsList)
		{
			FullQuestBase.QuestTarget item = targets.target;
			int item2 = targets.count;
			num2 += quest.GetCollectedCountOverride(item, item2);
			num += item.Count;
		}
		targetsList.Clear();
		rangeDisplay.MaxValue = (hideMax ? num2 : num);
		rangeDisplay.CurrentValue = num2;
	}
}
