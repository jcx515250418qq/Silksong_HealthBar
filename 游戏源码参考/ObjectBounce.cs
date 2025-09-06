using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class ObjectBounce : MonoBehaviour
{
	public delegate void BounceEvent();

	[Tooltip("1.0 = full bounce, 0.5 = half bounce, >1 makes the bounce increase speed")]
	public float bounceFactor;

	[Tooltip("If object's speed is below this, don't bounce")]
	public float speedThreshold = 1f;

	public bool doNotAffectX;

	public bool doNotAffectY;

	public bool playSound;

	public float playSoundCooldown;

	[Range(0f, 100f)]
	public int chanceToPlay = 100;

	public AudioClip[] clips;

	public RandomAudioClipTable randomAudioClipTable;

	public float pitchMin = 1f;

	public float pitchMax = 1f;

	public bool playAnimationOnBounce;

	public string animationName;

	public float animPause = 0.5f;

	public bool sendFSMEvent;

	[Space]
	public int recycleAfterBounces;

	[Space]
	public int heavyBounceSpeed;

	[Space]
	public UnityEvent OnBounced;

	[Space]
	public UnityEvent OnHeavyBounce;

	private float speed;

	private float animTimer;

	private tk2dSpriteAnimator animator;

	private PlayMakerFSM fsm;

	private Vector2 velocity;

	private Vector2 lastPos;

	private Rigidbody2D rb;

	private AudioSource audio;

	private int chooser;

	private bool isMoving;

	private int bounces;

	private float stoppedMovingTimer;

	private double playSoundCooldownTime;

	private float recycleTimer;

	private bool enteredWater;

	private bool bouncing = true;

	private bool hasRB2D;

	private bool started;

	public event BounceEvent Bounced;

	public event BounceEvent StartedMoving;

	public event BounceEvent StoppedMoving;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		audio = GetComponent<AudioSource>();
		animator = GetComponent<tk2dSpriteAnimator>();
		hasRB2D = rb != null;
		if (sendFSMEvent)
		{
			fsm = GetComponent<PlayMakerFSM>();
		}
		started = true;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate += OnUpdate;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate += OnUpdate;
			ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
		}
		isMoving = false;
		bounces = 0;
		recycleTimer = 0f;
		bouncing = true;
		stoppedMovingTimer = 0f;
		enteredWater = false;
	}

	private void OnDisable()
	{
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate -= OnUpdate;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate -= OnFixedUpdate;
	}

	private void OnUpdate()
	{
		if (stoppedMovingTimer > 0f)
		{
			stoppedMovingTimer -= Time.deltaTime;
			if (stoppedMovingTimer <= 0f)
			{
				if (this.StoppedMoving != null)
				{
					this.StoppedMoving();
				}
				if (recycleAfterBounces > 0)
				{
					DropAndRecycle();
				}
			}
		}
		bool flag = isMoving;
		isMoving = speed > 0.1f;
		if (!isMoving && flag)
		{
			stoppedMovingTimer = 0.5f;
		}
		else if (isMoving && !flag)
		{
			if (stoppedMovingTimer <= 0f && this.StartedMoving != null)
			{
				this.StartedMoving();
			}
			stoppedMovingTimer = 0f;
		}
		if (animTimer > 0f)
		{
			animTimer -= Time.deltaTime;
		}
		if (recycleTimer <= 0f)
		{
			return;
		}
		recycleTimer -= Time.deltaTime;
		if (!(recycleTimer > 0f))
		{
			PolygonCollider2D component = GetComponent<PolygonCollider2D>();
			if ((bool)component)
			{
				component.enabled = true;
			}
			BoxCollider2D component2 = GetComponent<BoxCollider2D>();
			if ((bool)component2)
			{
				component2.enabled = true;
			}
			CircleCollider2D component3 = GetComponent<CircleCollider2D>();
			if ((bool)component3)
			{
				component3.enabled = true;
			}
			StartBounce();
			base.gameObject.Recycle();
		}
	}

	private void OnFixedUpdate()
	{
		if (bouncing)
		{
			Vector2 vector = base.transform.position;
			velocity = vector - lastPos;
			lastPos = vector;
			if (!hasRB2D || !rb.IsAwake())
			{
				speed = 0f;
			}
			else
			{
				speed = rb.linearVelocity.magnitude;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (!hasRB2D || rb.isKinematic || !bouncing)
		{
			return;
		}
		if (speed >= speedThreshold)
		{
			if (Math.Abs(bounceFactor) > Mathf.Epsilon)
			{
				Vector3 inNormal = col.GetSafeContact().Normal;
				Vector3 normalized = Vector3.Reflect(velocity.normalized, inNormal).normalized;
				Vector2 linearVelocity = new Vector2(normalized.x, normalized.y) * (speed * (bounceFactor * UnityEngine.Random.Range(0.8f, 1.2f)));
				if (doNotAffectX)
				{
					rb.linearVelocity = new Vector2(rb.linearVelocity.x, linearVelocity.y);
				}
				else if (doNotAffectY)
				{
					rb.linearVelocity = new Vector2(linearVelocity.x, rb.linearVelocity.y);
				}
				else
				{
					rb.linearVelocity = linearVelocity;
				}
			}
			if (!GameManager.IsWaitingForSceneReady && !enteredWater)
			{
				if ((bool)randomAudioClipTable)
				{
					randomAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				}
				else if (playSound && Time.timeAsDouble >= playSoundCooldownTime)
				{
					playSoundCooldownTime = Time.timeAsDouble + (double)playSoundCooldown;
					chooser = UnityEngine.Random.Range(1, 100);
					if (chooser <= chanceToPlay)
					{
						int num = UnityEngine.Random.Range(0, clips.Length);
						AudioClip audioClip = clips[num];
						if (audioClip != null)
						{
							if (audio != null)
							{
								float pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
								audio.pitch = pitch;
								audio.PlayOneShot(audioClip);
							}
							else if (audioClip != null)
							{
								AudioEvent audioEvent = default(AudioEvent);
								audioEvent.Clip = audioClip;
								audioEvent.PitchMin = pitchMin;
								audioEvent.PitchMax = pitchMax;
								AudioEvent audioEvent2 = audioEvent;
								audioEvent2.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position);
							}
						}
					}
				}
			}
			if (playAnimationOnBounce && animTimer <= 0f)
			{
				animator.Play(animationName);
				animator.PlayFromFrame(0);
				animTimer = animPause;
			}
			if (sendFSMEvent && (bool)fsm)
			{
				fsm.SendEvent("BOUNCE");
			}
			if (this.Bounced != null)
			{
				this.Bounced();
			}
			if (recycleAfterBounces > 0)
			{
				bounces++;
				if (bounces >= recycleAfterBounces)
				{
					DropAndRecycle();
				}
			}
			OnBounced.Invoke();
			if (speed >= (float)heavyBounceSpeed)
			{
				OnHeavyBounce.Invoke();
			}
		}
		else if (recycleAfterBounces > 0)
		{
			DropAndRecycle();
		}
	}

	public void StopBounce()
	{
		bouncing = false;
	}

	public void StartBounce()
	{
		bouncing = true;
		lastPos = base.transform.position;
	}

	public void SetBounceFactor(float value)
	{
		bounceFactor = value;
	}

	public void SetBounceAnimation(bool set)
	{
		playAnimationOnBounce = set;
	}

	public void DropAndRecycle()
	{
		StopBounce();
		PolygonCollider2D component = GetComponent<PolygonCollider2D>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		BoxCollider2D component2 = GetComponent<BoxCollider2D>();
		if ((bool)component2)
		{
			component2.enabled = false;
		}
		CircleCollider2D component3 = GetComponent<CircleCollider2D>();
		if ((bool)component3)
		{
			component3.enabled = false;
		}
		recycleTimer = 1f;
	}

	public void SetEnteredWater()
	{
		enteredWater = true;
	}
}
