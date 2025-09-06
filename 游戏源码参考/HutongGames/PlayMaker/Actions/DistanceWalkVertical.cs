using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalkVertical : RigidBody2dActionBase
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

		private bool movingUp;

		private float ANIM_CHANGE_TIME = 0.6f;

		private float changeTimer;

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
						linearVelocity.y = speed.Value;
						movingUp = true;
						changeTimer = ANIM_CHANGE_TIME;
					}
				}
				else if (movingUp && changeTimer <= 0f)
				{
					linearVelocity.y = 0f - speed.Value;
					movingUp = false;
					changeTimer = ANIM_CHANGE_TIME;
				}
			}
			else if (distanceAway < distance.Value - range.Value)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y)
				{
					if (movingUp && changeTimer <= 0f)
					{
						linearVelocity.y = 0f - speed.Value;
						movingUp = false;
						changeTimer = ANIM_CHANGE_TIME;
					}
				}
				else if (!movingUp && changeTimer <= 0f)
				{
					linearVelocity.y = speed.Value;
					movingUp = true;
					changeTimer = ANIM_CHANGE_TIME;
				}
			}
			if (rb2d.linearVelocity.y > -0.1f && rb2d.linearVelocity.y < 0.1f)
			{
				if (Random.value > 0.5f)
				{
					linearVelocity.y = speed.Value;
					movingUp = true;
				}
				else
				{
					linearVelocity.y = 0f - speed.Value;
					movingUp = false;
				}
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
