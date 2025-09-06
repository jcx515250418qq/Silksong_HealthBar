using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BouncePodSequence : MonoBehaviour
{
	[Serializable]
	private class Sequence
	{
		public List<BouncePod> Pods;
	}

	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	private static readonly int _turnStartAnim = Animator.StringToHash("Turn Start");

	private static readonly int _turnStopAnim = Animator.StringToHash("Turn Stop");

	private static readonly int _turnFailAnim = Animator.StringToHash("Turn Fail");

	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	private TempPressurePlate startSequencePlate;

	[SerializeField]
	private float sequencePlayDelay;

	[SerializeField]
	private float sequencePodRingWait;

	[Space]
	[SerializeField]
	private Vector3 audioEventPosition;

	[Space]
	[SerializeField]
	private Transform progressIndicator;

	[SerializeField]
	private MinMaxFloat progressIndicatorRotationExtents;

	[SerializeField]
	private AnimationCurve progressIndicatorRotateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float progressIndicatorRotateDuration;

	[SerializeField]
	private float progressIndicatorRotateDelay;

	[SerializeField]
	private AudioEvent progressIndicatorMoveSound;

	[Space]
	[SerializeField]
	private Animator cogsAnimator;

	[SerializeField]
	private CogRotationController cogsRotation;

	[SerializeField]
	private float hitCogRotateAmount;

	[SerializeField]
	private AnimationCurve hitCogRotateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float hitCogRotateDuration;

	[SerializeField]
	private PlayParticleEffects sequenceStartEffects;

	[SerializeField]
	private CameraShakeTarget sequenceStartShake;

	[SerializeField]
	private AudioEvent platePressedAudio;

	[SerializeField]
	private AudioEvent sequenceStartAudio;

	[SerializeField]
	private AudioEvent sequenceStartStopAudio;

	[Space]
	[SerializeField]
	private PlayParticleEffects completionLoopEffects;

	[SerializeField]
	private CameraShakeTarget completionRumble;

	[SerializeField]
	private CameraShakeTarget completionRumbleStopShake;

	[SerializeField]
	private float unlockBellRingDelay;

	[SerializeField]
	private AudioEvent unlockBellRingSound;

	[SerializeField]
	private float unlockDelay;

	[SerializeField]
	private float unlockDuration;

	[SerializeField]
	private float rewardAppearDelay;

	[Space]
	[SerializeField]
	private List<Sequence> sequences;

	[SerializeField]
	private Animator rewardPillar;

	[SerializeField]
	private AudioEvent rewardAppearSound;

	[SerializeField]
	private UnlockablePropBase[] unlockables;

	private AnimatorActivatingStates[] podHolders;

	private BouncePod[] pods;

	private int currentSequenceIndex;

	private bool isComplete;

	private bool usingRewardPillar;

	private Coroutine playSequenceRoutine;

	private int targetPodIndex;

	private bool queuedFail;

	private bool queuedSuccess;

	private bool anonPodWasHit;

	private Coroutine indicatorAnimationRoutine;

	private Quaternion currentIndicatorRotation;

	private Quaternion targetIndicatorRotation;

	private Coroutine cogHitRotationRoutine;

	private float currentCogHitRotation;

	private float targetCogHitRotation;

	private AudioSource runningSequenceStartAudio;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(audioEventPosition, 0.2f);
	}

	private void OnValidate()
	{
		if (sequences.Count == 0)
		{
			sequences.Add(new Sequence
			{
				Pods = new List<BouncePod>()
			});
		}
	}

	private void Awake()
	{
		OnValidate();
		if ((bool)startSequencePlate)
		{
			startSequencePlate.Activated += OnPlatePressed;
		}
		foreach (Sequence sequence in sequences)
		{
			for (int num = sequence.Pods.Count - 1; num >= 0; num--)
			{
				if (!sequence.Pods[num])
				{
					sequence.Pods.RemoveAt(num);
				}
			}
		}
		pods = GetComponentsInChildren<BouncePod>(includeInactive: true);
		BouncePod[] array = pods;
		foreach (BouncePod obj in array)
		{
			obj.BounceHit += OnAnonPodHit;
			obj.InertHit += OnAnonPodHit;
		}
		for (int j = 0; j < sequences.Count; j++)
		{
			List<BouncePod> list = sequences[j].Pods;
			for (int k = 0; k < list.Count; k++)
			{
				BouncePod bouncePod = list[k];
				int sequenceIndex = j;
				int capturedIndex = k;
				Action value2 = delegate
				{
					OnPodHit(sequenceIndex, capturedIndex);
				};
				bouncePod.BounceHit += value2;
				bouncePod.InertHit += value2;
			}
		}
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out int value)
			{
				value = currentSequenceIndex;
			};
			persistent.OnSetSaveState += delegate(int value)
			{
				currentSequenceIndex = value;
				UpdateProgressIndicator(isInstant: true);
				isComplete = currentSequenceIndex >= sequences.Count;
				if (isComplete)
				{
					if ((bool)startSequencePlate)
					{
						startSequencePlate.ActivateSilent();
					}
					Unlock(isInstant: true);
				}
			};
		}
		podHolders = GetComponentsInChildren<AnimatorActivatingStates>();
		if ((bool)rewardPillar)
		{
			usingRewardPillar = rewardPillar.gameObject.activeInHierarchy;
			rewardPillar.gameObject.SetActive(value: false);
		}
		UpdateProgressIndicator(isInstant: true);
		ResetSequence(isInstant: true);
	}

	private void Update()
	{
		if (anonPodWasHit)
		{
			ResetSequence();
		}
		if (queuedSuccess)
		{
			PodHitSuccess();
		}
		else if (queuedFail)
		{
			PodHitFail();
		}
		queuedSuccess = false;
		queuedFail = false;
	}

	private void OnPlatePressed()
	{
		SetCogsRotating(value: true);
		platePressedAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(audioEventPosition));
		StartSequence();
	}

	private void StartSequence()
	{
		if (playSequenceRoutine != null)
		{
			StopCoroutine(playSequenceRoutine);
		}
		playSequenceRoutine = StartCoroutine(PlaySequence());
	}

	private IEnumerator PlaySequence()
	{
		if ((bool)sequenceStartEffects)
		{
			sequenceStartEffects.PlayParticleSystems();
		}
		sequenceStartShake.DoShake(this);
		AnimatorActivatingStates[] array = podHolders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		if (sequencePlayDelay > 0f)
		{
			yield return new WaitForSeconds(sequencePlayDelay);
		}
		WaitForSeconds wait = new WaitForSeconds(sequencePodRingWait);
		for (int j = 0; j < sequences[currentSequenceIndex].Pods.Count; j++)
		{
			BouncePod pod = sequences[currentSequenceIndex].Pods[j];
			if ((bool)pod)
			{
				if (j > 0)
				{
					yield return wait;
				}
				pod.Ring();
			}
		}
		SetCogsRotating(value: false);
		targetPodIndex = 0;
		playSequenceRoutine = null;
	}

	private void OnAnonPodHit()
	{
		if (!isComplete && targetPodIndex >= 0)
		{
			anonPodWasHit = true;
		}
	}

	private void OnPodHit(int sequenceIndex, int hitPodIndex)
	{
		if (!isComplete && targetPodIndex >= 0 && sequenceIndex == currentSequenceIndex)
		{
			anonPodWasHit = false;
			if (hitPodIndex == targetPodIndex)
			{
				queuedSuccess = true;
				queuedFail = false;
			}
			else if (!queuedSuccess)
			{
				queuedFail = true;
			}
		}
	}

	private void PodHitSuccess()
	{
		targetPodIndex++;
		HitProgressCogTurn();
		if (targetPodIndex >= sequences[currentSequenceIndex].Pods.Count)
		{
			currentSequenceIndex++;
			UpdateProgressIndicator(isInstant: false);
			if (currentSequenceIndex >= sequences.Count)
			{
				Unlock(isInstant: false);
			}
			else
			{
				ResetSequence(isInstant: false, autoStartNextSequence: true);
			}
		}
	}

	private void PodHitFail()
	{
		ResetSequence();
	}

	private void ResetSequence(bool isInstant = false, bool autoStartNextSequence = false)
	{
		anonPodWasHit = false;
		targetPodIndex = -1;
		if (playSequenceRoutine != null)
		{
			StopCoroutine(playSequenceRoutine);
		}
		if (isInstant)
		{
			if ((bool)startSequencePlate)
			{
				startSequencePlate.Deactivate();
			}
			AnimatorActivatingStates[] array = podHolders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		else
		{
			if (!autoStartNextSequence)
			{
				PlayCogAnim(_turnFailAnim);
			}
			StartCoroutine(ResetInteractivePartsDelayed(autoStartNextSequence));
		}
	}

	private IEnumerator ResetInteractivePartsDelayed(bool autoStartNextSequence)
	{
		while (indicatorAnimationRoutine != null)
		{
			yield return null;
		}
		if (autoStartNextSequence)
		{
			StartSequence();
			yield break;
		}
		AnimatorActivatingStates[] array = podHolders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		if ((bool)cogsAnimator)
		{
			yield return null;
			yield return new WaitForSeconds(cogsAnimator.GetCurrentAnimatorStateInfo(0).length + sequencePlayDelay);
		}
		if ((bool)startSequencePlate)
		{
			startSequencePlate.Deactivate();
		}
	}

	private void Unlock(bool isInstant)
	{
		isComplete = true;
		if (isInstant)
		{
			if (usingRewardPillar)
			{
				rewardPillar.gameObject.SetActive(value: true);
				rewardPillar.Play(_appearAnim, 0, 1f);
			}
			UnlockablePropBase[] array = unlockables;
			foreach (UnlockablePropBase unlockablePropBase in array)
			{
				if ((bool)unlockablePropBase)
				{
					unlockablePropBase.Opened();
				}
			}
		}
		else
		{
			StartCoroutine(CompletionSequence());
		}
	}

	private void UpdateProgressIndicator(bool isInstant)
	{
		if (!progressIndicator)
		{
			return;
		}
		float t2 = (float)currentSequenceIndex / (float)sequences.Count;
		targetIndicatorRotation = Quaternion.Euler(0f, 0f, progressIndicatorRotationExtents.GetLerpedValue(t2));
		if (indicatorAnimationRoutine != null)
		{
			StopCoroutine(indicatorAnimationRoutine);
			indicatorAnimationRoutine = null;
		}
		if (isInstant || progressIndicatorRotateDuration <= 0f)
		{
			progressIndicator.localRotation = targetIndicatorRotation;
			return;
		}
		currentIndicatorRotation = progressIndicator.localRotation;
		SetCogsRotating(value: true);
		indicatorAnimationRoutine = this.StartTimerRoutine(progressIndicatorRotateDelay, progressIndicatorRotateDuration, delegate(float t)
		{
			t = progressIndicatorRotateCurve.Evaluate(t);
			progressIndicator.localRotation = Quaternion.LerpUnclamped(currentIndicatorRotation, targetIndicatorRotation, t);
		}, delegate
		{
			progressIndicatorMoveSound.SpawnAndPlayOneShot(base.transform.TransformPoint(audioEventPosition));
		}, delegate
		{
			indicatorAnimationRoutine = null;
		});
	}

	private void HitProgressCogTurn()
	{
		if (!cogsRotation)
		{
			return;
		}
		currentCogHitRotation = targetCogHitRotation;
		targetCogHitRotation = currentCogHitRotation + hitCogRotateAmount;
		if (cogHitRotationRoutine != null)
		{
			StopCoroutine(cogHitRotationRoutine);
			cogHitRotationRoutine = null;
		}
		if (hitCogRotateDuration <= 0f)
		{
			cogsRotation.ApplyRotation(targetCogHitRotation);
			return;
		}
		cogHitRotationRoutine = this.StartTimerRoutine(0f, hitCogRotateDuration, delegate(float t)
		{
			t = hitCogRotateCurve.Evaluate(t);
			cogsRotation.ApplyRotation(Mathf.LerpUnclamped(currentCogHitRotation, targetCogHitRotation, t));
		});
	}

	private void PlayCogAnim(int animHash)
	{
		if ((bool)cogsAnimator)
		{
			cogsAnimator.Play(animHash);
			if ((bool)cogsRotation)
			{
				cogsRotation.CaptureAnimateRotation();
			}
			cogsAnimator.Update(0f);
		}
	}

	private IEnumerator CompletionSequence()
	{
		while (indicatorAnimationRoutine != null)
		{
			yield return null;
		}
		if ((bool)sequenceStartEffects)
		{
			sequenceStartEffects.PlayParticleSystems();
		}
		sequenceStartShake.DoShake(this);
		SetCogsRotating(value: false);
		yield return new WaitForSeconds(unlockBellRingDelay);
		unlockBellRingSound.SpawnAndPlayOneShot(base.transform.TransformPoint(audioEventPosition));
		BouncePod[] array = pods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Ring(playSound: false);
		}
		yield return new WaitForSeconds(sequencePodRingWait);
		AnimatorActivatingStates[] array2 = podHolders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(value: false);
		}
		yield return new WaitForSeconds(unlockDelay);
		SetCogsRotating(value: true);
		if ((bool)completionLoopEffects)
		{
			completionLoopEffects.PlayParticleSystems();
		}
		completionRumble.DoShake(this);
		yield return new WaitForSeconds(unlockDuration);
		SetCogsRotating(value: false);
		if ((bool)completionLoopEffects)
		{
			completionLoopEffects.StopParticleSystems();
		}
		completionRumble.CancelShake();
		completionRumbleStopShake.DoShake(this);
		if (rewardAppearDelay > 0f)
		{
			yield return new WaitForSeconds(rewardAppearDelay);
		}
		if (usingRewardPillar)
		{
			rewardAppearSound.SpawnAndPlayOneShot(rewardPillar.transform.position);
			rewardPillar.gameObject.SetActive(value: true);
			rewardPillar.Play(_appearAnim, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(rewardPillar.GetCurrentAnimatorStateInfo(0).length);
		}
		UnlockablePropBase[] array3 = unlockables;
		foreach (UnlockablePropBase unlockablePropBase in array3)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
	}

	private void SetCogsRotating(bool value)
	{
		if (value)
		{
			PlayCogAnim(_turnStartAnim);
			runningSequenceStartAudio = sequenceStartAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(audioEventPosition), delegate
			{
				runningSequenceStartAudio = null;
			});
			return;
		}
		PlayCogAnim(_turnStopAnim);
		if ((bool)runningSequenceStartAudio)
		{
			runningSequenceStartAudio.Stop();
			sequenceStartStopAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(audioEventPosition));
		}
	}
}
