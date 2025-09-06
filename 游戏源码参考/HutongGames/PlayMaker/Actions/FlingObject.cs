using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Fling")]
	public class FlingObject : RigidBody2dActionBase
	{
		[RequiredField]
		public FsmOwnerDefault flungObject;

		public FsmFloat speedMin;

		public FsmFloat speedMax;

		public FsmFloat angleMin;

		public FsmFloat angleMax;

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		public override void Reset()
		{
			flungObject = null;
			speedMin = null;
			speedMax = null;
			angleMin = null;
			angleMax = null;
		}

		public override void OnEnter()
		{
			if (flungObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(flungObject);
				if (ownerDefaultTarget != null)
				{
					float num = UnityEngine.Random.Range(speedMin.Value, speedMax.Value);
					float num2 = UnityEngine.Random.Range(angleMin.Value, angleMax.Value);
					vectorX = num * Mathf.Cos(num2 * (MathF.PI / 180f));
					vectorY = num * Mathf.Sin(num2 * (MathF.PI / 180f));
					Vector2 linearVelocity = default(Vector2);
					linearVelocity.x = vectorX;
					linearVelocity.y = vectorY;
					CacheRigidBody2d(ownerDefaultTarget);
					rb2d.linearVelocity = linearVelocity;
				}
			}
			Finish();
		}
	}
}
