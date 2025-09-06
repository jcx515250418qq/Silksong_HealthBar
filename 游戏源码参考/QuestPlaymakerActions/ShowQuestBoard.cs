using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class ShowQuestBoard : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmEvent CanceledEvent;

		public FsmEvent AcceptedEvent;

		[ArrayEditor(typeof(Quest), "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray AcceptedQuests;

		private QuestItemBoard board;

		public override void Reset()
		{
			Target = null;
			AcceptedEvent = null;
			CanceledEvent = null;
			AcceptedQuests = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				board = safe.GetComponent<QuestItemBoard>();
				if ((bool)board)
				{
					board.BoardClosed += OnBoardClosed;
					board.OpenPane();
				}
			}
			else
			{
				Finish();
			}
		}

		private void OnBoardClosed(List<BasicQuestBase> quests)
		{
			board.BoardClosed -= OnBoardClosed;
			if (quests != null && quests.Count > 0)
			{
				if (!AcceptedQuests.IsNone)
				{
					FsmArray acceptedQuests = AcceptedQuests;
					object[] values = quests.ToArray();
					acceptedQuests.Values = values;
				}
				base.Fsm.Event(AcceptedEvent);
			}
			else
			{
				base.Fsm.Event(CanceledEvent);
			}
		}
	}
}
