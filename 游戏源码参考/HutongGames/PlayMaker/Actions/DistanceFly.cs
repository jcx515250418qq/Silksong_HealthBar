using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceFly : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat distance;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		[Tooltip("If true, object tries to keep to a certain height relative to target")]
		public bool targetsHeight;

		public FsmFloat height;

		public FsmFloat minAboveHero;

		private float distanceAway;

		private FsmGameObject self;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			targetsHeight = false;
			height = null;
			acceleration = 0f;
			speedMax = 0f;
			minAboveHero = new FsmFloat
			{
				UseVariable = true
			};
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
			DoBuzz();
		}

		public override void OnFixedUpdate()
		{
			DoBuzz();
		}

		private void DoBuzz()
		{
			if (rb2d == null)
			{
				return;
			}
			distanceAway = Mathf.Sqrt(Mathf.Pow(self.Value.transform.position.x - target.Value.transform.position.x, 2f) + Mathf.Pow(self.Value.transform.position.y - target.Value.transform.position.y, 2f));
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (distanceAway > distance.Value)
			{
				if (self.Value.transform.position.x < target.Value.transform.position.x)
				{
					linearVelocity.x += acceleration.Value;
				}
				else
				{
					linearVelocity.x -= acceleration.Value;
				}
				if (!targetsHeight && minAboveHero.IsNone)
				{
					if (self.Value.transform.position.y < target.Value.transform.position.y)
					{
						linearVelocity.y += acceleration.Value;
					}
					else
					{
						linearVelocity.y -= acceleration.Value;
					}
				}
			}
			else
			{
				if (self.Value.transform.position.x < target.Value.transform.position.x)
				{
					linearVelocity.x -= acceleration.Value;
				}
				else
				{
					linearVelocity.x += acceleration.Value;
				}
				if (!targetsHeight && minAboveHero.IsNone)
				{
					if (self.Value.transform.position.y < target.Value.transform.position.y)
					{
						linearVelocity.y -= acceleration.Value;
					}
					else
					{
						linearVelocity.y += acceleration.Value;
					}
				}
			}
			if (targetsHeight)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y + height.Value)
				{
					linearVelocity.y += acceleration.Value;
				}
				if (self.Value.transform.position.y > target.Value.transform.position.y + height.Value)
				{
					linearVelocity.y -= acceleration.Value;
				}
			}
			if (!minAboveHero.IsNone)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y + minAboveHero.Value)
				{
					linearVelocity.y += acceleration.Value;
				}
				else
				{
					linearVelocity.y -= acceleration.Value;
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
			if (linearVelocity.y > speedMax.Value)
			{
				linearVelocity.y = speedMax.Value;
			}
			if (linearVelocity.y < 0f - speedMax.Value)
			{
				linearVelocity.y = 0f - speedMax.Value;
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
