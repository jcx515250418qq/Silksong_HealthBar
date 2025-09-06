using TeamCherry.SharedUtils;
using UnityEngine;

public class PushReactNonPhysical : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private Vector2 centreOffset;

	[SerializeField]
	private MinMaxFloat forceMagnitude;

	[SerializeField]
	private Vector2 minDirection;

	[SerializeField]
	private float cooldownDuration;

	private double cooldownFinishTime;

	private void OnValidate()
	{
		if (minDirection.x < 0f)
		{
			minDirection.x = 0f;
		}
		if (minDirection.y < 0f)
		{
			minDirection.y = 0f;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(centreOffset), 0.15f);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(Time.timeAsDouble < cooldownFinishTime))
		{
			cooldownFinishTime = Time.timeAsDouble + (double)cooldownDuration;
			Vector2 vector = (Vector2)base.transform.TransformPoint(centreOffset) - (Vector2)collision.transform.position;
			if (Mathf.Abs(vector.x) < minDirection.x)
			{
				vector.x = minDirection.x * Mathf.Sign(vector.x);
			}
			if (Mathf.Abs(vector.y) < minDirection.y)
			{
				vector.y = minDirection.y * Mathf.Sign(vector.y);
			}
			vector.Normalize();
			body.AddForce(vector * forceMagnitude.GetRandomValue(), ForceMode2D.Impulse);
		}
	}
}
