using System.Linq;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV5 : YesNoAction
	{
		public LocalisedFsmString Prompt;

		[ArrayEditor(typeof(SavedItem), "", 0, 0, 65536)]
		public FsmArray RequiredItems;

		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray RequiredAmounts;

		public FsmInt CurrencyCost;

		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

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
			RequiredItems = null;
			RequiredAmounts = null;
			CurrencyCost = null;
			CurrencyType = null;
			ShowCounter = true;
			ConsumeItem = null;
			TakeDisplay = null;
			WillGetItem = null;
		}

		protected override void DoOpen()
		{
			LocalisedString localisedString = Prompt;
			DialogueYesNoBox.Open(delegate
			{
				SendEvent(isYes: true);
			}, delegate
			{
				SendEvent(isYes: false);
			}, ReturnHUDAfter.Value, localisedString.IsEmpty ? null : ((string)localisedString), (CurrencyType)(object)CurrencyType.Value, CurrencyCost.Value, RequiredItems.objectReferences.Cast<SavedItem>().ToList(), RequiredAmounts.intValues, ShowCounter.Value, ConsumeItem.Value, WillGetItem.Value as SavedItem, (TakeItemTypes)(object)TakeDisplay.Value);
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
