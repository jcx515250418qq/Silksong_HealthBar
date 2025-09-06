using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Board List")]
public class QuestBoardList : NamedScriptableObjectList<QuestGroupBase>
{
	public int CompletedQuestCount
	{
		get
		{
			int num = 0;
			using IEnumerator<QuestGroupBase> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				QuestGroupBase current = enumerator.Current;
				if (!current)
				{
					continue;
				}
				foreach (BasicQuestBase quest in current.GetQuests())
				{
					if (quest is IQuestWithCompletion { IsCompleted: not false })
					{
						num++;
					}
				}
			}
			return num;
		}
	}
}
