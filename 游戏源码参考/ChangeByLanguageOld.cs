using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

public abstract class ChangeByLanguageOld<T> : ChangeByLanguageBase where T : ChangeByLanguageOld<T>.LanguageOverride
{
	[Serializable]
	public abstract class LanguageOverride
	{
		public SupportedLanguages languageCode;
	}

	[SerializeField]
	protected List<T> offsetOverrides = new List<T>();

	[SerializeField]
	private bool onStartOnly;

	[SerializeField]
	private bool doHandHeldUpdates;

	private Dictionary<LanguageCode, T> languageCodeOverrides = new Dictionary<LanguageCode, T>();

	private Vector2 originalPosition;

	private bool hasStarted;

	private bool hasChanged;

	protected bool recorded;

	private bool registeredEvents;

	private bool handHeldEvents;

	private void Awake()
	{
		CreateLookup();
		DoAwake();
		RecordOriginalValues();
	}

	private void CreateLookup(bool log = true)
	{
		foreach (T offsetOverride in offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)offsetOverride.languageCode;
			if (!languageCodeOverrides.ContainsKey(languageCode))
			{
				languageCodeOverrides[languageCode] = offsetOverride;
			}
		}
	}

	private void Start()
	{
		hasStarted = true;
		DoStart();
		DoUpdate();
	}

	protected virtual void DoAwake()
	{
	}

	protected virtual void DoStart()
	{
	}

	protected virtual void OnValidate()
	{
		if (Application.isPlaying)
		{
			CreateLookup(log: false);
		}
	}

	protected virtual void OnEnable()
	{
		RegisterEvents();
		if (hasStarted && (!onStartOnly || CheatManager.ForceLanguageComponentUpdates))
		{
			DoUpdate();
		}
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	protected abstract void RecordOriginalValues();

	private void RegisterEvents()
	{
		if (!registeredEvents)
		{
			registeredEvents = true;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.RefreshLanguageText += DoUpdate;
			}
			if (doHandHeldUpdates)
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
				instance.RefreshLanguageText -= DoUpdate;
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
		if (doHandHeldUpdates)
		{
			DoUpdate();
		}
	}

	private void RevertValues()
	{
		if (hasChanged && recorded)
		{
			hasChanged = false;
			DoRevertValues();
		}
	}

	protected abstract void DoRevertValues();

	protected abstract void ApplySetting(T setting);

	public virtual void ApplyHandHeld()
	{
	}

	protected bool ShouldApplyHandHeld()
	{
		if (!doHandHeldUpdates)
		{
			return false;
		}
		return Platform.Current.IsRunningOnHandHeld;
	}

	public override void DoUpdate()
	{
		RevertValues();
		if (ShouldApplyHandHeld())
		{
			ApplyHandHeld();
			hasChanged = true;
		}
		LanguageCode key = Language.CurrentLanguage();
		if (languageCodeOverrides.TryGetValue(key, out var value))
		{
			ApplySetting(value);
			hasChanged = true;
		}
	}
}
