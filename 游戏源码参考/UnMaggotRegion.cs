using System.Collections;
using UnityEngine;

public class UnMaggotRegion : MonoBehaviour
{
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	[SerializeField]
	private MaggotRegion maggotRegion;

	[Space]
	[SerializeField]
	private AlertRange alertRange;

	[Space]
	[SerializeField]
	private ParticleSystem heroMaggotsPrefab;

	[SerializeField]
	private float heroMaggotsYPos;

	[SerializeField]
	private bool positionRelativeToHero;

	[Space]
	[SerializeField]
	private GameObject maggotsBurstPrefab;

	[SerializeField]
	private AudioEvent maggotsBurstAudio;

	private const float UN_MAGGOT_DELAY = 2f;

	private HeroController insideHero;

	private Coroutine unMaggotRoutine;

	private ParticleSystem spawnedHeroMaggots;

	private void Start()
	{
		if ((bool)maggotRegion && maggotRegion.IsActive)
		{
			return;
		}
		if ((bool)surfaceWater)
		{
			surfaceWater.HeroEntered += OnHeroEnteredWater;
			surfaceWater.HeroExited += OnHeroExitedWater;
		}
		else if ((bool)alertRange)
		{
			alertRange.InsideStateChanged += delegate(bool isInside)
			{
				if (isInside)
				{
					Entered();
				}
				else
				{
					Exited();
				}
			};
		}
		if ((bool)heroMaggotsPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, heroMaggotsPrefab.gameObject, 3);
		}
	}

	private void OnHeroEnteredWater(HeroController hc)
	{
		insideHero = hc;
		Entered();
	}

	private void Entered()
	{
		HeroController heroController = (insideHero ? insideHero : HeroController.instance);
		if (heroController.cState.isMaggoted && unMaggotRoutine == null)
		{
			unMaggotRoutine = StartCoroutine(UnMaggot(heroController));
		}
	}

	private void OnHeroExitedWater(HeroController hc)
	{
		insideHero = null;
		Exited();
	}

	private void Exited()
	{
		if (unMaggotRoutine != null)
		{
			StopCoroutine(unMaggotRoutine);
			unMaggotRoutine = null;
		}
		StopMaggots();
	}

	private void StopMaggots()
	{
		if (!(spawnedHeroMaggots == null))
		{
			spawnedHeroMaggots.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			FadeOutAudioSource component = spawnedHeroMaggots.GetComponent<FadeOutAudioSource>();
			if ((bool)component)
			{
				component.StartFade(0.5f);
			}
			spawnedHeroMaggots = null;
		}
	}

	private IEnumerator UnMaggot(HeroController hero)
	{
		spawnedHeroMaggots = heroMaggotsPrefab.Spawn();
		spawnedHeroMaggots.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		Transform heroTrans = hero.transform;
		Transform trans = spawnedHeroMaggots.transform;
		Vector3 position = heroTrans.position;
		float num = (positionRelativeToHero ? position.y : base.transform.position.y);
		trans.SetPosition2D(position.x, num + heroMaggotsYPos);
		spawnedHeroMaggots.Play(withChildren: true);
		for (float elapsed = 0f; elapsed < 2f; elapsed += Time.deltaTime)
		{
			if (trans != null)
			{
				trans.SetPositionX(heroTrans.position.x);
			}
			yield return null;
		}
		StopMaggots();
		Vector3 position2 = heroTrans.position;
		maggotsBurstPrefab.Spawn(position2);
		maggotsBurstAudio.SpawnAndPlayOneShot(position2);
		MaggotRegion.SetIsMaggoted(value: false);
		unMaggotRoutine = null;
	}
}
