using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroSilkAcid : MonoBehaviour
{
	private class SizzleTracker
	{
		public float SizzleStartDelay;

		public float SizzleTime;

		public float SizzleTimeLeft;

		public float SizzleStopCooldownLeft;

		public bool QueuedUpdateSizzleTime;

		public readonly List<SilkAcidRegion> InsideRegions = new List<SilkAcidRegion>();

		public readonly List<SilkAcidRegion> InsideRegionsTemp = new List<SilkAcidRegion>();

		public SpriteFlash.FlashHandle HeroFlashHandle;

		public Coroutine FadeRoutine;
	}

	[Serializable]
	private class SizzleConfig
	{
		public bool RequireSilk;

		public PlayParticleEffects LoopParticles;

		public PlayParticleEffects SilkSizzleParticles;

		public GameObject SilkTakeSpawn;

		public GameObject SilkProtectedSpawn;
	}

	public enum SizzleTypes
	{
		Acid = 0,
		Void = 1
	}

	[SerializeField]
	[ArrayForEnum(typeof(SizzleTypes))]
	private SizzleConfig[] sizzleConfigs;

	[Space]
	[SerializeField]
	private float sizzleStopCooldown;

	private HeroController hc;

	private SpriteFlash heroFlash;

	private tk2dSprite sprite;

	private MeshRenderer meshRenderer;

	private bool isInsideAnyRegion;

	private bool hadSilk;

	private readonly Dictionary<SizzleTypes, SizzleTracker> sizzles = new Dictionary<SizzleTypes, SizzleTracker>();

	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref sizzleConfigs, typeof(SizzleTypes));
	}

	private void Awake()
	{
		OnValidate();
		hc = GetComponent<HeroController>();
		heroFlash = GetComponent<SpriteFlash>();
		sprite = GetComponent<tk2dSprite>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		int silk = PlayerData.instance.silk;
		foreach (var (sizzleTypes2, sizzleTracker2) in sizzles)
		{
			if (!isInsideAnyRegion)
			{
				if (!(sizzleTracker2.SizzleStopCooldownLeft <= 0f))
				{
					sizzleTracker2.SizzleStopCooldownLeft -= Time.deltaTime;
					if (sizzleTracker2.SizzleStopCooldownLeft <= 0f)
					{
						sizzleTracker2.SizzleTimeLeft = 0f;
					}
				}
				break;
			}
			if (sizzleTracker2.SizzleTimeLeft > 0f)
			{
				Vector3 position = hc.transform.position;
				SizzleConfig sizzleConfig = sizzleConfigs[(int)sizzleTypes2];
				if ((bool)sizzleConfig.LoopParticles)
				{
					sizzleConfig.LoopParticles.transform.SetPosition2D(position);
				}
				if ((bool)sizzleConfig.SilkSizzleParticles)
				{
					sizzleConfig.SilkSizzleParticles.transform.SetPosition2D(position);
				}
				if (sizzleConfig.RequireSilk && silk <= 0)
				{
					sizzleTracker2.SizzleTimeLeft = sizzleTracker2.SizzleTime;
					if (hadSilk && (bool)sizzleConfig.SilkSizzleParticles)
					{
						sizzleConfig.SilkSizzleParticles.StopParticleSystems();
					}
				}
				else
				{
					if (!hadSilk && (bool)sizzleConfig.SilkSizzleParticles)
					{
						sizzleConfig.SilkSizzleParticles.PlayParticleSystems();
					}
					sizzleTracker2.SizzleTimeLeft -= Time.deltaTime;
					if (sizzleTracker2.SizzleTimeLeft <= 0f)
					{
						TakeSilk(sizzleTracker2, sizzleConfig);
					}
				}
			}
			hadSilk = silk > 0;
		}
	}

	private void TakeSilk(SizzleTracker sizzle, SizzleConfig config)
	{
		PlayerData instance = PlayerData.instance;
		bool flag = false;
		sizzle.InsideRegionsTemp.AddRange(sizzle.InsideRegions);
		foreach (SilkAcidRegion item in sizzle.InsideRegionsTemp)
		{
			if (item.IsProtected)
			{
				item.Dispel();
			}
			else
			{
				flag = true;
			}
		}
		sizzle.InsideRegionsTemp.Clear();
		if (flag)
		{
			if (instance.silk > 0)
			{
				hc.TakeSilk(1);
				if ((bool)config.SilkTakeSpawn)
				{
					config.SilkTakeSpawn.Spawn(base.transform.position);
				}
			}
		}
		else if ((bool)config.SilkProtectedSpawn)
		{
			config.SilkProtectedSpawn.Spawn(base.transform.position);
		}
		sizzle.InsideRegionsTemp.AddRange(sizzle.InsideRegions);
		foreach (SilkAcidRegion item2 in sizzle.InsideRegionsTemp)
		{
			item2.OnTakenSilk(!config.RequireSilk || instance.silk > 0);
		}
		sizzle.InsideRegionsTemp.Clear();
		if (!isInsideAnyRegion)
		{
			return;
		}
		if ((bool)config.SilkSizzleParticles)
		{
			if (!config.RequireSilk || instance.silk > 0)
			{
				config.SilkSizzleParticles.PlayParticleSystems();
			}
			else
			{
				config.SilkSizzleParticles.StopParticleSystems();
			}
		}
		if (sizzle.QueuedUpdateSizzleTime)
		{
			sizzle.QueuedUpdateSizzleTime = false;
			SilkAcidRegion shortestSizzleRegion = GetShortestSizzleRegion(sizzle);
			if ((bool)shortestSizzleRegion)
			{
				sizzle.SizzleTime = shortestSizzleRegion.SizzleTime;
			}
		}
		sizzle.SizzleTimeLeft = sizzle.SizzleTime;
	}

	private static SilkSpool.SilkUsingFlags MapToUseType(SizzleTypes sizzleTypes)
	{
		return sizzleTypes switch
		{
			SizzleTypes.Acid => SilkSpool.SilkUsingFlags.Acid, 
			SizzleTypes.Void => SilkSpool.SilkUsingFlags.Void, 
			_ => throw new ArgumentOutOfRangeException("sizzleTypes", sizzleTypes, null), 
		};
	}

	private SizzleTracker GetSizzleTracker(SizzleTypes usingType)
	{
		if (!sizzles.TryGetValue(usingType, out var value))
		{
			value = (sizzles[usingType] = new SizzleTracker());
		}
		return value;
	}

	private void StartSizzle(SizzleTypes sizzleTypes)
	{
		SizzleTracker sizzleTracker = GetSizzleTracker(sizzleTypes);
		if (sizzleTracker.SizzleTimeLeft <= 0f)
		{
			sizzleTracker.SizzleTimeLeft = sizzleTracker.SizzleTime + sizzleTracker.SizzleStartDelay;
		}
		SilkSpool.SilkUsingFlags usingFlags = MapToUseType(sizzleTypes);
		SilkSpool.Instance.AddUsing(usingFlags);
		SizzleConfig sizzleConfig = sizzleConfigs[(int)sizzleTypes];
		if ((bool)sizzleConfig.LoopParticles)
		{
			sizzleConfig.LoopParticles.PlayParticleSystems();
		}
		if ((!sizzleConfig.RequireSilk || PlayerData.instance.silk > 0) && (bool)sizzleConfig.SilkSizzleParticles)
		{
			sizzleConfig.SilkSizzleParticles.PlayParticleSystems();
		}
		if (sizzleTracker.FadeRoutine != null)
		{
			StopCoroutine(sizzleTracker.FadeRoutine);
			sizzleTracker.FadeRoutine = null;
		}
		if ((bool)heroFlash)
		{
			switch (sizzleTypes)
			{
			case SizzleTypes.Acid:
				sizzleTracker.HeroFlashHandle = heroFlash.Flash(new Color(82f / 85f, 0.9372549f, 36f / 85f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, repeating: true, 0, 2);
				break;
			case SizzleTypes.Void:
				sprite.EnableKeyword("BLACKTHREAD");
				if (meshRenderer.enabled)
				{
					sprite.SetFloat(_blackThreadAmountProp, 0f);
					sizzleTracker.FadeRoutine = this.StartTimerRoutine(0f, 0.2f, delegate(float t)
					{
						sprite.SetFloat(_blackThreadAmountProp, t);
					});
				}
				else
				{
					sprite.SetFloat(_blackThreadAmountProp, 1f);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("sizzleTypes", sizzleTypes, null);
			}
		}
		hadSilk = PlayerData.instance.silk > 0;
	}

	private void StopSizzle(SizzleTypes sizzleTypes)
	{
		SilkSpool.SilkUsingFlags usingFlags = MapToUseType(sizzleTypes);
		SilkSpool.Instance.RemoveUsing(usingFlags);
		SizzleConfig sizzleConfig = sizzleConfigs[(int)sizzleTypes];
		if ((bool)sizzleConfig.LoopParticles)
		{
			sizzleConfig.LoopParticles.StopParticleSystems();
		}
		if ((bool)sizzleConfig.SilkSizzleParticles)
		{
			sizzleConfig.SilkSizzleParticles.StopParticleSystems();
		}
		if (!sizzles.TryGetValue(sizzleTypes, out var value))
		{
			Debug.LogError("No sizzle was ever started for type: " + sizzleTypes);
			return;
		}
		if ((bool)heroFlash)
		{
			heroFlash.CancelRepeatingFlash(value.HeroFlashHandle);
		}
		if (value.FadeRoutine != null)
		{
			StopCoroutine(value.FadeRoutine);
			value.FadeRoutine = null;
		}
		if (sizzleTypes == SizzleTypes.Void)
		{
			value.FadeRoutine = this.StartTimerRoutine(0f, 0.5f, delegate(float t)
			{
				sprite.SetFloat(_blackThreadAmountProp, 1f - t);
			}, null, delegate
			{
				sprite.DisableKeyword("BLACKTHREAD");
			});
		}
		if (value.SizzleTimeLeft > 0f && sizzleStopCooldown > 0f)
		{
			value.SizzleStopCooldownLeft = sizzleStopCooldown;
		}
		else
		{
			value.SizzleTimeLeft = 0f;
		}
	}

	public void AddInside(SilkAcidRegion region)
	{
		SizzleTracker sizzleTracker = GetSizzleTracker(region.SizzleType);
		bool flag = sizzleTracker.InsideRegions.Count == 0;
		sizzleTracker.InsideRegions.AddIfNotPresent(region);
		sizzleTracker.QueuedUpdateSizzleTime = true;
		if (flag)
		{
			isInsideAnyRegion = true;
			hc.SetSilkRegenBlocked(isBlocked: true);
			sizzleTracker.SizzleStartDelay = region.SizzleStartDelay;
			sizzleTracker.SizzleTime = region.SizzleTime;
			StartSizzle(region.SizzleType);
			if (region.SizzleType == SizzleTypes.Void)
			{
				StatusVignette.AddStatus(StatusVignette.StatusTypes.Voided);
			}
		}
	}

	public void RemoveInside(SilkAcidRegion region)
	{
		SizzleTracker sizzleTracker = GetSizzleTracker(region.SizzleType);
		sizzleTracker.InsideRegions.Remove(region);
		sizzleTracker.QueuedUpdateSizzleTime = true;
		if (sizzleTracker.InsideRegions.Count == 0)
		{
			isInsideAnyRegion = false;
			hc.SetSilkRegenBlocked(isBlocked: false);
			StopSizzle(region.SizzleType);
			if (region.SizzleType == SizzleTypes.Void)
			{
				StatusVignette.RemoveStatus(StatusVignette.StatusTypes.Voided);
			}
		}
	}

	private SilkAcidRegion GetShortestSizzleRegion(SizzleTracker sizzle)
	{
		float num = float.MaxValue;
		SilkAcidRegion result = null;
		foreach (SilkAcidRegion insideRegion in sizzle.InsideRegions)
		{
			if (!(insideRegion.SizzleTime >= num))
			{
				num = insideRegion.SizzleTime;
				result = insideRegion;
			}
		}
		return result;
	}
}
