using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public class ToolRing : MonoBehaviour
{
	private enum MaterialState
	{
		None = 0,
		Normal = 1,
		Poison = 2
	}

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private SpriteFlash spriteFlash;

	[SerializeField]
	private DamageEnemies damageEnemies;

	[SerializeField]
	private TrackTriggerObjects enemyRange;

	[SerializeField]
	private GameObject breakEffects;

	[SerializeField]
	private GameObject terrainHitPrefab;

	[SerializeField]
	private float speed;

	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private GameObject[] inactiveOnBreak;

	[SerializeField]
	private CogRollThenFallOver inertCog;

	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	public ParticleSystem ptPoisonIdle;

	public SpriteRenderer flattenSprite;

	[SerializeField]
	private SpriteRenderer fallenSprite;

	public ParticleSystem ptRingTrail;

	public Color ptRingTrailDefaultColour;

	public ParticleSystem ptShatter;

	public Color ptShatterDefaultColour;

	[Space]
	[SerializeField]
	private UnityEvent OnStop;

	private Color poisonTint;

	private float breakTimer;

	private float recycleTimer;

	private Vector3 inertCogInitialPos;

	private Quaternion inertCogInitialRot;

	private Rigidbody2D body;

	private int hitsLeft;

	private readonly HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

	private readonly List<ToolRing> cleanupQueue = new List<ToolRing>();

	private int lastBounceFrame;

	private MaterialState materialState;

	private void OnValidate()
	{
		if (hitsToBreak < 1)
		{
			hitsToBreak = 1;
		}
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		damageEnemies.DamagedEnemyGameObject += OnDamagedEnemy;
		if ((bool)inertCog)
		{
			Transform transform = inertCog.transform;
			inertCogInitialPos = transform.localPosition;
			inertCogInitialRot = transform.localRotation;
			inertCog.FallenCleanup += OnInertCogCleanup;
		}
		poisonTint = Gameplay.PoisonPouchTintColour;
	}

	private void Start()
	{
		CheckPoison();
	}

	private void OnEnable()
	{
		hitsLeft = hitsToBreak;
		hitEnemies.Clear();
		hitEnemies.Add(base.gameObject);
		sprite.enabled = true;
		inactiveOnBreak.SetAllActive(value: true);
		breakEffects.SetActive(value: false);
		breakTimer = 0.7f;
		recycleTimer = 0f;
		inertCog.gameObject.SetActive(value: false);
		Transform obj = inertCog.transform;
		obj.localPosition = inertCogInitialPos;
		obj.localRotation = inertCogInitialRot;
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance && silentInstance.RingTauntEffectConsume())
		{
			damageEnemies.DamageMultiplier = 1.5f;
			if ((bool)spriteFlash)
			{
				spriteFlash.flashWhiteQuick();
			}
		}
		else
		{
			damageEnemies.DamageMultiplier = 1f;
		}
		CheckPoison();
	}

	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			if (materialState != MaterialState.Poison)
			{
				materialState = MaterialState.Poison;
				SetMaterialPoison(sprite.material);
				SetMaterialPoison(flattenSprite.material);
				if ((bool)fallenSprite)
				{
					SetMaterialPoison(fallenSprite.material);
				}
				ParticleSystem.MainModule main = ptRingTrail.main;
				main.startColor = poisonTint;
				ParticleSystem.MainModule main2 = ptShatter.main;
				main2.startColor = poisonTint;
			}
			ptPoisonIdle.Play();
		}
		else if (materialState != MaterialState.Normal)
		{
			materialState = MaterialState.Normal;
			SetMaterialNormal(sprite.material);
			SetMaterialNormal(flattenSprite.material);
			if ((bool)fallenSprite)
			{
				SetMaterialNormal(fallenSprite.material);
			}
			ParticleSystem.MainModule main3 = ptRingTrail.main;
			main3.startColor = ptRingTrailDefaultColour;
			ParticleSystem.MainModule main4 = ptShatter.main;
			main4.startColor = ptShatterDefaultColour;
		}
	}

	private void SetMaterialNormal(Material material)
	{
		material.DisableKeyword("CAN_HUESHIFT");
		material.DisableKeyword("RECOLOUR");
		material.color = Color.white;
	}

	private void SetMaterialPoison(Material material)
	{
		if ((bool)getTintFrom)
		{
			material.EnableKeyword("CAN_HUESHIFT");
			material.SetFloat(PoisonTintBase.HueShiftPropId, getTintFrom.PoisonHueShift);
		}
		else
		{
			material.EnableKeyword("RECOLOUR");
			material.color = poisonTint;
		}
	}

	private void OnDisable()
	{
		cleanupQueue.Remove(this);
		hitEnemies.Clear();
	}

	private void Update()
	{
		if (breakTimer > 0f)
		{
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hitsLeft <= 0)
		{
			return;
		}
		hitsLeft--;
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		if (UnityEngine.Random.Range(0f, 1f) < 0.15f && (bool)inertCog)
		{
			if ((bool)terrainHitPrefab)
			{
				terrainHitPrefab.Spawn(safeContact.Point);
			}
			DoCogRoll(safeContact);
			return;
		}
		if (hitsLeft <= 0)
		{
			Break();
			return;
		}
		breakTimer = 0.7f;
		if ((bool)terrainHitPrefab)
		{
			terrainHitPrefab.Spawn(safeContact.Point);
		}
		if (!TryBounceTowardsEnemy())
		{
			Vector2 vector = (Vector2.Reflect(-collision.relativeVelocity, safeContact.Normal).DirectionToAngle() + UnityEngine.Random.Range(-20f, 20f)).AngleToDirection();
			body.linearVelocity = vector * speed;
			lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		}
	}

	private void OnInertCogCleanup()
	{
		base.gameObject.Recycle();
	}

	private void OnDamagedEnemy(GameObject enemy)
	{
		if (hitsLeft <= 0)
		{
			return;
		}
		hitsLeft--;
		if (hitsLeft <= 0)
		{
			Break();
			return;
		}
		breakTimer = 0.7f;
		hitEnemies.Add(enemy);
		if (!TryBounceTowardsEnemy())
		{
			Vector2 linearVelocity = (body.linearVelocity.DirectionToAngle() + UnityEngine.Random.Range(150f, 220f)).AngleToDirection() * speed;
			body.linearVelocity = linearVelocity;
			lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		}
	}

	private bool TryBounceTowardsEnemy()
	{
		GameObject closestInsideLineOfSight = enemyRange.GetClosestInsideLineOfSight(base.transform.position, hitEnemies);
		if (!closestInsideLineOfSight)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = closestInsideLineOfSight.transform.position;
		float y = position2.y - position.y;
		float x = position2.x - position.x;
		Vector2 linearVelocity = (Mathf.Atan2(y, x) * (180f / MathF.PI) + UnityEngine.Random.Range(-10f, 10f)).AngleToDirection() * speed;
		body.linearVelocity = linearVelocity;
		lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		return true;
	}

	public void TinkBounce()
	{
		if (lastBounceFrame != CustomPlayerLoop.FixedUpdateCycle)
		{
			lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
			Vector2 linearVelocity = (body.linearVelocity.DirectionToAngle() + UnityEngine.Random.Range(150f, 220f)).AngleToDirection() * speed;
			body.linearVelocity = linearVelocity;
		}
	}

	public void Break()
	{
		Stop();
		breakEffects.SetActive(value: true);
		recycleTimer = 2f;
	}

	private void Stop()
	{
		body.linearVelocity = Vector2.zero;
		sprite.enabled = false;
		inactiveOnBreak.SetAllActive(value: false);
		hitsLeft = 0;
		breakTimer = 0f;
		OnStop.Invoke();
	}

	private void DoCogRoll(Collision2DUtils.Collision2DSafeContact contact)
	{
		Stop();
		recycleTimer = 0f;
		inertCog.gameObject.SetActive(value: true);
		Rigidbody2D component = inertCog.GetComponent<Rigidbody2D>();
		float num = UnityEngine.Random.Range(3f, 8f);
		component.linearVelocity = new Vector2((contact.Normal.x > 0f) ? num : (0f - num), 10f);
		cleanupQueue.Add(this);
		if (cleanupQueue.Count > 3)
		{
			ToolRing toolRing = cleanupQueue[0];
			cleanupQueue.RemoveAt(0);
			toolRing.inertCog.MarkForCleanup();
		}
	}
}
