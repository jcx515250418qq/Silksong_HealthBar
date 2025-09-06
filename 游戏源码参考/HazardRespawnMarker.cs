using System;
using UnityEngine;

[ExecuteInEditMode]
public class HazardRespawnMarker : MonoBehaviour
{
	public enum FacingDirection
	{
		None = 0,
		Left = 1,
		Right = 2
	}

	[SerializeField]
	private FacingDirection respawnFacingDirection;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool respawnFacingRight;

	private float groundSenseDistance = 3f;

	private readonly Vector2 groundSenseRay = -Vector2.up;

	private Vector2 heroSpawnLocation;

	public bool drawDebugRays;

	public FacingDirection RespawnFacingDirection => respawnFacingDirection;

	private void OnValidate()
	{
		if (respawnFacingRight)
		{
			respawnFacingDirection = FacingDirection.Right;
			respawnFacingRight = false;
		}
	}

	private void Awake()
	{
		OnValidate();
		if (base.transform.parent != null && base.transform.parent.name.Contains("top"))
		{
			groundSenseDistance = 50f;
		}
		heroSpawnLocation = Helper.Raycast2D(base.transform.position, groundSenseRay, groundSenseDistance, 256).point;
	}
}
