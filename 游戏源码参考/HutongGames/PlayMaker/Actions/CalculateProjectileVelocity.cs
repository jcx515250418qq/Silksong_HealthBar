using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Calculates the required initial velocity required to launch a Rigidbody2D and have it land exactly at the target position (assuming target is on the ground).")]
	public class CalculateProjectileVelocity : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		private Transform transform;

		private Rigidbody2D body;

		private Collider2D collider;

		public FsmVector2 TargetPosition;

		public FsmFloat FireAngle;

		[UIHint(UIHint.Variable)]
		public FsmVector2 ImpulseVelocity;

		public bool EveryFrame;

		public override void Reset()
		{
			Target = null;
			TargetPosition = null;
			FireAngle = null;
			ImpulseVelocity = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			transform = safe.transform;
			body = safe.GetComponent<Rigidbody2D>();
			collider = safe.GetComponent<Collider2D>();
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (EveryFrame)
			{
				DoAction();
			}
		}

		private void DoAction()
		{
			Vector2 value = TargetPosition.Value;
			Vector2 vector = (collider ? transform.TransformPoint(collider.offset) : transform.position);
			float num = Mathf.Abs(Physics2D.gravity.y) * body.gravityScale;
			if (Math.Abs(num) < Mathf.Epsilon)
			{
				Debug.LogError("Projectile gravity is 0! Ballistic velocity will also be zero!", base.Owner);
			}
			Vector2 value2 = CalculateBallisticVelocity(vector, value, num, FireAngle.Value);
			Debug.DrawLine(vector, vector + value2.normalized);
			ImpulseVelocity.Value = value2;
		}

		private Vector3 CalculateBallisticVelocity(Vector2 sourcePos, Vector2 targetPos, float gravity, float angle)
		{
			Vector2 vector = targetPos - sourcePos;
			float y = vector.y;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			float num = angle * (MathF.PI / 180f);
			vector.y = magnitude * Mathf.Tan(num);
			magnitude += y / Mathf.Tan(num);
			return Mathf.Sqrt(magnitude * gravity / Mathf.Sin(2f * num)) * vector.normalized;
		}
	}
}
