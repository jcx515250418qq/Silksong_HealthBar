namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValueBool : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[UIHint(UIHint.Variable)]
		public FsmBool testBool;

		public FsmString trueValue;

		public FsmString falseValue;

		public bool everyframe;

		public override void Reset()
		{
			stringVariable = null;
			trueValue = null;
			falseValue = null;
		}

		public override void OnEnter()
		{
			DoSetStringValue();
			if (!everyframe)
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
			if (stringVariable == null)
			{
				return;
			}
			if (testBool.Value)
			{
				if (trueValue != null)
				{
					stringVariable.Value = trueValue.Value;
				}
			}
			else if (falseValue != null)
			{
				stringVariable.Value = falseValue.Value;
			}
		}
	}
}
