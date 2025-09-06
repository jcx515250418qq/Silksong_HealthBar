namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to set.")]
		public FsmString stringVariable;

		[UIHint(UIHint.TextArea)]
		[Tooltip("The value to set the variable to.")]
		public FsmString stringValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			stringVariable = null;
			stringValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetStringValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetStringValue();
		}

		private void DoSetStringValue()
		{
			if (stringVariable != null && stringValue != null)
			{
				stringVariable.Value = stringValue.Value;
			}
		}
	}
}
