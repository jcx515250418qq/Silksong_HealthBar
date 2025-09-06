using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

public class MaggotRegion : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem heroMaggotsPrefab;

	[SerializeField]
	private ParticleSystem otherMaggotsPrefab;

	[SerializeField]
	private float heroMaggotsYPos;

	[SerializeField]
	private Animator heroMaggotShieldPrefab;

	[Space]
	[SerializeField]
	private EnemyJournalRecord maggotJournalRecord;

	[SerializeField]
	private MinMaxInt maggotJournalRecordAmount;

	[SerializeField]
	private ParticleSystem maggotKillEffect;

	[Space]
	[SerializeField]
	private MinMaxInt lightningKillAmount = new MinMaxInt(3, 4);

	[SerializeField]
	private ParticleSystem lightningKillEffect;

	[Space]
	[SerializeField]
	private SurfaceWaterRegion surfaceWater;

	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZoneMask;

	[SerializeField]
	private OverrideBool overrideActive;

	[SerializeField]
	private bool forceSwampColour;

	private const float TAKE_SILK_DELAY = 0.5f;

	private const float EXIT_COOLDOWN = 1f;

	private static readonly int _endId = Animator.StringToHash("End");

	private HeroController insideHero;

	private bool isMaggoted;

	private double lastSilkTime;

	private Coroutine takeSilkRoutine;

	private Coroutine heroMaggotsRoutine;

	private ParticleSystem spawnedHeroMaggots;

	private Animator spawnedHeroShield;

	private static readonly List<MaggotRegion> _insideRegions = new List<MaggotRegion>();

	[UsedImplicitly]
	public static bool IsInsideAny => _insideRegions.Count > 0;

	public bool IsActive
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
			if ((bool)heroMaggotsPrefab)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, heroMaggotsPrefab.gameObject, 5, finished: false);
			}
			if ((bool)heroMaggotShieldPrefab)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, heroMaggotShieldPrefab.gameObject, 3, finished: false);
			}
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
			surfaceWater.HeroEntered += OnHeroEnteredWater;
			surfaceWater.HeroExited += OnHeroExitedWater;
			surfaceWater.CorpseEntered += OnOtherEnteredWater;
		}
		else if (!forceSwampColour)
		{
			return;
		}
		surfaceWater.Color = Effects.MossEffectsTintDust;
	}

	private void OnDisable()
	{
		_insideRegions.Remove(this);
	}

	private void OnHeroEnteredWater(HeroController hc)
	{
		insideHero = hc;
		_insideRegions.Add(this);
		EventRegister.SendEvent(EventRegisterEvents.MaggotCheck);
		if (Gameplay.MaggotCharm.IsEquipped && insideHero.playerData.MaggotCharmHits < 3)
		{
			insideHero.AddToMaggotCharmTimer(Gameplay.MaggotCharmEnterWaterAddTime);
			heroMaggotsRoutine = StartCoroutine(HeroInMaggotRegion());
		}
		else
		{
			StartHeroMaggoted();
			heroMaggotsRoutine = StartCoroutine(HeroInMaggotRegion());
		}
	}

	private void StartHeroMaggoted()
	{
		isMaggoted = true;
		SilkSpool.Instance.AddUsing(SilkSpool.SilkUsingFlags.Maggot);
		insideHero.SetSilkRegenBlocked(isBlocked: true);
		takeSilkRoutine = StartCoroutine(TakeSilk());
		SetIsMaggoted(value: true);
		StatusVignette.AddStatus(StatusVignette.StatusTypes.InMaggotRegion);
	}

	private void OnHeroExitedWater(HeroController hc)
	{
		insideHero = null;
		_insideRegions.Remove(this);
		EventRegister.SendEvent(EventRegisterEvents.MaggotCheck);
		if (heroMaggotsRoutine != null)
		{
			StopCoroutine(heroMaggotsRoutine);
			heroMaggotsRoutine = null;
			if ((bool)spawnedHeroMaggots)
			{
				spawnedHeroMaggots.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
				FadeOutAudioSource component = spawnedHeroMaggots.GetComponent<FadeOutAudioSource>();
				if ((bool)component)
				{
					component.StartFade(0.5f);
				}
				spawnedHeroMaggots = null;
			}
			EndHeroShield();
		}
		if (isMaggoted)
		{
			isMaggoted = false;
			StatusVignette.RemoveStatus(StatusVignette.StatusTypes.InMaggotRegion);
			SilkSpool.Instance.RemoveUsing(SilkSpool.SilkUsingFlags.Maggot);
			hc.SetSilkRegenBlocked(isBlocked: false);
			StopCoroutine(takeSilkRoutine);
			takeSilkRoutine = null;
		}
	}

	private void EndHeroShield()
	{
		if ((bool)spawnedHeroShield)
		{
			spawnedHeroShield.SetTrigger(_endId);
			AutoRecycleSelf component = spawnedHeroShield.GetComponent<AutoRecycleSelf>();
			if ((bool)component)
			{
				component.ActivateTimer();
			}
			spawnedHeroShield = null;
		}
	}

	private void OnOtherEnteredWater(Vector2 position)
	{
		if ((bool)otherMaggotsPrefab)
		{
			ParticleSystem particleSystem = otherMaggotsPrefab.Spawn();
			particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleSystem.transform.SetPosition2D(position);
			particleSystem.Play(withChildren: true);
		}
	}

	private IEnumerator TakeSilk()
	{
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		float num = (float)(Time.timeAsDouble - lastSilkTime);
		float num2 = 0.5f - num;
		if (num2 > -1f)
		{
			if (num2 > 0f)
			{
				yield return new WaitForSeconds(num2);
			}
		}
		else
		{
			lastSilkTime = Time.timeAsDouble;
			yield return wait;
		}
		while (true)
		{
			insideHero.TakeSilk(1);
			lastSilkTime = Time.timeAsDouble;
			yield return wait;
		}
	}

	private IEnumerator HeroInMaggotRegion()
	{
		Transform heroTrans = insideHero.transform;
		Transform heroMaggotsTrans;
		if ((bool)heroMaggotsPrefab)
		{
			spawnedHeroMaggots = heroMaggotsPrefab.Spawn();
			spawnedHeroMaggots.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			heroMaggotsTrans = spawnedHeroMaggots.transform;
			heroMaggotsTrans.SetPosition2D(heroTrans.position.x, base.transform.position.y + heroMaggotsYPos);
			spawnedHeroMaggots.Play(withChildren: true);
		}
		else
		{
			heroMaggotsTrans = null;
		}
		Transform heroShieldTrans;
		if (!isMaggoted && (bool)heroMaggotShieldPrefab)
		{
			spawnedHeroShield = heroMaggotShieldPrefab.Spawn(heroTrans.position);
			heroShieldTrans = spawnedHeroShield.transform;
			heroShieldTrans.SetPositionZ(heroMaggotShieldPrefab.transform.position.z);
		}
		else
		{
			heroShieldTrans = null;
		}
		while (true)
		{
			Vector3 position = heroTrans.position;
			if ((bool)heroMaggotsTrans)
			{
				heroMaggotsTrans.SetPositionX(position.x);
			}
			if ((bool)heroShieldTrans)
			{
				heroShieldTrans.SetPosition2D(position);
			}
			if (!isMaggoted)
			{
				insideHero.AddToMaggotCharmTimer(Time.deltaTime);
				if (insideHero.playerData.MaggotCharmHits >= 3)
				{
					EndHeroShield();
					StartHeroMaggoted();
				}
			}
			yield return null;
		}
	}

	public static void SetIsMaggoted(bool value)
	{
		HeroController.instance.SetIsMaggoted(value);
	}

	public void ReportExplosion(Vector2 position)
	{
		if (IsActive)
		{
			maggotJournalRecord.Get(maggotJournalRecordAmount.GetRandomValue());
			if ((bool)maggotKillEffect)
			{
				position.y = base.transform.position.y + heroMaggotsYPos;
				ParticleSystem particleSystem = maggotKillEffect.Spawn();
				particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				particleSystem.transform.SetPosition2D(position);
				particleSystem.Play(withChildren: true);
			}
		}
	}

	public void ReportLightningExplosion(Vector2 position)
	{
		if (IsActive)
		{
			maggotJournalRecord.Get(lightningKillAmount.GetRandomValue());
			if ((bool)lightningKillEffect)
			{
				position.y = base.transform.position.y + heroMaggotsYPos;
				ParticleSystem particleSystem = lightningKillEffect.Spawn();
				particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				particleSystem.transform.SetPosition2D(position);
				particleSystem.Play(withChildren: true);
			}
		}
	}
}
