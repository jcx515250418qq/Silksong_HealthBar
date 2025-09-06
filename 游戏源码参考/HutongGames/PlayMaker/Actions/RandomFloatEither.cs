using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float Variable to a random choice of two floats.")]
	public class RandomFloatEither : FsmStateAction
	{
		[RequiredField]
		public FsmFloat value1;

		[RequiredField]
		public FsmFloat value2;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			value1 = 0f;
			value2 = 1f;
			storeResult = null;
		}

		public override void OnEnter()
		{
			if ((float)Random.Range(0, 100) < 50f)
			{
				storeResult.Value = value1.Value;
			}
			else
			{
				storeResult.Value = value2.Value;
			}
			Finish();
		}
	}
}
