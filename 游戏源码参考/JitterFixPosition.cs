using UnityEngine;

public class JitterFixPosition : MonoBehaviour
{
	[SerializeField]
	private JitterSelf jitter;

	[SerializeField]
	private bool ignoreRotation;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private bool useLerp;

	[SerializeField]
	private float lerpDuration = 0.25f;

	private Vector3 localPosition;

	private Quaternion localRotation;

	private double nextFixTime;

	private float lerpMultiplier;

	private float rotationRate;

	private float lerpRate;

	private float timer;

	private bool isLerping;

	private void Start()
	{
		Transform transform = base.transform;
		localPosition = transform.localPosition;
		localRotation = transform.localRotation;
		jitter.PositionRestored += OnPositionRestored;
		nextFixTime = Time.timeAsDouble + (double)cooldown;
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if ((bool)jitter)
		{
			jitter.PositionRestored -= OnPositionRestored;
		}
	}

	private void LateUpdate()
	{
		if (isLerping)
		{
			timer += Time.deltaTime * lerpMultiplier;
			Transform transform = base.transform;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, localPosition, Time.deltaTime * lerpMultiplier);
			if (!ignoreRotation)
			{
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, localRotation, Time.deltaTime * rotationRate);
			}
			if (timer >= 1f)
			{
				isLerping = false;
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnPositionRestored()
	{
		if (Time.timeAsDouble < nextFixTime)
		{
			return;
		}
		nextFixTime = Time.timeAsDouble + (double)cooldown;
		Transform transform = base.transform;
		if (useLerp)
		{
			if (!isLerping)
			{
				timer = 0f;
				float num = ((lerpDuration > 0f) ? lerpDuration : 1f);
				lerpMultiplier = 1f / num;
				rotationRate = 360f / num;
				isLerping = true;
			}
			base.enabled = true;
		}
		else
		{
			transform.localPosition = localPosition;
			if (!ignoreRotation)
			{
				transform.localRotation = localRotation;
			}
		}
	}
}
