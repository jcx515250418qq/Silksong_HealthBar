using UnityEngine;

public class EnemyPusher : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.root.gameObject.name != collision.otherCollider.transform.root.gameObject.name)
		{
			Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
		}
	}
}
