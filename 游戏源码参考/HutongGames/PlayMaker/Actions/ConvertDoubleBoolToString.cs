namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	public class ConvertDoubleBoolToString : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable1;

		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable2;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		public FsmString bothFalseValue;

		public FsmString oneTrueValue;

		public FsmString bothTrueValue;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable1 = null;
			boolVariable2 = null;
			stringVariable = null;
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
				stringVariable.Value = bothFalseValue.Value;
			}
			else if (boolVariable1.Value && boolVariable2.Value)
			{
				stringVariable.Value = bothTrueValue.Value;
			}
			else
			{
				stringVariable.Value = oneTrueValue.Value;
			}
		}
	}
}
