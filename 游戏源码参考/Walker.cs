using UnityEngine;

public class Walker : MonoBehaviour
{
	private enum States
	{
		NotReady = 0,
		WaitingForConditions = 1,
		Stopped = 2,
		Walking = 3,
		Turning = 4
	}

	public enum StopReasons
	{
		Bored = 0,
		Controlled = 1
	}

	private const int LAYERMASK = 33024;

	[Header("Structure")]
	[SerializeField]
	private LineOfSightDetector lineOfSightDetector;

	[SerializeField]
	private AlertRange alertRange;

	private Rigidbody2D body;

	private Collider2D bodyCollider;

	private tk2dSpriteAnimator animator;

	private tk2dSpriteAnimator animator_child;

	private AudioSource audioSource;

	private Camera mainCamera;

	private HeroController hero;

	private const float CameraDistanceForActivation = 60f;

	private const float WaitHeroXThreshold = 1f;

	[Header("Configuration")]
	[SerializeField]
	private bool ambush;

	[SerializeField]
	private string idleClip;

	[SerializeField]
	private string turnClip;

	[SerializeField]
	private string walkClip;

	[SerializeField]
	private float edgeXAdjuster;

	[SerializeField]
	private bool preventScaleChange;

	[SerializeField]
	private bool preventTurn;

	[SerializeField]
	private float pauseTimeMin;

	[SerializeField]
	private float pauseTimeMax;

	[SerializeField]
	private float pauseWaitMin;

	[SerializeField]
	private float pauseWaitMax;

	[SerializeField]
	private bool pauses;

	[SerializeField]
	private float rightScale;

	[SerializeField]
	public bool startInactive;

	[SerializeField]
	private int turnAfterIdlePercentage;

	[SerializeField]
	private float turnPause;

	[SerializeField]
	private bool waitForHeroX;

	[SerializeField]
	private float waitHeroX;

	[SerializeField]
	public float walkSpeedL;

	[SerializeField]
	public float walkSpeedR;

	[SerializeField]
	public bool ignoreHoles;

	[SerializeField]
	private bool preventTurningToFaceHero;

	[SerializeField]
	private bool turnBeforeAnimation;

	[SerializeField]
	private bool turnStopMovement = true;

	[SerializeField]
	private GameObject animatedChild;

	[SerializeField]
	private string childAnimSuffix;

	[SerializeField]
	private float aggroEdgeTurnCooldown;

	private States state;

	private bool didFulfilCameraDistanceCondition;

	private bool didFulfilHeroXCondition;

	private int currentFacing;

	private int turningFacing;

	private float walkTimeRemaining;

	private float pauseTimeRemaining;

	private float turnCooldownRemaining;

	private float aggroEdgeTurnCooldownRemaining;

	private StopReasons stopReason;

