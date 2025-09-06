using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class ClockworkHatchling : MonoBehaviour
{
	[Serializable]
	public struct TypeDetails
	{
		public int damage;

		public AudioEvent birthSound;

		public Color spatterColor;

		public Transform groundPoint;

		public string attackAnim;

		public string flyAnim;

		public string hatchAnim;

		public string teleEndAnim;

		public string teleStartAnim;

		public string turnAttackAnim;

		public string turnFlyAnim;

		public string restStartAnim;

		public string restEndAnim;

		public string singAnim;
	}

	public enum State
	{
		None = 0,
		Follow = 1,
		Aggro = 2,
		Tele = 3,
		Attack = 4,
		BenchRestStart = 5,
		BenchRestLower = 6,
		BenchResting = 7,
		Recoil = 8,
		Sing = 9
	}

	private const float NEEDOLIN_REACT_MIN = 0.75f;

	private const float NEEDOLIN_REACT_MAX = 1f;

	[SerializeField]
	private ToolItem toolSource;

	public TypeDetails normalDetails = new TypeDetails
	{
		attackAnim = "Attack",
		flyAnim = "Fly",
		hatchAnim = "Hatch",
		teleEndAnim = "Tele End",
		teleStartAnim = "Tele Start",
		turnAttackAnim = "TurnToAttack",
		turnFlyAnim = "TurnToFly",
		restStartAnim = "Rest Start",
		restEndAnim = "Rest End",
		singAnim = "Sing",
		spatterColor = Color.black
	};

	[SerializeField]
	private TriggerEnterEvent enemyRange;

	[SerializeField]
	private TriggerEnterEvent groundRange;

	[SerializeField]
	private Collider2D terrainCollider;

	[SerializeField]
	private ParticleSystem hitParticles;

	[SerializeField]
	private ParticleSystem deathParticles;

	[SerializeField]
	private ParticleSystem damagedParticles;

	[SerializeField]
	private AudioSource damagedAudioLoop;

	[SerializeField]
	private AudioClip[] loopClips;

	[SerializeField]
	private int hp;

	[SerializeField]
	private RandomAudioClipTable attackChargeAudioTable;

	[SerializeField]
	private RandomAudioClipTable activateAudioTable;

	[SerializeField]
	private RandomAudioClipTable throwAudioTable;

	[SerializeField]
	private RandomAudioClipTable breakAudioTable;

	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private AudioEvent explodeSound = new AudioEvent
	{
		PitchMin = 0.85f,
		PitchMax = 1.15f,
		Volume = 1f
	};

	[SerializeField]
	private GameObject openEffect;

	[SerializeField]
	private float attackCooldown;

	[SerializeField]
	private DamageEnemies damager;

	[SerializeField]
	private GameObject pinPrefab;

	[SerializeField]
	private GameObject clearEffectsDummyPrefab;

	[SerializeField]
	private ParticleSystem ptPoisonTrail;

	[SerializeField]
	private Color particleColourDefault;

	[SerializeField]
	private ToolItem representingTool;

	[SerializeField]
	private float blockedResetDelay = 0.1f;

	[Header("Audio")]
	[SerializeField]
	private AudioSource voiceAudioSource;

	[SerializeField]
	private RandomAudioClipTable spawnVoiceTable;

	[SerializeField]
	private RandomAudioClipTable attackVoiceTable;

	[SerializeField]
	private RandomAudioClipTable turnVoiceTable;

	[SerializeField]
	private RandomAudioClipTable singVoiceTable;

	private readonly List<Collider2D> groundColliders = new List<Collider2D>();

	private GameObject target;

	private TypeDetails details;

	private State currentState;

	private int hpCurrent;

	private float targetRadius;

	private Vector3 offset;

	private float awayTimer;

	private double attackFinishTime;

	private double benchRestWaitTime;

	private bool quickSpawn;

	private bool dreamSpawn;

	private float startZ;

	private float sleepZ;

	private AudioSource audioSource;

	private MeshRenderer meshRenderer;

	private tk2dSprite sprite;

	private tk2dSpriteAnimator animator;

	private Rigidbody2D body;

	private Collider2D col;

	private SpriteFlash spriteFlash;

	private float cooldownTimer;

	[Space]
	public GameObject lastTargetDamaged;

	private bool needolinPlaying;

	private float needolinReactTime;

	private float needolinReactTimer;

	private bool isOnBench;

	private Coroutine hitBlockedRoutine;

	private int hitState;

	private int blockHitState;

	private int hitCount;

	private Coroutine hitLandedRoutine;

	private Coroutine spawnRoutine;

	private bool hasVoiceSource;

	private bool hasSingSource;

	private bool canDamage;

	private bool hasToolSource;

	private bool isClearingEffects;

	private float pauseTimer;

	private float xScale;

	private float startX;

	private float startY;

	private float accelY;

	private float accelX;

	private float waitTime;

	public State LastFrameState { get; private set; }

	public State PreviousState { get; private set; }

	public State CurrentState
	{
		get
		{
			return currentState;
		}
		private set
		{
			if (currentState != value)
			{
				PreviousState = currentState;
				if (PreviousState == State.Sing)
				{
					ToggleSingLoop(active: false);
				}
			}
			currentState = value;
		}
	}

	public bool IsGrounded => groundColliders.Count > 0;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		meshRenderer = GetComponent<MeshRenderer>();
		sprite = GetComponent<tk2dSprite>();
		animator = GetComponent<tk2dSpriteAnimator>();
		body = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();
		spriteFlash = GetComponent<SpriteFlash>();
		hasVoiceSource = voiceAudioSource != null;
		hasToolSource = toolSource;
	}

	private void Start()
	{
		if ((bool)enemyRange)
		{
			enemyRange.OnTriggerStayed += delegate(Collider2D collision, GameObject _)
			{
				if (canDamage)
				{
					if (!target)
					{
						if (collision.CompareTag("Hatchling Magnet"))
						{
							target = collision.gameObject;
						}
						else if (!collision.CompareTag("Ignore Hatchling") && !collision.CompareTag("FloorDisturber") && !collision.CompareTag("Enemy Range Exclude") && Physics2D.Linecast(base.transform.position, collision.transform.position, LayerMask.GetMask("Terrain", "Soft Terrain")).collider == null)
						{
							target = collision.gameObject;
						}
					}
					if (CurrentState == State.Follow && (bool)target && cooldownTimer <= 0f)
					{
						CurrentState = State.Attack;
					}
				}
			};
			enemyRange.OnTriggerExited += delegate(Collider2D collision, GameObject _)
			{
				if (CurrentState != State.Attack && (bool)target && target == collision.gameObject)
				{
					target = null;
				}
			};
		}
		if ((bool)groundRange)
		{
			groundRange.OnTriggerEntered += delegate(Collider2D collision, GameObject _)
			{
				if (!groundColliders.Contains(collision))
				{
					groundColliders.Add(collision);
				}
			};
			groundRange.OnTriggerExited += delegate(Collider2D collision, GameObject _)
			{
				if (groundColliders.Contains(collision))
				{
					groundColliders.Remove(collision);
				}
			};
		}
		startZ = UnityEngine.Random.Range(0.0041f, 0.0049f);
		sleepZ = UnityEngine.Random.Range(0.003f, 0.0035f);
		base.transform.SetPositionZ(startZ);
		CheckPoison();
	}

	private void OnEnable()
	{
		DoEnableReset();
		HeroPerformanceRegion.StartedPerforming += OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming += OnStoppedNeedolin;
		ToolItemManager.OnEquippedStateChanged += OnEquippedStateChanged;
		HeroController instance = HeroController.instance;
		instance.preHeroInPosition += OnPreHeroInPosition;
		instance.heroInPosition += OnHeroInPosition;
		instance.OnDeath += Break;
		instance.HeroLeavingScene += OnHeroLeavingScene;
		canDamage = instance.isHeroInPosition;
	}

	private void OnDisable()
	{
		DoDisableReset();
		isOnBench = false;
		isClearingEffects = false;
		HeroPerformanceRegion.StartedPerforming -= OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming -= OnStoppedNeedolin;
		ToolItemManager.OnEquippedStateChanged -= OnEquippedStateChanged;
		HeroController unsafeInstance = HeroController.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.preHeroInPosition -= OnPreHeroInPosition;
			unsafeInstance.heroInPosition -= OnHeroInPosition;
			unsafeInstance.OnDeath -= Break;
			unsafeInstance.HeroLeavingScene -= OnHeroLeavingScene;
		}
		lastTargetDamaged = null;
	}

	private void DoEnableReset()
	{
		hpCurrent = hp;
		details = normalDetails;
		if ((bool)audioSource)
		{
			audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
			if (loopClips.Length != 0)
			{
				audioSource.clip = loopClips[UnityEngine.Random.Range(0, loopClips.Length)];
			}
		}
		if ((bool)enemyRange)
		{
			enemyRange.gameObject.SetActive(value: false);
		}
		if ((bool)groundRange)
		{
			groundRange.gameObject.SetActive(value: true);
		}
		if ((bool)col)
		{
			col.enabled = false;
		}
		groundColliders.Clear();
		hitParticles.Stop();
		deathParticles.Stop();
		damagedParticles.Stop();
		damagedAudioLoop.Stop();
		target = null;
		LastFrameState = State.None;
		CurrentState = State.None;
		if ((bool)terrainCollider)
		{
			terrainCollider.enabled = true;
		}
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		cooldownTimer = attackCooldown * UnityEngine.Random.Range(0.8f, 1.2f);
		ResetNeedolinTimer();
		if (spawnRoutine != null)
		{
			StopCoroutine(spawnRoutine);
		}
		spawnRoutine = StartCoroutine(Spawn());
		CheckPoison();
	}

	private void DoDisableReset()
	{
		hitLandedRoutine = null;
		hitBlockedRoutine = null;
		quickSpawn = false;
		dreamSpawn = false;
	}

	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			Color poisonPouchTintColour = Gameplay.PoisonPouchTintColour;
			if ((bool)representingTool)
			{
				sprite.EnableKeyword("CAN_HUESHIFT");
				sprite.SetFloat(PoisonTintBase.HueShiftPropId, representingTool.PoisonHueShift);
				damager.OverridePoisonDamage(representingTool.PoisonDamageTicks);
			}
			else
			{
				sprite.EnableKeyword("RECOLOUR");
				sprite.color = poisonPouchTintColour;
			}
			ParticleSystem.MainModule main = hitParticles.main;
			main.startColor = poisonPouchTintColour;
			ParticleSystem.MainModule main2 = damagedParticles.main;
			main2.startColor = poisonPouchTintColour;
			ptPoisonTrail.Play();
		}
		else
		{
			sprite.DisableKeyword("CAN_HUESHIFT");
			sprite.DisableKeyword("RECOLOUR");
			sprite.color = Color.white;
			ParticleSystem.MainModule main3 = hitParticles.main;
			main3.startColor = particleColourDefault;
			ParticleSystem.MainModule main4 = damagedParticles.main;
			main4.startColor = particleColourDefault;
			damager.OverridePoisonDamage(0);
		}
	}

	private void Update()
	{
		if (cooldownTimer > 0f)
		{
			cooldownTimer -= Time.deltaTime;
		}
		if (needolinPlaying && IsInNeedolinRange() && CurrentState != State.Sing)
		{
			needolinReactTimer += Time.deltaTime;
			if (needolinReactTimer >= needolinReactTime)
			{
				CurrentState = State.Sing;
				animator.Play(details.singAnim);
				body.linearVelocity = new Vector2(body.linearVelocity.x, 4f);
				ResetNeedolinTimer();
				ToggleSingLoop(active: true);
			}
		}
		else if ((!needolinPlaying || !IsInNeedolinRange()) && CurrentState == State.Sing)
		{
			needolinReactTimer += Time.deltaTime;
			if (needolinReactTimer >= needolinReactTime)
			{
				if (isOnBench)
				{
					CurrentState = State.BenchRestStart;
				}
				else
				{
					StartCoroutine(WakeUp());
				}
				ResetNeedolinTimer();
			}
		}
		else if (needolinReactTimer > 0f)
		{
			needolinReactTimer = 0f;
		}
	}

	private void FixedUpdate()
	{
		State state = CurrentState;
		switch (CurrentState)
		{
		case State.Follow:
		{
			if (LastFrameState != State.Follow)
			{
				if ((bool)col)
				{
					col.enabled = true;
				}
				if ((bool)enemyRange)
				{
					enemyRange.gameObject.SetActive(value: true);
				}
				if ((bool)animator)
				{
					animator.Play(details.flyAnim);
				}
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
				body.isKinematic = false;
				targetRadius = UnityEngine.Random.Range(0.1f, 0.75f);
				offset = new Vector3(UnityEngine.Random.Range(-3.5f, 3.5f), UnityEngine.Random.Range(1.25f, 3f));
				awayTimer = 0f;
				base.transform.SetPositionZ(startZ);
			}
			float heroDistance = GetHeroDistance();
			float speedMax = Mathf.Clamp(heroDistance + 4f, 4f, 18f);
			DoFace(spriteFacesRight: false, playNewAnimation: true, details.turnFlyAnim, pauseBetweenTurns: true, 0.5f);
			DoChase(HeroController.instance.transform, 2f, speedMax, 40f, targetRadius, 0.9f, offset);
			DoBuzz(0.75f, 1f, 18f, 80f, 110f, new Vector2(50f, 50f));
			if (heroDistance * 1.15f > 10f)
			{
				awayTimer += Time.fixedDeltaTime;
				if (awayTimer >= 1f)
				{
					state = State.Tele;
				}
			}
			else
			{
				awayTimer = 0f;
			}
			break;
		}
		case State.Aggro:
		{
			if (lastTargetDamaged == null)
			{
				state = State.Follow;
			}
			if (LastFrameState != State.Aggro)
			{
				if ((bool)col)
				{
					col.enabled = true;
				}
				if ((bool)enemyRange)
				{
					enemyRange.gameObject.SetActive(value: true);
				}
				if ((bool)animator)
				{
					animator.Play(details.flyAnim);
				}
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
				body.isKinematic = false;
				targetRadius = UnityEngine.Random.Range(0.1f, 0.75f);
				offset = new Vector3(UnityEngine.Random.Range(-3.5f, 3.5f), UnityEngine.Random.Range(1.25f, 3f));
				awayTimer = 0f;
				base.transform.SetPositionZ(startZ);
			}
			float heroDistance2 = GetHeroDistance();
			float speedMax2 = Mathf.Clamp(heroDistance2 + 4f, 4f, 18f);
			DoFace(spriteFacesRight: false, playNewAnimation: true, details.turnFlyAnim, pauseBetweenTurns: true, 0.5f);
			DoChase(lastTargetDamaged.transform, 2f, speedMax2, 40f, targetRadius, 0.9f, offset);
			DoBuzz(0.75f, 1f, 18f, 80f, 110f, new Vector2(50f, 50f));
			if (heroDistance2 * 1.15f > 10f)
			{
				awayTimer += Time.fixedDeltaTime;
				if (awayTimer >= 1f)
				{
					state = State.Tele;
				}
			}
			else
			{
				awayTimer = 0f;
			}
			break;
		}
		case State.Tele:
			if (LastFrameState != State.Tele)
			{
				if ((bool)audioSource)
				{
					audioSource.Stop();
				}
				if ((bool)animator)
				{
					animator.Play(details.teleStartAnim);
				}
				if ((bool)enemyRange)
				{
					enemyRange.gameObject.SetActive(value: false);
				}
				if ((bool)groundRange)
				{
					groundRange.gameObject.SetActive(value: false);
				}
				if ((bool)terrainCollider)
				{
					terrainCollider.enabled = false;
				}
				if ((bool)attackChargeAudioTable)
				{
					attackChargeAudioTable.SpawnAndPlayOneShot(base.transform.position);
				}
			}
			DoChase(HeroController.instance.transform, 2f, 35f, 400f, 0f, 0f, new Vector2(0f, 0.5f));
			DoFace(spriteFacesRight: false, playNewAnimation: false, details.turnFlyAnim, pauseBetweenTurns: true, 0.5f);
			if (GetHeroDistance() < 2f)
			{
				state = State.None;
				StartCoroutine(TeleEnd());
			}
			break;
		case State.Attack:
			damager.gameObject.SetActive(value: true);
			if (LastFrameState != State.Attack)
			{
				if ((bool)audioSource)
				{
					audioSource.Stop();
					if ((bool)attackChargeAudioTable)
					{
						attackChargeAudioTable.SpawnAndPlayOneShot(base.transform.position);
					}
				}
				PlayAttackVoice();
				if ((bool)animator)
				{
					animator.Play(details.attackAnim);
				}
				if ((bool)enemyRange)
				{
					enemyRange.gameObject.SetActive(value: false);
				}
				attackFinishTime = Time.timeAsDouble + 2.0;
			}
			if (Time.timeAsDouble > attackFinishTime || target == null)
			{
				target = null;
				state = State.Follow;
			}
			else
			{
				DoFace(spriteFacesRight: false, playNewAnimation: true, details.turnAttackAnim, pauseBetweenTurns: true, 0.1f);
				DoChaseSimple(target.transform, 25f, 100f, 0f, 0f);
			}
			break;
		case State.BenchRestStart:
			if (LastFrameState != State.BenchRestStart)
			{
				body.linearVelocity = Vector2.zero;
				ToggleSingLoop(active: false);
				if ((bool)animator)
				{
					animator.Play(details.flyAnim);
				}
				benchRestWaitTime = Time.timeAsDouble + (double)UnityEngine.Random.Range(1f, 2.5f);
			}
			if (Time.timeAsDouble < benchRestWaitTime)
			{
				DoBuzz(0.75f, 1f, 2f, 30f, 50f, new Vector2(1f, 1f));
				DoFace(spriteFacesRight: false, playNewAnimation: true, details.turnFlyAnim, pauseBetweenTurns: true, 0.5f);
			}
			else
			{
				state = State.BenchRestLower;
			}
			break;
		case State.BenchRestLower:
		{
			if (LastFrameState != State.BenchRestLower)
			{
				if ((bool)animator)
				{
					animator.Play(details.flyAnim);
				}
				base.transform.SetPositionZ(sleepZ);
				body.isKinematic = false;
			}
			body.AddForce(new Vector2(0f, -5f));
			Vector2 linearVelocity3 = body.linearVelocity;
			linearVelocity3.x *= 0.85f;
			body.linearVelocity = linearVelocity3;
			DoFace(spriteFacesRight: false, playNewAnimation: true, details.turnFlyAnim, pauseBetweenTurns: true, 0.5f);
			if (!IsGrounded)
			{
				break;
			}
			state = State.BenchResting;
			if ((bool)audioSource)
			{
				audioSource.Stop();
			}
			body.isKinematic = true;
			body.linearVelocity = Vector2.zero;
			if ((bool)animator)
			{
				animator.Play(details.restStartAnim);
			}
			Vector3 position = base.transform.position;
			if ((bool)attackChargeAudioTable)
			{
				attackChargeAudioTable.SpawnAndPlayOneShot(position);
			}
			if ((bool)details.groundPoint)
			{
				RaycastHit2D raycastHit2D = Helper.Raycast2D(position, Vector2.down, 2f, 256);
				if (raycastHit2D.collider != null)
				{
					position.y = raycastHit2D.point.y + (position.y - details.groundPoint.position.y);
					base.transform.position = position;
				}
			}
			break;
		}
		case State.Recoil:
		{
			Vector2 linearVelocity2 = body.linearVelocity;
			body.linearVelocity = new Vector2(linearVelocity2.x * 0.9f, linearVelocity2.y * 0.975f);
			break;
		}
		case State.Sing:
		{
			Vector2 linearVelocity = body.linearVelocity;
			body.linearVelocity = new Vector2(linearVelocity.x * 0.9f, linearVelocity.y * 0.9f);
			break;
		}
		}
		LastFrameState = CurrentState;
		CurrentState = state;
	}

	private IEnumerator Spawn()
	{
		yield return null;
		if ((bool)audioSource)
		{
			audioSource.Play();
		}
		if ((bool)meshRenderer && !dreamSpawn)
		{
			meshRenderer.enabled = true;
		}
		damager.gameObject.SetActive(value: false);
		if (!quickSpawn)
		{
			float finishDelay = 0f;
			if ((bool)animator)
			{
				tk2dSpriteAnimationClip clipByName = animator.GetClipByName(details.hatchAnim);
				if (clipByName != null)
				{
					animator.Play(clipByName);
					finishDelay = clipByName.Duration;
				}
			}
			if ((bool)body)
			{
				float num = UnityEngine.Random.Range(65f, 75f);
				float num2 = UnityEngine.Random.Range(85f, 95f);
				Vector2 vector = default(Vector2);
				vector.x = num * Mathf.Cos(num2 * (MathF.PI / 180f));
				vector.y = num * Mathf.Sin(num2 * (MathF.PI / 180f));
				Vector2 linearVelocity = vector;
				body.linearVelocity = linearVelocity;
			}
			if ((bool)throwAudioTable)
			{
				throwAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			for (float elapsed = 0f; elapsed < finishDelay; elapsed += Time.fixedDeltaTime)
			{
				if ((bool)body)
				{
					body.linearVelocity *= 0.8f;
				}
				yield return new WaitForFixedUpdate();
			}
			openEffect.SetActive(value: true);
			Vector3 position = base.transform.position;
			pinPrefab.Spawn().transform.position = new Vector3(position.x, position.y, position.z + 0.01f);
			if ((bool)activateAudioTable)
			{
				activateAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			PlaySpawnVoice();
		}
		CurrentState = ((!GameManager.instance.playerData.atBench) ? State.Follow : State.BenchRestStart);
		spawnRoutine = null;
	}

	private float GetHeroDistance()
	{
		return Vector2.Distance(base.transform.position, HeroController.instance.transform.position);
	}

	private IEnumerator TeleEnd()
	{
		if ((bool)groundRange)
		{
			groundRange.gameObject.SetActive(value: true);
		}
		if ((bool)terrainCollider)
		{
			terrainCollider.enabled = true;
		}
		if ((bool)audioSource)
		{
			audioSource.Play();
		}
		if ((bool)animator)
		{
			tk2dSpriteAnimationClip clip = animator.GetClipByName(details.teleEndAnim);
			animator.Play(clip);
			for (float elapsed = 0f; elapsed < clip.Duration; elapsed += Time.fixedDeltaTime)
			{
				if ((bool)body)
				{
					body.linearVelocity *= 0.7f;
				}
				yield return new WaitForFixedUpdate();
			}
		}
		CurrentState = State.Follow;
	}

	public void Break()
	{
		damager.gameObject.SetActive(value: false);
		hpCurrent = 0;
		StartHitLanded();
	}

	public void FsmHitLanded()
	{
		StartHitLanded();
	}

	private void StartHitLanded()
	{
		hitCount++;
		blockHitState = ++hitState;
		if (hitLandedRoutine == null)
		{
			hitLandedRoutine = StartCoroutine(HitLanded());
		}
	}

	public void FsmHitBlocked()
	{
		blockHitState = ++hitState;
		if (hitBlockedRoutine == null)
		{
			hitBlockedRoutine = StartCoroutine(HitBlocked());
		}
	}

	private IEnumerator HitLanded()
	{
		yield return new WaitForFixedUpdate();
		explodeSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		body.linearVelocity = Vector2.zero;
		if ((bool)spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		float seconds = 2f;
		hpCurrent -= hitCount;
		hitCount = 0;
		hitParticles.Play();
		damager.gameObject.SetActive(value: false);
		if (hpCurrent > 0)
		{
			float num = UnityEngine.Random.Range(20, 30);
			num *= base.transform.localScale.x;
			body.linearVelocity = new Vector2(num, body.linearVelocity.y);
			CurrentState = State.Recoil;
			if (hpCurrent * 3 < hp)
			{
				damagedParticles.Play();
				damagedAudioLoop.Play();
			}
			yield return new WaitForSeconds(0.35f);
			float heroDistance = GetHeroDistance();
			CurrentState = ((!(heroDistance * 1.15f > 12f)) ? State.Follow : State.Tele);
			cooldownTimer = attackCooldown * UnityEngine.Random.Range(0.8f, 1.2f);
		}
		else
		{
			CurrentState = State.None;
			if ((bool)animator)
			{
				tk2dSpriteAnimationClip clipByName = animator.GetClipByName("Burst");
				if (clipByName != null)
				{
					animator.Play(clipByName);
					seconds = clipByName.Duration;
				}
			}
			if ((bool)breakAudioTable)
			{
				breakAudioTable.SpawnAndPlayOneShot(base.transform.position);
			}
			hitParticles.Stop();
			damagedParticles.Stop();
			damagedAudioLoop.Stop();
			deathParticles.Play();
			yield return new WaitForSeconds(seconds);
			meshRenderer.enabled = false;
			yield return new WaitForSeconds(1f);
			base.gameObject.Recycle();
		}
		hitLandedRoutine = null;
	}

	private IEnumerator HitBlocked()
	{
		yield return new WaitForFixedUpdate();
		if (blockHitState == hitState)
		{
			yield return new WaitForSeconds(blockedResetDelay);
		}
		if (blockHitState == hitState)
		{
			damager.gameObject.SetActive(value: false);
		}
		hitBlockedRoutine = null;
	}

	public void FsmHazardReload()
	{
	}

	[ContextMenu("Clear Effects")]
	public void ClearEffects()
	{
		ClearEffects(unEquipped: false);
	}

	public void ClearEffects(bool unEquipped)
	{
		if (!isClearingEffects && base.isActiveAndEnabled)
		{
			isClearingEffects = true;
			StopAllCoroutines();
			if (unEquipped)
			{
				StartCoroutine(UnEquippedRoutine());
			}
			else
			{
				StartCoroutine(ClearEffectsRoutine());
			}
		}
	}

	private IEnumerator UnEquippedRoutine()
	{
		explodeSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		body.linearVelocity = Vector2.zero;
		if ((bool)spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		float seconds = 2f;
		hpCurrent--;
		hitParticles.Play();
		damager.gameObject.SetActive(value: false);
		CurrentState = State.None;
		if ((bool)animator)
		{
			tk2dSpriteAnimationClip clipByName = animator.GetClipByName("Burst");
			if (clipByName != null)
			{
				animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		if ((bool)breakAudioTable)
		{
			breakAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		hitParticles.Stop();
		damagedParticles.Stop();
		damagedAudioLoop.Stop();
		deathParticles.Play();
		yield return new WaitForSeconds(seconds);
		meshRenderer.enabled = false;
		yield return new WaitForSeconds(1f);
		isClearingEffects = false;
		base.gameObject.Recycle();
	}

	private IEnumerator ClearEffectsRoutine()
	{
		yield return new WaitForSeconds(0.01f);
		UnityEngine.Object.Instantiate(clearEffectsDummyPrefab, base.transform.position, Quaternion.identity);
		meshRenderer.enabled = false;
		isClearingEffects = false;
		base.gameObject.Recycle();
	}

	public void FsmBenchRestStart()
	{
		CurrentState = State.BenchRestStart;
		isOnBench = true;
	}

	public void FsmBenchRestEnd()
	{
		isOnBench = false;
		if (CurrentState == State.BenchResting)
		{
			StartCoroutine(WakeUp());
		}
		else
		{
			CurrentState = State.Follow;
		}
	}

	private IEnumerator WakeUp()
	{
		float seconds = 0f;
		if ((bool)animator)
		{
			tk2dSpriteAnimationClip clipByName = animator.GetClipByName(details.restEndAnim);
			if (clipByName != null)
			{
				animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		if ((bool)activateAudioTable)
		{
			activateAudioTable.SpawnAndPlayOneShot(base.transform.position);
		}
		PlaySpawnVoice();
		ToggleSingLoop(active: false);
		yield return new WaitForSeconds(seconds);
		if (isOnBench)
		{
			currentState = State.BenchRestStart;
		}
		else
		{
			CurrentState = State.Follow;
		}
	}

	private void OnHeroLeavingScene()
	{
		canDamage = false;
		CurrentState = State.Follow;
		damager.gameObject.SetActive(value: false);
	}

	public void OnPreHeroInPosition(bool forceDirect)
	{
		HeroController instance = HeroController.instance;
		base.transform.SetPosition2D(instance.transform.position);
	}

	public void OnHeroInPosition(bool forceDirect)
	{
		DoDisableReset();
		HeroController instance = HeroController.instance;
		body.MovePosition((Vector2)instance.transform.position + new Vector2(2f * (float)((!instance.cState.facingRight) ? 1 : (-1)), 1.5f));
		quickSpawn = true;
		canDamage = true;
		DoEnableReset();
	}

	public void SetLastTargetDamaged(GameObject target)
	{
		lastTargetDamaged = target;
	}

	private void OnEquippedStateChanged()
	{
		if (hasToolSource && !toolSource.Status.IsEquippedAny)
		{
			ClearEffects(unEquipped: true);
		}
	}

	private void OnStartedNeedolin()
	{
		needolinPlaying = true;
	}

	private void OnStoppedNeedolin()
	{
		needolinPlaying = false;
	}

	private bool IsInNeedolinRange()
	{
		return HeroPerformanceRegion.GetAffectedState(base.transform, ignoreRange: false) == HeroPerformanceRegion.AffectedState.ActiveInner;
	}

	private void ResetNeedolinTimer()
	{
		needolinReactTime = UnityEngine.Random.Range(0.75f, 1f);
	}

	private void PlayVoiceTable(RandomAudioClipTable table)
	{
		if (hasVoiceSource && !(table == null))
		{
			AudioClip audioClip = table.SelectClip();
			if (!(audioClip == null))
			{
				float pitch = table.SelectPitch();
				float volume = table.SelectVolume();
				voiceAudioSource.pitch = pitch;
				voiceAudioSource.volume = volume;
				voiceAudioSource.PlayOneShot(audioClip);
			}
		}
	}

	private void PlayAttackVoice()
	{
		PlayVoiceTable(attackVoiceTable);
	}

	public void PlaySpawnVoice()
	{
		PlayVoiceTable(spawnVoiceTable);
	}

	public void PlayTurnVoice()
	{
		PlayVoiceTable(turnVoiceTable);
	}

	public void ToggleSingLoop(bool active)
	{
		if (!hasVoiceSource)
		{
			return;
		}
		if (active)
		{
			if (!(singVoiceTable == null))
			{
				AudioClip audioClip = singVoiceTable.SelectClip();
				if (!(audioClip == null))
				{
					voiceAudioSource.loop = true;
					voiceAudioSource.pitch = singVoiceTable.SelectPitch();
					voiceAudioSource.clip = audioClip;
					voiceAudioSource.time = UnityEngine.Random.Range(0f, audioClip.length);
					voiceAudioSource.volume = singVoiceTable.SelectVolume();
					voiceAudioSource.Play();
				}
			}
		}
		else
		{
			voiceAudioSource.Stop();
			voiceAudioSource.volume = 1f;
		}
	}

	private void DoFace(bool spriteFacesRight, bool playNewAnimation, string newAnimationClip, bool pauseBetweenTurns, float pauseTime)
	{
		if (body == null)
		{
			return;
		}
		Vector2 linearVelocity = body.linearVelocity;
		Vector3 localScale = base.transform.localScale;
		float x = linearVelocity.x;
		if (CurrentState != LastFrameState)
		{
			xScale = base.transform.localScale.x;
			pauseTimer = 0f;
		}
		if (xScale < 0f)
		{
			xScale *= -1f;
		}
		if (pauseTimer <= 0f || !pauseBetweenTurns)
		{
			if (x > 0f)
			{
				if (spriteFacesRight)
				{
					if (Math.Abs(localScale.x - xScale) > Mathf.Epsilon)
					{
						PlayTurnVoice();
						pauseTimer = pauseTime;
						localScale.x = xScale;
						if (playNewAnimation)
						{
							animator.Play(newAnimationClip);
							animator.PlayFromFrame(0);
						}
					}
				}
				else if (Math.Abs(localScale.x - (0f - xScale)) > Mathf.Epsilon)
				{
					PlayTurnVoice();
					pauseTimer = pauseTime;
					localScale.x = 0f - xScale;
					if (playNewAnimation)
					{
						animator.Play(newAnimationClip);
						animator.PlayFromFrame(0);
					}
				}
			}
			else if (x <= 0f)
			{
				if (spriteFacesRight)
				{
					if (Math.Abs(localScale.x - (0f - xScale)) > Mathf.Epsilon)
					{
						PlayTurnVoice();
						pauseTimer = pauseTime;
						localScale.x = 0f - xScale;
						if (playNewAnimation)
						{
							animator.Play(newAnimationClip);
							animator.PlayFromFrame(0);
						}
					}
				}
				else if (Math.Abs(localScale.x - xScale) > Mathf.Epsilon)
				{
					PlayTurnVoice();
					pauseTimer = pauseTime;
					localScale.x = xScale;
					if (playNewAnimation)
					{
						animator.Play(newAnimationClip);
						animator.PlayFromFrame(0);
					}
				}
			}
		}
		else
		{
			pauseTimer -= Time.deltaTime;
		}
		base.transform.SetScaleX(localScale.x);
	}

	private void DoChase(Transform chaseTarget, float distance, float speedMax, float accelerationForce, float chaseTargetRadius, float deceleration, Vector2 chaseOffset)
	{
		if (body == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = chaseTarget.position;
		float num = Mathf.Sqrt(Mathf.Pow(position.x - (position2.x + chaseOffset.x), 2f) + Mathf.Pow(position.y - (position2.y + chaseOffset.y), 2f));
		Vector2 linearVelocity = body.linearVelocity;
		if (!(num > distance - chaseTargetRadius) || !(num < distance + chaseTargetRadius))
		{
			Vector2 vector = new Vector2(position2.x + chaseOffset.x - position.x, position2.y + chaseOffset.y - position.y);
			vector = Vector2.ClampMagnitude(vector, 1f);
			vector = new Vector2(vector.x * accelerationForce, vector.y * accelerationForce);
			if (num < distance)
			{
				vector = new Vector2(0f - vector.x, 0f - vector.y);
			}
			body.AddForce(vector);
			linearVelocity = Vector2.ClampMagnitude(linearVelocity, speedMax);
			body.linearVelocity = linearVelocity;
			return;
		}
		linearVelocity = body.linearVelocity;
		if (linearVelocity.x < 0f)
		{
			linearVelocity.x *= deceleration;
			if (linearVelocity.x > 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		else if (linearVelocity.x > 0f)
		{
			linearVelocity.x *= deceleration;
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		if (linearVelocity.y < 0f)
		{
			linearVelocity.y *= deceleration;
			if (linearVelocity.y > 0f)
			{
				linearVelocity.y = 0f;
			}
		}
		else if (linearVelocity.y > 0f)
		{
			linearVelocity.y *= deceleration;
			if (linearVelocity.y < 0f)
			{
				linearVelocity.y = 0f;
			}
		}
		body.linearVelocity = linearVelocity;
	}

	private void DoBuzz(float waitMin, float waitMax, float speedMax, float accelerationMin, float accelerationMax, Vector2 roamingRange)
	{
		if (body == null)
		{
			return;
		}
		float num = 1.125f;
		Vector3 position = base.transform.position;
		if (CurrentState != LastFrameState)
		{
			startX = position.x;
			startY = position.y;
		}
		Vector2 linearVelocity = body.linearVelocity;
		if (position.y < startY - roamingRange.y)
		{
			if (linearVelocity.y < 0f)
			{
				accelY = accelerationMax;
				accelY /= 2000f;
				linearVelocity.y /= num;
				waitTime = UnityEngine.Random.Range(waitMin, waitMax);
			}
		}
		else if (position.y > startY + roamingRange.y && linearVelocity.y > 0f)
		{
			accelY = 0f - accelerationMax;
			accelY /= 2000f;
			linearVelocity.y /= num;
			waitTime = UnityEngine.Random.Range(waitMin, waitMax);
		}
		if (position.x < startX - roamingRange.x)
		{
			if (linearVelocity.x < 0f)
			{
				accelX = accelerationMax;
				accelX /= 2000f;
				linearVelocity.x /= num;
				waitTime = UnityEngine.Random.Range(waitMin, waitMax);
			}
		}
		else if (position.x > startX + roamingRange.x && linearVelocity.x > 0f)
		{
			accelX = 0f - accelerationMax;
			accelX /= 2000f;
			linearVelocity.x /= num;
			waitTime = UnityEngine.Random.Range(waitMin, waitMax);
		}
		if (waitTime <= Mathf.Epsilon)
		{
			if (base.transform.position.y < startY - roamingRange.y)
			{
				accelY = UnityEngine.Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.y > startY + roamingRange.y)
			{
				accelY = UnityEngine.Random.Range(0f - accelerationMax, accelerationMin);
			}
			else
			{
				accelY = UnityEngine.Random.Range(0f - accelerationMax, accelerationMax);
			}
			if (base.transform.position.x < startX - roamingRange.x)
			{
				accelX = UnityEngine.Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.x > startX + roamingRange.x)
			{
				accelX = UnityEngine.Random.Range(0f - accelerationMax, accelerationMin);
			}
			else
			{
				accelX = UnityEngine.Random.Range(0f - accelerationMax, accelerationMax);
			}
			accelY /= 2000f;
			accelX /= 2000f;
			waitTime = UnityEngine.Random.Range(waitMin, waitMax);
		}
		if (waitTime > Mathf.Epsilon)
		{
			waitTime -= Time.deltaTime;
		}
		linearVelocity.x += accelX;
		linearVelocity.y += accelY;
		if (linearVelocity.x > speedMax)
		{
			linearVelocity.x = speedMax;
		}
		if (linearVelocity.x < 0f - speedMax)
		{
			linearVelocity.x = 0f - speedMax;
		}
		if (linearVelocity.y > speedMax)
		{
			linearVelocity.y = speedMax;
		}
		if (linearVelocity.y < 0f - speedMax)
		{
			linearVelocity.y = 0f - speedMax;
		}
		body.linearVelocity = linearVelocity;
	}

	private void DoChaseSimple(Transform chaseTarget, float speedMax, float accelerationForce, float offsetX, float offsetY)
	{
		if (!(body == null))
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = chaseTarget.position;
			Vector2 vector = new Vector2(position2.x + offsetX - position.x, position2.y + offsetY - position.y);
			vector = Vector2.ClampMagnitude(vector, 1f);
			vector = new Vector2(vector.x * accelerationForce, vector.y * accelerationForce);
			body.AddForce(vector);
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity = Vector2.ClampMagnitude(linearVelocity, speedMax);
			body.linearVelocity = linearVelocity;
		}
	}
}
