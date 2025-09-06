using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class DialogueYesNoV2 : YesNoAction
	{
		public FsmString TranslationSheet;

		public FsmString TranslationKey;

		public FsmBool UseCurrency;

		[HideIf("IsNotUsingCurrency")]
		public FsmInt CurrencyCost;

		[HideIf("IsNotUsingCurrency")]
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		[HideIf("IsNotUsingCurrency")]
		public FsmBool ConsumeCurrency;

		[ObjectType(typeof(CollectableItem))]
		public FsmObject WillGetItem;

		public bool IsNotUsingCurrency()
		{
			return !UseCurrency.Value;
		}

		public override void Reset()
		{
			base.Reset();
			TranslationSheet = null;
			TranslationKey = null;
			UseCurrency = null;
			CurrencyCost = null;
			CurrencyType = null;
			ConsumeCurrency = null;
			WillGetItem = null;
		}

		protected override void DoOpen()
		{
			string text = new LocalisedString(TranslationSheet.Value, TranslationKey.Value);
			Action yes = delegate
			{
				SendEvent(isYes: true);
			};
			Action no = delegate
			{
				SendEvent(isYes: false);
			};
			if (IsNotUsingCurrency())
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, text, WillGetItem.Value as CollectableItem);
			}
			else
			{
				DialogueYesNoBox.Open(yes, no, ReturnHUDAfter.Value, text, (CurrencyType)(object)CurrencyType.Value, CurrencyCost.Value, displayHudPopup: true, ConsumeCurrency.Value, WillGetItem.Value as CollectableItem);
			}
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
