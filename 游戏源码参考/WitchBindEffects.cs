using System;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class WitchBindEffects : MonoBehaviour
{
	[Serializable]
	private struct HealEffects
	{
		public ParticleSystem Emitter;

		public MinMaxInt Emit;
	}

	[SerializeField]
	private HealEffects healEffects;

	[SerializeField]
	private GameObject healFlashEffectPrefab;

	private bool[] hasDamaged;

	private DamageEnemies[] damagers;

	private int healCapLeft;

	private GameManager gameManager;

	private HeroController heroController;

	private SpriteFlash heroSpriteFlash;

	private void Awake()
	{
		gameManager = GameManager.instance;
		heroController = HeroController.instance;
		heroSpriteFlash = heroController.GetComponent<SpriteFlash>();
		damagers = GetComponentsInChildren<DamageEnemies>(includeInactive: true);
		hasDamaged = new bool[damagers.Length];
		for (int i = 0; i < damagers.Length; i++)
		{
			DamageEnemies obj = damagers[i];
			obj.DamagedEnemyHealthManager += OnDamagedEnemyHealthManager;
			int damagerIndex = i;
			obj.DamagedEnemy += delegate
			{
				if (!hasDamaged[damagerIndex])
				{
					hasDamaged[damagerIndex] = true;
					gameManager.FreezeMoment(FreezeMomentTypes.WitchBindHit);
					GlobalSettings.Camera.MainCameraShakeManager.DoShake(GlobalSettings.Camera.EnemyKillShake, this);
					healFlashEffectPrefab.Spawn(base.transform.position + new Vector3(0f, 0f, -0.21f));
				}
			};
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < hasDamaged.Length; i++)
		{
			hasDamaged[i] = false;
		}
		healCapLeft = heroController.GetWitchHealCap();
	}

	private void OnDamagedEnemyHealthManager(HealthManager hm)
	{
		if ((bool)hm && !hm.ShouldIgnore(HealthManager.IgnoreFlags.WitchHeal))
		{
			OnDamagedEnemy();
		}
	}

	private void OnDamagedEnemy()
	{
		if (healCapLeft > 0)
		{
			heroController.AddHealth(1);
			healCapLeft--;
			heroSpriteFlash.flashFocusHeal();
			if ((bool)healEffects.Emitter)
			{
				healEffects.Emitter.Emit(healEffects.Emit.GetRandomValue());
			}
		}
	}

	public void CancelDamage()
	{
		if (hasDamaged != null)
		{
			for (int i = 0; i < hasDamaged.Length; i++)
			{
				hasDamaged[i] = true;
			}
			healCapLeft = 0;
		}
	}
}
