namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Wraps the value of Float Variable so it stays in a Min/Max range.\n\nExamples:\nWrap 120 between 0 and 100 -> 20\nWrap -10 between 0 and 100 -> 90")]
	public class FloatWrap : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to wrap.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("The minimum value allowed.")]
		public FsmFloat minValue;

		[RequiredField]
		[Tooltip("The maximum value allowed.")]
		public FsmFloat maxValue;

		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			minValue = null;
			maxValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoWrap();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoWrap();
		}

		private void DoWrap()
		{
			float value = floatVariable.Value;
			float value2 = minValue.Value;
			float value3 = maxValue.Value;
			if (value < value2)
			{
				floatVariable.Value = value3 - (value2 - value) % (value3 - value2);
			}
			else
			{
				floatVariable.Value = value2 + (value - value2) % (value3 - value2);
			}
		}
	}
}
