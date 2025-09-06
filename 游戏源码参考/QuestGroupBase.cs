using System.Collections.Generic;

public abstract class QuestGroupBase : SavedItem
{
	public abstract IEnumerable<BasicQuestBase> GetQuests();
}
