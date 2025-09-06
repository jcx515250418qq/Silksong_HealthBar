namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an Int Variable.")]
	public class SetIntValueBool : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[UIHint(UIHint.Variable)]
		public FsmBool testBool;

		public FsmInt trueValue;

		public FsmInt falseValue;

		public override void Reset()
		{
			intVariable = null;
			trueValue = null;
			falseValue = null;
		}

		public override void OnEnter()
		{
			DoSetStringValue();
			Finish();
		}

		private void DoSetStringValue()
		{
			if (intVariable == null)
			{
				return;
			}
			if (testBool.Value)
			{
				if (!trueValue.IsNone)
				{
					intVariable.Value = trueValue.Value;
				}
			}
			else if (!falseValue.IsNone)
			{
				intVariable.Value = falseValue.Value;
			}
		}
	}
}
