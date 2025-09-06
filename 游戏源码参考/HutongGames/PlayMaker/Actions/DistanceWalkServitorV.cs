using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalkServitorV : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat distance;

		public FsmFloat acceleration;

		public FsmFloat speed;

		public FsmFloat range;

		public bool changeAnimation;

		public bool spriteFacesRight;

		public FsmString forwardAnimation;

		public FsmString backAnimation;

		private float distanceAway;

		private FsmGameObject self;

		private tk2dSpriteAnimator animator;

		private bool movingUp;

		private const float ANIM_CHANGE_TIME = 0.5f;

		private float changeTimer;

		private float stoppedTimer;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			speed = 0f;
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
			if (changeAnimation)
			{
				animator = self.Value.GetComponent<tk2dSpriteAnimator>();
			}
			DoWalk();
		}

		public override void OnUpdate()
		{
			if (changeTimer > 0f)
			{
				changeTimer -= Time.deltaTime;
			}
		}

		public override void OnFixedUpdate()
		{
			DoWalk();
		}

		private void DoWalk()
		{
			if (rb2d == null)
			{
				return;
			}
			distanceAway = self.Value.transform.position.y - target.Value.transform.position.y;
			if (distanceAway < 0f)
			{
				distanceAway *= -1f;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (distanceAway > distance.Value + range.Value)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y)
				{
					if (!movingUp && changeTimer <= 0f)
					{
						movingUp = true;
						changeTimer = 0.5f;
					}
				}
				else if (movingUp && changeTimer <= 0f)
				{
					movingUp = false;
					changeTimer = 0.5f;
				}
			}
			else if (distanceAway < distance.Value - range.Value)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y)
				{
					if (movingUp && changeTimer <= 0f)
					{
						movingUp = false;
						changeTimer = 0.5f;
					}
				}
				else if (!movingUp && changeTimer <= 0f)
				{
					movingUp = true;
					changeTimer = 0.5f;
				}
			}
			if (rb2d.linearVelocity.y > -0.1f && rb2d.linearVelocity.y < 0.1f)
			{
				stoppedTimer += Time.deltaTime;
				if (stoppedTimer >= 0.15f)
				{
					if (Random.value > 0.5f)
					{
						movingUp = true;
					}
					else
					{
						movingUp = false;
					}
					stoppedTimer = 0f;
					changeTimer = 0.5f;
				}
			}
			else
			{
				stoppedTimer = 0f;
			}
			if (movingUp)
			{
				linearVelocity.y += acceleration.Value * Time.deltaTime;
			}
			else
			{
				linearVelocity.y -= acceleration.Value * Time.deltaTime;
			}
			if (linearVelocity.y < 0f - speed.Value)
			{
				linearVelocity.y = 0f - speed.Value;
			}
			if (linearVelocity.y > speed.Value)
			{
				linearVelocity.y = speed.Value;
			}
			rb2d.linearVelocity = linearVelocity;
			if (!changeAnimation)
			{
				return;
			}
			if (self.Value.transform.localScale.x > 0f)
			{
				if ((spriteFacesRight && movingUp) || (!spriteFacesRight && !movingUp))
				{
					animator.Play(forwardAnimation.Value);
				}
				if ((!spriteFacesRight && movingUp) || (spriteFacesRight && !movingUp))
				{
					animator.Play(backAnimation.Value);
				}
			}
			else
			{
				if ((spriteFacesRight && movingUp) || (!spriteFacesRight && !movingUp))
				{
					animator.Play(backAnimation.Value);
				}
				if ((!spriteFacesRight && movingUp) || (spriteFacesRight && !movingUp))
				{
					animator.Play(forwardAnimation.Value);
				}
			}
		}
	}
}
