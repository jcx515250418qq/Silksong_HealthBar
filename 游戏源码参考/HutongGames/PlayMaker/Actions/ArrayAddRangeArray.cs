namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	public class ArrayAddRangeArray : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray arrayTo;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray arrayFrom;

		public override string ErrorCheck()
		{
			if (arrayTo.TypeConstraint == arrayFrom.TypeConstraint)
			{
				return base.ErrorCheck();
			}
			return "Array types do not match";
		}

		public override void Reset()
		{
			arrayTo = null;
			arrayFrom = null;
		}

		public override void OnEnter()
		{
			DoAddRange();
			Finish();
		}

		private void DoAddRange()
		{
			int num = arrayFrom.Length;
			if (num > 0)
			{
				object[] values = arrayFrom.Values;
				foreach (object value in values)
				{
					arrayTo.Set(arrayTo.Length - num, value);
					num--;
				}
			}
		}
	}
}
