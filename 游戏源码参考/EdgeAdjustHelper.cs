using System;
using UnityEngine;

public static class EdgeAdjustHelper
{
	public static float GetEdgeAdjustX(Collider2D col2d, bool facingRight, float forwardEdgeFudge = 0f, float backwardEdgeFudge = 0f)
	{
		Bounds bounds = col2d.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (facingRight)
		{
			max.x -= forwardEdgeFudge;
			min.x += backwardEdgeFudge;
		}
		else
		{
			max.x -= backwardEdgeFudge;
			min.x += forwardEdgeFudge;
		}
		float edgeAdjust = GetEdgeAdjust(min, max, backwardEdgeFudge, facingRight, isForward: true);
		if (Math.Abs(edgeAdjust) <= Mathf.Epsilon)
		{
			edgeAdjust = GetEdgeAdjust(min, max, forwardEdgeFudge, facingRight, isForward: false);
		}
		return edgeAdjust;
	}

	private static float GetEdgeAdjust(Vector2 min, Vector2 max, float edgeFudge, bool facingRight, bool isForward)
	{
		float y = min.y + 0.1f;
		Vector2 vector;
		Vector2 direction;
		if (facingRight == isForward)
		{
			vector = new Vector2(max.x, y);
			direction = Vector2.left;
		}
		else
		{
			vector = new Vector2(min.x, y);
			direction = Vector2.right;
		}
		if (Helper.IsRayHittingNoTriggers(vector, Vector2.down, 0.3f, 256))
		{
			return 0f;
		}
		Vector2 origin = vector + Vector2.down * 0.3f;
		float num2;
		float num;
		if (facingRight == isForward)
		{
			num = origin.x - min.x;
			num2 = -1f;
		}
		else
		{
			num = max.x - origin.x;
			num2 = 1f;
		}
		num += edgeFudge;
		if (!Helper.IsRayHittingNoTriggers(origin, direction, num, 256, out var closestHit))
		{
			return 0f;
		}
		return closestHit.distance * num2;
	}
}
