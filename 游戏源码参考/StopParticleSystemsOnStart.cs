using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticleSystemsOnStart : MonoBehaviour, IBeginStopper
{
	[SerializeField]
	private int frameDelay;

	[SerializeField]
	private GameObject[] onlyChildren;

	private List<ParticleSystem> systems;

	private Coroutine stopDelayRoutine;

	private void OnValidate()
	{
		if (frameDelay < 0)
		{
			frameDelay = 0;
		}
	}

	private void Awake()
	{
		systems = new List<ParticleSystem>();
		if (onlyChildren == null || onlyChildren.Length == 0)
		{
			GetComponentsInChildren(includeInactive: true, systems);
			return;
		}
		GameObject[] array = onlyChildren;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.GetComponentsInChildren(includeInactive: true, systems);
			}
		}
	}

	private void OnEnable()
	{
		DoBeginStop();
	}

	public void DoBeginStop()
	{
		if (stopDelayRoutine != null)
		{
			StopCoroutine(stopDelayRoutine);
		}
		stopDelayRoutine = StartCoroutine(StopDelay());
	}

	private void OnDisable()
	{
		StopCoroutine(stopDelayRoutine);
	}

	private IEnumerator StopDelay()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		for (int i = 0; i <= frameDelay; i++)
		{
			yield return wait;
		}
		DoStop();
	}

	public void DoStop()
	{
		foreach (ParticleSystem system in systems)
		{
			if (!system.CompareTag("Ignore Particle Stop"))
			{
				system.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
	}
}
