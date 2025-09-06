using UnityEngine;

public class EnemyHitEffectsBlackKnight : MonoBehaviour, IHitEffectReciever
{
	public Vector3 effectOrigin;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent enemyDamage;

	[Space]
	public GameObject hitFlashOrange;

	public GameObject hitPuffLarge;

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
				spriteFlash.flashInfected();
			}
			hitFlashOrange.Spawn(base.transform.position + effectOrigin);
			GameObject gameObject = hitPuffLarge.Spawn(base.transform.position + effectOrigin);
			switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
			{
			case 0:
				gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
				break;
			case 2:
				gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
				break;
			case 1:
				gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
				break;
			case 3:
				gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
				break;
			}
			didFireThisFrame = true;
		}
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}
}
