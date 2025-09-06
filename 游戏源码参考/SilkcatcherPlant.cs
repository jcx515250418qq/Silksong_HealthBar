using System.Collections.Generic;
using UnityEngine;

public class SilkcatcherPlant : MonoBehaviour, IHitResponder, IBreakerBreakable
{
	public List<Sprite> plantSprites;

	public List<Sprite> silkSprites;

	public GameObject breakEffects;

	public GameObject silkGetEffect;

	public GameObject strikePrefab;

	public GameObject slashPrefab;

	public ParticleSystem ptIdle;

	public ParticleSystem ptBreak;

	public SpriteRenderer spriteRenderer;

	public SpriteRenderer silkSpriteRenderer;

	public CameraShakeTarget breakShake;

	[SerializeField]
	private bool doNotRotate;

	[SerializeField]
	private bool doNotFlip;

	private HeroController heroController;

	private bool destroyed;

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Basic;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	private void Awake()
	{
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				val = destroyed;
			};
			component.OnSetSaveState += delegate(bool val)
			{
				if (val)
				{
					base.gameObject.SetActive(value: false);
				}
			};
			component.SemiPersistentReset += delegate
			{
				destroyed = false;
				GetComponent<CircleCollider2D>().enabled = true;
				spriteRenderer.enabled = true;
				silkSpriteRenderer.enabled = true;
				breakEffects.SetActive(value: false);
				ptIdle.Play();
				ptBreak.Stop();
				if ((bool)silkGetEffect)
				{
					silkGetEffect.SetActive(value: false);
				}
			};
		}
		if ((bool)silkGetEffect)
		{
			silkGetEffect.SetActive(value: false);
		}
	}

	private void Start()
	{
		Transform transform = base.transform;
		if (!doNotRotate)
		{
			transform.SetRotationZ(Random.Range(0f, 360f));
		}
		int index = Random.Range(0, plantSprites.Count - 1);
		spriteRenderer.sprite = plantSprites[index];
		silkSpriteRenderer.sprite = silkSprites[index];
		if (!doNotFlip && Random.Range(1, 100) < 50)
		{
			Vector3 localScale = transform.localScale;
			localScale = new Vector3(0f - localScale.x, localScale.y, localScale.z);
			transform.localScale = localScale;
		}
		heroController = GameManager.instance.hero_ctrl;
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		Destroy(damageInstance.IsNailDamage);
		return IHitResponder.Response.GenericHit;
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		Destroy(giveSilk: false);
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		Destroy(giveSilk: false);
	}

	private void Destroy(bool giveSilk)
	{
		GetComponent<CircleCollider2D>().enabled = false;
		spriteRenderer.enabled = false;
		silkSpriteRenderer.enabled = false;
		breakEffects.SetActive(value: true);
		strikePrefab.Spawn(base.transform.position);
		slashPrefab.Spawn(base.transform.position);
		ptIdle.Stop();
		ptBreak.Play();
		if ((bool)silkGetEffect)
		{
			silkGetEffect.SetActive(giveSilk);
		}
		if (!giveSilk)
		{
			return;
		}
		if (!breakShake.TryShake(this))
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("CameraParent");
			if (gameObject != null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "CameraShake");
				if (playMakerFSM != null)
				{
					playMakerFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		heroController.AddSilk(2, heroEffect: true);
		destroyed = true;
	}
}
