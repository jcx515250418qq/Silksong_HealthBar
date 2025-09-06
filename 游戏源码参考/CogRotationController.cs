using System;
using UnityEngine;

[ExecuteInEditMode]
public class CogRotationController : MonoBehaviour
{
	[Serializable]
	private class Cog
	{
		public Transform Transform;

		public float RotationSpeed;
	}

	[SerializeField]
	private Cog[] cogs;

	[Space]
	[SerializeField]
	private float rotationMultiplier = 1f;

	[SerializeField]
	private float fpsLimit;

	[SerializeField]
	private bool scaleFpsLimit;

	[Space]
	[SerializeField]
	private float animateRotation;

	[SerializeField]
	private float rotationSpeed;

	private float capturedAnimateRotation;

	private float externalRotation;

	private float oldAnimateRotation;

	private float speedAnimation;

	private bool queueUpdateRotation;

	private double nextUpdateTime;

	public float RotationMultiplier
	{
		get
		{
			return rotationMultiplier;
		}
		set
		{
			rotationMultiplier = value;
		}
	}

	public float AnimateRotation
	{
		get
		{
			return animateRotation;
		}
		set
		{
			animateRotation = value;
		}
	}

	private void OnEnable()
	{
		capturedAnimateRotation = 0f;
		externalRotation = 0f;
		oldAnimateRotation = 0f;
		speedAnimation = 0f;
		queueUpdateRotation = true;
		nextUpdateTime = 0.0;
		ApplyRotation();
	}

	private void LateUpdate()
	{
		if (Math.Abs(animateRotation - oldAnimateRotation) > 0.0001f)
		{
			oldAnimateRotation = animateRotation;
			queueUpdateRotation = true;
		}
		if (Math.Abs(rotationSpeed) > Mathf.Epsilon)
		{
			speedAnimation += rotationSpeed * Time.deltaTime * rotationMultiplier;
			queueUpdateRotation = true;
		}
		if (queueUpdateRotation)
		{
			ApplyRotation();
		}
	}

	public void ApplyRotation(float rotation)
	{
		externalRotation = rotation;
		queueUpdateRotation = true;
		ApplyRotation();
	}

	private void ApplyRotation()
	{
		float num = ((!scaleFpsLimit) ? fpsLimit : (fpsLimit * rotationMultiplier));
		if (num > 0f)
		{
			if (Time.timeAsDouble < nextUpdateTime)
			{
				return;
			}
			nextUpdateTime = Time.timeAsDouble + (double)(1f / num);
		}
		float num2 = externalRotation + animateRotation + capturedAnimateRotation + speedAnimation;
		Cog[] array = cogs;
		foreach (Cog cog in array)
		{
			cog.Transform.SetLocalRotation2D(num2 * cog.RotationSpeed);
		}
		queueUpdateRotation = false;
	}

	public void ResetNextUpdateTime()
	{
		nextUpdateTime = 0.0;
	}

	public void CaptureAnimateRotation()
	{
		capturedAnimateRotation += animateRotation;
		animateRotation = 0f;
		oldAnimateRotation = 0f;
		ApplyRotation();
	}

	public void SetRotationSpeed(float value)
	{
		rotationSpeed = value;
	}
}
