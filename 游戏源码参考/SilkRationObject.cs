using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SilkRationObject : CurrencyObject<SilkRationObject>, IInitialisable, IBreakOnContact
{
	[Space]
	[SerializeField]
	[FormerlySerializedAs("value")]
	[Obsolete]
	[HideInInspector]
	private int silkPerHit = 1;

	[SerializeField]
	private int[] hitSilk = new int[1] { 1 };

	[Space]
	[SerializeField]
	private string idleAnim;

	[SerializeField]
	private string airAnim;

	[SerializeField]
	private string hitAnim;

	[SerializeField]
	private GameObject corePrefab;

	[SerializeField]
	private FlingUtils.ObjectFlingParams coreFlingParams;

	[SerializeField]
	private GameObject[] collectSpawnEffects;

	[SerializeField]
	private GameObject silkGetEffect;

	private int hits;

	private tk2dSpriteAnimator anim;

	private SpinSelf spin;

	private bool hasAwaken;

	private bool hasOnStarted;

	private bool ensureSpawn;

	protected override CurrencyType? CurrencyType => null;

	GameObject IInitialisable.gameObject => base.gameObject;

	private void OnValidate()
	{
		if (silkPerHit > 0)
		{
			hitSilk = new int[1] { silkPerHit };
			silkPerHit = 0;
		}
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		GameObject ownerObj = base.gameObject;
		if (corePrefab != null)
		{
			ensureSpawn = true;
			PersonalObjectPool.EnsurePooledInScene(ownerObj, corePrefab, 1, finished: false, initialiseSpawned: true);
		}
		if (collectSpawnEffects != null)
		{
			GameObject[] array = collectSpawnEffects;
			foreach (GameObject gameObject in array)
			{
				if (!(gameObject == null))
				{
					ensureSpawn = true;
					PersonalObjectPool.EnsurePooledInScene(ownerObj, gameObject, 1, finished: false, initialiseSpawned: true);
				}
			}
		}
		if (silkGetEffect != null)
		{
			ensureSpawn = true;
			PersonalObjectPool.EnsurePooledInScene(ownerObj, silkGetEffect, 1, finished: false, initialiseSpawned: true);
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasOnStarted)
		{
			return false;
		}
		hasOnStarted = true;
		if (ensureSpawn)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		}
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		OnAwake();
		anim = GetComponent<tk2dSpriteAnimator>();
		spin = GetComponent<SpinSelf>();
	}

	private new void Start()
	{
		OnStart();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		anim.Play(airAnim);
	}

	protected override bool Collected()
	{
		Transform transform = base.transform;
		AddSilk();
		hits++;
		bool num = hits >= hitSilk.Length;
		if (silkGetEffect != null)
		{
			silkGetEffect.Spawn(transform.position, transform.rotation);
		}
		collectSpawnEffects.SpawnAll((Vector2)base.transform.position);
		if (!num)
		{
			anim.Play(hitAnim);
			FlingUtils.FlingObject(coreFlingParams.GetSelfConfig(base.gameObject), transform, Vector3.zero);
			spin.ReSpin();
			return false;
		}
		if (!corePrefab)
		{
			return true;
		}
		GameObject flingObject = corePrefab.Spawn(transform.position, transform.rotation * corePrefab.transform.rotation);
		FlingUtils.FlingObject(coreFlingParams.GetSelfConfig(flingObject), transform, Vector3.zero);
		return true;
	}

	public void AddSilk()
	{
		int num = Mathf.Clamp(hits, 0, hitSilk.Length - 1);
		_ = hitSilk[num];
		HeroController.instance.AddSilk(1, heroEffect: true);
	}

	protected override void Land()
	{
		base.Land();
		if (hits <= 0)
		{
			tk2dSpriteAnimationClip clipByName = anim.GetClipByName(idleAnim);
			if (clipByName != null)
			{
				anim.PlayFromFrame(clipByName, UnityEngine.Random.Range(0, clipByName.frames.Length));
			}
		}
	}

	protected override void HandleHeroEnter(Collider2D collision, GameObject sender)
	{
	}
}
