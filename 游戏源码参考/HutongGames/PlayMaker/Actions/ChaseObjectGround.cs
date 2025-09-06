using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object runs towards target")]
	public class ChaseObjectGround : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat xOffset;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		public bool animateTurnAndRun;

		public FsmString runAnimation;

		public FsmString turnAnimation;

		public FsmFloat turnRange;

		public bool snapTo;

		public bool snapSpeedOnly;

		public bool onlyOnStateEntry;

		private FsmGameObject self;

		private tk2dSpriteAnimator animator;

		private bool turning;

		private bool movingRight;

		private bool snapFrame;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			xOffset = null;
			acceleration = 0f;
			speedMax = 0f;
			snapTo = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			animator = self.Value.GetComponent<tk2dSpriteAnimator>();
			DoChase();
			if (onlyOnStateEntry)
			{
				Finish();
			}
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			if (!snapFrame)
			{
				DoChase();
			}
			else
			{
				snapFrame = false;
			}
		}

		private void DoChase()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			float num = target.Value.transform.position.x + xOffset.Value;
			Vector3 position = self.Value.transform.position;
			if (position.x < num - turnRange.Value)
			{
				linearVelocity.x += acceleration.Value;
				movingRight = true;
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
			else if (position.x > num + turnRange.Value)
			{
				linearVelocity.x -= acceleration.Value;
				movingRight = false;
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
			else if (!snapTo)
			{
				if (movingRight)
				{
					linearVelocity.x += acceleration.Value;
				}
				else
				{
					linearVelocity.x -= acceleration.Value;
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
			if (snapTo && ((rb2d.linearVelocity.x < 0f && position.x < num) || (rb2d.linearVelocity.x > 0f && position.x > num)))
			{
				DoSnap();
			}
			rb2d.linearVelocity = linearVelocity;
		}

		private void DoSnap()
		{
			if (!snapSpeedOnly)
			{
				Vector3 position = self.Value.transform.position;
				self.Value.transform.position = new Vector3(target.Value.transform.position.x + xOffset.Value, position.y, position.z);
			}
			rb2d.linearVelocity = new Vector3(0f, rb2d.linearVelocity.y);
			snapFrame = true;
		}
	}
}
