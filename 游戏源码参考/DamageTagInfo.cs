using UnityEngine;

public class DamageTagInfo
{
	public int Stacked;

	public int HitsLeft;

	public double NextHitTime;

	public GameObject SpawnedLoopEffect;

	public bool RemoveAfterNextHit;

	public bool hasLerpEmission;

	public ParticleEffectsLerpEmission lerpEmission;

	public void CheckLerpEmission()
	{
		if (!hasLerpEmission && (bool)SpawnedLoopEffect)
		{
			lerpEmission = SpawnedLoopEffect.GetComponent<ParticleEffectsLerpEmission>();
			hasLerpEmission = lerpEmission;
		}
	}

	public void StopLoopEffect()
	{
		if ((bool)SpawnedLoopEffect)
		{
			ParticleSystem[] componentsInChildren = SpawnedLoopEffect.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	public void RefreshEmission(float duration)
	{
		if (hasLerpEmission)
		{
			lerpEmission.Play(duration);
		}
	}
}
