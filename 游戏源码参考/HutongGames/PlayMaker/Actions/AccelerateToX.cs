using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToX : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat accelerationFactor;

		public FsmFloat targetSpeed;

		private bool useSelfVelocity;

		private Vector2 velocity;

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
			if ((bool)rb2d)
			{
				HeroController component = rb2d.GetComponent<HeroController>();
				velocity = rb2d.linearVelocity;
				if ((bool)component)
				{
					useSelfVelocity = true;
					if (component.cState.inConveyorZone)
					{
						velocity.x -= component.conveyorSpeed;
					}
				}
				else
				{
					useSelfVelocity = false;
				}
			}
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
			if (!useSelfVelocity)
			{
				velocity = rb2d.linearVelocity;
			}
			else
			{
				velocity.y = rb2d.linearVelocity.y;
			}
			if (velocity.x > targetSpeed.Value)
			{
				velocity.x -= accelerationFactor.Value;
				if (velocity.x < targetSpeed.Value)
				{
					velocity.x = targetSpeed.Value;
				}
			}
			if (velocity.x < targetSpeed.Value)
			{
				velocity.x += accelerationFactor.Value;
				if (velocity.x > targetSpeed.Value)
				{
					velocity.x = targetSpeed.Value;
				}
			}
			rb2d.linearVelocity = velocity;
		}
	}
}
