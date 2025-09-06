using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and store it as a float.")]
	public class DistanceBetweenPoints : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat distanceResult;

		[RequiredField]
		public FsmVector3 point1;

		[RequiredField]
		public FsmVector3 point2;

		public bool ignoreX;

		public bool ignoreY;

		public bool ignoreZ;

		public bool everyFrame;

		public override void Reset()
		{
			distanceResult = null;
			point1 = null;
			point2 = null;
			ignoreX = false;
			ignoreY = false;
			ignoreZ = false;
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
			if (distanceResult == null)
			{
				return;
			}
			if (ignoreX && ignoreY)
			{
				_ = ignoreZ;
			}
			if (ignoreX)
			{
				if (ignoreY)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.z - point2.Value.z);
					return;
				}
				if (ignoreZ)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.y - point2.Value.y);
					return;
				}
				Vector2 a = new Vector2(point1.Value.y, point1.Value.z);
				Vector2 b = new Vector2(point2.Value.y, point2.Value.z);
				distanceResult.Value = Vector2.Distance(a, b);
			}
			else if (ignoreY)
			{
				if (ignoreX)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.z - point2.Value.z);
					return;
				}
				if (ignoreZ)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.x - point2.Value.x);
					return;
				}
				Vector2 a2 = new Vector2(point1.Value.x, point1.Value.z);
				Vector2 b2 = new Vector2(point2.Value.x, point2.Value.z);
				distanceResult.Value = Vector2.Distance(a2, b2);
			}
			else if (ignoreZ)
			{
				if (ignoreX)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.y - point2.Value.y);
					return;
				}
				if (ignoreY)
				{
					distanceResult.Value = Mathf.Abs(point1.Value.x - point2.Value.x);
					return;
				}
				Vector2 a3 = new Vector2(point1.Value.x, point1.Value.y);
				Vector2 b3 = new Vector2(point2.Value.x, point2.Value.y);
				distanceResult.Value = Vector2.Distance(a3, b3);
			}
			else
			{
				Vector3 a4 = new Vector3(point1.Value.x, point1.Value.y, point1.Value.z);
				Vector2 vector = new Vector3(point2.Value.x, point2.Value.y, point2.Value.z);
				distanceResult.Value = Vector3.Distance(a4, vector);
			}
		}
	}
}
