namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Checks whether a float value is within a certain range (inclusive)")]
	public class FloatInRange : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat lowerValue;

		[RequiredField]
		public FsmFloat upperValue;

		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[Tooltip("Repeat every frame. Useful if the variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			lowerValue = null;
			upperValue = null;
			boolVariable = null;
			everyFrame = false;
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter()
		{
			DoFloatRange();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFloatRange();
		}

		private void DoFloatRange()
		{
			if (!floatVariable.IsNone)
			{
				if (floatVariable.Value <= upperValue.Value && floatVariable.Value >= lowerValue.Value)
				{
					boolVariable.Value = true;
					base.Fsm.Event(trueEvent);
				}
				else
				{
					boolVariable.Value = false;
					base.Fsm.Event(falseEvent);
				}
			}
		}
	}
}
