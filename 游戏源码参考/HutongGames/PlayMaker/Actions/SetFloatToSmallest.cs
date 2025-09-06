namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable to the smallest of two values.")]
	public class SetFloatToSmallest : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat value1;

		[RequiredField]
		public FsmFloat value2;

		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			value1 = null;
			value2 = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			if (value1.Value < value2.Value)
			{
				floatVariable.Value = value1.Value;
			}
			else
			{
				floatVariable.Value = value2.Value;
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (value1.Value < value2.Value)
			{
				floatVariable.Value = value1.Value;
			}
			else
			{
				floatVariable.Value = value2.Value;
			}
		}
	}
}
