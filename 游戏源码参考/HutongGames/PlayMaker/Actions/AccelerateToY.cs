using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToY : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat accelerationFactor;

		public FsmFloat targetSpeed;

		public override void Reset()
		{
			gameObject = null;
			accelerationFactor = null;
			targetSpeed = null;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoAccelerate();
		}

		public override void OnFixedUpdate()
		{
			DoAccelerate();
		}

		private void DoAccelerate()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (linearVelocity.y > targetSpeed.Value)
			{
				linearVelocity.y -= accelerationFactor.Value;
				if (linearVelocity.y < targetSpeed.Value)
				{
					linearVelocity.y = targetSpeed.Value;
				}
			}
			if (linearVelocity.y < targetSpeed.Value)
			{
				linearVelocity.y += accelerationFactor.Value;
				if (linearVelocity.y > targetSpeed.Value)
				{
					linearVelocity.y = targetSpeed.Value;
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
