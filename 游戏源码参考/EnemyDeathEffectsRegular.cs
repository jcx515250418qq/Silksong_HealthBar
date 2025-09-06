using HutongGames.PlayMaker;
using UnityEngine;

[RequireComponent(typeof(EnemyHitEffectsRegular))]
public class EnemyDeathEffectsRegular : EnemyDeathEffects
{
	[ActionCategory("Hollow Knight")]
	public class SimulateDeath : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				HealthManager component2 = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					float? attackDirection2;
					if ((bool)component2)
					{
						int attackDirection = component2.GetAttackDirection();
						attackDirection2 = DirectionUtils.GetAngle(attackDirection);
					}
					else
					{
						attackDirection2 = null;
					}
					bool didCallCorpseBegin;
					GameObject corpseObj = ((!component.IsCorpseRecyclable) ? null : component.EmitCorpse(attackDirection2, 1f, AttackTypes.Generic, NailElements.None, null, null, out didCallCorpseBegin));
					component.EmitEffects(corpseObj);
					component.RecordKillForJournal();
				}
			}
			Finish();
		}
	}

	[ActionCategory("Hollow Knight")]
	public class SimulateDeathV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool ForceEmitIfCorpseNotRecyclable;

		public override void Reset()
		{
			Target = null;
			ForceEmitIfCorpseNotRecyclable = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				HealthManager component2 = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					float? attackDirection2;
					if ((bool)component2)
					{
						int attackDirection = component2.GetAttackDirection();
						attackDirection2 = DirectionUtils.GetAngle(attackDirection);
					}
					else
					{
						attackDirection2 = null;
					}
					bool didCallCorpseBegin;
					GameObject corpseObj = ((!component.IsCorpseRecyclable && !ForceEmitIfCorpseNotRecyclable.Value) ? null : component.EmitCorpse(attackDirection2, 1f, AttackTypes.Generic, NailElements.None, null, null, out didCallCorpseBegin));
					component.EmitEffects(corpseObj);
					component.RecordKillForJournal();
				}
			}
			Finish();
		}
	}

	[ActionCategory("Hollow Knight")]
	public class SimulateDeathNoCorpse : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				if (component != null)
				{
					component.EmitEffects(null);
					component.RecordKillForJournal();
				}
			}
			Finish();
		}
	}

	[Space]
	[SerializeField]
	private EnemyDeathEffectsProfile profile;

	private EnemyHitEffectsRegular hitEffects;

	private bool poolCreated;

	protected override Color? OverrideBloodColor
	{
		get
		{
			if (!hitEffects.OverrideBloodColor)
			{
				return null;
			}
			return hitEffects.BloodColorOverride;
		}
	}

	public EnemyDeathEffectsProfile Profile
	{
		get
		{
			return profile;
		}
		set
		{
			if (profile != value)
			{
				profile = value;
				if ((bool)profile)
				{
					profile.EnsurePersonalPool(base.gameObject);
					poolCreated = true;
				}
			}
		}
	}

	public override bool OnAwake()
	{
		if (base.OnAwake())
		{
			hitEffects = GetComponent<EnemyHitEffectsRegular>();
			if (!poolCreated && (bool)profile)
			{
				profile.EnsurePersonalPool(base.gameObject);
			}
			return true;
		}
		return false;
	}

	protected override void EmitEffects(GameObject corpseObj)
	{
		bool overrideBloodColor = hitEffects.OverrideBloodColor;
		Color bloodColorOverride = hitEffects.BloodColorOverride;
		EmitSound();
		ShakeCameraIfVisible();
		Transform transform = null;
		if (corpseObj != null)
		{
			transform = corpseObj.transform;
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.FlashEnemyHit();
			}
		}
		if ((bool)profile)
		{
			float blackThreadAmount = GetBlackThreadAmount();
			if (overrideBloodColor)
			{
				profile.SpawnEffects(base.transform, effectOrigin, transform, bloodColorOverride, blackThreadAmount);
				return;
			}
			EnemyDeathEffectsProfile enemyDeathEffectsProfile = profile;
			Transform spawnPoint = base.transform;
			Vector3 offset = effectOrigin;
			Transform corpse = transform;
			float blackThreadAmount2 = blackThreadAmount;
			enemyDeathEffectsProfile.SpawnEffects(spawnPoint, offset, corpse, null, blackThreadAmount2);
		}
	}
}
