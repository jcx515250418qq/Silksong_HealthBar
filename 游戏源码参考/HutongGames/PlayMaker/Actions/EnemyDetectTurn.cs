using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class EnemyDetectTurn : FsmStateAction
	{
		private const float SKIN_WIDTH = 0.1f;

		private const float TOP_RAY_PADDING = 0.5f;

		private const float BOTTOM_RAY_PADDING = 0.5f;

		private const float DOWN_RAY_DISTANCE = 0.5f;

		private const int LAYERMASK = 33024;

		public FsmOwnerDefault Target;

		public FsmFloat WallDistance;

		public FsmFloat GroundAheadDistance;

		public FsmBool DefaultFacingRight;

		public FsmEvent ShouldTurn;

		public bool EveryFrame;

		private bool isMoving;

		private BoxCollider2D box;

		private Rigidbody2D body;

		private Transform transform;

		public override void Reset()
		{
			Target = null;
			WallDistance = 1f;
			GroundAheadDistance = 0.5f;
			DefaultFacingRight = null;
			EveryFrame = true;
		}

		public override void OnDrawActionGizmos()
		{
			CacheComponents();
			if ((bool)box)
			{
				IsRaysHittingWall(isDrawingGizmos: true);
				IsRaysHittingGroundFront(isDrawingGizmos: true);
				IsRaysHittingGroundCentre(isDrawingGizmos: true);
			}
		}

		public override void OnEnter()
		{
			CacheComponents();
			Evaluate();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			isMoving = Mathf.Abs(body.linearVelocity.x) > 0f;
			Evaluate();
		}

		private void Evaluate()
		{
			if (IsRaysHittingWall() || (!IsRaysHittingGroundFront() && IsRaysHittingGroundCentre()))
			{
				base.Fsm.Event(ShouldTurn);
			}
		}

		private void CacheComponents()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				transform = safe.transform;
				box = safe.GetComponent<BoxCollider2D>();
			}
		}

		private bool IsRaysHittingWall(bool isDrawingGizmos = false)
		{
			float movingDirection = GetMovingDirection();
			UnityEngine.Bounds bounds = box.bounds;
			Vector2 vector = bounds.max;
			Vector2 vector2 = bounds.min;
			Vector2 vector3 = ((movingDirection > 0f) ? Vector2.right : Vector2.left);
			if (movingDirection < 0f)
			{
				vector.x = vector2.x;
			}
			else
			{
				vector2.x = vector.x;
			}
			vector.x -= 0.1f * movingDirection;
			vector2.x -= 0.1f * movingDirection;
			vector.y -= 0.5f;
			vector2.y += 0.5f;
			float num = (body ? Mathf.Max(WallDistance.Value, body.linearVelocity.x * Time.fixedDeltaTime) : WallDistance.Value) + 0.1f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector, vector + vector3 * num);
				Gizmos.DrawLine(vector2, vector2 + vector3 * num);
				return false;
			}
			bool num2 = Helper.IsRayHittingNoTriggers(vector, vector3, num, 33024);
			bool flag = Helper.IsRayHittingNoTriggers(vector2, vector3, num, 33024);
			return num2 || flag;
		}

		private bool IsRaysHittingGroundFront(bool isDrawingGizmos = false)
		{
			float movingDirection = GetMovingDirection();
			UnityEngine.Bounds bounds = box.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			Vector2 vector3 = bounds.center;
			if (movingDirection > 0f)
			{
				vector3.x = vector2.x + GroundAheadDistance.Value;
			}
			else
			{
				vector3.x = vector.x - GroundAheadDistance.Value;
			}
			float num = vector3.y - vector.y + 0.5f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector3, vector3 + Vector2.down * num);
				return false;
			}
			return Helper.IsRayHittingNoTriggers(vector3, Vector2.down, num, 33024);
		}

		private bool IsRaysHittingGroundCentre(bool isDrawingGizmos = false)
		{
			GetMovingDirection();
			UnityEngine.Bounds bounds = box.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.center;
			float num = vector2.y - vector.y + 0.5f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector2, vector2 + Vector2.down * num);
				return false;
			}
			return Helper.IsRayHittingNoTriggers(vector2, Vector2.down, num, 33024);
		}

		private float GetMovingDirection()
		{
			if (!body || body.linearVelocity.x == 0f)
			{
				return GetFacingDirection();
			}
			return Mathf.Sign(body.linearVelocity.x);
		}

		private float GetFacingDirection()
		{
			return Mathf.Sign(transform.localScale.x) * (float)(DefaultFacingRight.Value ? 1 : (-1));
		}
	}
}
