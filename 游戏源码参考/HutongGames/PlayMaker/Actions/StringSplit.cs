namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Splits a string into substrings using separator characters.")]
	public class StringSplit : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("String to split.")]
		public FsmString stringToSplit;

		[Tooltip("Characters used to split the string.\nUse '\\n' for newline\nUse '\\t' for tab")]
		public FsmString separators;

		[Tooltip("Remove all leading and trailing white-space characters from each separated string.")]
		public FsmBool trimStrings;

		[Tooltip("Optional characters used to trim each separated string.")]
		public FsmString trimChars;

		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		[Tooltip("Store the split strings in a String Array.")]
		public FsmArray stringArray;

		public override void Reset()
		{
			stringToSplit = null;
			separators = null;
			trimStrings = false;
			trimChars = null;
			stringArray = null;
		}

		public override void OnEnter()
		{
			char[] array = trimChars.Value.ToCharArray();
			if (!stringToSplit.IsNone && !stringArray.IsNone)
			{
				FsmArray fsmArray = stringArray;
				object[] values = stringToSplit.Value.Split(separators.Value.ToCharArray());
				fsmArray.Values = values;
				if (trimStrings.Value)
				{
					for (int i = 0; i < stringArray.Values.Length; i++)
					{
						if (stringArray.Values[i] is string text)
						{
							if (!trimChars.IsNone && array.Length != 0)
							{
								stringArray.Set(i, text.Trim(array));
							}
							else
							{
								stringArray.Set(i, text.Trim());
							}
						}
					}
				}
				stringArray.SaveChanges();
			}
			Finish();
		}
	}
}
