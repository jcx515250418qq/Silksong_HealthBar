using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	[Tooltip("Get the angle between two vector3 positions. 0 is right, 90 is up etc.")]
	public class GetAngleBetweenPoints : FsmStateAction
	{
		[RequiredField]
		public FsmVector3 point1;

		[RequiredField]
		public FsmVector3 point2;

		[RequiredField]
		public FsmFloat storeAngle;

		private FsmFloat x;

		private FsmFloat y;

		public bool everyFrame;

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			DoGetAngle();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetAngle();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoGetAngle()
		{
			float num = point1.Value.y - point2.Value.y;
			float num2 = point1.Value.x - point2.Value.x;
			float num3;
			for (num3 = Mathf.Atan2(num, num2) * (180f / MathF.PI); num3 < 0f; num3 += 360f)
			{
			}
			storeAngle.Value = num3;
		}
	}
}
