using System;
using UnityEngine;

public class KeepWorldPosition : MonoBehaviour
{
	public bool keepX;

	public float xPosition;

	public bool keepY;

	public float yPosition;

	[Space]
	public bool getPositionOnEnable;

	[Space]
	public bool keepScaleX;

	public bool keepScaleY;

	public bool deactivateIfFlippedOnX;

	[Space]
	public bool resetOnDisable;

	private Vector3 initialLocalPos;

	private Vector3 initialLocalScale;

	private Vector3 lastLossyScale;

	private bool started;

	private void Start()
	{
		Initialise();
		started = true;
	}

	private void OnEnable()
	{
		if (started)
		{
			Initialise();
		}
	}

	private void Initialise()
	{
		Transform transform = base.transform;
		initialLocalPos = transform.localPosition;
		initialLocalScale = transform.localScale;
		lastLossyScale = transform.lossyScale;
		if (getPositionOnEnable)
		{
			Vector3 position = transform.position;
			xPosition = position.x;
			yPosition = position.y;
		}
	}

	private void OnDisable()
	{
		if (resetOnDisable && started)
		{
			Transform obj = base.transform;
			obj.localPosition = initialLocalPos;
			obj.localScale = initialLocalScale;
		}
	}

	private void Update()
	{
		Transform transform = base.transform;
		Vector3 position = transform.position;
		if (keepX)
		{
			position.x = xPosition;
		}
		if (keepY)
		{
			position.y = yPosition;
		}
		transform.position = position;
		if (!keepScaleX && !keepScaleY && !deactivateIfFlippedOnX)
		{
			return;
		}
		Vector3 localScale = transform.localScale;
		Vector3 lossyScale = transform.lossyScale;
		if (Math.Abs(Mathf.Sign(lossyScale.x) - Mathf.Sign(lastLossyScale.x)) > Mathf.Epsilon)
		{
			if (keepScaleX)
			{
				localScale.x *= -1f;
			}
			if (deactivateIfFlippedOnX)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if (keepScaleY && Math.Abs(Mathf.Sign(lossyScale.y) - Mathf.Sign(lastLossyScale.y)) > Mathf.Epsilon)
		{
			localScale *= -1f;
		}
		transform.localScale = localScale;
		lastLossyScale = transform.lossyScale;
	}

	public void SetKeepWorldPosition(bool value)
	{
		base.enabled = value;
	}

	public void ForceUpdate()
	{
		Update();
	}
}
