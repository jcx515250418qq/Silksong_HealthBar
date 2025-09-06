using System;
using System.Collections;
using UnityEngine;

public class ScuttlerControl : MonoBehaviour, IHitResponder
{
	[Header("Instance Variables")]
	public bool startIdle;

	public bool startRunning;

	[Header("Other Variables")]
	public string killedPDBool = "killedOrangeScuttler";

	public string killsPDBool = "killsOrangeScuttler";

	public string newDataPDBool = "newDataOrangeScuttler";

	[Space]
	public string runAnim = "Run";

	public string landAnim = "Land";

	[Space]
	public GameObject corpsePrefab;

	public GameObject splatEffectChild;

	public GameObject journalUpdateMsgPrefab;

	[Space]
	public AudioSource audioSourcePrefab;

	public AudioEvent bounceSound;

	public TriggerEnterEvent heroAlert;

	[Space]
	public bool healthScuttler;

	[Header("Health Scuttler Variables")]
	public GameObject strikeNailPrefab;

	public GameObject slashImpactPrefab;

	public GameObject fireballHitPrefab;

	public AudioEvent deathSound1;

	public AudioEvent deathSound2;

	public GameObject pool;

	public GameObject screenFlash;

	public Color bloodColor;

	private Transform hero;

	private float maxSpeed;

	private float acceleration = 0.3f;

	private bool landed;

	private Coroutine runRoutine;

	private Coroutine bounceRoutine;

	private float rayLength;

	private Vector2 rayOrigin;

	private tk2dSpriteAnimator anim;

	private Rigidbody2D body;

	private AudioSource source;

	private bool alive = true;

	private bool reverseRun;

	private float activateDelay = 0.25f;

