namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an Int Variable to the smallest of two values.")]
	public class SetIntToSmallest : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[RequiredField]
		public FsmInt value1;

		[RequiredField]
		public FsmInt value2;

		public bool everyFrame;

		public override void Reset()
		{
			intVariable = null;
			value1 = null;
			value2 = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			if (value1.Value < value2.Value)
			{
				intVariable.Value = value1.Value;
			}
			else
			{
				intVariable.Value = value2.Value;
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
				intVariable.Value = value1.Value;
			}
			else
			{
				intVariable.Value = value2.Value;
			}
		}
	}
}
