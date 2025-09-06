using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CheckQuestBoard : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmInt AvailableQuestsCount;

		public FsmEvent HasQuestsEvent;

		public FsmEvent NoQuestsEvent;

		public override void Reset()
		{
			Target = null;
			AvailableQuestsCount = null;
			HasQuestsEvent = null;
			NoQuestsEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				QuestItemBoard component = safe.GetComponent<QuestItemBoard>();
				if ((bool)component)
				{
					int availableQuestsCount = component.AvailableQuestsCount;
					AvailableQuestsCount.Value = availableQuestsCount;
					base.Fsm.Event((availableQuestsCount > 0) ? HasQuestsEvent : NoQuestsEvent);
				}
			}
			Finish();
		}
	}
}
