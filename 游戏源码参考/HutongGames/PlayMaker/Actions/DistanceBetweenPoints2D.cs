using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and store it as a float.")]
	public class DistanceBetweenPoints2D : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat distanceResult;

		[RequiredField]
		public FsmVector2 point1;

		[RequiredField]
		public FsmVector2 point2;

		public bool everyFrame;

		public override void Reset()
		{
			distanceResult = null;
			point1 = null;
			point2 = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCalcDistance();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCalcDistance();
		}

		private void DoCalcDistance()
		{
			if (distanceResult != null)
			{
				Vector2 a = new Vector2(point1.Value.x, point1.Value.y);
				Vector2 b = new Vector2(point2.Value.x, point2.Value.y);
				float value = Vector2.Distance(a, b);
				distanceResult.Value = value;
			}
		}
	}
}
