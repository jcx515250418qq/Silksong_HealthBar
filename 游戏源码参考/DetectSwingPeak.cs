using UnityEngine;
using UnityEngine.Events;

public class DetectSwingPeak : MonoBehaviour
{
	public UnityEvent OnRangeExceeded;

	private const float STILL_TIME = 1f;

	private bool didGetPrevious;

	private float previousAngleZ;

	private float currentSign;

	private float stillTimeElapsed;

	private float lastPeakSign;

	private bool hasPeaked;

	private void OnEnable()
	{
		didGetPrevious = false;
		currentSign = 0f;
		hasPeaked = false;
	}

	private void Update()
	{
		float num = base.transform.localEulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num < -180f)
		{
			num += 360f;
		}
		float num2 = num - previousAngleZ;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		if (didGetPrevious && Mathf.Abs(num2) > Mathf.Epsilon)
		{
			stillTimeElapsed = 0f;
			float num3 = Mathf.Sign(num2);
			if (((num3 < 0f && currentSign > 0f) || (num3 > 0f && currentSign < 0f)) && (!hasPeaked || Mathf.Abs(lastPeakSign - Mathf.Sign(num)) > float.Epsilon))
			{
				OnRangeExceeded.Invoke();
				lastPeakSign = Mathf.Sign(num);
				hasPeaked = true;
			}
			currentSign = num3;
		}
		else if (stillTimeElapsed < 1f)
		{
			stillTimeElapsed += Time.deltaTime;
			if (stillTimeElapsed >= 1f)
			{
				OnRangeExceeded.Invoke();
				stillTimeElapsed = 0f;
			}
		}
		previousAngleZ = num;
		didGetPrevious = true;
	}
}
