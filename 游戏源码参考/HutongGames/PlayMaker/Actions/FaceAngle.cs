using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object rotates to face direction it is travelling in.")]
	public class FaceAngle : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Offset the angle. If sprite faces right, leave as 0.")]
		public FsmFloat angleOffset;

		public FsmFloat angleOffsetIfMirrored;

		[Tooltip("Optionally, rotate another object based on our own velocity")]
		public FsmGameObject otherGameObject;

		public bool everyFrame;

		private GameObject target;

		public override void Reset()
		{
			gameObject = null;
			angleOffset = 0f;
			everyFrame = false;
			otherGameObject = null;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoAngle();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAngle();
		}

		private void DoAngle()
		{
			if (!(rb2d == null))
			{
				Vector2 linearVelocity = rb2d.linearVelocity;
				float num = ((!angleOffsetIfMirrored.IsNone && !(target.transform.localScale.x > 0f)) ? angleOffsetIfMirrored.Value : angleOffset.Value);
				float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI) + num;
				if (!otherGameObject.IsNone && otherGameObject.Value != null)
				{
					otherGameObject.Value.transform.localEulerAngles = new Vector3(0f, 0f, z);
				}
				else
				{
					target.transform.localEulerAngles = new Vector3(0f, 0f, z);
				}
			}
		}
	}
}
