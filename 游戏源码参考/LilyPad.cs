using UnityEngine;

public class LilyPad : MonoBehaviour
{
	private Rigidbody2D rb;

	public float minSpeed;

	public float maxSpeed;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		float num = collision.transform.position.x - base.transform.position.x;
		float num2 = Random.Range(minSpeed, maxSpeed);
		if (num < 0f)
		{
			rb.linearVelocity = new Vector2(num2, 0f);
		}
		else
		{
			rb.linearVelocity = new Vector2(0f - num2, 0f);
		}
	}
}
