using GlobalEnums;
using UnityEngine;

public class HeroBox : DebugDrawColliderRuntimeAdder, CustomPlayerLoop.ILateFixedUpdate
{
	private HeroController heroCtrl;

	private DamageHero lastDamageHero;

	private GameObject lastDamagingObject;

	private bool isHitBuffered;

	private int damageDealt;

	private HazardType hazardType;

	private CollisionSide collisionSide;

	private DamagePropertyFlags damagePropertyFlags;

	private BoxCollider2D box;

	public static bool Inactive { get; set; }

	bool CustomPlayerLoop.ILateFixedUpdate.isActiveAndEnabled => base.isActiveAndEnabled;

	protected override void Awake()
	{
		CustomPlayerLoop.RegisterSuperLateFixedUpdate(this);
		base.Awake();
		box = GetComponent<BoxCollider2D>();
		heroCtrl = GetComponentInParent<HeroController>();
	}

	private void OnDestroy()
	{
		CustomPlayerLoop.UnregisterSuperLateFixedUpdate(this);
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (!Inactive)
		{
			CheckForDamage(otherCollider.gameObject);
		}
	}

	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		if (!Inactive)
		{
			CheckForDamage(otherCollider.gameObject);
		}
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Enemy);
	}

	public void CheckForDamage(GameObject otherGameObject)
	{
		if (FSMUtility.ContainsFSM(otherGameObject, "damages_hero"))
		{
			PlayMakerFSM fsm = FSMUtility.LocateFSM(otherGameObject, "damages_hero");
			int @int = FSMUtility.GetInt(fsm, "damageDealt");
			HazardType int2 = (HazardType)FSMUtility.GetInt(fsm, "hazardType");
			heroCtrl.TakeDamage(otherGameObject, (!(otherGameObject.transform.position.x > base.transform.position.x)) ? CollisionSide.left : CollisionSide.right, @int, int2);
		}
		else
		{
			DamageHero component = otherGameObject.GetComponent<DamageHero>();
			TakeDamageFromDamager(component, otherGameObject);
		}
	}

	public void TakeDamageFromDamager(DamageHero damageHero, GameObject damagingObject)
	{
		if (damageHero == null || !damageHero.CanCauseDamage)
		{
			return;
		}
		int num = (damageHero.enabled ? damageHero.damageDealt : 0);
		if (num == 0)
		{
			if (damageHero.enabled && damageHero.forceParry && IsHitTypeBuffered(hazardType))
			{
				hazardType = damageHero.hazardType;
				lastDamageHero = damageHero;
				lastDamagingObject = damagingObject;
				if (!isHitBuffered)
				{
					isHitBuffered = true;
					damageDealt = 0;
				}
			}
			return;
		}
		if (!isHitBuffered || num > damageDealt)
		{
			damageDealt = num;
		}
		hazardType = damageHero.hazardType;
		lastDamageHero = damageHero;
		lastDamagingObject = damagingObject;
		if (damageHero.OverrideCollisionSide)
		{
			collisionSide = damageHero.CollisionSide;
		}
		else
		{
			float num2 = damagingObject.transform.position.x;
			float num3 = base.transform.position.x;
			if (damageHero.InvertCollisionSide)
			{
				float num4 = num3;
				float num5 = num2;
				num2 = num4;
				num3 = num5;
			}
			collisionSide = ((!(num2 > num3)) ? CollisionSide.left : CollisionSide.right);
		}
		damagePropertyFlags = damageHero.damagePropertyFlags;
		if (!IsHitTypeBuffered(hazardType))
		{
			ApplyBufferedHit();
		}
		else
		{
			isHitBuffered = true;
		}
	}

	private static bool IsHitTypeBuffered(HazardType hazardType)
	{
		return hazardType <= HazardType.ENEMY;
	}

	public void LateFixedUpdate()
	{
		if (isHitBuffered)
		{
			ApplyBufferedHit();
		}
	}

	private void ApplyBufferedHit()
	{
		if (damageDealt == 0 && lastDamageHero != null && lastDamageHero.forceParry)
		{
			heroCtrl.CheckParry(lastDamageHero);
		}
		else
		{
			heroCtrl.TakeDamage(lastDamagingObject, collisionSide, damageDealt, hazardType, damagePropertyFlags);
		}
		lastDamageHero = null;
		lastDamagingObject = null;
		isHitBuffered = false;
	}

	public void HeroBoxOff()
	{
		box.enabled = false;
	}

	public void HeroBoxNormal()
	{
		if (heroCtrl.cState.onGround)
		{
			box.offset = new Vector2(0f, -0.3799f);
			box.size = new Vector2(0.4554f, 2.2498f);
		}
		else
		{
			box.offset = new Vector2(0f, -0.14f);
			box.size = new Vector2(0.4554f, 1.77f);
		}
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxDownspike()
	{
		box.offset = new Vector2(0f, -0.1f);
		box.size = new Vector2(0.4554f, 1.218f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxDownDrill()
	{
		box.offset = new Vector2(0f, 0.1f);
		box.size = new Vector2(0.4554f, 1.5f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxSprint()
	{
		box.offset = new Vector2(0f, -0.5844275f);
		box.size = new Vector2(0.4554f, 1.391145f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxAirdash()
	{
		box.offset = new Vector2(0f, -0.4780059f);
		box.size = new Vector2(0.4554f, 1.036407f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxReaperDownSlash()
	{
		box.offset = new Vector2(0.37f, 0.6319103f);
		box.size = new Vector2(0.4554f, 1.601192f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxWallSlide()
	{
		box.offset = new Vector2(0.2101475f, -0.2755051f);
		box.size = new Vector2(0.875695f, 1.280464f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxWallScramble()
	{
		box.offset = new Vector2(0.2101475f, 0.4f);
		box.size = new Vector2(0.875695f, 1.280464f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxWallScuttle()
	{
		box.offset = new Vector2(-0.2101475f, -0.4755051f);
		box.size = new Vector2(0.875695f, 1.280464f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxScuttleDash()
	{
		box.offset = new Vector2(0.11f, -0.34f);
		box.size = new Vector2(1.02f, 0.06f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxScuttle()
	{
		box.offset = new Vector2(0.11f, -0.81f);
		box.size = new Vector2(1.02f, 1f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxParryStance()
	{
		box.offset = new Vector2(-0.017f, -0.14f);
		box.size = new Vector2(1.18f, 1.77f);
		box.enabled = true;
		AddDebugDrawComponent();
	}

	public void HeroBoxHarpoon()
	{
		box.offset = new Vector2(0f, -0.33f);
		box.size = new Vector2(0.4554f, 1.46f);
		box.enabled = true;
		AddDebugDrawComponent();
	}
}
