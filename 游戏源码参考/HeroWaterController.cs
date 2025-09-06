using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class HeroWaterController : MonoBehaviour
{
	public enum States
	{
		Inactive = 0,
		Entered = 1,
		Idle = 2,
		SwimLeft = 3,
		SwimRight = 4,
		SprintLeft = 5,
		SprintRight = 6
	}

	private class AnimationGroup
	{
		public string In;

		public string InToIdle;

		public string Idle;

		public string IdleToSwim;

		public string TurnToSwim;

		public AnimationGroup()
		{
		}

		public AnimationGroup(AnimationGroup other)
		{
			In = other.In;
			InToIdle = other.InToIdle;
			Idle = other.Idle;
			IdleToSwim = other.IdleToSwim;
			TurnToSwim = other.TurnToSwim;
		}
	}

	private static readonly AnimationGroup _regularAnims = new AnimationGroup
	{
		In = "Surface In",
		InToIdle = "Surface InToIdle",
		Idle = "Surface Idle",
		IdleToSwim = "Surface IdleToSwim",
		TurnToSwim = "Surface TurnToSwim"
	};

	private static readonly AnimationGroup _spaAnims = new AnimationGroup(_regularAnims)
	{
		In = "Spa Surface In",
		InToIdle = "Spa Surface InToIdle",
		Idle = "Spa Surface Idle",
		IdleToSwim = "Spa Surface IdleToSwim",
		TurnToSwim = "Spa Surface TurnToSwim"
	};

	[SerializeField]
	private float swimSpeed = 5f;

	[SerializeField]
	private float quickSwimSpeed = 6f;

	[Space]
	[SerializeField]
	private float sprintSwimSpeed = 8f;

	[SerializeField]
	private float quickSprintSwimSpeed = 9f;

	[SerializeField]
	private float sprintAcceleration = 5f;

	[SerializeField]
	private float sprintBurstSpeed = 10f;

	[SerializeField]
	private float sprintBurstAcceleration = 5f;

	[SerializeField]
	private float sprintEndAcceleration = 5f;

	[SerializeField]
	private GameObject dashBurstEffect;

	[Space]
	[SerializeField]
	private float sprintBonkSpeed = -10f;

	[SerializeField]
	private GameObject[] sprintBonkActivate;

	[SerializeField]
	private CameraShakeTarget sprintBonkCameraShake;

	[SerializeField]
	private AudioEvent sprintBonkAudio;

	[Space]
	[SerializeField]
	private float idleTimePadding = 0.1f;

	private double? idleTime;

	[SerializeField]
	private AudioSource normalSwimAudio;

	[SerializeField]
	private AudioSource fastSwimAudio;

	[SerializeField]
	private AudioClip fastSwimStart;

	[SerializeField]
	private float swimAudioLerpSpeed = 5f;

	[SerializeField]
	private Transform[] jumpRaycastOrigins;

	[SerializeField]
	private float jumpRaycastDistance = 1.1f;

	[SerializeField]
	private float jumpTranslateHeight = 1f;

	[Space]
	[SerializeField]
	private FlingUtils.Config dropletFlingSprintBurst;

	[SerializeField]
	private FlingUtils.Config dropletFlingSprinting;

	[SerializeField]
	private Vector3 dropletFlingSprintingOffset;

	[SerializeField]
	private float dropletFlingSprintingDelay;

	[Space]
	[SerializeField]
	private FlingUtils.Config dropletFlingTumbling;

	[SerializeField]
	private MinMaxFloat dropletFlingTumblingDelay;

	private Coroutine swimBeginRoutine;

	private bool canAnimate;

	private string nextAnimation;

	private bool resetAnimation;

	private float waterEnterJumpQueueTimeLeft;

	private bool canJump;

	private bool isJumpQueued;

	private bool isEnterTumbling;

	private double nextDropletFlingTumblingTime;

	private float currentSprintSpeed;

	private float currentSprintAcceleration;

	private bool isSprinting;

	private float currentSwimSpeed;

	private bool doSprintDecelerate;

	private bool queueStartSprint;

	private double nextDashTime;

	private double dashBurstEndTime;

	private double nextSprintDropletSpawnTime;

	private int dashQueueStepsLeft;

	private bool isBonkBlocking;

	private bool isTouchingWall;

	private Color waterColor;

	private float waterFlowSpeed;

	private Bounds waterBounds;

	private readonly List<GameObject> flingSpawnTracker = new List<GameObject>();

	private AnimationGroup anims;

	private Quaternion waterSurfaceRotation;

	private InputHandler ih;

	private HeroController hc;

	private HeroAnimationController animCtrl;

	private HeroVibrationController vc;

	private tk2dSpriteAnimator animator;

	private Rigidbody2D body;

	private Dictionary<GameObject, OnDestroyEventAnnouncer> _flingDestroyEvents = new Dictionary<GameObject, OnDestroyEventAnnouncer>();

	private bool isInWater;

	private float previousYPos;

	private float reduceOdourAccumulate;

	private bool hasVibrationController;

	private float monitorTimer;

	private bool recoilCheck;

	private const float RE_ENTER_DELAY = 0.050000004f;

	private double entryDelayTime;

	public States CurrentState { get; private set; }

	private States PreviousState { get; set; }

	private States? PreviousSwimState { get; set; }

	public bool IsInWater => isInWater;

	private float SwimSpeed
	{
		get
		{
			if (!hc.IsUsingQuickening)
			{
				return swimSpeed;
			}
			return quickSwimSpeed;
		}
	}

	private float SprintSwimSpeed
	{
		get
		{
			if (!hc.IsUsingQuickening)
			{
				return sprintSwimSpeed;
			}
			return quickSprintSwimSpeed;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(dropletFlingSprintingOffset, 0.2f);
	}

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		body = GetComponent<Rigidbody2D>();
		if ((bool)dashBurstEffect)
		{
			dashBurstEffect.SetActive(value: false);
		}
	}

	private void Start()
	{
		ih = ManagerSingleton<InputHandler>.Instance;
		hc = HeroController.instance;
		animCtrl = hc.GetComponent<HeroAnimationController>();
		vc = hc.GetVibrationCtrl();
		hasVibrationController = vc;
		GameManager.instance.NextSceneWillActivate += OnNextSceneLoaded;
		hc.OnTakenDamage += OnTakenDamage;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "MULTI WOUND CANCEL").ReceivedEvent += OnMultiWoundCancel;
	}

	private void OnDestroy()
	{
		if ((bool)GameManager.instance)
		{
			GameManager.instance.NextSceneWillActivate -= OnNextSceneLoaded;
		}
		if ((bool)hc)
		{
			hc.OnTakenDamage -= OnTakenDamage;
		}
		foreach (KeyValuePair<GameObject, OnDestroyEventAnnouncer> flingDestroyEvent in _flingDestroyEvents)
		{
			flingDestroyEvent.Deconstruct(out var _, out var value);
			value.OnDestroyEvent -= OnSpawnedFlingDestroyed;
		}
		_flingDestroyEvents.Clear();
	}

	private void OnMultiWoundCancel()
	{
		TryEntry(forced: false);
	}

	private void TryEntry(bool forced)
	{
		monitorTimer = 0f;
		if (!IsInWater || forced)
		{
			SurfaceWaterRegion.TryReentry(this, hc);
		}
	}

	public void EnterWaterRegion(SurfaceWaterRegion surfaceWater)
	{
		isInWater = true;
		waterColor = surfaceWater.Color;
		waterFlowSpeed = surfaceWater.FlowSpeed;
		anims = (surfaceWater.UseSpaAnims ? _spaAnims : _regularAnims);
		waterBounds = surfaceWater.Bounds;
		waterSurfaceRotation = surfaceWater.transform.rotation;
		if (CurrentState == States.Inactive)
		{
			EnteredWater();
		}
	}

	public void OnNextSceneLoaded()
	{
		ExitWaterRegion(vibrate: false);
	}

	public void ExitWaterRegion()
	{
		ExitWaterRegion(vibrate: true);
	}

	private void OnTakenDamage()
	{
		if (isInWater)
		{
			ExitWaterRegion();
			monitorTimer = 0.5f;
		}
		else if (monitorTimer > 0f)
		{
			monitorTimer = 0.5f;
		}
	}

	public void ExitWaterRegion(bool vibrate)
	{
		isInWater = false;
		if (CurrentState != 0)
		{
			TumbleOut(vibrate);
		}
	}

	private void EnteredWater()
	{
		if (hc.cState.isSprinting && ih.inputActions.Dash.IsPressed)
		{
			queueStartSprint = true;
		}
		EventRegister.SendEvent(EventRegisterEvents.HeroSurfaceEnter);
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		if (hc.cState.recoiling)
		{
			hc.CancelDamageRecoil();
		}
		hc.ResetAirMoves();
		if (hc.cState.downSpiking)
		{
			hc.FinishDownspike();
		}
		hc.RelinquishControl();
		hc.StopAnimationControl();
		hc.AffectedByGravity(gravityApplies: false);
		hc.IsSwimming();
		hc.SetAllowNailChargingWhileRelinquished(value: false);
		body.linearVelocity = Vector2.zero;
		CurrentState = States.Entered;
		PreviousState = States.Inactive;
		normalSwimAudio.Play();
		fastSwimAudio.Play();
		canAnimate = false;
		normalSwimAudio.volume = 0f;
		fastSwimAudio.volume = 0f;
		currentSwimSpeed = SwimSpeed;
		currentSprintSpeed = SprintSwimSpeed;
		doSprintDecelerate = false;
		isBonkBlocking = false;
		if (hasVibrationController)
		{
			vc.PlaySwimEnter();
		}
		swimBeginRoutine = StartCoroutine(SwimBegin());
		hc.ReduceOdours(20);
	}

	private void ExitedWater(bool vibrate = true)
	{
		hc.RegainControl();
		hc.StartAnimationControl();
		hc.NotSwimming();
		EventRegister.SendEvent(EventRegisterEvents.HeroSurfaceExit);
		if (swimBeginRoutine != null)
		{
			StopCoroutine(swimBeginRoutine);
		}
		CurrentState = States.Inactive;
		PreviousState = States.Inactive;
		PreviousSwimState = null;
		idleTime = 0.0;
		resetAnimation = false;
		normalSwimAudio.Stop();
		fastSwimAudio.Stop();
		isSprinting = false;
		if (vibrate && hasVibrationController)
		{
			vc.PlaySwimExit();
		}
	}

	private IEnumerator SwimBegin()
	{
		canJump = false;
		isEnterTumbling = false;
		string anim;
		if (Mathf.Abs(waterFlowSpeed) > 0.01f)
		{
			anim = "Surface Current In Tumble";
			isEnterTumbling = true;
		}
		else
		{
			anim = anims.In;
		}
		yield return StartCoroutine(animator.PlayAnimWait(anim, OnAnimationTrigger));
		if (isEnterTumbling)
		{
			isEnterTumbling = false;
			yield return StartCoroutine(animator.PlayAnimWait("Surface Current In Recover"));
		}
		else if (GetSwimDirection() == 0)
		{
			yield return StartCoroutine(animator.PlayAnimWait(anims.InToIdle));
			animator.Play(anims.Idle);
		}
		else
		{
			animator.Play(anims.IdleToSwim);
		}
		canAnimate = true;
	}

	private void FixedUpdate()
	{
		if (dashQueueStepsLeft > 0)
		{
			dashQueueStepsLeft--;
		}
	}

	private void Update()
	{
		if (hc.IsPaused())
		{
			if (hasVibrationController)
			{
				vc.SetSwimming(swimming: false);
			}
			return;
		}
		if (recoilCheck || monitorTimer > 0f)
		{
			monitorTimer -= Time.deltaTime;
			if (hc.cState.recoiling)
			{
				recoilCheck = true;
			}
			else if (recoilCheck)
			{
				recoilCheck = false;
				if (hc.IsGravityApplied)
				{
					TryEntry(forced: true);
				}
			}
			else if (monitorTimer <= 0f)
			{
				OnMultiWoundCancel();
			}
		}
		if (queueStartSprint && !ih.inputActions.Dash.IsPressed)
		{
			queueStartSprint = false;
		}
		if (ih.inputActions.Jump.WasPressed)
		{
			if (!hc.cState.dashing && !hc.cState.airDashing)
			{
				waterEnterJumpQueueTimeLeft = 0.1f;
			}
		}
		else if (waterEnterJumpQueueTimeLeft > 0f)
		{
			waterEnterJumpQueueTimeLeft -= Time.deltaTime;
		}
		if (CurrentState != 0)
		{
			resetAnimation = false;
			PreviousState = CurrentState;
			if (waterEnterJumpQueueTimeLeft > 0f && TryDoJump())
			{
				return;
			}
			int num = GetSwimDirection();
			if (isSprinting)
			{
				if (!ih.inputActions.Dash.IsPressed && !IsInDashBurst())
				{
					isSprinting = false;
					currentSwimSpeed = currentSprintSpeed;
				}
			}
			else
			{
				if (queueStartSprint || ih.inputActions.Dash.WasPressed)
				{
					queueStartSprint = false;
					if (CanDash())
					{
						isSprinting = true;
					}
					else
					{
						dashQueueStepsLeft = hc.DASH_QUEUE_STEPS;
					}
				}
				else if (ih.inputActions.Dash.IsPressed && dashQueueStepsLeft > 0 && CanDash())
				{
					isSprinting = true;
				}
				if (isSprinting)
				{
					dashBurstEndTime = Time.timeAsDouble + (double)hc.DASH_TIME;
					nextDashTime = dashBurstEndTime + (double)hc.DASH_COOLDOWN;
					currentSprintSpeed = (hc.cState.facingRight ? sprintBurstSpeed : (0f - sprintBurstSpeed));
					currentSprintAcceleration = sprintBurstAcceleration;
					if (swimBeginRoutine != null)
					{
						StopCoroutine(swimBeginRoutine);
					}
					canAnimate = true;
					normalSwimAudio.volume = 0f;
					fastSwimAudio.volume = 1f;
					if ((bool)fastSwimStart)
					{
						fastSwimAudio.PlayOneShot(fastSwimStart);
					}
					canJump = true;
					if ((bool)dashBurstEffect)
					{
						dashBurstEffect.SetActive(value: true);
					}
					SpawnFlingsLocal(dropletFlingSprintBurst, Vector3.zero);
				}
			}
			if (isSprinting)
			{
				dashQueueStepsLeft = 0;
				if (num == 0 || IsInDashBurst())
				{
					num = (hc.cState.facingRight ? 1 : (-1));
				}
				CurrentState = ((num > 0) ? States.SprintRight : States.SprintLeft);
				idleTime = null;
			}
			else if (num < 0)
			{
				CurrentState = States.SwimLeft;
				idleTime = null;
			}
			else if (num > 0)
			{
				CurrentState = States.SwimRight;
				idleTime = null;
			}
			else
			{
				double valueOrDefault = idleTime.GetValueOrDefault();
				if (!idleTime.HasValue)
				{
					valueOrDefault = Time.timeAsDouble + (double)idleTimePadding;
					idleTime = valueOrDefault;
				}
				CurrentState = States.Idle;
			}
			if (doSprintDecelerate && CurrentState == States.Idle)
			{
				States previousState = PreviousState;
				if (previousState == States.SwimLeft || previousState == States.SwimRight)
				{
					doSprintDecelerate = false;
				}
			}
			bool flag = CurrentState != States.Idle;
			float t = swimAudioLerpSpeed * Time.deltaTime;
			normalSwimAudio.volume = Mathf.Lerp(normalSwimAudio.volume, (flag && !isSprinting) ? 1 : 0, t);
			fastSwimAudio.volume = Mathf.Lerp(fastSwimAudio.volume, (flag && isSprinting) ? 1 : 0, t);
			if (hasVibrationController)
			{
				vc.SetSwimAndSprint(flag, isSprinting);
			}
			float b = 0f;
			float b2 = 0f;
			float speedMultiplier = 1f;
			switch (CurrentState)
			{
			case States.Idle:
				if (PreviousState != States.Idle)
				{
					nextAnimation = anims.Idle;
				}
				b = 0f;
				break;
			case States.SwimLeft:
				if (PreviousState != States.SwimLeft)
				{
					hc.FaceLeft();
					PlayIdleToSwim();
				}
				if (PreviousSwimState == States.SwimRight || PreviousSwimState == States.SprintRight)
				{
					PlaySwimTurn();
					doSprintDecelerate = false;
				}
				b = 0f - SwimSpeed;
				speedMultiplier = SwimSpeed / swimSpeed;
				break;
			case States.SwimRight:
				if (PreviousState != States.SwimRight)
				{
					hc.FaceRight();
					PlayIdleToSwim();
				}
				if (PreviousSwimState == States.SwimLeft || PreviousSwimState == States.SprintLeft)
				{
					PlaySwimTurn();
					doSprintDecelerate = false;
				}
				b = SwimSpeed;
				speedMultiplier = SwimSpeed / swimSpeed;
				break;
			case States.SprintLeft:
				if (PreviousState != States.SprintLeft)
				{
					hc.FaceLeft();
					PlaySwimDash();
					doSprintDecelerate = true;
				}
				if (PreviousSwimState == States.SprintRight)
				{
					PlaySwimDashTurn();
				}
				b2 = 0f - SprintSwimSpeed;
				speedMultiplier = SprintSwimSpeed / sprintSwimSpeed;
				break;
			case States.SprintRight:
				if (PreviousState != States.SprintRight)
				{
					hc.FaceRight();
					PlaySwimDash();
					doSprintDecelerate = true;
				}
				if (PreviousSwimState == States.SprintLeft)
				{
					PlaySwimDashTurn();
				}
				b2 = SprintSwimSpeed;
				speedMultiplier = SprintSwimSpeed / sprintSwimSpeed;
				break;
			}
			if (CurrentState != States.Idle)
			{
				PreviousSwimState = CurrentState;
			}
			if (isSprinting)
			{
				if (!IsInDashBurst())
				{
					currentSprintSpeed = Mathf.Lerp(currentSprintSpeed, b2, currentSprintAcceleration * Time.deltaTime);
				}
				UpdateMoveVelocity(currentSprintSpeed);
				if (Time.timeAsDouble >= nextSprintDropletSpawnTime)
				{
					nextSprintDropletSpawnTime = Time.timeAsDouble + (double)dropletFlingSprintingDelay;
					SpawnFlingsLocal(dropletFlingSprinting, dropletFlingSprintingOffset);
				}
			}
			else
			{
				if (doSprintDecelerate)
				{
					currentSwimSpeed = Mathf.Lerp(currentSwimSpeed, b, sprintEndAcceleration * Time.deltaTime);
				}
				else
				{
					currentSwimSpeed = b;
				}
				UpdateMoveVelocity(currentSwimSpeed);
			}
			reduceOdourAccumulate += 10f * Time.deltaTime;
			if (reduceOdourAccumulate > 1f)
			{
				hc.ReduceOdours(Mathf.FloorToInt(reduceOdourAccumulate));
				reduceOdourAccumulate %= 1f;
			}
			if ((!idleTime.HasValue || Time.timeAsDouble >= idleTime.Value) && canAnimate && !string.IsNullOrEmpty(nextAnimation))
			{
				if (resetAnimation && !animator.IsPlaying(nextAnimation))
				{
					resetAnimation = false;
				}
				if (resetAnimation)
				{
					animator.PlayFromFrame(0);
				}
				else
				{
					PlayAnim(nextAnimation, speedMultiplier);
				}
				nextAnimation = string.Empty;
			}
			if (isSprinting && isTouchingWall)
			{
				DoSprintBonk();
			}
			if (isEnterTumbling && Time.timeAsDouble >= nextDropletFlingTumblingTime)
			{
				nextDropletFlingTumblingTime = Time.timeAsDouble + (double)dropletFlingTumblingDelay.GetRandomValue();
				SpawnFlingsLocal(dropletFlingTumbling, Vector3.zero);
			}
		}
		else if (isInWater && base.transform.position.y - previousYPos < 0f && entryDelayTime <= Time.timeAsDouble)
		{
			EnteredWater();
		}
		previousYPos = base.transform.position.y;
	}

	private void PlayAnim(string clipName, float speedMultiplier = 1f)
	{
		tk2dSpriteAnimationClip clipByName = animator.GetClipByName(clipName);
		if (!Mathf.Approximately(speedMultiplier, 1f))
		{
			animator.Play(clipByName, 0f, clipByName.fps * speedMultiplier);
		}
		else
		{
			animator.Play(clipByName);
		}
	}

	private void UpdateMoveVelocity(float moveSpeed)
	{
		Vector2 vector = new Vector2(moveSpeed + waterFlowSpeed, 0f);
		Vector3 vector2 = waterSurfaceRotation * vector;
		body.linearVelocity = vector2;
	}

	private bool IsInDashBurst()
	{
		return Time.timeAsDouble <= dashBurstEndTime;
	}

	private bool CanDash()
	{
		if (isEnterTumbling)
		{
			return false;
		}
		if (!PlayerData.instance.hasDash)
		{
			return false;
		}
		return Time.timeAsDouble >= nextDashTime;
	}

	private void PlayIdleToSwim()
	{
		nextAnimation = anims.IdleToSwim;
	}

	private void PlaySwimTurn()
	{
		if (isSprinting)
		{
			fastSwimAudio.volume = 1f;
		}
		else
		{
			normalSwimAudio.volume = 1f;
		}
		if (canAnimate)
		{
			nextAnimation = anims.TurnToSwim;
			resetAnimation = true;
		}
		currentSprintAcceleration = sprintAcceleration;
	}

	private void PlaySwimDash()
	{
		nextAnimation = "Swim Dash";
		resetAnimation = true;
	}

	private void PlaySwimDashTurn()
	{
		if (isSprinting)
		{
			fastSwimAudio.volume = 1f;
		}
		else
		{
			normalSwimAudio.volume = 1f;
		}
		nextAnimation = "Swim Dash Turn";
		resetAnimation = true;
	}

	private int GetSwimDirection()
	{
		if (isBonkBlocking || isEnterTumbling)
		{
			return 0;
		}
		int num = 0;
		if (ih.inputActions.Left.IsPressed)
		{
			num--;
		}
		if (ih.inputActions.Right.IsPressed)
		{
			num++;
		}
		return num;
	}

	private void SpawnFlingsLocal(FlingUtils.Config config, Vector3 offset)
	{
		if (hc.cState.facingRight)
		{
			config.AngleMin = Helper.GetReflectedAngle(config.AngleMin, reflectHorizontal: true, reflectVertical: false);
			config.AngleMax = Helper.GetReflectedAngle(config.AngleMax, reflectHorizontal: true, reflectVertical: false);
		}
		flingSpawnTracker.Clear();
		FlingUtils.SpawnAndFling(config, base.transform, offset, flingSpawnTracker);
		SetSpawnedGameObjectColorsTemp(flingSpawnTracker);
		RegisterSpawnedFlingDestroyCallbacks(flingSpawnTracker);
	}

	private void RegisterSpawnedFlingDestroyCallbacks(List<GameObject> flings)
	{
		for (int i = 0; i < flings.Count; i++)
		{
			GameObject gameObject = flings[i];
			if (!_flingDestroyEvents.ContainsKey(gameObject))
			{
				OnDestroyEventAnnouncer onDestroyEventAnnouncer = gameObject.AddComponent<OnDestroyEventAnnouncer>();
				onDestroyEventAnnouncer.OnDestroyEvent += OnSpawnedFlingDestroyed;
				_flingDestroyEvents.Add(gameObject, onDestroyEventAnnouncer);
			}
		}
	}

	private void OnSpawnedFlingDestroyed(OnDestroyEventAnnouncer announcer)
	{
		announcer.OnDestroyEvent -= OnSpawnedFlingDestroyed;
		flingSpawnTracker.Remove(announcer.gameObject);
		_flingDestroyEvents.Remove(announcer.gameObject);
	}

	private void SetSpawnedGameObjectColorsTemp(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
			if (!sprite)
			{
				break;
			}
			RecycleResetHandler obj = gameObject.GetComponent<RecycleResetHandler>() ?? gameObject.AddComponent<RecycleResetHandler>();
			Color initialColor = sprite.color;
			sprite.color = waterColor;
			obj.AddTempAction((Action)delegate
			{
				sprite.color = initialColor;
			});
		}
	}

	private void TranslateIfNecessary()
	{
		Transform[] array = jumpRaycastOrigins;
		for (int i = 0; i < array.Length; i++)
		{
			if (Helper.Raycast2D(array[i].position, Vector2.up, jumpRaycastDistance, 256).collider != null)
			{
				return;
			}
		}
		base.transform.Translate(new Vector3(0f, jumpTranslateHeight, 0f));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!isInWater || collision.gameObject.layer != 8)
		{
			return;
		}
		float x = base.transform.position.x;
		if (collision.GetSafeContact().Point.x - x > 0f)
		{
			if (!hc.cState.facingRight)
			{
				return;
			}
		}
		else if (hc.cState.facingRight)
		{
			return;
		}
		isTouchingWall = true;
		if (isSprinting)
		{
			DoSprintBonk();
		}
	}

	private void OnCollisionExit2D()
	{
		isTouchingWall = false;
	}

	private bool TryDoJump()
	{
		if (hc.cState.dashing || hc.cState.airDashing)
		{
			return false;
		}
		if (!canJump)
		{
			if (!isEnterTumbling)
			{
				isJumpQueued = true;
			}
			return false;
		}
		entryDelayTime = Time.timeAsDouble + 0.05000000447034836;
		isJumpQueued = false;
		waterEnterJumpQueueTimeLeft = 0f;
		Vector2 linearVelocity = body.linearVelocity;
		bool num = isSprinting;
		animator.Play("Airborne");
		body.linearVelocity = new Vector2(0f, 10f);
		TranslateIfNecessary();
		hc.ResetInputQueues();
		if (isSprinting)
		{
			hc.SetStartWithFlipJump();
		}
		else
		{
			hc.SetStartWithJump();
		}
		ExitedWater();
		if (num && Math.Abs(linearVelocity.x) > 0.01f)
		{
			hc.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
			{
				Velocity = new Vector2(linearVelocity.x, 0f),
				Decay = 3f,
				CancelOnTurn = true,
				SkipBehaviour = HeroController.DecayingVelocity.SkipBehaviours.WhileMoving
			});
		}
		return true;
	}

	private void DoSprintBonk()
	{
		canAnimate = false;
		animator.Play("Swim Dash Bonk");
		isBonkBlocking = true;
		isSprinting = false;
		currentSwimSpeed = (hc.cState.facingRight ? sprintBonkSpeed : (0f - sprintBonkSpeed));
		sprintBonkActivate.SetAllActive(value: true);
		sprintBonkCameraShake.DoShake(this);
		sprintBonkAudio.SpawnAndPlayOneShot(base.transform.position);
		DeliveryQuestItem.TakeHit();
		animator.AnimationCompleted = OnAnimationCompleted;
	}

	private void OnAnimationTrigger(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		if (clip.name == anims.In || clip.name == "Surface Current In Recover")
		{
			canJump = true;
			if (isJumpQueued)
			{
				TryDoJump();
			}
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == "Swim Dash Bonk")
		{
			canAnimate = true;
			nextAnimation = anims.IdleToSwim;
			isBonkBlocking = false;
		}
	}

	private void TumbleOut(bool vibrate = true)
	{
		animCtrl.SetPlayMantleCancel();
		hc.SetStartFromMantle();
		ExitedWater(vibrate);
		float x = hc.transform.position.x;
		float x2 = waterBounds.center.x;
		if (x < x2)
		{
			hc.FaceLeft();
			hc.RecoilLeftLong();
		}
		else
		{
			hc.FaceRight();
			hc.RecoilRightLong();
		}
	}

	public void Rejected()
	{
		if (!(entryDelayTime > Time.timeAsDouble))
		{
			hc.RelinquishControl();
			hc.StopAnimationControl();
			TumbleOut();
		}
	}
}
