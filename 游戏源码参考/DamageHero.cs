using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class DamageHero : MonoBehaviour, IInitialisable
{
	[Serializable]
	public class ClashEventsWrapper
	{
		public UnityEvent OnClashUp;

		public UnityEvent OnClashDown;

		public UnityEvent OnClashLeft;

		public UnityEvent OnClashRight;
	}

	[ModifiableProperty]
	[Conditional("damageAsset", false, false, false)]
	public int damageDealt = 1;

	public HazardType hazardType = HazardType.ENEMY;

	[SerializeField]
	[QuickCreateAsset("Data Assets/Damages", "damageDealt", "value")]
	private DamageReference damageAsset;

	[Space]
	[EnumPickerBitmask]
	public DamagePropertyFlags damagePropertyFlags;

	[Space]
	public bool resetOnEnable;

	private int? initialValue;

	public bool canClashTink;

	public bool forceParry;

	[ModifiableProperty]
	[Conditional("canClashTink", true, false, false)]
	public bool noClashFreeze;

	[ModifiableProperty]
	[Conditional("canClashTink", true, false, false)]
	public bool noTerrainThunk;

	public bool noTerrainRecoil;

	public bool noCorpseSpikeStick;

	public bool noBounceCooldown;

	[SerializeField]
	[Space]
	private bool overrideCollisionSide;

	[ModifiableProperty]
	[Conditional("overrideCollisionSide", true, false, true)]
	[SerializeField]
	private CollisionSide collisionSide;

	[SerializeField]
	private bool invertCollisionSide;

	[Space]
	public PlayMakerFSM HeroDamagedFSM;

	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	public bool AlwaysSendDamaged;

	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	[InspectorValidation("IsFsmEventValid")]
	public string HeroDamagedFSMEvent;

	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	[InspectorValidation("IsFsmBoolValid")]
	public string HeroDamagedFSMBool;

	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	public string HeroDamagedFSMGameObject;

	[Space]
	public ClashEventsWrapper ClashEvents;

	public UnityEvent OnDamagedHero;

	private bool preventClashTink;

	private double damageAllowedTime;

	private Coroutine nailClashRoutine;

	private Collider2D collider;

	private readonly ContactPoint2D[] contactsTempStore = new ContactPoint2D[10];

	private readonly Collider2D[] parentAttachedColliders = new Collider2D[10];

	private Collider2D parentCollider;

	private HealthManager healthManager;

	private Collider2D[] healthManagerColliders;

	private Recoil recoil;

	private static readonly Dictionary<GameObject, DamageHero> _damageHeroes = new Dictionary<GameObject, DamageHero>();

	private bool hasAwaken;

	private bool hasStarted;

	private bool hasNonBouncer;

	private NonBouncer nonBouncer;

	private bool cancelAttack;

	public bool OverrideCollisionSide => overrideCollisionSide;

	public CollisionSide CollisionSide => collisionSide;

	public bool InvertCollisionSide => invertCollisionSide;

	public bool CanCauseDamage => Time.timeAsDouble >= damageAllowedTime;

	GameObject IInitialisable.gameObject => base.gameObject;

	public event Action HeroDamaged;

	private bool? IsFsmEventValid(string eventName)
	{
		return HeroDamagedFSM.IsEventValid(eventName, isRequired: false);
	}

	private bool? IsFsmBoolValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName) || !HeroDamagedFSM)
		{
			return null;
		}
		return HeroDamagedFSM.FsmVariables.BoolVariables.Any((FsmBool fsmBool) => fsmBool.Name == eventName);
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		if ((bool)damageAsset)
		{
			damageDealt = damageAsset.Value;
		}
		healthManager = GetComponentInParent<HealthManager>();
		if ((bool)healthManager)
		{
			healthManagerColliders = healthManager.GetComponents<Collider2D>();
			healthManager.TookDamage += OnDamaged;
		}
		recoil = GetComponentInParent<Recoil>();
		nonBouncer = GetComponentInParent<NonBouncer>();
		hasNonBouncer = nonBouncer != null;
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		if (canClashTink && !noTerrainThunk && !component && (bool)collider)
		{
			collider.isTrigger = false;
			component = base.gameObject.AddComponent<Rigidbody2D>();
			component.bodyType = RigidbodyType2D.Kinematic;
			component.simulated = true;
			component.useFullKinematicContacts = true;
		}
		if ((bool)base.transform.parent)
		{
			parentCollider = base.transform.parent.GetComponent<Collider2D>();
		}
		if (hazardType == HazardType.STEAM)
		{
			FsmTemplate hornetMultiWounderFsmTemplate = Gameplay.HornetMultiWounderFsmTemplate;
			PlayMakerFSM playMakerFSM = base.gameObject.AddComponent<PlayMakerFSM>();
			playMakerFSM.Reset();
			playMakerFSM.SetFsmTemplate(hornetMultiWounderFsmTemplate);
			playMakerFSM.FsmVariables.FindFsmBool("z2 Steam Hazard").Value = true;
			damageDealt = 0;
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

	private void Awake()
	{
		_damageHeroes[base.gameObject] = this;
		OnAwake();
	}

	private void OnDestroy()
	{
		if ((bool)healthManager)
		{
			healthManager.TookDamage -= OnDamaged;
		}
		_damageHeroes.Remove(base.gameObject);
	}

	private void OnEnable()
	{
		if (resetOnEnable)
		{
			if (!initialValue.HasValue)
			{
				initialValue = damageDealt;
			}
			else
			{
				damageDealt = initialValue.Value;
			}
		}
		nailClashRoutine = null;
		preventClashTink = false;
		if ((bool)base.transform.parent && (bool)collider)
		{
			Rigidbody2D componentInParent = base.transform.parent.GetComponentInParent<Rigidbody2D>();
			if ((bool)componentInParent)
			{
				int attachedColliders = componentInParent.GetAttachedColliders(parentAttachedColliders);
				for (int i = 0; i < attachedColliders; i++)
				{
					Physics2D.IgnoreCollision(parentAttachedColliders[i], collider);
				}
			}
		}
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Danger);
	}

	private void OnDisable()
	{
		if (cancelAttack)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				instance.NailParryRecover();
			}
			cancelAttack = false;
		}
	}

	public static bool TryGet(GameObject gameObject, out DamageHero damageHero)
	{
		return _damageHeroes.TryGetValue(gameObject, out damageHero);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.enabled)
		{
			TryClashTinkCollider(collision);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (canClashTink && collision.gameObject.layer == 8 && !noTerrainThunk)
		{
			int recoilDir;
			int surfaceDir;
			if (!noTerrainRecoil && (bool)recoil)
			{
				TerrainThunkUtils.GenerateTerrainThunk(collision, contactsTempStore, TerrainThunkUtils.SlashDirection.None, recoil.transform.position, out recoilDir, out surfaceDir, TerrainThunkCondition);
				if (surfaceDir != 3 && surfaceDir != 1)
				{
					recoil.RecoilByDirection(recoilDir, 0.5f);
				}
			}
			else
			{
				TerrainThunkUtils.GenerateTerrainThunk(collision, contactsTempStore, TerrainThunkUtils.SlashDirection.None, Vector2.zero, out recoilDir, out surfaceDir, TerrainThunkCondition);
			}
		}
		TryClashTinkCollider(collision.collider);
	}

	private bool TerrainThunkCondition(TerrainThunkUtils.TerrainThunkConditionArgs args)
	{
		if (args.RecoilDirection == 1)
		{
			return false;
		}
		if (!parentCollider)
		{
			return true;
		}
		Vector3 min = parentCollider.bounds.min;
		return args.ThunkPos.y >= min.y + 0.05f;
	}

	private void TryClashTinkCollider(Collider2D collision)
	{
		if (hazardType == HazardType.SPIKES)
		{
			DamageEnemies component = collision.GetComponent<DamageEnemies>();
			if ((bool)component)
			{
				component.OnHitSpikes();
			}
		}
		if (!canClashTink || nailClashRoutine != null || preventClashTink || collision.gameObject.layer != 16)
		{
			return;
		}
		string text = collision.gameObject.tag;
		Transform obj = collision.transform;
		Vector3 position = obj.position;
		Transform parent = obj.parent;
		if (!parent)
		{
			return;
		}
		DamageEnemies componentInChildren = parent.GetComponentInChildren<DamageEnemies>();
		if (!componentInChildren)
		{
			return;
		}
		if (healthManager != null)
		{
			if (componentInChildren.HasBeenDamaged(healthManager))
			{
				return;
			}
			componentInChildren.PreventDamage(healthManager);
			healthManager.CancelLagHitsForSource(componentInChildren.gameObject);
		}
		HeroController instance = HeroController.instance;
		if (componentInChildren.doesNotParry)
		{
			return;
		}
		if (text == "Nail Attack" && !noClashFreeze && instance.parryInvulnTimer < Mathf.Epsilon)
		{
			GameManager.instance.FreezeMoment(FreezeMomentTypes.NailClashEffect);
		}
		componentInChildren.SendParried(!hasNonBouncer || !nonBouncer.active);
		if (healthManagerColliders != null)
		{
			Collider2D[] array = healthManagerColliders;
			foreach (Collider2D col in array)
			{
				componentInChildren.PreventDamage(col);
			}
		}
		NailAttackBase component2 = componentInChildren.GetComponent<NailAttackBase>();
		if ((bool)component2 && !component2.CanHitSpikes)
		{
			instance.TakeDamage(damageSide: (!overrideCollisionSide) ? (DirectionUtils.GetCardinalDirection(componentInChildren.direction) switch
			{
				0 => CollisionSide.right, 
				2 => CollisionSide.left, 
				3 => CollisionSide.bottom, 
				1 => CollisionSide.top, 
				_ => CollisionSide.other, 
			}) : collisionSide, go: base.gameObject, damageAmount: 1, hazardType: HazardType.ENEMY, damagePropertyFlags: damagePropertyFlags);
		}
		else
		{
			nailClashRoutine = StartCoroutine(NailClash(componentInChildren.direction, text, position));
		}
	}

	private IEnumerator NailClash(float direction, string colliderTag, Vector3 clasherPos)
	{
		HeroController hc = HeroController.instance;
		Effects.NailClashTinkShake.DoShake(this, shouldFreeze: false);
		Effects.NailClashParrySound.SpawnAndPlayOneShot(base.transform.position);
		if (colliderTag == "Nail Attack")
		{
			hc.NailParry();
			cancelAttack = true;
			if (direction < 45f)
			{
				if (noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(1.5f, 0f, 0f));
				}
				else
				{
					hc.RecoilLeft();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(1.5f, 0f, 0f));
				}
				ClashEvents.OnClashRight.Invoke();
			}
			else if (direction < 135f)
			{
				if (noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(0f, 1.5f, 0f));
				}
				else
				{
					hc.RecoilDown();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(0f, 1.5f, 0f));
				}
				ClashEvents.OnClashUp.Invoke();
			}
			else if (direction < 225f)
			{
				if (noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(-1.5f, 0f, 0f));
				}
				else
				{
					hc.RecoilRight();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(-1.5f, 0f, 0f));
				}
				ClashEvents.OnClashLeft.Invoke();
			}
			else if (direction < 360f)
			{
				if (noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(-1.5f * hc.gameObject.transform.localScale.x, -1f, 0f));
				}
				else
				{
					hc.DownspikeBounce(harpoonRecoil: false);
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(-1.5f * hc.gameObject.transform.localScale.x, -1f, 0f));
				}
				ClashEvents.OnClashDown.Invoke();
			}
		}
		else
		{
			cancelAttack = false;
			Effects.NailClashParryEffect.Spawn(clasherPos);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "PARRIED");
		if ((bool)base.transform.parent)
		{
			FSMUtility.SendEventToGameObject(base.transform.parent.gameObject, "PARRIED");
		}
		yield return new WaitForSeconds(0.1f);
		if (cancelAttack)
		{
			hc.NailParryRecover();
			cancelAttack = false;
		}
		yield return null;
		nailClashRoutine = null;
	}

	private void OnDamaged()
	{
		preventClashTink = true;
	}

	public void SendHeroDamagedEvent()
	{
		if (this.HeroDamaged != null)
		{
			this.HeroDamaged();
		}
		if (HeroDamagedFSM != null)
		{
			if (!string.IsNullOrEmpty(HeroDamagedFSMEvent))
			{
				HeroDamagedFSM.SendEvent(HeroDamagedFSMEvent);
			}
			if (!string.IsNullOrEmpty(HeroDamagedFSMBool))
			{
				FsmBool fsmBool = HeroDamagedFSM.FsmVariables.BoolVariables.FirstOrDefault((FsmBool b) => b.Name == HeroDamagedFSMBool);
				if (fsmBool != null)
				{
					fsmBool.Value = true;
				}
			}
			if (!string.IsNullOrEmpty(HeroDamagedFSMGameObject))
			{
				FsmGameObject fsmGameObject = HeroDamagedFSM.FsmVariables.GameObjectVariables.FirstOrDefault((FsmGameObject b) => b.Name == HeroDamagedFSMGameObject);
				if (fsmGameObject != null)
				{
					fsmGameObject.Value = base.transform.gameObject;
				}
			}
		}
		OnDamagedHero.Invoke();
	}

	public void SetDamageAmount(int amount)
	{
		damageDealt = amount;
	}

	public void SetCooldown(float cooldown)
	{
		if (!(cooldown <= 0f))
		{
			double num = Time.timeAsDouble + (double)cooldown;
			if (num > damageAllowedTime)
			{
				damageAllowedTime = num;
			}
		}
	}

	public bool IsDamagerSpikes()
	{
		if (hazardType == HazardType.SPIKES)
		{
			return true;
		}
		return false;
	}

	[ContextMenu("Test", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Test")]
	private void Test()
	{
		HeroController.instance.GetComponentInChildren<HeroBox>().TakeDamageFromDamager(this, base.gameObject);
	}
}
