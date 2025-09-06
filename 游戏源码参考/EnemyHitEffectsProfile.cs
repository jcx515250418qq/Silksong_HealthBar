using System;
using GlobalSettings;
using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Enemy Hit Effects Profile")]
public class EnemyHitEffectsProfile : ScriptableObject
{
	public enum EffectsTypes
	{
		Full = 0,
		Minimal = 1,
		LagHit = 2
	}

	[Serializable]
	public struct HitFlingConfig
	{
		public GameObject Prefab;

		public float SpeedMin;

		public float SpeedMax;

		public float OriginVariationX;

		public float OriginVariationY;

		public int AmountMin;

		public int AmountMax;

		public float AngleWidth;

		[Range(0f, 1f)]
		public float ProbabilitySelf;
	}

	[Serializable]
	public struct HitBloodConfig
	{
		public short MinCount;

		public short MaxCount;

		public float MinSpeed;

		public float MaxSpeed;

		public float AngleWidth;
	}

	[Serializable]
	public class ProfileSection
	{
		public HitBloodConfig[] Blood = new HitBloodConfig[0];

		public GameObject[] SpawnEffectPrefabs = new GameObject[0];

		public HitFlingConfig[] SpawnFlings = new HitFlingConfig[0];

		public AudioEvent[] DamageSounds = new AudioEvent[0];

		public VibrationDataAsset[] Vibrations = new VibrationDataAsset[0];

		public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance hitInstance, Color? bloodColorOverride, float blackThreadAmount = -1f)
		{
			Vector3 position = spawnPoint.TransformPoint(offset);
			float num;
			for (num = hitInstance.GetActualDirection(spawnPoint, HitInstance.TargetType.Regular); num < 0f; num += 360f)
			{
			}
			if (num.IsWithinTolerance(40f, 270f))
			{
				num = 90f;
			}
			HitBloodConfig[] blood = Blood;
			for (int i = 0; i < blood.Length; i++)
			{
				HitBloodConfig hitBloodConfig = blood[i];
				float num2 = hitBloodConfig.AngleWidth / 2f;
				BloodSpawner.Config config = default(BloodSpawner.Config);
				config.Position = offset;
				config.MinCount = hitBloodConfig.MinCount;
				config.MaxCount = hitBloodConfig.MaxCount;
				config.MinSpeed = hitBloodConfig.MinSpeed;
				config.MaxSpeed = hitBloodConfig.MaxSpeed;
				config.AngleMin = num - num2;
				config.AngleMax = num + num2;
				GameObject gameObject = BloodSpawner.SpawnBlood(config, spawnPoint, bloodColorOverride);
				if ((bool)gameObject)
				{
					FollowTransform follow = gameObject.GetComponent<FollowTransform>() ?? gameObject.AddComponent<FollowTransform>();
					follow.Target = spawnPoint;
					follow.Offset = offset;
					RecycleResetHandler.Add(gameObject, (Action)delegate
					{
						follow.Target = null;
					});
				}
			}
			AudioEvent[] damageSounds = DamageSounds;
			foreach (AudioEvent audioEvent in damageSounds)
			{
				audioEvent.SpawnAndPlayOneShot(position);
			}
			PlayRandomVibration();
			GameObject[] spawnEffectPrefabs = SpawnEffectPrefabs;
			foreach (GameObject gameObject2 in spawnEffectPrefabs)
			{
				if (!gameObject2)
				{
					continue;
				}
				GameObject gameObject3 = gameObject2.Spawn(position);
				if (blackThreadAmount > 0f)
				{
					BlackThreadEffectRendererGroup component = gameObject3.GetComponent<BlackThreadEffectRendererGroup>();
					if (component != null)
					{
						component.SetBlackThreadAmount(blackThreadAmount);
					}
				}
				gameObject3.transform.SetRotation2D(num);
			}
			HitFlingConfig[] spawnFlings = SpawnFlings;
			for (int i = 0; i < spawnFlings.Length; i++)
			{
				HitFlingConfig hitFlingConfig = spawnFlings[i];
				if (!(UnityEngine.Random.Range(0f, 1f) > hitFlingConfig.ProbabilitySelf))
				{
					float num3 = hitFlingConfig.AngleWidth / 2f;
					float angleMin = num - num3;
					float angleMax = num + num3;
					FlingUtils.Config config2 = default(FlingUtils.Config);
					config2.Prefab = hitFlingConfig.Prefab;
					config2.AmountMin = hitFlingConfig.AmountMin;
					config2.AmountMax = hitFlingConfig.AmountMax;
					config2.SpeedMin = hitFlingConfig.SpeedMin;
					config2.SpeedMax = hitFlingConfig.SpeedMax;
					config2.AngleMin = angleMin;
					config2.AngleMax = angleMax;
					config2.OriginVariationX = hitFlingConfig.OriginVariationX;
					config2.OriginVariationY = hitFlingConfig.OriginVariationY;
					FlingUtils.SpawnAndFling(config2, spawnPoint, offset);
				}
			}
			if (hitInstance.RageHit)
			{
				GameObject rageHitEffectPrefab = Effects.RageHitEffectPrefab;
				if ((bool)rageHitEffectPrefab)
				{
					rageHitEffectPrefab.Spawn(position).transform.SetRotation2D(num);
				}
			}
		}

