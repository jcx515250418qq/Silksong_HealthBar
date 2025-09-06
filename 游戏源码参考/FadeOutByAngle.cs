using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class FadeOutByAngle : MonoBehaviour
{
	[SerializeField]
	private MinMaxFloat angleLimits;

	[SerializeField]
	private NestedFadeGroupBase fadeTarget;

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeInDelay;

	private bool wasWithinLimits;

	private float fadeInDelayTimeLeft;

	public MinMaxFloat AngleLimitsNearest => new MinMaxFloat(Mathf.DeltaAngle(0f, angleLimits.Start), Mathf.DeltaAngle(0f, angleLimits.End));

	private void OnDrawGizmosSelected()
	{
		MinMaxFloat angleLimitsNearest = AngleLimitsNearest;
		HandleHelper.Draw2DAngle(base.transform.position, angleLimitsNearest.Start, angleLimitsNearest.End, 1f);
	}

	private void Update()
	{
		if (!fadeTarget)
		{
			return;
		}
		MinMaxFloat angleLimitsNearest = AngleLimitsNearest;
		float num = Vector3.SignedAngle(Vector3.right, base.transform.right, Vector3.forward);
		bool flag = num >= angleLimitsNearest.Start && num <= angleLimitsNearest.End;
		if (flag)
		{
			if (!wasWithinLimits)
			{
				fadeInDelayTimeLeft = fadeInDelay;
			}
			else if (fadeInDelayTimeLeft > 0f)
			{
				fadeInDelayTimeLeft -= Time.deltaTime;
				if (fadeInDelayTimeLeft <= 0f)
				{
					fadeTarget.FadeTo(1f, fadeInDuration);
				}
			}
		}
		else if (wasWithinLimits)
		{
			SetExitedLimits();
		}
		wasWithinLimits = flag;
	}

	public void SetExitedLimits()
	{
		fadeTarget.FadeTo(0f, fadeOutDuration);
		fadeInDelayTimeLeft = 0f;
		wasWithinLimits = false;
	}
}
