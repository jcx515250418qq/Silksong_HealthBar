using UnityEngine;

public class LechPusherWall : MonoBehaviour
{
	[SerializeField]
	private Vector2 force;

	private void OnCollisionEnter2D(Collision2D other)
	{
		Rigidbody2D rigidbody = other.rigidbody;
		if ((bool)rigidbody)
		{
			rigidbody.AddForce(force, ForceMode2D.Impulse);
		}
	}
}
