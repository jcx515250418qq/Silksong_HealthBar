using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

public class SplineRunner : MonoBehaviour
{
	private enum MoveStates
	{
		Running = 0,
		Flying = 1,
		Dashing = 2
	}

	private const string FLY_UP_ANIM = "Fly Up";

	private const string FLY_DOWN_ANIM = "Fly Down";

	private const string FLY_TURN_ANIM = "Fly Turn";

	private const string FLY_ANTIC_ANIM = "Antic Skid";

	private const string RUN_START_ANIM = "Sprint Start";

	private const string RUN_ANIM = "Sprint";

	private const string RUN_TURN_ANIM = "Turn";

	private const string DASH_ANIM = "Dash Forward";

	private const string DASH_MANTLE_ANIM = "Dash Forward Mantle";

	private const string BONK_ANIM = "Bonk";

	private const string DASH_DOWN_ANIM = "Dash Down";

	[SerializeField]
	private MinMaxFloat speedMultiplierRange;

	[SerializeField]
	private MinMaxFloat speedChangeDelay;

	[SerializeField]
	private float speedLerpSpeed;

	[SerializeField]
	private float maxSpeedHeroDistance;

	[Space]
	[SerializeField]
	private float dashSpeed;

	[SerializeField]
	private float dashDuration;

	[SerializeField]
	private MinMaxFloat dashCooldown;

	[SerializeField]
	private float dashDownCheckDistance;

	[SerializeField]
	private MinMaxFloat dashDownAngleRange;

	[SerializeField]
	private float dashDownSpeed;

	[SerializeField]
	private float runAnimStartSpeedMultiplier;

	[Space]
	[SerializeField]
	private bool spriteFacesRight;

	[SerializeField]
	private float groundDistanceThreshold;

	[SerializeField]
	private float targetDistance;

	[SerializeField]
	private RunEffects runEffectsPrefab;

	[SerializeField]
	private DashEffect dashEffectPrefab;

	[SerializeField]
	private GameObject dashBurst;

	[SerializeField]
	private GameObject dashBurstDown;

	[SerializeField]
	private JumpEffects jumpEffectsPrefab;

	[SerializeField]
	private Vector3 jumpEffectOffset;

	[Space]
	[SerializeField]
	private HitResponseBase bonkEffect;

	[SerializeField]
	private float bonkDeceleration;

	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string runnerWinEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string heroWinEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string disqualifiedEvent;

	[Space]
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip runAudioLoop;

	[SerializeField]
	private AudioEvent dashAudio;

	[SerializeField]
	private AudioClip dashAudioLoop;

	[SerializeField]
	private AudioClip flyAudioLoop;

	[SerializeField]
	private AudioClip jumpAnticClip;

	[SerializeField]
	private AudioClip jumpClip;

	[SerializeField]
	private AudioClip landClip;

	[Space]
	[SerializeField]
	private AudioSource voiceAudioSource;

	[SerializeField]
	private RandomAudioClipTable voiceRunTable;

	[SerializeField]
	private RandomAudioClipTable voiceEffortTable;

	[SerializeField]
	private RandomAudioClipTable voiceHitTable;

	private Vector2 targetPos;

	private Vector2 adjustedTargetPos;

	private Vector2 runnerVelocity;

	private bool isFollowingPath;

	private bool isInCustomAnim;

	private float splineDistance;

	private bool isTargetAtEnd;

	private bool queuedJumpEffect;

	private bool queuedDashEffect;

	private float currentSpeed;

	private float currentTargetSpeed;

	private float speedChangeDelayLeft;

	private float dashTimeLeft;

	private double nextDashTime;

	private bool isDashingDown;

	private MoveStates currentMoveState;

	private bool justStartedRunning;

	private MoveStates prevMoveState;

	private bool wasGroundSnapped;

	private float groundSnapLerpMultiplier;

	private float groundSnapT;

	private float previousGroundSnappedY;

	private SprintRaceController raceController;

	private Rigidbody2D body;

	private BoxCollider2D box;

	private tk2dSpriteAnimator animator;

	private RunEffects spawnedRunEffects;

	private Transform hero;

