using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class DistanceFlyHorizontal : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat distance;

		public FsmFloat startY;

		public FsmFloat floorDistance;

		public FsmFloat yDistanceAllowance;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		public FsmBool stayLeft;

		public FsmBool stayRight;

		public FsmBool dontAffectY;

		private FsmGameObject self;

		private LayerMask layerMask;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			distance = null;
			yDistanceAllowance = null;
			startY = null;
			floorDistance = null;
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
			layerMask = LayerMask.GetMask("Terrain");
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
			Vector3 position = self.Value.transform.position;
			float num = Mathf.Sqrt(Mathf.Pow(position.x - target.Value.transform.position.x, 2f));
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (num > distance.Value)
			{
				if (stayLeft.Value && !stayRight.Value && position.x > target.Value.transform.position.x + 1f)
				{
					linearVelocity.x -= acceleration.Value;
				}
				else if (stayRight.Value && !stayLeft.Value && position.x < target.Value.transform.position.x - 1f)
				{
					linearVelocity.x += acceleration.Value;
				}
				else if (position.x < target.Value.transform.position.x)
				{
					linearVelocity.x += acceleration.Value;
				}
				else
				{
					linearVelocity.x -= acceleration.Value;
				}
			}
			else if (stayLeft.Value && !stayRight.Value && self.Value.transform.position.x > target.Value.transform.position.x + 1f)
			{
				linearVelocity.x -= acceleration.Value;
			}
			else if (stayRight.Value && !stayLeft.Value && self.Value.transform.position.x < target.Value.transform.position.x - 1f)
			{
				linearVelocity.x += acceleration.Value;
			}
			else if (self.Value.transform.position.x < target.Value.transform.position.x)
			{
				linearVelocity.x -= acceleration.Value;
			}
			else
			{
				linearVelocity.x += acceleration.Value;
			}
			if (!dontAffectY.Value)
			{
				if (floorDistance.Value == 0f)
				{
					if (position.y < startY.Value - yDistanceAllowance.Value)
					{
						linearVelocity.y += acceleration.Value;
					}
					else if (position.y > startY.Value + yDistanceAllowance.Value)
					{
						linearVelocity.y -= acceleration.Value;
					}
				}
				else if (Helper.Raycast2D(position, Vector2.down, floorDistance.Value, layerMask).collider == null)
				{
					linearVelocity.y -= acceleration.Value;
				}
				else
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
			if (!dontAffectY.Value)
			{
				if (linearVelocity.y > speedMax.Value)
				{
					linearVelocity.y = speedMax.Value;
				}
				if (linearVelocity.y < 0f - speedMax.Value)
				{
					linearVelocity.y = 0f - speedMax.Value;
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
