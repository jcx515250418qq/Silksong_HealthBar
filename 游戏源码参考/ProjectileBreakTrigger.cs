using UnityEngine;

public sealed class ProjectileBreakTrigger : MonoBehaviour
{
	[SerializeField]
	private bool isWall;

	private void OnTriggerEnter2D(Collider2D other)
	{
		other.GetComponentInParent<IBreakableProjectile>()?.QueueBreak(new IBreakableProjectile.HitInfo
		{
			isWall = isWall
		});
	}
}
