namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Vector2 value.")]
	public class ConvertBoolToVector2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to test.")]
		public FsmBool BoolVariable;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 variable to set based on the Bool variable value.")]
		public FsmVector2 Vector2Variable;

		[Tooltip("Vector2 value if Bool variable is false.")]
		public FsmVector2 FalseValue;

		[Tooltip("Vector2 value if Bool variable is true.")]
		public FsmVector2 TrueValue;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool EveryFrame;

		public override void Reset()
		{
			BoolVariable = null;
			Vector2Variable = null;
			FalseValue = null;
			TrueValue = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToVector2();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoConvertBoolToVector2();
		}

		private void DoConvertBoolToVector2()
		{
			Vector2Variable.Value = (BoolVariable.Value ? TrueValue.Value : FalseValue.Value);
		}
	}
}
