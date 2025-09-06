using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Group")]
public class QuestGroup : QuestGroupBase
{
	[SerializeField]
	private List<BasicQuestBase> quests;

	public override bool CanGetMore()
	{
		Debug.LogError("CanGetMore() is not valid for this item", this);
		return false;
	}

	public override void Get(bool showPopup = true)
	{
		Debug.LogError("Get() is not valid for this item", this);
	}

	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		return quests;
	}

	public IEnumerable<FullQuestBase> GetFullQuests()
	{
		return GetQuests().OfType<FullQuestBase>();
	}

	public void Evaluate(out FullQuestBase quest, out int index)
	{
		quest = null;
		index = -1;
		if (quests == null || quests.Count == 0)
		{
			return;
		}
		List<FullQuestBase> source = (from q in GetFullQuests()
			where (q.IsAvailable || q.IsAccepted) && !q.IsCompleted
			select q).ToList();
		quest = source.FirstOrDefault((FullQuestBase q) => q.CanComplete);
		if (quest == null)
		{
			quest = source.FirstOrDefault((FullQuestBase q) => !q.IsAccepted);
		}
		if (quest == null)
		{
			quest = source.FirstOrDefault((FullQuestBase q) => q.IsAccepted);
		}
		if (quest != null)
		{
			index = quests.IndexOf(quest);
		}
	}
}
