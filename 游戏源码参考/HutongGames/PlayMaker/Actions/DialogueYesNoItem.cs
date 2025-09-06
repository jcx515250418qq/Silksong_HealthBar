using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItem : YesNoAction
	{
		public FsmString TranslationSheet;

		public FsmString TranslationKey;

		[ObjectType(typeof(CollectableItem))]
		public FsmObject RequiredItem;

		public FsmInt RequiredAmount;

		public FsmBool ConsumeItem;

		[ObjectType(typeof(CollectableItem))]
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
			string text = new LocalisedString(TranslationSheet.Value, TranslationKey.Value);
			bool flag;
			try
			{
				string.Format(text, "TEST");
				flag = true;
			}
			catch (FormatException)
			{
				flag = false;
			}
			if (flag)
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, RequiredItem.Value as CollectableItem, RequiredAmount.Value, displayHudPopup: true, ConsumeItem.Value, WillGetItem.Value as CollectableItem);
			}
			else
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, text, RequiredItem.Value as CollectableItem, RequiredAmount.Value, displayHudPopup: true, ConsumeItem.Value, WillGetItem.Value as CollectableItem);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
