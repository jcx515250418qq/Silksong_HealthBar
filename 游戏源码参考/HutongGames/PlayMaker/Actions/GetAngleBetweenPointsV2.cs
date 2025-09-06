using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Math")]
	[Tooltip("Get the angle between two vector3 positions. 0 is right, 90 is up etc.")]
	public class GetAngleBetweenPointsV2 : FsmStateAction
	{
		public FsmFloat originX;

		public FsmFloat originY;

		public FsmFloat targetX;

		public FsmFloat targetY;

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
		}

		private void DoGetAngle()
		{
			float num = targetY.Value - originY.Value;
			float num2 = targetX.Value - originX.Value;
			float num3;
			for (num3 = Mathf.Atan2(num, num2) * (180f / MathF.PI); num3 < 0f; num3 += 360f)
			{
			}
			storeAngle.Value = num3;
		}
	}
}
