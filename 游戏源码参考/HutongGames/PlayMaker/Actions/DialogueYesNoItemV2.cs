using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV2 : YesNoAction
	{
		public FsmString TranslationSheet;

		public FsmString TranslationKey;

		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		public FsmInt RequiredAmount;

		public FsmBool ConsumeItem;

		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;

		public override void Reset()
		{
			base.Reset();
			TranslationSheet = null;
			TranslationKey = null;
			RequiredItem = null;
			RequiredAmount = null;
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
			LocalisedString localisedString = new LocalisedString(TranslationSheet.Value, TranslationKey.Value);
			if (localisedString.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, RequiredItem.Value as SavedItem, RequiredAmount.Value, displayHudPopup: true, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
			else
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, localisedString, RequiredItem.Value as SavedItem, RequiredAmount.Value, displayHudPopup: true, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
