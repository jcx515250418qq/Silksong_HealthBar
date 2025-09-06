using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class RelicBoardOwnerYesNo : YesNoAction
	{
		public FsmOwnerDefault Target;

		public FsmString TranslationSheet;

		public FsmString TranslationKey;

		public FsmString TranslationKeyPlural;

		private List<CollectableItemRelicType> relics;

		private List<int> amounts;

		public override void Reset()
		{
			base.Reset();
			Target = null;
			TranslationSheet = null;
			TranslationKey = null;
			TranslationKeyPlural = null;
		}

		protected override void DoOpen()
		{
			RelicBoardOwner component = Target.GetSafe(this).GetComponent<RelicBoardOwner>();
			if (relics == null)
			{
				relics = new List<CollectableItemRelicType>();
			}
			relics.Clear();
			relics.AddRange((from relic in component.GetRelicsToDeposit()
				select relic.RelicType).Distinct());
			if (amounts == null)
			{
				amounts = new List<int>();
			}
			amounts.Clear();
			amounts.AddRange(relics.Select((CollectableItemRelicType type) => type.CollectedAmount));
			string text = new LocalisedString(TranslationSheet.Value, (relics.Count > 1) ? TranslationKeyPlural.Value : TranslationKey.Value);
			DialogueYesNoBox.Open(delegate
			{
				SendEvent(isYes: true);
			}, delegate
			{
				SendEvent(isYes: false);
			}, ReturnHUDAfter.Value, text, relics, amounts, displayHudPopup: false, consumeCurrency: false, null);
		}

		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}
	}
}
