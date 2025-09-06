namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an integer variable using a float value.")]
	public class SetIntFromFloat : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int variable to set.")]
		public FsmInt intVariable;

		[Tooltip("The float value.")]
		public FsmFloat floatValue;

		[Tooltip("Do it every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			intVariable = null;
			floatValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			intVariable.Value = (int)floatValue.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			intVariable.Value = (int)floatValue.Value;
		}
	}
}
