using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	public class GetDifferenceBetweenFloats : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat differenceResult;

		[RequiredField]
		public FsmFloat float1;

		[RequiredField]
		public FsmFloat float2;

		public bool everyFrame;

		public override void Reset()
		{
			differenceResult = null;
			float1 = null;
			float2 = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCalcDifference();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCalcDifference();
		}

		private void DoCalcDifference()
		{
			if (differenceResult != null)
			{
				float value = Mathf.Abs(float2.Value - float1.Value);
				differenceResult.Value = value;
			}
		}
	}
}
