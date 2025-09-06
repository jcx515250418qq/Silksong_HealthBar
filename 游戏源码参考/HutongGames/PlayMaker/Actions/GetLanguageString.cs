using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game Text")]
	[Tooltip("Grab a string from the Hollow Knight game text database in the correct language.")]
	public class GetLanguageString : FsmStateAction
	{
		[RequiredField]
		public FsmString sheetName;

		[RequiredField]
		public FsmString convName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString storeValue;

		public override void Reset()
		{
			sheetName = null;
			convName = null;
			storeValue = null;
		}

		public override void OnEnter()
		{
			storeValue.Value = Language.Get(convName.Value, sheetName.Value);
			storeValue.Value = storeValue.Value.Replace("<br>", "\n");
			Finish();
		}
	}
}
