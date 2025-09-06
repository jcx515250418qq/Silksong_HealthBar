namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares 2 Strings and sends Events based on the result.")]
	public class StringCompare : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to compare.")]
		public FsmString stringVariable;

		[Tooltip("Compare to this text.")]
		public FsmString compareTo;

		[Tooltip("Event to send if strings are equal.")]
		public FsmEvent equalEvent;

		[Tooltip("Event to send if strings are not equal.")]
		public FsmEvent notEqualEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			stringVariable = null;
			compareTo = "";
			equalEvent = null;
			notEqualEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoStringCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoStringCompare();
		}

		private void DoStringCompare()
		{
			if (stringVariable != null && compareTo != null)
			{
				bool flag = (string.IsNullOrEmpty(stringVariable.Value) ? string.IsNullOrEmpty(compareTo.Value) : (stringVariable.Value == compareTo.Value));
				if (storeResult != null)
				{
					storeResult.Value = flag;
				}
				if (flag && equalEvent != null)
				{
					base.Fsm.Event(equalEvent);
				}
				else if (!flag && notEqualEvent != null)
				{
					base.Fsm.Event(notEqualEvent);
				}
			}
		}
	}
}
