using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class LavaBellCoalRegionShield : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private float fadeUpTime;

	[SerializeField]
	private float fadeDownTime;

	[SerializeField]
	private ParticleSystem[] controlEmission;

	private void OnEnable()
	{
		fadeGroup.AlphaSelf = 0f;
		SetParticleEmission(value: false);
	}

	public void Begin()
	{
		StopAllCoroutines();
		fadeGroup.FadeTo(1f, fadeUpTime);
		SetParticleEmission(value: true);
	}

	public void End()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(FadeToDeactivate());
		}
	}

	private void SetParticleEmission(bool value)
	{
		ParticleSystem[] array = controlEmission;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.EmissionModule emission = array[i].emission;
			emission.enabled = value;
		}
	}

	private IEnumerator FadeToDeactivate()
	{
		SetParticleEmission(value: false);
		yield return new WaitForSeconds(fadeGroup.FadeTo(0f, fadeDownTime));
		ParticleSystem[] array = controlEmission;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		bool waitForParticles = true;
		while (waitForParticles)
		{
			waitForParticles = false;
			yield return null;
			array = controlEmission;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(withChildren: true))
				{
					waitForParticles = true;
					break;
				}
			}
		}
		base.gameObject.SetActive(value: false);
	}
}
