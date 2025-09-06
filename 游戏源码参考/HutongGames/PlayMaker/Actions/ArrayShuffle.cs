using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Shuffle values in an array. Optionally set a start index and range to shuffle only part of the array.")]
	public class ArrayShuffle : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array to shuffle.")]
		public FsmArray array;

		[Tooltip("Optional start Index for the shuffling. Leave it to none or 0 for no effect")]
		public FsmInt startIndex;

		[Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to none or 0 for no effect, it will shuffle the whole array")]
		public FsmInt shufflingRange;

		public override void Reset()
		{
			array = null;
			startIndex = new FsmInt
			{
				UseVariable = true
			};
			shufflingRange = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			List<object> list = new List<object>(array.Values);
			int num = 0;
			int num2 = list.Count - 1;
			if (startIndex.Value > 0)
			{
				num = Mathf.Min(startIndex.Value, num2);
			}
			if (shufflingRange.Value > 0)
			{
				num2 = Mathf.Min(list.Count - 1, num + shufflingRange.Value);
			}
			for (int num3 = num2; num3 > num; num3--)
			{
				int index = Random.Range(num, num3 + 1);
				object value = list[num3];
				list[num3] = list[index];
				list[index] = value;
			}
			array.Values = list.ToArray();
			Finish();
		}
	}
}
