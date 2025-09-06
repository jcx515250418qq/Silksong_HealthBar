namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class SetFloatValueV2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat floatValue;

		public bool everyFrame;

		public FsmBool activeBool;

		public override void Reset()
		{
			floatVariable = null;
			floatValue = null;
			everyFrame = false;
			activeBool = null;
		}

		public override void OnEnter()
		{
			if (activeBool.IsNone || activeBool.Value)
			{
				floatVariable.Value = floatValue.Value;
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (activeBool.IsNone || activeBool.Value)
			{
				floatVariable.Value = floatValue.Value;
			}
		}
	}
}
