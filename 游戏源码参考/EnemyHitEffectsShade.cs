using UnityEngine;

public class EnemyHitEffectsShade : MonoBehaviour, IHitEffectReciever
{
	public Vector3 effectOrigin;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent hollowShadeStartled;

	public AudioEvent heroDamage;

	[Space]
	public GameObject hitFlashBlack;

	public GameObject hitShade;

	public GameObject slashEffectGhostDark1;

	public GameObject slashEffectGhostDark2;

	public GameObject slashEffectShade;

	private tk2dSprite sprite;

	private bool didFireThisFrame;

	private void Awake()
	{
		sprite = GetComponent<tk2dSprite>();
	}

	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (!didFireThisFrame)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", isRecursive: true);
			hollowShadeStartled.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			heroDamage.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			sprite.color = Color.black;
			SendMessage("ColorReturnNeutral");
			hitFlashBlack.Spawn(base.transform.position + effectOrigin);
			GameObject gameObject = hitShade.Spawn(base.transform.position + effectOrigin);
			float minInclusive = 1f;
			float maxInclusive = 1f;
			float minInclusive2 = 0f;
			float maxInclusive2 = 360f;
			switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
			{
			case 2:
			{
				gameObject.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				minInclusive = -1f;
				maxInclusive = -1.75f;
				minInclusive2 = -30f;
				maxInclusive2 = 30f;
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhostDark1;
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
				config.Prefab = slashEffectGhostDark2;
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
			case 0:
			{
				gameObject.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				minInclusive = 1f;
				maxInclusive = 1.75f;
				minInclusive2 = -30f;
				maxInclusive2 = 30f;
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhostDark1;
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
				config.Prefab = slashEffectGhostDark2;
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
			case 1:
			{
				gameObject.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
				minInclusive = 1f;
				maxInclusive = 1.75f;
				minInclusive2 = 60f;
				maxInclusive2 = 120f;
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhostDark1;
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
				config.Prefab = slashEffectGhostDark2;
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
				gameObject.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
				minInclusive = 1f;
				maxInclusive = 1.75f;
				minInclusive2 = -60f;
				maxInclusive2 = -120f;
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = slashEffectGhostDark1;
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
				config.Prefab = slashEffectGhostDark2;
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
			for (int i = 0; i < 3; i++)
			{
				GameObject obj = slashEffectShade.Spawn(base.transform.position + effectOrigin);
				obj.transform.SetScaleX(Random.Range(minInclusive, maxInclusive));
				obj.transform.SetRotation2D(Random.Range(minInclusive2, maxInclusive2));
			}
			didFireThisFrame = true;
		}
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}
}
