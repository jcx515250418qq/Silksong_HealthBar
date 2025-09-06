using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToXByScale : RigidBody2dActionBase
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
					velocity.x -= component.conveyorSpeed;
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!useSelfVelocity)
			{
				velocity = rb2d.linearVelocity;
			}
			else
			{
				velocity.y = rb2d.linearVelocity.y;
			}
			float num = ((!(ownerDefaultTarget.transform.localScale.x > 0f)) ? (0f - targetSpeed.Value) : targetSpeed.Value);
			if (velocity.x > num)
			{
				velocity.x -= accelerationFactor.Value;
				if (velocity.x < num)
				{
					velocity.x = num;
				}
			}
			if (velocity.x < num)
			{
				velocity.x += accelerationFactor.Value;
				if (velocity.x > num)
				{
					velocity.x = num;
				}
			}
			rb2d.linearVelocity = velocity;
		}
	}
}
