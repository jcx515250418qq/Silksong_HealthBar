using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class HeroPerformanceSingReaction : MonoBehaviour
{
	private enum LookAnimNpcActivateBehaviours
	{
		Any = 0,
		FaceLeft = 1,
		FaceRight = 2
	}

	[Serializable]
	private class AnimGroup
	{
		public string IdleAnim;

		public string SingAnim;

		public string ReturnToIdleAnim;

		public string NoiseStartleAnim;
	}

	[SerializeField]
	private NPCControlBase npcControl;

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private Animator unityAnimator;

	[SerializeField]
	private LookAnimNPC lookAnimNPC;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("lookAnimNPC", true, false, true)]
	private LookAnimNpcActivateBehaviours lookAnimNPCActivate;

	[Space]
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string idleAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string singAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string returnToIdleAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string noiseStartleAnim;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingLeftAnims", true, true, true)]
	private AnimGroup leftAnims;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRightAnims", true, true, true)]
	private AnimGroup rightAnims;

	[SerializeField]
	private bool startleAnimEndsItself;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("startleAnimEndsItself", false, false, false)]
	private MinMaxFloat startleLoopTime;

	[SerializeField]
	private MinMaxFloat startDelay;

	[SerializeField]
	private MinMaxFloat endDelay;

	[SerializeField]
	private bool reactToOuter;

	[SerializeField]
	private bool ignoreNeedolinRange;

	[SerializeField]
	private MinMaxFloat singTimeRange;

	[SerializeField]
	private MinMaxFloat restTimeRange;

	[SerializeField]
	private NoiseMaker.Intensities minNoiseIntensity;

	[SerializeField]
	private MinMaxFloat noiseRespondDelay;

	[SerializeField]
	private float noiseRespondCooldown;

	[SerializeField]
	private NeedolinTextOwner needolinTextOwner;

	[SerializeField]
	private AudioSource singAudioSource;

	[SerializeField]
	private RandomAudioClipTable singAudioClipTable;

	[SerializeField]
	private RandomAudioClipTable startleAudioClipTable;

	[SerializeField]
	private bool startleUsesSingAudioSource;

	[SerializeField]
	private RandomAudioClipTable movementAudioClipTable;

	[Space]
	public UnityEvent OnSingStarted;

	public UnityEvent OnSingTrigger;

	public UnityEvent OnSingEnding;

	public UnityEvent OnSingEnded;

	public UnityEvent OnStartleStarted;

	public UnityEvent OnStartleEnded;

	private HeroPerformanceRegion.AffectedState affectedState;

	private float delay;

	private bool isInside;

	private bool wasForcedSoft;

	private double nextRespondTime;

	private Coroutine behaviourRoutine;

	private Coroutine startleRoutine;

	private bool disabledNpcControl;

	private bool hasUnityAnimator;

	private bool registeredAnimationTrigger;

	public bool IsForcedSoft { get; set; }

	public bool IsForced { get; set; }

	public bool IsForcedAny
	{
		get
		{
			if (!IsForced)
			{
				return IsForcedSoft;
			}
			return true;
		}
	}

	private bool ShouldSing
	{
		get
		{
			if (!isInside)
			{
				return IsForced;
			}
			return true;
		}
	}

	private bool IsUsingLeftAnims()
	{
		LookAnimNpcActivateBehaviours lookAnimNpcActivateBehaviours = lookAnimNPCActivate;
		return lookAnimNpcActivateBehaviours == LookAnimNpcActivateBehaviours.Any || lookAnimNpcActivateBehaviours == LookAnimNpcActivateBehaviours.FaceLeft;
	}

	private bool IsUsingRightAnims()
	{
		LookAnimNpcActivateBehaviours lookAnimNpcActivateBehaviours = lookAnimNPCActivate;
		return lookAnimNpcActivateBehaviours == LookAnimNpcActivateBehaviours.Any || lookAnimNpcActivateBehaviours == LookAnimNpcActivateBehaviours.FaceRight;
	}

	private void Reset()
	{
		npcControl = GetComponent<NPCControlBase>();
	}

	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(idleAnim))
		{
			leftAnims.IdleAnim = idleAnim;
			rightAnims.IdleAnim = idleAnim;
			idleAnim = null;
		}
		if (!string.IsNullOrEmpty(singAnim))
		{
			leftAnims.SingAnim = singAnim;
			rightAnims.SingAnim = singAnim;
			singAnim = null;
		}
		if (!string.IsNullOrEmpty(returnToIdleAnim))
		{
			leftAnims.ReturnToIdleAnim = returnToIdleAnim;
			rightAnims.ReturnToIdleAnim = returnToIdleAnim;
			returnToIdleAnim = null;
		}
		if (!string.IsNullOrEmpty(noiseStartleAnim))
		{
			leftAnims.NoiseStartleAnim = noiseStartleAnim;
			rightAnims.NoiseStartleAnim = noiseStartleAnim;
			noiseStartleAnim = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		if (!npcControl)
		{
			npcControl = GetComponent<NPCControlBase>();
		}
		hasUnityAnimator = unityAnimator;
	}

	private void OnEnable()
	{
		ComponentSingleton<HeroPerformanceSingReactionCallbackHooks>.Instance.OnUpdate += OnUpdate;
		NoiseMaker.NoiseCreated += OnNoiseCreated;
		StartBehaviour();
	}

	private void OnDisable()
	{
		ComponentSingleton<HeroPerformanceSingReactionCallbackHooks>.Instance.OnUpdate -= OnUpdate;
		NoiseMaker.NoiseCreated -= OnNoiseCreated;
		StopBehaviour();
		if (startleRoutine != null)
		{
			StopCoroutine(startleRoutine);
			startleRoutine = null;
		}
		if (disabledNpcControl)
		{
			ToggleNpcControl(active: true);
		}
		isInside = false;
		IsForced = false;
	}

	private void StopBehaviour()
	{
		if (behaviourRoutine != null)
		{
			StopCoroutine(behaviourRoutine);
			behaviourRoutine = null;
		}
		if ((bool)needolinTextOwner)
		{
			needolinTextOwner.RemoveNeedolinText();
		}
		UnregisterAnimationTrigger();
	}

	private void OnUpdate()
	{
		bool flag;
		if (IsForcedSoft)
		{
			if (!wasForcedSoft)
			{
				delay = startDelay.GetRandomValue();
				wasForcedSoft = true;
			}
			flag = true;
			this.affectedState = HeroPerformanceRegion.AffectedState.None;
		}
		else
		{
			wasForcedSoft = false;
			HeroPerformanceRegion.AffectedState affectedState = this.affectedState;
			this.affectedState = HeroPerformanceRegion.GetAffectedState(base.transform, ignoreNeedolinRange);
			if (this.affectedState != 0 && affectedState == HeroPerformanceRegion.AffectedState.None)
			{
				delay = startDelay.GetRandomValue();
			}
			else if (this.affectedState == HeroPerformanceRegion.AffectedState.None && affectedState != 0)
			{
				delay = endDelay.GetRandomValue();
			}
			flag = this.affectedState switch
			{
				HeroPerformanceRegion.AffectedState.ActiveInner => true, 
				HeroPerformanceRegion.AffectedState.ActiveOuter => reactToOuter, 
				HeroPerformanceRegion.AffectedState.None => false, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
		if (delay <= 0f && flag != isInside)
		{
			isInside = flag;
		}
		delay -= Time.deltaTime;
	}

	private void StartBehaviour()
	{
		if (behaviourRoutine != null)
		{
			StopCoroutine(behaviourRoutine);
		}
		behaviourRoutine = StartCoroutine(Behaviour());
	}

	private void RegisterAnimationTrigger()
	{
		if (!registeredAnimationTrigger && (bool)animator)
		{
			registeredAnimationTrigger = true;
			animator.AnimationEventTriggeredEvent += OnAnimationEventTriggered;
		}
	}

	private void UnregisterAnimationTrigger()
	{
		if (registeredAnimationTrigger)
		{
			registeredAnimationTrigger = false;
			if ((bool)animator)
			{
				animator.AnimationEventTriggeredEvent -= OnAnimationEventTriggered;
			}
		}
	}

	private IEnumerator Behaviour()
	{
		AnimGroup currentAnimGroup = GetCurrentAnimGroup();
		if (!lookAnimNPC)
		{
			if ((bool)animator)
			{
				animator.TryPlay(currentAnimGroup.IdleAnim);
			}
			if (hasUnityAnimator)
			{
				unityAnimator.Play(currentAnimGroup.IdleAnim);
			}
		}
		while (true)
		{
			if (!ShouldSing)
			{
				yield return null;
				continue;
			}
			if ((bool)lookAnimNPC)
			{
				lookAnimNPC.DeactivateInstant();
			}
			ToggleNpcControl(active: false);
			while (true)
			{
				IL_00d8:
				currentAnimGroup = GetCurrentAnimGroup();
				if ((bool)animator)
				{
					RegisterAnimationTrigger();
					animator.TryPlay(currentAnimGroup.SingAnim);
				}
				if (hasUnityAnimator)
				{
					unityAnimator.Play(currentAnimGroup.SingAnim);
				}
				OnSingStarted?.Invoke();
				if ((bool)needolinTextOwner && !IsForced)
				{
					needolinTextOwner.AddNeedolinText();
				}
				if ((bool)singAudioSource && (bool)singAudioClipTable)
				{
					singAudioSource.clip = singAudioClipTable.SelectClip();
					singAudioSource.pitch = singAudioClipTable.SelectPitch();
					singAudioSource.volume = singAudioClipTable.SelectVolume();
					singAudioSource.Play();
				}
				if ((bool)movementAudioClipTable)
				{
					movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				}
				float singingTime = 0f;
				float singingEndTime = singTimeRange.GetRandomValue();
				while (ShouldSing)
				{
					yield return null;
					singingTime += Time.deltaTime;
					if (singingEndTime > 0f && singingTime >= singingEndTime)
					{
						break;
					}
				}
				OnSingEnding?.Invoke();
				if ((bool)needolinTextOwner)
				{
					needolinTextOwner.RemoveNeedolinText();
				}
				if ((bool)singAudioSource)
				{
					singAudioSource.Stop();
				}
				if ((bool)movementAudioClipTable)
				{
					movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				}
				if ((bool)animator)
				{
					UnregisterAnimationTrigger();
					if (!string.IsNullOrEmpty(currentAnimGroup.ReturnToIdleAnim))
					{
						yield return StartCoroutine(animator.PlayAnimWait(currentAnimGroup.ReturnToIdleAnim));
					}
					animator.TryPlay(currentAnimGroup.IdleAnim);
				}
				if (hasUnityAnimator)
				{
					unityAnimator.Play(currentAnimGroup.ReturnToIdleAnim);
					yield return new WaitForEndOfFrame();
					AnimatorStateInfo currentAnimatorStateInfo = unityAnimator.GetCurrentAnimatorStateInfo(0);
					if (currentAnimatorStateInfo.IsName(currentAnimGroup.ReturnToIdleAnim))
					{
						yield return new WaitForSeconds(currentAnimatorStateInfo.length);
					}
					unityAnimator.Play(currentAnimGroup.IdleAnim);
				}
				OnSingEnded?.Invoke();
				float idleTime = 0f;
				float idleEndTime = restTimeRange.GetRandomValue();
				while (ShouldSing)
				{
					yield return null;
					idleTime += Time.deltaTime;
					if (idleEndTime > 0f && idleTime >= idleEndTime)
					{
						goto IL_00d8;
					}
				}
				break;
			}
			ActivateLookAnimNpc();
			yield return new WaitForSeconds(0.5f);
			ToggleNpcControl(active: true);
		}
	}

	private void ToggleNpcControl(bool active)
	{
		disabledNpcControl = active;
		if ((bool)npcControl)
		{
			if (active)
			{
				npcControl.Activate();
			}
			else
			{
				npcControl.Deactivate(allowQueueing: false);
			}
		}
	}

	private void ActivateLookAnimNpc()
	{
		if (!lookAnimNPC)
		{
			return;
		}
		switch (lookAnimNPCActivate)
		{
		case LookAnimNpcActivateBehaviours.Any:
			if (leftAnims.IdleAnim != rightAnims.IdleAnim)
			{
				lookAnimNPC.Activate(lookAnimNPC.WasFacingLeft);
			}
			else
			{
				lookAnimNPC.Activate();
			}
			disabledNpcControl = false;
			break;
		case LookAnimNpcActivateBehaviours.FaceLeft:
			lookAnimNPC.Activate(facingLeft: true);
			lookAnimNPC.ResetRestTimer();
			lookAnimNPC.ClearTurnDelaySkip();
			disabledNpcControl = false;
			break;
		case LookAnimNpcActivateBehaviours.FaceRight:
			lookAnimNPC.Activate(facingLeft: false);
			lookAnimNPC.ResetRestTimer();
			lookAnimNPC.ClearTurnDelaySkip();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator currentAnimator, tk2dSpriteAnimationClip currentClip, int currentFrame)
	{
		if (currentClip.name == leftAnims.SingAnim || currentClip.name == rightAnims.SingAnim)
		{
			OnSingTrigger.Invoke();
		}
	}

	private void OnNoiseCreated(Vector2 _, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffScreen)
	{
		if (intensity >= minNoiseIntensity && isNoiseInRange(base.transform.position) && startleRoutine == null && (!npcControl || !(InteractManager.BlockingInteractable == npcControl)) && !(Time.timeAsDouble < nextRespondTime) && !IsForcedAny)
		{
			nextRespondTime = Time.timeAsDouble + (double)noiseRespondCooldown;
			StopBehaviour();
			startleRoutine = StartCoroutine(Startle());
		}
	}

	private AnimGroup GetCurrentAnimGroup()
	{
		switch (lookAnimNPCActivate)
		{
		case LookAnimNpcActivateBehaviours.FaceLeft:
			return leftAnims;
		case LookAnimNpcActivateBehaviours.FaceRight:
			return rightAnims;
		default:
			if ((bool)lookAnimNPC)
			{
				if (!lookAnimNPC.WasFacingLeft)
				{
					return rightAnims;
				}
				return leftAnims;
			}
			if (lookAnimNPCActivate != LookAnimNpcActivateBehaviours.FaceRight)
			{
				return leftAnims;
			}
			return rightAnims;
		}
	}

	private IEnumerator Startle()
	{
		ToggleNpcControl(active: false);
		if ((bool)lookAnimNPC)
		{
			lookAnimNPC.Deactivate();
		}
		float randomValue = noiseRespondDelay.GetRandomValue();
		if (randomValue > 0f)
		{
			yield return new WaitForSeconds(randomValue);
		}
		OnStartleStarted.Invoke();
		if ((bool)movementAudioClipTable)
		{
			movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		}
		if ((bool)startleAudioClipTable)
		{
			if ((bool)singAudioSource)
			{
				singAudioSource.Stop();
			}
			if (startleUsesSingAudioSource && (bool)singAudioSource)
			{
				startleAudioClipTable.PlayOneShot(singAudioSource);
			}
			else
			{
				startleAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
			}
		}
		AnimGroup currentAnimGroup = GetCurrentAnimGroup();
		bool playStartleAnim = !string.IsNullOrEmpty(currentAnimGroup.NoiseStartleAnim);
		if (playStartleAnim)
		{
			tk2dSpriteAnimationClip clipByName = animator.GetClipByName(currentAnimGroup.NoiseStartleAnim);
			if (clipByName != null)
			{
				yield return new WaitForSeconds(animator.PlayAnimGetTime(clipByName));
			}
			if (hasUnityAnimator)
			{
				unityAnimator.Play(currentAnimGroup.NoiseStartleAnim);
				yield return new WaitForEndOfFrame();
				AnimatorStateInfo currentAnimatorStateInfo = unityAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(currentAnimGroup.NoiseStartleAnim))
				{
					yield return new WaitForSeconds(currentAnimatorStateInfo.length);
				}
			}
			if (!startleAnimEndsItself)
			{
				float randomValue2 = startleLoopTime.GetRandomValue();
				if (randomValue2 > Mathf.Epsilon)
				{
					yield return new WaitForSeconds(randomValue2);
				}
				clipByName = animator.GetClipByName(currentAnimGroup.ReturnToIdleAnim);
				if (clipByName != null)
				{
					yield return new WaitForSeconds(animator.PlayAnimGetTime(clipByName));
				}
				if (hasUnityAnimator)
				{
					unityAnimator.Play(currentAnimGroup.ReturnToIdleAnim);
					yield return new WaitForEndOfFrame();
					AnimatorStateInfo currentAnimatorStateInfo2 = unityAnimator.GetCurrentAnimatorStateInfo(0);
					if (currentAnimatorStateInfo2.IsName(currentAnimGroup.ReturnToIdleAnim))
					{
						yield return new WaitForSeconds(currentAnimatorStateInfo2.length);
					}
				}
			}
		}
		OnStartleEnded.Invoke();
		if (playStartleAnim)
		{
			ActivateLookAnimNpc();
		}
		else if ((bool)lookAnimNPC)
		{
			if (lookAnimNPC.State == LookAnimNPC.AnimState.Disabled)
			{
				ActivateLookAnimNpc();
			}
			else if (lookAnimNPC.State != LookAnimNPC.AnimState.Resting)
			{
				lookAnimNPC.ResetRestTimer();
			}
		}
		ToggleNpcControl(active: true);
		startleRoutine = null;
		StartBehaviour();
	}
}
