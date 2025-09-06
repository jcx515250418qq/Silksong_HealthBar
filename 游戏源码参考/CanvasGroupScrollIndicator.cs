using System;
using System.Collections;
using UnityEngine;

public sealed class CanvasGroupScrollIndicator : ScrollIndicator
{
	[Serializable]
	private class Transition
	{
		public float duration = 0.15f;

		public float alpha;
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Transition transitionIn;

	[SerializeField]
	private Transition transitionOut;

	private UIManager ui;

	private Coroutine coroutine;

	private void Start()
	{
		ui = UIManager.instance;
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = transitionOut.alpha;
		}
	}

	private void OnValidate()
	{
		if (!canvasGroup)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
	}

	public override void Show()
	{
		DoTransition(transitionIn);
	}

	public override void Hide()
	{
		DoTransition(transitionOut);
	}

	private void DoTransition(Transition transition)
	{
		if (coroutine != null)
		{
			ui.StopCoroutine(coroutine);
		}
		coroutine = ui.StartCoroutine(DoTransitionRoutine(transition));
	}

	private IEnumerator DoTransitionRoutine(Transition transition)
	{
		if (!canvasGroup)
		{
			yield break;
		}
		if (transition.duration > 0f)
		{
			float t = 0f;
			float inverse = 1f / transition.duration;
			float startValue = canvasGroup.alpha;
			while (t < 1f)
			{
				yield return null;
				t += Time.deltaTime * inverse;
				canvasGroup.alpha = Mathf.Lerp(startValue, transition.alpha, t);
			}
		}
		canvasGroup.alpha = transition.alpha;
	}
}
