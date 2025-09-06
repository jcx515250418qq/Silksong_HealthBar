using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class EngageMenuScreen : MonoBehaviour
{
	private abstract class Transition
	{
		public abstract bool Update(float deltaTime);
	}

	private sealed class CrossFadeTransition : Transition
	{
		private enum State
		{
			NotStarted = 0,
			FadeOut = 1,
			FadeIn = 2,
			Finished = 3
		}

		private CanvasGroup fadeOutGroup;

		private CanvasGroup fadeInGroup;

		private float rate;

		private State state;

		private const float MENU_FADE_FAILSAFE = 2f;

		private float fadeInTimer;

		private float fadeOutTimer;

		private bool hasFadeOut;

		private bool hasFadeIn;

		public event Action OnTransitionComplete;

		public CrossFadeTransition(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup, float rate, Action callback = null)
		{
			this.fadeOutGroup = fadeOutGroup;
			this.fadeInGroup = fadeInGroup;
			this.rate = rate;
			hasFadeOut = fadeOutGroup;
			hasFadeIn = fadeInGroup;
			if (callback != null)
			{
				OnTransitionComplete += callback;
			}
		}

		public override bool Update(float deltaTime)
		{
			bool result = false;
			while (true)
			{
				switch (state)
				{
				case State.NotStarted:
					state = State.FadeOut;
					continue;
				case State.FadeOut:
					if (hasFadeOut)
					{
						float num = Mathf.Clamp01(fadeOutGroup.alpha - rate * deltaTime);
						fadeOutGroup.alpha = num;
						if (num > 0f)
						{
							fadeOutTimer += deltaTime;
							if (fadeOutTimer < 2f)
							{
								break;
							}
						}
					}
					state = State.FadeIn;
					continue;
				case State.FadeIn:
					if (hasFadeIn)
					{
						float num2 = Mathf.Clamp01(fadeInGroup.alpha + rate * deltaTime);
						fadeInGroup.alpha = num2;
						if (num2 < 1f)
						{
							fadeInTimer += deltaTime;
							if (fadeInTimer < 2f)
							{
								break;
							}
						}
					}
					state = State.Finished;
					continue;
				case State.Finished:
					this.OnTransitionComplete?.Invoke();
					result = true;
					break;
				default:
					state = State.NotStarted;
					continue;
				}
				break;
			}
			return result;
		}
	}

	private sealed class PulseTransition : Transition
	{
		private enum State
		{
			FadeOut = 0,
			FadeIn = 1
		}

		private CanvasGroup canvasGroup;

		private float fadeRate;

		private float minPause;

		private float maxPause;

		private float minAlpha;

		private float maxAlpha;

		private bool isValid;

		private float pauseTimer;

		private State state;

		public PulseTransition(CanvasGroup canvasGroup, float pulseDuration, float minPause, float maxPause, MinMaxFloat pulseMinMax)
		{
			this.canvasGroup = canvasGroup;
			this.minPause = minPause;
			this.maxPause = maxPause;
			minAlpha = pulseMinMax.Start;
			maxAlpha = pulseMinMax.End;
			pauseTimer = 0f;
			if (minAlpha > maxAlpha)
			{
				float num = maxAlpha;
				float num2 = minAlpha;
				minAlpha = num;
				maxAlpha = num2;
			}
			if (pulseDuration <= 0f)
			{
				pulseDuration = 1f;
			}
			fadeRate = (maxAlpha - minAlpha) / pulseDuration;
			isValid = fadeRate > 0f && (bool)canvasGroup;
		}

		public override bool Update(float deltaTime)
		{
			if (!isValid)
			{
				return true;
			}
			if (pauseTimer > 0f)
			{
				pauseTimer -= deltaTime;
				if (pauseTimer > 0f)
				{
					return false;
				}
			}
			switch (state)
			{
			case State.FadeOut:
			{
				float num2 = Mathf.Max(minAlpha, canvasGroup.alpha - fadeRate * deltaTime);
				canvasGroup.alpha = num2;
				if (!(num2 > minAlpha))
				{
					pauseTimer = minPause;
					state = State.FadeIn;
				}
				break;
			}
			case State.FadeIn:
			{
				float num = Mathf.Min(maxAlpha, canvasGroup.alpha + fadeRate * deltaTime);
				canvasGroup.alpha = num;
				if (!(num < maxAlpha))
				{
					pauseTimer = maxPause;
					state = State.FadeOut;
				}
				break;
			}
			default:
				state = State.FadeOut;
				break;
			}
			return false;
		}
	}

	[SerializeField]
	private CanvasGroup startGroup;

	[SerializeField]
	private CanvasGroup pendingGroup;

	[Header("Pulsing Settings")]
	[SerializeField]
	private MinMaxFloat pulseRange = new MinMaxFloat(0f, 1f);

	[SerializeField]
	private float pulseDuration = 0.5f;

	[SerializeField]
	private float minPause = 0.1f;

	[SerializeField]
	private float maxPause = 0.1f;

	[Header("Debug")]
	[SerializeField]
	private bool doDebug;

	[SerializeField]
	private Platform.EngagementStates debugState;

	private const float FADE_RATE = 3.2f;

	private Platform.EngagementStates currentState;

	private bool hasPlatform;

	private Transition transition;

	private Transition queuedTransition;

	private Transition QueuedTransition
	{
		get
		{
			return queuedTransition;
		}
		set
		{
			if (transition == null)
			{
				transition = value;
			}
			else
			{
				queuedTransition = value;
			}
		}
	}

	private void OnEnable()
	{
		hasPlatform = Platform.Current;
		if (hasPlatform)
		{
			SetEngagementStateInstant(Platform.Current.EngagementState);
		}
	}

	private void OnDisable()
	{
		transition = null;
		QueuedTransition = null;
	}

	private void Update()
	{
		if (hasPlatform)
		{
			UpdateEngagementState(Platform.Current.EngagementState);
		}
		if (transition != null && transition.Update(Time.unscaledDeltaTime))
		{
			if (QueuedTransition != null)
			{
				transition = QueuedTransition;
				queuedTransition = null;
			}
			else
			{
				transition = null;
			}
		}
	}

	private void SetEngagementStateInstant(Platform.EngagementStates state)
	{
		currentState = state;
		transition = null;
		QueuedTransition = null;
		switch (state)
		{
		case Platform.EngagementStates.NotEngaged:
			startGroup.alpha = 1f;
			pendingGroup.alpha = 0f;
			break;
		case Platform.EngagementStates.EngagePending:
		case Platform.EngagementStates.Engaged:
			startGroup.alpha = 0f;
			pendingGroup.alpha = 1f;
			transition = new PulseTransition(pendingGroup, pulseDuration, minPause, maxPause, pulseRange);
			break;
		}
	}

	private void UpdateEngagementState(Platform.EngagementStates state)
	{
		if (currentState == state)
		{
			return;
		}
		currentState = state;
		switch (state)
		{
		case Platform.EngagementStates.NotEngaged:
		{
			float fadeRate2 = GetFadeRate();
			transition = new CrossFadeTransition(pendingGroup, startGroup, fadeRate2);
			break;
		}
		case Platform.EngagementStates.EngagePending:
		case Platform.EngagementStates.Engaged:
		{
			float fadeRate = GetFadeRate();
			transition = new CrossFadeTransition(startGroup, pendingGroup, fadeRate, delegate
			{
				QueuedTransition = new PulseTransition(pendingGroup, pulseDuration, minPause, maxPause, pulseRange);
			});
			break;
		}
		}
	}

	private static float GetFadeRate()
	{
		float result = 3.2f;
		if ((bool)UIManager.instance)
		{
			result = UIManager.instance.MENU_FADE_SPEED;
		}
		return result;
	}
}
