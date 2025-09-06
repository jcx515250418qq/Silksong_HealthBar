using System.Collections;
using System.Collections.Generic;
using Coffee.UISoftMask;
using UnityEngine;

public sealed class RestoreSaveLoadIcon : MonoBehaviour
{
	private sealed class ThrobberControl
	{
		public SoftMask softMask;

		public float alpha;

		private float fadeDuration;

		private float fadeElapsed;

		private bool isThrobbing;

		public bool IsThrobbing => isThrobbing;

		public void Reset(bool fromZero)
		{
			alpha = (fromZero ? 0f : 1f);
			softMask.alpha = alpha;
			isThrobbing = false;
		}

		public void StartThrob(float duration)
		{
			fadeDuration = duration;
			fadeElapsed = 0f;
			isThrobbing = true;
		}

		public void UpdateThrob(float deltaTime, bool fromZero)
		{
			if (isThrobbing)
			{
				fadeElapsed += deltaTime;
				alpha = Mathf.Lerp(fromZero ? 0f : 1f, fromZero ? 1f : 0f, fadeElapsed / fadeDuration);
				softMask.alpha = alpha;
				if (fadeElapsed >= fadeDuration)
				{
					isThrobbing = false;
				}
			}
		}
	}

	private enum FadeState
	{
		None = 0,
		FadeIn = 1,
		FadeOut = 2
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private List<SoftMask> softMasks = new List<SoftMask>();

	[SerializeField]
	private bool fadeInOnEnable = true;

	[SerializeField]
	private float fadeInDuration = 0.5f;

	[SerializeField]
	private float fadeOutDuration = 0.5f;

	[SerializeField]
	private float throbInterval = 0.125f;

	[SerializeField]
	private float throbDuration = 0.5f;

	[SerializeField]
	private bool startFromZero;

	[SerializeField]
	private bool throbFromZero = true;

	private List<ThrobberControl> throbberControls = new List<ThrobberControl>();

	private int currentThrobberIndex;

	private float throbTimer;

	private Coroutine fadeRoutine;

	private FadeState fadeState;

	private float menuFadeSpeed = 3.2f;

	private bool fadeInQueued;

	private void Awake()
	{
		softMasks.RemoveAll((SoftMask o) => o == null);
		foreach (SoftMask softMask in softMasks)
		{
			ThrobberControl item = new ThrobberControl
			{
				softMask = softMask
			};
			throbberControls.Add(item);
		}
		if (fadeState != FadeState.FadeIn)
		{
			canvasGroup.alpha = 0f;
		}
	}

	private void OnValidate()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
	}

	private void OnEnable()
	{
		UIManager instance = UIManager.instance;
		if ((bool)instance)
		{
			menuFadeSpeed = instance.MENU_FADE_SPEED;
		}
		if (fadeInOnEnable || fadeInQueued)
		{
			StartFadeIn();
		}
		foreach (ThrobberControl throbberControl in throbberControls)
		{
			throbberControl.Reset(startFromZero);
		}
		currentThrobberIndex = 0;
		throbTimer = 0f;
	}

	private void Update()
	{
		if (throbberControls.Count == 0)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		foreach (ThrobberControl throbberControl in throbberControls)
		{
			throbberControl.UpdateThrob(deltaTime, throbFromZero);
		}
		throbTimer += deltaTime;
		if (throbTimer >= throbInterval)
		{
			throbTimer = 0f;
			throbberControls[currentThrobberIndex].StartThrob(throbDuration);
			currentThrobberIndex = (currentThrobberIndex + 1) % throbberControls.Count;
		}
	}

	public void StartFadeIn()
	{
		if (fadeState == FadeState.FadeIn)
		{
			return;
		}
		fadeState = FadeState.FadeIn;
		base.gameObject.SetActive(value: true);
		if (!base.gameObject.activeInHierarchy)
		{
			fadeState = FadeState.None;
			fadeInQueued = true;
			return;
		}
		fadeInQueued = false;
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeInRoutine());
	}

	public void StartFadeOut()
	{
		if (fadeState == FadeState.FadeOut)
		{
			return;
		}
		fadeState = FadeState.FadeOut;
		fadeInQueued = false;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(value: false);
			canvasGroup.alpha = 0f;
			return;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeOutRoutine());
	}

	public void HideInstant()
	{
		fadeState = FadeState.None;
		canvasGroup.alpha = 0f;
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator FadeInRoutine()
	{
		fadeState = FadeState.FadeIn;
		float loopFailsafe = 0f;
		while (canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += Time.unscaledDeltaTime * menuFadeSpeed;
			loopFailsafe += Time.unscaledDeltaTime;
			if (canvasGroup.alpha >= 0.95f)
			{
				canvasGroup.alpha = 1f;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		canvasGroup.alpha = 1f;
	}

	private IEnumerator FadeOutRoutine()
	{
		fadeState = FadeState.FadeOut;
		float loopFailsafe = 0f;
		while (canvasGroup.alpha > 0.05f)
		{
			canvasGroup.alpha -= Time.unscaledDeltaTime * menuFadeSpeed;
			loopFailsafe += Time.unscaledDeltaTime;
			if (canvasGroup.alpha <= 0.05f || loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		canvasGroup.alpha = 0f;
		base.gameObject.SetActive(value: false);
	}

	[ContextMenu("Gather SoftMasks")]
	private void GatherSoftMasks()
	{
		softMasks.Clear();
		softMasks.AddRange(base.gameObject.GetComponentsInChildren<SoftMask>(includeInactive: true));
	}
}
