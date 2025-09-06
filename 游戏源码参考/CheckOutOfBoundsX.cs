using UnityEngine;
using UnityEngine.Events;

public class CheckOutOfBoundsX : MonoBehaviour
{
	public float xMin;

	public float xMax;

	public UnityEvent onOutOfBounds;

	private bool fired;

	private void OnEnable()
	{
		fired = false;
	}

	private void Update()
	{
		if (!fired)
		{
			float x = base.transform.position.x;
			if (x < xMin || x > xMax)
			{
				onOutOfBounds.Invoke();
				fired = true;
			}
		}
	}
}
