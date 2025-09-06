using HutongGames.PlayMaker;
using TeamCherry.Localization;

[ActionCategory("Game Text")]
public class GetLanguageStringProcessed : FsmStateAction
{
	[RequiredField]
	public FsmString sheetName;

	[RequiredField]
	public FsmString convName;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString storeValue;

	[ObjectType(typeof(LocalisationHelper.FontSource))]
	public FsmEnum fontSource;

	public override void Reset()
	{
		sheetName = null;
		convName = null;
		storeValue = null;
		fontSource = null;
	}

	public override void Awake()
	{
		fontSource.Value = fontSource.Value;
	}

	public override void OnEnter()
	{
		string text = Language.Get(convName.Value, sheetName.Value);
		text = text.Replace("<br>", "\n");
		storeValue.Value = text.GetProcessed((LocalisationHelper.FontSource)(object)fontSource.Value);
		Finish();
	}
}
