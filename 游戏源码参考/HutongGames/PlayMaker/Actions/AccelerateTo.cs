using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y separately. Uses multiplication.")]
	public class AccelerateTo : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat decelerationX;

		public FsmFloat decelerationY;

		public override void Reset()
		{
			gameObject = null;
			decelerationX = null;
			decelerationY = null;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DecelerateSelf();
		}

		public override void OnFixedUpdate()
		{
			DecelerateSelf();
		}

		private void DecelerateSelf()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (!decelerationX.IsNone)
			{
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x *= decelerationX.Value;
					if (linearVelocity.x > 0f)
					{
						linearVelocity.x = 0f;
					}
				}
				else if (linearVelocity.x > 0f)
				{
					linearVelocity.x *= decelerationX.Value;
					if (linearVelocity.x < 0f)
					{
						linearVelocity.x = 0f;
					}
				}
			}
			if (!decelerationY.IsNone)
			{
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y *= decelerationY.Value;
					if (linearVelocity.y > 0f)
					{
						linearVelocity.y = 0f;
					}
				}
				else if (linearVelocity.y > 0f)
				{
					linearVelocity.y *= decelerationY.Value;
					if (linearVelocity.y < 0f)
					{
						linearVelocity.y = 0f;
					}
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
