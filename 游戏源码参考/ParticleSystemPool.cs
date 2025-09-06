using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : MonoBehaviour, IInitialisable
{
	[SerializeField]
	private ParticleSystem original;

	[SerializeField]
	private int initialPoolSize = 1;

	[SerializeField]
	private bool deparentOnPlay;

	private Transform parentTrans;

	private Vector3 initialPos;

	private Quaternion initialRotation;

	private Vector3 initialScale;

	private bool noParticle;

	private readonly List<ParticleSystem> spawnedParticles = new List<ParticleSystem>();

	private bool hasAwaken;

	private bool hasStarted;

	private bool hasParticle;

	GameObject IInitialisable.gameObject => base.gameObject;

	private void OnValidate()
	{
		if (initialPoolSize <= 0)
		{
			initialPoolSize = 1;
		}
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		if (original == null)
		{
			base.enabled = false;
			return true;
		}
		hasParticle = true;
		Transform transform = original.transform;
		parentTrans = transform.parent;
		initialPos = transform.localPosition;
		initialRotation = transform.localRotation;
		initialScale = transform.localScale;
		ParticleSystem.MainModule main = original.main;
		main.playOnAwake = false;
		original.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		spawnedParticles.Add(original);
		for (int i = 0; i < initialPoolSize - 1; i++)
		{
			ParticleSystem particleSystem = Object.Instantiate(original, original.transform.parent);
			particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			spawnedParticles.Add(particleSystem);
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	public void PlayParticles()
	{
		if (!hasParticle)
		{
			return;
		}
		bool flag = true;
		ParticleSystem particleSystem = null;
		for (int i = 0; i < spawnedParticles.Count; i++)
		{
			ParticleSystem particleSystem2 = spawnedParticles[i];
			if (!particleSystem2.IsAlive(withChildren: true))
			{
				particleSystem = particleSystem2;
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ParticleSystem particleSystem3 = Object.Instantiate(original, original.transform.parent);
			spawnedParticles.Add(particleSystem3);
			particleSystem = particleSystem3;
		}
		particleSystem.gameObject.SetActive(value: false);
		if (deparentOnPlay)
		{
			Transform obj = particleSystem.transform;
			obj.parent = parentTrans;
			obj.localPosition = initialPos;
			obj.localRotation = initialRotation;
			obj.localScale = initialScale;
			obj.SetParent(null, worldPositionStays: true);
		}
		particleSystem.gameObject.SetActive(value: true);
		particleSystem.Play();
	}

	public void StopParticles()
	{
		foreach (ParticleSystem spawnedParticle in spawnedParticles)
		{
			spawnedParticle.Stop(withChildren: true);
		}
	}

	public bool IsAlive()
	{
		foreach (ParticleSystem spawnedParticle in spawnedParticles)
		{
			if (spawnedParticle.IsAlive(withChildren: true))
			{
				return true;
			}
		}
		return false;
	}
}