	[UsedImplicitly]
	private bool IsEventTargetEventValid(string fsmEvent)
	{
		return eventTarget.IsEventValid(fsmEvent);
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(targetPos, 0.15f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(adjustedTargetPos, 0.2f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if ((bool)raceController)
		{
			Vector2 positionAlongSpline = raceController.GetPositionAlongSpline(splineDistance + maxSpeedHeroDistance);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(positionAlongSpline, 0.2f);
		}
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		box = GetComponent<BoxCollider2D>();
		animator = GetComponent<tk2dSpriteAnimator>();
		animator.AnimationEventTriggeredEvent += OnAnimationEventTriggered;
		animator.AnimationCompletedEvent += OnAnimationCompleted;
		if ((bool)dashBurst)
		{
			dashBurst.SetActive(value: false);
		}
		if ((bool)dashBurstDown)
		{
			dashBurstDown.SetActive(value: false);
		}
		hero = HeroController.instance.transform;
		if ((bool)bonkEffect)
		{
			bonkEffect.IsActive = false;
			bonkEffect.HitInDirection += OnBonked;
		}
		if ((bool)audioSource)
		{
			audioSource.loop = true;
			audioSource.playOnAwake = false;
			audioSource.Stop();
		}
		SwitchFromPhysics();
	}

	private void FixedUpdate()
	{
		if (isFollowingPath && animator.IsPlaying("Bonk"))
		{
			runnerVelocity *= bonkDeceleration * Time.deltaTime;
			body.linearVelocity = runnerVelocity;
		}
	}

	private void Update()
	{
		UpdateState();
		if (currentMoveState != prevMoveState || justStartedRunning)
		{
			if (currentMoveState == MoveStates.Running)
			{
				if ((bool)runEffectsPrefab && !spawnedRunEffects)
				{
					spawnedRunEffects = runEffectsPrefab.Spawn(base.transform);
					spawnedRunEffects.StartEffect(isHero: false);
				}
			}
			else if ((bool)spawnedRunEffects)
			{
				spawnedRunEffects.Stop();
				spawnedRunEffects = null;
			}
		}
		if (queuedJumpEffect)
		{
			queuedJumpEffect = false;
			if ((bool)jumpEffectsPrefab)
			{
				jumpEffectsPrefab.Spawn(base.transform.TransformPoint(jumpEffectOffset)).Play(base.gameObject, runnerVelocity, jumpEffectOffset);
			}
		}
		prevMoveState = currentMoveState;
		justStartedRunning = false;
	}

	public void SetRaceController(SprintRaceController newRaceController)
	{
		if ((bool)raceController)
		{
			raceController.RaceCompleted -= OnRaceCompleted;
			raceController.RaceDisqualified -= OnRaceDisqualified;
		}
		raceController = newRaceController;
		if ((bool)raceController)
		{
			raceController.RaceCompleted += OnRaceCompleted;
			raceController.RaceDisqualified += OnRaceDisqualified;
		}
		if (isFollowingPath)
		{
			FinishedRunning();
		}
	}

	private void OnRaceCompleted(bool didHeroWin)
	{
		if (!didHeroWin)
		{
			FinishedRunning();
		}
		if ((bool)eventTarget)
		{
			string text = (didHeroWin ? heroWinEvent : runnerWinEvent);
			if (!string.IsNullOrEmpty(text))
			{
				eventTarget.SendEvent(text);
			}
		}
	}

	private void OnRaceDisqualified()
	{
		if ((bool)eventTarget && !string.IsNullOrEmpty(disqualifiedEvent))
		{
			eventTarget.SendEvent(disqualifiedEvent);
		}
	}

	private void UpdateState()
	{
		if (!isFollowingPath || isInCustomAnim)
		{
			return;
		}
		UpdateTarget();
		if (dashTimeLeft > 0f)
		{
			dashTimeLeft -= Time.deltaTime;
			if (!(dashTimeLeft <= 0f))
			{
				if (isDashingDown)
				{
					Transform obj = base.transform;
					Vector3 position = obj.position;
					position.y -= currentSpeed * Time.deltaTime;
					obj.position = position;
				}
				else
				{
					Transform transform = base.transform;
					Vector3 position2 = transform.position;
					position2.x += currentSpeed * Mathf.Sign(transform.lossyScale.x) * (float)(spriteFacesRight ? 1 : (-1)) * Time.deltaTime;
					transform.position = position2;
				}
				return;
			}
			FastForwardToCurrentPoint();
			nextDashTime = Time.timeAsDouble + (double)dashCooldown.GetRandomValue();
			StopLoop();
			if (isDashingDown)
			{
				raceController.GetRaceInfo(out var _, out var _, out var currentBaseSpeed);
				float num = currentBaseSpeed * speedMultiplierRange.Start;
				if (currentSpeed > num)
				{
					Debug.Log($"Dashing finished! Reducing speed from {currentSpeed} to {num}");
					currentSpeed = num;
				}
			}
		}
		UpdateRunner();
		if (!isFollowingPath || isInCustomAnim)
		{
			return;
		}
		if ((bool)audioSource)
		{
			audioSource.pitch = Time.timeScale;
		}
		switch (currentMoveState)
		{
		case MoveStates.Running:
			if (prevMoveState != 0)
			{
				animator.Play("Sprint Start");
				PlayLoop(runAudioLoop);
				if ((bool)voiceAudioSource)
				{
					voiceAudioSource.clip = voiceRunTable.SelectClip();
					voiceAudioSource.volume = voiceRunTable.SelectVolume();
					voiceAudioSource.pitch = voiceRunTable.SelectPitch();
					voiceAudioSource.Play();
				}
			}
			break;
		case MoveStates.Flying:
			if (!animator.IsPlaying("Fly Turn"))
			{
				string text = ((runnerVelocity.y > 0f) ? "Fly Up" : "Fly Down");
				if (!animator.IsPlaying(text))
				{
					animator.Play(text);
				}
			}
			break;
		}
	}

	private void UpdateTarget()
	{
		if (isTargetAtEnd)
		{
			return;
		}
		Vector2 vector = base.transform.position;
		targetPos = raceController.GetPositionAlongSpline(splineDistance);
		while (!((targetPos - vector).magnitude >= targetDistance))
		{
			splineDistance += 0.1f;
			targetPos = raceController.GetPositionAlongSpline(splineDistance);
			if (!(splineDistance < raceController.TotalPathDistance))
			{
				isTargetAtEnd = true;
				break;
			}
		}
	}

	private void UpdateRunner()
	{
		raceController.GetRaceInfo(out var runnerLaps, out var heroLaps, out var currentBaseSpeed);
		if (heroLaps > runnerLaps || (runnerLaps <= heroLaps && raceController.GetDistanceAlongSpline(hero.position) - splineDistance > maxSpeedHeroDistance))
		{
			if (CanDashForward())
			{
				DashForward();
				return;
			}
			currentTargetSpeed = currentBaseSpeed * speedMultiplierRange.End;
		}
		else
		{
			speedChangeDelayLeft -= Time.deltaTime;
			if (speedChangeDelayLeft <= 0f)
			{
				speedChangeDelayLeft = speedChangeDelay.GetRandomValue();
				currentTargetSpeed = currentBaseSpeed * speedMultiplierRange.GetRandomValue();
			}
		}
		currentSpeed = Mathf.Lerp(currentSpeed, currentTargetSpeed, Time.deltaTime * speedLerpSpeed);
		float num = (animator.IsPlaying("Sprint Start") ? runAnimStartSpeedMultiplier : 1f);
		TrySnapToGround(targetPos, out adjustedTargetPos, isTarget: true);
		Transform transform = base.transform;
		Vector2 vector = transform.position;
		Vector2 vector2 = adjustedTargetPos - vector;
		Vector2 vector3 = vector2;
		if (currentMoveState == MoveStates.Running)
		{
			vector3.y = 0f;
		}
		vector3 = ((vector3.magnitude > 0f) ? vector3.normalized : Vector2.zero);
		runnerVelocity = vector3 * (currentSpeed * num);
		if (isTargetAtEnd && ShouldTurn(runnerVelocity.x))
		{
			raceController.ReportRunnerLapCompleted(out var isRaceComplete);
			if (isRaceComplete)
			{
				FinishedRunning();
			}
			else
			{
				StartLoop();
			}
			return;
		}
		vector += runnerVelocity * Time.deltaTime;
		Vector2 position = vector;
		Vector2 newPos;
		float groundDistance;
		if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
		{
			StartFlying(doLaunch: true);
		}
		else if (TrySnapToGround(vector, out newPos, isTarget: false, out groundDistance))
		{
			if (wasGroundSnapped)
			{
				if (newPos.y + 0.2f < previousGroundSnappedY)
				{
					wasGroundSnapped = false;
				}
				else if (newPos.y - 0.2f > previousGroundSnappedY)
				{
					if (animator.IsPlaying("Sprint"))
					{
						animator.PlayFromFrame("Sprint Start", 0);
					}
					else if (animator.IsPlaying("Dash Forward") || animator.IsPlaying("Dash Forward Mantle"))
					{
						animator.PlayFromFrame("Dash Forward Mantle", 0);
					}
				}
			}
			if (groundDistance < 0.1f)
			{
				if (!wasGroundSnapped)
				{
					PlayOneShot(landClip);
				}
				currentMoveState = MoveStates.Running;
				wasGroundSnapped = true;
				groundSnapT = 1f;
				groundSnapLerpMultiplier = 1f;
				position = newPos;
			}
			else
			{
				if (!wasGroundSnapped)
				{
					wasGroundSnapped = true;
					groundSnapLerpMultiplier = 1f;
					groundSnapT = 0f;
				}
				groundSnapLerpMultiplier += Time.deltaTime * Mathf.Abs(Physics2D.gravity.y);
				groundSnapT += Time.deltaTime * groundSnapLerpMultiplier;
				if (groundSnapT > 1f)
				{
					groundSnapT = 1f;
				}
				position = Vector2.Lerp(vector, newPos, groundSnapT);
			}
			previousGroundSnappedY = newPos.y;
		}
		else
		{
			StartFlying(vector2.y > 0f);
		}
		if (ShouldTurn(runnerVelocity.x))
		{
			if (currentMoveState == MoveStates.Flying)
			{
				animator.Play("Fly Turn");
			}
			else
			{
				animator.Play("Turn");
				isInCustomAnim = true;
				StopLoop();
			}
		}
		transform.SetPosition2D(position);
	}

	private void StartFlying(bool doLaunch)
	{
		if (doLaunch && currentMoveState != MoveStates.Flying)
		{
			dashTimeLeft = 0f;
			animator.Play("Antic Skid");
			isInCustomAnim = true;
			StopLoop();
			PlayOneShot(jumpAnticClip);
			if ((bool)voiceAudioSource)
			{
				voiceAudioSource.Stop();
				voiceAudioSource.loop = false;
				voiceAudioSource.clip = voiceEffortTable.SelectClip();
				voiceAudioSource.volume = voiceEffortTable.SelectVolume();
				voiceAudioSource.pitch = voiceEffortTable.SelectPitch();
				voiceAudioSource.Play();
			}
		}
		else if (!isInCustomAnim)
		{
			PlayLoop(flyAudioLoop);
		}
		currentMoveState = MoveStates.Flying;
		wasGroundSnapped = false;
	}

	private void FinishedRunning()
	{
		if ((bool)spawnedRunEffects)
		{
			spawnedRunEffects.Stop();
			spawnedRunEffects = null;
		}
		isFollowingPath = false;
		if ((bool)bonkEffect)
		{
			bonkEffect.IsActive = false;
		}
		if ((bool)voiceAudioSource)
		{
			voiceAudioSource.Stop();
			voiceAudioSource.clip = null;
		}
		StopLoop();
	}

	private void StartLoop()
	{
		splineDistance = 0f;
		isTargetAtEnd = false;
		FastForwardToCurrentPoint(useShouldTurn: true);
	}

	private void FastForwardToCurrentPoint(bool useShouldTurn = false)
	{
		if (!useShouldTurn)
		{
			splineDistance = raceController.GetDistanceAlongSpline(base.transform.position, getNext: true);
			return;
		}
		float num = 0f;
		bool flag = false;
		while (ShouldTurn(GetDirectionToSplineDistance(num)))
		{
			num += 0.1f;
			if (!(num < raceController.TotalPathDistance))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			splineDistance = num;
		}
	}

	private float GetDirectionToSplineDistance(float distance)
	{
		Vector2 vector = base.transform.position;
		return Mathf.Sign((raceController.GetPositionAlongSpline(distance) - vector).x);
	}

	private bool ShouldTurn(float moveDir)
	{
		float num = base.transform.localScale.x * (float)(spriteFacesRight ? 1 : (-1));
		if (!(num < 0f) || !(moveDir > 0f))
		{
			if (num > 0f)
			{
				return moveDir < 0f;
			}
			return false;
		}
		return true;
	}

	private bool TrySnapToGround(Vector2 initialPos, out Vector2 newPos, bool isTarget)
	{
		float groundDistance;
		return TrySnapToGround(initialPos, out newPos, isTarget, out groundDistance);
	}

	private bool TrySnapToGround(Vector2 initialPos, out Vector2 newPos, bool isTarget, out float groundDistance)
	{
		Vector2 vector = initialPos + box.offset;
		Vector2 vector2 = box.size * 0.5f;
		Vector2 vector3 = vector - vector2;
		float num = vector.y - vector3.y;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(distance: num + groundDistanceThreshold, origin: vector, direction: Vector2.down, layerMask: 256);
		bool result;
		if ((bool)raycastHit2D)
		{
			newPos = raycastHit2D.point;
			newPos.y += vector2.y - box.offset.y;
			result = true;
			groundDistance = vector3.y - raycastHit2D.point.y;
		}
		else
		{
			newPos = initialPos;
			if (isTarget)
			{
				newPos.y += num;
			}
			result = false;
			groundDistance = float.MaxValue;
		}
		return result;
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip)
	{
		isInCustomAnim = false;
		if (!isFollowingPath)
		{
			return;
		}
		switch (clip.name)
		{
		case "Fly Turn":
			spriteAnimator.transform.FlipLocalScale(x: true);
			if (runnerVelocity.y > 0f)
			{
				spriteAnimator.Play("Fly Up");
			}
			else if (CanDashDown())
			{
				DashDown();
			}
			else
			{
				spriteAnimator.Play("Fly Down");
			}
			break;
		case "Sprint Start":
			spriteAnimator.Play("Sprint");
			break;
		case "Turn":
			spriteAnimator.Play("Sprint");
			spriteAnimator.transform.FlipLocalScale(x: true);
			break;
		case "Antic Skid":
			queuedJumpEffect = true;
			PlayLoop(flyAudioLoop);
			PlayOneShot(jumpClip);
			break;
		case "Bonk":
			spriteAnimator.Play("Sprint Start");
			PlayLoop(runAudioLoop);
			if ((bool)voiceAudioSource)
			{
				voiceAudioSource.loop = true;
				voiceAudioSource.clip = voiceRunTable.SelectClip();
				voiceAudioSource.volume = voiceRunTable.SelectVolume();
				voiceAudioSource.pitch = voiceRunTable.SelectPitch();
				voiceAudioSource.Play();
			}
			SwitchFromPhysics();
			break;
		}
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (!queuedDashEffect)
		{
			return;
		}
		string text = clip.name;
		if (!(text == "Dash Forward") && !(text == "Dash Down"))
		{
			return;
		}
		queuedDashEffect = false;
		isInCustomAnim = false;
		if (!isDashingDown && (bool)dashEffectPrefab)
		{
			Transform transform = base.transform;
			Vector3 localScale = transform.localScale;
			DashEffect dashEffect = dashEffectPrefab.Spawn(transform.position);
			dashEffect.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
			dashEffect.Play(base.gameObject);
		}
		GameObject gameObject = (isDashingDown ? dashBurstDown : dashBurst);
		if ((bool)gameObject)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
			gameObject.SetActive(value: true);
		}
	}

	private void PlayOneShot(AudioClip clip)
	{
		if ((bool)audioSource && (bool)clip)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	private void PlayLoop(AudioClip clip)
	{
		if ((bool)audioSource && !(audioSource.clip == clip))
		{
			audioSource.pitch = 1f;
			audioSource.clip = clip;
			audioSource.Play();
		}
	}

	private void StopLoop()
	{
		if ((bool)audioSource)
		{
			audioSource.Stop();
			audioSource.pitch = 1f;
		}
	}

	[ContextMenu("TEST", true)]
	public bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("TEST")]
	public void StartRunning()
	{
		PositionAtStart();
		isFollowingPath = true;
		justStartedRunning = true;
		raceController.GetRaceInfo(out var _, out var _, out var currentBaseSpeed);
		currentSpeed = (currentTargetSpeed = currentBaseSpeed * speedMultiplierRange.End);
		speedChangeDelayLeft = speedChangeDelay.GetRandomValue();
		if (CanDashForward())
		{
			DashForward();
		}
		raceController.StartTracking();
		if ((bool)bonkEffect)
		{
			bonkEffect.IsActive = true;
		}
	}

	public void PositionAtStart()
	{
		ResetRace();
		raceController.BeginInRace();
		Vector2 newPos = raceController.GetPositionAlongSpline(0f);
		TrySnapToGround(newPos, out newPos, isTarget: false);
		base.transform.SetPosition2D(newPos);
		float moveDir = raceController.GetPositionAlongSpline(0.1f).x - newPos.x;
		if (ShouldTurn(moveDir))
		{
			base.transform.FlipLocalScale(x: true);
		}
	}

	public void ResetRace()
	{
		FinishedRunning();
		isInCustomAnim = false;
		splineDistance = 0f;
		isTargetAtEnd = false;
		dashTimeLeft = 0f;
		nextDashTime = 0.0;
		if (raceController != null)
		{
			raceController.StopTracking();
			raceController.EndInRace();
		}
	}

	private bool CanDashForward()
	{
		if (currentMoveState != 0)
		{
			return false;
		}
		if (dashTimeLeft > 0f || Time.timeAsDouble < nextDashTime)
		{
			return false;
		}
		float num = dashSpeed * dashDuration;
		Transform obj = base.transform;
		float num2 = obj.localScale.x * (float)(spriteFacesRight ? 1 : (-1));
		Vector2 newPos = obj.position;
		newPos.x += num * num2;
		return TrySnapToGround(newPos, out newPos, isTarget: false);
	}

	private void DashForward()
	{
		animator.PlayFromFrame("Dash Forward", 0);
		isDashingDown = false;
		dashTimeLeft = dashDuration;
		currentSpeed = dashSpeed;
		currentMoveState = MoveStates.Dashing;
		DashShared();
	}

	private bool CanDashDown()
	{
		if (currentMoveState != MoveStates.Flying)
		{
			return false;
		}
		if (isDashingDown && (dashTimeLeft > 0f || Time.timeAsDouble < nextDashTime))
		{
			return false;
		}
		Vector2 vector = base.transform.position;
		float num = splineDistance;
		Vector2 positionAlongSpline;
		Vector2 vector2;
		do
		{
			num += 0.1f;
			positionAlongSpline = raceController.GetPositionAlongSpline(num);
			if (num >= raceController.TotalPathDistance)
			{
				break;
			}
			vector2 = positionAlongSpline - vector;
		}
		while (!(vector2.y > Mathf.Epsilon) && !(vector2.magnitude >= dashDownCheckDistance));
		Vector2 direction = positionAlongSpline - vector;
		if (direction.y > Mathf.Epsilon)
		{
			return false;
		}
		float num2;
		for (num2 = direction.DirectionToAngle(); num2 < 0f; num2 += 360f)
		{
		}
		while (num2 >= 360f)
		{
			num2 -= 360f;
		}
		return dashDownAngleRange.IsInRange(num2);
	}

	private void DashDown()
	{
		animator.PlayFromFrame("Dash Down", 0);
		isInCustomAnim = true;
		isDashingDown = true;
		dashTimeLeft = raceController.DashDownDuration;
		currentSpeed = dashDownSpeed;
		DashShared();
	}

	private void DashShared()
	{
		queuedDashEffect = true;
		StopLoop();
		PlayLoop(dashAudioLoop);
		dashAudio.SpawnAndPlayOneShot(base.transform.position);
	}

	private void OnBonked(GameObject source, HitInstance.HitDirection direction)
	{
		animator.Play("Bonk");
		StopLoop();
		isInCustomAnim = true;
		if ((bool)voiceHitTable)
		{
			voiceAudioSource.Stop();
			voiceHitTable.SpawnAndPlayOneShot(base.transform.position);
		}
		SwitchToPhysics();
	}

	private void SwitchToPhysics()
	{
		body.constraints = RigidbodyConstraints2D.FreezeRotation;
		body.linearVelocity = runnerVelocity;
	}

	private void SwitchFromPhysics()
	{
		body.constraints = RigidbodyConstraints2D.FreezeAll;
		body.linearVelocity = Vector2.zero;
	}
}
