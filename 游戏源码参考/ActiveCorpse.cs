using System;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

public class ActiveCorpse : CorpseItems, AntRegion.ICheck
{
	public enum Types
	{
		Small = 0,
		Medium = 1,
		Large = 2,
		Object = 3
	}

	private enum State
	{
		InAir = 0,
		Landed = 1,
		BounceFrame = 2,
		WallHitFrame = 3,
		ExplodeAntic = 4,
		SpikeLand = 5
	}

	public Types corpseType;

	public bool noTint;

	public bool noBounce;

	[Space]
	[SerializeField]
	private bool isRecyclable;

	[SerializeField]
	private bool useVisiblity;

	public bool bounceAway;

	public bool useAfterBounceAnim;

	[ModifiableProperty]
	[Conditional("bounceAway", true, false, true)]
	public float bounceAwayTerminateY;

	[ModifiableProperty]
	[Conditional("bounceAway", true, false, true)]
	public GameObject bounceAwayEffect;

	private bool bouncingAway;

	private bool inAirZone;

	private float minBounceSpeed;

	private float minWallSpeed;

	private float maxBounceLaunchVelocity;

	private float bounceFactor;

	[SerializeField]
	protected AudioSource audioPlayerPrefab;

	[SerializeField]
	private bool noVoice;

	[SerializeField]
	private AudioEventRandom startAudioClips;

	[SerializeField]
	private RandomAudioClipTable startVoiceAudioTable;

	[SerializeField]
	private RandomAudioClipTable landAudioTable;

	[SerializeField]
	private RandomAudioClipTable landVoiceAudioTable;

	[SerializeField]
	private float voicePitchOffset;

	[SerializeField]
	private GameObject activateOnLand;

	[Space]
	public bool explodes;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public float explodePause;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public float anticTime;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public ParticleSystem anticParticle;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public GameObject anticObject;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public GameObject explosionObject;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public bool corpseRemains;

	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public bool explodeInstantlyOnSpikes;

	[Space]
	[SerializeField]
	private Vector2 effectPos;

	private const string DEATH_AIR_CLIP = "Death Air";

	private const string DEATH_LAND_CLIP = "Death Land";

	private const string DEATH_LAND_AIR_CLIP = "Death Land Air";

	private const string DEATH_BOUNCE_AWAY_CLIP = "Death BounceAway";

	private const string DEATH_AIR_AFTER_BOUNCE_CLIP = "Death Air AfterBounce";

	private const string DEATH_LAND_AFTER_BOUNCE_CLIP = "Death Land AfterBounce";

	private State state;

	private float timer;

	private float explodeTimer;

	private float forceLandingTimer;

	private float velocityX;

	private float prevVelocityX;

	private float velocityY;

	private float prevVelocityY;

	private bool hasLanded;

	private bool hasPlayedLandVoice;

	private bool didSoftWallHit;

	private bool didActivateOnLand;

	private bool isInert;

	private bool hasStarted;

	private bool hasEnteredWater;

	private bool hasLandedEver;

	private PolygonCollider2D polyCollider;

	private bool hasCollider;

	private AudioSource audio;

	private Rigidbody2D rb;

	private tk2dSprite sprite;

	private MeshRenderer renderer;

	private CorpseLandTint landTint;

	private bool blockAudio;

	private bool queuedBurn;

	private AttackTypes queuedBurnAttackType;

	private NailElements queuedBurnNailElement;

	private GameObject queuedBurnDamageSource;

	private float queuedBurnScale;

	private bool hasQueuedTagDamageTaker;

	private TagDamageTaker queuedTagDamageTaker;

	private List<DamageTag> queuedTagDamage = new List<DamageTag>();

	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	private static Dictionary<GameObject, ActiveCorpse> activeCorpses = new Dictionary<GameObject, ActiveCorpse>();

	private bool colliderTriggerChanged;

	private bool colliderWasTrigger;

	private bool hasVisibility;

	private VisibilityGroup visibility;

