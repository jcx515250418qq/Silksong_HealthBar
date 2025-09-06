using UnityEngine;

public sealed class BounceOnWater : MonoBehaviour
{
	[Header("Bounce Settings")]
	[Tooltip("Adjust the bounciness of the reflection (1 = perfect reflection, <1 = dampened, >1 = amplified).")]
	[SerializeField]
	private float bounceMultiplier = 0.5f;

	[SerializeField]
	private Vector2 normal = Vector2.up;

	[Tooltip("The Rigidbody2D component of the object.")]
	[SerializeField]
	private Rigidbody2D rb;

	private void Awake()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody2D>();
			if (rb == null)
			{
				Debug.LogError("BounceOnTrigger requires a Rigidbody2D component to be assigned or present on the GameObject.", this);
			}
		}
	}

	private void OnValidate()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody2D>();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(rb == null) && collision.CompareTag("Water Surface"))
		{
			Vector2 vector = Vector2.Reflect(rb.linearVelocity, normal);
			rb.linearVelocity = vector * bounceMultiplier;
		}
	}
}
