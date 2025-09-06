using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : DebugDrawColliderRuntimeAdder, IHitResponder, IBreakerBreakable
{
	[Serializable]
	public class FlingObject
	{
		public GameObject referenceObject;

		[Space]
		public int spawnMin;

		public int spawnMax;

		public float speedMin;

		public float speedMax;

		public float angleMin;

		public float angleMax;

		public Vector2 originVariation;

		public FlingObject()
		{
			spawnMin = 25;
			spawnMax = 30;
			speedMin = 9f;
			speedMax = 20f;
			angleMin = 20f;
			angleMax = 160f;
			originVariation = new Vector2(0.5f, 0.5f);
		}

		public void Fling(Vector3 origin)
		{
			if (!referenceObject)
			{
				return;
			}
			int num = UnityEngine.Random.Range(spawnMin, spawnMax + 1);
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = referenceObject.Spawn();
				if ((bool)gameObject)
				{
					gameObject.transform.position = origin + new Vector3(UnityEngine.Random.Range(0f - originVariation.x, originVariation.x), UnityEngine.Random.Range(0f - originVariation.y, originVariation.y), 0f);
					float num2 = UnityEngine.Random.Range(speedMin, speedMax);
					float num3 = UnityEngine.Random.Range(angleMin, angleMax);
					float x = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
					float y = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
					Vector2 force = new Vector2(x, y);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if ((bool)component)
					{
						component.AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
		}
	}

	[Serializable]
	private class ItemDropProbability : Probability.ProbabilityBase<SavedItem>
	{
		[SerializeField]
		private SavedItem item;

		public CollectableItemPickup CustomPickupPrefab;

		public override SavedItem Item => item;
	}

	[Serializable]
	private class ItemDropGroup
	{
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		public ItemDropProbability[] Drops;
	}

	private Collider2D bodyCollider;

	[SerializeField]
	private PersistentBoolItem persistent;

	[HideInInspector]
	[SerializeField]
	private bool ignorePersistence;

	[Space]
	[SerializeField]
	private bool resetOnEnable;

	[SerializeField]
	private bool useAltDebris;

	[SerializeField]
	private float altDebrisChance = 100f;

	[Tooltip("Renderer which presents the undestroyed object.")]
	[SerializeField]
	private Renderer wholeRenderer;

	[Tooltip("List of child game objects which also represent the whole object.")]
	[SerializeField]
	private GameObject[] wholeParts;

	[Tooltip("List of child game objects which represent remnants that remain static after destruction.")]
	[SerializeField]
	private GameObject[] remnantParts;

	[SerializeField]
	private GameObject[] flingDrops;

	[SerializeField]
	private List<GameObject> debrisParts;

	[SerializeField]
	private List<GameObject> debrisPartsAlt;

	[SerializeField]
	private float angleOffset = -60f;

	[SerializeField]
	[Tooltip("Breakables behind this threshold are inert.")]
	[ModifiableProperty]
	[Conditional("useHeroPlane", false, false, false)]
	private float inertBackgroundThreshold;

	[SerializeField]
	[Tooltip("Breakables in front of this threshold are inert.")]
	[ModifiableProperty]
	[Conditional("useHeroPlane", false, false, false)]
	private float inertForegroundThreshold;

	[SerializeField]
	private bool useHeroPlane;

	[Tooltip("Breakable effects are spawned at this offset.")]
	[SerializeField]
	private Vector3 effectOffset;

	[EnsurePrefab]
	[Tooltip("Prefab to spawn for audio.")]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private AudioEvent hitAudioEvent;

	[Tooltip("Table of audio clips to play upon hit.")]
	[SerializeField]
	private RandomAudioClipTable hitAudioClipTable;

	[Tooltip("Table of audio clips to play upon break.")]
	[SerializeField]
	private AudioEvent breakAudioEvent;

	[Tooltip("Table of audio clips to play upon break.")]
	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	[Tooltip("Prefab to spawn when hit from a non-down angle.")]
	[SerializeField]
	private Transform dustHitRegularPrefab;

	[Tooltip("Prefab to spawn when hit from a down angle.")]
	[SerializeField]
	private Transform dustHitDownPrefab;

	[Tooltip("Optional prefab to spawn additional effects on break.")]
	[SerializeField]
	private GameObject breakEffectPrefab;

	[SerializeField]
	private float flingSpeedMin;

	[SerializeField]
	private float flingSpeedMax;

	[SerializeField]
	private bool flingSelfOnHit;

	[Tooltip("Strike effect prefab to spawn.")]
	[SerializeField]
	private Transform strikeEffectPrefab;

	[Tooltip("Nail hit prefab to spawn.")]
	[SerializeField]
	private Transform nailHitEffectPrefab;

	[Tooltip("Spell hit effect prefab to spawn.")]
	[SerializeField]
	private Transform spellHitEffectPrefab;

	[Tooltip("Legacy flag that was set but has always been broken but is no longer used?")]
	[SerializeField]
	private bool preventParticleRotation;

	[Tooltip("Object to send HIT event to.")]
	[SerializeField]
	private GameObject hitEventReciever;

	[Tooltip("Forward break effect to sibling FSMs.")]
	[SerializeField]
	private bool forwardBreakEvent;

	[SerializeField]
	[Tooltip("If enabled, this breakable can only be broken using e.g., a BreakableBreaker.")]
	private bool ignoreDamagers;

	[Space]
	[SerializeField]
	private EnemyHitEffectsRegular passHitToEffects;

	[Space]
	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private float hitCoolDown = 0.1f;

	[SerializeField]
	private bool firstHitOnly;

	[Obsolete("Just use noise radius now.")]
	[HideInInspector]
	[SerializeField]
	private bool emitNoise = true;

	[SerializeField]
	private float noiseRadius = 3f;

	[SerializeField]
	private bool autoAddJitterComponent = true;

	[SerializeField]
	private TrackTriggerObjects breakableRange;

	[SerializeField]
	private CameraShakeTarget hitShake;

	[SerializeField]
	private CameraShakeTarget breakShake;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private int freezeMoment;

	[SerializeField]
	private FreezeMomentTypes hitFreezeMoment = FreezeMomentTypes.None;

	[SerializeField]
	private FreezeMomentTypes breakFreezeMoment = FreezeMomentTypes.None;

	[Space]
	public Probability.ProbabilityGameObject[] containingParticles;

	public FlingObject[] flingObjectRegister;

	[Space]
	[SerializeField]
	private MinMaxInt smallGeoDrops;

	[SerializeField]
	private MinMaxInt mediumGeoDrops;

	[SerializeField]
	private MinMaxInt largeGeoDrops;

	[SerializeField]
	private MinMaxInt largeSmoothGeoDrops;

	[SerializeField]
	private bool megaFlingGeo;

	[SerializeField]
	private MinMaxInt shellShardDrops;

	[SerializeField]
	private int silkGain;

	[SerializeField]
	private ItemDropGroup[] itemDropGroups;

	[SerializeField]
	private MinMaxInt silkDrops;

	[SerializeField]
	private GameObject silkDropEffect;

	[SerializeField]
	private GameObject silkGetEffect;

	[SerializeField]
	private GameObject silkDropsCondition;

	[SerializeField]
	private GameObject threadThinObject;

	[SerializeField]
	private PersistentBoolItem silkDropsPersistent;

	[SerializeField]
	private bool deparentOnBreak;

	[SerializeField]
	private bool immuneToBreakableBreaker;

	[Space]
	[SerializeField]
	private Breakable[] requireBroken;

	[Space]
	[SerializeField]
	private UnityEvent onHit;

	[SerializeField]
	private UnityEvent onBreak;

	[SerializeField]
	private UnityEvent onBroken;

	private GameObject breakEffects;

	private double hitCooldownEndTime;

	private bool isBroken;

	private bool wasAlreadyBroken;

	private int hitsLeft;

	private int silkHits;

	private int startSilkHits;

	private bool brokenByNail;

	private Coroutine silkAddRoutine;

	private JitterSelfForTime hitJitter;

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Basic;

	public UnityEvent OnHit => onHit;

	public UnityEvent OnBreak => onBreak;

	public UnityEvent OnBroken => onBroken;

	public bool IsBroken => isBroken;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	public event Action<HitInstance> BrokenHit;

	public event Action AlreadyBroken;

	protected void Reset()
	{
		inertBackgroundThreshold = 1f;
		inertForegroundThreshold = -1f;
		effectOffset = new Vector3(0f, 0.5f, 0f);
		flingSpeedMin = 10f;
		flingSpeedMax = 17f;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(effectOffset), 0.15f);
	}

	private void OnValidate()
	{
		if (freezeMoment > 0)
		{
			breakFreezeMoment = (FreezeMomentTypes)freezeMoment;
			freezeMoment = 0;
		}
		if (emitNoise)
		{
			emitNoise = false;
			noiseRadius = 0f;
		}
		if (hitsToBreak <= 0)
		{
			hitsToBreak = 1;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		Transform transform = base.transform;
		bodyCollider = GetComponent<Collider2D>();
		flingDrops.SetAllActive(value: false);
		remnantParts.SetAllActive(value: false);
		debrisParts.SetAllActive(value: false);
		debrisPartsAlt.SetAllActive(value: false);
		if (!persistent && !silkDropsPersistent)
		{
			persistent = GetComponent<PersistentBoolItem>();
		}
		if (transform.Find("rosary") != null && transform.Find("rosary").gameObject.activeSelf)
		{
			smallGeoDrops = new MinMaxInt(8, 10);
			if (!persistent)
			{
				persistent = base.gameObject.AddComponent<PersistentBoolItem>();
			}
		}
		if (persistent != null)
		{
			persistent.OnGetSaveState += delegate(out bool val)
			{
				val = isBroken;
			};
			persistent.OnSetSaveState += delegate(bool val)
			{
				isBroken = val;
				if (isBroken)
				{
					SetAlreadyBroken();
				}
			};
		}
		if (hitsToBreak > 1)
		{
			hitJitter = GetComponent<JitterSelfForTime>();
			if (!hitJitter && autoAddJitterComponent)
			{
				hitJitter = JitterSelfForTime.AddHandler(base.gameObject, new Vector3(0.1f, 0.1f, 0f), 0.2f, 30f);
			}
		}
		silkHits = 0;
		bool flag = IsPartVisible(silkDropsCondition);
		bool flag2 = IsPartVisible(threadThinObject);
		if (flag || flag2)
		{
			hitsToBreak++;
			silkHits = 1;
			if (flag)
			{
				hitsToBreak++;
				silkHits = 2;
			}
			if ((bool)silkDropsPersistent)
			{
				silkDropsPersistent.OnGetSaveState += delegate(out bool val)
				{
					val = silkHits < startSilkHits || isBroken;
				};
				silkDropsPersistent.OnSetSaveState += delegate(bool val)
				{
					silkDropsCondition.SetActive(!val);
					if ((bool)threadThinObject)
					{
						threadThinObject.SetActive(!val);
					}
					if (val)
					{
						hitsLeft -= silkHits;
						hitsToBreak -= silkHits;
						silkHits = 0;
					}
				};
				ResetDynamicHierarchy resetter = base.gameObject.AddComponent<ResetDynamicHierarchy>();
				silkDropsPersistent.SemiPersistentReset += delegate
				{
					SetCooldown();
					isBroken = false;
					hitsLeft = hitsToBreak;
					silkHits = startSilkHits;
					resetter.DoReset(alsoRoot: true);
				};
			}
		}
		startSilkHits = silkHits;
	}

	private void OnEnable()
	{
		SetCooldown();
		if (resetOnEnable)
		{
			isBroken = false;
		}
	}

	public void SetCooldown()
	{
		hitCooldownEndTime = Time.timeAsDouble + (double)(5f * Time.fixedDeltaTime);
	}

	public void SetHitCoolDown()
	{
		hitCooldownEndTime = Time.timeAsDouble + (double)hitCoolDown;
	}

	protected void Start()
	{
		CreateAdditionalDebrisParts(debrisParts);
		bool flag;
		if (ignoreDamagers)
		{
			flag = false;
			if (!base.gameObject.GetComponent<NonBouncer>())
			{
				base.gameObject.AddComponent<NonBouncer>();
			}
		}
		else if (useHeroPlane)
		{
			flag = !base.transform.IsOnHeroPlane();
		}
		else
		{
			float z = base.transform.position.z;
			flag = (Math.Abs(inertBackgroundThreshold) > Mathf.Epsilon && z > inertBackgroundThreshold) || (Math.Abs(inertForegroundThreshold) > Mathf.Epsilon && z < inertForegroundThreshold);
		}
		if (flag)
		{
			BoxCollider2D component = GetComponent<BoxCollider2D>();
			if (component != null)
			{
				component.enabled = false;
			}
			UnityEngine.Object.Destroy(this);
			return;
		}
		Transform transform = base.transform;
		angleOffset *= Mathf.Sign(transform.localScale.x);
		if (breakEffectPrefab != null)
		{
			breakEffects = UnityEngine.Object.Instantiate(breakEffectPrefab, transform.position, transform.rotation);
			breakEffects.SetActive(value: false);
		}
		hitsLeft = hitsToBreak;
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ForceBuild();
		}
	}

	protected virtual void CreateAdditionalDebrisParts(List<GameObject> sourceDebrisParts)
	{
	}

	[ContextMenu("Break Self")]
	public void BreakSelf()
	{
		if (!isBroken)
		{
			Break(70f, 110f, 1f);
		}
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if (!immuneToBreakableBreaker)
		{
			BreakSelf();
		}
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if (!immuneToBreakableBreaker)
		{
			float direction = ((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
			Hit(new HitInstance
			{
				Source = breaker.gameObject,
				AttackType = AttackTypes.Generic,
				DamageDealt = 1,
				Direction = direction,
				Multiplier = 1f
			});
		}
	}

	public void SetAlreadyBroken()
	{
		silkHits = 0;
		wasAlreadyBroken = true;
		SetStaticPartsActivation(broken: true);
		GameObject[] array = flingDrops;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(value: true);
			}
		}
		if (this.AlreadyBroken != null)
		{
			this.AlreadyBroken();
		}
		if (onBroken != null)
		{
			onBroken.Invoke();
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (firstHitOnly && !damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (ignoreDamagers)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage && damageInstance.IsManualTrigger)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble <= hitCooldownEndTime)
		{
			if (damageInstance.AttackType == AttackTypes.Spikes)
			{
				DamageEnemies component = damageInstance.Source.GetComponent<DamageEnemies>();
				if ((bool)component)
				{
					component.PreventDamage(this);
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		if ((bool)breakableRange && breakableRange.isActiveAndEnabled && !breakableRange.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (isBroken)
		{
			return IHitResponder.Response.None;
		}
		Breakable[] array = requireBroken;
		foreach (Breakable breakable in array)
		{
			if ((bool)breakable && !breakable.isBroken)
			{
				return IHitResponder.Response.None;
			}
		}
		float impactAngle = damageInstance.Direction;
		float num = damageInstance.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		bool flag = false;
		if (damageInstance.AttackType == AttackTypes.Spell && spellHitEffectPrefab != null)
		{
			spellHitEffectPrefab.Spawn(base.transform.position).SetPositionZ(0.0031f);
			flag = true;
		}
		if (!flag)
		{
			if (!damageInstance.IsNailDamage && damageInstance.AttackType != AttackTypes.Generic)
			{
				impactAngle = 90f;
				num = 1f;
			}
			Vector3 position = (damageInstance.Source.transform.position + base.transform.TransformPoint(effectOffset)) * 0.5f;
			SpawnNailHitEffect(strikeEffectPrefab, position, impactAngle);
			SpawnNailHitEffect(nailHitEffectPrefab, position, impactAngle);
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		Transform transform = dustHitRegularPrefab;
		float num2;
		float num3;
		Vector3 euler;
		switch (cardinalDirection)
		{
		case 2:
			angleOffset *= -1f;
			num2 = 120f;
			num3 = 160f;
			euler = new Vector3(180f, 90f, 270f);
			break;
		case 0:
			num2 = 30f;
			num3 = 70f;
			euler = new Vector3(0f, 90f, 270f);
			break;
		case 1:
			angleOffset = 0f;
			num2 = 70f;
			num3 = 110f;
			num *= 1.5f;
			euler = new Vector3(270f, 90f, 270f);
			break;
		default:
			angleOffset = 0f;
			num2 = 160f;
			num3 = 380f;
			transform = dustHitDownPrefab;
			euler = new Vector3(-72.5f, -180f, -180f);
			break;
		}
		Vector3 position2 = base.transform.TransformPoint(effectOffset);
		if (transform != null)
		{
			transform.Spawn(position2, Quaternion.Euler(euler));
		}
		GameManager instance = GameManager.instance;
		SetHitCoolDown();
		if (onHit != null)
		{
			onHit.Invoke();
		}
		if ((bool)hitJitter)
		{
			hitJitter.StartTimedJitter();
		}
		if (damageInstance.AttackType == AttackTypes.Heavy || damageInstance.AttackType == AttackTypes.Explosion)
		{
			hitsLeft = 0;
		}
		else
		{
			hitsLeft--;
		}
		if ((bool)passHitToEffects)
		{
			passHitToEffects.ReceiveHitEffect(damageInstance, passHitToEffects.transform.InverseTransformPoint(position2));
		}
		if (hitsLeft > 0)
		{
			instance.FreezeMoment(hitFreezeMoment);
			hitShake.DoShake(this);
			hitAudioEvent.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
			if ((bool)hitAudioClipTable)
			{
				hitAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
			}
			if (flingSelfOnHit)
			{
				Rigidbody2D component2 = GetComponent<Rigidbody2D>();
				if (component2 != null)
				{
					float num4 = UnityEngine.Random.Range(num2, num3);
					Vector2 vector = new Vector2(Mathf.Cos(num4 * (MathF.PI / 180f)), Mathf.Sin(num4 * (MathF.PI / 180f)));
					float num5 = UnityEngine.Random.Range(flingSpeedMin, flingSpeedMax) * num;
					component2.linearVelocity = vector * num5;
					SpinSelfSimple component3 = GetComponent<SpinSelfSimple>();
					if (component3 != null)
					{
						component3.DoSpin();
					}
				}
			}
			if (silkHits > 0)
			{
				if (damageInstance.IsNailDamage)
				{
					if (silkGetEffect != null)
					{
						Transform transform2 = base.transform;
						silkGetEffect.Spawn(transform2.position, transform2.rotation);
					}
					HeroController.instance.AddSilk(1, heroEffect: false);
				}
				if (silkDropEffect != null)
				{
					Transform transform3 = base.transform;
					silkDropEffect.Spawn(transform3.position, transform3.rotation);
				}
				silkHits--;
				if (silkHits == 1)
				{
					silkDropsCondition.SetActive(value: false);
					if ((bool)threadThinObject)
					{
						threadThinObject.SetActive(value: true);
					}
				}
				else if (silkHits == 0)
				{
					silkDropsCondition.SetActive(value: false);
					if ((bool)threadThinObject)
					{
						threadThinObject.SetActive(value: false);
					}
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		if ((bool)breakEffects)
		{
			breakEffects.SetActive(value: true);
		}
		brokenByNail = damageInstance.IsNailDamage;
		Break(num2, num3, num);
		if ((bool)threadThinObject)
		{
			threadThinObject.SetActive(value: false);
		}
		if (this.BrokenHit != null)
		{
			this.BrokenHit(damageInstance);
		}
		instance.FreezeMoment(breakFreezeMoment);
		return IHitResponder.Response.GenericHit;
	}

	private static void SpawnNailHitEffect(Transform nailHitEffectPrefab, Vector3 position, float impactAngle)
	{
		if (!(nailHitEffectPrefab == null))
		{
			int cardinalDirection = DirectionUtils.GetCardinalDirection(impactAngle);
			float y = 1.5f;
			float minInclusive;
			float maxInclusive;
			switch (cardinalDirection)
			{
			case 3:
				minInclusive = 250f;
				maxInclusive = 290f;
				break;
			case 1:
				minInclusive = 70f;
				maxInclusive = 110f;
				break;
			default:
				minInclusive = 340f;
				maxInclusive = 380f;
				break;
			}
			float x = ((cardinalDirection == 2) ? (-1.5f) : 1.5f);
			Transform obj = nailHitEffectPrefab.Spawn(position);
			Vector3 eulerAngles = obj.eulerAngles;
			eulerAngles.z = UnityEngine.Random.Range(minInclusive, maxInclusive);
			obj.eulerAngles = eulerAngles;
			Vector3 localScale = obj.localScale;
			localScale.x = x;
			localScale.y = y;
			obj.localScale = localScale;
		}
	}

	private bool IsPartVisible(GameObject obj)
	{
		if (!obj)
		{
			return false;
		}
		if (!obj.activeInHierarchy)
		{
			return false;
		}
		SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
		if ((bool)component && !component.enabled)
		{
			return false;
		}
		return true;
	}

	private void SetStaticPartsActivation(bool broken)
	{
		if (!wasAlreadyBroken)
		{
			silkDrops.GetRandomValue();
			if ((bool)silkDropsCondition)
			{
				IsPartVisible(silkDropsCondition);
			}
		}
		if (wholeRenderer != null)
		{
			wholeRenderer.enabled = !broken;
		}
		GameObject[] array = wholeParts;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(!broken);
			}
		}
		array = remnantParts;
		foreach (GameObject gameObject2 in array)
		{
			if (!(gameObject2 == null))
			{
				gameObject2.SetActive(broken);
			}
		}
		if (hitEventReciever != null)
		{
			FSMUtility.SendEventToGameObject(hitEventReciever, "HIT");
		}
		if ((bool)bodyCollider)
		{
			bodyCollider.enabled = !broken;
		}
	}

	public void Break(float flingAngleMin, float flingAngleMax, float impactMultiplier)
	{
		if (isBroken)
		{
			return;
		}
		Vector3 position = base.transform.position;
		SetStaticPartsActivation(broken: true);
		List<GameObject> list = debrisParts;
		if (useAltDebris && (float)UnityEngine.Random.Range(1, 100) <= altDebrisChance)
		{
			list = debrisPartsAlt;
		}
		foreach (GameObject item in list)
		{
			if (!(item == null))
			{
				FlingDebrisPart(item, flingAngleMin, flingAngleMax, impactMultiplier);
			}
		}
		GameObject[] array = flingDrops;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				FlingDebrisPart(gameObject, flingAngleMin, flingAngleMax, impactMultiplier);
			}
		}
		if (containingParticles.Length != 0)
		{
			GameObject gameObject2 = Probability.GetRandomGameObjectByProbability(containingParticles);
			if ((bool)gameObject2)
			{
				if (gameObject2.transform.parent != base.transform)
				{
					gameObject2 = gameObject2.Spawn(position);
				}
				gameObject2.SetActive(value: true);
			}
		}
		FlingObject[] array2 = flingObjectRegister;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Fling(position);
		}
		breakAudioEvent.SpawnAndPlayOneShot(audioSourcePrefab, position);
		breakAudioClipTable.SpawnAndPlayOneShot(audioSourcePrefab, position);
		if (hitEventReciever != null)
		{
			FSMUtility.SendEventToGameObject(hitEventReciever, "HIT");
		}
		if (forwardBreakEvent)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "BREAK");
		}
		if ((bool)breakShake.Camera)
		{
			breakShake.DoShake(this);
		}
		else
		{
			GameObject gameObject3 = GameObject.FindGameObjectWithTag("CameraParent");
			if (gameObject3 != null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject3, "CameraShake");
				if (playMakerFSM != null)
				{
					playMakerFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		float angleMin = (megaFlingGeo ? 65 : 80);
		float angleMax = (megaFlingGeo ? 115 : 100);
		float speedMin = (megaFlingGeo ? 30 : 15);
		float speedMax = (megaFlingGeo ? 45 : 30);
		FlingUtils.Config config = default(FlingUtils.Config);
		config.Prefab = Gameplay.SmallGeoPrefab;
		config.AmountMin = smallGeoDrops.Start;
		config.AmountMax = smallGeoDrops.End;
		config.SpeedMin = speedMin;
		config.SpeedMax = speedMax;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFling(config, base.transform, effectOffset);
		config = default(FlingUtils.Config);
		config.Prefab = Gameplay.MediumGeoPrefab;
		config.AmountMin = mediumGeoDrops.Start;
		config.AmountMax = mediumGeoDrops.End;
		config.SpeedMin = speedMin;
		config.SpeedMax = speedMax;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFling(config, base.transform, effectOffset);
		config = default(FlingUtils.Config);
		config.Prefab = Gameplay.LargeGeoPrefab;
		config.AmountMin = largeGeoDrops.Start;
		config.AmountMax = largeGeoDrops.End;
		config.SpeedMin = speedMin;
		config.SpeedMax = speedMax;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFling(config, base.transform, effectOffset);
		config = default(FlingUtils.Config);
		config.Prefab = Gameplay.LargeSmoothGeoPrefab;
		config.AmountMin = largeSmoothGeoDrops.Start;
		config.AmountMax = largeSmoothGeoDrops.End;
		config.SpeedMin = speedMin;
		config.SpeedMax = speedMax;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFling(config, base.transform, effectOffset);
		config = default(FlingUtils.Config);
		config.Prefab = Gameplay.ShellShardPrefab;
		config.AmountMin = shellShardDrops.Start;
		config.AmountMax = shellShardDrops.End;
		config.SpeedMin = speedMin;
		config.SpeedMax = speedMax;
		config.AngleMin = angleMin;
		config.AngleMax = angleMax;
		FlingUtils.SpawnAndFlingShellShards(config, base.transform, effectOffset);
		ItemDropGroup[] array3 = itemDropGroups;
		foreach (ItemDropGroup itemDropGroup in array3)
		{
			if (itemDropGroup.Drops.Length == 0 || itemDropGroup.TotalProbability < 1f)
			{
				continue;
			}
			int chosenIndex;
			SavedItem randomItemByProbability = Probability.GetRandomItemByProbability<ItemDropProbability, SavedItem>(itemDropGroup.Drops, out chosenIndex);
			if ((bool)randomItemByProbability && randomItemByProbability.CanGetMore() && (bool)Gameplay.CollectableItemPickupPrefab)
			{
				CollectableItemPickup collectableItemPickup = itemDropGroup.Drops[chosenIndex].CustomPickupPrefab;
				if (!collectableItemPickup)
				{
					collectableItemPickup = Gameplay.CollectableItemPickupInstantPrefab;
				}
				CollectableItemPickup collectableItemPickup2 = UnityEngine.Object.Instantiate(collectableItemPickup);
				collectableItemPickup2.transform.SetPosition2D(base.transform.TransformPoint(effectOffset));
				collectableItemPickup2.SetItem(randomItemByProbability);
				FlingUtils.SelfConfig config2 = default(FlingUtils.SelfConfig);
				config2.Object = collectableItemPickup2.gameObject;
				config2.SpeedMin = speedMin;
				config2.SpeedMax = speedMax;
				config2.AngleMin = angleMin;
				config2.AngleMax = angleMax;
				FlingUtils.FlingObject(config2, base.transform, effectOffset);
			}
		}
		if ((bool)bodyCollider)
		{
			bodyCollider.enabled = false;
		}
		isBroken = true;
		if (deparentOnBreak)
		{
			base.transform.parent = null;
		}
		if (onBreak != null)
		{
			onBreak.Invoke();
		}
		NoiseMaker.CreateNoise(base.transform.position, noiseRadius, NoiseMaker.Intensities.Normal);
	}

	private void FlingDebrisPart(GameObject debrisPart, float flingAngleMin, float flingAngleMax, float impactMultiplier)
	{
		Rigidbody2D component = debrisPart.GetComponent<Rigidbody2D>();
		SpinSelf component2 = debrisPart.GetComponent<SpinSelf>();
		SpinSelfSimple component3 = debrisPart.GetComponent<SpinSelfSimple>();
		debrisPart.SetActive(value: true);
		if ((!component || !component.freezeRotation) && !component2 && !component3)
		{
			debrisPart.transform.SetRotationZ(debrisPart.transform.localEulerAngles.z + angleOffset);
		}
		debrisPart.transform.SetParent(null, worldPositionStays: true);
		if (component != null)
		{
			float num = UnityEngine.Random.Range(flingAngleMin, flingAngleMax);
			Vector2 vector = new Vector2(Mathf.Cos(num * (MathF.PI / 180f)), Mathf.Sin(num * (MathF.PI / 180f)));
			float num2 = UnityEngine.Random.Range(flingSpeedMin, flingSpeedMax) * impactMultiplier;
			component.linearVelocity = vector * num2;
		}
	}

	private IEnumerator SilkAddRoutine(int silkToAdd)
	{
		yield return new WaitForSeconds(0.01f);
		HeroController.instance.AddSilk(silkToAdd, heroEffect: true);
		silkAddRoutine = null;
	}

	public void SetHitsToBreak(int hits)
	{
		hitsToBreak = hits;
		hitsLeft = hits;
	}

	public float GetHitCoolDown()
	{
		return hitCoolDown;
	}

	public void SetHitCoolDownDuration(float value)
	{
		hitCoolDown = value;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject);
	}
}