	private RigidbodyType2D originalBodyType;

	private bool destroyed;

	private static RaycastHit2D[] buffer = new RaycastHit2D[1];

	private ParticleEffectsLerpEmission spellBurnEffect;

	private double burnEndTime;

	public bool CanEnterAntRegion => rb.bodyType != RigidbodyType2D.Kinematic;

	public static bool TryGetCorpse(GameObject gameObject, out ActiveCorpse corpse)
	{
		return activeCorpses.TryGetValue(gameObject, out corpse);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(effectPos, 0.2f);
	}

	protected override void Awake()
	{
		base.Awake();
		sprite = GetComponent<tk2dSprite>();
		if (sprite == null)
		{
			destroyed = true;
			UnityEngine.Object.Destroy(this);
			return;
		}
		renderer = sprite.GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody2D>();
		polyCollider = GetComponent<PolygonCollider2D>();
		hasCollider = polyCollider;
		originalBodyType = rb.bodyType;
		if (useVisiblity)
		{
			visibility = base.gameObject.AddComponentIfNotPresent<VisibilityGroup>();
			hasVisibility = true;
		}
		audio = GetComponent<AudioSource>();
		activeCorpses[base.gameObject] = this;
		landTint = base.gameObject.AddComponent<CorpseLandTint>();
		if (corpseType == Types.Small)
		{
			minBounceSpeed = 25f;
			minWallSpeed = 14f;
			bounceFactor = 0.5f;
			maxBounceLaunchVelocity = -60f;
		}
		else if (corpseType == Types.Medium)
		{
			minBounceSpeed = 25f;
			minWallSpeed = 15f;
			bounceFactor = 0.45f;
			maxBounceLaunchVelocity = -50f;
		}
		else if (corpseType == Types.Large)
		{
			minBounceSpeed = 0f;
		}
	}

	protected override void Start()
	{
		if (!destroyed)
		{
			base.Start();
			PlayStartAudio();
			hasStarted = true;
		}
	}

	private void OnEnable()
	{
		if (!destroyed && hasStarted)
		{
			PlayStartAudio();
		}
	}

	private void OnDisable()
	{
		if (destroyed)
		{
			return;
		}
		blockAudio = false;
		spellBurnEffect = null;
		queuedBurn = false;
		hasEnteredWater = false;
		rb.bodyType = originalBodyType;
		if (colliderTriggerChanged)
		{
			if (hasCollider)
			{
				polyCollider.isTrigger = colliderWasTrigger;
			}
			colliderTriggerChanged = false;
		}
		if (isRecyclable)
		{
			state = State.InAir;
			hasLandedEver = false;
			bouncingAway = false;
		}
	}

	private void OnDestroy()
	{
		activeCorpses.Remove(base.gameObject);
	}

