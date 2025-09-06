using System;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class DamageEnemies : DebugDrawColliderRuntimeAdder, CustomPlayerLoop.ILateFixedUpdate
{
	public struct HitResponse
	{
		public GameObject Target;

		public IHitResponder Responder;

		public HitInstance Hit;

		public HealthManager HealthManager;

		public PhysLayers LayerOnHit;
	}

	private enum DirectionSourceOverrides
	{
		None = 0,
		CircleDirection = 1,
		AwayFromHero = 2
	}

	private sealed class HitOrderComparer : IComparer<HitResponse>
	{
		public int Compare(HitResponse x, HitResponse y)
		{
			return y.Responder.HitPriority.CompareTo(x.Responder.HitPriority);
		}
	}

	public delegate void EndedDamageDelegate(bool didHit);

	private struct SpikeSlashData
	{
		public SpikeSlashReaction spikeSlashReaction;

		public Collider2D collider2D;

		public SpikeSlashData(SpikeSlashReaction spikeSlashReaction, Collider2D collider2D)
		{
			this.spikeSlashReaction = spikeSlashReaction;
			this.collider2D = collider2D;
		}
	}

	[Serializable]
	[Flags]
	private enum DoesNotFlags
	{
		None = 0,
		PoisonTicks = 1,
		LightningTicks = 2
	}

	private static HitOrderComparer orderComparer = new HitOrderComparer();

	public AttackTypes attackType = AttackTypes.Generic;

	[EnumPickerBitmask]
	public SpecialTypes specialType;

	[Space]
	public bool useNailDamage;

	[ModifiableProperty]
	[Conditional("useNailDamage", true, false, false)]
	public float nailDamageMultiplier = 1f;

	[ModifiableProperty]
	[Conditional("ShowDamageDealt", true, true, false)]
	public int damageDealt;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("useNailDamage", false, false, false)]
	[QuickCreateAsset("Data Assets/Damages", "damageDealt", "value")]
	private DamageReference damageAsset;

	[ModifiableProperty]
	[Conditional("useNailDamage", false, false, false)]
	public bool useHeroDamageAffectors;

	public int damageDealtOffset;

	[SerializeField]
	private bool ignoreNailPosition;

	[SerializeField]
	private float[] damageMultPerHit;

	[ModifiableProperty]
	[Conditional("doesNotStun", false, false, false)]
	public float stunDamage = 1f;

	public bool canWeakHit;

	[SerializeField]
	private bool onlyDamageEnemies;

	[SerializeField]
	private int onlyDamageEnemiesChance = 100;

	[SerializeField]
	private bool isHeroDamage;

	[Space]
	[SerializeField]
	private SharedDamagedGroup sharedDamagedGroup;

	private bool hasSharedDamageGroup;

	[Space]
	[SerializeField]
	private DirectionSourceOverrides directionSourceOverride;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool circleDirection;

	public float direction;

	public OverrideFloat corpseDirection;

	[SerializeField]
	private bool canTriggerBouncePod;

	[SerializeField]
	private bool useBouncePodDirection;

	[SerializeField]
	private float bouncePodDirection;

	[SerializeField]
	private bool flipDirectionIfXScaleNegative;

	[Space]
	[SerializeField]
	private bool flipDirectionIfBehind;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("flipDirectionIfBehind", true, false, false)]
	private Vector2 forwardVector;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("flipDirectionIfBehind", true, false, false)]
	private float flippedDirMagnitude = 1f;

	[Space]
	public bool ignoreInvuln = true;

	public float magnitudeMult;

	public OverrideFloat corpseMagnitudeMult;

	public OverrideFloat currencyMagnitudeMult;

	public bool moveDirection;

	[SerializeField]
	private ToolItem representingTool;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private ToolDamageFlags damageFlags;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private int poisonDamageTicks;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private int zapDamageTicks;

	public bool manualTrigger;

	public bool hitOnceUntilEnd;

	public bool endDamageEnemyExit;

	[Space]
	[SerializeField]
	private int damageScalingLevel;

	[Space]
	public bool multiHitter;

	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public int stepsPerHit;

	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public int hitsUntilDeath;

	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public float finalHitMagMultOverride;

	[ModifiableProperty]
	[Conditional("HandlesMultiHitsDeath", true, true, false)]
	public PlayMakerFSM deathEventTarget;

	[ModifiableProperty]
	[InspectorValidation("IsDeathEventValid")]
	[Conditional("ShowDeathEventField", true, true, false)]
	public string deathEvent;

	[ModifiableProperty]
	[Conditional("HandlesMultiHitsDeath", true, true, false)]
	public bool deathEndDamage;

	public string contactFSMEvent;

	public string damageFSMEvent;

	public PlayMakerFSM dealtDamageFSM;

	public string dealtDamageFSMEvent;

	public string targetRecordedFSMEvent;

	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public EnemyHitEffectsProfile.EffectsTypes multiHitFirstEffects;

	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public EnemyHitEffectsProfile.EffectsTypes multiHitEffects;

	[Space]
	public GameObject[] slashEffectOverrides;

	[ModifiableProperty]
	[Conditional("multiHitter", false, false, false)]
	public bool minimalHitEffects;

	public bool doesNotStun;

	public bool doesNotTink;

	public bool doesNotTinkThroughWalls;

	public bool doesNotParry;

	[SerializeField]
	private DoesNotFlags doesNotFlags;

	[SerializeField]
	private ITinkResponder.TinkFlags allowedTinkFlags;

	public bool nonLethal;

	[SerializeField]
	private bool doesNotCriticalHit;

	[SerializeField]
	private bool forceSpikeUpdate;

	[SerializeField]
	private HitSilkGeneration silkGeneration;

	[SerializeField]
	private bool awardJournalKill = true;

	[SerializeField]
	private FreezeMomentTypes firstHitFreeze = FreezeMomentTypes.None;

	[Space]
	[SerializeField]
	private LagHitOptionsProfile lagHitOptionsProfile;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("lagHitOptionsProfile", false, false, false)]
	private LagHitOptions lagHitOptions;

	[Space]
	[SerializeField]
	public UnityEvent DealtDamage;

	[SerializeField]
	public UnityEvent Tinked;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool doesNotGenerateSilk;

	private int stepsToNextHit;

	private int hitsLeftUntilDeath;

	private bool endedDamage;

	private GameObject lastTargetDamaged;

	private GameObject lastRecordedTarget;

	private bool isPoisonDamageOverridden;

	private bool wasCriticalHitForced;

	private readonly HashSet<Collider2D> frameQueue = new HashSet<Collider2D>();

	private readonly HashSet<Collider2D> enteredColliders = new HashSet<Collider2D>();

	private readonly List<Collider2D> evaluatingColliders = new List<Collider2D>();

	private readonly HashSet<Collider2D> damagedColliders = new HashSet<Collider2D>();

	private readonly HashSet<IHitResponder> hitsResponded = new HashSet<IHitResponder>();

	private readonly List<IHitResponder> tempHitsResponded = new List<IHitResponder>();

	private readonly HashSet<IHitResponder> damagePrevented = new HashSet<IHitResponder>();

	private List<HitResponse> currentDamageBuffer = new List<HitResponse>();

	private List<HitResponse> processingDamageBuffer = new List<HitResponse>();

	private List<SpikeSlashData> spikeSlashReactions = new List<SpikeSlashData>();

	private int lastRespondedCycle = -1;

	private Dictionary<GameObject, int> hitCounts;

	private float damageMultiplier = 1f;

	private bool isProcessingBuffer;

	private bool isDoingDamage;

	private bool sourceIsHero;

	private bool isNailAttack;

	private bool started;

	private readonly DamageStack tempDamageStack = new DamageStack();

	public bool IgnoreNailPosition => ignoreNailPosition;

	public bool OnlyDamageEnemies => onlyDamageEnemies;

	public ToolItem RepresentingTool => representingTool;

	public int PoisonDamageTicks
	{
		get
		{
			if (isPoisonDamageOverridden)
			{
				return poisonDamageTicks;
			}
			if (doesNotFlags.HasFlag(DoesNotFlags.PoisonTicks))
			{
				return 0;
			}
			if (!Gameplay.PoisonPouchTool.Status.IsEquipped)
			{
				return 0;
			}
			if (!representingTool)
			{
				return poisonDamageTicks;
			}
			return representingTool.PoisonDamageTicks;
		}
	}

	public int ZapDamageTicks
	{
		get
		{
			if (doesNotFlags.HasFlag(DoesNotFlags.LightningTicks))
			{
				return 0;
			}
			if (!Gameplay.ZapImbuementTool.Status.IsEquipped)
			{
				return 0;
			}
			if (!representingTool)
			{
				return zapDamageTicks;
			}
			return representingTool.ZapDamageTicks;
		}
	}

	public ITinkResponder.TinkFlags AllowedTinkFlags => allowedTinkFlags;

	public float? ExtraUpDirection { get; set; }

	public NailElements NailElement { get; set; }

	public NailImbuementConfig NailImbuement { get; set; }

	public bool AwardJournalKill
	{
		get
		{
			return awardJournalKill;
		}
		set
		{
			awardJournalKill = value;
		}
	}

	public bool DidHit { get; private set; }

	public bool DidHitEnemy { get; private set; }

	public bool CircleDirection => directionSourceOverride == DirectionSourceOverrides.CircleDirection;

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier;
		}
		set
		{
			damageMultiplier = value;
		}
	}

	public LagHitOptions LagHits
	{
		get
		{
			if (!lagHitOptionsProfile)
			{
				return lagHitOptions;
			}
			return lagHitOptionsProfile.Options;
		}
	}

	bool CustomPlayerLoop.ILateFixedUpdate.isActiveAndEnabled => base.isActiveAndEnabled;

	public event Action WillDamageEnemy;

	public event Action<Collider2D> WillDamageEnemyCollider;

	public event Action<HealthManager, HitInstance> WillDamageEnemyOptions;

	public event Action ParriedEnemy;

	public event EndedDamageDelegate EndedDamage;

	public event Action DamagedEnemy;

	public event Action<GameObject> DamagedEnemyGameObject;

	public event Action<HealthManager> DamagedEnemyHealthManager;

	public event Action MultiHitEvaluated;

	public event Action<HitResponse> HitResponded;

	private bool? IsDeathEventValid()
	{
		return deathEventTarget.IsEventValid(deathEvent, isRequired: false);
	}

	private bool ShowDeathEventField()
	{
		if (HandlesMultiHitsDeath())
		{
			return deathEventTarget;
		}
		return false;
	}

	private bool HandlesMultiHitsDeath()
	{
		if (multiHitter)
		{
			return hitsUntilDeath > 0;
		}
		return false;
	}

	[UsedImplicitly]
	private bool ShowDamageDealt()
	{
		if (!useNailDamage)
		{
			return !damageAsset;
		}
		return false;
	}

	private void OnValidate()
	{
		if (doesNotGenerateSilk)
		{
			silkGeneration = HitSilkGeneration.None;
			doesNotGenerateSilk = false;
		}
		if (circleDirection)
		{
			directionSourceOverride = DirectionSourceOverrides.CircleDirection;
			circleDirection = false;
		}
		if (attackType == AttackTypes.Piercer_OBSOLETE)
		{
			specialType |= SpecialTypes.Piercer;
			attackType = AttackTypes.Generic;
		}
	}

	protected override void Awake()
	{
		CustomPlayerLoop.RegisterLateFixedUpdate(this);
		base.Awake();
		OnValidate();
		if (multiHitter)
		{
			SpriteFlash spriteFlash = GetComponent<SpriteFlash>();
			if ((bool)spriteFlash)
			{
				WillDamageEnemy += delegate
				{
					spriteFlash.flashWhiteQuick();
				};
			}
		}
		if ((bool)damageAsset)
		{
			damageDealt = damageAsset.Value;
		}
		if (onlyDamageEnemiesChance < 100)
		{
			CheckOnlyDamageEnemies();
		}
		hasSharedDamageGroup = sharedDamagedGroup;
	}

	private void Start()
	{
		isNailAttack = base.gameObject.CompareTag("Nail Attack");
		sourceIsHero = isNailAttack || GetComponentInParent<HeroController>() != null || representingTool != null;
		started = true;
		ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
		}
		hitsResponded.Clear();
		StartDamage();
		if (onlyDamageEnemiesChance < 100)
		{
			CheckOnlyDamageEnemies();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		DoDamage(collision.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.isActiveAndEnabled || (HandlesMultiHitsDeath() && hitsLeftUntilDeath <= 0))
		{
			return;
		}
		PhysLayers layer = (PhysLayers)collision.gameObject.layer;
		if (layer == PhysLayers.HERO_BOX || layer == PhysLayers.PLAYER || layer == PhysLayers.ENEMY_ATTACK || layer == PhysLayers.CORPSE || layer == PhysLayers.ATTACK_DETECTOR)
		{
			return;
		}
		SpikeSlashReaction spikeSlashReaction = null;
		bool? flag = null;
		if (layer == PhysLayers.HERO_ATTACK && !collision.gameObject.CompareTag("Enemy Projectile") && !collision.gameObject.CompareTag("Hero Damage Target") && !collision.GetComponent<TinkEffect>())
		{
			spikeSlashReaction = collision.GetComponent<SpikeSlashReaction>();
			flag = spikeSlashReaction;
			if (flag.HasValue && !flag.Value)
			{
				return;
			}
		}
		if (forceSpikeUpdate)
		{
			if (!flag.HasValue)
			{
				spikeSlashReaction = collision.GetComponent<SpikeSlashReaction>();
				flag = spikeSlashReaction;
			}
			if (flag.HasValue && flag.Value)
			{
				spikeSlashReactions.Add(new SpikeSlashData(spikeSlashReaction, collision));
			}
		}
		enteredColliders.Add(collision);
		frameQueue.Add(collision);
		if (contactFSMEvent != "")
		{
			FSMUtility.SendEventToGameObject(collision.gameObject, contactFSMEvent);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		enteredColliders.Remove(collision);
	}

	private void OnDisable()
	{
		ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate -= OnFixedUpdate;
		EndDamage();
		ClearLists();
	}

	public void ClearLists()
	{
		ClearDamageBuffer();
		if (!isProcessingBuffer)
		{
			processingDamageBuffer.Clear();
		}
		hitsResponded.Clear();
		if (!isDoingDamage)
		{
			tempHitsResponded.Clear();
		}
		ClearPreventDamage();
		damagedColliders.Clear();
		spikeSlashReactions.Clear();
		frameQueue.Clear();
		enteredColliders.Clear();
		evaluatingColliders.Clear();
	}

	private void OnDestroy()
	{
		CustomPlayerLoop.UnregisterLateFixedUpdate(this);
	}

	private void OnFixedUpdate()
	{
		if (multiHitter && stepsToNextHit > 0)
		{
			stepsToNextHit--;
		}
	}

	public void ForceUpdate()
	{
		LateFixedUpdate();
	}

	public void LateFixedUpdate()
	{
		hitsResponded.Clear();
		evaluatingColliders.Clear();
		if (frameQueue.Count > 0)
		{
			foreach (Collider2D enteredCollider in enteredColliders)
			{
				frameQueue.Add(enteredCollider);
			}
			evaluatingColliders.AddRange(frameQueue);
			frameQueue.Clear();
		}
		else
		{
			evaluatingColliders.AddRange(enteredColliders);
		}
		bool flag = true;
		if (!multiHitter)
		{
			if (!manualTrigger)
			{
				EvaluateDamage();
			}
			else
			{
				EvaluateSpikeColliders();
			}
		}
		else if (stepsToNextHit <= 0 && evaluatingColliders.Count > 0)
		{
			ClearPreventDamage();
			if (EvaluateDamage())
			{
				stepsToNextHit = stepsPerHit;
			}
			this.MultiHitEvaluated?.Invoke();
		}
		else
		{
			if (stepsToNextHit > 0 && evaluatingColliders.Count > 0)
			{
				EvaluateDamage();
			}
			flag = false;
		}
		evaluatingColliders.Clear();
		spikeSlashReactions.Clear();
		if (flag && DidHit && endDamageEnemyExit && currentDamageBuffer.Count == 0)
		{
			EndDamage();
		}
		else
		{
			ProcessDamageBuffer();
		}
	}

	private bool EvaluateDamage()
	{
		bool result = false;
		for (int i = 0; i < evaluatingColliders.Count; i++)
		{
			Collider2D collider2D = evaluatingColliders[i];
			if (collider2D == null)
			{
				enteredColliders.Remove(collider2D);
			}
			else if (!collider2D.isActiveAndEnabled)
			{
				enteredColliders.Remove(collider2D);
			}
			else if (attackType == AttackTypes.Spikes || damagedColliders.Add(collider2D) || PreventDamage(collider2D))
			{
				if (DoDamage(collider2D))
				{
					result = true;
				}
			}
			else if (multiHitter && stepsToNextHit <= 0 && DoDamage(collider2D, isFirstHit: false))
			{
				result = true;
			}
		}
		return result;
	}

	private void EvaluateSpikeColliders()
	{
		for (int num = spikeSlashReactions.Count - 1; num >= 0; num--)
		{
			SpikeSlashData spikeSlashData = spikeSlashReactions[num];
			if (!(spikeSlashData.spikeSlashReaction == null) && spikeSlashData.spikeSlashReaction.isActiveAndEnabled)
			{
				if (attackType == AttackTypes.Spikes || damagedColliders.Add(spikeSlashData.collider2D))
				{
					if (!DoDamage(spikeSlashData.collider2D))
					{
					}
				}
				else if (multiHitter && stepsToNextHit <= 0)
				{
					DoDamage(spikeSlashData.collider2D, isFirstHit: false);
				}
			}
		}
		spikeSlashReactions.Clear();
	}

	private void ProcessDamageBuffer()
	{
		List<HitResponse> list = currentDamageBuffer;
		List<HitResponse> list2 = processingDamageBuffer;
		processingDamageBuffer = list;
		currentDamageBuffer = list2;
		if (processingDamageBuffer.Count == 0)
		{
			return;
		}
		bool flag = false;
		try
		{
			isProcessingBuffer = true;
			foreach (HitResponse item in processingDamageBuffer)
			{
				if (HasBeenDamaged(item.Responder))
				{
					continue;
				}
				HitInstance hit = item.Hit;
				if (multiHitter && hitsLeftUntilDeath == 1 && finalHitMagMultOverride > 0f)
				{
					hit.MagnitudeMultiplier = finalHitMagMultOverride;
				}
				IHitResponder.HitResponse hitResponse = item.Responder.Hit(hit);
				if ((IHitResponder.Response)hitResponse == IHitResponder.Response.None)
				{
					continue;
				}
				if (hitResponse.consumeCharges)
				{
					flag = true;
				}
				hitsResponded.Add(item.Responder);
				lastRespondedCycle = CustomPlayerLoop.FixedUpdateCycle;
				if (hitOnceUntilEnd)
				{
					PreventDamage(item.Responder);
				}
				if (!DidHit && firstHitFreeze > FreezeMomentTypes.None)
				{
					GameManager.instance.FreezeMoment(firstHitFreeze);
				}
				this.HitResponded?.Invoke(item);
				DidHit = true;
				if ((bool)item.HealthManager)
				{
					bool num = (IHitResponder.Response)hitResponse == IHitResponder.Response.DamageEnemy;
					if (num)
					{
						item.HealthManager.DoLagHits(LagHits, hit);
					}
					DidHitEnemy = true;
					if (attackType != AttackTypes.Spikes && !manualTrigger)
					{
						PreventDamage(item.HealthManager);
					}
					if (num)
					{
						this.DamagedEnemyGameObject?.Invoke(item.Target);
						this.DamagedEnemyHealthManager?.Invoke(item.HealthManager);
						this.DamagedEnemy?.Invoke();
						DoEnemyDamageNailImbuement(item.HealthManager, item.Hit);
					}
				}
			}
		}
		finally
		{
			isProcessingBuffer = false;
			processingDamageBuffer.Clear();
			if (wasCriticalHitForced)
			{
				HeroController instance = HeroController.instance;
				HeroController.WandererCrestStateInfo wandererState = instance.WandererState;
				wandererState.QueuedNextHitCritical = false;
				instance.WandererState = wandererState;
				wasCriticalHitForced = false;
			}
		}
		if (!flag || hitsLeftUntilDeath <= 0)
		{
			return;
		}
		hitsLeftUntilDeath--;
		if (hitsLeftUntilDeath <= 0)
		{
			if (!string.IsNullOrEmpty(deathEvent))
			{
				deathEventTarget.SendEvent(deathEvent);
			}
			if (deathEndDamage)
			{
				EndDamage();
			}
		}
	}

	private void CheckOnlyDamageEnemies()
	{
		if (UnityEngine.Random.Range(1, 101) <= onlyDamageEnemiesChance)
		{
			onlyDamageEnemies = true;
		}
		else
		{
			onlyDamageEnemies = false;
		}
	}

	public void setOnlyDamageEnemies(bool onlyDamage)
	{
		onlyDamageEnemies = onlyDamage;
	}

	public void SetNailDamageMultiplier(float multiplier)
	{
		nailDamageMultiplier = multiplier;
	}

	public static bool IsNailAttackObject(GameObject gameObject)
	{
		if (!gameObject.CompareTag("Nail Attack"))
		{
			return gameObject.GetComponentInParent<HeroExtraNailSlash>();
		}
		return true;
	}

	public bool DoDamage(Collider2D col, bool isFirstHit = true)
	{
		this.WillDamageEnemyCollider?.Invoke(col);
		return DoDamage(col.gameObject, isFirstHit);
	}

	public bool TryDoDamage(Collider2D target, bool isFirstHit = true)
	{
		if (target == null || !target.isActiveAndEnabled)
		{
			return false;
		}
		if (attackType == AttackTypes.Spikes || damagedColliders.Add(target) || PreventDamage(target))
		{
			return DoDamage(target);
		}
		return false;
	}

	public bool DoDamage(GameObject target, bool isFirstHit = true)
	{
		NailElements nailElement = NailElement;
		NailImbuementConfig nailImbuement = NailImbuement;
		try
		{
			if (attackType == AttackTypes.Fire)
			{
				NailElement = NailElements.Fire;
				NailImbuement = Effects.FireNail;
			}
			if (target == null)
			{
				return false;
			}
			isDoingDamage = true;
			HeroController instance = HeroController.instance;
			tempDamageStack.SetupNew(damageDealt);
			float num = damageDealtOffset;
			float num2 = magnitudeMult;
			bool rageHit = false;
			bool hunterCombo = false;
			if (useNailDamage || useHeroDamageAffectors)
			{
				if (useNailDamage)
				{
					tempDamageStack.SetupNew(PlayerData.instance.nailDamage);
					tempDamageStack.AddMultiplier(nailDamageMultiplier);
				}
				if ((bool)NailImbuement)
				{
					tempDamageStack.AddMultiplier(NailImbuement.NailDamageMultiplier);
					num2 *= NailImbuement.NailDamageMultiplier;
				}
				if (instance.WarriorState.IsInRageMode && IsNailAttackObject(base.gameObject))
				{
					float warriorDamageMultiplier = Gameplay.WarriorDamageMultiplier;
					tempDamageStack.AddMultiplier(warriorDamageMultiplier);
					num2 *= warriorDamageMultiplier;
					rageHit = true;
				}
				AttackTypes attackTypes = attackType;
				if (attackTypes == AttackTypes.Nail || attackTypes == AttackTypes.NailBeam || isNailAttack)
				{
					HeroController.HunterUpgCrestStateInfo hunterUpgState = instance.HunterUpgState;
					if (hunterUpgState.CurrentMeterHits > 0)
					{
						if (Gameplay.HunterCrest2.IsEquipped)
						{
							if (hunterUpgState.CurrentMeterHits >= Gameplay.HunterComboHits)
							{
								tempDamageStack.AddMultiplier(Gameplay.HunterComboDamageMult);
								hunterCombo = true;
							}
						}
						else if (Gameplay.HunterCrest3.IsEquipped)
						{
							if (hunterUpgState.IsComboMeterAboveExtra)
							{
								tempDamageStack.AddMultiplier(Gameplay.HunterCombo2ExtraDamageMult);
								hunterCombo = true;
							}
							else if (hunterUpgState.CurrentMeterHits >= Gameplay.HunterCombo2Hits)
							{
								tempDamageStack.AddMultiplier(Gameplay.HunterCombo2DamageMult);
								hunterCombo = true;
							}
						}
					}
					if (instance.SilkTauntEffectConsume())
					{
						tempDamageStack.AddMultiplier(1.5f);
					}
					ToolItem barbedWireTool = Gameplay.BarbedWireTool;
					if ((bool)barbedWireTool && barbedWireTool.Status.IsEquipped)
					{
						tempDamageStack.AddMultiplier(Gameplay.BarbedWireDamageDealtMultiplier);
					}
				}
				tempDamageStack.AddOffset(0f - num);
			}
			else if ((bool)representingTool && representingTool.Type.IsAttackType())
			{
				float multiplier = 1f + (float)PlayerData.instance.ToolKitUpgrades * Gameplay.ToolKitDamageIncrease;
				tempDamageStack.AddMultiplier(multiplier);
				if (representingTool.Type == ToolItemType.Skill && Gameplay.ZapImbuementTool.Status.IsEquipped)
				{
					tempDamageStack.AddMultiplier(Gameplay.ZapDamageMult);
				}
			}
			bool criticalHit = false;
			if ((!doesNotCriticalHit && attackType == AttackTypes.Nail && isFirstHit) || isNailAttack)
			{
				if (instance.WandererState.QueuedNextHitCritical)
				{
					criticalHit = true;
					wasCriticalHitForced = true;
				}
				else if (!instance.WandererState.CriticalHitsLocked && instance.IsWandererLucky && UnityEngine.Random.Range(0f, 1f) <= Gameplay.WandererCritChance * instance.GetLuckModifier())
				{
					criticalHit = true;
				}
			}
			tempHitsResponded.Clear();
			HitTaker.GetHitResponders(tempHitsResponded, target, hitsResponded);
			HealthManager healthManager = null;
			bool flag = false;
			for (int i = 0; i < tempHitsResponded.Count; i++)
			{
				IHitResponder hitResponder = tempHitsResponded[i];
				healthManager = hitResponder as HealthManager;
				if ((bool)healthManager)
				{
					if (healthManager.IsPartOfSendToTarget && (bool)healthManager.SendDamageTo)
					{
						tempHitsResponded[i] = healthManager.SendDamageTo;
						healthManager = healthManager.SendDamageTo;
					}
					break;
				}
				if ((specialType & SpecialTypes.Taunt) == 0 && !flag && (hitResponder is ReceivedDamageProxy || hitResponder is global::HitResponse || hitResponder is CurrencyObjectBase))
				{
					flag = true;
				}
			}
			if (onlyDamageEnemies && !healthManager && !flag)
			{
				return false;
			}
			tempDamageStack.AddMultiplier(DamageMultiplier);
			float num3 = tempDamageStack.PopDamage();
			float[] array = damageMultPerHit;
			if (array != null && array.Length > 0)
			{
				if (hitCounts == null)
				{
					hitCounts = new Dictionary<GameObject, int>();
				}
				if (isFirstHit || !hitCounts.TryGetValue(target, out var value))
				{
					value = 0;
				}
				if (value >= damageMultPerHit.Length)
				{
					value = damageMultPerHit.Length - 1;
				}
				float num4 = damageMultPerHit[value];
				num3 *= num4;
				if (num4 > Mathf.Epsilon && num3 < 0.51f)
				{
					num3 = 0.51f;
				}
				hitCounts[target] = value + 1;
			}
			int num5 = Mathf.RoundToInt(num3);
			bool flag2 = num5 > 0 || canWeakHit;
			EnemyHitEffectsProfile.EffectsTypes hitEffectsType = ((!multiHitter) ? (minimalHitEffects ? EnemyHitEffectsProfile.EffectsTypes.Minimal : EnemyHitEffectsProfile.EffectsTypes.Full) : (isFirstHit ? multiHitFirstEffects : multiHitEffects));
			float num6;
			if (directionSourceOverride == DirectionSourceOverrides.AwayFromHero)
			{
				Transform transform = HeroController.instance.transform;
				Vector2 normalized = ((Vector2)base.transform.position - (Vector2)transform.position).normalized;
				num6 = Vector2.Angle(Vector2.right, normalized);
			}
			else
			{
				num6 = direction;
				if (flipDirectionIfXScaleNegative && base.transform.lossyScale.x < 0f)
				{
					num6 += 180f;
				}
				if (flipDirectionIfBehind && forwardVector != Vector2.zero)
				{
					Vector2 vector = base.transform.position;
					Vector2 vector2 = (Vector2)target.transform.position - vector;
					Vector2 vector3 = base.transform.TransformDirection(forwardVector);
					if (Vector2.Dot(vector2.normalized, vector3.normalized) < 0f)
					{
						num6 += 180f;
						num2 *= flippedDirMagnitude;
					}
				}
			}
			for (; num6 < 0f; num6 += 360f)
			{
			}
			while (num6 >= 360f)
			{
				num6 -= 360f;
			}
			ToolItem toolItem = (representingTool ? representingTool : ((!NailImbuement) ? null : NailImbuement.ToolSource));
			HitInstance hitInstance = default(HitInstance);
			hitInstance.Source = base.gameObject;
			hitInstance.IsFirstHit = isFirstHit;
			hitInstance.AttackType = attackType;
			hitInstance.NailElement = (NailImbuement ? NailElement : NailElements.None);
			hitInstance.NailImbuement = NailImbuement;
			hitInstance.IsUsingNeedleDamageMult = useNailDamage || useHeroDamageAffectors;
			hitInstance.RepresentingTool = representingTool;
			hitInstance.PoisonDamageTicks = PoisonDamageTicks;
			hitInstance.ZapDamageTicks = ZapDamageTicks;
			hitInstance.DamageScalingLevel = damageScalingLevel;
			hitInstance.ToolDamageFlags = (toolItem ? toolItem.DamageFlags : damageFlags);
			hitInstance.CircleDirection = directionSourceOverride == DirectionSourceOverrides.CircleDirection;
			hitInstance.DamageDealt = num5;
			hitInstance.StunDamage = (doesNotStun ? 0f : stunDamage);
			hitInstance.CanWeakHit = canWeakHit;
			hitInstance.Direction = num6;
			hitInstance.UseCorpseDirection = corpseDirection.IsEnabled;
			hitInstance.CorpseDirection = corpseDirection.Value;
			hitInstance.CanTriggerBouncePod = canTriggerBouncePod;
			hitInstance.ExtraUpDirection = ExtraUpDirection;
			hitInstance.IgnoreInvulnerable = ignoreInvuln;
			hitInstance.MagnitudeMultiplier = num2;
			hitInstance.UseCorpseMagnitudeMult = corpseMagnitudeMult.IsEnabled;
			hitInstance.CorpseMagnitudeMultiplier = corpseMagnitudeMult.Value;
			hitInstance.UseCurrencyMagnitudeMult = currencyMagnitudeMult.IsEnabled;
			hitInstance.CurrencyMagnitudeMult = currencyMagnitudeMult.Value;
			hitInstance.MoveAngle = 0f;
			hitInstance.MoveDirection = moveDirection;
			hitInstance.Multiplier = 1f;
			hitInstance.SpecialType = specialType;
			hitInstance.SlashEffectOverrides = ((slashEffectOverrides.Length != 0) ? slashEffectOverrides : null);
			hitInstance.HitEffectsType = hitEffectsType;
			hitInstance.SilkGeneration = silkGeneration;
			hitInstance.NonLethal = nonLethal;
			hitInstance.RageHit = rageHit;
			hitInstance.CriticalHit = criticalHit;
			hitInstance.HunterCombo = hunterCombo;
			hitInstance.UseBouncePodDirection = useBouncePodDirection;
			hitInstance.BouncePodDirection = bouncePodDirection;
			hitInstance.IsManualTrigger = manualTrigger;
			hitInstance.IsHeroDamage = isHeroDamage || sourceIsHero;
			hitInstance.IsNailTag = isNailAttack;
			hitInstance.IgnoreNailPosition = IgnoreNailPosition;
			HitInstance hitInstance2 = hitInstance;
			bool flag3 = false;
			PhysLayers layer;
			if (healthManager != null)
			{
				layer = (PhysLayers)healthManager.gameObject.layer;
				flag3 = true;
				lastTargetDamaged = target;
				if (flag2)
				{
					FSMUtility.SendEventToGameObject(target, "TAKE DAMAGE");
					if (damageFSMEvent != "")
					{
						FSMUtility.SendEventToGameObject(target, damageFSMEvent);
					}
					if ((bool)dealtDamageFSM)
					{
						if (dealtDamageFSMEvent == "DASH HIT")
						{
							NonBouncer component = healthManager.GetComponent<NonBouncer>();
							bool flag4 = false;
							if ((bool)component)
							{
								flag4 = component.active;
							}
							if (!flag4)
							{
								dealtDamageFSM.SendEventSafe(dealtDamageFSMEvent);
							}
						}
						else
						{
							dealtDamageFSM.SendEventSafe(dealtDamageFSMEvent);
						}
					}
					if ((bool)dealtDamageFSM && !healthManager.GetComponent<NonBouncer>())
					{
						dealtDamageFSM.SendEventSafe(dealtDamageFSMEvent);
					}
					DealtDamage.Invoke();
					this.WillDamageEnemy?.Invoke();
					this.WillDamageEnemyOptions?.Invoke(healthManager, hitInstance2);
				}
			}
			else
			{
				layer = (PhysLayers)target.layer;
				lastTargetDamaged = target;
				if ((bool)dealtDamageFSM && target.CompareTag("Recoiler") && (bool)target.GetComponent<IsCoralCrustWall>())
				{
					dealtDamageFSM.SendEventSafe(dealtDamageFSMEvent);
				}
			}
			if (!flag2)
			{
				isDoingDamage = false;
				if (!base.enabled || !base.gameObject.activeInHierarchy)
				{
					tempHitsResponded.Clear();
				}
				return flag3;
			}
			if (flag3 && (bool)dealtDamageFSM)
			{
				lastRecordedTarget = target;
				dealtDamageFSM.SendEventSafe(targetRecordedFSMEvent);
			}
			foreach (IHitResponder item in tempHitsResponded)
			{
				AddAndOrder(new HitResponse
				{
					Target = target,
					Responder = item,
					Hit = hitInstance2,
					HealthManager = healthManager,
					LayerOnHit = layer
				});
				flag3 = true;
			}
			isDoingDamage = false;
			tempHitsResponded.Clear();
			return flag3;
		}
		finally
		{
			NailElement = nailElement;
			NailImbuement = nailImbuement;
		}
	}

	private void AddAndOrder(HitResponse response)
	{
		int i = currentDamageBuffer.BinarySearch(response, orderComparer);
		if (i >= 0)
		{
			for (; i < currentDamageBuffer.Count && currentDamageBuffer[i].Responder.HitPriority == response.Responder.HitPriority; i++)
			{
			}
		}
		else
		{
			i = ~i;
		}
		currentDamageBuffer.Insert(i, response);
	}

	private void DoEnemyDamageNailImbuement(HealthManager healthManager, HitInstance hitInstance)
	{
		NailImbuementConfig nailImbuement = hitInstance.NailImbuement;
		if (!nailImbuement || !healthManager || !hitInstance.IsFirstHit)
		{
			return;
		}
		EnemyHitEffectsProfile inertHitEffect = nailImbuement.InertHitEffect;
		if ((bool)inertHitEffect)
		{
			inertHitEffect.SpawnEffects(healthManager.transform, healthManager.EffectOrigin, hitInstance, null);
		}
		HeroController instance = HeroController.instance;
		int randomValue = ((UnityEngine.Random.Range(0f, 1f) <= nailImbuement.LuckyHitChance * instance.GetLuckModifier()) ? nailImbuement.LuckyHitsToTag : nailImbuement.HitsToTag).GetRandomValue();
		if (randomValue > 1 && !healthManager.CheckNailImbuementHit(nailImbuement, randomValue - 1))
		{
			return;
		}
		EnemyHitEffectsProfile startHitEffect = nailImbuement.StartHitEffect;
		if ((bool)startHitEffect)
		{
			startHitEffect.SpawnEffects(healthManager.transform, healthManager.EffectOrigin, hitInstance, null);
		}
		DamageTag damageTag = nailImbuement.DamageTag;
		if ((bool)damageTag)
		{
			healthManager.AddDamageTagToStack(damageTag, nailImbuement.DamageTagTicksOverride);
			return;
		}
		NailImbuementConfig.ImbuedLagHitOptions lagHits = nailImbuement.LagHits;
		if (lagHits != null)
		{
			healthManager.DoLagHits(lagHits, hitInstance);
		}
	}

	public void DoDamageFSM(GameObject target)
	{
		DoDamage(target);
	}

	public int GetDamageDealt()
	{
		return damageDealt;
	}

	public float GetDirection()
	{
		float num = direction;
		if (flipDirectionIfXScaleNegative && base.transform.lossyScale.x < 0f)
		{
			num += 180f;
		}
		return num;
	}

	public GameObject GetLastTargetDamaged()
	{
		return lastTargetDamaged;
	}

	public GameObject GetLastRecordedTarget()
	{
		if (!lastRecordedTarget)
		{
			return lastTargetDamaged;
		}
		return lastRecordedTarget;
	}

	public bool GetDoesNotTink()
	{
		return doesNotTink;
	}

	public void SetDirection(float newDirection)
	{
		direction = newDirection;
	}

	public void SetDirectionByHeroFacing()
	{
		HeroController instance = HeroController.instance;
		direction = ((!instance.cState.facingRight) ? 180 : 0);
	}

	public void FlipDirection()
	{
		direction += 180f;
		if (direction > 360f)
		{
			direction -= 360f;
		}
	}

	public void StartDamage()
	{
		if (hasSharedDamageGroup)
		{
			sharedDamagedGroup.DamageStart(this);
		}
		stepsToNextHit = 0;
		hitsLeftUntilDeath = (HandlesMultiHitsDeath() ? hitsUntilDeath : 0);
		DidHit = false;
		DidHitEnemy = false;
		endedDamage = false;
		damagePrevented.Clear();
		frameQueue.Clear();
	}

	private void ClearDamageBuffer()
	{
		currentDamageBuffer.Clear();
	}

	public void EndDamage()
	{
		if (hasSharedDamageGroup)
		{
			sharedDamagedGroup.DamageEnd(this);
		}
		enteredColliders.Clear();
		damagedColliders.Clear();
		hitCounts?.Clear();
		stepsToNextHit = 0;
		hitsLeftUntilDeath = 0;
		if (!endedDamage)
		{
			endedDamage = true;
			this.EndedDamage?.Invoke(DidHit);
		}
		DidHit = false;
		DidHitEnemy = false;
	}

	public bool PreventDamage(Collider2D col)
	{
		if (hasSharedDamageGroup)
		{
			return sharedDamagedGroup.PreventDamage(col);
		}
		return damagedColliders.Add(col);
	}

	public void PreventDamage(IHitResponder hitResponder)
	{
		if (hasSharedDamageGroup)
		{
			sharedDamagedGroup.PreventDamage(hitResponder);
		}
		damagePrevented.Add(hitResponder);
	}

	public void ClearPreventDamage()
	{
		if (hasSharedDamageGroup)
		{
			sharedDamagedGroup.ClearDamagePrevented();
		}
		damagePrevented.Clear();
	}

	public bool HasBeenDamaged(IHitResponder hitResponder)
	{
		if (hasSharedDamageGroup)
		{
			return sharedDamagedGroup.HasDamaged(hitResponder);
		}
		if (!hitsResponded.Contains(hitResponder))
		{
			return damagePrevented.Contains(hitResponder);
		}
		return true;
	}

	public bool HasResponded(IHitResponder hitResponder)
	{
		return hitsResponded.Contains(hitResponder);
	}

	public void TryClearRespondedList()
	{
		if (lastRespondedCycle != CustomPlayerLoop.FixedUpdateCycle)
		{
			hitsResponded.Clear();
		}
	}

	public void SendParried()
	{
		SendParried(bouncable: true);
	}

	public void SendParried(bool bouncable)
	{
		this.ParriedEnemy?.Invoke();
		if (bouncable)
		{
			OnBounceableTink();
		}
	}

	public void OnTinkEffectTink()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "DAMAGER TINKED");
		Tinked.Invoke();
	}

	public void OnBounceableTink()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED");
	}

	public void OnBounceableTinkDown()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED DOWN");
	}

	public void OnBounceableTinkUp()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED UP");
	}

	public void OnBounceableTinkRight()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED RIGHT");
	}

	public void OnBounceableTinkLeft()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED LEFT");
	}

	public void OnHitSpikes()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "DAMAGER HIT SPIKES");
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject);
	}

	public void CopyDamagePropsFrom(DamageEnemies otherDamager)
	{
		attackType = otherDamager.attackType;
		useNailDamage = otherDamager.useNailDamage;
		nailDamageMultiplier = otherDamager.nailDamageMultiplier;
		damageDealt = otherDamager.damageDealt;
		damageAsset = otherDamager.damageAsset;
		useHeroDamageAffectors = otherDamager.useHeroDamageAffectors;
		canWeakHit = otherDamager.canWeakHit;
	}

	public void OverridePoisonDamage(int value)
	{
		isPoisonDamageOverridden = true;
		poisonDamageTicks = value;
	}
}
