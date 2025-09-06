using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class ChangePositionByLanguage : MonoBehaviour
{
	[Serializable]
	private struct Override
	{
		public SupportedLanguages languageCode;

		public OverrideFloat xOverride;

		public OverrideFloat yOverride;

		public HandHeldOverride handHeldOverride;
	}

	[Serializable]
	private struct HandHeldOverride
	{
		public Platform.HandHeldTypes targetHandHeld;

		public OverrideFloat xOverride;

		public OverrideFloat yOverride;
	}

	[SerializeField]
	private List<Override> offsetOverrides = new List<Override>();

	[SerializeField]
	private bool onStartOnly;

	[SerializeField]
	private bool doHandHeldUpdate;

	[SerializeField]
	private HandHeldOverride handHeldOverride;

	private Dictionary<LanguageCode, Override> languageCodeOverrides = new Dictionary<LanguageCode, Override>();

	private Vector3 originalPosition;

	private bool hasStarted;

	private bool registeredEvents;

	private bool handHeldEvents;

	private void Awake()
	{
		foreach (Override offsetOverride in offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)offsetOverride.languageCode;
			if (!languageCodeOverrides.ContainsKey(languageCode))
			{
				languageCodeOverrides[languageCode] = offsetOverride;
			}
		}
		originalPosition = base.transform.localPosition;
	}

	private void Start()
	{
		hasStarted = true;
		DoOffset();
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (Override offsetOverride in offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)offsetOverride.languageCode;
			languageCodeOverrides[languageCode] = offsetOverride;
		}
	}

	private void OnEnable()
	{
		if (hasStarted && (!onStartOnly || CheatManager.ForceLanguageComponentUpdates))
		{
			DoOffset();
		}
		RegisterEvents();
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvents)
		{
			registeredEvents = true;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.RefreshLanguageText += DoOffset;
			}
			if (doHandHeldUpdate)
			{
				Platform.Current.OnScreenModeChanged += OnScreenModeChanged;
				handHeldEvents = true;
			}
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvents)
		{
			registeredEvents = false;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				registeredEvents = true;
				instance.RefreshLanguageText -= DoOffset;
			}
			if (handHeldEvents)
			{
				Platform.Current.OnScreenModeChanged -= OnScreenModeChanged;
				handHeldEvents = false;
			}
		}
	}

	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		if (handHeldEvents)
		{
			DoOffset();
		}
	}

	public void DoOffset()
	{
		LanguageCode key = Language.CurrentLanguage();
		Vector3 localPosition = originalPosition;
		if (languageCodeOverrides.TryGetValue(key, out var value))
		{
			if (value.xOverride.IsEnabled)
			{
				localPosition.x = value.xOverride.Value;
			}
			if (value.yOverride.IsEnabled)
			{
				localPosition.y = value.yOverride.Value;
			}
			if (doHandHeldUpdate && Platform.Current.IsRunningOnHandHeld && Platform.Current.IsTargetHandHeld(value.handHeldOverride.targetHandHeld))
			{
				if (value.handHeldOverride.xOverride.IsEnabled)
				{
					localPosition.x = value.handHeldOverride.xOverride.Value;
				}
				if (value.handHeldOverride.yOverride.IsEnabled)
				{
					localPosition.y = value.handHeldOverride.yOverride.Value;
				}
			}
		}
		else if (doHandHeldUpdate && Platform.Current.IsRunningOnHandHeld && Platform.Current.IsTargetHandHeld(handHeldOverride.targetHandHeld))
		{
			if (handHeldOverride.xOverride.IsEnabled)
			{
				localPosition.x = handHeldOverride.xOverride.Value;
			}
			if (handHeldOverride.yOverride.IsEnabled)
			{
				localPosition.y = handHeldOverride.yOverride.Value;
			}
		}
		base.transform.localPosition = localPosition;
	}

	[ContextMenu("Print Local Position")]
	private void PrintLocalPosition()
	{
		Debug.Log($"{this} local position: {base.transform.localPosition}", this);
	}
}
