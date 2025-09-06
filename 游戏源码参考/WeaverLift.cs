using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class WeaverLift : MonoBehaviour
{
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	private static readonly int _isVisibleAnimParam = Animator.StringToHash("IsVisible");

	private static readonly int _swapAnimParam = Animator.StringToHash("Swap");

	private static readonly int _isChargingAnimParam = Animator.StringToHash("IsCharging");

	private static readonly int _chargeUpAnim = Animator.StringToHash("Charge Up");

	private static readonly int _isActiveAnimParam = Animator.StringToHash("IsActive");

	private static readonly int _activeAnim = Animator.StringToHash("Active");

	private static readonly int _inactiveAnim = Animator.StringToHash("Inactive");

	[SerializeField]
	private WeaverLift liftUp;

	[SerializeField]
	private WeaverLift liftDown;

	[SerializeField]
	private NestedFadeGroupBase shaftGlowsUp;

	[SerializeField]
	private NestedFadeGroupBase shaftGlowsDown;

	[SerializeField]
	private float shaftGlowsFadeTime;

	[SerializeField]
	private PersistentBoolItem isUnlockedTarget;

	[Space]
	[SerializeField]
	private Animator mainAnimator;

	[SerializeField]
	private CameraShakeTarget chargePlateShake;

	[SerializeField]
	private CameraShakeTarget chargeRumble;

	[SerializeField]
	private ParticleSystemPool chargeUpParticles;

	[SerializeField]
	private GameObject chargeUpWindRegion;

	[SerializeField]
	private GameObject chargeActivate;

	[SerializeField]
	private TriggerEnterEvent firstActivateRange;

	[SerializeField]
	private GameObject preTeleportActivate;

	[SerializeField]
	private float teleportWait;

	[SerializeField]
	private GameObject arriveActivate;

	[SerializeField]
	private float arriveWait;

	[Space]
	[SerializeField]
	private Transform heroTeleportPoint;

	[SerializeField]
	private float teleportDuration;

	[SerializeField]
	private GameObject heroTeleportObject;

	[SerializeField]
	private float cameraYPos;

	private CaptureAnimationEvent mainAnimatorCapture;

	private bool isActive;

	private Coroutine chargeUpRoutine;

	private Coroutine teleportRoutine;

	private bool teleportDirectionUp;

	private static WeaverLift _isTeleportingLift;

	public bool IsAvailable
	{
		get
		{
			if (!isUnlockedTarget)
			{
				return true;
			}
			return isUnlockedTarget.GetCurrentValue();
		}
	}

	public bool HasDirections
	{
		get
		{
			if ((bool)liftUp && liftUp.IsAvailable && (bool)liftDown)
			{
				return liftDown.IsAvailable;
			}
			return false;
		}
	}

	private void Awake()
	{
		if ((bool)isUnlockedTarget)
		{
			isUnlockedTarget.OnSetSaveState += delegate(bool value)
			{
				SetActive(value, isInstant: true);
			};
			if ((bool)firstActivateRange)
			{
				firstActivateRange.OnTriggerEntered += delegate
				{
					if (!isActive && isUnlockedTarget.GetCurrentValue())
					{
						SetActive(value: true, isInstant: false);
					}
				};
			}
		}
		else
		{
			SetActive(value: true, isInstant: true);
		}
		if ((bool)preTeleportActivate)
		{
			preTeleportActivate.SetActive(value: false);
		}
		if ((bool)arriveActivate)
		{
			arriveActivate.SetActive(value: false);
		}
		if ((bool)chargeUpWindRegion)
		{
			chargeUpWindRegion.SetActive(value: false);
		}
		if ((bool)shaftGlowsUp)
		{
			shaftGlowsUp.AlphaSelf = 0f;
		}
		if ((bool)shaftGlowsDown)
		{
			shaftGlowsDown.AlphaSelf = 0f;
		}
		if ((bool)chargeActivate)
		{
			chargeActivate.SetActive(value: false);
		}
		heroTeleportObject.SetActive(value: false);
		mainAnimatorCapture = mainAnimator.GetComponent<CaptureAnimationEvent>();
	}

	private void Start()
	{
		if ((bool)liftUp)
		{
			if ((bool)liftUp.isUnlockedTarget)
			{
				liftUp.isUnlockedTarget.PreSetup();
			}
			teleportDirectionUp = liftUp.IsAvailable;
		}
	}

	private void OnDestroy()
	{
		if (_isTeleportingLift == this)
		{
			_isTeleportingLift = null;
		}
	}

	private void SetActive(bool value, bool isInstant)
	{
		isActive = value;
		mainAnimator.SetBool(_isActiveAnimParam, value);
		if (isInstant)
		{
			mainAnimator.Play(value ? _activeAnim : _inactiveAnim, 0, 1f);
		}
	}

	public void BeginTransport()
	{
		if (!(_isTeleportingLift != null) && IsAvailable)
		{
			chargePlateShake.DoShake(this);
			if ((bool)chargeActivate)
			{
				chargeActivate.SetActive(value: true);
			}
			if (chargeUpRoutine != null)
			{
				StopCoroutine(chargeUpRoutine);
			}
			chargeUpRoutine = StartCoroutine(ChargeUp());
		}
	}

	public void CancelTransport()
	{
		if (teleportRoutine == null)
		{
			if (_isTeleportingLift == this)
			{
				_isTeleportingLift = null;
			}
			if (chargeUpRoutine != null)
			{
				StopCoroutine(chargeUpRoutine);
			}
			ChargeUpStopped();
			mainAnimator.SetBool(_isChargingAnimParam, value: false);
			if ((bool)chargeActivate)
			{
				chargeActivate.SetActive(value: false);
			}
		}
	}

	private IEnumerator ChargeUp()
	{
		mainAnimator.SetBool(_isChargingAnimParam, value: true);
		if ((bool)mainAnimatorCapture)
		{
			mainAnimatorCapture.EventFiredTemp += ChargeUpStarted;
		}
		else
		{
			ChargeUpStarted();
		}
		if ((bool)chargeUpParticles)
		{
			chargeUpParticles.PlayParticles();
		}
		yield return new WaitForEndOfFrame();
		AnimatorStateInfo currentAnimatorStateInfo = mainAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.length == 0f)
		{
			yield return new WaitForEndOfFrame();
			currentAnimatorStateInfo = mainAnimator.GetCurrentAnimatorStateInfo(0);
		}
		yield return new WaitForSeconds(currentAnimatorStateInfo.length);
		chargeUpRoutine = null;
		Teleport();
	}

	private void ChargeUpStarted()
	{
		chargeRumble.DoShake(this);
		if ((bool)chargeUpWindRegion)
		{
			chargeUpWindRegion.SetActive(value: true);
		}
	}

	private void ChargeUpStopped()
	{
		if ((bool)mainAnimatorCapture)
		{
			mainAnimatorCapture.ClearTempEvent();
		}
		chargeRumble.CancelShake();
		if ((bool)chargeUpParticles)
		{
			chargeUpParticles.StopParticles();
		}
		if ((bool)chargeUpWindRegion)
		{
			chargeUpWindRegion.SetActive(value: false);
		}
	}

	private void Teleport()
	{
		WeaverLift weaverLift;
		NestedFadeGroupBase shaftGlows;
		if (HasDirections)
		{
			if (teleportDirectionUp)
			{
				weaverLift = liftUp;
				shaftGlows = shaftGlowsUp;
			}
			else
			{
				weaverLift = liftDown;
				shaftGlows = shaftGlowsDown;
			}
		}
		else if ((bool)liftUp && liftUp.IsAvailable)
		{
			weaverLift = liftUp;
			shaftGlows = shaftGlowsUp;
		}
		else if ((bool)liftDown && liftDown.IsAvailable)
		{
			weaverLift = liftDown;
			shaftGlows = shaftGlowsDown;
		}
		else
		{
			weaverLift = null;
			shaftGlows = null;
		}
		if (weaverLift == null)
		{
			Debug.LogError("No target!", this);
		}
		else if (!weaverLift.heroTeleportPoint)
		{
			Debug.LogError("Target has no teleport point!", weaverLift);
		}
		else
		{
			teleportRoutine = StartCoroutine(TeleportRoutine(weaverLift, shaftGlows));
		}
	}

	private IEnumerator TeleportRoutine(WeaverLift target, NestedFadeGroupBase shaftGlows)
	{
		_isTeleportingLift = this;
		HeroController hc = HeroController.instance;
		tk2dSpriteAnimator heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
		SpriteFlash heroFlash = hc.GetComponent<SpriteFlash>();
		CameraController cam = GameCameras.instance.cameraController;
		MeshRenderer heroRenderer = hc.GetComponent<MeshRenderer>();
		heroTeleportObject.SetActive(value: true);
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		hc.RelinquishControl();
		hc.StopAnimationControl();
		hc.AffectedByGravity(gravityApplies: false);
		hc.cState.invulnerable = true;
		hc.Body.linearVelocity = Vector2.zero;
		heroRenderer.enabled = false;
		CameraController.CameraMode prevCamMode = cam.mode;
		cam.SetMode(CameraController.CameraMode.PANNING);
		Vector2 initialHeroPos = hc.transform.position;
		Vector2 startPoint = target.heroTeleportPoint.position;
		Vector2 targetHeroPos = hc.FindGroundPoint(startPoint);
		if (startPoint.y - initialHeroPos.y > 0f)
		{
			tk2dSpriteAnimationClip clipByName = heroAnimator.GetClipByName("Updraft Rise");
			heroAnimator.PlayFromFrame(clipByName, clipByName.loopStart);
		}
		else
		{
			heroAnimator.Play("Fall");
		}
		if ((bool)preTeleportActivate)
		{
			preTeleportActivate.SetActive(value: true);
		}
		if ((bool)shaftGlows)
		{
			shaftGlows.FadeTo(1f, shaftGlowsFadeTime);
		}
		yield return new WaitForSeconds(teleportWait);
		if ((bool)target.arriveActivate)
		{
			target.arriveActivate.SetActive(value: false);
		}
		Transform heroTrans = hc.transform;
		heroTrans.SetPositionX(targetHeroPos.x);
		initialHeroPos.x = targetHeroPos.x;
		MinMaxFloat cameraYRange = new MinMaxFloat(cameraYPos, target.cameraYPos);
		for (float elapsed = 0f; elapsed < teleportDuration; elapsed += Time.deltaTime)
		{
			float t = elapsed / teleportDuration;
			Vector2 position = Vector2.Lerp(initialHeroPos, targetHeroPos, t);
			heroTrans.SetPosition2D(position);
			heroTeleportObject.transform.SetPosition2D(position);
			cam.SnapToY(cameraYRange.GetLerpedValue(t));
			yield return null;
		}
		hc.transform.SetPosition2D(targetHeroPos);
		heroRenderer.enabled = true;
		heroTeleportObject.SetActive(value: false);
		mainAnimator.SetBool(_isChargingAnimParam, value: false);
		if ((bool)preTeleportActivate)
		{
			preTeleportActivate.SetActive(value: false);
		}
		target.mainAnimator.SetBool(_isChargingAnimParam, value: true);
		target.mainAnimator.Play(_chargeUpAnim, 0, 1f);
		if ((bool)target.arriveActivate)
		{
			target.arriveActivate.SetActive(value: true);
		}
		tk2dSpriteAnimationClip clipByName2 = heroAnimator.GetClipByName("Collect Normal 2");
		heroAnimator.PlayFromFrame(clipByName2, clipByName2.frames.Length - 1);
		heroFlash.flashWhiteLong();
		if ((bool)shaftGlows)
		{
			shaftGlows.FadeTo(0f, shaftGlowsFadeTime);
		}
		cam.camTarget.PositionToStart();
		cam.SnapToY(cameraYRange.End);
		cam.SetMode(prevCamMode);
		yield return new WaitForSeconds(target.arriveWait);
		target.CancelTransport();
		ChargeUpStopped();
		yield return new WaitForSeconds(heroAnimator.PlayAnimGetTime("Collect Normal 3"));
		hc.RegainControl();
		hc.StartAnimationControl();
		hc.AffectedByGravity(gravityApplies: true);
		hc.cState.invulnerable = false;
		teleportRoutine = null;
		if (_isTeleportingLift == this)
		{
			_isTeleportingLift = null;
		}
	}
}
