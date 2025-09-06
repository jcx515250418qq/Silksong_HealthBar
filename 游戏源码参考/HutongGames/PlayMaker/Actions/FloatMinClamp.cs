namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class FloatMinClamp : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("The minimum value.")]
		public FsmFloat minValue;

		[RequiredField]
		[Tooltip("The maximum value.")]
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
			DoClamp();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoClamp();
		}

		private void DoClamp()
		{
			float value = floatVariable.Value;
			bool flag = false;
			if (value < 0f && value > minValue.Value)
			{
				value = minValue.Value;
				flag = true;
			}
			if (value > 0f && value < maxValue.Value)
			{
				value = maxValue.Value;
				flag = true;
			}
			if (flag)
			{
				floatVariable.Value = value;
			}
		}
	}
}
