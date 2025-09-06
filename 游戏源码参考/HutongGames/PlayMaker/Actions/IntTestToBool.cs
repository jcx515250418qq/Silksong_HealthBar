namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Set bools based on the comparison of 2 ints.")]
	public class IntTestToBool : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The first int variable.")]
		public FsmInt int1;

		[RequiredField]
		[Tooltip("The second int variable.")]
		public FsmInt int2;

		[Tooltip("Bool set if Int 1 equals Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		[Tooltip("Bool set if Int 1 is less than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		[Tooltip("Bool set if Int 1 is greater than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public override void Reset()
		{
			int1 = 0;
			int2 = 0;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompare();
		}

		private void DoCompare()
		{
			if (int1.Value == int2.Value)
			{
				equalBool.Value = true;
			}
			else
			{
				equalBool.Value = false;
			}
			if (int1.Value < int2.Value)
			{
				lessThanBool.Value = true;
			}
			else
			{
				lessThanBool.Value = false;
			}
			if (int1.Value > int2.Value)
			{
				greaterThanBool.Value = true;
			}
			else
			{
				greaterThanBool.Value = false;
			}
		}
	}
}