		private void PlayRandomVibration()
		{
			if (Vibrations.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, Vibrations.Length);
				VibrationManager.PlayVibrationClipOneShot(Vibrations[num % Vibrations.Length], null);
			}
		}
	}

	[Serializable]
	private class ConditionalAlt
	{
		public HeroControllerConfig HeroConfig;

		public EnemyHitEffectsProfile Profile;
	}

	[SerializeField]
	private ProfileSection FullEffects;

	[Space]
	[SerializeField]
	private ProfileSection MinimalEffects;

	[Space]
	[SerializeField]
	private ProfileSection LagHitEffects;

	[Space]
	[SerializeField]
	private bool doHitFlash = true;

	[SerializeField]
	private bool overrideHitFlashColor;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideHitFlashColor", true, false, false)]
	private Color hitFlashColor;

	[Space]
	[SerializeField]
	private ConditionalAlt[] conditionalAlts;

	public bool DoHitFlash => Get().doHitFlash;

	public bool OverrideHitFlashColor => Get().overrideHitFlashColor;

	public Color HitFlashColor => Get().hitFlashColor;

	private EnemyHitEffectsProfile Get()
	{
		if (conditionalAlts == null || conditionalAlts.Length == 0)
		{
			return this;
		}
		HeroControllerConfig config = HeroController.instance.Config;
		ConditionalAlt[] array = conditionalAlts;
		foreach (ConditionalAlt conditionalAlt in array)
		{
			if (conditionalAlt.HeroConfig == config)
			{
				return conditionalAlt.Profile;
			}
		}
		return this;
	}

	public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance damageInstance, Color? bloodColorOverride = null, float blackThreadAmount = -1f)
	{
		(damageInstance.HitEffectsType switch
		{
			EffectsTypes.Full => Get().FullEffects, 
			EffectsTypes.Minimal => Get().MinimalEffects, 
			EffectsTypes.LagHit => Get().LagHitEffects, 
			_ => throw new NotImplementedException(), 
		}).SpawnEffects(spawnPoint, offset, damageInstance, bloodColorOverride, blackThreadAmount);
	}

	public void EnsurePersonalPool(GameObject gameObject)
	{
		EnsurePersonalPool(gameObject, FullEffects);
		EnsurePersonalPool(gameObject, MinimalEffects);
		EnsurePersonalPool(gameObject, LagHitEffects);
		PersonalObjectPool.CreateIfRequired(gameObject);
	}

	private void EnsurePersonalPool(GameObject gameObject, ProfileSection profileSection)
	{
		HitFlingConfig[] spawnFlings = profileSection.SpawnFlings;
		for (int i = 0; i < spawnFlings.Length; i++)
		{
			HitFlingConfig hitFlingConfig = spawnFlings[i];
			if (!(hitFlingConfig.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, hitFlingConfig.Prefab, 3, finished: false);
			}
		}
		GameObject[] spawnEffectPrefabs = profileSection.SpawnEffectPrefabs;
		foreach (GameObject gameObject2 in spawnEffectPrefabs)
		{
			if (!(gameObject2 == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, 3, finished: false);
			}
		}
	}
}
