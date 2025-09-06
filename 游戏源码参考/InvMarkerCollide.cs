using System.Collections.Generic;
using UnityEngine;

public class InvMarkerCollide : MonoBehaviour
{
	[SerializeField]
	private MapMarkerMenu markerMenu;

	private CircleCollider2D circle;

	private readonly List<InvMarker> previousMarkers = new List<InvMarker>();

	private void Awake()
	{
		circle = GetComponent<CircleCollider2D>();
	}

	private void OnDisable()
	{
		ResetMarkerSelection();
	}

	private void Update()
	{
		if (!PlayerData.instance.isInventoryOpen)
		{
			return;
		}
		Vector2 worldPos = base.transform.TransformPoint(circle.offset);
		float radius = circle.radius;
		ResetMarkerSelection();
		foreach (InvMarker marker in InvMarker.GetMarkerList())
		{
			previousMarkers.Add(marker);
			if (marker.IsColliding(worldPos, radius))
			{
				markerMenu.AddToCollidingList(marker.gameObject);
			}
			else
			{
				markerMenu.RemoveFromCollidingList(marker.gameObject);
			}
		}
	}

	private void ResetMarkerSelection()
	{
		foreach (InvMarker previousMarker in previousMarkers)
		{
			if (!InvMarker.GetMarkerList().Contains(previousMarker))
			{
				markerMenu.RemoveFromCollidingList(previousMarker.gameObject);
			}
		}
		previousMarkers.Clear();
	}
}
