using UnityEngine;

public class InfectedEnemyEffects : MonoBehaviour, IHitEffectReciever
{
	private SpriteFlash spriteFlash;

	[SerializeField]
	private Vector3 effectOrigin;

	[SerializeField]
	private AudioEvent impactAudio;

	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private GameObject hitFlashOrangePrefab;

	[SerializeField]
	private GameObject spatterOrangePrefab;

	[SerializeField]
	private GameObject hitPuffPrefab;

	[SerializeField]
	private bool noBlood;

	private bool didFireThisFrame;

	protected void Reset()
	{
		impactAudio.Reset();
	}

	protected void Awake()
	{
		spriteFlash = GetComponent<SpriteFlash>();
	}

	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		if (didFireThisFrame)
		{
			return;
		}
		if (spriteFlash != null)
		{
			spriteFlash.flashInfected();
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", isRecursive: true);
		impactAudio.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		hitFlashOrangePrefab.Spawn(base.transform.TransformPoint(effectOrigin));
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			if (!noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 3, 4, 10f, 15f, 120f, 150f, null);
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 8, 15, 10f, 25f, 30f, 60f, null);
			}
			hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(0f, 90f, 270f));
			break;
		case 2:
			if (!noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 3, 4, 10f, 15f, 30f, 60f, null);
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 8, 10, 15f, 25f, 120f, 150f, null);
			}
			hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(180f, 90f, 270f));
			break;
		case 1:
			if (!noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 8, 10, 20f, 30f, 80f, 100f, null);
			}
			hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(270f, 90f, 270f));
			break;
		case 3:
			if (!noBlood)
			{
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 4, 5, 15f, 25f, 140f, 180f, null);
				BloodSpawner.SpawnBlood(base.transform.position + effectOrigin, 4, 5, 15f, 25f, 360f, 400f, null);
			}
			hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(-72.5f, -180f, -180f));
			break;
		}
		didFireThisFrame = true;
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}
}
