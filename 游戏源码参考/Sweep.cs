using System.Collections.Generic;
using UnityEngine;

public struct Sweep
{
	public Vector2 Direction;

	public Collider2D Collider;

	public float SkinThickness;

	public float SkinWideThickness;

	public int RayCount;

	private const float DefaultSkinThickness = 0.1f;

	private const float DefaultSkinWideThickness = 0.01f;

	public const int DefaultRayCount = 3;

	private static List<Collider2D> buffer = new List<Collider2D>();

	public Sweep(Collider2D collider, int cardinalDirection, int rayCount, float skinThickness = 0.1f, float skinWideThickness = 0.01f)
	{
		Direction = new Vector2(DirectionUtils.GetX(cardinalDirection), DirectionUtils.GetY(cardinalDirection));
		Collider = collider;
		RayCount = rayCount;
		SkinThickness = skinThickness;
		SkinWideThickness = skinWideThickness;
	}

	public bool Check(float distance, int layerMask)
	{
		float clippedDistance;
		return Check(distance, layerMask, out clippedDistance);
	}

	public bool Check(float distance, int layerMask, bool useTriggers)
	{
		float clippedDistance;
		return Check(distance, layerMask, out clippedDistance, useTriggers);
	}

	public bool Check(float distance, int layerMask, out float clippedDistance)
	{
		return Check(distance, layerMask, out clippedDistance, useTriggers: false);
	}

	public bool Check(float distance, int layerMask, out float clippedDistance, bool useTriggers)
	{
		return Check(distance, layerMask, out clippedDistance, useTriggers, Vector2.zero);
	}

	public bool Check(float distance, int layerMask, out float clippedDistance, bool useTriggers, Vector2 offset)
	{
		if (distance <= 0f)
		{
			clippedDistance = 0f;
			return false;
		}
		Bounds bounds = GetBounds();
		Vector3 extents = bounds.extents;
		Vector2 vector = Vector2.Scale(extents, Direction);
		Vector2 vector2 = (Vector2)bounds.center + vector;
		Vector2 vector3 = Vector2.Scale(extents, new Vector2(Mathf.Abs(Direction.y), Mathf.Abs(Direction.x)));
		float num = distance;
		float num2 = Mathf.Max(vector.magnitude * 2f - SkinThickness, SkinThickness);
		Vector2 vector4 = Direction * (0f - num2);
		Vector2 vector5 = Vector3.Cross(Direction, -Vector3.forward);
		for (int i = 0; i < RayCount; i++)
		{
			float num3 = 2f * ((float)i / (float)(RayCount - 1)) - 1f;
			Vector2 vector6 = vector2 + vector3 * num3 + vector4 + vector5 * (num3 * SkinWideThickness);
			Vector2 origin = offset + vector6;
			RaycastHit2D closestHit;
			bool flag;
			if (useTriggers)
			{
				closestHit = Helper.Raycast2D(origin, Direction, num + num2, layerMask);
				flag = closestHit;
			}
			else
			{
				flag = Helper.IsRayHittingNoTriggers(origin, Direction, num + num2, layerMask, out closestHit);
			}
			float num4 = closestHit.distance - num2;
			if (flag && num4 < num)
			{
				num = num4;
			}
		}
		clippedDistance = num;
		return distance - num > Mathf.Epsilon;
	}

	private Bounds GetBounds()
	{
		if (!Collider.enabled)
		{
			if (Collider is BoxCollider2D boxCollider)
			{
				return CalculateBoxCollider2DBounds(boxCollider);
			}
			Rigidbody2D attachedRigidbody = Collider.attachedRigidbody;
			if ((bool)attachedRigidbody)
			{
				int attachedColliders = attachedRigidbody.GetAttachedColliders(buffer);
				for (int i = 0; i < attachedColliders; i++)
				{
					Collider2D collider2D = buffer[i];
					if (!collider2D.isTrigger && collider2D.enabled)
					{
						buffer.Clear();
						return collider2D.bounds;
					}
				}
				buffer.Clear();
			}
		}
		return Collider.bounds;
	}

	private static Bounds CalculateBoxCollider2DBounds(BoxCollider2D boxCollider)
	{
		Transform transform = boxCollider.transform;
		Vector2 offset = boxCollider.offset;
		Vector2 vector = boxCollider.size * 0.5f;
		Vector2[] array = new Vector2[4]
		{
			offset + new Vector2(0f - vector.x, 0f - vector.y),
			offset + new Vector2(vector.x, 0f - vector.y),
			offset + new Vector2(vector.x, vector.y),
			offset + new Vector2(0f - vector.x, vector.y)
		};
		Vector3[] array2 = new Vector3[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = transform.TransformPoint(array[i]);
		}
		Vector3 vector2 = array2[0];
		Vector3 vector3 = array2[0];
		Vector3[] array3 = array2;
		foreach (Vector3 rhs in array3)
		{
			vector2 = Vector3.Min(vector2, rhs);
			vector3 = Vector3.Max(vector3, rhs);
		}
		Bounds result = default(Bounds);
		result.SetMinMax(vector2, vector3);
		return result;
	}
}
