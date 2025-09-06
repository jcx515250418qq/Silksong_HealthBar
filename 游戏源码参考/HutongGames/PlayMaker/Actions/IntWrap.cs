namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Wraps the value of Int Variable so it stays in a Min/Max range.\n\nExamples:\nWrap 120 between 0 and 100 -> 20\nWrap -10 between 0 and 100 -> 90")]
	public class IntWrap : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Int variable to wrap.")]
		public FsmInt intVariable;

		[RequiredField]
		[Tooltip("The minimum value allowed.")]
		public FsmInt minValue;

		[RequiredField]
		[Tooltip("The maximum value allowed.")]
		public FsmInt maxValue;

		[Tooltip("Repeat every frame. Useful if the int variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			intVariable = null;
			minValue = null;
			maxValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoWrap();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoWrap();
		}

		private void DoWrap()
		{
			int value = intVariable.Value;
			int value2 = minValue.Value;
			int value3 = maxValue.Value;
			if (value < value2)
			{
				intVariable.Value = value3 - (value2 - value) % (value3 - value2);
			}
			else
			{
				intVariable.Value = value2 + (value - value2) % (value3 - value2);
			}
		}
	}
}
