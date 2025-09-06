using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class HealthManager : MonoBehaviour, IHitResponder, ITagDamageTakerOwner, IInitialisable
{
	[Serializable]
	private class ItemDropProbability : Probability.ProbabilityBase<SavedItem>
	{
		[SerializeField]
		private SavedItem item;

		public MinMaxInt Amount = new MinMaxInt(1, 1);

		public CollectableItemPickup CustomPickupPrefab;

		public int LimitActiveInScene;

		public override SavedItem Item => item;
	}

	[Serializable]
	private class ItemDropGroup
	{
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		public List<ItemDropProbability> Drops;
	}

	[Serializable]
	private class DamageScalingConfig
	{
		[FormerlySerializedAs("Upg1Mult")]
		public float Level1Mult = 1f;

		[FormerlySerializedAs("Upg2Mult")]
		public float Level2Mult = 1f;

		[FormerlySerializedAs("Upg3Mult")]
		public float Level3Mult = 1f;

		[FormerlySerializedAs("Upg4Mult")]
		public float Level4Mult = 1f;

		[FormerlySerializedAs("Upg5Mult")]
		public float Level5Mult = 1f;

		public float GetMultFromLevel(int level)
		{
			if (level >= 0)
			{
				return level switch
				{
					0 => Level1Mult, 
					1 => Level2Mult, 
					2 => Level3Mult, 
					3 => Level4Mult, 
					_ => Level5Mult, 
				};
			}
			return 1f;
		}
	}

	[Serializable]
	public enum EnemyTypes
	{
		Regular = 0,
		Shade = 3,
		Armoured = 6
	}

	public enum ReaperBundleTiers
	{
		Normal = 0,
		Reduced = 1,
		None = 2
	}

	public enum EnemySize
	{
		Small = 0,
		Regular = 1,
		Large = 2
	}

	[Flags]
	public enum IgnoreFlags
	{
		None = 0,
		RageHeal = 1,
		WitchHeal = 2
	}

	private class LagHitsTracker
	{
		public GameObject Source;

		public Coroutine LagHitsRoutine;

		public Action OnLagHitsEnd;
	}

	public delegate void DeathEvent();

	public struct QueuedDropItem
	{
		public bool isQueued;

		public CollectableItemToolDamage dropItem;

		public int amount;

		public QueuedDropItem(CollectableItemToolDamage dropItem, int amount)
		{
			this = default(QueuedDropItem);
			isQueued = true;
			this.dropItem = dropItem;
			this.amount = amount;
		}

		public void Reset()
		{
			isQueued = false;
			dropItem = null;
		}
	}

	private class StealLagHit : LagHitOptions
	{
		private readonly HealthManager healthManager;

		private readonly int stolenGeo;

		private readonly int flingGeo;

		private readonly int stolenShards;

		private readonly int flingShards;

		public StealLagHit(HealthManager healthManager, int stolenGeo, int flingGeo, int stolenShards, int flingShards)
		{
			this.healthManager = healthManager;
			this.stolenGeo = stolenGeo;
			this.flingGeo = flingGeo;
			this.stolenShards = stolenShards;
			this.flingShards = flingShards;
			UseNailDamage = true;
			NailDamageMultiplier = 0.25f;
			DamageType = LagHitDamageType.Slash;
			MagnitudeMult = 0.3f;
			StartDelay = 0.2f;
			HitCount = 1;
		}

		public override void OnEnd(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
		{
			Transform transform = healthManager.transform;
			Gameplay.ThiefSnatchEffectPrefab.Spawn(transform.position).Setup(transform, stolenGeo > 0, stolenShards > 0);
			if (flingGeo > 0)
			{
				healthManager.FlingStealCurrency(flingGeo, Gameplay.SmallGeoPrefab);
			}
			if (stolenGeo > 0)
			{
				CurrencyManager.AddGeo(stolenGeo);
			}
			if (stolenShards > 0)
			{
				CurrencyManager.AddShards(stolenShards);
			}
			if (flingShards > 0)
			{
				healthManager.FlingStealCurrency(flingShards, Gameplay.ShellShardPrefab);
			}
		}
	}

	private BoxCollider2D boxCollider;

	private Recoil recoil;

	private IHitEffectReciever hitEffectReceiver;

	private EnemyDeathEffects enemyDeathEffects;

	private tk2dSpriteAnimator animator;

	private DamageHero damageHero;

	private TagDamageTaker tagDamageTaker;

	private Dictionary<NailImbuementConfig, int> damageTagHitsLeftTracker;

	[Header("Assets")]
	[SerializeField]
	private AudioSource audioPlayerPrefab;

	[SerializeField]
	private AudioEvent regularInvincibleAudio;

	[SerializeField]
	private GameObject blockHitPrefab;

	[SerializeField]
	private GameObject strikeNailPrefab;

	[SerializeField]
	private GameObject slashImpactPrefab;

	[SerializeField]
	private GameObject corpseSplatPrefab;

	[Header("Body")]
	[SerializeField]
	public int hp;

	[SerializeField]
	private DamageScalingConfig damageScaling;

	[SerializeField]
	private EnemyTypes enemyType;

	[SerializeField]
	private bool doNotGiveSilk;

	[SerializeField]
	private IgnoreFlags ignoreFlags;

	[SerializeField]
	private ReaperBundleTiers reaperBundles;

	[SerializeField]
	private Vector3 effectOrigin;

	[SerializeField]
	private bool ignoreKillAll;

	[SerializeField]
	private HealthManager sendDamageTo;

	[SerializeField]
	private bool isPartOfSendToTarget;

	[SerializeField]
	private bool tagDamageTakerIgnoreColliderState;

	[SerializeField]
	private bool takeTagDamageWhileInvincible;

	[SerializeField]
	private Transform targetPointOverride;

	private bool hasTargetPointOverride;

	[Header("Scene")]
	[SerializeField]
	private GameObject battleScene;

	[SerializeField]
	private GameObject sendHitTo;

	[SerializeField]
	private GameObject sendKilledToObject;

	[SerializeField]
	private string sendKilledToName;

	[Header("Drops")]
	[SerializeField]
	private int smallGeoDrops;

	[SerializeField]
	private int mediumGeoDrops;

	[SerializeField]
	private int largeGeoDrops;

	[SerializeField]
	private int largeSmoothGeoDrops;

	[SerializeField]
	private bool megaFlingGeo;

	[Space]
	[SerializeField]
	private int shellShardDrops;

	[Space]
	[SerializeField]
	private bool flingSilkOrbsDown;

	[SerializeField]
	private GameObject flingSilkOrbsAimObject;

	[Space]
	[SerializeField]
	private List<ItemDropGroup> itemDropGroups;

	[SerializeField]
	[Range(0f, 1f)]
	[FormerlySerializedAs("itemDropProbability")]
	[HideInInspector]
	private float _itemDropProbability;

	[SerializeField]
	[FormerlySerializedAs("itemDrops")]
	[HideInInspector]
	private ItemDropProbability[] _itemDrops;

	[Header("Hit")]
	[SerializeField]
	private bool hasAlternateHitAnimation;

	[SerializeField]
	private string alternateHitAnimation;

	[Header("Invincible")]
	[SerializeField]
	private bool invincible;

	[SerializeField]
	private bool piercable;

	[SerializeField]
	private int invincibleFromDirection;

	[SerializeField]
	private bool preventInvincibleEffect;

	[SerializeField]
	private bool preventInvincibleShake;

	[SerializeField]
	private bool preventInvincibleAttackBlock;

	[SerializeField]
	private bool invincibleRecoil;

	[SerializeField]
	private bool dontSendTinkToDamager;

	[SerializeField]
	private bool hasAlternateInvincibleSound;

	[SerializeField]
	private AudioClip alternateInvincibleSound;

	[SerializeField]
	private bool immuneToNailAttacks;

	[SerializeField]
	private bool immuneToExplosions;

	[SerializeField]
	private bool immuneToBeams;

	[SerializeField]
	private bool immuneToHunterWeapon;

	[SerializeField]
	private bool immuneToCoal;

	[SerializeField]
	private bool immuneToTraps;

	[SerializeField]
	private bool immuneToWater;

	[SerializeField]
	private bool immuneToSpikes;

	[SerializeField]
	private bool immuneToLava;

	[SerializeField]
	private bool isMossExtractable;

	[SerializeField]
	private bool isSwampExtractable;

	[SerializeField]
	private bool isBluebloodExtractable;

	[Header("Death")]
	[SerializeField]
	private AudioMixerSnapshot deathAudioSnapshot;

	[SerializeField]
	public bool hasSpecialDeath;

	[SerializeField]
	public bool deathReset;

	[SerializeField]
	public bool damageOverride;

	[SerializeField]
	private bool ignoreAcid;

	[SerializeField]
	private bool ignoreWater;

	[SerializeField]
	private GameObject zeroHPEventOverride;

	[SerializeField]
	private bool dontDropMeat;

	[SerializeField]
	private EnemySize enemySize = EnemySize.Regular;

	[SerializeField]
	private bool bigEnemyDeath;

	[SerializeField]
	private bool preventDeathAfterHero;

	private EventRelayResponder corpseEventResponder;

	[Header("Deprecated/Unusued Variables")]
	[SerializeField]
	private bool ignoreHazards;

	[SerializeField]
	private float invulnerableTime;

	[SerializeField]
	private bool semiPersistent;

	public bool isDead;

	public bool ignorePersistence;

	private GameObject sendKilledTo;

	private float evasionByHitRemaining;

	private HitInstance lastHitInstance;

	private int directionOfLastAttack;

	private int initHp;

	private bool hasTakenDamage;

	private AttackTypes lastAttackType;

	private const float rapidBulletTime = 0.15f;

	private float rapidBulletTimer;

	private int rapidBulletCount;

	private const float rapidBombTime = 0.75f;

	private float rapidBombTimer;

	private int rapidBombCount;

	public float tinkTimer;

	public const float timeBetweenTinks = 0.1f;

	private static readonly float[] _shellShardMultiplierArray = new float[8] { 1f, 1f, 1f, 1.25f, 1.25f, 1.25f, 1.5f, 1.5f };

	private PlayMakerFSM stunControlFsm;

	private bool notifiedBattleScene;

	private readonly List<LagHitsTracker> runningLagHits = new List<LagHitsTracker>();

	private readonly Dictionary<LagHitDamageType, LagHitsTracker> runningSingleLagHits = new Dictionary<LagHitDamageType, LagHitsTracker>();

	private readonly List<GameObject> spawnedFlingTracker = new List<GameObject>();

	private static readonly List<HealthManager> _activeHealthManagers = new List<HealthManager>();

	private bool addedPhysicalPusher;

	private GameObject physicalPusher;

	private bool hasBlackThreadState;

	private BlackThreadState blackThreadState;

	private QueuedDropItem queuedDropItem;

	private bool hasAwaken;

	private bool hasStarted;

	public HealthManager SendDamageTo => sendDamageTo;

	public bool IsPartOfSendToTarget => isPartOfSendToTarget;

	public Vector3 EffectOrigin => effectOrigin;

	public EnemyTypes EnemyType
	{
		get
		{
			return enemyType;
		}
		set
		{
			enemyType = value;
		}
	}

	public bool DoNotGiveSilk => doNotGiveSilk;

	public Vector3 TargetPoint
	{
		get
		{
			if (hasTargetPointOverride)
			{
				return targetPointOverride.position;
			}
			return base.transform.position;
		}
	}

	public bool MegaFlingGeo => megaFlingGeo;

	public bool PreventInvincibleEffect => preventInvincibleEffect;

	public bool IsInvincible
	{
		get
		{
			return invincible;
		}
		set
		{
			invincible = value;
		}
	}

	public int InvincibleFromDirection
	{
		get
		{
			return invincibleFromDirection;
		}
		set
		{
			invincibleFromDirection = value;
		}
	}

	public bool ImmuneToCoal => immuneToCoal;

	public bool HasClearedItemDrops { get; private set; }

	public bool WillAwardJournalKill { get; private set; }

	public SpriteFlash SpriteFlash => GetComponent<SpriteFlash>();

	public Vector2 TagDamageEffectPos => EffectOrigin;

	Transform ITagDamageTakerOwner.transform => base.transform;

	GameObject IInitialisable.gameObject => base.gameObject;

	public event DeathEvent OnDeath;

	public event Action StartedDead;

	public event Action TookDamage;

	private void OnValidate()
	{
		if ((itemDropGroups == null || itemDropGroups.Count == 0) && (_itemDropProbability > 0f || (_itemDrops != null && _itemDrops.Length != 0)))
		{
			itemDropGroups = new List<ItemDropGroup>
			{
				new ItemDropGroup
				{
					TotalProbability = _itemDropProbability,
					Drops = new List<ItemDropProbability>(_itemDrops)
				}
			};
			_itemDropProbability = 0f;
			_itemDrops = null;
		}
		hasTargetPointOverride = targetPointOverride != null;
	}

	protected void Awake()
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
		OnValidate();
		corpseEventResponder = base.gameObject.GetComponent<EventRelayResponder>();
		boxCollider = GetComponent<BoxCollider2D>();
		recoil = GetComponent<Recoil>();
		hitEffectReceiver = GetComponent<IHitEffectReciever>();
		enemyDeathEffects = GetComponent<EnemyDeathEffects>();
		animator = GetComponent<tk2dSpriteAnimator>();
		damageHero = GetComponent<DamageHero>();
		tagDamageTaker = TagDamageTaker.Add(base.gameObject, this);
		tagDamageTaker.SetIgnoreColliderState(tagDamageTakerIgnoreColliderState);
		blackThreadState = GetComponent<BlackThreadState>();
		hasBlackThreadState = blackThreadState != null;
		initHp = hp;
		HealToMax();
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if (playMakerFSM.FsmName == "Stun Control" || playMakerFSM.FsmName == "Stun")
			{
				stunControlFsm = playMakerFSM;
				break;
			}
		}
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				if (ignorePersistence)
				{
					val = false;
				}
				else
				{
					val = isDead;
				}
			};
			if (component.LoadedValue && component.ItemData.Value)
			{
				OnSaveState(val: true);
			}
			component.OnSetSaveState += OnSaveState;
		}
		return true;
		void OnSaveState(bool val)
		{
			if (!ignorePersistence && val)
			{
				isDead = true;
				this.StartedDead?.Invoke();
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		evasionByHitRemaining = -1f;
		if (!string.IsNullOrEmpty(sendKilledToName))
		{
			sendKilledTo = GameObject.Find(sendKilledToName);
		}
		else if (sendKilledToObject != null)
		{
			sendKilledTo = sendKilledToObject;
		}
		AddPhysicalPusher();
		foreach (ItemDropGroup itemDropGroup in itemDropGroups)
		{
			if (itemDropGroup.Drops.Count <= 0 || itemDropGroup.TotalProbability < 1f)
			{
				continue;
			}
			foreach (ItemDropProbability drop in itemDropGroup.Drops)
			{
				if ((bool)drop.CustomPickupPrefab && drop.LimitActiveInScene > 0)
				{
					PersonalObjectPool.EnsurePooledInScene(base.gameObject, drop.CustomPickupPrefab.gameObject, drop.LimitActiveInScene, finished: false);
				}
			}
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		return true;
	}

	protected void OnEnable()
	{
		_activeHealthManagers.AddIfNotPresent(this);
		hasTargetPointOverride = targetPointOverride != null;
		StartCoroutine(CheckPersistence());
	}

	private void OnDisable()
	{
		_activeHealthManagers.Remove(this);
		queuedDropItem.Reset();
	}

	public static IEnumerable<HealthManager> EnumerateActiveEnemies()
	{
		return _activeHealthManagers;
	}

	protected void Start()
	{
		OnStart();
	}

	protected IEnumerator CheckPersistence()
	{
		yield return null;
		if (isDead)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	protected void Update()
	{
		if (!isDead && base.transform.position.y < -400f)
		{
			Die(null, AttackTypes.Generic, NailElements.None, null);
		}
		evasionByHitRemaining -= Time.deltaTime;
		if (rapidBulletTimer > 0f)
		{
			rapidBulletTimer -= Time.deltaTime;
			if (rapidBulletTimer <= 0f)
			{
				rapidBulletCount = 0;
			}
		}
		if (rapidBombTimer > 0f)
		{
			rapidBombTimer -= Time.deltaTime;
			if (rapidBombTimer <= 0f)
			{
				rapidBombCount = 0;
			}
		}
		if (tinkTimer > 0f)
		{
			tinkTimer -= Time.deltaTime;
		}
		tagDamageTaker.Tick(takeTagDamageWhileInvincible || !IsInvincible || piercable || invincibleFromDirection != 0);
	}

	public IHitResponder.HitResponse Hit(HitInstance hitInstance)
	{
		if (isDead)
		{
			return IHitResponder.Response.None;
		}
		if (evasionByHitRemaining > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit && hitInstance.DamageDealt <= 0 && !hitInstance.CanWeakHit)
		{
			return IHitResponder.Response.None;
		}
		FSMUtility.SendEventToGameObject(hitInstance.Source, "DEALT DAMAGE");
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		if (IsBlockingByDirection(cardinalDirection, hitInstance.AttackType, hitInstance.SpecialType))
		{
			Invincible(hitInstance);
			return IHitResponder.Response.Invincible;
		}
		TakeDamage(hitInstance);
		return IHitResponder.Response.DamageEnemy;
	}

	private void Invincible(HitInstance hitInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		lastHitInstance = hitInstance;
		directionOfLastAttack = cardinalDirection;
		lastAttackType = hitInstance.AttackType;
		DamageEnemies component = hitInstance.Source.GetComponent<DamageEnemies>();
		if ((bool)component && !dontSendTinkToDamager)
		{
			component.OnTinkEffectTink();
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT");
		NonBouncer component2 = GetComponent<NonBouncer>();
		bool flag = false;
		if ((bool)component2)
		{
			flag = component2.active;
		}
		if (!flag)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED");
		}
		if (!preventInvincibleAttackBlock)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "ATTACK BLOCKED");
		}
		if (invincibleRecoil && recoil != null)
		{
			recoil.RecoilByDirection(cardinalDirection, hitInstance.MagnitudeMultiplier);
		}
		if (!(GetComponent<DontClinkGates>() != null) && tinkTimer <= 0f)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT");
			if (!PreventInvincibleEffect)
			{
				if (!preventInvincibleShake && hitInstance.AttackType == AttackTypes.Nail)
				{
					switch (cardinalDirection)
					{
					case 0:
						HeroController.instance.RecoilLeft();
						break;
					case 2:
						HeroController.instance.RecoilRight();
						break;
					}
				}
				if (!preventInvincibleShake)
				{
					if (hitInstance.IsNailTag)
					{
						Effects.BlockedHitShake.DoShake(this);
					}
					else
					{
						Effects.BlockedHitShakeNoFreeze.DoShake(this);
					}
				}
				BoxCollider2D component3 = boxCollider;
				if (component3 == null)
				{
					component3 = GetComponent<BoxCollider2D>();
				}
				Vector2 vector;
				Vector3 eulerAngles;
				if (component3 != null)
				{
					switch (cardinalDirection)
					{
					case 0:
						vector = new Vector2(base.transform.GetPositionX() + component3.offset.x - component3.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
						eulerAngles = new Vector3(0f, 0f, 0f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT R");
						break;
					case 2:
						vector = new Vector2(base.transform.GetPositionX() + component3.offset.x + component3.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
						eulerAngles = new Vector3(0f, 0f, 180f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT L");
						break;
					case 1:
						vector = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Max(hitInstance.Source.transform.GetPositionY(), base.transform.GetPositionY() + component3.offset.y - component3.size.y * 0.5f));
						eulerAngles = new Vector3(0f, 0f, 90f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT U");
						break;
					case 3:
					{
						float positionX = hitInstance.Source.transform.GetPositionX();
						float positionY = hitInstance.Source.transform.GetPositionY();
						float num = base.transform.GetPositionX() + component3.offset.x - component3.size.x * 0.5f;
						float num2 = base.transform.GetPositionX() + component3.offset.x + component3.size.x * 0.5f;
						float num3 = base.transform.GetPositionY() + component3.offset.y + component3.size.y * 0.5f;
						float x = ((positionX < num) ? num : ((!(positionX > num2)) ? positionX : num2));
						float y = ((!(positionY > num3)) ? positionY : num3);
						vector = new Vector2(x, y);
						eulerAngles = new Vector3(0f, 0f, 270f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED DOWN");
						break;
					}
					default:
						vector = base.transform.position;
						eulerAngles = new Vector3(0f, 0f, 0f);
						break;
					}
				}
				else
				{
					vector = base.transform.position;
					eulerAngles = new Vector3(0f, 0f, 0f);
				}
				GameObject obj = blockHitPrefab.Spawn();
				obj.transform.position = vector;
				obj.transform.eulerAngles = eulerAngles;
				if (hasAlternateInvincibleSound)
				{
					AudioSource component4 = GetComponent<AudioSource>();
					if (alternateInvincibleSound != null && component4 != null)
					{
						component4.PlayOneShot(alternateInvincibleSound);
					}
				}
				else
				{
					regularInvincibleAudio.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
				}
			}
			tinkTimer = 0.1f;
		}
		evasionByHitRemaining = 0f;
	}

	private HitInstance ApplyDamageScaling(HitInstance hitInstance)
	{
		int level = (hitInstance.IsUsingNeedleDamageMult ? PlayerData.instance.nailUpgrades : ((!hitInstance.RepresentingTool || hitInstance.RepresentingTool.Type == ToolItemType.Skill) ? (hitInstance.DamageScalingLevel - 1) : PlayerData.instance.ToolKitUpgrades));
		float multFromLevel = damageScaling.GetMultFromLevel(level);
		hitInstance.DamageDealt = Mathf.RoundToInt((float)hitInstance.DamageDealt * multFromLevel);
		return hitInstance;
	}

	private void TakeDamage(HitInstance hitInstance)
	{
		if (hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full && hasBlackThreadState && blackThreadState.IsVisiblyThreaded)
		{
			hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.Minimal;
		}
		if (IsImmuneTo(hitInstance, wasFullHit: true))
		{
			return;
		}
		DamageEnemies component = hitInstance.Source.GetComponent<DamageEnemies>();
		if ((bool)component && component.AwardJournalKill)
		{
			WillAwardJournalKill = true;
		}
		hitInstance = ApplyDamageScaling(hitInstance);
		if ((hitInstance.SpecialType & SpecialTypes.RapidBullet) != 0)
		{
			rapidBulletTimer = 0.15f;
			rapidBulletCount++;
			if (rapidBulletCount > 1)
			{
				hitInstance.DamageDealt /= rapidBulletCount;
				if (hitInstance.DamageDealt < 1)
				{
					hitInstance.DamageDealt = 1;
				}
			}
		}
		if ((hitInstance.SpecialType & SpecialTypes.RapidBomb) != 0)
		{
			rapidBombTimer = 0.75f;
			rapidBombCount++;
			if (rapidBombCount > 1)
			{
				Debug.Log("Rapid Bomb trying to deal " + hitInstance.DamageDealt + " damage. Getting divided by " + rapidBombCount + "...");
				hitInstance.DamageDealt /= rapidBombCount;
				if (hitInstance.DamageDealt < 1)
				{
					hitInstance.DamageDealt = 1;
				}
				Debug.Log("New rapid bomb damage: " + hitInstance.DamageDealt);
			}
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && isMossExtractable && QuestManager.GetQuest("Extractor Green").IsAccepted && !QuestManager.GetQuest("Extractor Green").IsCompleted)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT");
			EventRegister.SendEvent(EventRegisterEvents.StartExtractM);
			return;
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && isSwampExtractable && QuestManager.GetQuest("Extractor Swamp").IsAccepted && !QuestManager.GetQuest("Extractor Swamp").IsCompleted)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT");
			EventRegister.SendEvent(EventRegisterEvents.StartExtractSwamp);
			return;
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && isBluebloodExtractable)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT");
			EventRegister.SendEvent(EventRegisterEvents.StartExtractBlueblood);
			return;
		}
		if (hitInstance.CriticalHit)
		{
			hitInstance.DamageDealt = Mathf.RoundToInt((float)hitInstance.DamageDealt * Gameplay.WandererCritMultiplier);
			hitInstance.MagnitudeMultiplier *= Gameplay.WandererCritMagnitudeMult;
			GameObject wandererCritEffect = Gameplay.WandererCritEffect;
			if ((bool)wandererCritEffect)
			{
				wandererCritEffect.Spawn(base.transform.TransformPoint(effectOrigin)).transform.SetRotation2D(hitInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
			}
			GameManager.instance.FreezeMoment(FreezeMomentTypes.HeroDamageShort);
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		if (hitInstance.HunterCombo)
		{
			GameObject hunterComboDamageEffect = Gameplay.HunterComboDamageEffect;
			if ((bool)hunterComboDamageEffect)
			{
				GameObject gameObject = hunterComboDamageEffect.Spawn(base.transform.TransformPoint(effectOrigin));
				switch (cardinalDirection)
				{
				case 2:
					gameObject.transform.SetRotation2D(180f);
					break;
				case 0:
					gameObject.transform.SetRotation2D(0f);
					break;
				case 1:
				case 3:
					gameObject.transform.SetRotation2D(90f);
					break;
				}
				Vector3 localScale = hunterComboDamageEffect.transform.localScale;
				if (Gameplay.HunterCrest3.IsEquipped && HeroController.instance.HunterUpgState.IsComboMeterAboveExtra)
				{
					Vector2 hunterCombo2DamageExtraScale = Gameplay.HunterCombo2DamageExtraScale;
					localScale.x *= hunterCombo2DamageExtraScale.x;
					localScale.y *= hunterCombo2DamageExtraScale.y;
				}
				gameObject.transform.localScale = localScale;
			}
		}
		lastHitInstance = hitInstance;
		directionOfLastAttack = cardinalDirection;
		lastAttackType = hitInstance.AttackType;
		bool flag = hitInstance.DamageDealt <= 0 && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit;
		if (hitInstance.AttackType == AttackTypes.Heavy)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK HEAVY DAMAGE");
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "HIT");
		NonBouncer component2 = GetComponent<NonBouncer>();
		bool flag2 = false;
		if ((bool)component2)
		{
			flag2 = component2.active;
		}
		if (!flag2)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED");
			FSMUtility.SendEventToGameObject(hitInstance.Source, "DEALT ACTUAL DAMAGE");
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK DAMAGE");
		if (!hasBlackThreadState || !blackThreadState.IsInForcedSing)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "SING DURATION END");
		}
		if (sendHitTo != null)
		{
			FSMUtility.SendEventToGameObject(sendHitTo, "HIT");
		}
		if (flag)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "WEAK HIT");
		}
		if (hitInstance.AttackType == AttackTypes.Spell)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK SPELL DAMAGE");
		}
		if (hitInstance.AttackType == AttackTypes.Explosion)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK EXPLOSION DAMAGE");
		}
		if (recoil != null)
		{
			recoil.RecoilByDirection(cardinalDirection, hitInstance.MagnitudeMultiplier);
		}
		bool flag3 = false;
		bool flag4 = false;
		switch (hitInstance.AttackType)
		{
		case AttackTypes.Heavy:
			if ((bool)hitInstance.Source && DamageEnemies.IsNailAttackObject(hitInstance.Source))
			{
				goto case AttackTypes.Nail;
			}
			goto case AttackTypes.Spell;
		case AttackTypes.Nail:
		case AttackTypes.NailBeam:
			HeroController.instance.NailHitEnemy(this, hitInstance);
			goto case AttackTypes.Spell;
		case AttackTypes.Spell:
			if (hitInstance.AttackType == AttackTypes.Nail && enemyType != EnemyTypes.Shade && enemyType != EnemyTypes.Armoured && !DoNotGiveSilk)
			{
				if (!flag)
				{
					HeroController.instance.SilkGain(hitInstance);
				}
				if (hitInstance.SilkGeneration != HitSilkGeneration.None && PlayerData.instance.CurrentCrestID == "Reaper" && HeroController.instance.ReaperState.IsInReaperMode)
				{
					float angleMin = 0f;
					float angleMax = 360f;
					int num = reaperBundles switch
					{
						ReaperBundleTiers.Normal => HeroController.instance.GetReaperPayout(), 
						ReaperBundleTiers.Reduced => 1, 
						ReaperBundleTiers.None => 0, 
						_ => throw new ArgumentOutOfRangeException(), 
					};
					if (num > 0)
					{
						float degrees;
						if (flingSilkOrbsAimObject != null)
						{
							Vector3 position = flingSilkOrbsAimObject.transform.position;
							Vector3 position2 = base.transform.position;
							float y = position.y - position2.y;
							float x = position.x - position2.x;
							float num2 = Mathf.Atan2(y, x) * (180f / MathF.PI);
							angleMin = num2 - 45f;
							angleMax = num2 + 45f;
							degrees = num2;
						}
						else if (flingSilkOrbsDown)
						{
							angleMin = 225f;
							angleMax = 315f;
							degrees = 270f;
						}
						else
						{
							switch (cardinalDirection)
							{
							case 2:
								angleMin = 125f;
								angleMax = 225f;
								degrees = 180f;
								break;
							case 0:
								angleMin = 315f;
								angleMax = 415f;
								degrees = 0f;
								break;
							case 1:
								angleMin = 45f;
								angleMax = 135f;
								degrees = 90f;
								break;
							case 3:
								angleMin = 225f;
								angleMax = 315f;
								degrees = 270f;
								break;
							default:
								degrees = 0f;
								break;
							}
						}
						FlingUtils.Config config = default(FlingUtils.Config);
						config.Prefab = Gameplay.ReaperBundlePrefab;
						config.AmountMin = num;
						config.AmountMax = num;
						config.SpeedMin = 25f;
						config.SpeedMax = 50f;
						config.AngleMin = angleMin;
						config.AngleMax = angleMax;
						FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
						GameObject reapHitEffectPrefab = Effects.ReapHitEffectPrefab;
						if ((bool)reapHitEffectPrefab)
						{
							Vector2 original;
							float rotation;
							switch (DirectionUtils.GetCardinalDirection(degrees))
							{
							case 1:
								original = new Vector2(1f, 1f);
								rotation = 90f;
								break;
							case 2:
								original = new Vector2(-1f, 1f);
								rotation = 0f;
								break;
							case 3:
								original = new Vector2(1f, -1f);
								rotation = 90f;
								break;
							default:
								original = new Vector2(1f, 1f);
								rotation = 0f;
								break;
							}
							GameObject gameObject2 = reapHitEffectPrefab.Spawn(base.transform.TransformPoint(effectOrigin));
							gameObject2.transform.SetRotation2D(rotation);
							gameObject2.transform.localScale = original.ToVector3(gameObject2.transform.localScale.z);
						}
					}
				}
			}
			if (!flag && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit)
			{
				flag3 = true;
			}
			flag4 = true;
			break;
		case AttackTypes.Generic:
			flag4 = true;
			break;
		}
		Vector3 position3 = (hitInstance.Source.transform.position + base.transform.position) * 0.5f + effectOrigin;
		if (flag4 && hitInstance.SlashEffectOverrides != null && hitInstance.SlashEffectOverrides.Length != 0)
		{
			GameObject[] array = hitInstance.SlashEffectOverrides.SpawnAll(base.transform.position);
			float num3 = Mathf.Sign(HeroController.instance.transform.localScale.x);
			GameObject[] array2 = array;
			foreach (GameObject gameObject3 in array2)
			{
				if (!gameObject3)
				{
					continue;
				}
				Vector3 initialScale = gameObject3.transform.localScale;
				Vector3 localScale2 = initialScale;
				localScale2.x *= num3;
				gameObject3.transform.localScale = localScale2;
				if (!gameObject3.GetComponent<ActiveRecycler>())
				{
					GameObject closureEffect = gameObject3;
					RecycleResetHandler.Add(gameObject3, (Action)delegate
					{
						closureEffect.transform.localScale = initialScale;
					});
				}
			}
			flag3 = false;
		}
		if (flag3 && (bool)slashImpactPrefab)
		{
			GameObject gameObject4 = slashImpactPrefab.Spawn(position3, Quaternion.identity);
			float num4 = 1.7f;
			switch (cardinalDirection)
			{
			case 2:
				gameObject4.transform.SetRotation2D(UnityEngine.Random.Range(340, 380));
				gameObject4.transform.localScale = new Vector3(0f - num4, num4, num4);
				break;
			case 0:
				gameObject4.transform.SetRotation2D(UnityEngine.Random.Range(340, 380));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			case 1:
				gameObject4.transform.SetRotation2D(UnityEngine.Random.Range(70, 110));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			case 3:
				gameObject4.transform.SetRotation2D(180f - hitInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			}
		}
		if (hitEffectReceiver != null && hitInstance.AttackType != AttackTypes.RuinsWater)
		{
			hitEffectReceiver.ReceiveHitEffect(hitInstance);
		}
		int num5 = Mathf.RoundToInt((float)hitInstance.DamageDealt * hitInstance.Multiplier);
		if (damageOverride)
		{
			num5 = 1;
		}
		switch (CheatManager.NailDamage)
		{
		case CheatManager.NailDamageStates.InstaKill:
			num5 = int.MaxValue;
			break;
		case CheatManager.NailDamageStates.NoDamage:
			num5 = 0;
			break;
		}
		hasTakenDamage = true;
		if (sendDamageTo == null)
		{
			hp = Mathf.Max(hp - num5, -1000);
		}
		else
		{
			sendDamageTo.hp = Mathf.Max(sendDamageTo.hp - num5, -1000);
		}
		if (hitInstance.NonLethal && hp <= 0)
		{
			hp = 1;
		}
		if (hitInstance.PoisonDamageTicks > 0)
		{
			DamageTag poisonPouchDamageTag = Gameplay.PoisonPouchDamageTag;
			tagDamageTaker.AddDamageTagToStack(poisonPouchDamageTag, hitInstance.PoisonDamageTicks);
		}
		if (hitInstance.ZapDamageTicks > 0)
		{
			DamageTag zapDamageTag = Gameplay.ZapDamageTag;
			tagDamageTaker.AddDamageTagToStack(zapDamageTag, hitInstance.ZapDamageTicks);
		}
		if (hp > 0)
		{
			if (this.TookDamage != null)
			{
				this.TookDamage();
			}
			NonFatalHit(hitInstance.IgnoreInvulnerable);
			ApplyStunDamage(hitInstance.StunDamage);
			return;
		}
		float num6 = hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Corpse);
		float magnitudeMultForType = hitInstance.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		bool disallowDropFling;
		if (hitInstance.Source != null)
		{
			Transform root = hitInstance.Source.transform.root;
			if (root.CompareTag("Player") && !hitInstance.UseCorpseDirection && num6.IsWithinTolerance(Mathf.Epsilon, 270f))
			{
				num6 = ((!(root.lossyScale.x < 0f)) ? 180 : 0);
			}
			disallowDropFling = hitInstance.Source.GetComponent<BreakItemsOnContact>();
		}
		else
		{
			disallowDropFling = false;
		}
		DieDropFling(hitInstance.ToolDamageFlags, disallowDropFling);
		Die(num6, hitInstance.AttackType, hitInstance.NailElement, hitInstance.Source, hitInstance.IgnoreInvulnerable, magnitudeMultForType, overrideSpecialDeath: false, disallowDropFling);
	}

	private bool IsImmuneTo(HitInstance hitInstance, bool wasFullHit)
	{
		switch (hitInstance.AttackType)
		{
		case AttackTypes.Nail:
			if (!immuneToNailAttacks)
			{
				break;
			}
			goto IL_007c;
		case AttackTypes.Acid:
			if (!ignoreAcid)
			{
				break;
			}
			goto IL_007c;
		case AttackTypes.RuinsWater:
			if (ignoreWater)
			{
				goto IL_007c;
			}
			if (!immuneToWater)
			{
				break;
			}
			goto IL_00c2;
		case AttackTypes.Hunter:
			if (!immuneToHunterWeapon)
			{
				break;
			}
			goto IL_007c;
		case AttackTypes.Spikes:
			if (!immuneToSpikes)
			{
				break;
			}
			goto IL_007c;
		case AttackTypes.Explosion:
			if (immuneToExplosions)
			{
				if (wasFullHit)
				{
					FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED EXPLOSION");
				}
				return true;
			}
			break;
		case AttackTypes.Coal:
			if (!immuneToCoal)
			{
				break;
			}
			goto IL_00c2;
		case AttackTypes.Trap:
			if (!immuneToTraps)
			{
				break;
			}
			goto IL_00c2;
		case AttackTypes.Lava:
			{
				if (!immuneToLava)
				{
					break;
				}
				goto IL_00c2;
			}
			IL_00c2:
			return true;
			IL_007c:
			return true;
		}
		return false;
	}

	private void NonFatalHit(bool ignoreEvasion)
	{
		if (ignoreEvasion)
		{
			return;
		}
		if (hasAlternateHitAnimation)
		{
			if (animator != null)
			{
				animator.Play(alternateHitAnimation);
			}
		}
		else
		{
			evasionByHitRemaining = 0f;
		}
	}

	public void ApplyStunDamage(float stunDamage)
	{
		if ((bool)stunControlFsm && !(stunDamage <= Mathf.Epsilon))
		{
			if (CheatManager.ForceNextHitStun || CheatManager.ForceStun)
			{
				stunDamage = float.MaxValue;
				CheatManager.ForceNextHitStun = false;
			}
			stunControlFsm.FsmVariables.FindFsmFloat("Stun Damage").Value = stunDamage;
			stunControlFsm.SendEvent("STUN DAMAGE");
		}
	}

	public void ApplyExtraDamage(int damageAmount)
	{
		hp = Mathf.Max(hp - damageAmount, 0);
		if (hp <= 0)
		{
			Die(null, AttackTypes.Generic, ignoreEvasion: true);
		}
	}

	public void ApplyExtraDamage(HitInstance hitInstance)
	{
		hitInstance = ApplyDamageScaling(hitInstance);
		hp = Mathf.Max(hp - hitInstance.DamageDealt, 0);
		if (hp <= 0)
		{
			DieDropFling(hitInstance.ToolDamageFlags, disallowDropFling: false);
			Die(null, hitInstance.AttackType, hitInstance.NailElement, hitInstance.Source, hitInstance.IgnoreInvulnerable, 0f);
		}
	}

	public bool CheckNailImbuementHit(NailImbuementConfig nailImbuement, int setHitCount)
	{
		if (damageTagHitsLeftTracker == null)
		{
			damageTagHitsLeftTracker = new Dictionary<NailImbuementConfig, int>(1);
		}
		if (damageTagHitsLeftTracker.TryGetValue(nailImbuement, out var value))
		{
			value--;
			if (value <= 0)
			{
				damageTagHitsLeftTracker.Remove(nailImbuement);
				return true;
			}
			damageTagHitsLeftTracker[nailImbuement] = value;
			return false;
		}
		damageTagHitsLeftTracker[nailImbuement] = setHitCount;
		return false;
	}

	public void AddDamageTagToStack(DamageTag damageTag, int hitAmountOverride = 0)
	{
		tagDamageTaker.AddDamageTagToStack(damageTag, hitAmountOverride);
	}

	public bool ApplyTagDamage(DamageTag.DamageTagInstance damageTagInstance)
	{
		if (isDead)
		{
			return false;
		}
		if (damageTagInstance.isHeroDamage)
		{
			WillAwardJournalKill = true;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK TAG DAMAGE");
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK DAMAGE");
		if (!hasBlackThreadState || !blackThreadState.IsInForcedSing)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "SING DURATION END");
		}
		hasTakenDamage = true;
		ApplyExtraDamage(new HitInstance
		{
			AttackType = AttackTypes.Generic,
			NailElement = damageTagInstance.nailElements,
			IsHeroDamage = damageTagInstance.isHeroDamage,
			DamageDealt = damageTagInstance.amount
		});
		return true;
	}

	private void DieDropFling(ToolDamageFlags toolDamageFlags, bool disallowDropFling)
	{
		if (!disallowDropFling && GameManager.instance.GetCurrentMapZoneEnum() == MapZone.MEMORY)
		{
			disallowDropFling = true;
		}
		if (!(dontDropMeat || disallowDropFling))
		{
			int amount;
			CollectableItemToolDamage item = CollectableItemToolDamage.GetItem(toolDamageFlags, enemySize, out amount);
			if ((bool)item && item.CanGetMore())
			{
				queuedDropItem = new QueuedDropItem(item, amount);
			}
		}
	}

	public void Die(float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
	{
		Die(attackDirection, attackType, ignoreEvasion, 1f);
	}

	public void Die(float? attackDirection, AttackTypes attackType, bool ignoreEvasion, float corpseFlingMultiplier)
	{
		Die(attackDirection, attackType, NailElements.None, null, ignoreEvasion, corpseFlingMultiplier);
	}

	public void Die(float? attackDirection, AttackTypes attackType, NailElements nailElement, GameObject damageSource, bool ignoreEvasion = false, float corpseFlingMultiplier = 1f, bool overrideSpecialDeath = false, bool disallowDropFling = false)
	{
		if (isDead || (preventDeathAfterHero && HeroController.instance.cState.dead))
		{
			return;
		}
		CancelAllLagHits();
		float minCorpseFlingMagnitudeMult = GlobalSettings.Corpse.MinCorpseFlingMagnitudeMult;
		if (corpseFlingMultiplier < minCorpseFlingMagnitudeMult && Math.Abs(corpseFlingMultiplier) > Mathf.Epsilon)
		{
			corpseFlingMultiplier = minCorpseFlingMagnitudeMult;
		}
		Action<Transform> action = null;
		if (!disallowDropFling && attackType != AttackTypes.RuinsWater && GameManager.instance.GetCurrentMapZoneEnum() != MapZone.MEMORY)
		{
			int smallGeoExtra = 0;
			int mediumGeoExtra = 0;
			int largeGeoExtra = 0;
			int largeSmoothGeoExtra = 0;
			int shellShardExtra = 0;
			int shellShardBase = shellShardDrops;
			bool thiefCharmEquipped = Gameplay.ThiefCharmTool.IsEquipped;
			if (thiefCharmEquipped)
			{
				smallGeoExtra = Mathf.CeilToInt((float)smallGeoDrops * Gameplay.ThiefCharmGeoSmallIncrease);
				mediumGeoExtra = Mathf.CeilToInt((float)mediumGeoDrops * Gameplay.ThiefCharmGeoMedIncrease);
				largeGeoExtra = Mathf.CeilToInt((float)largeGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
				largeSmoothGeoExtra = Mathf.CeilToInt((float)largeSmoothGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
			}
			float num = _shellShardMultiplierArray[UnityEngine.Random.Range(0, _shellShardMultiplierArray.Length)];
			int num2 = Mathf.CeilToInt((float)shellShardBase * num - (float)shellShardBase);
			if (num2 > 8)
			{
				num2 = 8;
			}
			shellShardBase += num2;
			ToolItem boneNecklaceTool = Gameplay.BoneNecklaceTool;
			if ((bool)boneNecklaceTool && boneNecklaceTool.IsEquipped)
			{
				shellShardExtra = Mathf.CeilToInt((float)shellShardBase * Gameplay.BoneNecklaceShellshardIncrease);
			}
			int dropFlingAngleMin = (megaFlingGeo ? 65 : 80);
			int dropFlingAngleMax = (megaFlingGeo ? 115 : 100);
			int dropFlingSpeedMin = (megaFlingGeo ? 30 : 15);
			int dropFlingSpeedMax = (megaFlingGeo ? 45 : 30);
			Transform sourceTransform = base.transform;
			QueuedDropItem queuedItem = queuedDropItem;
			queuedDropItem.Reset();
			action = delegate(Transform spawnPoint)
			{
				SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax, dropFlingAngleMin, dropFlingAngleMax, smallGeoDrops, mediumGeoDrops, largeGeoDrops, largeSmoothGeoDrops, shouldGeoFlash: false, shellShardBase, shouldShardsFlash: false);
				SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax, dropFlingAngleMin, dropFlingAngleMax, 0, 0, 0, 0, shouldGeoFlash: false, shellShardExtra, shouldShardsFlash: true);
				if (smallGeoExtra > 0 || mediumGeoExtra > 0 || largeGeoExtra > 0 || largeSmoothGeoExtra > 0)
				{
					if (spawnPoint == sourceTransform)
					{
						SpawnExtraGeo();
					}
					else
					{
						spawnPoint.GetComponent<MonoBehaviour>().ExecuteDelayed(0.2f, SpawnExtraGeo);
					}
				}
				SpawnQueuedItemDrop(queuedItem, spawnPoint);
				foreach (ItemDropGroup itemDropGroup in itemDropGroups)
				{
					if (itemDropGroup.Drops.Count > 0 && !(itemDropGroup.TotalProbability < 1f))
					{
						ItemDropProbability itemDropProbability = (ItemDropProbability)Probability.GetRandomItemRootByProbability<ItemDropProbability, SavedItem>(itemDropGroup.Drops.ToArray());
						if (itemDropProbability != null)
						{
							SpawnItemDrop(itemDropProbability.Item, itemDropProbability.Amount.GetRandomValue(), itemDropProbability.CustomPickupPrefab, spawnPoint, itemDropProbability.LimitActiveInScene);
						}
					}
				}
				void SpawnExtraGeo()
				{
					if (thiefCharmEquipped)
					{
						Gameplay.ThiefCharmEnemyDeathAudio.SpawnAndPlayOneShot(spawnPoint.position);
					}
					SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax, dropFlingAngleMin, dropFlingAngleMax, smallGeoExtra, mediumGeoExtra, largeGeoExtra, largeSmoothGeoExtra, shouldGeoFlash: true, 0, shouldShardsFlash: false);
				}
			};
		}
		if (!overrideSpecialDeath)
		{
			GameObject go = (zeroHPEventOverride ? zeroHPEventOverride : base.gameObject);
			FSMUtility.SendEventToGameObject(go, "ZERO HP");
			if ((bool)blackThreadState)
			{
				blackThreadState.CancelAttack();
			}
			if (attackType == AttackTypes.Lava)
			{
				FSMUtility.SendEventToGameObject(go, "LAVA DEATH");
			}
			if (hasSpecialDeath)
			{
				NonFatalHit(ignoreEvasion);
				action?.Invoke(base.transform);
				return;
			}
		}
		isDead = true;
		if (damageHero != null)
		{
			damageHero.damageDealt = 0;
		}
		if (battleScene != null && !notifiedBattleScene)
		{
			BattleScene component = battleScene.GetComponent<BattleScene>();
			if (component != null)
			{
				if (!bigEnemyDeath)
				{
					component.DecrementEnemy();
				}
				else
				{
					component.DecrementBigEnemy();
				}
			}
		}
		if (deathAudioSnapshot != null)
		{
			deathAudioSnapshot.TransitionTo(6f);
		}
		if (sendKilledTo != null)
		{
			FSMUtility.SendEventToGameObject(sendKilledTo, "KILLED");
		}
		SendDeathEvent();
		if (attackType == AttackTypes.Splatter)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
			corpseSplatPrefab.Spawn(base.transform.position + effectOrigin, Quaternion.identity);
			if ((bool)enemyDeathEffects)
			{
				enemyDeathEffects.EmitSound();
			}
			base.gameObject.SetActive(value: false);
			return;
		}
		bool didCallCorpseBegin;
		GameObject corpseObj;
		if (enemyDeathEffects != null)
		{
			if (attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid || attackType == AttackTypes.Generic)
			{
				enemyDeathEffects.SkipKillFreeze = true;
			}
			enemyDeathEffects.ReceiveDeathEvent(attackDirection, attackType, nailElement, damageSource, corpseFlingMultiplier, deathReset, action, out didCallCorpseBegin, out corpseObj);
		}
		else
		{
			didCallCorpseBegin = false;
			corpseObj = null;
		}
		if (!didCallCorpseBegin)
		{
			action?.Invoke(corpseObj ? corpseObj.transform : base.transform);
		}
		if (corpseObj != null && corpseEventResponder != null)
		{
			corpseObj.GetComponent<EventRelay>().TemporaryEvent += corpseEventResponder.ReceiveEvent;
		}
	}

	private void SpawnCurrency(Transform spawnPoint, float dropFlingSpeedMin, float dropFlingSpeedMax, float dropFlingAngleMin, float dropFlingAngleMax, int smallGeoCount, int mediumGeoCount, int largeGeoCount, int largeSmoothGeoCount, bool shouldGeoFlash, int shellShardCount, bool shouldShardsFlash)
	{
		if (smallGeoCount > 0)
		{
			spawnedFlingTracker.Clear();
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = Gameplay.SmallGeoPrefab;
			config.AmountMin = smallGeoCount;
			config.AmountMax = smallGeoCount;
			config.SpeedMin = dropFlingSpeedMin;
			config.SpeedMax = dropFlingSpeedMax;
			config.AngleMin = dropFlingAngleMin;
			config.AngleMax = dropFlingAngleMax;
			FlingUtils.SpawnAndFling(config, spawnPoint, effectOrigin, spawnedFlingTracker);
			if (shouldGeoFlash)
			{
				SetCurrencyFlashing(spawnedFlingTracker, isShellShard: false);
			}
		}
		if (mediumGeoCount > 0)
		{
			spawnedFlingTracker.Clear();
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = Gameplay.MediumGeoPrefab;
			config.AmountMin = mediumGeoCount;
			config.AmountMax = mediumGeoCount;
			config.SpeedMin = dropFlingSpeedMin;
			config.SpeedMax = dropFlingSpeedMax;
			config.AngleMin = dropFlingAngleMin;
			config.AngleMax = dropFlingAngleMax;
			FlingUtils.SpawnAndFling(config, spawnPoint, effectOrigin, spawnedFlingTracker);
			if (shouldGeoFlash)
			{
				SetCurrencyFlashing(spawnedFlingTracker, isShellShard: false);
			}
		}
		if (largeGeoCount > 0)
		{
			spawnedFlingTracker.Clear();
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = Gameplay.LargeGeoPrefab;
			config.AmountMin = largeGeoCount;
			config.AmountMax = largeGeoCount;
			config.SpeedMin = dropFlingSpeedMin;
			config.SpeedMax = dropFlingSpeedMax;
			config.AngleMin = dropFlingAngleMin;
			config.AngleMax = dropFlingAngleMax;
			FlingUtils.SpawnAndFling(config, spawnPoint, effectOrigin, spawnedFlingTracker);
			if (shouldGeoFlash)
			{
				SetCurrencyFlashing(spawnedFlingTracker, isShellShard: false);
			}
		}
		if (largeSmoothGeoCount > 0)
		{
			spawnedFlingTracker.Clear();
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = Gameplay.LargeSmoothGeoPrefab;
			config.AmountMin = largeSmoothGeoCount;
			config.AmountMax = largeSmoothGeoCount;
			config.SpeedMin = dropFlingSpeedMin;
			config.SpeedMax = dropFlingSpeedMax;
			config.AngleMin = dropFlingAngleMin;
			config.AngleMax = dropFlingAngleMax;
			FlingUtils.SpawnAndFling(config, spawnPoint, effectOrigin, spawnedFlingTracker);
			if (shouldGeoFlash)
			{
				SetCurrencyFlashing(spawnedFlingTracker, isShellShard: false);
			}
		}
		if (shellShardCount > 0)
		{
			spawnedFlingTracker.Clear();
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = Gameplay.ShellShardPrefab;
			config.AmountMin = shellShardCount;
			config.AmountMax = shellShardCount;
			config.SpeedMin = dropFlingSpeedMin;
			config.SpeedMax = dropFlingSpeedMax;
			config.AngleMin = dropFlingAngleMin;
			config.AngleMax = dropFlingAngleMax;
			FlingUtils.SpawnAndFlingShellShards(config, spawnPoint, effectOrigin, spawnedFlingTracker);
			if (shouldShardsFlash)
			{
				SetCurrencyFlashing(spawnedFlingTracker, isShellShard: true);
			}
		}
	}

	private void SpawnQueuedItemDrop(QueuedDropItem queuedDropItem, Transform spawnPoint)
	{
		if (queuedDropItem.isQueued)
		{
			CollectableItemToolDamage dropItem = queuedDropItem.dropItem;
			if (!(dropItem == null))
			{
				SpawnItemDrop(dropItem, queuedDropItem.amount, Gameplay.CollectableItemPickupMeatPrefab, spawnPoint, 0);
			}
		}
	}

	private void SpawnItemDrop(SavedItem dropItem, int count, CollectableItemPickup prefab, Transform spawnPoint, int limit)
	{
		if (!dropItem || !dropItem.CanGetMore())
		{
			return;
		}
		if (!prefab)
		{
			prefab = Gameplay.CollectableItemPickupInstantPrefab;
		}
		if (!prefab)
		{
			return;
		}
		Vector3 vector = spawnPoint.TransformPoint(effectOrigin);
		for (int i = 0; i < count; i++)
		{
			Vector3 position = vector;
			PrefabCollectable prefabCollectable = dropItem as PrefabCollectable;
			GameObject gameObject;
			bool flag;
			if (prefabCollectable != null)
			{
				gameObject = prefabCollectable.Spawn();
				flag = false;
			}
			else
			{
				CollectableItemPickup collectableItemPickup;
				if (limit > 0)
				{
					collectableItemPickup = ObjectPool.Spawn(prefab.gameObject, null, position, Quaternion.identity, stealActiveSpawned: true).GetComponent<CollectableItemPickup>();
				}
				else
				{
					collectableItemPickup = UnityEngine.Object.Instantiate(prefab);
					collectableItemPickup.transform.position = position;
				}
				flag = true;
				collectableItemPickup.SetItem(dropItem);
				gameObject = collectableItemPickup.gameObject;
			}
			FlingUtils.SelfConfig config = default(FlingUtils.SelfConfig);
			config.Object = gameObject;
			config.SpeedMin = 15f;
			config.SpeedMax = 30f;
			config.AngleMin = 80f;
			config.AngleMax = 100f;
			FlingUtils.FlingObject(config, spawnPoint, effectOrigin);
			if (flag)
			{
				gameObject.transform.SetPositionZ(UnityEngine.Random.Range(0.003f, 0.0039f));
			}
		}
	}

	public void SendDeathEvent()
	{
		if (this.OnDeath != null)
		{
			this.OnDeath();
		}
	}

	public void SetDead()
	{
		isDead = true;
	}

	private void SetCurrencyFlashing(IReadOnlyList<GameObject> gameObjects, bool isShellShard)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			SpriteFlash component = gameObject.GetComponent<SpriteFlash>();
			if (!component)
			{
				continue;
			}
			if (isShellShard)
			{
				component.GeoFlash();
				continue;
			}
			component.FlashExtraRosary();
			GeoControl component2 = gameObject.GetComponent<GeoControl>();
			if ((bool)component2)
			{
				component2.SpawnThiefCharmEffect();
			}
		}
	}

	public bool IsBlockingByDirection(int cardinalDirection, AttackTypes attackType, SpecialTypes specialType = SpecialTypes.None)
	{
		if (!invincible)
		{
			return false;
		}
		switch (attackType)
		{
		case AttackTypes.Lava:
		case AttackTypes.Coal:
			return false;
		case AttackTypes.Spell:
		case AttackTypes.SharpShadow:
		case AttackTypes.Explosion:
			if (base.gameObject.CompareTag("Spell Vulnerable"))
			{
				return false;
			}
			break;
		}
		if ((piercable || invincibleFromDirection != 0) && (attackType == AttackTypes.Explosion || attackType == AttackTypes.Lightning || (specialType & SpecialTypes.Piercer) != 0))
		{
			return false;
		}
		if (invincibleFromDirection == 0)
		{
			return true;
		}
		switch (cardinalDirection)
		{
		case 0:
			switch (invincibleFromDirection)
			{
			case 1:
			case 5:
			case 8:
			case 10:
			case 12:
			case 13:
				return true;
			default:
				return false;
			}
		case 1:
			switch (invincibleFromDirection)
			{
			case 2:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 13:
				return true;
			default:
				return false;
			}
		case 2:
			switch (invincibleFromDirection)
			{
			case 3:
			case 6:
			case 9:
			case 11:
			case 12:
			case 13:
				return true;
			default:
				return false;
			}
		case 3:
		{
			int num = invincibleFromDirection;
			if (num == 4 || (uint)(num - 7) <= 5u)
			{
				return true;
			}
			return false;
		}
		default:
			return false;
		}
	}

	public void SetBattleScene(GameObject newBattleScene)
	{
		battleScene = newBattleScene;
	}

	public int GetAttackDirection()
	{
		return directionOfLastAttack;
	}

	public AttackTypes GetLastAttackType()
	{
		return lastAttackType;
	}

	public void SetPreventInvincibleEffect(bool set)
	{
		preventInvincibleEffect = set;
	}

	public void SetGeoSmall(int amount)
	{
		smallGeoDrops = amount;
	}

	public void SetGeoMedium(int amount)
	{
		mediumGeoDrops = amount;
	}

	public void SetGeoLarge(int amount)
	{
		largeGeoDrops = amount;
	}

	public void SetShellShards(int amount)
	{
		shellShardDrops = amount;
	}

	public void ClearItemDropsBattleScene()
	{
		if (HasClearedItemDrops)
		{
			return;
		}
		for (int num = itemDropGroups.Count - 1; num >= 0; num--)
		{
			ItemDropGroup itemDropGroup = itemDropGroups[num];
			if (itemDropGroup.Drops.Count != 0)
			{
				for (int num2 = itemDropGroup.Drops.Count - 1; num2 >= 0; num2--)
				{
					ItemDropProbability itemDropProbability = itemDropGroup.Drops[num2];
					if ((bool)itemDropProbability.Item && !(itemDropProbability.Item is FullQuestBase))
					{
						itemDropGroup.Drops.RemoveAt(num2);
					}
				}
				if (itemDropGroup.Drops.Count == 0)
				{
					itemDropGroups.RemoveAt(num);
				}
			}
		}
		HasClearedItemDrops = true;
	}

	public IEnumerable<SavedItem> EnumerateItemDrops()
	{
		foreach (ItemDropGroup itemDropGroup in itemDropGroups)
		{
			foreach (ItemDropProbability drop in itemDropGroup.Drops)
			{
				if ((bool)drop.Item)
				{
					yield return drop.Item;
				}
			}
		}
	}

	public bool GetIsDead()
	{
		return isDead;
	}

	public void SetIsDead(bool set)
	{
		isDead = set;
	}

	public void SetDamageOverride(bool set)
	{
		damageOverride = set;
	}

	public void SetSendKilledToObject(GameObject killedObject)
	{
		if (killedObject != null)
		{
			sendKilledToObject = killedObject;
		}
	}

	public bool CheckInvincible()
	{
		return invincible;
	}

	public void HealToMax()
	{
		hp = initHp;
	}

	public bool HasTakenDamage()
	{
		return hasTakenDamage;
	}

	public void AddHP(int hpAdd, int hpMax)
	{
		hp += hpAdd;
		isDead = false;
		if (hp > hpMax)
		{
			hp = hpMax;
		}
	}

	public void RefillHP()
	{
		isDead = false;
		hp = initHp;
		hasTakenDamage = false;
	}

	public void SetImmuneToNailAttacks(bool immune)
	{
		immuneToNailAttacks = immune;
	}

	public void SetImmuneToTraps(bool immune)
	{
		immuneToTraps = immune;
	}

	public void SetImmuneToSpikes(bool immune)
	{
		immuneToSpikes = immune;
	}

	public bool ShouldIgnore(IgnoreFlags ignoreFlags)
	{
		if (ignoreFlags == IgnoreFlags.None)
		{
			return false;
		}
		return (this.ignoreFlags & ignoreFlags) == ignoreFlags;
	}

	public void DoLagHits(LagHitOptions options, HitInstance hitInstance)
	{
		if (hp <= 0 || IsImmuneTo(hitInstance, wasFullHit: false) || !options.ShouldDoLagHits())
		{
			return;
		}
		bool doStartDelay = true;
		if (runningSingleLagHits.TryGetValue(options.DamageType, out var value))
		{
			StopCoroutine(value.LagHitsRoutine);
			value.OnLagHitsEnd();
			runningSingleLagHits.Remove(options.DamageType);
			doStartDelay = false;
		}
		int damageDealt = (options.UseNailDamage ? Mathf.RoundToInt((float)PlayerData.instance.nailDamage * options.NailDamageMultiplier) : options.HitDamage);
		if (hitInstance.AttackType == AttackTypes.Nail)
		{
			hitInstance.AttackType = AttackTypes.Generic;
		}
		hitInstance.DamageDealt = damageDealt;
		GameObject[] slashEffectOverrides = options.SlashEffectOverrides;
		hitInstance.SlashEffectOverrides = ((slashEffectOverrides != null && slashEffectOverrides.Length != 0) ? options.SlashEffectOverrides : null);
		hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.LagHit;
		hitInstance.MagnitudeMultiplier = options.MagnitudeMult;
		hitInstance.CriticalHit = false;
		hitInstance.StunDamage = 0f;
		options.OnStart(base.transform, effectOrigin, hitInstance, out var spawnedHitMarkedEffect);
		LagHitsTracker tracker = new LagHitsTracker
		{
			Source = hitInstance.Source
		};
		tracker.OnLagHitsEnd = delegate
		{
			if ((bool)spawnedHitMarkedEffect)
			{
				spawnedHitMarkedEffect.Stop();
				spawnedHitMarkedEffect = null;
			}
			runningLagHits.Remove(tracker);
		};
		LagHitDamageType damageType = options.DamageType;
		if (damageType == LagHitDamageType.WitchPoison || damageType == LagHitDamageType.Flintstone || damageType == LagHitDamageType.Dazzle)
		{
			runningSingleLagHits[options.DamageType] = tracker;
			LagHitsTracker lagHitsTracker = tracker;
			lagHitsTracker.OnLagHitsEnd = (Action)Delegate.Combine(lagHitsTracker.OnLagHitsEnd, (Action)delegate
			{
				runningSingleLagHits.Remove(options.DamageType);
			});
		}
		runningLagHits.Add(tracker);
		tracker.LagHitsRoutine = StartCoroutine(LagHits(options, hitInstance, tracker.OnLagHitsEnd, doStartDelay));
	}

	public static void CancelAllLagHitsForSource(GameObject source)
	{
		foreach (HealthManager activeHealthManager in _activeHealthManagers)
		{
			activeHealthManager.CancelLagHitsForSource(source);
		}
	}

	public void CancelLagHitsForSource(GameObject source)
	{
		for (int num = runningLagHits.Count - 1; num >= 0; num--)
		{
			LagHitsTracker lagHitsTracker = runningLagHits[num];
			if (!(lagHitsTracker.Source != source))
			{
				StopCoroutine(lagHitsTracker.LagHitsRoutine);
				lagHitsTracker.OnLagHitsEnd();
			}
		}
	}

	public void CancelAllLagHits()
	{
		for (int num = runningLagHits.Count - 1; num >= 0; num--)
		{
			LagHitsTracker lagHitsTracker = runningLagHits[num];
			StopCoroutine(lagHitsTracker.LagHitsRoutine);
			lagHitsTracker.OnLagHitsEnd();
		}
	}

	private IEnumerator LagHits(LagHitOptions options, HitInstance hitInstance, Action onLagHitsEnd, bool doStartDelay)
	{
		if (doStartDelay)
		{
			yield return new WaitForSeconds(options.StartDelay);
		}
		int hitsDone = 0;
		float elapsed = 0f;
		while (hitsDone < options.HitCount)
		{
			if (!options.IgnoreBlock)
			{
				int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
				if (IsBlockingByDirection(cardinalDirection, hitInstance.AttackType, hitInstance.SpecialType))
				{
					break;
				}
			}
			if (elapsed >= options.HitDelay)
			{
				elapsed %= options.HitDelay;
				bool flag;
				if (options.IsExtraDamage)
				{
					ApplyExtraDamage(hitInstance);
					flag = true;
				}
				else
				{
					bool flag2 = DoNotGiveSilk;
					if (!options.HitsGiveSilk)
					{
						doNotGiveSilk = true;
					}
					flag = (IHitResponder.Response)Hit(hitInstance) == IHitResponder.Response.DamageEnemy;
					doNotGiveSilk = flag2;
				}
				if (flag)
				{
					options.OnHit(base.transform, effectOrigin, hitInstance);
				}
				hitsDone++;
			}
			yield return null;
			elapsed += Time.deltaTime;
		}
		if (onLagHitsEnd != null)
		{
			onLagHitsEnd();
		}
		else
		{
			options.OnEnd(base.transform, effectOrigin, hitInstance);
		}
	}

	private void AddPhysicalPusher()
	{
		if ((bool)boxCollider && !addedPhysicalPusher)
		{
			addedPhysicalPusher = true;
			GameObject enemyPhysicalPusher = Effects.EnemyPhysicalPusher;
			physicalPusher = UnityEngine.Object.Instantiate(enemyPhysicalPusher);
			physicalPusher.name = $"{enemyPhysicalPusher.gameObject.name} ({base.gameObject.name})";
			physicalPusher.layer = 27;
			physicalPusher.transform.SetParentReset(base.transform);
			CapsuleCollider2D component = physicalPusher.GetComponent<CapsuleCollider2D>();
			component.offset = boxCollider.offset;
			Vector2 size = boxCollider.size;
			size.x = Mathf.Max(size.x - 0.5f, 0.5f);
			component.size = size;
		}
	}

	public GameObject GetPhysicalPusher()
	{
		if (!addedPhysicalPusher)
		{
			AddPhysicalPusher();
		}
		return physicalPusher;
	}

	public void DoStealHit()
	{
		if (TrySteal(out var stolenGeo, out var flingGeo, out var stolenShards, out var flingShards))
		{
			DoLagHits(new StealLagHit(this, stolenGeo, flingGeo, stolenShards, flingShards), lastHitInstance);
		}
	}

	private bool TrySteal(out int stolenGeo, out int flingGeo, out int stolenShards, out int flingShards)
	{
		stolenGeo = 0;
		flingGeo = 0;
		stolenShards = 0;
		flingShards = 0;
		if (!Gameplay.ThiefPickTool.IsEquipped)
		{
			return false;
		}
		int value = Gameplay.SmallGeoValue.Value;
		int value2 = Gameplay.MediumGeoValue.Value;
		int value3 = Gameplay.LargeGeoValue.Value;
		int num = smallGeoDrops * value + mediumGeoDrops * value2 + (largeGeoDrops + largeSmoothGeoDrops) * value3;
		int randomValue = Gameplay.ThiefPickGeoStealMin.GetRandomValue();
		float randomValue2 = Gameplay.ThiefPickGeoSteal.GetRandomValue();
		stolenGeo = Mathf.CeilToInt(randomValue2 * (float)num);
		if (stolenGeo < randomValue)
		{
			stolenGeo = randomValue;
		}
		bool flag = UnityEngine.Random.Range(0f, 1f) < Gameplay.ThiefPickLooseChance;
		flingGeo = (flag ? Gameplay.ThiefPickGeoLoose.GetRandomValue() : 0);
		int num2 = stolenGeo + flingGeo;
		while (num2 > 0)
		{
			if (smallGeoDrops > 0)
			{
				smallGeoDrops--;
				num2--;
				continue;
			}
			if (mediumGeoDrops > 0)
			{
				mediumGeoDrops--;
				smallGeoDrops += value2;
				continue;
			}
			if (largeGeoDrops > 0 || largeSmoothGeoDrops > 0)
			{
				if (largeSmoothGeoDrops > 0)
				{
					largeSmoothGeoDrops--;
				}
				else
				{
					largeGeoDrops--;
				}
				mediumGeoDrops += value3 / value2;
				smallGeoDrops += value3 % value2 / value;
				continue;
			}
			if (flingGeo > 0)
			{
				flingGeo--;
			}
			else
			{
				if (stolenGeo <= 0)
				{
					break;
				}
				stolenGeo--;
			}
			num2--;
		}
		if (shellShardDrops > 0)
		{
			stolenShards = Gameplay.ThiefPickShardSteal.GetRandomValue();
			if (flag)
			{
				flingShards = Gameplay.ThiefPickShardLoose.GetRandomValue();
			}
		}
		return true;
	}

	private void FlingStealCurrency(int amount, GameObject currencyPrefab)
	{
		Vector3 position = HeroController.instance.transform.position;
		Vector3 position2 = base.transform.position;
		float angleMin;
		float angleMax;
		if (position.x > position2.x)
		{
			angleMin = 30f;
			angleMax = 65f;
		}
		else
		{
			angleMin = 150f;
			angleMax = 115f;
		}
		FlingUtils.Config config = default(FlingUtils.Config);
		config.Prefab = currencyPrefab;
		config.AmountMin = amount;
		config.AmountMax = amount;
		config.SpeedMin = 5f;
		config.SpeedMax = 20f;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
	}

	public void SetFlingSilkOrbsDown(bool set)
	{
		flingSilkOrbsDown = set;
	}

	public void SetEffectOrigin(Vector3 set)
	{
		effectOrigin = set;
	}
}
