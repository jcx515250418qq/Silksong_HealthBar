using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Find the intersection point between a given point and direction with the top edge of a BoxCollider2D, clamping to the closest point on the line if it isn't on the line.")]
	public sealed class FindTopEdgeIntersectionX : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(BoxCollider2D))]
		[Tooltip("The GameObject with the BoxCollider2D.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The starting point of the line.")]
		public FsmFloat xCoordinate;

		[Tooltip("The intersection point.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 intersectionPoint;

		[UIHint(UIHint.Variable)]
		public FsmFloat xIntersect;

		[UIHint(UIHint.Variable)]
		public FsmFloat yIntersect;

		[UIHint(UIHint.Variable)]
		public FsmFloat zIntersect;

		public override void Reset()
		{
			gameObject = null;
			xCoordinate = null;
			intersectionPoint = null;
			xIntersect = null;
			yIntersect = null;
			zIntersect = null;
		}

		public override void OnEnter()
		{
			DoFindIntersection();
			Finish();
		}

		private void DoFindIntersection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			BoxCollider2D component = ownerDefaultTarget.GetComponent<BoxCollider2D>();
			if (!(component == null))
			{
				Vector2 pivot = component.bounds.center;
				Vector2 size = component.size;
				float num = size.y / 2f;
				Vector2 vector = RotatePointAroundPivot(new Vector2(pivot.x - size.x / 2f, pivot.y + num), pivot, component.transform.eulerAngles.z);
				Vector2 vector2 = RotatePointAroundPivot(new Vector2(pivot.x + size.x / 2f, pivot.y + num), pivot, component.transform.eulerAngles.z);
				float num2 = Mathf.Min(vector.x, vector2.x);
				float num3 = Mathf.Max(vector.x, vector2.x);
				float value = xCoordinate.Value;
				Vector2 vector3;
				if (!(value >= num2) || !(value <= num3))
				{
					vector3 = ((!(Mathf.Abs(value - vector.x) < Mathf.Abs(value - vector2.x))) ? vector2 : vector);
				}
				else
				{
					float t = (value - vector.x) / (vector2.x - vector.x);
					vector3 = Vector2.Lerp(vector, vector2, t);
				}
				intersectionPoint.Value = new Vector3(vector3.x, vector3.y, 0f);
				xIntersect.Value = vector3.x;
				yIntersect.Value = vector3.y;
			}
		}

		private Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
		{
			Vector2 vector = point - pivot;
			vector = Quaternion.Euler(0f, 0f, angle) * vector;
			point = vector + pivot;
			return point;
		}
	}
}
