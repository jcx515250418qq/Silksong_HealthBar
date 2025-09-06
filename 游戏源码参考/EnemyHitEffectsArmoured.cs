using UnityEngine;

public class EnemyHitEffectsArmoured : MonoBehaviour, IHitEffectReciever
{
	public Vector3 effectOrigin;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent enemyDamage;

	[Space]
	public GameObject dustHit;

	public GameObject armourHit;

	private SpriteFlash spriteFlash;

	private bool didFireThisFrame;

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
		enemyDamage.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		if ((bool)spriteFlash)
		{
			spriteFlash.flashArmoured();
		}
		GameObject gameObject = (dustHit ? dustHit.Spawn(base.transform.position + effectOrigin) : null);
		if ((bool)gameObject)
		{
			gameObject.transform.SetPositionZ(-0.01f);
		}
		switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
		{
		case 0:
			if ((bool)gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
			}
			if ((bool)armourHit)
			{
				FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT R");
			}
			break;
		case 2:
			if ((bool)gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
			}
			if ((bool)armourHit)
			{
				FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT L");
			}
			break;
		case 1:
			if ((bool)gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
			}
			if ((bool)armourHit)
			{
				FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT U");
			}
			break;
		case 3:
			if ((bool)gameObject)
			{
				gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
			}
			if ((bool)armourHit)
			{
				FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT D");
			}
			break;
		}
		didFireThisFrame = true;
	}

	protected void Update()
	{
		didFireThisFrame = false;
	}
}
