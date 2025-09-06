using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Travel in a straight line towards target at set speed.")]
	public class FireAtTarget : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmGameObject target;

		[RequiredField]
		public FsmFloat speed;

		public FsmVector3 position;

		public FsmFloat spread;

		private FsmGameObject self;

		private FsmFloat x;

		private FsmFloat y;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			speed = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			if (self == null || rb2d == null || target.Value == null)
			{
				Finish();
				return;
			}
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetVelocity()
		{
			if (!(rb2d == null) && !(target.Value == null))
			{
				float num = target.Value.transform.position.y + position.Value.y - self.Value.transform.position.y;
				float num2 = target.Value.transform.position.x + position.Value.x - self.Value.transform.position.x;
				float num3 = Mathf.Atan2(num, num2) * (180f / MathF.PI);
				if (!spread.IsNone)
				{
					num3 += UnityEngine.Random.Range(0f - spread.Value, spread.Value);
				}
				x = speed.Value * Mathf.Cos(num3 * (MathF.PI / 180f));
				y = speed.Value * Mathf.Sin(num3 * (MathF.PI / 180f));
				Vector2 linearVelocity = default(Vector2);
				linearVelocity.x = x.Value;
				linearVelocity.y = y.Value;
				rb2d.linearVelocity = linearVelocity;
			}
		}
	}
}
