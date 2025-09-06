using System;
using System.Collections;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class Remasker : MaskerBase, ScenePrefabInstanceFix.ICheckFields
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private AudioSource audioSource;

	[Space]
	[SerializeField]
	private float fadeTime = 0.5f;

	[SerializeField]
	private bool playSound;

	[SerializeField]
	private TrackTriggerObjects onlyPlaySoundInside;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("blackoutMask", false, false, false)]
	private bool inverse;

	[SerializeField]
	private bool blackoutMask;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase linkedInverseGroup;

	[SerializeField]
	private bool debugMe;

	private bool isCovered;

	private bool hasBeenUncovered;

	private bool isFrozen;

	private bool isInside;

	private bool hazardDisabled;

	private bool hazardEndIsInside;

	private int heroReadyFrame = -1;

	private Coroutine fadeWatchRoutine;

	private SpriteRenderer[] childSprites;

	private Color lastSetColor;

	private static Remasker _wasLastExited;

	private bool subscribedHeroInPosition;

	[NonSerialized]
	private bool hasSetInitialHeroPosition;

	private HeroController hc;

	private GameManager gm;

	private bool exitQueued;

	public bool ForceUncovered { get; set; }

	private bool IsCovered
	{
		get
		{
			if (isCovered)
			{
				return !ForceUncovered;
			}
			return false;
		}
	}

	private float BlackoutFadeTime
	{
		get
		{
			if (Time.frameCount - 1 <= heroReadyFrame || !hc || !hc.isHeroInPosition)
			{
				return 0f;
			}
			return fadeTime;
		}
	}

	private void Reset()
	{
		persistent = GetComponent<PersistentBoolItem>();
		audioSource = GetComponent<AudioSource>();
	}

	protected override void Awake()
	{
		base.Awake();
		if (!Application.isPlaying)
		{
			return;
		}
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = hasBeenUncovered;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				hasBeenUncovered = value;
			};
		}
		if (!blackoutMask)
		{
			return;
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			SpriteRenderer component2 = new GameObject("SpriteChild", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
			component2.transform.SetParentReset(base.transform);
			component2.sprite = component.sprite;
			NestedFadeGroupSpriteRenderer component3 = component.GetComponent<NestedFadeGroupSpriteRenderer>();
			if ((bool)component3)
			{
				UnityEngine.Object.DestroyImmediate(component3);
			}
			UnityEngine.Object.DestroyImmediate(component);
		}
		Material cutoutSpriteMaterial = Effects.CutoutSpriteMaterial;
		childSprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		SpriteRenderer[] array = childSprites;
		foreach (SpriteRenderer obj in array)
		{
			obj.sharedMaterial = cutoutSpriteMaterial;
			obj.gameObject.layer = 1;
			obj.sortingLayerName = "Over";
		}
	}

	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			hc = HeroController.instance;
			hc.OnDeath += OnHeroDeath;
			hc.OnHazardDeath += OnHeroHazardDeath;
			hc.OnHazardRespawn += OnHeroHazardRespawn;
			hc.heroInPosition += InitialHeroInPosition;
			gm = GameManager.instance;
			gm.OnFinishedEnteringScene += OnHeroFinishedEnteringScene;
			if (hc.isHeroInPosition)
			{
				OnHeroInPosition(forceDirect: false);
			}
			else
			{
				hc.heroInPosition += OnHeroInPosition;
				subscribedHeroInPosition = true;
			}
			EventRegister.GetRegisterGuaranteed(base.gameObject, "REMASKER FREEZE").ReceivedEvent += delegate
			{
				isFrozen = true;
			};
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Application.isPlaying)
		{
			if ((bool)hc)
			{
				hc.OnDeath -= OnHeroDeath;
				hc.OnHazardDeath -= OnHeroHazardDeath;
				hc.OnHazardRespawn -= OnHeroHazardRespawn;
				hc.heroInPosition -= InitialHeroInPosition;
			}
			if ((bool)gm)
			{
				gm.OnFinishedEnteringScene -= OnHeroFinishedEnteringScene;
			}
			if (_wasLastExited == this)
			{
				_wasLastExited = null;
			}
			MaskerBlackout.RemoveInside(this, BlackoutFadeTime);
		}
	}

	private void OnHeroDeath()
	{
		isFrozen = true;
	}

	private void OnHeroHazardDeath()
	{
		hazardDisabled = true;
		hazardEndIsInside = isInside;
	}

	private void OnHeroHazardRespawn()
	{
		hazardDisabled = false;
		if (isInside)
		{
			if (!hazardEndIsInside)
			{
				Exited(wasDisabled: false);
			}
		}
		else if (hazardEndIsInside)
		{
			Entered();
		}
	}

	private void OnHeroFinishedEnteringScene()
	{
		if (!isInside && !isCovered)
		{
			isCovered = true;
		}
	}

	private void InitialHeroInPosition(bool forcedirect)
	{
		if (hasSetInitialHeroPosition)
		{
			return;
		}
		hasSetInitialHeroPosition = true;
		if (!isInside)
		{
			isCovered = true;
		}
		heroReadyFrame = Time.frameCount;
		if (blackoutMask && isInside)
		{
			MaskerBlackout.SetMaskFade(1f);
		}
		else if (exitQueued)
		{
			exitQueued = false;
			if (MaskerBlackout.RemoveInside(this, 0f))
			{
				_wasLastExited = this;
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (Application.isPlaying)
		{
			if (blackoutMask)
			{
				base.AlphaSelf = 1f;
				SetColor(Color.green);
			}
			else
			{
				base.AlphaSelf = ((!inverse) ? 1 : 0);
			}
			isCovered = true;
			if ((bool)hc && hc.isHeroInPosition)
			{
				OnHeroInPosition(forceDirect: false);
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (Application.isPlaying)
		{
			if ((bool)hc && subscribedHeroInPosition)
			{
				hc.heroInPosition -= OnHeroInPosition;
				subscribedHeroInPosition = false;
			}
			StopFadeWatchRoutine();
			if (isInside)
			{
				Exited(wasDisabled: true);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.isActiveAndEnabled && collision.CompareTag("Player"))
		{
			if (hazardDisabled)
			{
				hazardEndIsInside = true;
			}
			else if (!hc || !hc.cState.isTriggerEventsPaused)
			{
				Entered();
			}
		}
	}

	private void Entered()
	{
		isInside = true;
		if (isFrozen)
		{
			return;
		}
		if (blackoutMask)
		{
			if (MaskerBlackout.AddInside(this, BlackoutFadeTime))
			{
				SetColor(Color.blue);
			}
			else
			{
				Color color = lastSetColor;
				color.g = 0f;
				SetColor(color);
			}
			_wasLastExited = null;
		}
		exitQueued = false;
		isCovered = false;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (base.isActiveAndEnabled && collision.CompareTag("Player"))
		{
			if (hazardDisabled)
			{
				hazardEndIsInside = false;
			}
			else if (!hc || !hc.cState.isTriggerEventsPaused)
			{
				Exited(wasDisabled: false);
			}
		}
	}

	public void Exited(bool wasDisabled)
	{
		isInside = false;
		if (isFrozen || hc.cState.hazardDeath)
		{
			return;
		}
		if (!wasDisabled && hc.transitionState == HeroTransitionState.EXITING_SCENE)
		{
			exitQueued = true;
			return;
		}
		if (blackoutMask)
		{
			float fadeOutTime = (wasDisabled ? 0f : BlackoutFadeTime);
			if (MaskerBlackout.RemoveInside(this, fadeOutTime))
			{
				_wasLastExited = this;
			}
		}
		isCovered = true;
	}

	private void StopFadeWatchRoutine()
	{
		if (fadeWatchRoutine != null)
		{
			StopCoroutine(fadeWatchRoutine);
			fadeWatchRoutine = null;
		}
	}

	private void OnHeroInPosition(bool forceDirect)
	{
		InitialHeroInPosition(forceDirect);
		if (subscribedHeroInPosition)
		{
			hc.heroInPosition -= OnHeroInPosition;
			subscribedHeroInPosition = false;
		}
		StopFadeWatchRoutine();
		fadeWatchRoutine = StartCoroutine(FadeWatch());
	}

	private IEnumerator FadeWatch()
	{
		SetInitialState();
		yield return null;
		SetInitialState();
		while (true)
		{
			if (IsCovered)
			{
				yield return null;
				continue;
			}
			if (!hasBeenUncovered)
			{
				hasBeenUncovered = true;
				if (playSound && (!onlyPlaySoundInside || onlyPlaySoundInside.IsInside))
				{
					EventRegister.SendEvent("SECRET TONE");
				}
			}
			if (blackoutMask)
			{
				yield return StartCoroutine(FadeColor(Color.blue, fadeTime, targetCoveredState: false));
			}
			else
			{
				yield return StartCoroutine(FadeAlpha(inverse ? 1 : 0, fadeTime, targetCoveredState: false));
			}
			while (!IsCovered)
			{
				yield return null;
			}
			if (blackoutMask)
			{
				if (_wasLastExited == this)
				{
					while (_wasLastExited == this)
					{
						if (!MaskerBlackout.IsAnyFading)
						{
							_wasLastExited = null;
							break;
						}
						yield return null;
					}
					SetColor(Color.green);
				}
				else
				{
					yield return StartCoroutine(FadeColor(Color.green, fadeTime, targetCoveredState: true));
				}
			}
			else
			{
				yield return StartCoroutine(FadeAlpha((!inverse) ? 1 : 0, fadeTime, targetCoveredState: true));
			}
		}
		void SetInitialState()
		{
			if (blackoutMask)
			{
				SetColor((IsCovered && !isInside) ? Color.green : Color.blue);
			}
			else if (IsCovered && !isInside)
			{
				base.AlphaSelf = ((!inverse) ? 1 : 0);
			}
			else
			{
				base.AlphaSelf = (inverse ? 1 : 0);
			}
		}
	}

	private IEnumerator FadeAlpha(float toAlpha, float time, bool targetCoveredState)
	{
		float fadingTime = FadeTo(toAlpha, time);
		for (float elapsed = 0f; elapsed < fadingTime; elapsed += Time.deltaTime)
		{
			if (IsCovered != targetCoveredState)
			{
				break;
			}
			yield return null;
		}
	}

	private IEnumerator FadeColor(Color toColor, float time, bool targetCoveredState)
	{
		Color fromColor = lastSetColor;
		for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
		{
			if (IsCovered != targetCoveredState)
			{
				break;
			}
			float t = elapsed / time;
			SetColor(Color.Lerp(fromColor, toColor, t));
			yield return null;
		}
		if (IsCovered == targetCoveredState)
		{
			SetColor(toColor);
		}
	}

	protected override void OnAlphaChanged(float alpha)
	{
		base.OnAlphaChanged(alpha);
		if ((bool)linkedInverseGroup)
		{
			linkedInverseGroup.AlphaSelf = ((MaskerBase.ApplyToInverseMasks && (MaskerBase.UseTestingAlphaInPlayMode || !Application.isPlaying)) ? alpha : (1f - alpha));
		}
	}

	private void SetColor(Color color)
	{
		lastSetColor = color;
		SpriteRenderer[] array = childSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			color.a = spriteRenderer.color.a;
			spriteRenderer.color = color;
		}
	}

	public void SetPlaySound(bool setPlaySound)
	{
		playSound = setPlaySound;
	}

	public void OnPrefabInstanceFix()
	{
		if ((bool)persistent)
		{
			ScenePrefabInstanceFix.CheckField(ref persistent);
		}
	}
}
