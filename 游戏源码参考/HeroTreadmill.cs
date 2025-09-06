using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class HeroTreadmill : MonoBehaviour
{
	[Header("Treadmill")]
	[SerializeField]
	private ConveyorBelt conveyorBelt;

	[SerializeField]
	private Animator speedControlAnimator;

	[SerializeField]
	private CogRotationController cogRotationController;

	[SerializeField]
	private Transform vectorCurveAnimatorParent;

	[SerializeField]
	private ParticleSystemPool moveParticles;

	[SerializeField]
	private AudioSource runSource;

	[SerializeField]
	private AudioEvent startRunSound;

	[SerializeField]
	private AudioSource sprintSource;

	[SerializeField]
	private AudioEvent startSprintSound;

	[SerializeField]
	private AudioEvent endRunSound;

	[Space]
	[SerializeField]
	private MinMaxFloat speedRange;

	[SerializeField]
	private MinMaxFloat speedXRange;

	[SerializeField]
	private float speedLerpMultiplier;

	[Space]
	[SerializeField]
	private float heroReferenceSpeed;

	[Header("Gauge")]
	[SerializeField]
	private Transform needlePivot;

	[SerializeField]
	private PlayMakerFSM needleFsm;

	[SerializeField]
	private AudioSource needleRiseLoop;

	[Space]
	[SerializeField]
	private MinMaxFloat needleRange;

	[SerializeField]
	private float needleTarget;

	[SerializeField]
	private float needleRiseSpeed;

	[SerializeField]
	private float needleRiseLerpSpeed;

	[SerializeField]
	private float needleFallDelay;

	[SerializeField]
	private float needleFallSpeed;

	[SerializeField]
	private float needleFallLerpSpeed;

	[SerializeField]
	private JitterSelf needleJitter;

	[SerializeField]
	private float needleFpsLimit;

	private float oldSpeedMult = -1f;

	private HeroController capturedHero;

	private HeroController lastCapturedHero;

	private float targetSpeed;

	private float multiplier;

	private float currentSpeed;

	private bool wasMoving;

	private bool forceNeedleDrop;

	private double needleUpdateTime;

	private VectorCurveAnimator[] curveAnimators;

	private static readonly int _speedAnimatorParam = Animator.StringToHash("Speed");

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(new Vector3(speedXRange.Start, 2f, 0f), new Vector3(speedXRange.Start, 4f, 0f));
		Gizmos.DrawLine(new Vector3(speedXRange.End, 2f, 0f), new Vector3(speedXRange.End, 4f, 0f));
	}

	private void OnValidate()
	{
		if (needleFpsLimit < 0f)
		{
			needleFpsLimit = 0f;
		}
	}

	private void Awake()
	{
		curveAnimators = (vectorCurveAnimatorParent ? vectorCurveAnimatorParent.GetComponentsInChildren<VectorCurveAnimator>() : new VectorCurveAnimator[0]);
		conveyorBelt.CapturedHero += delegate(HeroController hero)
		{
			if ((bool)capturedHero)
			{
				capturedHero.BeforeApplyConveyorSpeed -= OnBeforeHeroConveyor;
			}
			if ((bool)hero)
			{
				hero.BeforeApplyConveyorSpeed += OnBeforeHeroConveyor;
				lastCapturedHero = hero;
			}
			else
			{
				targetSpeed = 0f;
			}
			capturedHero = hero;
		};
	}

	private void Start()
	{
		SetSpeedMultiplier(0f);
		StartCoroutine(NeedleControlRoutine());
	}

	private void OnDisable()
	{
		if ((bool)capturedHero)
		{
			capturedHero.BeforeApplyConveyorSpeed -= OnBeforeHeroConveyor;
		}
	}

	private void Update()
	{
		if (Math.Abs(currentSpeed - targetSpeed) < 0.1f)
		{
			currentSpeed = targetSpeed;
		}
		currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedLerpMultiplier);
		SetSpeedMultiplier(currentSpeed * multiplier);
		bool flag = currentSpeed > 0.5f;
		if (flag)
		{
			if (!wasMoving)
			{
				cogRotationController.ResetNextUpdateTime();
				moveParticles.PlayParticles();
				if (lastCapturedHero.cState.isSprinting)
				{
					startSprintSound.SpawnAndPlayOneShot(sprintSource.transform.position);
				}
				else
				{
					startRunSound.SpawnAndPlayOneShot(runSource.transform.position);
				}
			}
			if (lastCapturedHero.cState.isSprinting)
			{
				if (!sprintSource.isPlaying)
				{
					sprintSource.Play();
				}
				if (runSource.isPlaying)
				{
					runSource.Stop();
				}
			}
			else
			{
				if (sprintSource.isPlaying)
				{
					sprintSource.Stop();
				}
				if (!runSource.isPlaying)
				{
					runSource.Play();
				}
			}
		}
		else if (wasMoving)
		{
			moveParticles.StopParticles();
			endRunSound.SpawnAndPlayOneShot(runSource.transform.position);
			runSource.Stop();
			sprintSource.Stop();
		}
		wasMoving = flag;
	}

	private void OnBeforeHeroConveyor(Vector2 heroVelocity)
	{
		if (heroVelocity.x > 0f)
		{
			float x = base.transform.InverseTransformPoint(capturedHero.transform.position).x;
			float tBetween = speedXRange.GetTBetween(x);
			targetSpeed = ((tBetween > 0f) ? speedRange.GetLerpUnclampedValue(tBetween) : speedRange.Start);
			multiplier = heroVelocity.x / heroReferenceSpeed;
		}
		else
		{
			targetSpeed = 0f;
		}
	}

	private void SetSpeedMultiplier(float value)
	{
		if (!(Math.Abs(value - oldSpeedMult) < 0.001f))
		{
			oldSpeedMult = value;
			conveyorBelt.SpeedMultiplier = value;
			speedControlAnimator.SetFloat(_speedAnimatorParam, value);
			cogRotationController.RotationMultiplier = value;
			VectorCurveAnimator[] array = curveAnimators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SpeedMultiplier = value;
			}
		}
	}

	private IEnumerator NeedleControlRoutine()
	{
		SetNeedlePosition(0f);
		needleJitter.StopJitter();
		float needleT = 0f;
		float needleSpeed = 0f;
		float fallDelayElapsed = 0f;
		float targetT = needleRange.GetTBetween(needleTarget);
		bool wasAboveTarget = false;
		bool wasNeedleMoving = false;
		while (true)
		{
			if (wasMoving && !forceNeedleDrop)
			{
				needleSpeed = Mathf.Lerp(needleSpeed, needleRiseSpeed * multiplier, Time.deltaTime * needleRiseLerpSpeed);
				fallDelayElapsed = 0f;
			}
			else if (needleT <= 0f)
			{
				needleSpeed = 0f;
			}
			else if (fallDelayElapsed >= needleFallDelay || forceNeedleDrop)
			{
				needleSpeed = Mathf.Lerp(needleSpeed, 0f - needleFallSpeed, Time.deltaTime * needleFallLerpSpeed);
			}
			else
			{
				needleSpeed = Mathf.Lerp(needleSpeed, 0f, Time.deltaTime * needleRiseLerpSpeed);
				fallDelayElapsed += Time.deltaTime;
			}
			bool flag = Math.Abs(needleSpeed) > 0.01f;
			if (flag)
			{
				if (!wasNeedleMoving)
				{
					needleJitter.StartJitter();
					needleRiseLoop.Play();
				}
			}
			else if (wasNeedleMoving)
			{
				needleJitter.StopJitter();
				needleRiseLoop.Stop();
			}
			needleT = Mathf.Clamp01(needleT + needleSpeed * Time.deltaTime);
			if (needleFpsLimit > 0f)
			{
				if (Time.timeAsDouble > needleUpdateTime)
				{
					needleUpdateTime = Time.timeAsDouble + (double)(1f / needleFpsLimit);
					SetNeedlePosition(needleT);
				}
			}
			else
			{
				SetNeedlePosition(needleT);
			}
			if (needleT <= 0.01f)
			{
				forceNeedleDrop = false;
			}
			bool flag2 = needleT >= targetT;
			if (flag2)
			{
				if (!wasAboveTarget)
				{
					needleFsm.SendEvent("NEEDLE ABOVE");
				}
			}
			else if (wasAboveTarget)
			{
				needleFsm.SendEvent("NEEDLE BELOW");
			}
			wasAboveTarget = flag2;
			wasNeedleMoving = flag;
			yield return null;
		}
	}

	private void SetNeedlePosition(float value)
	{
		float lerpedValue = needleRange.GetLerpedValue(value);
		needlePivot.transform.SetLocalRotation2D(lerpedValue);
	}

	public void DropNeedle()
	{
		forceNeedleDrop = true;
	}
}
