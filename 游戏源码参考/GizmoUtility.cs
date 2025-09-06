using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class GizmoUtility
{
	private class MeshHelper
	{
		public Mesh mesh;

		public uint drawCalls;

		public MeshHelper()
		{
			mesh = new Mesh();
			mesh.name = "GizmoUtilityMesh";
			mesh.hideFlags = (mesh.hideFlags |= HideFlags.DontSave);
		}

		~MeshHelper()
		{
			if (mesh != null)
			{
				Object.Destroy(mesh);
			}
		}
	}

	private static HashSet<string> errors = new HashSet<string>();

	private static ConditionalWeakTable<PolygonCollider2D, MeshHelper> polyMeshMap = new ConditionalWeakTable<PolygonCollider2D, MeshHelper>();

	public static bool IsChildSelected(Transform transform)
	{
		return false;
	}

	public static bool IsTargetSelected(Transform target)
	{
		return false;
	}

	public static bool IsSelfOrChildSelected(Transform transform)
	{
		if (!IsTargetSelected(transform))
		{
			return IsChildSelected(transform);
		}
		return true;
	}

	public static void DrawSceneLabel(Vector3 position, string label, int fontSize = 30, GUIStyle style = null)
	{
	}

	public static void DrawCollider2D(Collider2D collider2D)
	{
		if (!collider2D)
		{
			return;
		}
		if (!(collider2D is EdgeCollider2D edgeCollider2D))
		{
			if (!(collider2D is BoxCollider2D boxCollider2D))
			{
				if (!(collider2D is PolygonCollider2D polygonCollider2D))
				{
					if (collider2D is CircleCollider2D circleCollider2D)
					{
						DrawCircleCollider2D(circleCollider2D);
						return;
					}
					string text = $"Draw method for {collider2D} not implemented";
					if (errors.Add(text))
					{
						Debug.LogError(text);
					}
				}
				else
				{
					DrawPolygonCollider2D(polygonCollider2D);
				}
			}
			else
			{
				DrawBoxCollider2D(boxCollider2D);
			}
		}
		else
		{
			DrawEdgeCollider2D(edgeCollider2D);
		}
	}

	private static Matrix4x4 UpdateMatrix(Collider2D collider2D)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = collider2D.transform.localToWorldMatrix;
		return matrix;
	}

	private static Matrix4x4 UpdateMatrix(Transform transform)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		return matrix;
	}

	private static void DrawCircleCollider2D(CircleCollider2D circleCollider2D)
	{
		Matrix4x4 matrix = UpdateMatrix(circleCollider2D);
		Gizmos.DrawWireSphere(circleCollider2D.offset, circleCollider2D.radius);
		Gizmos.matrix = matrix;
	}

	private static void DrawPolygonCollider2D(PolygonCollider2D polygonCollider2D)
	{
		Matrix4x4 matrix = UpdateMatrix(polygonCollider2D);
		Vector2[] points = polygonCollider2D.points;
		Vector2 offset = polygonCollider2D.offset;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector2 vector = points[i] + offset;
			Gizmos.DrawLine(to: points[i + 1] + offset, from: vector);
		}
		if (points.Length > 2)
		{
			Vector2 vector2 = points[^1] + offset;
			Gizmos.DrawLine(to: points[0] + offset, from: vector2);
		}
		Gizmos.matrix = matrix;
	}

	public static void DrawPolygonPointList(Transform transform, List<Vector2> points)
	{
		Matrix4x4 matrix = UpdateMatrix(transform);
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2 vector = points[i];
			Gizmos.DrawLine(to: points[i + 1], from: vector);
		}
		if (points.Count > 2)
		{
			Vector2 vector2 = points[points.Count - 1];
			Gizmos.DrawLine(to: points[0], from: vector2);
		}
		Gizmos.matrix = matrix;
	}

	private static void DrawBoxCollider2D(BoxCollider2D boxCollider2D)
	{
		Matrix4x4 matrix = UpdateMatrix(boxCollider2D);
		Gizmos.DrawWireCube(boxCollider2D.offset, boxCollider2D.size);
		Gizmos.matrix = matrix;
	}

	private static void DrawEdgeCollider2D(EdgeCollider2D edgeCollider2D)
	{
		Matrix4x4 matrix = UpdateMatrix(edgeCollider2D);
		Vector2[] points = edgeCollider2D.points;
		Vector2 offset = edgeCollider2D.offset;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector2 vector = points[i] + offset;
			Gizmos.DrawLine(to: points[i + 1] + offset, from: vector);
		}
		Gizmos.matrix = matrix;
	}

	public static void DrawWireCircle(Vector3 position, float radius)
	{
	}
}
