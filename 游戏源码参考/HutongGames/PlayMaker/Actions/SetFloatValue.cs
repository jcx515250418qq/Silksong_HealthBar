namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class SetFloatValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Variable to set.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("Value to set it to.")]
		public FsmFloat floatValue;

		[Tooltip("Perform this action every frame. Useful if the Value is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			floatValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			floatVariable.Value = floatValue.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			floatVariable.Value = floatValue.Value;
		}
	}
}
