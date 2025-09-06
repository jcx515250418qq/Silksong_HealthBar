using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

[RequireComponent(typeof(DamageHero))]
public class FireballProjectile : MonoBehaviour, IHitResponder
{
	private const int MAX_STATIONARY_STEPS = 5;

	private const float STEP_MOVEMENT_THRESHOLD = 0.01f;

	public int TerrainBounces = 3;

	private int terrainBouncesLeft;

	public float IdleLifeTime = 2f;

	private float idleTimeLeft;

	public ParticleSystem[] TrailParticles;

	public ParticleSystem[] AnticParticles;

	[Space]
	public GameObject ActiveBeforeExplosion;

	[Space]
	public GameObject ExplosionChild;

	public float ExplodeAnticTime = 0.5f;

	public Vector2 ExplodeAnticJitter = new Vector2(0.1f, 0.1f);

	public float GroundedDistance = 1f;

	public bool DoRecycle = true;

	public bool ExplosionRock;

	[Space]
	public float CanHitBackPause;

	public float HitBackSpeed;

	public float HitBackAngle;

	[Space]
	[SerializeField]
	private AudioSource loopSource;

	[SerializeField]
	private AudioClip loopClip;

	[SerializeField]
	private AudioClip anticLoopClip;

	[SerializeField]
	private AudioEvent knockbackSound;

	[SerializeField]
	private AudioEvent bounceSound;

	[SerializeField]
	private AudioEvent deathSound;

	private int heroDamageAmount;

	private int enemyDamageAmount;

	private ParticleSystem[] childParticles;

	private bool isActive;

	private bool wasHit;

	private Vector2 previousPosition;

	private int stationarySteps;

	private bool isInExplodeAntic;

	private Coroutine stopMovementRoutine;

	private JitterSelf jitterSelf;

	private DamageHero heroDamager;

	private DamageEnemies damageEnemies;

	private Rigidbody2D body;

	private Collider2D collider;

	private SpinSelf spinSelf;

	private ObjectBounce bouncer;

	private TinkEffect tinkEffect;

