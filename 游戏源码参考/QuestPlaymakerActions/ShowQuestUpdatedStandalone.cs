using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class ShowQuestUpdatedStandalone : FsmStateAction
	{
		[ObjectType(typeof(BasicQuestBase))]
		public FsmObject Quest;

		public override void Reset()
		{
			Quest = null;
		}

		public override void OnEnter()
		{
			if (!Quest.IsNone)
			{
				BasicQuestBase basicQuestBase = Quest.Value as BasicQuestBase;
				if ((bool)basicQuestBase)
				{
					if (basicQuestBase.IsAccepted && !(basicQuestBase is FullQuestBase { IsCompleted: not false }))
					{
						QuestManager.ShowQuestUpdatedStandalone(basicQuestBase);
					}
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			Finish();
		}
	}
}
