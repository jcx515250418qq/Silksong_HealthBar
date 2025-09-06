using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	public abstract class QuestFsmAction : FsmStateAction
	{
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;

		protected virtual bool CustomFinish => false;

		public override void Reset()
		{
			Quest = null;
		}

		public override void OnEnter()
		{
			if (!Quest.IsNone)
			{
				FullQuestBase fullQuestBase = Quest.Value as FullQuestBase;
				if ((bool)fullQuestBase)
				{
					DoQuestAction(fullQuestBase);
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			if (!CustomFinish)
			{
				Finish();
			}
		}

		protected abstract void DoQuestAction(FullQuestBase quest);
	}
}
