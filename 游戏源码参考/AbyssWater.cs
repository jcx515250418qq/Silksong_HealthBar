using System.Collections;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class AbyssWater : MonoBehaviour
{
	[SerializeField]
	private GameObject effectPrefab;

	[SerializeField]
	private float effectFadeOutDuration;

	[Space]
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZoneMask;

	[SerializeField]
	private OverrideBool overrideActive;

	private HeroController insideHero;

	private bool IsActive
	{
		get
		{
			if (overrideActive.IsEnabled)
			{
				if (!overrideActive.Value)
				{
					return false;
				}
			}
			else
			{
				MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
				if (!mapZoneMask.IsBitSet((int)currentMapZoneEnum))
				{
					return false;
				}
			}
			return true;
		}
	}

	private void Start()
	{
		if (IsActive)
		{
			if ((bool)effectPrefab)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, effectPrefab.gameObject, 2);
			}
			surfaceWater.HeroEntered += OnHeroEnteredWater;
			surfaceWater.HeroExited += OnHeroExitedWater;
		}
	}

	private void OnHeroEnteredWater(HeroController hc)
	{
		insideHero = hc;
		if ((bool)effectPrefab)
		{
			StartCoroutine(EffectFollowHero());
		}
	}

	private void OnHeroExitedWater(HeroController hc)
	{
		insideHero = null;
	}

	private IEnumerator EffectFollowHero()
	{
		GameObject spawnedEffect = effectPrefab.Spawn();
		NestedFadeGroupBase group = spawnedEffect.GetComponent<NestedFadeGroupBase>();
		group.AlphaSelf = 1f;
		ParticleSystem[] particles = spawnedEffect.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		Transform heroTrans = insideHero.transform;
		Transform trans = spawnedEffect.transform;
		trans.position = heroTrans.position;
		do
		{
			trans.position = heroTrans.position;
			yield return null;
		}
		while ((bool)insideHero);
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		if ((bool)group)
		{
			for (float elapsed = 0f; elapsed < effectFadeOutDuration; elapsed += Time.deltaTime)
			{
				trans.position = heroTrans.position;
				float num = elapsed / effectFadeOutDuration;
				group.AlphaSelf = 1f - num;
				yield return null;
			}
		}
		while (true)
		{
			trans.position = heroTrans.position;
			bool flag = false;
			array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		spawnedEffect.Recycle();
	}
}
