using GlobalSettings;
using UnityEngine;

public sealed class EnemyHitEffectsBasic : MonoBehaviour, IHitEffectReciever, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	public delegate void ReceivedHitEffectDelegate(HitInstance hitInstance, Vector2 effectOrigin2D);

	[SerializeField]
	private Vector3 effectOrigin;

	[Tooltip("Disable if there are no listeners for this event, to save the expensive recursive send event.")]
	public bool sendDamageFlashEvent = true;

	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile.ProfileSection profile;

	[SerializeField]
	private bool doHitFlash;

	[SerializeField]
	private Color hitFlashColor = Color.white;

	[SerializeField]
	private bool overrideBloodColor;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideBloodColor", true, false, true)]
	private Color bloodColorOverride;

	[SerializeField]
	private bool forceMinimalEffects;

	private SpriteFlash spriteFlash;

	private bool didFireThisFrame;

	private bool poolCreated;

	private bool hasAwaken;

	private bool hasStarted;

	private bool isBlackThreaded;

	public bool OverrideBloodColor
	{
		get
		{
			return overrideBloodColor;
		}
		set
		{
			overrideBloodColor = value;
		}
	}

	public Color BloodColorOverride
	{
		get
		{
			return bloodColorOverride;
		}
		set
		{
			bloodColorOverride = value;
		}
	}

	GameObject IInitialisable.gameObject => base.gameObject;

	public event ReceivedHitEffectDelegate ReceivedHitEffect;

	private void Awake()
	{
		OnAwake();
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		spriteFlash = GetComponent<SpriteFlash>();
		if (!poolCreated && profile != null)
		{
			EnsurePersonalPool(profile);
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	public void ReceiveHitEffect(float attackDirection)
	{
		ReceiveHitEffect(new HitInstance
		{
			Direction = attackDirection,
			AttackType = AttackTypes.Generic
		});
	}

	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		ReceiveHitEffect(hitInstance, effectOrigin);
	}

	public void ReceiveHitEffect(HitInstance hitInstance, Vector2 effectOrigin2D)
	{
		if (didFireThisFrame)
		{
			return;
		}
		didFireThisFrame = true;
		Vector3 vector = effectOrigin2D.ToVector3(effectOrigin.z);
		if (sendDamageFlashEvent)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", isRecursive: true);
		}
		bool flag = false;
		if (hitInstance.AttackType == AttackTypes.Coal)
		{
			flag = true;
			Effects.EnemyCoalHurtSound.SpawnAndPlayOneShot(base.transform.TransformPoint(vector));
		}
		if (!hitInstance.ForceNotWeakHit && hitInstance.DamageDealt <= 0 && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit)
		{
			Effects.WeakHitEffectShake.DoShake(this);
			GameObject weakHitEffectPrefab = Effects.WeakHitEffectPrefab;
			if ((bool)weakHitEffectPrefab)
			{
				float hitDirectionAsAngle = hitInstance.GetHitDirectionAsAngle(HitInstance.TargetType.Regular);
				weakHitEffectPrefab.Spawn(base.transform.TransformPoint(vector), Quaternion.Euler(0f, 0f, hitDirectionAsAngle));
			}
			return;
		}
		if (!flag && this.ReceivedHitEffect != null)
		{
			this.ReceivedHitEffect(hitInstance, effectOrigin2D);
		}
		if (hitInstance.AttackType != AttackTypes.Lightning)
		{
			_ = hitFlashColor;
		}
		else
		{
			_ = Effects.LightningHitEffects.HitFlashColor;
		}
		if (!flag)
		{
			if (doHitFlash && (bool)spriteFlash)
			{
				if (hitInstance.RageHit)
				{
					spriteFlash.FlashEnemyHitRage();
				}
				else
				{
					spriteFlash.FlashEnemyHit(hitInstance);
				}
			}
			if (forceMinimalEffects && hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full)
			{
				hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.Minimal;
			}
			if (overrideBloodColor)
			{
				SpawnEffects(base.transform, vector, hitInstance, bloodColorOverride);
			}
			else
			{
				SpawnEffects(base.transform, vector, hitInstance, null);
			}
		}
		else if ((bool)spriteFlash)
		{
			if (hitInstance.RageHit)
			{
				spriteFlash.FlashEnemyHitRage();
			}
			else
			{
				spriteFlash.FlashEnemyHit(hitInstance);
			}
		}
	}

	public void SetEffectOrigin(Vector3 newEffectOrigin)
	{
		effectOrigin = newEffectOrigin;
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}

	public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance damageInstance, Color? bloodColorOverride = null)
	{
		float blackThreadAmount = (isBlackThreaded ? 1 : 0);
		profile.SpawnEffects(spawnPoint, offset, damageInstance, bloodColorOverride, blackThreadAmount);
	}

	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			isBlackThreaded = true;
		}
		else
		{
			isBlackThreaded = false;
		}
	}

	private void EnsurePersonalPool(EnemyHitEffectsProfile.ProfileSection profileSection)
	{
		EnemyHitEffectsProfile.HitFlingConfig[] spawnFlings = profileSection.SpawnFlings;
		for (int i = 0; i < spawnFlings.Length; i++)
		{
			EnemyHitEffectsProfile.HitFlingConfig hitFlingConfig = spawnFlings[i];
			if (!(hitFlingConfig.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, hitFlingConfig.Prefab, 3, finished: false);
			}
		}
		GameObject[] spawnEffectPrefabs = profileSection.SpawnEffectPrefabs;
		foreach (GameObject gameObject in spawnEffectPrefabs)
		{
			if (!(gameObject == null))
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, gameObject, 3, finished: false);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(effectOrigin, 0.25f);
	}
}
