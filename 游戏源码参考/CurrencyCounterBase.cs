using System;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.UI;

public abstract class CurrencyCounterBase : MonoBehaviour
{
	private enum RollerState
	{
		Down = 0,
		Up = 1,
		Rolling = 2
	}

	public enum StateEvents
	{
		StartTake = 0,
		StartAdd = 1,
		Stopped = 2,
		RollerTicked = 3,
		FadeDelayElapsed = 4
	}

	private enum QueuedAction
	{
		None = 0,
		Add = 1,
		Take = 2,
		Zero = 3,
		Popup = 4
	}

	private const float ROLLER_START_PAUSE = 1f;

	private const float DIGIT_CHANGE_TIME = 0.0125f;

	public Action Appeared;

	public Action Disappeared;

	[Space]
	[SerializeField]
	private CurrencyCounterIcon icon;

	[Space]
	[SerializeField]
	protected TextBridge geoTextMesh;

	[SerializeField]
	private TextBridge subTextMesh;

	private float initialSubTextX;

	[SerializeField]
	private TextBridge addTextMesh;

	private float initialAddTextX;

	[SerializeField]
	private float characterOffset;

	private int initialCounter;

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private NestedFadeGroupBase rollerFade;

	[SerializeField]
	private float fadeInTime = 0.1f;

	[SerializeField]
	private float fadeOutDelay = 5f;

	[SerializeField]
	private float completedFadeOutDelay = 0.5f;

	private Coroutine rollerFadeOut;

	private Coroutine fadeOutRoutine;

	private Action onAfterDelay;

	private Action onTimerEnd;

	[SerializeField]
	private float fadeOutTime = 0.5f;

	[SerializeField]
	private LayoutGroup amountLayoutGroup;

	[SerializeField]
	private AudioSource rollSound;

	[SerializeField]
	private AudioClip rollEndSound;

	[SerializeField]
	private bool skipRoller;

	[Space]
	[SerializeField]
	private Animator failAnimator;

	private int counterCurrent;

	private int geoChange;

	private int addCounter;

	private int takeCounter;

	private RollerState addRollerState;

	private RollerState takeRollerState;

	private int changePerTick;

	private bool isPlayingRollSound;

	private float addRollerStartTimer;

	private float takeRollerStartTimer;

	private float digitChangeTimer;

	private bool toZero;

	private int showCount;

	private bool isActive;

	private bool wantsToHide;

	private bool isVisible;

	private float targetHideTime;

	private CurrencyCounterIcon iconOverride;

	private QueuedAction queuedAction;

	private int queuedValue;

	private static readonly List<CurrencyCounterBase> _allCounters = new List<CurrencyCounterBase>();

	private static readonly int _failedPropId = Animator.StringToHash("Failed");

	private bool isFadingOut;

	private int targetEndCount;

	private static int orderCounter;

	private bool hideQueued;

	private bool IsForcedActive => showCount > 0;

	protected bool IsActive
	{
		get
		{
			return isActive;
		}
		private set
		{
			_ = isActive;
			isActive = value;
			if (value)
			{
				Appeared?.Invoke();
				if (geoChange == 0 && (bool)rollerFade)
				{
					rollerFade.FadeToZero(fadeOutTime);
				}
				isFadingOut = false;
			}
			else
			{
				Disappeared?.Invoke();
			}
		}
	}

	protected bool IsActiveOrQueued
	{
		get
		{
			if (!isActive)
			{
				return queuedAction != QueuedAction.None;
			}
			return true;
		}
	}

	private bool IsRolling
	{
		get
		{
			if (addRollerState != RollerState.Rolling && !(addRollerStartTimer > 0f) && takeRollerState != RollerState.Rolling)
			{
				return takeRollerStartTimer > 0f;
			}
			return true;
		}
	}

