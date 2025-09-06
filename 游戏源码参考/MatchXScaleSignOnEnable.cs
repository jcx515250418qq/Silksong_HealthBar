using UnityEngine;

public class MatchXScaleSignOnEnable : MonoBehaviour
{
	public Transform matchObject;

	public bool reverseMatch;

	private bool targetSignPositive;

	private void OnEnable()
	{
		targetSignPositive = matchObject.localScale.x > 0f;
		if (reverseMatch)
		{
			targetSignPositive = !targetSignPositive;
		}
	}

	private void LateUpdate()
	{
		if ((base.transform.lossyScale.x < 0f && targetSignPositive) || (base.transform.lossyScale.x > 0f && !targetSignPositive))
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	public void SetTargetSign(bool positive)
	{
		targetSignPositive = positive;
	}
}
