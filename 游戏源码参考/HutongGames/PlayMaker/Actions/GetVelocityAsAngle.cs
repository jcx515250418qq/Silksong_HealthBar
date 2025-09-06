using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
	public class GetVelocityAsAngle : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		public bool everyFrame;

		public override void Reset()
		{
			storeAngle = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoGetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetVelocity();
		}

		private void DoGetVelocity()
		{
			if (!(rb2d == null))
			{
				Vector2 linearVelocity = rb2d.linearVelocity;
				float num = Mathf.Atan2(linearVelocity.x, 0f - linearVelocity.y) * 180f / MathF.PI - 90f;
				if (num < 0f)
				{
					num += 360f;
				}
				storeAngle.Value = num;
			}
		}
	}
}
