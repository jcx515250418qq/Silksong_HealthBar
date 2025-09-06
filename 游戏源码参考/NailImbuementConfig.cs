using System;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Nail Imbuement Config")]
public class NailImbuementConfig : ScriptableObject
{
	[Serializable]
	public class ImbuedLagHitOptions : LagHitOptions
	{
		public ParticleEffectsLerpEmission HitMarkedEffect;

		public EnemyHitEffectsProfile LagHitEffect;

		[Space]
		public SpriteFlash.FlashConfig EnemyHitFlash;

		public AudioEvent HitSound;

		public override bool IsExtraDamage => true;

		public override bool CanStack => false;

		public override void OnStart(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance, out ParticleEffectsLerpEmission spawnedHitMarkedEffect)
		{
			if ((bool)HitMarkedEffect)
			{
				spawnedHitMarkedEffect = HitMarkedEffect.Spawn(effectsPoint, effectOrigin);
				spawnedHitMarkedEffect.transform.SetPositionZ(HitMarkedEffect.transform.position.z);
				float duration = StartDelay + HitDelay * (float)HitCount;
				spawnedHitMarkedEffect.Play(duration);
			}
			else
			{
				spawnedHitMarkedEffect = null;
			}
		}

		public override void OnHit(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
		{
			if ((bool)LagHitEffect)
			{
				LagHitEffect.SpawnEffects(effectsPoint, effectOrigin, hitInstance, null);
			}
			SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
			if ((bool)component)
			{
				component.Flash(EnemyHitFlash);
			}
			HitSound.SpawnAndPlayOneShot(effectsPoint.position + effectOrigin);
		}
	}

	public Color NailTintColor = Color.white;

	public float Duration;

	public SpriteFlash.FlashConfig HeroFlashing;

	public Color ExtraHeroLightColor;

	public PlayParticleEffects HeroParticles;

	public EnemyHitEffectsProfile InertHitEffect;

	public EnemyHitEffectsProfile StartHitEffect;

	public GameObject SlashEffect;

	public AudioEvent ExtraSlashAudio;

	[Space]
	public float NailDamageMultiplier = 1f;

	[Space]
	public ToolItem ToolSource;

	[Space]
	public DamageTag DamageTag;

	[ModifiableProperty]
	[Conditional("DamageTag", true, false, false)]
	public int DamageTagTicksOverride;

	[Space]
	public MinMaxInt HitsToTag = new MinMaxInt(1, 1);

	[Range(0f, 1f)]
	public float LuckyHitChance;

	public MinMaxInt LuckyHitsToTag = new MinMaxInt(1, 1);

	[Space]
	[ModifiableProperty]
	[Conditional("DamageTag", false, false, false)]
	public ImbuedLagHitOptions LagHits;

	public void EnsurePersonalPool(GameObject gameObject)
	{
		if ((bool)StartHitEffect)
		{
			StartHitEffect.EnsurePersonalPool(gameObject);
		}
		if (LagHits != null && LagHits.LagHitEffect != null)
		{
			LagHits.LagHitEffect.EnsurePersonalPool(gameObject);
		}
	}
}
