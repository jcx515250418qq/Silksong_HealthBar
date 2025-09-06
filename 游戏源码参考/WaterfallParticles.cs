using UnityEngine;

public class WaterfallParticles : MonoBehaviour
{
	public Vector2 raycastOrigin;

	public float raycastDistance = 2f;

	public LayerMask raycastLayers;

	public float raycastRadius = 1f;

	[Space]
	public ParticleSystem splashParticles;

	public float splashOffset = -0.35f;

	[Space]
	public ParticleSystem slashParticles;

	public AudioSource slashSound;

	public float minPitch = 0.85f;

	public float maxPitch = 1.15f;

	private void Start()
	{
		if ((bool)splashParticles)
		{
			splashParticles.Stop();
		}
	}

	private void Update()
	{
		if (!splashParticles)
		{
			return;
		}
		RaycastHit2D raycastHit2D = Physics2D.CircleCast((Vector2)base.transform.position + raycastOrigin, raycastRadius, Vector2.down, raycastDistance, raycastLayers);
		if (raycastHit2D.collider != null)
		{
			if (!splashParticles.isPlaying)
			{
				splashParticles.Play();
			}
			splashParticles.transform.position = raycastHit2D.point + Vector2.up * splashOffset;
		}
		else if (splashParticles.isPlaying)
		{
			splashParticles.Stop();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.enabled && collision.tag == "Nail Attack")
		{
			float num = Mathf.Sign(collision.transform.position.x - base.transform.position.x);
			if ((bool)slashParticles)
			{
				slashParticles.transform.SetScaleX(0f - num);
				slashParticles.transform.SetPositionY(collision.transform.position.y);
				slashParticles.Play();
			}
			if ((bool)slashSound)
			{
				slashSound.transform.SetPositionY(collision.transform.position.y);
				slashSound.pitch = Random.Range(minPitch, maxPitch);
				slashSound.Play();
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.DrawWireSphere(base.transform.position + (Vector3)raycastOrigin, raycastRadius);
			Gizmos.DrawLine(base.transform.position + (Vector3)raycastOrigin, base.transform.position + (Vector3)raycastOrigin + Vector3.down * raycastDistance);
		}
	}
}
