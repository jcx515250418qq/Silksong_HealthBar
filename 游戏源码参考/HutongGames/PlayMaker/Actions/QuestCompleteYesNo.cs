using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class QuestCompleteYesNo : YesNoAction
	{
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;

		public FsmBool ConsumeCurrency;

		public override void Reset()
		{
			base.Reset();
			Quest = null;
			ConsumeCurrency = null;
		}

		protected override void DoOpen()
		{
			FullQuestBase fullQuestBase = Quest.Value as FullQuestBase;
			if (fullQuestBase == null)
			{
				return;
			}
			_ = fullQuestBase.CanComplete;
			bool consumeCurrency = ConsumeCurrency.Value && fullQuestBase.ConsumeTargetIfApplicable;
			SavedItem savedItem = null;
			QuestTargetCurrency questTargetCurrency = null;
			int amount = 0;
			int num = 0;
			foreach (FullQuestBase.QuestTarget target in fullQuestBase.Targets)
			{
				SavedItem counter = target.Counter;
				QuestTargetCurrency questTargetCurrency2 = target.Counter as QuestTargetCurrency;
				if (target.Count > 0 && (!(counter == null) || !(questTargetCurrency2 == null)))
				{
					num++;
					if (num <= 1)
					{
						savedItem = counter;
						questTargetCurrency = questTargetCurrency2;
						amount = target.Count;
					}
				}
			}
			switch (num)
			{
			case 0:
				NoTarget();
				return;
			case 1:
				if (questTargetCurrency != null)
				{
					DialogueYesNoBox.Open(TrueWrapper, FalseWrapper, ReturnHUDAfter.Value, questTargetCurrency.GivePromptText, questTargetCurrency.CurrencyType, amount, displayHudPopup: true, consumeCurrency);
				}
				else if ((bool)savedItem)
				{
					DialogueYesNoBox.Open(TrueWrapper, FalseWrapper, ReturnHUDAfter.Value, savedItem, amount, displayHudPopup: false, consumeCurrency);
				}
				else
				{
					NoTarget();
				}
				return;
			}
			List<QuestTargetCounter> items = fullQuestBase.Targets.Select((FullQuestBase.QuestTarget target) => target.Counter).ToList();
			List<int> amounts = fullQuestBase.Targets.Select((FullQuestBase.QuestTarget target) => target.Count).ToList();
			string text;
			if (fullQuestBase.GiveNameOverride.IsEmpty)
			{
				text = Language.Get("GIVE_ITEMS_PROMPT", "UI");
			}
			else
			{
				text = Language.Get("GIVE_ITEM_PROMPT", "UI");
				text = string.Format(text, fullQuestBase.GiveNameOverride);
			}
			DialogueYesNoBox.Open(TrueWrapper, FalseWrapper, ReturnHUDAfter.Value, text, items, amounts, displayHudPopup: false, consumeCurrency, null);
			void FalseWrapper()
			{
				SendEvent(isYes: false);
			}
			void NoTarget()
			{
				DialogueYesNoBox.Open(TrueWrapper, FalseWrapper, ReturnHUDAfter.Value, "! Turn in Quest? !");
			}
			void TrueWrapper()
			{
				SendEvent(isYes: true);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
