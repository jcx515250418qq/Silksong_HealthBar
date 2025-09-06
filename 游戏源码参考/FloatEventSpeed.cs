using System;
using UnityEngine;
using UnityEngine.Events;

public class FloatEventSpeed : MonoBehaviour
{
	[SerializeField]
	private float speed;

	public UnityEvent<float> SpeedEvent;

	public UnityEvent<float> TotalEvent;

	private float total;

	private float previousSpeed;

	private void LateUpdate()
	{
		if (!(Math.Abs(speed) <= Mathf.Epsilon) || !(Math.Abs(previousSpeed) <= Mathf.Epsilon))
		{
			SpeedEvent.Invoke(speed);
			total += speed * Time.deltaTime;
			TotalEvent.Invoke(total);
		}
	}
}
