using UnityEngine;

public class DisableParticleCollisonDelay : MonoBehaviour
{
	public float delay;

	private float timer;

	private bool played;

	private bool didCollisionEnd;

	private ParticleSystem particle_system;

	public void Awake()
	{
		particle_system = GetComponent<ParticleSystem>();
	}

	public void Update()
	{
		if (didCollisionEnd)
		{
			return;
		}
		if (!played && particle_system.IsAlive())
		{
			played = true;
		}
		if (played)
		{
			timer += Time.deltaTime;
			if (Time.deltaTime >= delay)
			{
				ParticleSystem.CollisionModule collision = particle_system.collision;
				collision.enabled = false;
			}
		}
	}
}
