using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Flies and keeps a certain distance from target, with smoother movement")]
	public class DirectlyFlyTo : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat minSpeed;

		public FsmFloat maxSpeed;

		public FsmFloat maxSpeedDistance;

		public FsmFloat targetRadius;

		public FsmVector3 offset;

		private FsmGameObject self;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			targetRadius = 0.1f;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoFlyTo();
		}

		public override void OnUpdate()
		{
			DoFlyTo();
		}

		private void DoFlyTo()
		{
			if (rb2d == null)
			{
				return;
			}
			float num = Mathf.Sqrt(Mathf.Pow(self.Value.transform.position.x - (target.Value.transform.position.x + offset.Value.x), 2f) + Mathf.Pow(self.Value.transform.position.y - (target.Value.transform.position.y + offset.Value.y), 2f));
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (!(num > 0f - targetRadius.Value) || !(num < targetRadius.Value))
			{
				float y = target.Value.transform.position.y + offset.Value.y - self.Value.transform.position.y;
				float x = target.Value.transform.position.x + offset.Value.x - self.Value.transform.position.x;
				float num2;
				for (num2 = Mathf.Atan2(y, x) * (180f / MathF.PI); num2 < 0f; num2 += 360f)
				{
				}
				float num3;
				if (num > maxSpeedDistance.Value)
				{
					num3 = maxSpeed.Value;
				}
				else
				{
					float num4 = num / maxSpeedDistance.Value;
					num3 = maxSpeed.Value * num4;
					if (num3 < minSpeed.Value)
					{
						num3 = minSpeed.Value;
					}
				}
				float x2 = num3 * Mathf.Cos(num2 * (MathF.PI / 180f));
				float y2 = num3 * Mathf.Sin(num2 * (MathF.PI / 180f));
				linearVelocity.x = x2;
				linearVelocity.y = y2;
				rb2d.linearVelocity = linearVelocity;
			}
			else
			{
				linearVelocity = new Vector2(0f, 0f);
				rb2d.linearVelocity = linearVelocity;
			}
		}
	}
}
