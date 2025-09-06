using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public abstract class LookAnimNPC : MonoBehaviour
{
	[Serializable]
	private struct TalkAnims
	{
		public string Enter;

		public string Talk;

		public string Listen;

		public string Exit;
	}

	public enum AnimState
	{
		Left = 0,
		TurningLeft = 1,
		Right = 2,
		TurningRight = 3,
		Talking = 4,
		Resting = 5,
		Disabled = 6
	}

	protected enum TurnFlipTypes
	{
		NoFlip = 0,
		AfterTurn = 1,
		BeforeTurn = 2
	}

	[Tooltip("Automatically remove behaviour on Start if further than this from the hero's Z. Leave 0 for no limit.")]
	[SerializeField]
	private float limitZ;

	[Space]
	[SerializeField]
	private string leftAnim;

	[SerializeField]
	private string rightAnim;

	[SerializeField]
	private string turnLeftAnim;

	[SerializeField]
	private string turnRightAnim;

	[SerializeField]
	protected TurnFlipTypes turnFlipType;

	[SerializeField]
	private RandomAudioClipTable turnAudioClipTable;

	[SerializeField]
	private bool playTurnAudioOnTalkStart;

	[SerializeField]
	protected bool defaultLeft = true;

	[SerializeField]
	private bool facingIgnoresScale;

	[Space]
	[SerializeField]
	private float centreOffset;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("DoesIdleTurn", true, true, false)]
	private TriggerEnterEvent enterDetector;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("DoesIdleTurn", true, true, false)]
	private TriggerEnterEvent exitDetector;

	[SerializeField]
	private MinMaxFloat exitDelay;

	[SerializeField]
	private bool turnOnInteract;

	[Space]
	[SerializeField]
	private float minReactDelay = 0.3f;

	[SerializeField]
	private float maxReactDelay = 0.5f;

	[SerializeField]
	private bool waitForHeroInPosition;

	[Space]
	public UnityEvent OnStartTurn;

	[Header("Talking")]
	[SerializeField]
	private NPCControlBase npcControl;

	[SerializeField]
	private string talkingPageEvent;

	[SerializeField]
	private TalkAnims talkLeftAnims;

	[SerializeField]
	private TalkAnims talkRightAnims;

	[Space]
	[SerializeField]
	private NPCControlBase[] extraConvoTrackers;

	[Header("Resting")]
	[SerializeField]
	private float restEnterTime = 3f;

	[Space]
	[SerializeField]
	private PlayMakerFSM restAnimFSM;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmBoolValid")]
	private string restingFSMBool = "Resting";

	[SerializeField]
	private bool debugMe;

	private bool disabled;

	private float restTimer;

	private int startingConversationCount;

	private int startedConversationCount;

	private bool isCurrentLineNpc;

	private bool isTurning;

	private string waitingForTurnStart;

	private Vector2 previousTargetPosition;

	private bool preventMoveAttention;

	private bool forceWake;

	private bool isSpriteFlipped;

	private bool faceLeft;

	protected float turnDelay;

	private bool skipNextDelay;

	private bool lastTalkedLeft;

	private bool waitingForHero;

	private double turnAudioSuppressor;

	private Coroutine talkRoutine;

	private NoiseResponder noiseResponder;

	private Transform target;

	private double keepTargetUntil;

	private AnimState state;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkLeftAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkRightAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool flipAfterTurn;

	private bool justFinishedTurning;

	private bool isTryingToRest;

	private bool wasFlippedOnDeactivate;

	private bool isForcedDirection;

	private bool forcedLeft;

	private bool waitingForBench;

	protected bool DefaultLeft
	{
		get
		{
			if (facingIgnoresScale && base.transform.lossyScale.x < 0f)
			{
				return !defaultLeft;
			}
			return defaultLeft;
		}
	}

	public Transform TargetOverride { get; set; }

	public Transform CurrentTarget
	{
		get
		{
			if ((bool)TargetOverride)
			{
				return TargetOverride;
			}
			if ((bool)target)
			{
				return target;
			}
			if (turnOnInteract && startedConversationCount > 0)
			{
				return HeroController.instance.transform;
			}
			return null;
		}
	}

	public AnimState State
	{
		get
		{
			return state;
		}
		protected set
		{
			PreviousState = state;
			state = value;
		}
	}

	public AnimState PreviousState { get; private set; }

	public bool WasFacingLeft { get; private set; }

	public bool IsNPCInConversation => startingConversationCount > 0;

	public bool IsNPCTalking
	{
		get
		{
			if (IsNPCInConversation)
			{
				return isCurrentLineNpc;
			}
			return false;
		}
	}

	public bool IsTurning
	{
		get
		{
			if (state != AnimState.TurningLeft)
			{
				return state == AnimState.TurningRight;
			}
			return true;
		}
	}

	public int CurrentLineNumber { get; private set; }

	public bool ForceShouldTurnChecking { get; set; }

	private bool? IsFsmBoolValid(string boolName)
	{
		return restAnimFSM.IsVariableValid(boolName, isRequired: true);
	}

	private bool DoesIdleTurn()
	{
		if (!string.IsNullOrEmpty(leftAnim) || !string.IsNullOrEmpty(turnLeftAnim))
		{
			if (string.IsNullOrEmpty(rightAnim))
			{
				return !string.IsNullOrEmpty(turnRightAnim);
			}
			return true;
		}
		return false;
	}

	protected virtual void OnValidate()
	{
		if (!string.IsNullOrEmpty(talkLeftAnim))
		{
			talkLeftAnims = new TalkAnims
			{
				Talk = talkLeftAnim,
				Listen = leftAnim
			};
			talkLeftAnim = null;
		}
		if (!string.IsNullOrEmpty(talkRightAnim))
		{
			talkRightAnims = new TalkAnims
			{
				Talk = talkRightAnim,
				Listen = rightAnim
			};
			talkRightAnim = null;
		}
		if (flipAfterTurn)
		{
			turnFlipType = TurnFlipTypes.AfterTurn;
			flipAfterTurn = false;
		}
	}

	protected virtual void Awake()
	{
		OnValidate();
		noiseResponder = GetComponent<NoiseResponder>() ?? base.gameObject.AddComponent<NoiseResponder>();
	}

	private void OnEnable()
	{
		noiseResponder.NoiseStarted += OnNoiseStarted;
		if (isTryingToRest)
		{
			if (talkRoutine != null)
			{
				StopCoroutine(talkRoutine);
				talkRoutine = null;
			}
			talkRoutine = StartCoroutine(Rest());
		}
	}

	private void OnDisable()
	{
		noiseResponder.NoiseStarted -= OnNoiseStarted;
		talkRoutine = null;
	}

	private void Start()
	{
		if (limitZ > 0f && Mathf.Abs(base.transform.position.z - 0.004f) > limitZ)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		faceLeft = DefaultLeft;
		WasFacingLeft = GetWasFacingLeft();
		lastTalkedLeft = WasFacingLeft;
		if (State != AnimState.Disabled)
		{
			if (WasFacingLeft)
			{
				if (!string.IsNullOrEmpty(leftAnim))
				{
					PlayAnim(leftAnim);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(rightAnim))
				{
					PlayAnim(rightAnim);
				}
				State = AnimState.Right;
			}
		}
		bool flag = false;
		if (DoesIdleTurn())
		{
			if ((bool)enterDetector)
			{
				flag = true;
				enterDetector.OnTriggerEntered += delegate(Collider2D col, GameObject _)
				{
					TargetEntered(col.gameObject);
				};
			}
			TriggerEnterEvent triggerEnterEvent = (exitDetector ? exitDetector : enterDetector);
			if ((bool)triggerEnterEvent)
			{
				flag = true;
				triggerEnterEvent.OnTriggerExited += delegate
				{
					StartExitDelay();
				};
				triggerEnterEvent.DelayUpdateGrounded = true;
			}
		}
		if (!flag && !turnOnInteract)
		{
			target = HeroController.instance.transform;
		}
		List<NPCControlBase> list = new List<NPCControlBase>(extraConvoTrackers.Length + 1);
		if ((bool)npcControl)
		{
			list.Add(npcControl);
		}
		NPCControlBase[] array = extraConvoTrackers;
		foreach (NPCControlBase nPCControlBase in array)
		{
			if ((bool)nPCControlBase)
			{
				list.Add(nPCControlBase);
			}
		}
		foreach (NPCControlBase item in list)
		{
			if (!item)
			{
				continue;
			}
			item.StartingDialogue += delegate
			{
				startingConversationCount++;
				if (startingConversationCount == 1)
				{
					CurrentLineNumber = 0;
				}
			};
			item.StartedDialogue += delegate
			{
				isCurrentLineNpc = false;
				preventMoveAttention = false;
				startedConversationCount++;
				if (State != AnimState.Disabled && base.isActiveAndEnabled && State != AnimState.Resting && !turnOnInteract)
				{
					StartTalk();
				}
			};
			item.EndedDialogue += delegate
			{
				if (turnOnInteract)
				{
					StartExitDelay();
				}
				startingConversationCount--;
				startedConversationCount--;
			};
			item.StartedNewLine += delegate(DialogueBox.DialogueLine line)
			{
				isCurrentLineNpc = line.IsNpcEvent(talkingPageEvent);
				CurrentLineNumber++;
			};
		}
		if (State == AnimState.Disabled)
		{
			return;
		}
		if ((bool)restAnimFSM)
		{
			ResetRestTimer();
			isTryingToRest = true;
			talkRoutine = StartCoroutine(Rest());
		}
		else
		{
			bool flag2 = DefaultLeft;
			if (base.transform.lossyScale.x < 0f)
			{
				flag2 = !flag2;
			}
			State = ((!flag2) ? AnimState.Right : AnimState.Left);
			PlayAnim(flag2 ? leftAnim : rightAnim);
			bool flag4 = (WasFacingLeft = flag2);
			faceLeft = flag4;
		}
		HeroController hc;
		if (waitForHeroInPosition)
		{
			hc = HeroController.instance;
			if (hc != null && !hc.isHeroInPosition)
			{
				hc.heroInPosition += OnHeroInPosition;
				waitingForHero = true;
			}
		}
		void OnHeroInPosition(bool forcedirect)
		{
			hc.heroInPosition -= OnHeroInPosition;
			waitingForHero = false;
			waitingForBench = true;
			bool flag5 = ShouldFaceLeft(isTalking: false);
			if (faceLeft != flag5)
			{
				skipNextDelay = true;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? z = 0f;
		Gizmos.DrawWireSphere(position.Where(null, null, z) + new Vector3(centreOffset, 0f, 0f), 0.25f);
	}

	private void Update()
	{
		if (waitingForHero)
		{
			return;
		}
		if (waitingForBench)
		{
			if (PlayerData.instance.atBench)
			{
				return;
			}
			waitingForBench = false;
			skipNextDelay = true;
		}
		if (disabled)
		{
			return;
		}
		if ((bool)target && keepTargetUntil > (double)Mathf.Epsilon && Time.timeAsDouble >= keepTargetUntil)
		{
			target = null;
		}
		if (State < AnimState.Talking)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (!isTurning && !justFinishedTurning)
			{
				WasFacingLeft = GetWasFacingLeft();
				faceLeft = ShouldFaceLeft(isTalking: false);
				bool flag4 = faceLeft;
				if (WasFacingLeft != faceLeft)
				{
					if (skipNextDelay)
					{
						skipNextDelay = false;
					}
					else if (turnDelay <= 0f)
					{
						turnDelay = UnityEngine.Random.Range(minReactDelay, maxReactDelay);
					}
				}
				if (turnDelay > 0f)
				{
					turnDelay -= Time.deltaTime;
					if (turnDelay > 0f)
					{
						flag4 = !flag4;
					}
				}
				switch (State)
				{
				case AnimState.Left:
					if (!flag4)
					{
						State = AnimState.TurningRight;
						isTurning = true;
						if (!string.IsNullOrEmpty(turnRightAnim))
						{
							PlayAnim(turnRightAnim);
							waitingForTurnStart = turnRightAnim;
							flag2 = true;
						}
						else
						{
							flag3 = true;
						}
					}
					flag = true;
					break;
				case AnimState.Right:
					if (flag4)
					{
						State = AnimState.TurningLeft;
						isTurning = true;
						if (!string.IsNullOrEmpty(turnLeftAnim))
						{
							PlayAnim(turnLeftAnim);
							waitingForTurnStart = turnLeftAnim;
							flag2 = true;
						}
						else
						{
							flag3 = true;
						}
					}
					flag = true;
					break;
				}
				if (isTurning)
				{
					if (turnFlipType == TurnFlipTypes.BeforeTurn)
					{
						DoFlip();
					}
					DidTurn();
				}
			}
			else
			{
				justFinishedTurning = false;
			}
			bool flag5 = false;
			if (!flag2 && !string.IsNullOrEmpty(waitingForTurnStart) && !IsAnimPlaying(waitingForTurnStart))
			{
				flag5 = true;
				waitingForTurnStart = null;
			}
			if ((isTurning && flag5) || flag3)
			{
				isTurning = false;
				turnDelay = 0f;
				switch (State)
				{
				case AnimState.TurningLeft:
					State = AnimState.Left;
					if (!string.IsNullOrEmpty(leftAnim))
					{
						PlayAnim(leftAnim);
					}
					break;
				case AnimState.TurningRight:
					State = AnimState.Right;
					if (!string.IsNullOrEmpty(rightAnim))
					{
						PlayAnim(rightAnim);
					}
					break;
				case AnimState.Left:
					if (!string.IsNullOrEmpty(leftAnim))
					{
						PlayAnim(leftAnim);
					}
					break;
				case AnimState.Right:
					if (!string.IsNullOrEmpty(rightAnim))
					{
						PlayAnim(rightAnim);
					}
					break;
				}
				if (turnFlipType == TurnFlipTypes.AfterTurn)
				{
					DoFlip();
				}
				EnsureCorrectFacing();
			}
			else if (flag && (bool)restAnimFSM)
			{
				if (HasAttention())
				{
					ResetRestTimer();
				}
				else
				{
					restTimer -= Time.deltaTime;
					if (restTimer <= 0f)
					{
						preventMoveAttention = true;
						StartCoroutine(Rest());
					}
				}
			}
			if (!isTurning && turnDelay <= 0f && talkRoutine == null && startedConversationCount > 0)
			{
				StartTalk();
			}
			Transform currentTarget = CurrentTarget;
			if (currentTarget != null)
			{
				previousTargetPosition = currentTarget.position;
			}
		}
		else
		{
			ResetRestTimer();
		}
	}

	public void SuppressTurnAudio(float duration)
	{
		turnAudioSuppressor = Time.timeAsDouble + (double)duration;
	}

	public void ResetRestTimer()
	{
		restTimer = restEnterTime;
	}

	public void ClearTurnDelaySkip()
	{
		skipNextDelay = false;
	}

	private void StartExitDelay()
	{
		keepTargetUntil = Time.timeAsDouble + (double)exitDelay.GetRandomValue();
		if (Time.timeAsDouble >= keepTargetUntil)
		{
			target = null;
		}
	}

	public void TargetEntered(GameObject obj)
	{
		target = obj.transform;
		keepTargetUntil = 0.0;
		preventMoveAttention = false;
	}

	public bool ShouldFaceLeft()
	{
		return ShouldFaceLeft(isTalking: false);
	}

	protected virtual float GetXScale()
	{
		return base.transform.lossyScale.x;
	}

	public bool ShouldFaceLeft(bool isTalking)
	{
		if (!ForceShouldTurnChecking && !DoesIdleTurn() && !isTalking)
		{
			if (base.transform.lossyScale.x < 0f)
			{
				return !DefaultLeft;
			}
			return DefaultLeft;
		}
		Transform currentTarget = CurrentTarget;
		if (!currentTarget)
		{
			if (turnOnInteract)
			{
				return lastTalkedLeft;
			}
			if (base.transform.lossyScale.x < 0f)
			{
				return !DefaultLeft;
			}
			return DefaultLeft;
		}
		Transform transform = base.transform;
		float num = ((turnFlipType == TurnFlipTypes.NoFlip) ? base.transform.lossyScale.x : 1f);
		float x = currentTarget.transform.position.x;
		float num2 = transform.position.x + centreOffset;
		float num3 = x - num2;
		if (isForcedDirection)
		{
			num3 = ((!forcedLeft) ? 1f : (-1f));
		}
		return num3 * num < 0f;
	}

	public bool ForceTurn(bool left)
	{
		isForcedDirection = true;
		forcedLeft = left;
		bool flag = ShouldFaceLeft();
		bool wasFacingLeft = GetWasFacingLeft();
		if (!isTurning && flag != wasFacingLeft)
		{
			skipNextDelay = true;
			isTurning = true;
			StartTurning(wasFacingLeft, flag);
		}
		return isTurning;
	}

	public bool UnlockTurn()
	{
		isForcedDirection = false;
		bool flag = ShouldFaceLeft();
		bool wasFacingLeft = GetWasFacingLeft();
		if (!isTurning && flag != wasFacingLeft)
		{
			skipNextDelay = true;
			isTurning = true;
			StartTurning(wasFacingLeft, flag);
		}
		return isTurning;
	}

	private void StartTurning(bool wasFacingLeft, bool lookLeft)
	{
		WasFacingLeft = wasFacingLeft;
		faceLeft = lookLeft;
		switch (State)
		{
		case AnimState.Left:
			if (!lookLeft)
			{
				State = AnimState.TurningRight;
				isTurning = true;
				if (!string.IsNullOrEmpty(turnRightAnim))
				{
					PlayAnim(turnRightAnim);
					waitingForTurnStart = turnRightAnim;
				}
			}
			break;
		case AnimState.Right:
			if (lookLeft)
			{
				State = AnimState.TurningLeft;
				isTurning = true;
				if (!string.IsNullOrEmpty(turnLeftAnim))
				{
					PlayAnim(turnLeftAnim);
					waitingForTurnStart = turnLeftAnim;
				}
			}
			break;
		}
		if (isTurning)
		{
			if (turnFlipType == TurnFlipTypes.BeforeTurn)
			{
				DoFlip();
			}
			DidTurn();
		}
	}

	private void DidTurn()
	{
		if (Time.timeAsDouble >= turnAudioSuppressor)
		{
			turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		}
		OnStartTurn?.Invoke();
	}

	public bool HasAttention()
	{
		if (startingConversationCount > 0)
		{
			return true;
		}
		if (preventMoveAttention)
		{
			return false;
		}
		Transform currentTarget = CurrentTarget;
		if (!currentTarget)
		{
			return false;
		}
		if (restEnterTime <= 0f)
		{
			return true;
		}
		return ((Vector2)currentTarget.position - previousTargetPosition).magnitude > 0.001f;
	}

	private IEnumerator Rest()
	{
		AnimState previousState = State;
		State = AnimState.Resting;
		isTryingToRest = true;
		HeroController hc = HeroController.instance;
		if (hc != null && !hc.isHeroInPosition)
		{
			while (!hc.isHeroInPosition)
			{
				yield return null;
			}
		}
		bool didRest = false;
		if (waitingForBench)
		{
			if ((bool)restAnimFSM)
			{
				restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: true);
				didRest = true;
			}
			while (waitingForBench)
			{
				yield return null;
			}
		}
		bool flag = HasAttention();
		if (!flag && !forceWake)
		{
			if ((bool)restAnimFSM)
			{
				restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: true);
				didRest = true;
			}
			while (!flag && !forceWake)
			{
				yield return null;
				flag = HasAttention();
			}
		}
		if (!forceWake)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(minReactDelay, maxReactDelay));
		}
		forceWake = false;
		if (!didRest)
		{
			State = previousState;
		}
		if ((bool)restAnimFSM)
		{
			if (didRest && (startingConversationCount > 0 || startedConversationCount > 0))
			{
				WasFacingLeft = (faceLeft = ShouldFaceLeft(isTalking: true));
			}
			restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: false);
		}
		isTryingToRest = false;
	}

	public void ClearRestBehaviour()
	{
		restAnimFSM = null;
	}

	private void OnNoiseStarted()
	{
		if (State == AnimState.Resting)
		{
			ForceWake();
		}
	}

	public void ForceWake()
	{
		forceWake = true;
	}

	public void SetForceWake(bool value)
	{
		forceWake = value;
	}

	public void EndRest(bool facingLeft)
	{
		EndRestInternal(facingLeft);
	}

	public void EndRest()
	{
		EndRestInternal(null);
	}

	private void EndRestInternal(bool? facingLeft)
	{
		bool flag = DefaultLeft;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		if (startingConversationCount > 0)
		{
			if (facingLeft.HasValue)
			{
				PlayIdle(facingLeft.Value);
			}
			bool num = facingLeft ?? flag;
			bool wasFacingLeft = num;
			faceLeft = num;
			WasFacingLeft = wasFacingLeft;
			if (target == null)
			{
				target = HeroController.instance.transform;
				keepTargetUntil = 0.0;
			}
			StartTalk();
		}
		else
		{
			AnimState num2 = State;
			ResetState(facingLeft ?? flag);
			if (num2 == AnimState.Disabled)
			{
				State = AnimState.Disabled;
			}
		}
	}

	public void ResetState(bool facingLeft)
	{
		State = ((!facingLeft) ? AnimState.Right : AnimState.Left);
		WasFacingLeft = (faceLeft = facingLeft);
		skipNextDelay = true;
		turnDelay = 0f;
		bool flag = DefaultLeft;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		PlayIdle(DoesIdleTurn() ? facingLeft : flag);
	}

	private void PlayIdle(bool facingLeft)
	{
		string text = (facingLeft ? leftAnim : rightAnim);
		if (!string.IsNullOrEmpty(text))
		{
			PlayAnim(text);
		}
	}

	[ContextMenu("Activate")]
	public void Activate()
	{
		bool flag = GetIsSpriteFlipped();
		Activate(flag ? (!DefaultLeft) : DefaultLeft);
	}

	public void Activate(bool facingLeft)
	{
		if (wasFlippedOnDeactivate)
		{
			wasFlippedOnDeactivate = false;
			FlipSprite();
			base.transform.FlipLocalScale(x: true);
			isSpriteFlipped = true;
		}
		bool num = State == AnimState.Resting;
		ResetState(facingLeft);
		if (num && (bool)restAnimFSM)
		{
			restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: false);
			restAnimFSM.SendEvent("DEACTIVATED");
		}
	}

	public void Enable()
	{
		disabled = false;
	}

	public void Disable()
	{
		disabled = true;
	}

	[ContextMenu("Deactivate")]
	public void Deactivate()
	{
		State = AnimState.Disabled;
		if ((bool)restAnimFSM)
		{
			restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: false);
			restAnimFSM.SendEvent("DEACTIVATED");
		}
		if (talkRoutine != null)
		{
			StopCoroutine(talkRoutine);
			talkRoutine = null;
		}
		if (turnFlipType != 0 && isSpriteFlipped)
		{
			FlipSprite();
			isSpriteFlipped = false;
			base.transform.FlipLocalScale(x: true);
			wasFlippedOnDeactivate = true;
		}
	}

	public void DeactivateInstant()
	{
		State = AnimState.Disabled;
		if ((bool)restAnimFSM)
		{
			restAnimFSM.SetFsmBoolIfExists(restingFSMBool, value: false);
			restAnimFSM.SendEvent("DEACTIVATED_INSTANT");
			EndRest();
		}
		if (talkRoutine != null)
		{
			StopCoroutine(talkRoutine);
			talkRoutine = null;
		}
		if (turnFlipType != 0 && isSpriteFlipped)
		{
			FlipSprite();
			isSpriteFlipped = false;
			base.transform.FlipLocalScale(x: true);
			wasFlippedOnDeactivate = true;
		}
	}

	private void StartTalk()
	{
		if (talkRoutine != null)
		{
			StopCoroutine(talkRoutine);
			talkRoutine = null;
		}
		talkRoutine = StartCoroutine(Talk());
	}

	private IEnumerator Talk()
	{
		while (turnDelay > 0f || IsTurning)
		{
			yield return null;
		}
		State = AnimState.Talking;
		TalkAnims anims;
		while (true)
		{
			bool shouldFaceLeft = ShouldFaceLeft(isTalking: true);
			bool wasFacingLeft = GetWasFacingLeft();
			if (shouldFaceLeft != wasFacingLeft)
			{
				string turnAnim = (shouldFaceLeft ? turnLeftAnim : turnRightAnim);
				if (!string.IsNullOrEmpty(turnAnim))
				{
					PlayAnim(turnAnim);
					if (turnFlipType == TurnFlipTypes.BeforeTurn)
					{
						DoFlip();
					}
					DidTurn();
					yield return new WaitUntil(() => IsAnimPlaying(turnAnim));
					yield return new WaitUntil(() => !IsAnimPlaying(turnAnim));
					if (turnFlipType == TurnFlipTypes.AfterTurn)
					{
						DoFlip();
					}
				}
				WasFacingLeft = (faceLeft = shouldFaceLeft);
			}
			PlayIdle(faceLeft);
			while (true)
			{
				if (startedConversationCount == 0)
				{
					yield return null;
					if (startingConversationCount != 0)
					{
						continue;
					}
				}
				else
				{
					if (ShouldFaceLeft(isTalking: true) != shouldFaceLeft)
					{
						break;
					}
					anims = (shouldFaceLeft ? talkLeftAnims : talkRightAnims);
					if (!string.IsNullOrEmpty(anims.Enter))
					{
						PlayAnim(anims.Enter);
						yield return new WaitUntil(() => IsAnimPlaying(anims.Enter));
						yield return new WaitUntil(() => !IsAnimPlaying(anims.Enter));
					}
					if (playTurnAudioOnTalkStart && (bool)turnAudioClipTable)
					{
						turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
					}
					bool wasNpcTalking = !isCurrentLineNpc;
					do
					{
						if (isCurrentLineNpc)
						{
							if (!wasNpcTalking && !string.IsNullOrEmpty(anims.Talk))
							{
								PlayAnim(anims.Talk);
							}
						}
						else if (wasNpcTalking && !string.IsNullOrEmpty(anims.Listen))
						{
							PlayAnim(anims.Listen);
						}
						wasNpcTalking = isCurrentLineNpc;
						yield return null;
					}
					while (startingConversationCount > 0);
					if (!string.IsNullOrEmpty(anims.Exit))
					{
						PlayAnim(anims.Exit);
						yield return new WaitUntil(() => IsAnimPlaying(anims.Exit));
						yield return new WaitUntil(() => !IsAnimPlaying(anims.Exit));
					}
					if (playTurnAudioOnTalkStart && (bool)turnAudioClipTable)
					{
						turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
					}
				}
				lastTalkedLeft = faceLeft;
				talkRoutine = null;
				if (State != AnimState.Disabled)
				{
					ResetState(faceLeft);
				}
				yield break;
			}
		}
	}

	[UsedImplicitly]
	public bool IsFacingLeft()
	{
		AnimState animState = State;
		if ((uint)animState <= 1u)
		{
			return true;
		}
		return false;
	}

	[UsedImplicitly]
	public bool IsFacingLeftScaled()
	{
		bool flag = base.transform.lossyScale.x < 0f;
		AnimState animState = State;
		if ((uint)animState <= 1u)
		{
			return !flag;
		}
		return flag;
	}

	private void DoFlip()
	{
		FlipSprite();
		isSpriteFlipped = !isSpriteFlipped;
	}

	protected abstract void PlayAnim(string animName);

	protected abstract bool IsAnimPlaying(string animName);

	protected abstract void FlipSprite();

	protected abstract bool GetIsSpriteFlipped();

	protected virtual bool GetWasFacingLeft()
	{
		return faceLeft;
	}

	protected virtual void EnsureCorrectFacing()
	{
	}

	public void SetDefaultFacingLeft(bool set)
	{
		defaultLeft = set;
	}

	public void FaceTargetInstant()
	{
		if (isTurning)
		{
			return;
		}
		WasFacingLeft = GetWasFacingLeft();
		faceLeft = ShouldFaceLeft(isTalking: false);
		if (WasFacingLeft == faceLeft)
		{
			return;
		}
		bool flag = faceLeft;
		switch (State)
		{
		case AnimState.Left:
			if (!flag)
			{
				State = AnimState.TurningRight;
				if (!string.IsNullOrEmpty(turnRightAnim))
				{
					PlayAnim(turnRightAnim);
					waitingForTurnStart = turnRightAnim;
				}
			}
			break;
		case AnimState.Right:
			if (flag)
			{
				State = AnimState.TurningLeft;
				if (!string.IsNullOrEmpty(turnLeftAnim))
				{
					PlayAnim(turnLeftAnim);
					waitingForTurnStart = turnLeftAnim;
				}
			}
			break;
		}
		if (turnFlipType == TurnFlipTypes.BeforeTurn)
		{
			DoFlip();
		}
		DidTurn();
		isTurning = false;
		turnDelay = 0f;
		switch (State)
		{
		case AnimState.TurningLeft:
			State = AnimState.Left;
			if (!string.IsNullOrEmpty(leftAnim))
			{
				PlayAnim(leftAnim);
			}
			break;
		case AnimState.TurningRight:
			State = AnimState.Right;
			if (!string.IsNullOrEmpty(rightAnim))
			{
				PlayAnim(rightAnim);
			}
			break;
		}
		if (turnFlipType == TurnFlipTypes.AfterTurn)
		{
			DoFlip();
		}
		EnsureCorrectFacing();
	}
}
