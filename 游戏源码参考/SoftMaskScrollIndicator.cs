using System;
using System.Collections;
using Coffee.UISoftMask;
using UnityEngine;

public sealed class SoftMaskScrollIndicator : ScrollIndicator
{
	[Serializable]
	private class Transition
	{
		public float duration = 0.15f;

		public float alpha;
	}

	[SerializeField]
	private SoftMask softMask;

	[SerializeField]
	private Transition transitionIn;

	[SerializeField]
	private Transition transitionOut;

	private UIManager ui;

	private Coroutine coroutine;

	private void Start()
	{
		ui = UIManager.instance;
		if ((bool)softMask)
		{
			softMask.alpha = transitionOut.alpha;
		}
	}

	private void OnValidate()
	{
		if (!softMask)
		{
			softMask = GetComponent<SoftMask>();
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
		if (!softMask)
		{
			yield break;
		}
		if (transition.duration > 0f)
		{
			float t = 0f;
			float inverse = 1f / transition.duration;
			float startValue = softMask.alpha;
			while (t < 1f)
			{
				yield return null;
				t += Time.deltaTime * inverse;
				softMask.alpha = Mathf.Lerp(startValue, transition.alpha, t);
			}
		}
		softMask.alpha = transition.alpha;
	}
}
