namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Float value.")]
	public class ConvertDoubleBoolToFloat : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable1;

		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable2;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to set based on the Bool variable value.")]
		public FsmFloat floatVariable;

		public FsmFloat bothFalseValue;

		public FsmFloat oneTrueValue;

		public FsmFloat bothTrueValue;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable1 = null;
			boolVariable2 = null;
			floatVariable = null;
			bothFalseValue = 0f;
			oneTrueValue = 0.5f;
			bothTrueValue = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToFloat();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoConvertBoolToFloat();
		}

		private void DoConvertBoolToFloat()
		{
			if (!boolVariable1.Value && !boolVariable2.Value)
			{
				floatVariable.Value = bothFalseValue.Value;
			}
			else if (boolVariable1.Value && boolVariable2.Value)
			{
				floatVariable.Value = bothTrueValue.Value;
			}
			else
			{
				floatVariable.Value = oneTrueValue.Value;
			}
		}
	}
}
