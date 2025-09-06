namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Divides one Float by another.")]
	public class FloatDivideV2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to divide.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("Divide the float variable by this value.")]
		public FsmFloat divideBy;

		[Tooltip("Repeate every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		public bool fixedUpdate;

		public override void Reset()
		{
			floatVariable = null;
			divideBy = null;
			everyFrame = false;
			fixedUpdate = false;
		}

		public override void OnEnter()
		{
			floatVariable.Value /= divideBy.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnUpdate()
		{
			if (!fixedUpdate)
			{
				floatVariable.Value /= divideBy.Value;
			}
		}

		public override void OnFixedUpdate()
		{
			if (fixedUpdate)
			{
				floatVariable.Value /= divideBy.Value;
			}
		}
	}
}
