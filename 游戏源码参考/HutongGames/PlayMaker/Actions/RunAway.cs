using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object runs away from target")]
	public class RunAway : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		public bool animateTurnAndRun;

		public FsmString runAnimation;

		public FsmString turnAnimation;

		private FsmGameObject self;

		private tk2dSpriteAnimator animator;

		private bool turning;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			acceleration = 0f;
			speedMax = 0f;
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
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			animator = self.Value.GetComponent<tk2dSpriteAnimator>();
			DoChase();
		}

		public override void OnFixedUpdate()
		{
			DoChase();
		}

		private void DoChase()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (self.Value.transform.position.x < target.Value.transform.position.x)
			{
				linearVelocity.x -= acceleration.Value;
				if (animateTurnAndRun)
				{
					if (linearVelocity.x < 0f && !turning)
					{
						animator.Play(turnAnimation.Value);
						turning = true;
					}
					if (linearVelocity.x > 0f && turning)
					{
						animator.Play(runAnimation.Value);
						turning = false;
					}
				}
			}
			else
			{
				linearVelocity.x += acceleration.Value;
				if (animateTurnAndRun)
				{
					if (linearVelocity.x > 0f && !turning)
					{
						animator.Play(turnAnimation.Value);
						turning = true;
					}
					if (linearVelocity.x < 0f && turning)
					{
						animator.Play(runAnimation.Value);
						turning = false;
					}
				}
			}
			if (linearVelocity.x > speedMax.Value)
			{
				linearVelocity.x = speedMax.Value;
			}
			if (linearVelocity.x < 0f - speedMax.Value)
			{
				linearVelocity.x = 0f - speedMax.Value;
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
