using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Set bools based on the comparison of 2 Floats.")]
	public class FloatTestToBool : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The first float variable.")]
		public FsmFloat float1;

		[RequiredField]
		[Tooltip("The second float variable.")]
		public FsmFloat float2;

		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		[Tooltip("Bool set if Float 1 equals Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		[Tooltip("Bool set if Float 1 is less than Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		[Tooltip("Bool set if Float 1 is greater than Float 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public override void Reset()
		{
			float1 = 0f;
			float2 = 0f;
			tolerance = 0f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompare();
		}

		private void DoCompare()
		{
			if (Mathf.Abs(float1.Value - float2.Value) <= tolerance.Value)
			{
				equalBool.Value = true;
			}
			else
			{
				equalBool.Value = false;
			}
			if (float1.Value < float2.Value)
			{
				lessThanBool.Value = true;
			}
			else
			{
				lessThanBool.Value = false;
			}
			if (float1.Value > float2.Value)
			{
				greaterThanBool.Value = true;
			}
			else
			{
				greaterThanBool.Value = false;
			}
		}
	}
}
