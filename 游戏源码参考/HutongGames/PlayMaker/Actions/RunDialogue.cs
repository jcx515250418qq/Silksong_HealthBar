using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class RunDialogue : RunDialogueBase
	{
		[RequiredField]
		public FsmString Sheet;

		[RequiredField]
		public FsmString Key;

		protected override string DialogueText
		{
			get
			{
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(Sheet.Value, Key.Value).ToString(allowBlankText: false);
				}
				return Sheet.Value + " / " + Key.Value;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Sheet = null;
			Key = null;
		}
	}
}
