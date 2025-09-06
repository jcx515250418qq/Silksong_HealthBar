using UnityEngine;

public class RepositionFromWalls : MonoBehaviour
{
	[SerializeField]
	private Collider2D collider;

	[SerializeField]
	private float rayLengthFromEdge = 1f;

	[SerializeField]
	private int raysPerUnitWidth = 3;

	[SerializeField]
	private bool everyFrame;

	private Vector2 previousPosition;

	private float previousRotation;

	private void OnDrawGizmosSelected()
	{
		if ((bool)collider)
		{
			GetSideRaysPenetration(out var distanceInsideTop, out var distanceInsideBottom, out var distanceInsideRight, out var distanceInsideLeft, isDrawingGizmos: true);
			GetCornerRaysPenetration(out distanceInsideTop, out distanceInsideBottom, out distanceInsideRight, out distanceInsideLeft, isDrawingGizmos: true);
		}
	}

	private void OnEnable()
	{
		Reposition();
		previousPosition = base.transform.position;
		previousRotation = base.transform.eulerAngles.z;
	}

	private void Update()
	{
		if (everyFrame && (((Vector2)base.transform.position - previousPosition).magnitude > 0.01f || Mathf.Abs(base.transform.eulerAngles.z - previousRotation) > 0.1f))
		{
			Reposition();
			previousPosition = base.transform.position;
		}
	}

	private void Reposition()
	{
		if ((bool)collider && collider.enabled)
		{
			GetCornerRaysPenetration(out var distanceInsideTop, out var distanceInsideBottom, out var distanceInsideRight, out var distanceInsideLeft, isDrawingGizmos: false);
			DoMove(distanceInsideTop, distanceInsideBottom, distanceInsideRight, distanceInsideLeft);
			GetSideRaysPenetration(out distanceInsideTop, out distanceInsideBottom, out distanceInsideRight, out distanceInsideLeft, isDrawingGizmos: false);
			DoMove(distanceInsideTop, distanceInsideBottom, distanceInsideRight, distanceInsideLeft);
		}
	}

	private void DoMove(float distanceInsideTop, float distanceInsideBottom, float distanceInsideRight, float distanceInsideLeft)
	{
		Vector3 position = base.transform.position;
		if (distanceInsideTop < Mathf.Epsilon || distanceInsideBottom < Mathf.Epsilon)
		{
			position.y -= distanceInsideTop;
			position.y += distanceInsideBottom;
		}
		if (distanceInsideRight < Mathf.Epsilon || distanceInsideLeft < Mathf.Epsilon)
		{
			position.x -= distanceInsideRight;
			position.x += distanceInsideLeft;
		}
		base.transform.position = position;
	}

	private void GetSideRaysPenetration(out float distanceInsideTop, out float distanceInsideBottom, out float distanceInsideRight, out float distanceInsideLeft, bool isDrawingGizmos)
	{
		Bounds bounds = collider.bounds;
		Vector3 center = bounds.center;
		Vector3 size = bounds.size;
		if (isDrawingGizmos)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(center, size);
		}
		float x = size.x;
		float y = size.y;
		Vector2 up = Vector2.up;
		float distance = y * 0.5f + rayLengthFromEdge;
		Vector2 down = Vector2.down;
		Vector2 right = Vector2.right;
		float distance2 = x * 0.5f + rayLengthFromEdge;
		Vector2 left = Vector2.left;
		distanceInsideTop = GetRaycastPenetration(center, up, distance, x, isDrawingGizmos);
		distanceInsideBottom = GetRaycastPenetration(center, down, distance, x, isDrawingGizmos);
		distanceInsideRight = GetRaycastPenetration(center, right, distance2, y, isDrawingGizmos);
		distanceInsideLeft = GetRaycastPenetration(center, left, distance2, y, isDrawingGizmos);
	}

	private void GetCornerRaysPenetration(out float distanceInsideTop, out float distanceInsideBottom, out float distanceInsideRight, out float distanceInsideLeft, bool isDrawingGizmos)
	{
		Bounds bounds = collider.bounds;
		Vector3 max = bounds.max;
		Vector3 min = bounds.min;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		Vector2 normalized = new Vector2(1f, 1f).normalized;
		Vector2 normalized2 = new Vector2(-1f, 1f).normalized;
		Vector2 normalized3 = new Vector2(-1f, -1f).normalized;
		Vector2 normalized4 = new Vector2(1f, -1f).normalized;
		num = GetRaycastPenetration(max, normalized, rayLengthFromEdge, 0f, isDrawingGizmos);
		num2 = GetRaycastPenetration(new Vector2(min.x, max.y), normalized2, rayLengthFromEdge, 0f, isDrawingGizmos);
		num3 = GetRaycastPenetration(min, normalized3, rayLengthFromEdge, 0f, isDrawingGizmos);
		num4 = GetRaycastPenetration(new Vector2(max.x, min.y), normalized4, rayLengthFromEdge, 0f, isDrawingGizmos);
		Vector2 vector = normalized * num;
		Vector2 vector2 = normalized2 * num2;
		Vector2 vector3 = normalized3 * num3;
		Vector2 vector4 = normalized4 * num4;
		distanceInsideTop = Mathf.Max(Mathf.Abs(vector.y), Mathf.Abs(vector2.y));
		distanceInsideBottom = Mathf.Max(Mathf.Abs(vector4.y), Mathf.Abs(vector3.y));
		distanceInsideRight = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector4.x));
		distanceInsideLeft = Mathf.Max(Mathf.Abs(vector2.x), Mathf.Abs(vector3.x));
	}

	private float GetRaycastPenetration(Vector2 origin, Vector2 direction, float distance, float width, bool isDrawingGizmos)
	{
		int num = ((!(width > 0f)) ? 1 : Mathf.Max(Mathf.FloorToInt((float)raysPerUnitWidth * width), 1));
		int num2 = num - 1;
		Vector2 vector = new Vector2(direction.y, 0f - direction.x) * 0.5f * width;
		Vector2 a = origin - vector;
		Vector2 b = origin + vector;
		float num3 = distance;
		for (int i = 0; i < num; i++)
		{
			Vector2 vector2 = ((num2 > 0) ? Vector2.Lerp(a, b, (float)i / (float)num2) : origin);
			RaycastHit2D raycastHit2D = Helper.Raycast2D(vector2, direction, distance, 256);
			bool flag = raycastHit2D.collider != null;
			if (isDrawingGizmos)
			{
				Gizmos.color = (flag ? new Color(1f, 0f, 0f, 0.5f) : Color.yellow);
				Gizmos.DrawLine(vector2, vector2 + direction * distance);
				if (flag)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(vector2, vector2 + direction * raycastHit2D.distance);
				}
			}
			if (flag)
			{
				num3 = Mathf.Min(num3, raycastHit2D.distance);
			}
		}
		return distance - num3;
	}
}