	private Renderer renderer;

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position, (Vector2)base.transform.position + Vector2.down * GroundedDistance);
	}

	private void Awake()
	{
		heroDamager = GetComponent<DamageHero>();
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		damageEnemies = GetComponent<DamageEnemies>();
		spinSelf = GetComponent<SpinSelf>();
		renderer = GetComponent<Renderer>();
		bouncer = GetComponent<ObjectBounce>();
		tinkEffect = GetComponent<TinkEffect>();
		childParticles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		jitterSelf = GetComponent<JitterSelf>();
		if (!jitterSelf)
		{
			jitterSelf = JitterSelf.Add(base.gameObject, new JitterSelfConfig
			{
				AmountMin = ExplodeAnticJitter,
				AmountMax = ExplodeAnticJitter,
				UseCameraRenderHooks = true
			}, CameraRenderHooks.CameraSource.MainCamera);
		}
	}

	private void Start()
	{
		if ((bool)heroDamager)
		{
			heroDamager.HeroDamaged += OnDamagedPlayer;
		}
		if ((bool)damageEnemies)
		{
			damageEnemies.DamagedEnemy += OnDamagedEnemy;
		}
		if ((bool)bouncer)
		{
			bouncer.Bounced += OnBounce;
		}
	}

	private void OnDestroy()
	{
		if ((bool)heroDamager)
		{
			heroDamager.HeroDamaged -= OnDamagedPlayer;
		}
		if ((bool)damageEnemies)
		{
			damageEnemies.DamagedEnemy -= OnDamagedEnemy;
		}
		if ((bool)bouncer)
		{
			bouncer.Bounced -= OnBounce;
		}
	}

	private void OnEnable()
	{
		SetActive(isActive: true);
		terrainBouncesLeft = TerrainBounces;
		idleTimeLeft = IdleLifeTime;
		wasHit = false;
		if ((bool)spinSelf)
		{
			spinSelf.enabled = true;
		}
		isInExplodeAntic = false;
		if ((bool)body)
		{
			body.isKinematic = false;
		}
		if ((bool)jitterSelf)
		{
			jitterSelf.StopJitter();
		}
		if ((bool)collider)
		{
			collider.enabled = true;
		}
		if ((bool)tinkEffect)
		{
			tinkEffect.enabled = true;
		}
		if ((bool)ActiveBeforeExplosion)
		{
			ActiveBeforeExplosion.SetActive(value: true);
		}
		if ((bool)damageEnemies && enemyDamageAmount == 0)
		{
			enemyDamageAmount = damageEnemies.damageDealt;
		}
		if ((bool)heroDamager)
		{
			if (heroDamageAmount == 0)
			{
				heroDamageAmount = heroDamager.damageDealt;
			}
			else
			{
				heroDamager.damageDealt = heroDamageAmount;
			}
		}
		ExplosionChild.SetActive(value: false);
		if ((bool)loopSource)
		{
			loopSource.clip = loopClip;
			loopSource.Play();
		}
		DisableEnemyDamage();
	}

	public void DisableEnemyDamage()
	{
		damageEnemies.damageDealt = 0;
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		float num = idleTimeLeft + ExplodeAnticTime;
		if (num <= 0f)
		{
			Break();
		}
		else if (num <= ExplodeAnticTime && !isInExplodeAntic)
		{
			isInExplodeAntic = true;
			ParticleSystem[] anticParticles = AnticParticles;
			for (int i = 0; i < anticParticles.Length; i++)
			{
				anticParticles[i].Play(withChildren: true);
			}
			stopMovementRoutine = StartCoroutine(StopMovementOnGround());
			if ((bool)tinkEffect)
			{
				tinkEffect.enabled = false;
			}
			if ((bool)loopSource)
			{
				loopSource.clip = anticLoopClip;
				loopSource.Play();
			}
			if ((bool)jitterSelf)
			{
				jitterSelf.StartJitter();
			}
		}
		idleTimeLeft -= Time.deltaTime;
		if (CanHitBackPause > 0f)
		{
			CanHitBackPause -= Time.deltaTime;
		}
	}

	private IEnumerator StopMovementOnGround()
	{
		yield return new WaitUntil(() => Helper.Raycast2D(base.transform.position, Vector2.down, GroundedDistance, 256).collider != null && (!body || body.linearVelocity.y <= 0f));
		if ((bool)body)
		{
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (isActive && !isInExplodeAntic)
		{
			if (((Vector2)base.transform.position - previousPosition).magnitude < 0.01f)
			{
				stationarySteps++;
			}
			else
			{
				stationarySteps = 0;
			}
			previousPosition = base.transform.position;
			if (stationarySteps >= 5)
			{
				idleTimeLeft = 0f;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (isActive && !isInExplodeAntic && collision.gameObject.layer == 8)
		{
			if (terrainBouncesLeft <= 0)
			{
				idleTimeLeft = 0f;
			}
			else
			{
				idleTimeLeft = IdleLifeTime;
			}
			terrainBouncesLeft--;
			bounceSound.SpawnAndPlayOneShot(base.transform.position);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Lava"))
		{
			Break();
		}
		if (!ExplosionRock)
		{
			return;
		}
		switch ((PhysLayers)collision.gameObject.layer)
		{
		case PhysLayers.PLAYER:
		case PhysLayers.HERO_BOX:
			Break();
			break;
		case PhysLayers.ENEMIES:
			if (wasHit)
			{
				Break();
			}
			break;
		}
	}

	private IEnumerator MonitorExplosion()
	{
		if (childParticles.Length == 0)
		{
			if ((bool)ExplosionChild)
			{
				yield return new WaitUntil(() => !ExplosionChild.activeInHierarchy);
			}
		}
		else
		{
			yield return new WaitUntil(delegate
			{
				bool flag = false;
				ParticleSystem[] array = childParticles;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].IsAlive(withChildren: true))
					{
						flag = true;
					}
				}
				return !flag;
			});
		}
		if (DoRecycle)
		{
			base.gameObject.Recycle();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void SetActive(bool isActive)
	{
		this.isActive = isActive;
		if ((bool)spinSelf)
		{
			spinSelf.enabled = isActive;
		}
		if ((bool)body)
		{
			body.isKinematic = !isActive;
			body.angularVelocity = 0f;
			body.linearVelocity = Vector2.zero;
		}
		if ((bool)renderer)
		{
			renderer.enabled = isActive;
		}
		ParticleSystem[] trailParticles;
		if (isActive)
		{
			trailParticles = TrailParticles;
			for (int i = 0; i < trailParticles.Length; i++)
			{
				trailParticles[i].Play(withChildren: true);
			}
			return;
		}
		trailParticles = TrailParticles;
		for (int i = 0; i < trailParticles.Length; i++)
		{
			trailParticles[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (stopMovementRoutine != null)
		{
			StopCoroutine(stopMovementRoutine);
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!isActive || !body || CanHitBackPause > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage && damageInstance.AttackType != AttackTypes.Generic)
		{
			return IHitResponder.Response.None;
		}
		if (isInExplodeAntic && !ExplosionRock)
		{
			Break();
			return IHitResponder.Response.GenericHit;
		}
		float num = 0f;
		if (damageInstance.IsNailDamage)
		{
			switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
			{
			case HitInstance.HitDirection.Left:
				num = 180f - HitBackAngle;
				break;
			case HitInstance.HitDirection.Right:
				num = HitBackAngle;
				break;
			case HitInstance.HitDirection.Up:
				num = 90f;
				break;
			case HitInstance.HitDirection.Down:
				num = 270f;
				break;
			}
		}
		else
		{
			num = damageInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular);
		}
		float value = HitBackSpeed * Mathf.Cos(num * (MathF.PI / 180f));
		float value2 = HitBackSpeed * Mathf.Sin(num * (MathF.PI / 180f));
		body.SetVelocity(value, value2);
		if (!ExplosionRock)
		{
			terrainBouncesLeft = 0;
			idleTimeLeft = IdleLifeTime;
		}
		wasHit = true;
		damageEnemies.damageDealt = enemyDamageAmount;
		damageEnemies.AwardJournalKill = true;
		knockbackSound.SpawnAndPlayOneShot(base.transform.position);
		return IHitResponder.Response.GenericHit;
	}

	public void Break()
	{
		if (!isActive)
		{
			return;
		}
		SetActive(isActive: false);
		base.transform.SetRotation2D(0f);
		if ((bool)ExplosionChild)
		{
			ExplosionChild.SetActive(value: true);
			if ((bool)damageEnemies)
			{
				DamageEnemies componentInChildren = ExplosionChild.GetComponentInChildren<DamageEnemies>();
				if ((bool)componentInChildren)
				{
					componentInChildren.AwardJournalKill = damageEnemies.AwardJournalKill;
				}
			}
		}
		if ((bool)ActiveBeforeExplosion)
		{
			ActiveBeforeExplosion.SetActive(value: false);
		}
		if ((bool)damageEnemies)
		{
			damageEnemies.damageDealt = 0;
		}
		if ((bool)heroDamager)
		{
			heroDamager.damageDealt = 0;
		}
		if ((bool)body)
		{
			body.isKinematic = true;
		}
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		StartCoroutine(MonitorExplosion());
		deathSound.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)loopSource)
		{
			loopSource.Stop();
		}
	}

	public void OnDamagedPlayer()
	{
		Break();
	}

	public void OnDamagedEnemy()
	{
		if (wasHit)
		{
			Break();
		}
	}

	public void OnBounce()
	{
		if ((bool)spinSelf)
		{
			spinSelf.enabled = false;
		}
	}
}
