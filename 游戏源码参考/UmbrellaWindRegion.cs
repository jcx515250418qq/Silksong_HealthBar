using System.Collections.Generic;
using UnityEngine;

public class UmbrellaWindRegion : WindRegion
{
	[Space]
	[SerializeField]
	private float speedX;

	public float SpeedMultiplier = 1f;

	private WindZone childWindZone;

	private static readonly List<UmbrellaWindRegion> _activeRegions = new List<UmbrellaWindRegion>();

	public float SpeedX => speedX * SpeedMultiplier;

	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = new GameObject("WindZone", typeof(WindZone));
		Transform obj = gameObject.transform;
		obj.eulerAngles = new Vector3(0f, (speedX > 0f) ? 90 : (-90), 0f);
		obj.SetParent(base.transform, worldPositionStays: true);
		obj.localPosition = Vector3.zero;
		childWindZone = gameObject.GetComponent<WindZone>();
		childWindZone.mode = WindZoneMode.Directional;
		childWindZone.windMain = Mathf.Abs(speedX);
		childWindZone.gameObject.SetActive(value: false);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_activeRegions.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_activeRegions.Remove(this);
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		base.OnInsideStateChanged(isInside);
		childWindZone.gameObject.SetActive(isInside);
	}

	public static IEnumerable<UmbrellaWindRegion> EnumerateActiveRegions()
	{
		foreach (UmbrellaWindRegion activeRegion in _activeRegions)
		{
			yield return activeRegion;
		}
	}
}