	public static bool IsAnyCounterRolling
	{
		get
		{
			foreach (CurrencyCounterBase allCounter in _allCounters)
			{
				if (allCounter.IsRolling)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected CurrencyCounterIcon IconOverride
	{
		get
		{
			return iconOverride;
		}
		set
		{
			iconOverride = value;
			icon.gameObject.SetActive(!iconOverride);
		}
	}

	private CurrencyCounterIcon CurrentIcon
	{
		get
		{
			if (!iconOverride)
			{
				return icon;
			}
			return iconOverride;
		}
	}

	protected abstract int Count { get; }

	public int StackOrder { get; private set; }

	protected virtual void Awake()
	{
		_allCounters.Add(this);
		if ((bool)addTextMesh)
		{
			initialAddTextX = addTextMesh.transform.localPosition.x;
		}
		if ((bool)subTextMesh)
		{
			initialSubTextX = subTextMesh.transform.localPosition.x;
		}
	}

	protected virtual void Start()
	{
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = 0f;
		}
		GameManager instance = GameManager.instance;
		instance.GamePausedChange += OnGamePauseChanged;
		instance.OnBeforeFinishedSceneTransition += OnLevelLoaded;
	}

	private void OnDisable()
	{
		addRollerState = RollerState.Down;
		addRollerStartTimer = 0f;
		takeRollerState = RollerState.Down;
		takeRollerStartTimer = 0f;
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
		}
		onAfterDelay?.Invoke();
		onTimerEnd?.Invoke();
	}

	protected virtual void OnDestroy()
	{
		_allCounters.Remove(this);
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.GamePausedChange -= OnGamePauseChanged;
			silentInstance.OnBeforeFinishedSceneTransition -= OnLevelLoaded;
		}
	}

	public static void FadeInIfActive()
	{
		for (int num = _allCounters.Count - 1; num >= 0; num--)
		{
			_allCounters[num].InternalFadeInIfActive();
		}
		orderCounter = 0;
	}

	public static void HideAllInstant()
	{
		for (int num = _allCounters.Count - 1; num >= 0; num--)
		{
			_allCounters[num].HideInstant();
		}
	}

	private void OnLevelLoaded()
	{
		if (!CheatManager.ForceCurrencyCountersAppear)
		{
			ResetState();
			HideIconInstant();
			if (fadeOutRoutine != null)
			{
				StopCoroutine(fadeOutRoutine);
			}
			if ((bool)fadeGroup)
			{
				fadeGroup.FadeTo(0f, 0f, null, isRealtime: true);
			}
			StopRollSound();
			IsActive = false;
		}
	}

	protected void UpdateCounterStart()
	{
		counterCurrent = Count;
		geoTextMesh.Text = counterCurrent.ToString();
		initialCounter = geoTextMesh.Text.Length;
		if ((bool)addTextMesh)
		{
			addTextMesh.Text = string.Empty;
		}
		if ((bool)subTextMesh)
		{
			subTextMesh.Text = string.Empty;
		}
		ResetState();
		RefreshText(isCountingUp: false);
		UpdateRectLayout();
	}

	private void ResetState()
	{
		if (addCounter > 0 || takeCounter > 0)
		{
			SendStateChangedEvent(StateEvents.Stopped);
		}
		geoChange = 0;
		addCounter = 0;
		takeCounter = 0;
		addRollerStartTimer = 0f;
		addRollerState = RollerState.Down;
		takeRollerStartTimer = 0f;
		takeRollerState = RollerState.Down;
		digitChangeTimer = 0f;
		toZero = false;
	}

	public void UpdateValue()
	{
		int num = Count - counterCurrent;
		if (num > 0)
		{
			InternalAdd(num);
		}
		else if (num < 0)
		{
			InternalTake(-num);
		}
	}

	private void UpdateRectLayout()
	{
		if ((bool)amountLayoutGroup)
		{
			amountLayoutGroup.ForceUpdateLayoutNoCanvas();
		}
	}

	protected virtual void RefreshText(bool isCountingUp)
	{
	}