	private void Update()
	{
		if (queuedBurn)
		{
			DoQueuedBurnEffects();
		}
		if (!hasCollider)
		{
			return;
		}
		if (state == State.InAir)
		{
			float num = velocityY;
			if (num >= 0.1f || num <= -0.1f)
			{
				prevVelocityY = velocityY;
			}
			Vector2 linearVelocity = rb.linearVelocity;
			velocityY = linearVelocity.y;
			prevVelocityX = velocityX;
			velocityX = linearVelocity.x;
			num = velocityY;
			if (num < 0.1f && num > -0.1f)
			{
				Vector3 position = base.transform.position;
				Bounds bounds = polyCollider.bounds;
				Vector3 vector = new Vector3(position.x - 0.3f, bounds.min.y + 0.2f, position.z);
				Vector3 vector2 = new Vector3(position.x + 0.3f, bounds.min.y + 0.2f, position.z);
				RaycastHit2D raycastHit2D = Helper.Raycast2D(vector, -Vector2.up, 1f, 256);
				RaycastHit2D raycastHit2D2 = Helper.Raycast2D(vector2, -Vector2.up, 1f, 256);
				if (raycastHit2D.collider != null || raycastHit2D2.collider != null)
				{
					if (Math.Abs(minBounceSpeed) > Mathf.Epsilon && prevVelocityY <= 0f - minBounceSpeed && !noBounce)
					{
						Bounce();
					}
					else if (bounceAway)
					{
						Bounce();
					}
					else
					{
						Land();
					}
				}
				forceLandingTimer += Time.deltaTime;
				if (forceLandingTimer > 0.25f)
				{
					Land();
				}
			}
			else
			{
				forceLandingTimer = 0f;
			}
			num = velocityX;
			if (num < 0.1f && num > -0.1f && state == State.InAir)
			{
				if (Mathf.Abs(prevVelocityX) > minWallSpeed && !noBounce)
				{
					WallHit();
				}
				else if (Mathf.Abs(prevVelocityX) > 0.2f && !didSoftWallHit)
				{
					WallHitSoft();
				}
			}
			if (bouncingAway && hasVisibility && !visibility.IsVisible)
			{
				Disable();
			}
			else if (bounceAway && base.transform.position.y < bounceAwayTerminateY)
			{
				if ((bool)bounceAwayEffect)
				{
					bounceAwayEffect.SetActive(value: true);
					bounceAwayEffect.transform.parent = null;
				}
				Disable();
			}
		}
		else if (state == State.Landed)
		{
			velocityY = rb.linearVelocity.y;
			if (velocityY < -0.1f || velocityY > 0.5f)
			{
				explodeTimer = 0f;
				if (!isInert)
				{
					Fall();
				}
			}
			if (explodes)
			{
				if (explodeTimer < explodePause)
				{
					explodeTimer += Time.deltaTime;
				}
				else if (Math.Abs(anticTime) < Mathf.Epsilon)
				{
					Explode();
				}
				else
				{
					StartExplodeAntic();
				}
			}
		}
		else if (state == State.BounceFrame)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				BounceFling();
			}
		}
		else if (state == State.WallHitFrame)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				WallFling();
			}
		}
		else if (state == State.ExplodeAntic)
		{
			if (explodeTimer < anticTime)
			{
				explodeTimer += Time.deltaTime;
			}
			else
			{
				Explode();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (destroyed || state != 0 || collision.gameObject.layer != 17 || !(rb.linearVelocity.y <= -0.1f))
		{
			return;
		}
		DamageHero component = collision.gameObject.GetComponent<DamageHero>();
		if (component != null && component.hazardType == HazardType.SPIKES && !component.noCorpseSpikeStick)
		{
			if (!explodeInstantlyOnSpikes)
			{
				SpikeLand();
			}
			else
			{
				Explode();
			}
		}
	}

	public void SetBlockAudio(bool blockAudio)
	{
		this.blockAudio = blockAudio;
	}

	public void SetOnGround()
	{
		Vector3 position = base.transform.position;
		Bounds bounds = polyCollider.bounds;
		if (Physics2D.RaycastNonAlloc(new Vector3(position.x - 0.3f, bounds.min.y + 0.2f, position.z), Vector2.down, buffer, 200f, 256) > 0)
		{
			RaycastHit2D raycastHit2D = buffer[0];
			position.y -= raycastHit2D.distance - 1f;
			base.transform.position = position;
		}
		else if (Physics2D.RaycastNonAlloc(new Vector3(position.x + 0.3f, bounds.min.y + 0.2f, position.z), Vector2.down, buffer, 200f, 256) > 0)
		{
			RaycastHit2D raycastHit2D2 = buffer[0];
			position.y -= raycastHit2D2.distance - 1f;
			base.transform.position = position;
		}
	}

	private void PlayStartAudio()
	{
		if (blockAudio)
		{
			blockAudio = false;
			return;
		}
		if (!noVoice && (bool)startVoiceAudioTable)
		{
			if ((bool)audio)
			{
				audio.Stop();
				audio.clip = startVoiceAudioTable.SelectClip();
				audio.pitch = startVoiceAudioTable.SelectPitch();
				audio.volume = startVoiceAudioTable.SelectVolume();
				audio.loop = false;
				audio.Play();
			}
			else
			{
				startVoiceAudioTable.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			}
		}
		startAudioClips.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		if (HasAnimator)
		{
			Animator.Play("Death Air");
		}
	}

	private void Land()
	{
		hasLandedEver = true;
		if (HasAnimator)
		{
			if (inAirZone)
			{
				Animator.Play((Animator.GetClipByName("Death Land Air") != null) ? "Death Land Air" : "Death Land");
			}
			else
			{
				Animator.Play((hasLandedEver && useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
			}
		}
		if (!hasEnteredWater)
		{
			landAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		if (!noVoice && (bool)landVoiceAudioTable && !hasPlayedLandVoice && !hasEnteredWater)
		{
			if ((bool)audio)
			{
				audio.Stop();
				audio.clip = landVoiceAudioTable.SelectClip();
				audio.pitch = landVoiceAudioTable.SelectPitch();
				audio.volume = landVoiceAudioTable.SelectVolume();
				audio.loop = false;
				audio.Play();
			}
			else
			{
				landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			hasPlayedLandVoice = true;
		}
		state = State.Landed;
		if ((bool)activateOnLand && !didActivateOnLand)
		{
			activateOnLand.SetActive(value: true);
			didActivateOnLand = true;
		}
		if (!hasLanded)
		{
			hasLanded = true;
			ActivatePickup();
		}
		if (!noTint)
		{
			landTint.Landed(desaturate: false);
		}
		Vector2 linearVelocity = rb.linearVelocity;
		linearVelocity.x = 0f;
		rb.linearVelocity = linearVelocity;
	}

	private void Fall()
	{
		if (HasAnimator)
		{
			if (!bouncingAway)
			{
				Animator.PlayFromFrame("Death Air", 0);
				Animator.PlayFromFrame((hasLandedEver && useAfterBounceAnim) ? "Death Air AfterBounce" : "Death Air", 0);
			}
			else
			{
				Animator.PlayFromFrame((Animator.GetClipByName("Death BounceAway") != null) ? "Death BounceAway" : "Death Air", 0);
			}
		}
		state = State.InAir;
		if (hasLanded)
		{
			hasLanded = false;
			DeactivatePickup();
		}
	}

	private void Bounce()
	{
		hasLandedEver = true;
		if (explodes && (!HasAnimator || Animator.GetClipByName("Death Land") == null))
		{
			Explode();
			return;
		}
		rb.linearVelocity = new Vector2(0f, 0f);
		rb.bodyType = RigidbodyType2D.Kinematic;
		if (HasAnimator)
		{
			Animator.Play((hasLandedEver && useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
		}
		if (!hasEnteredWater)
		{
			landAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		if (!noVoice && !hasPlayedLandVoice && !hasEnteredWater && (bool)landVoiceAudioTable)
		{
			if ((bool)audio)
			{
				audio.Stop();
				audio.clip = landVoiceAudioTable.SelectClip();
				audio.pitch = landVoiceAudioTable.SelectPitch();
				audio.volume = landVoiceAudioTable.SelectVolume();
				audio.loop = false;
				audio.Play();
			}
			else
			{
				landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			hasPlayedLandVoice = true;
		}
		timer = 0.08f;
		if ((bool)activateOnLand && !didActivateOnLand)
		{
			activateOnLand.SetActive(value: true);
			activateOnLand.transform.parent = null;
			didActivateOnLand = true;
		}
		state = State.BounceFrame;
	}

	private void BounceFling()
	{
		base.transform.Translate(0f, 0.2f, 0f);
		if (bounceAway && prevVelocityY > -25f)
		{
			prevVelocityY = -25f;
		}
		if (prevVelocityY < maxBounceLaunchVelocity)
		{
			prevVelocityY = maxBounceLaunchVelocity;
		}
		rb.linearVelocity = new Vector2(prevVelocityX * bounceFactor, 0f - prevVelocityY * bounceFactor);
		rb.bodyType = RigidbodyType2D.Dynamic;
		prevVelocityY = 0f;
		if (bounceAway)
		{
			SetColliderTrigger();
			bouncingAway = true;
		}
		Fall();
	}

	private void WallHit()
	{
		rb.linearVelocity = new Vector2(0f, 0f);
		rb.bodyType = RigidbodyType2D.Kinematic;
		if (HasAnimator)
		{
			Animator.Play("Death Land");
			Animator.Stop();
		}
		if (!hasEnteredWater)
		{
			landAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		timer = 0.08f;
		state = State.WallHitFrame;
	}

	private void WallHitSoft()
	{
		rb.linearVelocity = new Vector2(prevVelocityX * (0f - bounceFactor) / 2f, 0f);
		prevVelocityX = 0f;
		didSoftWallHit = true;
	}

	private void WallFling()
	{
		rb.linearVelocity = new Vector2(prevVelocityX * (0f - bounceFactor) / 1.75f, 0f);
		rb.bodyType = RigidbodyType2D.Dynamic;
		base.transform.localEulerAngles = Vector3.zero;
		prevVelocityY = 0f;
		prevVelocityX = 0f;
		Fall();
	}

	private void SpikeLand()
	{
		base.transform.Translate(0f, -0.25f, 0f);
		if (HasAnimator)
		{
			Animator.Play((hasLandedEver && useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
		}
		if (corpseType == Types.Object)
		{
			Audio.ObjectSpikeLandAudioTable.SpawnAndPlayOneShot(base.transform.position);
			Transform obj = base.transform;
			Vector3 position = obj.position;
			obj.position = new Vector3(position.x, position.y - 0.5f, position.z);
		}
		else
		{
			Audio.CorpseSpikeLandAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		rb.linearVelocity = new Vector2(0f, 0f);
		rb.angularVelocity = 0f;
		rb.bodyType = RigidbodyType2D.Kinematic;
		if (!noVoice && (bool)landVoiceAudioTable && !hasPlayedLandVoice)
		{
			if ((bool)audio)
			{
				audio.Stop();
				landVoiceAudioTable.PlayOneShot(audio, voicePitchOffset);
			}
			else
			{
				landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			hasPlayedLandVoice = true;
		}
		JitterSelfForTime jitterSelfForTime = base.gameObject.GetComponent<JitterSelfForTime>();
		if (!jitterSelfForTime)
		{
			jitterSelfForTime = JitterSelfForTime.AddHandler(base.gameObject, new Vector3(0.075f, 0.075f), 0.25f, 30f);
		}
		jitterSelfForTime.StartTimedJitter();
		if (!noTint)
		{
			landTint.Landed(desaturate: false);
		}
		state = State.SpikeLand;
	}

	private void StartExplodeAntic()
	{
		explodeTimer = 0f;
		if ((bool)anticParticle)
		{
			anticParticle.Play();
		}
		if ((bool)anticObject)
		{
			anticObject.SetActive(value: true);
		}
		state = State.ExplodeAntic;
	}

	private void Explode()
	{
		if ((bool)anticParticle)
		{
			anticParticle.Stop();
			anticParticle.gameObject.transform.parent = null;
		}
		if ((bool)anticObject)
		{
			anticObject.SetActive(value: false);
		}
		if ((bool)explosionObject)
		{
			explosionObject.SetActive(value: true);
			explosionObject.transform.parent = null;
		}
		if (!corpseRemains)
		{
			Disable();
		}
	}

	public void SetInAirZone(bool isInAirZone)
	{
		inAirZone = isInAirZone;
	}

	public void SetInert(bool setIsInert)
	{
		isInert = setIsInert;
	}

	public void SetNoTint(bool setNoTint)
	{
		noTint = setNoTint;
	}

	public void SetBounceAway(bool setBounceAway)
	{
		bounceAway = setBounceAway;
	}

	public void QueueBurnEffects(tk2dSprite enemySprite, AttackTypes attackType, NailElements nailElement, GameObject damageSource, float scale, TagDamageTaker enemyTagDamageTaker)
	{
		queuedBurn = true;
		queuedBurnAttackType = attackType;
		queuedBurnNailElement = nailElement;
		queuedBurnDamageSource = damageSource;
		queuedBurnScale = scale;
		queuedTagDamageTaker = enemyTagDamageTaker;
		hasQueuedTagDamageTaker = queuedTagDamageTaker != null;
		if (hasQueuedTagDamageTaker)
		{
			queuedTagDamage.Clear();
			queuedTagDamage.AddRange(queuedTagDamageTaker.TaggedDamage.Keys);
		}
		if ((bool)enemySprite && enemySprite.HasKeywordsDefined && (bool)sprite)
		{
			Color color = enemySprite.color * sprite.color.grayscale;
			color.a = sprite.color.a;
			sprite.color = color;
			enemySprite.MoveKeywords(sprite);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			enemySprite.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
			float @float = materialPropertyBlock.GetFloat(_blackThreadAmountProp);
			materialPropertyBlock.Clear();
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(_blackThreadAmountProp, @float);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	private void DoQueuedBurnEffects()
	{
		queuedBurn = false;
		AttackTypes attackTypes = queuedBurnAttackType;
		queuedBurnAttackType = AttackTypes.Nail;
		NailElements nailElements = queuedBurnNailElement;
		queuedBurnNailElement = NailElements.None;
		GameObject gameObject = queuedBurnDamageSource;
		queuedBurnDamageSource = null;
		float scale = queuedBurnScale;
		queuedBurnScale = 0f;
		Transform transform = base.transform;
		bool flag = false;
		if ((bool)queuedTagDamageTaker)
		{
			foreach (KeyValuePair<DamageTag, DamageTagInfo> item in queuedTagDamageTaker.TaggedDamage)
			{
				ParticleEffectsLerpEmission corpseBurnEffect = item.Key.CorpseBurnEffect;
				if ((bool)corpseBurnEffect)
				{
					item.Key.SpawnDeathEffects(transform.transform.position);
					DoBurnEffect(transform, effectPos, corpseBurnEffect, scale);
					if (item.Key.NailElement == NailElements.Fire || item.Key.SpecialDamageType == DamageTag.SpecialDamageTypes.Lightning)
					{
						flag = true;
					}
				}
			}
			queuedTagDamageTaker = null;
		}
		else if (hasQueuedTagDamageTaker)
		{
			foreach (DamageTag item2 in queuedTagDamage)
			{
				ParticleEffectsLerpEmission corpseBurnEffect2 = item2.CorpseBurnEffect;
				if ((bool)corpseBurnEffect2)
				{
					if (item2.NailElement == NailElements.Fire || item2.SpecialDamageType == DamageTag.SpecialDamageTypes.Lightning)
					{
						flag = true;
					}
					item2.SpawnDeathEffects(transform.transform.position);
					DoBurnEffect(transform, effectPos, corpseBurnEffect2, scale);
					break;
				}
			}
			queuedTagDamage.Clear();
		}
		hasQueuedTagDamageTaker = false;
		if (!flag)
		{
			if (attackTypes == AttackTypes.Spell)
			{
				flag = true;
				DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.SpellBurnEffect, scale);
			}
			else if (attackTypes == AttackTypes.Fire || attackTypes == AttackTypes.Explosion || nailElements == NailElements.Fire)
			{
				flag = true;
				DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.FireBurnEffect, scale, isFire: true);
			}
			else if (attackTypes == AttackTypes.Lightning)
			{
				flag = true;
				DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.ZapBurnEffect, scale);
			}
			else if ((bool)gameObject)
			{
				if (gameObject.CompareTag("Explosion"))
				{
					flag = true;
					DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.FireBurnEffect, scale);
				}
				else
				{
					DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
					if ((bool)component)
					{
						ToolItem representingTool = component.RepresentingTool;
						if ((bool)representingTool && representingTool.Type == ToolItemType.Skill && Gameplay.SpellCrest.IsEquipped)
						{
							flag = true;
							DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.SoulBurnEffect, scale);
						}
						else if (component.ZapDamageTicks > 0)
						{
							flag = true;
							DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.ZapBurnEffect, scale);
						}
						else if (component.PoisonDamageTicks > 0)
						{
							DoBurnEffect(transform, effectPos, GlobalSettings.Corpse.PoisonBurnEffect, scale);
						}
					}
				}
			}
		}
		if (flag && sprite != null)
		{
			if (base.IsBlackThreaded)
			{
				sprite.color *= GlobalSettings.Corpse.SpellBurnColorBlackThread;
			}
			else
			{
				sprite.color *= GlobalSettings.Corpse.SpellBurnColor;
			}
		}
	}

	private void DoBurnEffect(Transform corpseTrans, Vector2 localPos, ParticleEffectsLerpEmission spellBurnEffect, float scale, bool isFire = false)
	{
		float spellBurnDuration = GlobalSettings.Corpse.SpellBurnDuration;
		if ((bool)spellBurnEffect && !(spellBurnDuration < 0f))
		{
			ParticleEffectsLerpEmission particleEffectsLerpEmission = spellBurnEffect.Spawn(corpseTrans);
			Transform transform = particleEffectsLerpEmission.transform;
			transform.SetLocalPosition2D(localPos);
			if (isFire)
			{
				RecordFireBurn(particleEffectsLerpEmission, spellBurnDuration);
			}
			FollowTransform component = particleEffectsLerpEmission.GetComponent<FollowTransform>();
			if ((bool)component)
			{
				transform.SetParent(null, worldPositionStays: true);
				transform.SetRotation2D(spellBurnEffect.transform.localEulerAngles.z);
				component.Target = corpseTrans;
				component.ClearTargetOnDisable();
			}
			ParticleEffectsScaleToCollider component2 = particleEffectsLerpEmission.GetComponent<ParticleEffectsScaleToCollider>();
			Collider2D component3 = corpseTrans.GetComponent<Collider2D>();
			if ((bool)component2 && (bool)component3)
			{
				component2.SetScaleToCollider(component3);
			}
			else
			{
				particleEffectsLerpEmission.TotalMultiplier = scale;
				Vector3 localScale = spellBurnEffect.transform.localScale;
				localScale.x *= scale;
				localScale.y *= scale;
				transform.localScale = localScale;
			}
			particleEffectsLerpEmission.Play(spellBurnDuration);
		}
	}

	private void RecordFireBurn(ParticleEffectsLerpEmission spellBurnEffect, float duration)
	{
		this.spellBurnEffect = spellBurnEffect;
		burnEndTime = Time.timeAsDouble + (double)duration;
	}

	public void Acid()
	{
		hasEnteredWater = true;
		SetColliderTrigger();
		if (state == State.Landed)
		{
			state = State.InAir;
		}
		if (Time.timeAsDouble < burnEndTime && spellBurnEffect != null)
		{
			burnEndTime = 0.0;
			spellBurnEffect.Stop();
			ExtinguishEffectSpawner component = spellBurnEffect.GetComponent<ExtinguishEffectSpawner>();
			if (component != null)
			{
				component.PlayEffect(base.transform.position);
			}
			spellBurnEffect = null;
		}
	}

	private void SetColliderTrigger()
	{
		if (hasCollider && !polyCollider.isTrigger)
		{
			if (!colliderTriggerChanged)
			{
				colliderWasTrigger = polyCollider.isTrigger;
			}
			polyCollider.isTrigger = true;
			colliderTriggerChanged = true;
		}
	}

	private void Disable()
	{
		if (isRecyclable)
		{
			base.gameObject.Recycle();
			SetIsBlackThreaded(isThreaded: false);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
