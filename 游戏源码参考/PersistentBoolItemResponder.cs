using System;
using UnityEngine;
using UnityEngine.Events;

public class PersistentBoolItemResponder : MonoBehaviour
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private bool initialValue;

	[SerializeField]
	[Tooltip("If persistent hasn't loaded a save state then get it to save one first.")]
	private bool forcePersistentSave;

	[SerializeField]
	[Tooltip("If persistent hasn't loaded a save state then get it to try load.")]
	private bool forcePersistentLoad;

	[Space]
	public UnityBoolEvent OnGetValue;

	public UnityBoolEvent OnGetValueInverse;

	public UnityEvent OnGetValueTrue;

	public UnityEvent OnGetValueFalse;

	private bool started;

	private void Awake()
	{
		InvokeEvents(initialValue);
		if ((bool)persistent)
		{
			persistent.OnSetSaveState += InvokeEvents;
		}
	}

	private void Start()
	{
		started = true;
		if ((bool)persistent)
		{
			if (!persistent.HasLoadedValue && forcePersistentLoad)
			{
				persistent.LoadIfNeverStarted();
			}
			if (!persistent.HasLoadedValue && forcePersistentSave)
			{
				persistent.SaveState();
			}
			if (persistent.HasLoadedValue)
			{
				InvokeEvents(persistent.LoadedValue);
			}
		}
	}

	private void OnEnable()
	{
		if (started && (bool)persistent)
		{
			if (!persistent.HasLoadedValue && forcePersistentSave)
			{
				persistent.SaveState();
			}
			if (persistent.HasLoadedValue)
			{
				InvokeEvents(persistent.LoadedValue);
			}
		}
	}

	private void InvokeEvents(bool value)
	{
		OnGetValue.Invoke(value);
		OnGetValueInverse.Invoke(!value);
		if (value)
		{
			OnGetValueTrue.Invoke();
		}
		else
		{
			OnGetValueFalse.Invoke();
		}
	}
}
