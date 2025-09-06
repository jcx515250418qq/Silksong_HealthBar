using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target. Optionally try to stay on left or right of target")]
	public class DistanceFlyV2 : RigidBody2dActionBase
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

		[UIHint(UIHint.Variable)]
		public FsmFloat maxHeight;

		public FsmBool stayLeft;

		public FsmBool stayRight;

		private float distanceAway;

		private FsmGameObject self;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			targetsHeight = false;
			height = null;
			maxHeight = null;
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
			GameObject value = target.Value;
			if (value == null)
			{
				return;
			}
			GameObject value2 = self.Value;
			_ = value2 == null;
			distanceAway = Mathf.Sqrt(Mathf.Pow(value2.transform.position.x - value.transform.position.x, 2f) + Mathf.Pow(value2.transform.position.y - value.transform.position.y, 2f));
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (distanceAway > distance.Value)
			{
				if (stayLeft.Value && !stayRight.Value && value2.transform.position.x > value.transform.position.x + 1f)
				{
					linearVelocity.x -= acceleration.Value;
				}
				else if (stayRight.Value && !stayLeft.Value && value2.transform.position.x < value.transform.position.x - 1f)
				{
					linearVelocity.x += acceleration.Value;
				}
				else if (value2.transform.position.x < value.transform.position.x)
				{
					linearVelocity.x += acceleration.Value;
				}
				else
				{
					linearVelocity.x -= acceleration.Value;
				}
				if (!targetsHeight)
				{
					if (value2.transform.position.y < value.transform.position.y)
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
				if (stayLeft.Value && !stayRight.Value && value2.transform.position.x > value.transform.position.x + 1f)
				{
					linearVelocity.x -= acceleration.Value;
				}
				else if (stayRight.Value && !stayLeft.Value && value2.transform.position.x < value.transform.position.x - 1f)
				{
					linearVelocity.x += acceleration.Value;
				}
				else if (value2.transform.position.x < value.transform.position.x)
				{
					linearVelocity.x -= acceleration.Value;
				}
				else
				{
					linearVelocity.x += acceleration.Value;
				}
				if (!targetsHeight)
				{
					if (value2.transform.position.y < value.transform.position.y)
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
				if (value2.transform.position.y > value.transform.position.y + height.Value)
				{
					linearVelocity.y -= acceleration.Value;
				}
				else if (!maxHeight.IsNone && maxHeight.Value != 0f && value2.transform.position.y > maxHeight.Value)
				{
					linearVelocity.y -= acceleration.Value;
				}
				else if (value2.transform.position.y < value.transform.position.y + height.Value)
				{
					linearVelocity.y += acceleration.Value;
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
