using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class QuestConsumeTargetTake : FsmStateAction
	{
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;

		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;

		public override void Reset()
		{
			Quest = null;
			TakeDisplay = TakeItemTypes.Taken;
		}

		public override void OnEnter()
		{
			if (!Quest.IsNone)
			{
				FullQuestBase fullQuestBase = Quest.Value as FullQuestBase;
				if ((bool)fullQuestBase)
				{
					DoAction(fullQuestBase);
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			Finish();
		}

		private void DoAction(FullQuestBase quest)
		{
			if (!quest.IsAccepted || quest.IsCompleted)
			{
				return;
			}
			foreach (var (questTarget, amount) in quest.TargetsAndCountersNotHidden)
			{
				if ((bool)questTarget.Counter)
				{
					questTarget.Counter.Consume(amount, showCounter: false);
					CollectableUIMsg.ShowTakeMsg(questTarget.Counter, (TakeItemTypes)(object)TakeDisplay.Value);
				}
			}
		}
	}
}
