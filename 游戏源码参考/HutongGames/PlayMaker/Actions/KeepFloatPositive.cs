namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class KeepFloatPositive : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		public bool everyFrame;

		public override void Reset()
		{
			floatVariable = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			KeepPositive();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			KeepPositive();
		}

		private void KeepPositive()
		{
			if (floatVariable.Value < 0f)
			{
				floatVariable.Value *= -1f;
			}
		}
	}
}
