using UnityEngine;

public class LoopRotator : MonoBehaviour
{
	[SerializeField]
	private float rotationSpeed = 10f;

	[SerializeField]
	private bool allowReversed;

	[SerializeField]
	private float acceleration = 10f;

	[SerializeField]
	private float deceleration = 10f;

	[SerializeField]
	private float fpsLimit;

	[SerializeField]
	private bool startOnEnable;

	private float currentSpeed;

	private bool isRotating;

	private bool isReversed;

	private double nextUpdateTime;

	private void OnValidate()
	{
		if (acceleration < 0f)
		{
			acceleration = 0f;
		}
		if (deceleration < 0f)
		{
			deceleration = 0f;
		}
		if (fpsLimit < 0f)
		{
			fpsLimit = 0f;
		}
	}

	private void Start()
	{
		currentSpeed = (isRotating ? rotationSpeed : 0f);
	}

	private void OnEnable()
	{
		if (startOnEnable)
		{
			StartRotation();
		}
	}

	private void Update()
	{
		if (Mathf.Abs(currentSpeed) <= 0.01f && !isRotating)
		{
			return;
		}
		float num;
		if (fpsLimit > 0f)
		{
			if (Time.timeAsDouble < nextUpdateTime)
			{
				return;
			}
			num = 1f / fpsLimit;
			nextUpdateTime = Time.timeAsDouble + (double)num;
		}
		else
		{
			num = Time.deltaTime;
		}
		float b = (isReversed ? (0f - rotationSpeed) : rotationSpeed);
		if (isRotating)
		{
			if (acceleration > 0f)
			{
				currentSpeed = Mathf.Lerp(currentSpeed, b, acceleration * num);
			}
			else
			{
				currentSpeed = rotationSpeed;
			}
		}
		else if (deceleration > 0f)
		{
			currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * num);
		}
		else
		{
			currentSpeed = 0f;
		}
		Transform obj = base.transform;
		Vector3 localEulerAngles = obj.localEulerAngles;
		localEulerAngles.z += currentSpeed * num;
		obj.localEulerAngles = localEulerAngles;
	}

	[ContextMenu("Test Rotation", true)]
	[ContextMenu("Stop Rotation", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Test Rotation")]
	public void StartRotation()
	{
		isRotating = true;
		isReversed = false;
	}

	public void StartRotationReversed()
	{
		isRotating = true;
		isReversed = allowReversed;
	}

	[ContextMenu("Stop Rotation")]
	public void StopRotation()
	{
		isRotating = false;
	}
}
