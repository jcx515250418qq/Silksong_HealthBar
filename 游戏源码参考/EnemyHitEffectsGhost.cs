using UnityEngine;

public class EnemyHitEffectsGhost : MonoBehaviour, IHitEffectReciever
{
	public Vector3 effectOrigin;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent enemyDamage;

	[Space]
	public GameObject ghostHitPt;

	public GameObject slashEffectGhost1;

	public GameObject slashEffectGhost2;

	private SpriteFlash spriteFlash;

	private bool didFireThisFrame;

	protected void Awake()
	{
		spriteFlash = GetComponent<SpriteFlash>();
	}

	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (!didFireThisFrame)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", isRecursive: true);
			enemyDamage.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			if ((bool)spriteFlash)
			{
				spriteFlash.FlashEnemyHit();
			}
			GameObject gameObject = ghostHitPt.Spawn(base.transform.position + effectOrigin);
			switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
			{
			case 0:
			{
				gameObject.transform.SetRotation2D(-22.5f);
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost1;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = -40f;
				config.AngleMax = 40f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost2;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = -40f;
				config.AngleMax = 40f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				break;
			}
			case 2:
			{
				gameObject.transform.SetRotation2D(160f);
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost1;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 140f;
				config.AngleMax = 220f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost2;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 140f;
				config.AngleMax = 220f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				break;
			}
			case 1:
			{
				gameObject.transform.SetRotation2D(70f);
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost1;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 50f;
				config.AngleMax = 130f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost2;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 50f;
				config.AngleMax = 130f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				break;
			}
			case 3:
			{
				gameObject.transform.SetRotation2D(-110f);
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost1;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 230f;
				config.AngleMax = 310f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhost2;
				config.AmountMin = 2;
				config.AmountMax = 3;
				config.SpeedMin = 20f;
				config.SpeedMax = 35f;
				config.AngleMin = 230f;
				config.AngleMax = 310f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.SpawnAndFling(config, base.transform, effectOrigin);
				break;
			}
			}
			didFireThisFrame = true;
		}
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}
}
