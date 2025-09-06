using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Remove an item from an array.")]
	public class ArrayRemove : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("Item to remove.")]
		public FsmVar value;

		[Tooltip("Remove all instances of the value. Otherwise removes only the first instance.")]
		public FsmBool allMatches;

		public override void Reset()
		{
			array = null;
			value = null;
			allMatches = new FsmBool
			{
				Value = true
			};
		}

		public override void OnEnter()
		{
			DoRemoveValue();
			Finish();
		}

		private void DoRemoveValue()
		{
			if (array == null || value == null)
			{
				return;
			}
			value.UpdateValue();
			List<object> list = new List<object>(array.Values);
			if (allMatches.Value)
			{
				list.RemoveAll((object x) => (x == null && value.GetValue() == null) || (x?.Equals(value.GetValue()) ?? false));
			}
			else
			{
				list.Remove(value.GetValue());
			}
			array.Values = list.ToArray();
			array.SaveChanges();
		}
	}
}
