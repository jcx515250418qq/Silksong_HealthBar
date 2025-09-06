using UnityEngine;

public class FollowRotation : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private Transform[] fallbackTargets;

	[SerializeField]
	private float offset;

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private float fpsLimit;

	private double nextUpdateTime;

	private Quaternion currentRotation;

	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
			currentRotation = target.rotation;
		}
	}

	public Transform CurrentTarget
	{
		get
		{
			if ((bool)target && target.gameObject.activeInHierarchy)
			{
				return target;
			}
			if (fallbackTargets != null)
			{
				Transform[] array = fallbackTargets;
				foreach (Transform transform in array)
				{
					if ((bool)transform && transform.gameObject.activeInHierarchy)
					{
						return transform;
					}
				}
			}
			return null;
		}
	}

	private void OnValidate()
	{
		if (lerpSpeed < 0f)
		{
			lerpSpeed = 0f;
		}
		if (fpsLimit < 0f)
		{
			fpsLimit = 0f;
		}
	}

	private void OnEnable()
	{
		currentRotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		Transform currentTarget = CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		Quaternion quaternion = currentTarget.rotation * Quaternion.Euler(0f, 0f, offset);
		currentRotation = ((lerpSpeed <= Mathf.Epsilon) ? quaternion : Quaternion.Lerp(currentRotation, quaternion, lerpSpeed * Time.deltaTime));
		if (fpsLimit > 0f)
		{
			if (Time.timeAsDouble < nextUpdateTime)
			{
				return;
			}
			nextUpdateTime = Time.timeAsDouble + (double)(1f / fpsLimit);
		}
		base.transform.rotation = currentRotation;
	}
}
