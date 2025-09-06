namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	public class ToggleString : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		public FsmString stringValue1;

		public FsmString stringValue2;

		public override void Reset()
		{
			stringVariable = null;
			stringValue1 = null;
			stringValue2 = null;
		}

		public override void OnEnter()
		{
			if (stringVariable.Value == stringValue1.Value)
			{
				stringVariable.Value = stringValue2.Value;
			}
			else if (stringVariable.Value == stringValue2.Value)
			{
				stringVariable.Value = stringValue1.Value;
			}
			Finish();
		}
	}
}
