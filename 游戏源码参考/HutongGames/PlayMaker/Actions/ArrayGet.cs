namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Get a value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
	public class ArrayGet : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[Tooltip("The index into the array.")]
		public FsmInt index;

		[RequiredField]
		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a variable. NOTE: must be of the same type as the array.")]
		public FsmVar storeValue;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		[ActionSection("Events")]
		[Tooltip("The event to trigger if the index is out of range.")]
		public FsmEvent indexOutOfRange;

		public override void Reset()
		{
			array = null;
			index = null;
			everyFrame = false;
			storeValue = null;
			indexOutOfRange = null;
		}

		public override void OnEnter()
		{
			DoGetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetValue();
		}

		private void DoGetValue()
		{
			if (!array.IsNone && !storeValue.IsNone)
			{
				if (index.Value >= 0 && index.Value < array.Length)
				{
					storeValue.SetValue(array.Get(index.Value));
				}
				else
				{
					base.Fsm.Event(indexOutOfRange);
				}
			}
		}
	}
}
