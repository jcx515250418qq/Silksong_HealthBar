using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class ApplyForceToCurrency : MonoBehaviour
{
	[SerializeField]
	private MinMaxFloat xForce = new MinMaxFloat(-9f, -3f);

	[SerializeField]
	private MinMaxFloat yForce = new MinMaxFloat(0f, 0f);

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Geo"))
		{
			AddForce(other.rigidbody);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Geo"))
		{
			AddForce(other.GetComponent<Rigidbody2D>());
		}
	}

	private void AddForce(Rigidbody2D rb)
	{
		if (rb != null)
		{
			float num = Mathf.Sign(base.transform.lossyScale.x);
			rb.AddForce(new Vector2(xForce.GetRandomValue() * num, yForce.GetRandomValue()), ForceMode2D.Impulse);
		}
	}
}
