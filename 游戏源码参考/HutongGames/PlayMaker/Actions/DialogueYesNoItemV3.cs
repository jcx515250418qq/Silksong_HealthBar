using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV3 : YesNoAction
	{
		public LocalisedFsmString Prompt;

		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		public FsmInt RequiredAmount;

		public FsmBool ShowCounter;

		public FsmBool ConsumeItem;

		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;

		public override void Reset()
		{
			base.Reset();
			Prompt = null;
			RequiredItem = null;
			RequiredAmount = null;
			ShowCounter = true;
			ConsumeItem = null;
			WillGetItem = null;
		}

		protected override void DoOpen()
		{
			Action yes = delegate
			{
				SendEvent(isYes: true);
			};
			Action no = delegate
			{
				SendEvent(isYes: false);
			};
			LocalisedString localisedString = Prompt;
			if (localisedString.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, RequiredItem.Value as SavedItem, RequiredAmount.Value, ShowCounter.Value, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
			else
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, localisedString, RequiredItem.Value as SavedItem, RequiredAmount.Value, ShowCounter.Value, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
