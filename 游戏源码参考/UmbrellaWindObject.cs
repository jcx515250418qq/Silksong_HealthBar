using System;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaWindObject : MonoBehaviour, ITrackTriggerObject
{
	private const float LERP_SPEED_UP = 5f;

	private const float LERP_SPEED_DOWN = 2f;

	private const float LERP_SPEED_UPDRAFT = 10f;

	private float currentAddSpeed;

	private Rigidbody2D body;

	private HeroController hc;

	private readonly List<UmbrellaWindRegion> insideRegions = new List<UmbrellaWindRegion>();

	public bool IsActive { get; set; }

	public float SelfXSpeed { get; set; }

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		hc = GetComponent<HeroController>();
	}

	private void FixedUpdate()
	{
		if (!IsActive)
		{
			currentAddSpeed = 0f;
			return;
		}
		float num;
		float num2;
		if ((bool)hc && hc.cState.inUpdraft)
		{
			num = 0f;
			num2 = 10f;
		}
		else
		{
			num = 0f;
			foreach (UmbrellaWindRegion insideRegion in insideRegions)
			{
				num += insideRegion.SpeedX;
			}
			num2 = ((num > currentAddSpeed) ? 5f : 2f);
		}
		currentAddSpeed = ((Math.Abs(num - currentAddSpeed) < 0.001f) ? num : Mathf.Lerp(currentAddSpeed, num, Time.deltaTime * num2));
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity.x = SelfXSpeed + currentAddSpeed;
		body.linearVelocity = linearVelocity;
		if ((bool)hc)
		{
			hc.DoRecoilMovement();
		}
	}

	public void OnTrackTriggerEntered(TrackTriggerObjects enteredRange)
	{
		UmbrellaWindRegion umbrellaWindRegion = enteredRange as UmbrellaWindRegion;
		if ((bool)umbrellaWindRegion)
		{
			insideRegions.AddIfNotPresent(umbrellaWindRegion);
		}
	}

	public void OnTrackTriggerExited(TrackTriggerObjects exitedRange)
	{
		UmbrellaWindRegion umbrellaWindRegion = exitedRange as UmbrellaWindRegion;
		if ((bool)umbrellaWindRegion)
		{
			insideRegions.Remove(umbrellaWindRegion);
		}
	}
}