	private double activateTime;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
		body = GetComponent<Rigidbody2D>();
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		base.transform.SetScaleMatching(UnityEngine.Random.Range(1.35f, 1.5f));
		maxSpeed = UnityEngine.Random.Range(6f, 9f);
		hero = HeroController.instance.transform;
		activateTime = Time.timeAsDouble + (double)activateDelay;
		Collider2D component = GetComponent<Collider2D>();
		if ((bool)component)
		{
			rayLength = component.bounds.size.x / 2f + 0.1f;
			rayOrigin = component.bounds.center - base.transform.position;
		}
		source.enabled = false;
		if (healthScuttler)
		{
			reverseRun = GameManager.instance.playerData.GetBool("equippedCharm_27");
		}
		if (!startRunning && !startIdle)
		{
			CollisionEnterEvent componentInChildren = GetComponentInChildren<CollisionEnterEvent>();
			if (!componentInChildren)
			{
				return;
			}
			componentInChildren.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D collision)
			{
				if (!landed && direction == CollisionEnterEvent.Direction.Bottom)
				{
					landed = true;
					StartCoroutine(Land());
				}
			};
		}
		else if (startIdle && (bool)heroAlert)
		{
			heroAlert.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
			{
				if (!landed && collider.tag == "Player")
				{
					landed = true;
					StartCoroutine(Land());
				}
			};
		}
		else if (startRunning)
		{
			runRoutine = StartCoroutine(Run());
		}
	}

	private void Update()
	{
		if (alive && Helper.Raycast2D((Vector2)base.transform.position + rayOrigin, new Vector2(Mathf.Sign(body.linearVelocity.x), 0f), rayLength, 256).collider != null && bounceRoutine == null && runRoutine != null)
		{
			StopCoroutine(runRoutine);
			bounceRoutine = StartCoroutine((body.linearVelocity.x > 0f) ? Bounce(110f, 130f) : Bounce(50f, 70f));
		}
	}

	private IEnumerator Land()
	{
		yield return StartCoroutine(anim.PlayAnimWait(landAnim));
		source.enabled = true;
		runRoutine = StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		anim.Play(runAnim);
		source.enabled = true;
		Vector3 velocity = body.linearVelocity;
		while (true)
		{
			float num = Mathf.Sign(hero.position.x - base.transform.position.x) * (float)((!reverseRun) ? 1 : (-1));
			float currentDirection = num;
			base.transform.SetScaleX(Mathf.Abs(base.transform.localScale.x) * num);
			while (currentDirection == num)
			{
				velocity.x += acceleration * (0f - num);
				velocity.x = Mathf.Clamp(velocity.x, 0f - maxSpeed, maxSpeed);
				velocity.y = body.linearVelocity.y;
				body.linearVelocity = velocity;
				yield return null;
				num = Mathf.Sign(hero.position.x - base.transform.position.x) * (float)((!reverseRun) ? 1 : (-1));
			}
			yield return null;
		}
	}

	private IEnumerator Bounce(float angleMin, float angleMax)
	{
		source.enabled = false;
		bounceSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		Vector2 zero = Vector2.zero;
		float num = UnityEngine.Random.Range(angleMin, angleMax);
		zero.x = 5f * Mathf.Cos(num * (MathF.PI / 180f));
		zero.y = 5f * Mathf.Sin(num * (MathF.PI / 180f));
		body.linearVelocity = zero;
		yield return new WaitForSeconds(0.5f);
		source.enabled = true;
		bounceRoutine = null;
		runRoutine = StartCoroutine(Run());
	}

	private IEnumerator Heal()
	{
		Action doHeal = null;
		doHeal = delegate
		{
			GameManager instance = GameManager.instance;
			instance.AddBlueHealthQueued();
			instance.UnloadingLevel -= doHeal;
			doHeal = null;
		};
		GameManager.instance.UnloadingLevel += doHeal;
		if ((bool)HeroController.instance && Vector2.Distance(base.transform.position, HeroController.instance.transform.position) > 40f)
		{
			base.gameObject.SetActive(value: false);
		}
		yield return new WaitForSeconds(1.2f);
		if ((bool)screenFlash)
		{
			GameObject obj = screenFlash.Spawn();
			obj.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 0.7f, 1f));
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(obj, "Fade Away");
			if ((bool)playMakerFSM)
			{
				FSMUtility.SetFloat(playMakerFSM, "Alpha", 0.75f);
			}
		}
		if (doHeal != null)
		{
			doHeal();
		}
		base.gameObject.SetActive(value: false);
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (Time.timeAsDouble < activateTime)
		{
			return IHitResponder.Response.None;
		}
		if (alive)
		{
			alive = false;
			if (runRoutine != null)
			{
				StopCoroutine(runRoutine);
			}
			if (bounceRoutine != null)
			{
				StopCoroutine(bounceRoutine);
			}
			if ((bool)corpsePrefab)
			{
				UnityEngine.Object.Instantiate(corpsePrefab, base.transform.position, base.transform.rotation);
			}
			if ((bool)splatEffectChild)
			{
				splatEffectChild.SetActive(value: true);
			}
			PlayerData playerData = GameManager.instance.playerData;
			if (playerData.GetBool("hasJournal"))
			{
				if (!playerData.GetBool(killedPDBool))
				{
					playerData.SetBool(killedPDBool, value: true);
					if ((bool)journalUpdateMsgPrefab)
					{
						UnityEngine.Object.Instantiate(journalUpdateMsgPrefab);
					}
				}
				int @int = playerData.GetInt(killsPDBool);
				if (@int > 0)
				{
					@int--;
					playerData.SetInt(killsPDBool, @int);
					if (@int <= 0 && (bool)journalUpdateMsgPrefab)
					{
						PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(UnityEngine.Object.Instantiate(journalUpdateMsgPrefab), "Journal Msg");
						if ((bool)playMakerFSM)
						{
							FSMUtility.SetBool(playMakerFSM, "Full", value: true);
						}
					}
				}
				playerData.SetBool(newDataPDBool, value: true);
			}
			bool flag = false;
			if (!healthScuttler)
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				if (damageInstance.IsNailDamage)
				{
					flag = true;
					if ((bool)strikeNailPrefab)
					{
						strikeNailPrefab.Spawn(base.transform.position);
					}
					if ((bool)slashImpactPrefab)
					{
						GameObject gameObject = slashImpactPrefab.Spawn(base.transform.position);
						float direction = damageInstance.Direction;
						if (direction < 45f)
						{
							gameObject.transform.SetRotation2D(UnityEngine.Random.Range(340f, 380f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
						else if (direction < 135f)
						{
							gameObject.transform.SetRotation2D(UnityEngine.Random.Range(70f, 110f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
						else if (direction < 225f)
						{
							gameObject.transform.SetRotation2D(UnityEngine.Random.Range(340f, 380f));
							gameObject.transform.localScale = new Vector3(-0.9f, 0.9f, 1f);
						}
						else if (direction < 360f)
						{
							gameObject.transform.SetRotation2D(UnityEngine.Random.Range(250f, 290f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
					}
				}
				else if (damageInstance.AttackType == AttackTypes.Spell || damageInstance.AttackType == AttackTypes.NailBeam)
				{
					flag = true;
					if ((bool)fireballHitPrefab)
					{
						GameObject obj = fireballHitPrefab.Spawn(base.transform.position);
						obj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
						obj.transform.SetPositionZ(0.0031f);
					}
				}
				else if (damageInstance.AttackType == AttackTypes.Generic)
				{
					flag = true;
				}
				deathSound1.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
				deathSound2.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
				GameCameras gameCameras = UnityEngine.Object.FindObjectOfType<GameCameras>();
				if ((bool)gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
				BloodSpawner.SpawnBlood(base.transform.position, 12, 18, 4f, 22f, 30f, 150f, bloodColor);
				Renderer component = GetComponent<Renderer>();
				if ((bool)component)
				{
					component.enabled = false;
				}
				if (flag)
				{
					if ((bool)pool)
					{
						pool.transform.SetPositionZ(-0.2f);
						FlingUtils.ChildrenConfig config = default(FlingUtils.ChildrenConfig);
						config.Parent = pool;
						config.AmountMin = 8;
						config.AmountMax = 10;
						config.SpeedMin = 15f;
						config.SpeedMax = 20f;
						config.AngleMin = 30f;
						config.AngleMax = 150f;
						config.OriginVariationX = 0f;
						config.OriginVariationY = 0f;
						FlingUtils.FlingChildren(config, base.transform, Vector3.zero, null);
					}
					StartCoroutine(Heal());
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		return IHitResponder.Response.None;
	}
}
