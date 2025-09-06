using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class BellBench : UnlockablePropBase
{
	[SerializeField]
	private Animator fakeBenchAnimator;

	[SerializeField]
	private AudioEvent benchAppearSound;

	[SerializeField]
	private AudioEvent benchActivateSound;

	[SerializeField]
	private NestedFadeGroupBase realBenchFade;

	[SerializeField]
	private float realBenchFadeInTime;

	[SerializeField]
	private float benchRaiseDelay;

	[SerializeField]
	private PlayMakerFSM rosaryMachine;

	[SerializeField]
	private GameObject tollMachine;

	[SerializeField]
	private Transform frame;

	[SerializeField]
	private Vector2 downPositionUnlocked;

	[SerializeField]
	private Vector2 downPositionLocked;

	[SerializeField]
	private float moveUpSpeed;

	[SerializeField]
	private AnimationCurve moveUpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float moveUpPause;

	[SerializeField]
	private CogRotationController cogController;

	[SerializeField]
	private float cogRotationMultiplier = 1f;

	[SerializeField]
	private float shakeDuration = 1f;

	[SerializeField]
	private float shakeIntensity = 5f;

	[SerializeField]
	private float shakeFrequency = 10f;

	[SerializeField]
	private CameraShakeTarget cameraRumble;

	[SerializeField]
	private CameraShakeTarget cameraRumbleEndShake;

	[SerializeField]
	private PlayParticleEffects rumbleDust;

	[SerializeField]
	private PlayParticleEffects raiseDust;

	[SerializeField]
	private PlayParticleEffects raiseStopDust;

	[SerializeField]
	private AudioSource riseSource;

	[SerializeField]
	private VibrationDataAsset riseVibration;

	[SerializeField]
	private AudioEvent arriveSound;

	[SerializeField]
	private AudioSource lowerSource;

	[SerializeField]
	private VibrationDataAsset lowerVibration;

	[Space]
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	[SerializeField]
	private SpriteRenderer frameRenderer;

	[SerializeField]
	private Color frameInactiveColor = Color.grey;

	[SerializeField]
	private float frameColorLerpTime = 0.5f;

	[SerializeField]
	private PlayMakerFSM bellToneFSM;

	[SerializeField]
	private GameObject[] activateWhenActive;

	[Space]
	[SerializeField]
	private PersistentBoolItem startWorkingPersistent;

	[SerializeField]
	private TrackTriggerObjects startWorkingTrigger;

	[SerializeField]
	private bool startUnlocked;

	[Header("Broken Options")]
	[SerializeField]
	private Animator brokenAnimator;

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string fixedPDBool;

	[SerializeField]
	private TrackTriggerObjects brokenShakeRange;

	[SerializeField]
	private float fixedRiseDelay;

	[Header("Events")]
	public UnityEvent OnBeginRaise;

	public UnityEvent OnEndRaise;

	private readonly int appearAnim = Animator.StringToHash("Appear");

	private readonly int silentAppearAnim = Animator.StringToHash("Silent Appear");

	private bool isSetup;

	private Vector3 initialFramePos;

	private bool isRaiseBlocked;

	private Coroutine behaviourRoutine;

	private Coroutine musicRoutine;

	private bool isActivated;

	private bool isHeroInPosition;

	private bool isRumbling;

	private bool forceStartDown;

	private Color initialFrameColor;

	private Rigidbody2DDisturberImpulse[] bellDisturbersImpulse;

	private Rigidbody2DDisturber[] bellDisturbersRumble;

	private VibrationEmission riseEmission;

	private VibrationEmission lowerEmission;

	public bool IsBenchBroken
	{
		get
		{
			if (!string.IsNullOrEmpty(fixedPDBool))
			{
				return !PlayerData.instance.GetVariable<bool>(fixedPDBool);
			}
			return false;
		}
	}

	private void Awake()
	{
		if ((bool)startWorkingPersistent)
		{
			isRaiseBlocked = true;
			startWorkingPersistent.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					isRaiseBlocked = false;
				}
			};
		}
		if ((bool)brokenAnimator)
		{
			brokenAnimator.enabled = false;
		}
		if ((bool)startWorkingTrigger)
		{
			isRaiseBlocked = true;
		}
		bellDisturbersImpulse = GetComponentsInChildren<Rigidbody2DDisturberImpulse>();
		bellDisturbersRumble = GetComponentsInChildren<Rigidbody2DDisturber>();
		if (startUnlocked && (bool)tollMachine)
		{
			tollMachine.SetActive(value: false);
		}
	}

	private void Start()
	{
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			isHeroInPosition = true;
			Setup();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			isHeroInPosition = true;
			Setup();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	private void Setup()
	{
		if (!isSetup && (bool)frame)
		{
			initialFramePos = frame.localPosition;
			if ((bool)frameRenderer)
			{
				initialFrameColor = frameRenderer.color;
			}
			activateWhenActive.SetAllActive(value: false);
			isSetup = true;
			StartBehaviour();
			if (startUnlocked)
			{
				Opened();
			}
			else
			{
				isActivated = false;
			}
		}
	}

	private void SetStartingBenchState(bool startUp)
	{
		if (!startUp)
		{
			frame.SetLocalPosition2D(isActivated ? downPositionUnlocked : downPositionLocked);
			realBenchFade.gameObject.SetActive(value: false);
			rosaryMachine.SetFsmBoolIfExists("Raise", value: false);
			rosaryMachine.SendEventSafe("START DOWN");
			if ((bool)frameRenderer)
			{
				frameRenderer.color = (isActivated ? initialFrameColor : frameInactiveColor);
			}
		}
		else
		{
			realBenchFade.AlphaSelf = 1f;
			realBenchFade.gameObject.SetActive(value: true);
			rosaryMachine.SetFsmBoolIfExists("Raise", value: true);
			rosaryMachine.SendEventSafe("START UP");
		}
	}

	private void StartBehaviour()
	{
		if (behaviourRoutine != null)
		{
			StopCoroutine(behaviourRoutine);
		}
		behaviourRoutine = StartCoroutine(Behaviour());
	}

	private IEnumerator Behaviour()
	{
		while (!isHeroInPosition)
		{
			yield return null;
		}
		yield return null;
		bool startUp;
		if (forceStartDown)
		{
			startUp = false;
			forceStartDown = false;
		}
		else
		{
			startUp = isActivated && !IsBenchBroken;
		}
		SetStartingBenchState(startUp);
		fakeBenchAnimator.gameObject.SetActive(value: false);
		activateWhenActive.SetAllActive(isActivated);
		bool wasActivated = isActivated;
		while (!isActivated)
		{
			yield return null;
		}
		if (!wasActivated && (bool)frameRenderer)
		{
			this.StartTimerRoutine(0f, frameColorLerpTime, delegate(float t)
			{
				frameRenderer.color = Color.Lerp(frameInactiveColor, initialFrameColor, t);
			});
		}
		if (IsBenchBroken)
		{
			frame.SetLocalPosition2D(downPositionLocked);
			if ((bool)brokenAnimator)
			{
				brokenAnimator.enabled = true;
				brokenAnimator.Play("Rise", 0, wasActivated ? 1f : 0f);
			}
			while (IsBenchBroken)
			{
				yield return null;
			}
			EndBrokenState();
			yield return new WaitForSeconds(fixedRiseDelay);
			forceStartDown = true;
			StartBehaviour();
			yield break;
		}
		if (startUp)
		{
			isRaiseBlocked = false;
		}
		while (isRaiseBlocked)
		{
			yield return null;
			if ((bool)startWorkingTrigger && startWorkingTrigger.IsInside)
			{
				isRaiseBlocked = false;
			}
		}
		if (!startUp)
		{
			yield return new WaitForSeconds(moveUpPause);
			if (OnBeginRaise != null)
			{
				OnBeginRaise.Invoke();
			}
			StartRumble(isRaising: true);
			fakeBenchAnimator.gameObject.SetActive(value: false);
			yield return StartCoroutine(MoveFrameTo(initialFramePos, moveUpSpeed, moveUpCurve));
			arriveSound.SpawnAndPlayOneShot(base.transform.position);
			StopRumble(isRaising: false);
			if (cogController != null && shakeDuration > 0f)
			{
				StartCoroutine(CogShake());
			}
			if (OnEndRaise != null)
			{
				OnEndRaise.Invoke();
			}
			yield return new WaitForSeconds(benchRaiseDelay);
			benchAppearSound.SpawnAndPlayOneShot(fakeBenchAnimator.transform.position);
			fakeBenchAnimator.gameObject.SetActive(value: true);
			fakeBenchAnimator.Play(appearAnim);
			yield return null;
			yield return new WaitForSeconds(fakeBenchAnimator.GetCurrentAnimatorStateInfo(0).length);
			rosaryMachine.SetFsmBoolIfExists("Raise", value: true);
			realBenchFade.AlphaSelf = 0f;
			realBenchFade.gameObject.SetActive(value: true);
			benchActivateSound.SpawnAndPlayOneShot(realBenchFade.transform.position);
			yield return new WaitForSeconds(realBenchFade.FadeTo(1f, realBenchFadeInTime));
		}
		else
		{
			fakeBenchAnimator.gameObject.SetActive(value: true);
			fakeBenchAnimator.Play(silentAppearAnim, 0, 1f);
			realBenchFade.AlphaSelf = 1f;
			realBenchFade.gameObject.SetActive(value: true);
		}
		activateWhenActive.SetAllActive(value: true);
	}

	[ContextMenu("Cog Shake")]
	private void DoCogShake()
	{
		if ((bool)cogController && shakeDuration > 0f)
		{
			StartCoroutine(CogShake());
		}
	}

	private IEnumerator CogShake()
	{
		float t = 1f;
		float inv = 1f / shakeDuration;
		float lastShakeAmount = 0f;
		while (t > 0f)
		{
			t -= Time.deltaTime * inv;
			float num = shakeIntensity * Mathf.Clamp01(t);
			float num2 = Mathf.PerlinNoise1D(Time.time * shakeFrequency) * 2f - 1f;
			float num3 = num * num2;
			float num4 = num3 - lastShakeAmount;
			lastShakeAmount = num3;
			cogController.AnimateRotation += num4;
			yield return null;
		}
	}

	private void EndBrokenState()
	{
		if ((bool)brokenAnimator)
		{
			brokenAnimator.enabled = false;
		}
		StopRumble(isRaising: true);
		StopRumble(isRaising: false);
	}

	private IEnumerator MoveFrameTo(Vector2 targetPosition, float speed, AnimationCurve curve)
	{
		if ((bool)cogController)
		{
			cogController.CaptureAnimateRotation();
		}
		Vector2 startPosition = frame.localPosition;
		float num = Vector2.Distance(startPosition, targetPosition);
		float lerpTime = num / speed;
		for (float elapsed = 0f; elapsed < lerpTime; elapsed += Time.deltaTime)
		{
			float num2 = curve.Evaluate(elapsed / lerpTime);
			frame.SetLocalPosition2D(Vector2.Lerp(startPosition, targetPosition, num2));
			if ((bool)cogController)
			{
				cogController.AnimateRotation = num2 * cogRotationMultiplier;
			}
			yield return null;
		}
		frame.SetLocalPosition2D(targetPosition);
	}

	private bool CanDoEffects()
	{
		if (!IsBenchBroken)
		{
			return true;
		}
		if ((bool)brokenShakeRange)
		{
			return brokenShakeRange.IsInside;
		}
		return true;
	}

	private void StartRumble(bool isRaising)
	{
		if (isRumbling)
		{
			return;
		}
		isRumbling = true;
		if (CanDoEffects())
		{
			cameraRumble.DoShake(this);
			if (isRaising)
			{
				raiseDust.PlayParticleSystems();
			}
			rumbleDust.PlayParticleSystems();
		}
		AudioSource audioSource = (isRaising ? riseSource : lowerSource);
		if ((bool)audioSource)
		{
			audioSource.Play();
		}
		if (isRaising)
		{
			if ((bool)riseVibration)
			{
				if ((bool)heroVibrationRegion)
				{
					riseEmission = heroVibrationRegion.PlayVibrationOneShot(riseVibration, requireInside: false);
				}
				else
				{
					riseEmission = VibrationManager.PlayVibrationClipOneShot(riseVibration, null);
				}
			}
		}
		else if ((bool)lowerVibration)
		{
			if ((bool)heroVibrationRegion)
			{
				lowerEmission = heroVibrationRegion.PlayVibrationOneShot(lowerVibration, requireInside: false, HeroVibrationRegion.VibrationSettings.Loop);
			}
			else
			{
				lowerEmission = VibrationManager.PlayVibrationClipOneShot(lowerVibration, null, isLooping: true);
			}
		}
		Rigidbody2DDisturber[] array = bellDisturbersRumble;
		foreach (Rigidbody2DDisturber rigidbody2DDisturber in array)
		{
			if (rigidbody2DDisturber.isActiveAndEnabled)
			{
				rigidbody2DDisturber.StartRumble();
			}
		}
	}

	public void StartRumbleRaising()
	{
		StartRumble(isRaising: true);
	}

	public void StartRumbleLowering()
	{
		StartRumble(isRaising: false);
	}

	private void StopRumble(bool isRaising)
	{
		cameraRumble.CancelShake();
		raiseDust.StopParticleSystems();
		rumbleDust.StopParticleSystems();
		if (isRaising)
		{
			riseEmission?.Stop();
			riseEmission = null;
		}
		else
		{
			if ((bool)lowerSource)
			{
				lowerSource.Stop();
			}
			lowerEmission?.Stop();
			lowerEmission = null;
		}
		Rigidbody2DDisturber[] array = bellDisturbersRumble;
		foreach (Rigidbody2DDisturber rigidbody2DDisturber in array)
		{
			if (rigidbody2DDisturber.isActiveAndEnabled)
			{
				rigidbody2DDisturber.StopRumble();
			}
		}
		if (!isRumbling)
		{
			return;
		}
		isRumbling = false;
		if (CanDoEffects())
		{
			cameraRumbleEndShake.DoShake(this);
			raiseStopDust.PlayParticleSystems();
		}
		Rigidbody2DDisturberImpulse[] array2 = bellDisturbersImpulse;
		foreach (Rigidbody2DDisturberImpulse rigidbody2DDisturberImpulse in array2)
		{
			if (rigidbody2DDisturberImpulse.isActiveAndEnabled)
			{
				rigidbody2DDisturberImpulse.Impulse();
			}
		}
	}

	public void StopRumbleRaising()
	{
		StopRumble(isRaising: true);
	}

	public void StopRumbleLowering()
	{
		StopRumble(isRaising: false);
	}

	public override void Open()
	{
		bellToneFSM.SendEventSafe("ACTIVATED");
		isActivated = true;
	}

	public override void Opened()
	{
		Setup();
		Open();
		StartBehaviour();
		if ((bool)lowerSource)
		{
			lowerSource.Stop();
		}
	}
}
