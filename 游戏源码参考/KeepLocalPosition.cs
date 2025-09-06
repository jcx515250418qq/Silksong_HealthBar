using UnityEngine;

public class KeepLocalPosition : MonoBehaviour
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

	[Space]
	public bool resetOnDisable;

	private Vector3 initialLocalPos;

	private Vector3 initialLocalScale;

	private Vector3 lastLossyScale;

	private void OnEnable()
	{
		initialLocalPos = base.transform.localPosition;
		initialLocalScale = base.transform.localScale;
		lastLossyScale = base.transform.lossyScale;
		if (getPositionOnEnable)
		{
			xPosition = initialLocalPos.x;
			yPosition = initialLocalPos.y;
		}
	}

	private void OnDisable()
	{
		if (resetOnDisable)
		{
			base.transform.localPosition = initialLocalPos;
			base.transform.localScale = initialLocalScale;
		}
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (keepX)
		{
			localPosition.x = xPosition;
		}
		if (keepY)
		{
			localPosition.y = yPosition;
		}
		base.transform.localPosition = localPosition;
		if (keepScaleX || keepScaleY)
		{
			Vector3 localScale = base.transform.localScale;
			Vector3 lossyScale = base.transform.lossyScale;
			if (keepScaleX && Mathf.Sign(lossyScale.x) != Mathf.Sign(lastLossyScale.x))
			{
				localScale.x *= -1f;
			}
			if (keepScaleY && Mathf.Sign(lossyScale.y) != Mathf.Sign(lastLossyScale.y))
			{
				localScale *= -1f;
			}
			base.transform.localScale = localScale;
			lastLossyScale = base.transform.lossyScale;
		}
	}

	public void SetKeepWorldPosition(bool value)
	{
		base.enabled = value;
	}
}
