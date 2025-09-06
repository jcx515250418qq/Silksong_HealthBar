using System;
using UnityEngine;

public class FlingSelf : MonoBehaviour
{
	public float speedMin;

	public float speedMax;

	public float angleMin;

	public float angleMax;

	public bool relativeToParent;

	public bool deparent = true;

	public bool onEnable;

	private void Start()
	{
		if (!onEnable)
		{
			DoFling();
		}
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			DoFling();
		}
	}

	public void DoFling()
	{
		Rigidbody2D component = base.gameObject.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			Transform transform = base.transform;
			float num = UnityEngine.Random.Range(speedMin, speedMax);
			float num2 = UnityEngine.Random.Range(angleMin, angleMax);
			float num3 = num * Mathf.Cos(num2 * (MathF.PI / 180f));
			float num4 = num * Mathf.Sin(num2 * (MathF.PI / 180f));
			if (relativeToParent)
			{
				Vector3 lossyScale = transform.lossyScale;
				num3 *= Mathf.Sign(lossyScale.x);
				num4 *= Mathf.Sign(lossyScale.y);
			}
			if (deparent)
			{
				transform.parent = null;
			}
			component.linearVelocity = new Vector2(num3, num4);
		}
	}

	public void SetSpeedMin(float newSpeed)
	{
		speedMin = newSpeed;
	}

	public void SetSpeedMax(float newSpeed)
	{
		speedMax = newSpeed;
	}
}
