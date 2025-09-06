using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class SurfaceWaterRegion : MonoBehaviour
{
	private const float ORIGINAL_HERO_OFFSET = 0.4f;

	[Header("Main Config")]
	[SerializeField]
	private Color color;

	[Space]
	[SerializeField]
	private float flowSpeed;

	[SerializeField]
	private bool useSpaAnims;

	[Header("Prefab")]
	[SerializeField]
	private GameObject splashInPrefab;

	[SerializeField]
	private GameObject spatterPrefab;

	[SerializeField]
	private Dripper dripperPrefab;

	[SerializeField]
	private AudioEvent splashSound;

	[SerializeField]
	private AudioEvent bigSplashSound;

	[SerializeField]
	private CameraShakeTarget bigSplashShake;

	[SerializeField]
	private GameObject splashOutPrefab;

	[SerializeField]
	private AudioEvent exitSound;

	[SerializeField]
	private GameObject fireNailExtinguishPrefab;

	private Collider2D heroColliderInside;

	private CameraTarget cameraTarget;

	private bool isHeroInside;

	private float heroSurfaceY;

	private readonly List<GameObject> spawnedObjects = new List<GameObject>();

	private bool angled;

	private BoxCollider2D col;

	private static readonly UniqueList<SurfaceWaterRegion> INSIDE_REGIONS = new UniqueList<SurfaceWaterRegion>();

	private const float enterTopThreshold = 0.5f;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public float FlowSpeed => flowSpeed;

	public bool UseSpaAnims => useSpaAnims;

	private BoxCollider2D Col
	{
		get
		{
			if (col == null)
			{
				col = GetComponent<BoxCollider2D>();
			}
			return col;
		}
	}

	public Bounds Bounds => Col.bounds;

	public static int InsideCount => INSIDE_REGIONS.Count;

	public event Action<HeroController> HeroEntered;

	public event Action<HeroController> HeroExited;

	public event Action<Vector2> CorpseEntered;

	private void Start()
	{
		heroSurfaceY = base.transform.position.y + 0.4f;
		if (!base.transform.eulerAngles.z.IsWithinTolerance(0.1f, 0f))
		{
			angled = true;
		}
		cameraTarget = GameCameras.instance.cameraTarget;
	}

	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, spatterPrefab, 100, finished: false);
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, splashInPrefab, 5, finished: false);
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, splashOutPrefab, 5, finished: false);
		if ((bool)fireNailExtinguishPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, fireNailExtinguishPrefab, 2, finished: false);
		}
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, dripperPrefab.gameObject, 5);
	}

	private void OnDisable()
	{
		if (isHeroInside)
		{
			INSIDE_REGIONS.Remove(this);
		}
		isHeroInside = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		HeroController hero = collision.gameObject.GetComponent<HeroController>();
		if (!hero || !hero.isHeroInPosition || hero.cState.hazardDeath || hero.cState.hazardRespawning || hero.playerData.isInvincible)
		{
			return;
		}
		if (hero.cState.isBinding && !Gameplay.SpellCrest.IsEquipped)
		{
			Rigidbody2D component = hero.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				float? y = 0f;
				component.SetVelocity(null, y);
			}
			hero.transform.Translate(new Vector3(0f, 0.2f), Space.World);
			return;
		}
		heroColliderInside = collision;
		if (GameManager.instance.HasFinishedEnteringScene)
		{
			DoTriggerEnter(hero);
			return;
		}
		GameManager.EnterSceneEvent temp = null;
		temp = delegate
		{
			if ((bool)heroColliderInside)
			{
				DoTriggerEnter(hero);
			}
			GameManager.instance.OnFinishedEnteringScene -= temp;
		};
		GameManager.instance.OnFinishedEnteringScene += temp;
	}

	private void DoTriggerEnter(HeroController hero)
	{
		HeroWaterController component = hero.GetComponent<HeroWaterController>();
		if (!component.IsInWater)
		{
			if (!angled && CheckBelowWaterSurface(hero))
			{
				component.Rejected();
			}
			else
			{
				DoSplashHeroIn(hero, component);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (heroColliderInside == collision)
		{
			heroColliderInside = null;
		}
		if (isHeroInside)
		{
			HeroController component = collision.gameObject.GetComponent<HeroController>();
			if ((bool)component)
			{
				DoSplashHeroOut(component);
			}
		}
	}

	public bool CheckBelowWaterSurface(HeroController hero)
	{
		float num = heroSurfaceY - 0.5f;
		Vector3 position = hero.transform.position;
		if (position.y >= num)
		{
			return false;
		}
		Vector2[] positionHistory = hero.PositionHistory;
		for (int i = 0; i < positionHistory.Length; i++)
		{
			if (positionHistory[i].y >= num)
			{
				return false;
			}
		}
		Bounds bounds = Bounds;
		if (position.x > bounds.min.x + 0.5f && position.x < bounds.max.x - 0.5f)
		{
			return false;
		}
		return true;
	}

	private void DoSplashHeroIn(HeroController hero, HeroWaterController waterControl)
	{
		if (!isHeroInside)
		{
			INSIDE_REGIONS.Add(this);
			isHeroInside = true;
		}
		bool isBigSplash = hero.cState.willHardLand || hero.cState.isScrewDownAttacking;
		Transform transform = hero.transform;
		if (!angled)
		{
			transform.SetPositionY(heroSurfaceY);
		}
		if (angled)
		{
			cameraTarget.SetUpdraft(active: true);
		}
		DoSplashIn(transform, isBigSplash);
		FSMUtility.SendEventToGlobalGameObject("Inventory", "INVENTORY CANCEL");
		if ((bool)waterControl)
		{
			waterControl.EnterWaterRegion(this);
		}
		NailElements currentElement = hero.NailImbuement.CurrentElement;
		if (currentElement != 0 && currentElement != NailElements.Poison)
		{
			hero.NailImbuement.SetElement(NailElements.None);
			if (currentElement == NailElements.Fire && (bool)fireNailExtinguishPrefab)
			{
				fireNailExtinguishPrefab.Spawn(hero.transform.position);
			}
		}
		this.HeroEntered?.Invoke(hero);
	}

	private void DoSplashHeroOut(HeroController hero)
	{
		if (isHeroInside)
		{
			INSIDE_REGIONS.Remove(this);
		}
		isHeroInside = false;
		DoSplashOut(hero.transform, new Vector3(0f, -0.5f, -0.001f));
		HeroWaterController component = hero.GetComponent<HeroWaterController>();
		if ((bool)component)
		{
			component.ExitWaterRegion();
		}
		if (angled)
		{
			cameraTarget.SetUpdraft(active: false);
		}
		if (this.HeroExited != null)
		{
			this.HeroExited(hero);
		}
	}

	public void DoSplashIn(Transform obj, bool isBigSplash)
	{
		DoSplashIn(obj, Vector3.zero, isBigSplash);
	}

	public void DoSplashIn(Transform obj, Vector3 localPosition, bool isBigSplash)
	{
		spawnedObjects.Clear();
		Vector3 position = obj.TransformPoint(localPosition);
		if (!isBigSplash)
		{
			position = new Vector3(position.x, position.y + 0.3f, position.z - 0.001f);
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = spatterPrefab;
			config.AmountMin = 12;
			config.AmountMax = 15;
			config.SpeedMin = 8f;
			config.SpeedMax = 22f;
			config.AngleMin = 80f;
			config.AngleMax = 100f;
			config.OriginVariationX = 0.75f;
			config.OriginVariationY = 0f;
			FlingUtils.SpawnAndFling(config, obj, localPosition, spawnedObjects);
			splashSound.SpawnAndPlayOneShot(position);
			GameObject gameObject = splashInPrefab.Spawn(position);
			gameObject.transform.SetScaleMatching(2f);
			spawnedObjects.Add(gameObject);
		}
		else
		{
			bigSplashShake.DoShake(this);
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = spatterPrefab;
			config.AmountMin = 60;
			config.AmountMax = 60;
			config.SpeedMin = 10f;
			config.SpeedMax = 30f;
			config.AngleMin = 80f;
			config.AngleMax = 100f;
			config.OriginVariationX = 1f;
			config.OriginVariationY = 0f;
			FlingUtils.SpawnAndFling(config, obj, localPosition + new Vector3(0f, 0f, 0f), spawnedObjects);
			bigSplashSound.SpawnAndPlayOneShot(position);
			GameObject gameObject2 = splashInPrefab.Spawn(position);
			gameObject2.transform.SetScaleMatching(2.5f);
			spawnedObjects.Add(gameObject2);
			GameObject gameObject3 = splashOutPrefab.Spawn(position);
			gameObject3.transform.SetScaleMatching(2f);
			spawnedObjects.Add(gameObject3);
		}
		SetSpawnedGameObjectColorsTemp(spawnedObjects);
		spawnedObjects.Clear();
	}

	public void DoSplashInSmall(Transform obj, Vector3 localPosition)
	{
		spawnedObjects.Clear();
		Vector3 position = obj.TransformPoint(localPosition);
		FlingUtils.Config config = default(FlingUtils.Config);
		config.Prefab = spatterPrefab;
		config.AmountMin = 3;
		config.AmountMax = 4;
		config.SpeedMin = 6f;
		config.SpeedMax = 15f;
		config.AngleMin = 80f;
		config.AngleMax = 100f;
		config.OriginVariationX = 0.75f;
		config.OriginVariationY = 0f;
		FlingUtils.SpawnAndFling(config, obj, localPosition, spawnedObjects);
		splashSound.SpawnAndPlayOneShot(position);
		GameObject gameObject = splashInPrefab.Spawn(position);
		gameObject.transform.SetScaleMatching(1.25f);
		spawnedObjects.Add(gameObject);
		SetSpawnedGameObjectColorsTemp(spawnedObjects);
		spawnedObjects.Clear();
	}

	public void DoSplashOut(Transform obj, Vector2 effectsOffset)
	{
		spawnedObjects.Clear();
		exitSound.SpawnAndPlayOneShot(obj.position);
		if ((bool)dripperPrefab)
		{
			Dripper dripper = dripperPrefab.Spawn(obj.transform.position);
			dripper.StartDripper(obj);
			spawnedObjects.Add(dripper.gameObject);
		}
		spawnedObjects.Add(splashOutPrefab.Spawn(obj.position + (Vector3)effectsOffset));
		SetSpawnedGameObjectColorsTemp(spawnedObjects);
	}

	public void CorpseSplashedIn(Vector2 splashPos)
	{
		this.CorpseEntered?.Invoke(splashPos);
	}

	private void SetSpawnedGameObjectColorsTemp(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			RecycleResetHandler recycleResetHandler = gameObject.GetComponent<RecycleResetHandler>() ?? gameObject.AddComponent<RecycleResetHandler>();
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
			if ((bool)sprite)
			{
				Color initialColor = sprite.color;
				sprite.color = color;
				recycleResetHandler.AddTempAction((Action)delegate
				{
					sprite.color = initialColor;
				});
			}
			tk2dSprite tk2dSprite2 = gameObject.GetComponent<tk2dSprite>();
			if ((bool)tk2dSprite2)
			{
				Color initialColor2 = tk2dSprite2.color;
				tk2dSprite2.color = color;
				recycleResetHandler.AddTempAction((Action)delegate
				{
					tk2dSprite2.color = initialColor2;
				});
			}
			Dripper dripper = gameObject.GetComponent<Dripper>();
			if ((bool)dripper)
			{
				dripper.OnSpawned += SetSpawnedGameObjectColorsTemp;
				recycleResetHandler.AddTempAction((Action)delegate
				{
					dripper.OnSpawned -= SetSpawnedGameObjectColorsTemp;
				});
			}
		}
	}

	private bool TryReentryInternal(HeroWaterController waterControl, HeroController hero)
	{
		if (!angled && CheckBelowWaterSurface(hero))
		{
			waterControl.Rejected();
			return false;
		}
		DoSplashHeroIn(hero, waterControl);
		return true;
	}

	public static void TryReentry(HeroWaterController waterControl, HeroController hero)
	{
		INSIDE_REGIONS.ReserveListUsage();
		foreach (SurfaceWaterRegion item in INSIDE_REGIONS.List)
		{
			if (item.TryReentryInternal(waterControl, hero))
			{
				return;
			}
		}
		INSIDE_REGIONS.ReleaseListUsage();
	}

	private void DrawGizmos()
	{
		float y = base.transform.position.y + 0.4f;
		Bounds bounds = Bounds;
		Gizmos.color = Color.blue;
		GizmoUtility.DrawCollider2D(col);
		Gizmos.color = Color.green;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		min.y = y;
		max.y = y;
		Gizmos.DrawLine(min, max);
		Gizmos.color = Color.red;
		min.y -= 0.5f;
		max.y -= 0.5f;
		Gizmos.DrawLine(min, max);
	}

	private void OnDrawGizmosSelected()
	{
		DrawGizmos();
	}
}
