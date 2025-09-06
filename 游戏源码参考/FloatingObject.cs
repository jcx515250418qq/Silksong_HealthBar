using UnityEngine;

public class FloatingObject : MonoBehaviour
{
	public Vector2 variance = Vector2.one;

	public float speedX = 0.5f;

	public float speedY = 1f;

	private Vector3 initialPos;

	private void Start()
	{
		initialPos = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = initialPos + new Vector3(variance.x * Mathf.Sin(Time.time * speedX + initialPos.z), variance.y * Mathf.Sin(Time.time * speedY + initialPos.z), 0f);
	}
}
