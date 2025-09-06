using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV4 : YesNoAction
	{
		public LocalisedFsmString Prompt;

		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		public FsmInt RequiredAmount;

		public FsmBool ShowCounter;

		public FsmBool ConsumeItem;

		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;

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
			TakeDisplay = null;
			WillGetItem = null;
		}

		protected override void DoOpen()
		{
			SavedItem requiredItem = RequiredItem.Value as SavedItem;
			Action yes = delegate
			{
				if (requiredItem is ICollectableUIMsgItem item)
				{
					TakeItemTypes takeItemType = (TakeItemTypes)(object)TakeDisplay.Value;
					CollectableUIMsg.ShowTakeMsg(item, takeItemType);
				}
				SendEvent(isYes: true);
			};
			Action no = delegate
			{
				SendEvent(isYes: false);
			};
			LocalisedString localisedString = Prompt;
			if (localisedString.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, requiredItem, RequiredAmount.Value, ShowCounter.Value, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
			else
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, localisedString, requiredItem, RequiredAmount.Value, ShowCounter.Value, ConsumeItem.Value, WillGetItem.Value as SavedItem);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
