namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets a sub-string from a String Variable.")]
	public class GetSubstring : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string to get characters from.")]
		public FsmString stringVariable;

		[RequiredField]
		[Tooltip("The start of the substring (0 = first character).")]
		public FsmInt startIndex;

		[RequiredField]
		[Tooltip("The number of characters to get.")]
		public FsmInt length;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a string variable.")]
		public FsmString storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			stringVariable = null;
			startIndex = 0;
			length = 1;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetSubstring();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetSubstring();
		}

		private void DoGetSubstring()
		{
			if (stringVariable != null && storeResult != null)
			{
				storeResult.Value = stringVariable.Value.Substring(startIndex.Value, length.Value);
			}
		}
	}
}
