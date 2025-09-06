using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y until 0 reached.")]
	public class Decelerate : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat deceleration;

		public override void Reset()
		{
			gameObject = null;
			deceleration = 0f;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
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
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x += deceleration.Value;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			else if (linearVelocity.x > 0f)
			{
				linearVelocity.x -= deceleration.Value;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.y < 0f)
			{
				linearVelocity.y += deceleration.Value;
				if (linearVelocity.y > 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			else if (linearVelocity.y > 0f)
			{
				linearVelocity.y -= deceleration.Value;
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
