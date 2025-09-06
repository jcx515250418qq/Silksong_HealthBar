using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Movement for swaying ghosts, like zote salubra")]
	public class GhostMovement : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat xPosMin;

		public FsmFloat xPosMax;

		public FsmFloat accel_x;

		public FsmFloat speedMax_x;

		public FsmFloat yPosMin;

		public FsmFloat yPosMax;

		public FsmFloat accel_y;

		public FsmFloat speedMax_y;

		private FsmGameObject target;

		private Transform transform;

		public FsmInt direction_x;

		public FsmInt direction_y;

		public override void Reset()
		{
			gameObject = null;
			xPosMin = null;
			xPosMax = null;
			accel_x = null;
			speedMax_x = null;
			yPosMin = null;
			yPosMax = null;
			accel_y = null;
			speedMax_y = null;
			target = null;
			direction_x = null;
			direction_y = null;
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
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			transform = target.Value.GetComponent<Transform>();
			DoMove();
		}

		public override void OnFixedUpdate()
		{
			DoMove();
		}

		private void DoMove()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			Vector3 position = transform.position;
			if (direction_x.Value == 0)
			{
				if (linearVelocity.x > 0f - speedMax_x.Value)
				{
					linearVelocity.x -= accel_x.Value;
					if (linearVelocity.x < 0f - speedMax_x.Value)
					{
						linearVelocity.x = 0f - speedMax_x.Value;
					}
				}
				if (position.x < xPosMin.Value)
				{
					direction_x.Value = 1;
				}
			}
			else
			{
				if (linearVelocity.x < speedMax_x.Value)
				{
					linearVelocity.x += accel_x.Value;
					if (linearVelocity.x > speedMax_x.Value)
					{
						linearVelocity.x = speedMax_x.Value;
					}
				}
				if (position.x > xPosMax.Value)
				{
					direction_x.Value = 0;
				}
			}
			if (direction_y.Value == 0)
			{
				if (linearVelocity.y > 0f - speedMax_y.Value)
				{
					linearVelocity.y -= accel_y.Value;
					if (linearVelocity.y < 0f - speedMax_y.Value)
					{
						linearVelocity.y = 0f - speedMax_y.Value;
					}
				}
				if (position.y < yPosMin.Value)
				{
					direction_y.Value = 1;
				}
			}
			else
			{
				if (linearVelocity.y < speedMax_y.Value)
				{
					linearVelocity.y += accel_y.Value;
					if (linearVelocity.y > speedMax_y.Value)
					{
						linearVelocity.y = speedMax_y.Value;
					}
				}
				if (position.y > yPosMax.Value)
				{
					direction_y.Value = 0;
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}