	private int logState;

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<tk2dSpriteAnimator>();
		audioSource = GetComponent<AudioSource>();
		if (animatedChild != null)
		{
			animator_child = animatedChild.GetComponent<tk2dSpriteAnimator>();
		}
		GetCollider();
	}

	private void GetCollider()
	{
		if (!bodyCollider)
		{
			bodyCollider = GetComponent<Collider2D>();
		}
	}

	protected void Start()
	{
		mainCamera = GameCameras.instance.mainCamera;
		hero = HeroController.instance;
		if (currentFacing == 0)
		{
			currentFacing = GetFacingFromScale();
		}
		if (state == States.NotReady)
		{
			turnCooldownRemaining = 0f - Mathf.Epsilon;
			BeginWaitingForConditions();
		}
	}

	private int GetFacingFromScale()
	{
		if (!(base.transform.localScale.x * rightScale >= 0f))
		{
			return -1;
		}
		return 1;
	}

	private void Update()
	{
		turnCooldownRemaining -= Time.deltaTime;
		aggroEdgeTurnCooldownRemaining -= Time.deltaTime;
		GetCollider();
		switch (state)
		{
		case States.WaitingForConditions:
			UpdateWaitingForConditions();
			break;
		case States.Stopped:
			UpdateStopping();
			break;
		case States.Walking:
			UpdateWalking();
			break;
		case States.Turning:
			UpdateTurning();
			break;
		}
		if (aggroEdgeTurnCooldown > 0f && state == States.Turning && alertRange.IsHeroInRange() && (!lineOfSightDetector || lineOfSightDetector.CanSeeHero))
		{
			aggroEdgeTurnCooldownRemaining = aggroEdgeTurnCooldown;
		}
	}

	public void StartMoving()
	{
		if (state == States.Stopped || state == States.WaitingForConditions)
		{
			startInactive = false;
			int facing = ((currentFacing != 0) ? (currentFacing = GetFacingFromScale()) : ((Random.Range(0, 2) != 0) ? 1 : (-1)));
			BeginWalkingOrTurning(facing);
		}
		Update();
	}

	public void CancelTurn()
	{
		if (state == States.Turning)
		{
			BeginWalking(currentFacing);
		}
	}

	public void Go(int facing)
	{
		turnCooldownRemaining = 0f - Mathf.Epsilon;
		if (state == States.Stopped || state == States.Walking)
		{
			BeginWalkingOrTurning(facing);
		}
		else if (state == States.Turning && currentFacing == facing)
		{
			CancelTurn();
		}
		Update();
	}

	public void RecieveGoMessage(int facing)
	{
		if (state != States.Stopped || stopReason != StopReasons.Controlled)
		{
			Go(facing);
		}
	}

	public void Stop(StopReasons reason)
	{
		BeginStopped(reason);
	}

	public void ChangeFacing(int facing)
	{
		if (state == States.Turning)
		{
			turningFacing = facing;
			currentFacing = -facing;
		}
		else
		{
			currentFacing = facing;
		}
	}

	private void BeginWaitingForConditions()
	{
		state = States.WaitingForConditions;
		didFulfilCameraDistanceCondition = false;
		didFulfilHeroXCondition = false;
		UpdateWaitingForConditions();
	}

	private void UpdateWaitingForConditions()
	{
		if (!didFulfilCameraDistanceCondition && (mainCamera.transform.position - base.transform.position).sqrMagnitude < 3600f)
		{
			didFulfilCameraDistanceCondition = true;
		}
		if (didFulfilCameraDistanceCondition && !didFulfilHeroXCondition && hero != null && Mathf.Abs(hero.transform.GetPositionX() - waitHeroX) < 1f)
		{
			didFulfilHeroXCondition = true;
		}
		if (didFulfilCameraDistanceCondition && (!waitForHeroX || didFulfilHeroXCondition) && !startInactive && !ambush)
		{
			BeginStopped(StopReasons.Bored);
			StartMoving();
		}
	}

	private void BeginStopped(StopReasons reason)
	{
		state = States.Stopped;
		stopReason = reason;
		if ((bool)audioSource)
		{
			audioSource.Stop();
		}
		if (reason != 0)
		{
			return;
		}
		tk2dSpriteAnimationClip clipByName = animator.GetClipByName(idleClip);
		if (clipByName != null && !animator.IsPlaying(clipByName))
		{
			animator.Play(clipByName);
		}
		if (!string.IsNullOrEmpty(childAnimSuffix) && (bool)animator_child)
		{
			tk2dSpriteAnimationClip clipByName2 = animator_child.GetClipByName(idleClip + childAnimSuffix);
			if (clipByName2 != null && !animator_child.IsPlaying(clipByName2))
			{
				animator_child.Play(clipByName2);
			}
		}
		body.linearVelocity = Vector2.Scale(body.linearVelocity, new Vector2(0f, 1f));
		if (pauses)
		{
			pauseTimeRemaining = Random.Range(pauseTimeMin, pauseTimeMax);
		}
		else
		{
			EndStopping();
		}
	}

	private void UpdateStopping()
	{
		if (stopReason == StopReasons.Bored)
		{
			pauseTimeRemaining -= Time.deltaTime;
			if (pauseTimeRemaining <= 0f)
			{
				EndStopping();
			}
		}
	}

	private void EndStopping()
	{
		if (currentFacing == 0)
		{
			BeginWalkingOrTurning((Random.Range(0, 2) == 0) ? 1 : (-1));
		}
		else if (Random.Range(0, 100) < turnAfterIdlePercentage)
		{
			BeginTurning(-currentFacing);
		}
		else
		{
			BeginWalking(currentFacing);
		}
	}

	private void BeginWalkingOrTurning(int facing)
	{
		if (currentFacing == facing)
		{
			BeginWalking(facing);
		}
		else
		{
			BeginTurning(facing);
		}
	}

	private void BeginWalking(int facing)
	{
		state = States.Walking;
		animator.Play(walkClip);
		if (!string.IsNullOrEmpty(childAnimSuffix) && (bool)animator_child)
		{
			animator_child.Play(walkClip + childAnimSuffix);
		}
		UpdateScale(facing);
		walkTimeRemaining = Random.Range(pauseWaitMin, pauseWaitMax);
		if ((bool)audioSource)
		{
			audioSource.Play();
		}
		body.linearVelocity = new Vector2((facing > 0) ? walkSpeedR : walkSpeedL, body.linearVelocity.y);
	}

	private void UpdateWalking()
	{
		bool num = turnCooldownRemaining <= 0f;
		bool flag = aggroEdgeTurnCooldownRemaining <= 0f;
		if (num)
		{
			if (new Sweep(bodyCollider, 1 - currentFacing, 3).Check(bodyCollider.bounds.extents.x + 0.5f, 33024, useTriggers: false))
			{
				BeginTurning(-currentFacing);
				return;
			}
			if (!preventTurningToFaceHero && flag && hero != null && hero.transform.GetPositionX() > base.transform.GetPositionX() != currentFacing > 0 && ((lineOfSightDetector != null && lineOfSightDetector.CanSeeHero) || lineOfSightDetector == null) && alertRange != null && alertRange.IsHeroInRange())
			{
				BeginTurning(-currentFacing);
				return;
			}
			if (!ignoreHoles && !new Sweep(bodyCollider, 3, 3).Check(0.25f, 33024, out var _, useTriggers: false, new Vector2((bodyCollider.bounds.extents.x + 0.5f + edgeXAdjuster) * (float)currentFacing, 0f)))
			{
				BeginTurning(-currentFacing);
				return;
			}
		}
		if (!flag)
		{
			walkTimeRemaining = 0f;
		}
		else if (pauses && (((!(lineOfSightDetector != null) || !lineOfSightDetector.CanSeeHero) && !(lineOfSightDetector == null)) || !(alertRange != null) || !alertRange.IsHeroInRange()))
		{
			walkTimeRemaining -= Time.deltaTime;
			if (walkTimeRemaining <= 0f)
			{
				BeginStopped(StopReasons.Bored);
				return;
			}
		}
		body.linearVelocity = new Vector2((currentFacing > 0) ? walkSpeedR : walkSpeedL, body.linearVelocity.y);
	}

	private void BeginTurning(int facing)
	{
		if (!new Sweep(bodyCollider, 3, 3).Check(0.25f, 33024, useTriggers: false))
		{
			return;
		}
		state = States.Turning;
		turningFacing = facing;
		if (preventTurn)
		{
			EndTurning();
			return;
		}
		turnCooldownRemaining = turnPause;
		body.linearVelocity = Vector2.Scale(body.linearVelocity, new Vector2((!turnStopMovement) ? (-1) : 0, 1f));
		if (turnBeforeAnimation)
		{
			currentFacing = turningFacing;
			UpdateScale(currentFacing);
		}
		animator.Play(turnClip);
		if (!string.IsNullOrEmpty(childAnimSuffix) && (bool)animator_child)
		{
			animator_child.Play(turnClip + childAnimSuffix);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, (facing > 0) ? "TURN RIGHT" : "TURN LEFT");
	}

	private void UpdateTurning()
	{
		body.linearVelocity = Vector2.Scale(body.linearVelocity, new Vector2((!turnStopMovement) ? 1 : 0, 1f));
		if (!animator.Playing)
		{
			EndTurning();
		}
	}

	private void EndTurning()
	{
		currentFacing = turningFacing;
		BeginWalking(currentFacing);
	}

	public void ClearTurnCooldown()
	{
		turnCooldownRemaining = 0f - Mathf.Epsilon;
	}

	private void UpdateScale(int facing)
	{
		if (!preventScaleChange)
		{
			base.transform.SetScaleX((float)facing * rightScale);
		}
	}
}
