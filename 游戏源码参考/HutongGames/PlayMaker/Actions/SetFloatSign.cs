namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class SetFloatSign : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatValue;

		public FsmBool setPositive;

		[Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public override void Reset()
		{
			floatValue = null;
			setPositive = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSignTest();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSignTest();
		}

		private void DoSignTest()
		{
			if (floatValue == null)
			{
				return;
			}
			if (setPositive.Value)
			{
				if (floatValue.Value < 0f)
				{
					floatValue.Value *= -1f;
				}
			}
			else if (floatValue.Value > 0f)
			{
				floatValue.Value *= -1f;
			}
		}
	}
}
