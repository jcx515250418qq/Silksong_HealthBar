using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Finds the furthest ground point for an enemy to jump to without going off an edge.")]
	public class GetGroundPointClampedToEdge : FsmStateAction
	{
		private const float FIT_SKIN_WIDTH_SIDE = 0.1f;

		[RequiredField]
		[CheckForComponent(typeof(BoxCollider2D))]
		public FsmOwnerDefault Source;

		public FsmFloat TargetDistance;

		public FsmFloat MinJumpDistance;

		public FsmBool DefaultFacingRight;

		public FsmFloat ReductionDistance;

		public FsmFloat MaxGroundDistance;

		public FsmFloat GroundRayHeight;

		[UIHint(UIHint.Variable)]
		public FsmBool DidFindGroundPoint;

		[UIHint(UIHint.Variable)]
		public FsmVector2 GroundPoint;

		public bool EveryFrame;

		private Transform transform;

		private BoxCollider2D collider;

		public override void Reset()
		{
			TargetDistance = null;
			MinJumpDistance = new FsmFloat(1f);
			DefaultFacingRight = new FsmBool(true);
			ReductionDistance = new FsmFloat(1f);
			MaxGroundDistance = new FsmFloat(1.5f);
			GroundRayHeight = new FsmFloat(0.1f);
			DidFindGroundPoint = null;
			GroundPoint = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Source.GetSafe(this);
			transform = safe.transform;
			collider = safe.GetComponent<BoxCollider2D>();
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (EveryFrame)
			{
				DoAction();
			}
		}

		private void DoAction()
		{
			bool flag = DefaultFacingRight.Value;
			Vector2 vector = collider.offset - new Vector2(0f, collider.size.y / 2f - GroundRayHeight.Value);
			float num = TargetDistance.Value;
			if (num < 0f)
			{
				flag = !flag;
				num *= -1f;
			}
			float num2 = collider.size.x / 2f;
			Vector2 directionLocal = (flag ? Vector2.right : Vector2.left);
			float length = num + num2;
			RaycastHit2D raycastHit2D = CastRayLocal(vector, directionLocal, length);
			if (raycastHit2D.collider != null)
			{
				num = raycastHit2D.distance - num2;
			}
			Vector2 down = Vector2.down;
			float length2 = MaxGroundDistance.Value + GroundRayHeight.Value;
			while (num >= MinJumpDistance.Value)
			{
				Vector2 vector2 = vector + new Vector2(flag ? num : (0f - num), 0f);
				RaycastHit2D raycastHit2D2 = CastRayLocal(vector2, down, length2);
				if (raycastHit2D2.collider != null)
				{
					Vector2 point = raycastHit2D2.point;
					point.y -= collider.offset.y - collider.size.y / 2f;
					float num3 = collider.offset.x + collider.size.x / 2f;
					if (CastRayLocal(vector2 + new Vector2(num3 - 0.1f, 0f), down, length2, secondaryDebug: true).collider != null)
					{
						point.x += num3 * transform.localScale.x;
					}
					if (CastRayLocal(vector2 - new Vector2(num3 - 0.1f, 0f), down, length2, secondaryDebug: true).collider != null)
					{
						point.x -= num3 * transform.localScale.x;
					}
					if (Mathf.Abs(point.x - transform.position.x) < MinJumpDistance.Value)
					{
						break;
					}
					GroundPoint.Value = point;
					DidFindGroundPoint.Value = true;
					return;
				}
				num -= ReductionDistance.Value;
			}
			DidFindGroundPoint.Value = false;
		}

		private RaycastHit2D CastRayLocal(Vector2 originLocal, Vector2 directionLocal, float length, bool secondaryDebug = false)
		{
			Vector2 origin = transform.TransformPoint(originLocal);
			Vector2 direction = transform.TransformVector(directionLocal);
			return Helper.Raycast2D(origin, direction, length, 256);
		}
	}
}
