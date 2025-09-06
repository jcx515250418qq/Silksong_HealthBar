using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class ConvertNormalToAngle : RigidBody2dActionBase
	{
		public FsmVector3 contactNormal;

		public FsmFloat storeAngle;

		public bool everyFrame;

		public override void Reset()
		{
			contactNormal = null;
			storeAngle = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			Convert();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			Convert();
		}

		private void Convert()
		{
			Vector2 vector = new Vector2(contactNormal.Value.x, contactNormal.Value.y);
			float num = Mathf.Atan2(vector.x, 0f - vector.y) * 180f / MathF.PI - 90f;
			if (num < 0f)
			{
				num += 360f;
			}
			storeAngle.Value = num;
		}
	}
}
