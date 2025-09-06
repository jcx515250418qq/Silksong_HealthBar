using System;
using System.Collections;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

public class ToolPin : MonoBehaviour, ITinkResponder, IProjectile, IBreakableProjectile
{
	public string flyClip;

	public string thunkClip;

	public string tinkClip;

	public bool breaksOnTink;

	public bool piercer;

	public tk2dSprite sprite;

	public tk2dSpriteAnimator animator;

	public BoxCollider2D boxCollider;

	public Rigidbody2D rigidBody;

	public MeshRenderer meshRenderer;

	public GameObject terrainBox;

	public GameObject reboundBox;

	public GameObject tinker;

	public GameObject thunkEffect;

	public GameObject spark;

	public GameObject slashImpact;

	public GameObject throwEffect;

	public GameObject reboundHitEffectGameobject;

	public ParticleSystem ptShatter;

	public ParticleSystem ptSpeedLines;

	[SerializeField]
	private AudioEventRandom spawnSound;

	[SerializeField]
	private AudioEventRandom wallHitSound;

	[SerializeField]
	private AudioEventRandom reboundSound;

	[SerializeField]
	private AudioEventRandom breakSound;

	[SerializeField]
	private int hitsToBreak = 1;

	[SerializeField]
	private bool rotateWithVelocity;

	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	public Color ptShatterDefaultColour = Color.white;

	public ParticleSystem ptPoisonIdle;

	public ParticleSystem ptPoisonThunk;

	private Color poisonTint;

	private float breakTimer;

	private float recycleTimer;

	private int hitsLeft;

	private float? initialGravityScale;

	private bool hasStarted;

	private bool doDeflect;

	private int doBreak;

	private bool wasDeflected;

	private bool tinked;

	private bool rebounded;

	private bool hasThunked;

	private int travelFrames;

	private bool isPoison;

	private bool isBroken;

	private Collider2D thunkedCollider;

	private int initialDamage;

	private DamageEnemies damageEnemies;

	private FaceAngleSimple faceAngle;

	private SpriteFlash spriteFlash;

	private IBreakableProjectile.HitInfo hitInfo;

	public ITinkResponder.TinkFlags ResponderType => ITinkResponder.TinkFlags.Projectile;

	private void Awake()
	{
		damageEnemies = GetComponent<DamageEnemies>();
		if (damageEnemies != null)
		{
			damageEnemies.WillDamageEnemyOptions += HitEnemy;
			damageEnemies.HitResponded += HitEnemy;
		}
		faceAngle = GetComponent<FaceAngleSimple>();
		spriteFlash = GetComponent<SpriteFlash>();
		poisonTint = Gameplay.PoisonPouchTintColour;
	}

	private void OnEnable()
	{
		doBreak = 0;
		isBroken = false;
		rigidBody.WakeUp();
		rigidBody.isKinematic = false;
		boxCollider.enabled = true;
		thunkEffect.SetActive(value: false);
		if ((bool)reboundBox)
		{
			reboundBox.SetActive(value: false);
		}
		tinker.SetActive(value: true);
		damageEnemies.doesNotTink = false;
		spark.SetActive(value: false);
		slashImpact.SetActive(value: false);
		throwEffect.SetActive(value: true);
		animator.Play(flyClip);
		breakTimer = 4f;
		recycleTimer = 0f;
		meshRenderer.enabled = true;
		doDeflect = false;
		wasDeflected = false;
		tinked = false;
		rebounded = false;
		thunkedCollider = null;
		hasThunked = false;
		rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
		base.transform.rotation = Quaternion.identity;
		hitsLeft = hitsToBreak;
		if (!initialGravityScale.HasValue)
		{
			initialGravityScale = rigidBody.gravityScale;
		}
		else
		{
			rigidBody.gravityScale = initialGravityScale.Value;
		}
		rigidBody.linearVelocity = Vector2.zero;
		rigidBody.angularVelocity = 0f;
		if ((bool)faceAngle)
		{
			faceAngle.enabled = true;
		}
		if (hasStarted)
		{
			spawnSound.SpawnAndPlayOneShot(base.transform.position);
			damageEnemies.damageDealt = initialDamage;
			travelFrames = 0;
			CheckPoison();
		}
	}

