using System.Collections.Generic;
using UnityEngine;

public class InvMarker : MonoBehaviour
{
	private CircleCollider2D circle;

	private static readonly List<InvMarker> _activeMarkers = new List<InvMarker>();

	public MapMarkerMenu.MarkerTypes Colour { get; set; }

	public int Index { get; set; }

	private void Awake()
	{
		circle = GetComponent<CircleCollider2D>();
	}

	private void OnEnable()
	{
		_activeMarkers.Add(this);
	}

	private void OnDisable()
	{
		_activeMarkers.Remove(this);
	}

	public static List<InvMarker> GetMarkerList()
	{
		return _activeMarkers;
	}

	public bool IsColliding(Vector2 worldPos, float radius)
	{
		Vector2 b = base.transform.TransformPoint(circle.offset);
		float num = Vector2.Distance(worldPos, b);
		float num2 = radius + circle.radius;
		return num <= num2;
	}
}
