using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class HeroTriggerFader : MonoBehaviour
{
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}

	[SerializeField]
	private TrackTriggerObjects trigger;

	[SerializeField]
	private MinMaxFloat valueRange;

	[SerializeField]
	private float fadeTime;

	[SerializeField]
	private MinMaxFloat fadeUpFromZeroDelay;

	[SerializeField]
	private OverrideFloat fadeUpFromZeroTime;

	[SerializeField]
	private OverrideFloat fadeDownTime;

	[SerializeField]
	private bool fadeDownOnInspect;

	[Space]
	public UnityFloatEvent FadeValueChanged;

	public UnityEvent OnFadeFromZero;

	private bool hasStarted;

	private float currentValue;

	private void OnEnable()
	{
		if (HeroController.instance.isHeroInPosition)
		{
			StartCoroutine(MonitorTriggerAndFade());
		}
		else
		{
			HeroController.instance.heroInPosition += Delayed;
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnDestroy()
	{
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			instance.heroInPosition -= Delayed;
		}
	}

	private void Delayed(bool _)
	{
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(MonitorTriggerAndFade());
		}
		HeroController.instance.heroInPosition -= Delayed;
	}

	private IEnumerator MonitorTriggerAndFade()
	{
		SetValue(trigger.IsInside ? valueRange.End : valueRange.Start);
		while (true)
		{
			bool wasShowing = ShouldBeVisible();
			float startAlpha = currentValue;
			float fadeUpTime = fadeTime;
			if (Math.Abs(startAlpha) <= Mathf.Epsilon)
			{
				float randomValue = fadeUpFromZeroDelay.GetRandomValue();
				if (randomValue > 0f)
				{
					yield return new WaitForSeconds(randomValue);
				}
				OnFadeFromZero.Invoke();
				if (fadeUpFromZeroTime.IsEnabled)
				{
					fadeUpTime = fadeUpFromZeroTime.Value;
				}
			}
			float targetAlpha = (wasShowing ? valueRange.End : valueRange.Start);
			float num = Mathf.Abs(startAlpha - targetAlpha);
			float num2 = ((!wasShowing && fadeDownTime.IsEnabled) ? fadeDownTime.Value : fadeUpTime);
			float currentFadeTime = num * num2;
			for (float elapsedTime = 0f; elapsedTime < currentFadeTime; elapsedTime += Time.deltaTime)
			{
				if (ShouldBeVisible() != wasShowing)
				{
					break;
				}
				float t = Mathf.Clamp01(elapsedTime / currentFadeTime);
				SetValue(Mathf.Lerp(startAlpha, targetAlpha, t));
				yield return null;
			}
			SetValue(targetAlpha);
			while (ShouldBeVisible() == wasShowing)
			{
				yield return null;
			}
		}
	}

	private bool ShouldBeVisible()
	{
		if (fadeDownOnInspect)
		{
			if ((bool)InteractManager.BlockingInteractable)
			{
				return false;
			}
			if (InteractManager.IsPromptVisible)
			{
				return false;
			}
		}
		return trigger.IsInside;
	}

	private void SetValue(float value)
	{
		currentValue = value;
		if (FadeValueChanged != null)
		{
			FadeValueChanged.Invoke(value);
		}
	}
}
