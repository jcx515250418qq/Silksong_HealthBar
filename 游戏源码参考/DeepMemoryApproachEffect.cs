using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class DeepMemoryApproachEffect : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects insideRange;

	[SerializeField]
	private Transform followHero;

	[SerializeField]
	private MemoryHeartBeat heartBeatCtrl;

	[SerializeField]
	private NestedFadeGroupBase group;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private GameObject activeWhileInside;

	private bool wasInside;

	private Coroutine fadeRoutine;

	private float previousValue;

	private HeroController hc;

	private readonly int enterEventId = EventRegister.GetEventHashCode("ENTER DEEP MEMORY ZONE");

	private readonly int exitEventId = EventRegister.GetEventHashCode("EXIT DEEP MEMORY ZONE");

	private void Awake()
	{
		group.AlphaSelf = 0f;
	}

	private void Start()
	{
		hc = HeroController.instance;
		heartBeatCtrl.Multiplier = 0f;
		SetInside(isInside: false);
	}

	private void OnDisable()
	{
		SetInside(isInside: false);
	}

	private void LateUpdate()
	{
		if (!insideRange.IsInside)
		{
			if (previousValue > Mathf.Epsilon)
			{
				UpdatePosition();
			}
			if (wasInside)
			{
				if (fadeRoutine != null)
				{
					StopCoroutine(fadeRoutine);
				}
				fadeRoutine = StartCoroutine(FadeRoutine(0f, fadeOutDuration, fadeOutCurve));
				SetInside(isInside: false);
				EventRegister.SendEvent(exitEventId);
				wasInside = false;
			}
			return;
		}
		UpdatePosition();
		if (!wasInside)
		{
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeRoutine(1f, fadeInDuration, fadeInCurve));
			SetInside(isInside: true);
			EventRegister.SendEvent(enterEventId);
			wasInside = true;
		}
	}

	private void SetInside(bool isInside)
	{
		if ((bool)activeWhileInside)
		{
			activeWhileInside.SetActive(isInside);
		}
		if (isInside)
		{
			NeedolinMsgBox.AddBlocker(this);
		}
		else
		{
			NeedolinMsgBox.RemoveBlocker(this);
		}
	}

	private void UpdatePosition()
	{
		Vector2 position = hc.transform.position;
		followHero.SetPosition2D(position);
	}

	private IEnumerator FadeRoutine(float targetValue, float fadeDuration, AnimationCurve curve)
	{
		float initialValue = previousValue;
		for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.deltaTime)
		{
			float t = curve.Evaluate(elapsed / fadeDuration);
			float value = Mathf.Lerp(initialValue, targetValue, t);
			SetValue(value);
			yield return null;
		}
		SetValue(targetValue);
		fadeRoutine = null;
	}

	private void SetValue(float value)
	{
		previousValue = value;
		group.AlphaSelf = value;
		heartBeatCtrl.Multiplier = value;
	}
}
