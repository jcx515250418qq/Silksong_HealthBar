using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Check if an Array contains a value. Optionally get its index.")]
	public class ArrayContains : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("The value to check against in the array.")]
		public FsmVar value;

		[ActionSection("Result")]
		[Tooltip("The index of the value in the array.")]
		[UIHint(UIHint.Variable)]
		public FsmInt index;

		[Tooltip("Store in a bool whether it contains that element or not.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		[Tooltip("Event sent if the array contains that element.")]
		public FsmEvent isContainedEvent;

		[Tooltip("Event sent if the array does not contain that element.")]
		public FsmEvent isNotContainedEvent;

		public override void Reset()
		{
			array = null;
			value = null;
			index = null;
			isContained = null;
			isContainedEvent = null;
			isNotContainedEvent = null;
		}

		public override void OnEnter()
		{
			DoCheckContainsValue();
			Finish();
		}

		private void DoCheckContainsValue()
		{
			value.UpdateValue();
			int num = -1;
			num = ((value.GetValue() != null && !value.GetValue().Equals(null)) ? Array.IndexOf(array.Values, value.GetValue()) : Array.FindIndex(array.Values, (object x) => x?.Equals(null) ?? true));
			bool flag = num != -1;
			isContained.Value = flag;
			index.Value = num;
			if (flag)
			{
				base.Fsm.Event(isContainedEvent);
			}
			else
			{
				base.Fsm.Event(isNotContainedEvent);
			}
		}
	}
}
