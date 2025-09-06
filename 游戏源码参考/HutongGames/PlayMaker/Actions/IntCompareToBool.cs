namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares two ints and sets a bool value depending on result")]
	public class IntCompareToBool : FsmStateAction
	{
		[RequiredField]
		public FsmInt integer1;

		[RequiredField]
		public FsmInt integer2;

		[Tooltip("Bool set if Int 1 equals Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		[Tooltip("Bool set if Int 1 is less than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		[Tooltip("Bool set if Int 1 is greater than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		public bool everyFrame;

		public override void Reset()
		{
			integer1 = 0;
			integer2 = 0;
			equalBool = null;
			lessThanBool = null;
			greaterThanBool = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoIntCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoIntCompare();
		}

		private void DoIntCompare()
		{
			if (!equalBool.IsNone)
			{
				if (integer1.Value == integer2.Value)
				{
					equalBool.Value = true;
				}
				else
				{
					equalBool.Value = false;
				}
			}
			if (!lessThanBool.IsNone)
			{
				if (integer1.Value < integer2.Value)
				{
					lessThanBool.Value = true;
				}
				else
				{
					lessThanBool.Value = false;
				}
			}
			if (!greaterThanBool.IsNone)
			{
				if (integer1.Value > integer2.Value)
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
}
