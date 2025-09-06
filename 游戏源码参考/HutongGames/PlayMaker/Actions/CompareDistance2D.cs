using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and compare it against a known distance value.")]
	public class CompareDistance2D : FsmStateAction
	{
		[RequiredField]
		public FsmVector2 point1;

		[RequiredField]
		public FsmVector2 point2;

		[RequiredField]
		public FsmFloat knownDistance;

		public bool everyFrame;

		private float sqrDistanceTest;

		public override void Reset()
		{
			point1 = null;
			point2 = null;
			knownDistance = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			sqrDistanceTest = knownDistance.Value * knownDistance.Value;
			DoCompareDistance();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompareDistance();
		}

		private void DoCalcDistance()
		{
		}

		private void DoCompareDistance()
		{
			Vector2 vector = new Vector2(point1.Value.x, point1.Value.y);
			Vector2 vector2 = new Vector2(point2.Value.x, point2.Value.y);
			_ = (vector - vector2).magnitude;
			_ = knownDistance.Value;
		}
	}
}
