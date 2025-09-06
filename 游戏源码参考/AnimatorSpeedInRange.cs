using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpeedInRange : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private ParticleSystem activeParticles;

	private bool hasStarted;

	private readonly List<Collider2D> insideColliders = new List<Collider2D>();

	private void Start()
	{
		animator.speed = 0f;
		activeParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		hasStarted = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (hasStarted)
		{
			insideColliders.AddIfNotPresent(other);
			if (insideColliders.Count == 1)
			{
				animator.speed = 1f;
				activeParticles.Play(withChildren: true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		insideColliders.Remove(other);
		if (insideColliders.Count == 0)
		{
			animator.speed = 0f;
			activeParticles.Stop(withChildren: true);
		}
	}
}
