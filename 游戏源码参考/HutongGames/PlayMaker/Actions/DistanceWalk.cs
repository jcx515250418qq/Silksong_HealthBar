using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalk : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat distance;

		public FsmFloat speed;

		public FsmFloat range;

		public bool changeAnimation;

		public bool spriteFacesRight;

		public FsmString forwardAnimation;

		public FsmString backAnimation;

		private float distanceAway;

		private FsmGameObject self;

		private tk2dSpriteAnimator animator;

		private bool movingRight;

		private float ANIM_CHANGE_TIME = 0.6f;

		private float changeTimer;

		private bool randomStart;

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
			changeTimer = 0f;
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
			distanceAway = self.Value.transform.position.x - target.Value.transform.position.x;
			if (distanceAway < 0f)
			{
				distanceAway *= -1f;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (distanceAway > distance.Value + range.Value)
			{
				if (self.Value.transform.position.x < target.Value.transform.position.x)
				{
					if (!movingRight && changeTimer <= 0f)
					{
						linearVelocity.x = speed.Value;
						movingRight = true;
						changeTimer = ANIM_CHANGE_TIME;
					}
				}
				else if (movingRight && changeTimer <= 0f)
				{
					linearVelocity.x = 0f - speed.Value;
					movingRight = false;
					changeTimer = ANIM_CHANGE_TIME;
				}
			}
			else if (distanceAway < distance.Value - range.Value)
			{
				if (self.Value.transform.position.x < target.Value.transform.position.x)
				{
					if (movingRight && changeTimer <= 0f)
					{
						linearVelocity.x = 0f - speed.Value;
						movingRight = false;
						changeTimer = ANIM_CHANGE_TIME;
					}
				}
				else if (!movingRight && changeTimer <= 0f)
				{
					linearVelocity.x = speed.Value;
					movingRight = true;
					changeTimer = ANIM_CHANGE_TIME;
				}
			}
			if (rb2d.linearVelocity.x > -0.1f && rb2d.linearVelocity.x < 0.1f)
			{
				if (distanceAway >= distance.Value)
				{
					if (self.Value.transform.position.x < target.Value.transform.position.x)
					{
						linearVelocity.x = speed.Value;
						movingRight = true;
					}
					else
					{
						linearVelocity.x = 0f - speed.Value;
						movingRight = false;
					}
				}
				else if (self.Value.transform.position.x < target.Value.transform.position.x)
				{
					linearVelocity.x = 0f - speed.Value;
					movingRight = false;
				}
				else
				{
					linearVelocity.x = speed.Value;
					movingRight = true;
				}
			}
			rb2d.linearVelocity = linearVelocity;
			if (!changeAnimation)
			{
				return;
			}
			if (self.Value.transform.localScale.x > 0f)
			{
				if ((spriteFacesRight && movingRight) || (!spriteFacesRight && !movingRight))
				{
					animator.Play(forwardAnimation.Value);
				}
				if ((!spriteFacesRight && movingRight) || (spriteFacesRight && !movingRight))
				{
					animator.Play(backAnimation.Value);
				}
			}
			else
			{
				if ((spriteFacesRight && movingRight) || (!spriteFacesRight && !movingRight))
				{
					animator.Play(backAnimation.Value);
				}
				if ((!spriteFacesRight && movingRight) || (spriteFacesRight && !movingRight))
				{
					animator.Play(forwardAnimation.Value);
				}
			}
		}
	}
}
