using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Accelerates objects velocity, and clamps top speed")]
	public class AccelerateVelocity : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat xAccel;

		public FsmFloat yAccel;

		public FsmFloat xMaxSpeed;

		public FsmFloat yMaxSpeed;

		public override void Reset()
		{
			gameObject = null;
			xAccel = new FsmFloat
			{
				UseVariable = true
			};
			yAccel = new FsmFloat
			{
				UseVariable = true
			};
			xMaxSpeed = new FsmFloat
			{
				UseVariable = true
			};
			yMaxSpeed = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			DoSetVelocity();
		}

		private void DoSetVelocity()
		{
			if (!(rb2d == null))
			{
				Vector2 linearVelocity = rb2d.linearVelocity;
				if (!xAccel.IsNone)
				{
					float value = linearVelocity.x + xAccel.Value;
					value = Mathf.Clamp(value, 0f - xMaxSpeed.Value, xMaxSpeed.Value);
					linearVelocity = new Vector2(value, linearVelocity.y);
				}
				if (!yAccel.IsNone)
				{
					float value2 = linearVelocity.y + yAccel.Value;
					value2 = Mathf.Clamp(value2, 0f - yMaxSpeed.Value, yMaxSpeed.Value);
					linearVelocity = new Vector2(linearVelocity.x, value2);
				}
				rb2d.linearVelocity = linearVelocity;
			}
		}
	}
}
