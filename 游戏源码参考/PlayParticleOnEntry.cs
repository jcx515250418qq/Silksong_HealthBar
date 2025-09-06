using UnityEngine;

public class PlayParticleOnEntry : MonoBehaviour
{
	private ParticleSystem particle;

	private void Start()
	{
		particle = GetComponent<ParticleSystem>();
		if ((bool)particle)
		{
			particle.Stop();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((bool)particle)
		{
			particle.Play();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((bool)particle)
		{
			particle.Stop();
		}
	}
}
