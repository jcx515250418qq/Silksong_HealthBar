using System;
using GlobalSettings;
using UnityEngine;

[Serializable]
public class LagHitOptions
{
	public bool UseNailDamage;

	[ModifiableProperty]
	[Conditional("UseNailDamage", true, false, false)]
	public float NailDamageMultiplier = 1f;

	[ModifiableProperty]
	[Conditional("UseNailDamage", false, false, false)]
	public int HitDamage;

	public LagHitDamageType DamageType;

	public float StartDelay;

	public int HitCount;

	public float HitDelay;

	public float MagnitudeMult = 1f;

	public bool HitsGiveSilk;

	public GameObject[] SlashEffectOverrides;

	public bool IgnoreBlock;

	public virtual bool IsExtraDamage
	{
		get
		{
			if (DamageType == LagHitDamageType.Slash)
			{
				return false;
			}
			return true;
		}
	}

	public virtual bool CanStack => true;

	public bool ShouldDoLagHits()
	{
		return HitCount > 0;
	}

	public virtual void OnStart(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance, out ParticleEffectsLerpEmission spawnedHitMarkedEffect)
	{
		spawnedHitMarkedEffect = null;
		if (DamageType == LagHitDamageType.WitchPoison)
		{
			GameObject enemyWitchPoisonHitEffectPrefab = Effects.EnemyWitchPoisonHitEffectPrefab;
			if ((bool)enemyWitchPoisonHitEffectPrefab)
			{
				float hitDirectionAsAngle = hitInstance.GetHitDirectionAsAngle(HitInstance.TargetType.Regular);
				enemyWitchPoisonHitEffectPrefab.Spawn(effectsPoint.TransformPoint(effectOrigin), Quaternion.Euler(0f, 0f, hitDirectionAsAngle));
			}
			SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
			if ((bool)component)
			{
				component.FlashWitchPoison();
			}
		}
	}

	public virtual void OnHit(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
	{
		switch (DamageType)
		{
		case LagHitDamageType.WitchPoison:
			DoPoisonEffects(effectsPoint, effectOrigin);
			break;
		case LagHitDamageType.Dazzle:
		{
			SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
			if ((bool)component)
			{
				component.FlashDazzleQuick();
			}
			Vector3 vector = effectsPoint.TransformPoint(effectOrigin);
			Effects.EnemyDamageTickSoundTable.SpawnAndPlayOneShot(vector);
			GameObject[] slashEffectOverrides = SlashEffectOverrides;
			foreach (GameObject gameObject in slashEffectOverrides)
			{
				float? z = gameObject.transform.localPosition.z;
				gameObject.Spawn(vector.Where(null, null, z));
			}
			break;
		}
		}
	}

	private void DoPoisonEffects(Transform effectsPoint, Vector3 effectOrigin)
	{
		Vector3 vector = effectsPoint.TransformPoint(effectOrigin);
		SpriteFlash component = effectsPoint.GetComponent<SpriteFlash>();
		if ((bool)component)
		{
			component.FlashWitchPoison();
		}
		GameObject enemyWitchPoisonHurtEffectPrefab = Effects.EnemyWitchPoisonHurtEffectPrefab;
		if ((bool)enemyWitchPoisonHurtEffectPrefab)
		{
			float? z = enemyWitchPoisonHurtEffectPrefab.transform.localPosition.z;
			FollowTransform component2 = enemyWitchPoisonHurtEffectPrefab.Spawn(vector.Where(null, null, z)).GetComponent<FollowTransform>();
			if ((bool)component2)
			{
				component2.Target = effectsPoint;
			}
		}
		BloodSpawner.SpawnBlood(Effects.EnemyWitchPoisonBloodBurst, vector);
	}

	public virtual void OnEnd(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
	{
	}
}
