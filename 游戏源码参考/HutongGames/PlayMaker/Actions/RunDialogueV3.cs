using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class RunDialogueV3 : RunDialogueBase
	{
		public FsmString CustomText;

		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		[HideIf("UsesCustomText")]
		public FsmString Key;

		public FsmColor TextColor;

		protected override string DialogueText
		{
			get
			{
				if (UsesCustomText())
				{
					return CustomText.Value;
				}
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(Sheet.Value, Key.Value).ToString(allowBlankText: false);
				}
				return Sheet.Value + " / " + Key.Value;
			}
		}

		public bool UsesCustomText()
		{
			return !CustomText.IsNone;
		}

		public override void Reset()
		{
			base.Reset();
			CustomText = new FsmString
			{
				UseVariable = true
			};
			Sheet = null;
			Key = null;
			TextColor = new FsmColor
			{
				UseVariable = true
			};
		}

		protected override DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			if (!TextColor.IsNone)
			{
				defaultOptions.TextColor = TextColor.Value;
			}
			return defaultOptions;
		}
	}
}
