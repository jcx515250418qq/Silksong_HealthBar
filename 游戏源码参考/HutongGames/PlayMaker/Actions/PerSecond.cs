using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Multiplies a Float by Time.deltaTime to use in frame-rate independent operations. E.g., 10 becomes 10 units per second.")]
	public class PerSecond : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The float value to multiply be Time.deltaTime.")]
		public FsmFloat floatValue;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat storeResult;

		[Tooltip("Do it every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			floatValue = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoPerSecond();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoPerSecond();
		}

		private void DoPerSecond()
		{
			if (storeResult != null)
			{
				storeResult.Value = floatValue.Value * Time.deltaTime;
			}
		}
	}
}