	private void OnDisable()
	{
		thunkedCollider = null;
	}

	private void Start()
	{
		spawnSound.SpawnAndPlayOneShot(base.transform.position);
		initialDamage = damageEnemies.damageDealt;
		hasStarted = true;
		CheckPoison();
	}

	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			if ((bool)getTintFrom)
			{
				sprite.EnableKeyword("CAN_HUESHIFT");
				sprite.SetFloat(PoisonTintBase.HueShiftPropId, getTintFrom.PoisonHueShift);
			}
			else
			{
				sprite.EnableKeyword("RECOLOUR");
				sprite.color = poisonTint;
			}
			ParticleSystem.MainModule main = ptShatter.main;
			main.startColor = poisonTint;
			ptPoisonIdle.Play();
			isPoison = true;
		}
		else
		{
			sprite.DisableKeyword("CAN_HUESHIFT");
			sprite.DisableKeyword("RECOLOUR");
			sprite.color = Color.white;
			ParticleSystem.MainModule main2 = ptShatter.main;
			main2.startColor = ptShatterDefaultColour;
			isPoison = false;
		}
	}

	private void Update()
	{
		if (breakTimer > 0f)
		{
			if ((bool)thunkedCollider && !thunkedCollider.isActiveAndEnabled)
			{
				breakTimer = 0f;
			}
			breakTimer -= Time.deltaTime;
			if (breakTimer <= 0f)
			{
				Break();
			}
		}
		if (recycleTimer > 0f)
		{
			recycleTimer -= Time.deltaTime;
			if (recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
	}

	private void LateUpdate()
	{
		if (doDeflect)
		{
			Deflect();
			doDeflect = false;
		}
	}

	private void FixedUpdate()
	{
		if (rotateWithVelocity && boxCollider.enabled)
		{
			VelocityWasSet();
		}
		if (travelFrames < 10)
		{
			travelFrames++;
		}
		if (doBreak > 0)
		{
			doBreak--;
			if (doBreak <= 0)
			{
				DoQueuedBreak();
			}
		}
	}

	public void VelocityWasSet()
	{
		Vector2 normalized = rigidBody.linearVelocity.normalized;
		Vector3 vector = new Vector3(normalized.y, 0f - normalized.x);
		if (base.transform.localScale.x < 0f)
		{
			vector = -vector;
		}
		base.transform.rotation = Quaternion.LookRotation(Vector3.forward, -vector);
		if ((bool)damageEnemies)
		{
			float direction = Vector2.Angle(Vector2.right, normalized);
			damageEnemies.direction = direction;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (recycleTimer > 0f && !rebounded)
		{
			return;
		}
		if (!wasDeflected && !tinked && (bool)collision.gameObject.GetComponent<TinkEffect>())
		{
			if (!piercer)
			{
				return;
			}
			BoxCollider2D component = collision.gameObject.GetComponent<BoxCollider2D>();
			if (component != null && !component.isTrigger)
			{
				Break();
				return;
			}
			PolygonCollider2D component2 = collision.gameObject.GetComponent<PolygonCollider2D>();
			if (component != null && !component2.isTrigger)
			{
				Break();
			}
			return;
		}
		if (travelFrames < 2)
		{
			wallHitSound.SpawnAndPlayOneShot(base.transform.position);
			breakSound.SpawnAndPlayOneShot(base.transform.position);
			Break();
			return;
		}
		if (collision.contacts.Length != 0 && !tinked)
		{
			ContactPoint2D contactPoint2D = collision.contacts[0];
			if (Mathf.Abs(Vector2.Dot(base.transform.right, contactPoint2D.normal)) > 0.05f)
			{
				thunkedCollider = collision.collider;
				Thunk();
				return;
			}
		}
		breakSound.SpawnAndPlayOneShot(base.transform.position);
		Break();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Deflector"))
		{
			doDeflect = true;
		}
		DamageHero component = collision.gameObject.GetComponent<DamageHero>();
		if ((bool)component)
		{
			if (component.hazardType == HazardType.LAVA)
			{
				Break();
			}
			if (component.canClashTink)
			{
				doDeflect = true;
			}
		}
		if (tinked && collision.gameObject.CompareTag("Nail Attack"))
		{
			StartCoroutine(Rebound(collision.gameObject));
		}
	}

	public void Deflect()
	{
		damageEnemies.damageDealt = 0;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (UnityEngine.Random.Range(1, 100) < 51)
		{
			localEulerAngles.z += UnityEngine.Random.Range(9, 12);
		}
		else
		{
			localEulerAngles.z += UnityEngine.Random.Range(-9, -12);
		}
		base.transform.localEulerAngles = localEulerAngles;
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		float num = localEulerAngles.z;
		if (base.transform.localScale.x < 0f)
		{
			num += 180f;
		}
		float magnitude = component.linearVelocity.magnitude;
		float x = magnitude * Mathf.Cos(num * (MathF.PI / 180f));
		float y = magnitude * Mathf.Sin(num * (MathF.PI / 180f));
		Vector2 linearVelocity = new Vector2(x, y);
		component.linearVelocity = linearVelocity;
		tinker.SetActive(value: false);
		damageEnemies.doesNotTink = true;
		wasDeflected = true;
	}

	private void Thunk()
	{
		rigidBody.isKinematic = true;
		rigidBody.linearVelocity = new Vector2(0f, 0f);
		boxCollider.enabled = false;
		thunkEffect.SetActive(value: true);
		animator.Play(thunkClip);
		tinker.SetActive(value: false);
		damageEnemies.doesNotTink = true;
		tinked = false;
		breakTimer = UnityEngine.Random.Range(1.8f, 2.2f);
		wallHitSound.SpawnAndPlayOneShot(base.transform.position);
		ptPoisonIdle.Stop();
		if (isPoison)
		{
			ptPoisonThunk.Play();
		}
		ReportEnded();
	}

	public void Break()
	{
		if (!isBroken)
		{
			doBreak = 0;
			isBroken = true;
			breakTimer = 0f;
			rigidBody.isKinematic = true;
			rigidBody.linearVelocity = Vector2.zero;
			rigidBody.angularVelocity = 0f;
			rigidBody.Sleep();
			if (meshRenderer.isVisible)
			{
				spark.SetActive(value: true);
				ptShatter.Play();
			}
			boxCollider.enabled = false;
			meshRenderer.enabled = false;
			tinker.SetActive(value: false);
			damageEnemies.doesNotTink = true;
			if ((bool)reboundBox)
			{
				reboundBox.SetActive(value: false);
			}
			tinked = false;
			recycleTimer = 1f;
			ptPoisonIdle.Stop();
			ReportEnded();
		}
	}

	public void QueueBreak(IBreakableProjectile.HitInfo hitInfo)
	{
		if (!isBroken && doBreak <= 0)
		{
			doBreak = 2;
			this.hitInfo = hitInfo;
		}
	}

	private void DoQueuedBreak()
	{
	}

	private void ReportEnded()
	{
		if (!hasThunked)
		{
			hasThunked = true;
			EventRegister.SendEvent(EventRegisterEvents.ToolPinThunked);
		}
	}

	private void HitEnemy(HealthManager enemyHealthManager, HitInstance damageInstance)
	{
		if (enemyHealthManager.IsBlockingByDirection(DirectionUtils.GetCardinalDirection(damageInstance.Direction), damageInstance.AttackType) && !piercer)
		{
			Tinked();
		}
		else
		{
			HitEnemy();
		}
	}

	private void HitEnemy(DamageEnemies.HitResponse hitResponse)
	{
		if (hitResponse.Target.gameObject.GetComponent<AttackDetonator>() != null)
		{
			HitEnemy();
		}
	}

	private void HitEnemy()
	{
		if ((bool)spriteFlash)
		{
			spriteFlash.flashWhiteQuick();
		}
		slashImpact.SetActive(value: true);
		if (hitsToBreak <= 0)
		{
			return;
		}
		hitsLeft--;
		if (hitsLeft <= 0)
		{
			breakTimer = 0f;
			rigidBody.Sleep();
			boxCollider.enabled = false;
			meshRenderer.enabled = false;
			tinked = false;
			recycleTimer = 0.5f;
			tinker.SetActive(value: false);
			damageEnemies.doesNotTink = true;
			ptPoisonIdle.Stop();
			if (isPoison)
			{
				ptPoisonThunk.Play();
			}
		}
	}

	public void Tinked()
	{
		if (piercer)
		{
			return;
		}
		if (rebounded || breaksOnTink)
		{
			Break();
			return;
		}
		boxCollider.enabled = false;
		reboundBox.SetActive(value: true);
		breakTimer = 2f;
		if ((bool)faceAngle)
		{
			faceAngle.enabled = false;
		}
		rigidBody.gravityScale = 2.75f;
		float num = UnityEngine.Random.Range(13f, 16f);
		float y = UnityEngine.Random.Range(28f, 33f);
		float num2 = UnityEngine.Random.Range(5f, 8f) * 360f;
		rigidBody.constraints = RigidbodyConstraints2D.None;
		if (base.transform.localScale.x > 0f)
		{
			rigidBody.linearVelocity = new Vector2(0f - num, y);
			rigidBody.angularVelocity = num2;
		}
		else
		{
			rigidBody.linearVelocity = new Vector2(num, y);
			rigidBody.angularVelocity = 0f - num2;
		}
		tinked = true;
		damageEnemies.doesNotTink = true;
		if (!string.IsNullOrEmpty(tinkClip))
		{
			animator.Play(tinkClip);
		}
	}

	public void TornadoEffect()
	{
		boxCollider.enabled = false;
		if ((bool)reboundBox)
		{
			reboundBox.SetActive(value: true);
		}
		breakTimer = 2f;
		if ((bool)faceAngle)
		{
			faceAngle.enabled = false;
		}
		rigidBody.gravityScale = 2.75f;
		float num = UnityEngine.Random.Range(13f, 16f);
		float y = UnityEngine.Random.Range(28f, 33f);
		float num2 = UnityEngine.Random.Range(5f, 8f) * 360f;
		rigidBody.constraints = RigidbodyConstraints2D.None;
		if (base.transform.localScale.x > 0f)
		{
			rigidBody.linearVelocity = new Vector2(0f - num, y);
			rigidBody.angularVelocity = num2;
		}
		else
		{
			rigidBody.linearVelocity = new Vector2(num, y);
			rigidBody.angularVelocity = 0f - num2;
		}
		tinked = true;
		damageEnemies.doesNotTink = true;
		if (!string.IsNullOrEmpty(tinkClip))
		{
			animator.Play(tinkClip);
		}
	}

	public IEnumerator Rebound(GameObject attackObject)
	{
		tinked = false;
		rebounded = true;
		boxCollider.enabled = true;
		reboundBox.SetActive(value: false);
		breakTimer = 0f;
		if (!initialGravityScale.HasValue)
		{
			initialGravityScale = rigidBody.gravityScale;
		}
		else
		{
			rigidBody.gravityScale = initialGravityScale.Value;
		}
		damageEnemies.damageDealt = 5;
		rigidBody.angularVelocity = 0f;
		animator.Play(flyClip);
		breakTimer = 4f;
		recycleTimer = 0f;
		reboundSound.SpawnAndPlayOneShot(base.transform.position);
		reboundHitEffectGameobject.Spawn(base.transform.position);
		float launchAngle = attackObject.GetComponent<DamageEnemies>().direction;
		float f = MathF.PI / 180f * launchAngle;
		Vector2 linearVelocity = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		linearVelocity *= 50f;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		rigidBody.linearVelocity = linearVelocity;
		if (launchAngle >= 80f && launchAngle <= 100f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
		}
		yield return new WaitForSeconds(0.01f);
		base.transform.localEulerAngles = new Vector3(0f, 0f, launchAngle);
	}
}