	private void DoChange()
	{
		UpdateLayout();
		FadeIn();
		IsActive = true;
		if (geoChange == 0)
		{
			RefreshText(isCountingUp: true);
		}
		StopRollSound();
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
		}
		float toAlpha = 1f;
		if ((bool)rollerFade)
		{
			if (geoChange == 0)
			{
				toAlpha = 0f;
				CurrentIcon.GetSingle();
			}
			if (rollerFadeOut != null)
			{
				StopCoroutine(rollerFadeOut);
				rollerFadeOut = null;
			}
			rollerFade.FadeTo(toAlpha, 0f);
		}
		Action<float> timer = null;
		if (geoChange != 0)
		{
			PlayIdle();
		}
		onAfterDelay = delegate
		{
			wantsToHide = true;
			SendStateChangedEvent(StateEvents.FadeDelayElapsed);
			onAfterDelay = null;
		};
		onTimerEnd = delegate
		{
			onTimerEnd = null;
		};
		float y = ((geoChange > 0) ? ((float)geoChange / (float)changePerTick * 0.0125f + 0.3f) : 0f);
		targetHideTime = Time.realtimeSinceStartup + MathF.Max(fadeOutDelay, y);
		fadeOutRoutine = this.StartTimerRoutine(fadeOutDelay, fadeOutTime, delegate(float time)
		{
			timer?.Invoke(time);
		}, onAfterDelay, onTimerEnd, isRealtime: true);
	}

	private void Update()
	{
		if (toZero)
		{
			if (digitChangeTimer < 0f)
			{
				if (counterCurrent <= 0)
				{
					return;
				}
				counterCurrent += changePerTick;
				if (counterCurrent <= 0)
				{
					takeRollerState = RollerState.Down;
					counterCurrent = 0;
					toZero = false;
					StopRollSound();
					if (IsForcedActive)
					{
						FadeRollers();
					}
					else
					{
						DelayedHide();
					}
				}
				else if (takeRollerState != RollerState.Rolling)
				{
					takeRollerState = RollerState.Rolling;
					PlayRollSound();
				}
				geoTextMesh.Text = counterCurrent.ToString();
				digitChangeTimer += 0.0125f;
				RefreshText(isCountingUp: false);
				UpdateRectLayout();
			}
			else
			{
				digitChangeTimer -= Time.unscaledDeltaTime;
			}
			return;
		}
		if (addRollerState == RollerState.Up)
		{
			if (addRollerStartTimer > 0f)
			{
				addRollerStartTimer -= Time.unscaledDeltaTime;
			}
			else
			{
				addRollerState = RollerState.Rolling;
				SendStateChangedEvent(StateEvents.StartAdd);
			}
		}
		if (addRollerState == RollerState.Rolling)
		{
			if (addCounter > 0)
			{
				CurrentIcon.Get();
				if (digitChangeTimer < 0f)
				{
					SendStateChangedEvent(StateEvents.RollerTicked);
					addCounter -= changePerTick;
					counterCurrent += changePerTick;
					geoTextMesh.Text = counterCurrent.ToString();
					if (addTextMesh != null)
					{
						addTextMesh.Text = "+ " + addCounter;
					}
					if (addCounter <= 0)
					{
						PlayIdle();
						addCounter = 0;
						if ((bool)addTextMesh)
						{
							addTextMesh.Text = "+ 0";
						}
						addRollerState = RollerState.Down;
						counterCurrent = targetEndCount;
						geoTextMesh.Text = counterCurrent.ToString();
						SendStateChangedEvent(StateEvents.Stopped);
						if (!IsForcedActive)
						{
							wantsToHide = true;
						}
					}
					digitChangeTimer += 0.0125f;
					RefreshText(isCountingUp: true);
					UpdateRectLayout();
				}
				else
				{
					digitChangeTimer -= Time.unscaledDeltaTime;
				}
			}
			else
			{
				PlayIdle();
				addCounter = 0;
				if ((bool)addTextMesh)
				{
					addTextMesh.Text = "+ 0";
				}
				addRollerState = RollerState.Down;
				counterCurrent = targetEndCount;
				geoTextMesh.Text = counterCurrent.ToString();
				SendStateChangedEvent(StateEvents.Stopped);
				if (!IsForcedActive)
				{
					wantsToHide = true;
				}
			}
		}
		if (takeRollerState == RollerState.Up)
		{
			if (takeRollerStartTimer > 0f)
			{
				takeRollerStartTimer -= Time.unscaledDeltaTime;
			}
			else
			{
				takeRollerState = RollerState.Rolling;
				SendStateChangedEvent(StateEvents.StartTake);
			}
		}
		if (takeRollerState == RollerState.Rolling)
		{
			if (takeCounter < 0)
			{
				CurrentIcon.Take();
				if (digitChangeTimer < 0f)
				{
					SendStateChangedEvent(StateEvents.RollerTicked);
					takeCounter -= changePerTick;
					counterCurrent += changePerTick;
					geoTextMesh.Text = counterCurrent.ToString();
					if (subTextMesh != null)
					{
						subTextMesh.Text = "- " + -takeCounter;
					}
					if (takeCounter >= 0)
					{
						PlayIdle();
						takeCounter = 0;
						if ((bool)subTextMesh)
						{
							subTextMesh.Text = "- 0";
						}
						takeRollerState = RollerState.Down;
						counterCurrent = targetEndCount;
						geoTextMesh.Text = counterCurrent.ToString();
						SendStateChangedEvent(StateEvents.Stopped);
						if (!IsForcedActive)
						{
							wantsToHide = true;
						}
					}
					digitChangeTimer += 0.0125f;
					RefreshText(isCountingUp: false);
					UpdateRectLayout();
				}
				else
				{
					digitChangeTimer -= Time.unscaledDeltaTime;
				}
			}
			else
			{
				PlayIdle();
				takeCounter = 0;
				if ((bool)subTextMesh)
				{
					subTextMesh.Text = "- 0";
				}
				takeRollerState = RollerState.Down;
				counterCurrent = targetEndCount;
				geoTextMesh.Text = counterCurrent.ToString();
				SendStateChangedEvent(StateEvents.Stopped);
				if (!IsForcedActive)
				{
					wantsToHide = true;
				}
			}
		}
		if (!toZero && addRollerState == RollerState.Down && takeRollerState == RollerState.Down && wantsToHide && isVisible && !IsForcedActive && Time.realtimeSinceStartup >= targetHideTime && !IsAnyCounterRolling)
		{
			DelayedHide();
		}
	}

	private void LateUpdate()
	{
		switch (queuedAction)
		{
		case QueuedAction.Add:
			InternalAdd(queuedValue);
			queuedAction = QueuedAction.None;
			break;
		case QueuedAction.Take:
			if (queuedValue > 0)
			{
				InternalTake(queuedValue);
			}
			queuedAction = QueuedAction.None;
			break;
		case QueuedAction.Zero:
			if (counterCurrent > 0)
			{
				InternalToZero();
			}
			queuedAction = QueuedAction.None;
			break;
		case QueuedAction.Popup:
			InternalPopup();
			queuedAction = QueuedAction.None;
			break;
		}
		if (!hideQueued)
		{
			return;
		}
		hideQueued = false;
		if (showCount > 0)
		{
			return;
		}
		FadeOut(delegate(bool finished)
		{
			if (finished)
			{
				IsActive = false;
			}
		});
	}

	public float FadeIn()
	{
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
			fadeOutRoutine = null;
		}
		isFadingOut = false;
		ShowIcon();
		if ((bool)fadeGroup)
		{
			return fadeGroup.FadeTo(1f, fadeInTime, null, isRealtime: true);
		}
		return 0f;
	}

	public float FadeOut()
	{
		return FadeOut(null);
	}

	public float FadeOut(Action<bool> callback)
	{
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
			fadeOutRoutine = null;
		}
		isFadingOut = true;
		HideIcon();
		if (fadeGroup != null)
		{
			return fadeGroup.FadeTo(0f, fadeOutTime, null, isRealtime: true, callback);
		}
		FadeRollers(callback);
		return 0f;
	}

	[ContextMenu("Do Hide")]
	private void DelayedHide()
	{
		wantsToHide = false;
		hideQueued = false;
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
		}
		if (CheatManager.ForceCurrencyCountersAppear)
		{
			return;
		}
		fadeOutRoutine = this.StartTimerRoutine(completedFadeOutDelay, 0f, null, delegate
		{
			FadeOut(delegate(bool finished)
			{
				if (finished && showCount <= 0)
				{
					IsActive = false;
				}
			});
			fadeOutRoutine = null;
		}, null, isRealtime: true);
	}

	private void ShowIcon()
	{
		wantsToHide = false;
		hideQueued = false;
		if (!isVisible)
		{
			isVisible = true;
			CurrentIcon.Appear();
		}
	}

	private void HideIcon()
	{
		wantsToHide = false;
		hideQueued = false;
		isVisible = false;
		CurrentIcon.Disappear();
	}

	private void HideIconInstant()
	{
		IsActive = false;
		wantsToHide = false;
		hideQueued = false;
		isVisible = false;
		CurrentIcon.HideInstant();
	}

	public void HideInstant()
	{
		if ((bool)fadeGroup)
		{
			fadeGroup.FadeTo(0f, 0f, null, isRealtime: true);
		}
		HideIconInstant();
	}

	private void InternalFadeInIfActive()
	{
		if (isVisible || isFadingOut)
		{
			return;
		}
		if (!IsActive)
		{
			if (showCount <= 0)
			{
				return;
			}
			IsActive = true;
		}
		FadeIn();
	}

	private void PlayIdle()
	{
		if (isVisible)
		{
			CurrentIcon.Idle();
		}
	}

	private void UpdateLayout()
	{
		int num = counterCurrent.ToString().Length - initialCounter.ToString().Length;
		if ((bool)subTextMesh)
		{
			subTextMesh.transform.SetLocalPositionX(initialSubTextX + (float)num * characterOffset);
		}
		if ((bool)addTextMesh)
		{
			addTextMesh.transform.SetLocalPositionX(initialAddTextX + (float)num * characterOffset);
		}
	}

	protected void QueueAdd(int geo)
	{
		queuedAction = QueuedAction.Add;
		queuedValue = geo;
	}

	protected void QueueTake(int geo)
	{
		queuedAction = QueuedAction.Take;
		queuedValue = geo;
	}

	protected void QueueToValue(int geo)
	{
		int num = geo - counterCurrent;
		if (num > 0)
		{
			queuedAction = QueuedAction.Add;
			queuedValue = num;
		}
		else
		{
			queuedAction = QueuedAction.Take;
			queuedValue = -num;
		}
	}

	protected void QueueToZero()
	{
		queuedAction = QueuedAction.Zero;
		queuedValue = 0;
	}

	protected void QueuePopup()
	{
		queuedAction = QueuedAction.Popup;
		queuedValue = 0;
	}

	protected void InternalAdd(int geo)
	{
		if ((bool)subTextMesh)
		{
			subTextMesh.Text = string.Empty;
		}
		targetEndCount = Count;
		if (takeRollerState > RollerState.Down)
		{
			geoChange = geo;
			addCounter = geoChange;
			takeRollerState = RollerState.Down;
			counterCurrent = Count - addCounter;
			geoTextMesh.Text = counterCurrent.ToString();
		}
		if (skipRoller)
		{
			counterCurrent = Count;
			geoTextMesh.Text = counterCurrent.ToString();
			return;
		}
		if (addRollerState == RollerState.Down)
		{
			geoChange = geo;
			addCounter = geoChange;
			counterCurrent = Count - addCounter;
			if ((bool)addTextMesh)
			{
				addTextMesh.Text = "+ " + addCounter;
			}
			addRollerStartTimer = 1f;
			addRollerState = RollerState.Up;
		}
		else if (addRollerState == RollerState.Up)
		{
			geoChange = Count - counterCurrent;
			addCounter = geoChange;
			if ((bool)addTextMesh)
			{
				addTextMesh.Text = "+ " + addCounter;
			}
			addRollerStartTimer = 1f;
		}
		else if (addRollerState == RollerState.Rolling)
		{
			geoChange = geo;
			addCounter = geoChange;
			counterCurrent = Count - geoChange;
			geoTextMesh.Text = counterCurrent.ToString();
			if ((bool)addTextMesh)
			{
				addTextMesh.Text = "+ " + addCounter;
			}
			addRollerState = RollerState.Up;
			addRollerStartTimer = 1f;
		}
		changePerTick = (int)((double)((float)addCounter * 0.0125f) * 1.75);
		changePerTick = Mathf.Max(1, changePerTick);
		DoChange();
	}

	protected void InternalTake(int geo)
	{
		ResetState();
		targetEndCount = Count;
		if ((bool)addTextMesh)
		{
			addTextMesh.Text = string.Empty;
		}
		if (addRollerState > RollerState.Down)
		{
			geoChange = -geo;
			takeCounter = geoChange;
			addRollerState = RollerState.Down;
			counterCurrent = Count + -takeCounter;
			geoTextMesh.Text = counterCurrent.ToString();
		}
		if (skipRoller)
		{
			counterCurrent = Count;
			geoTextMesh.Text = counterCurrent.ToString();
			return;
		}
		if (takeRollerState == RollerState.Down)
		{
			geoChange = -geo;
			takeCounter = geoChange;
			counterCurrent = Count + geo;
			if (isVisible)
			{
				geoTextMesh.Text = counterCurrent.ToString();
			}
			if ((bool)subTextMesh)
			{
				subTextMesh.Text = "- " + -takeCounter;
			}
			takeRollerStartTimer = 1f;
			takeRollerState = RollerState.Up;
		}
		else if (takeRollerState == RollerState.Up)
		{
			geoChange = -geo;
			takeCounter += geoChange;
			if ((bool)subTextMesh)
			{
				subTextMesh.Text = "- " + -takeCounter;
			}
			takeRollerStartTimer = 1f;
		}
		else if (takeRollerState == RollerState.Rolling)
		{
			geoChange = -geo;
			takeCounter = geoChange;
			counterCurrent = Count;
			geoTextMesh.Text = counterCurrent.ToString();
			if ((bool)subTextMesh)
			{
				subTextMesh.Text = "- " + -takeCounter;
			}
			takeRollerState = RollerState.Up;
			takeRollerStartTimer = 1f;
		}
		changePerTick = (int)((double)((float)takeCounter * 0.0125f) * 1.75);
		changePerTick = Mathf.Min(-1, changePerTick);
		DoChange();
	}

	protected void InternalToZero()
	{
		ResetState();
		if ((bool)addTextMesh)
		{
			addTextMesh.Text = string.Empty;
		}
		if ((bool)subTextMesh)
		{
			subTextMesh.Text = string.Empty;
		}
		if (counterCurrent != 0)
		{
			changePerTick = -(int)((float)counterCurrent * 0.0125f * 1.75f);
			changePerTick = Mathf.Min(-1, changePerTick);
			toZero = true;
		}
		DoChange();
	}

	private void InternalPopup()
	{
		ResetState();
		geoTextMesh.Text = string.Empty;
		if ((bool)addTextMesh)
		{
			addTextMesh.Text = string.Empty;
		}
		if ((bool)subTextMesh)
		{
			subTextMesh.Text = string.Empty;
		}
		changePerTick = 0;
		counterCurrent = 0;
		DoChange();
	}

	protected void InternalShow()
	{
		if (showCount < 0)
		{
			showCount = 0;
		}
		showCount++;
		if (showCount > 1)
		{
			return;
		}
		StackOrder += ++orderCounter;
		if (addRollerState == RollerState.Down && takeRollerState == RollerState.Down)
		{
			if ((bool)addTextMesh)
			{
				addTextMesh.Text = string.Empty;
			}
			if ((bool)subTextMesh)
			{
				subTextMesh.Text = string.Empty;
			}
			FadeIn();
		}
		else
		{
			wantsToHide = false;
			if (!isVisible)
			{
				FadeIn();
			}
		}
		IsActive = true;
	}

	protected void InternalHide(bool forced = false)
	{
		showCount--;
		if (CheatManager.ForceCurrencyCountersAppear)
		{
			return;
		}
		if (forced)
		{
			showCount = 0;
		}
		if (forced || showCount == 0)
		{
			StackOrder = 0;
			if (!forced && IsRolling)
			{
				wantsToHide = true;
			}
			else
			{
				hideQueued = true;
			}
		}
	}

	protected void InternalFail()
	{
		if ((bool)failAnimator)
		{
			failAnimator.SetTrigger(_failedPropId);
		}
	}

	protected virtual void SendStateChangedEvent(StateEvents stateEvent)
	{
		switch (stateEvent)
		{
		case StateEvents.StartTake:
		case StateEvents.StartAdd:
			if (geoChange != 0)
			{
				PlayRollSound();
			}
			break;
		case StateEvents.Stopped:
			StopRollSound();
			if (IsForcedActive)
			{
				FadeRollers();
			}
			break;
		case StateEvents.RollerTicked:
			UpdateLayout();
			break;
		}
	}

	private void FadeRollers(Action<bool> callback = null)
	{
		if ((bool)rollerFade)
		{
			if (rollerFadeOut != null)
			{
				StopCoroutine(rollerFadeOut);
			}
			rollerFadeOut = this.StartTimerRoutine(completedFadeOutDelay, 0f, null, delegate
			{
				rollerFade.FadeTo(0f, fadeOutTime, null, isRealtime: true, callback);
				rollerFadeOut = null;
			}, null, isRealtime: true);
		}
	}

	private void PlayRollSound()
	{
		if (!isPlayingRollSound && (bool)rollSound)
		{
			isPlayingRollSound = true;
			rollSound.Play();
		}
	}

	private void StopRollSound()
	{
		if (isPlayingRollSound && (bool)rollSound)
		{
			isPlayingRollSound = false;
			rollSound.Stop();
			if ((bool)rollEndSound)
			{
				rollSound.PlayOneShot(rollEndSound);
			}
		}
	}

	private void OnGamePauseChanged(bool isPaused)
	{
		if (PlayerData.instance.isInventoryOpen)
		{
			OnLevelLoaded();
			return;
		}
		if (!isPaused && showCount > 0)
		{
			if (!isVisible)
			{
				FadeIn();
			}
			IsActive = true;
		}
		if (isPlayingRollSound && (bool)rollSound)
		{
			if (isPaused)
			{
				rollSound.Pause();
			}
			else
			{
				rollSound.UnPause();
			}
		}
	}
}
