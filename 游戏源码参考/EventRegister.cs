using System;
using System.Collections.Generic;
using UnityEngine;

public class EventRegister : EventBase
{
	[Serializable]
	private enum AliasEventMode
	{
		None = 0,
		SendAlias = 1
	}

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckSubscribedEvent")]
	private string subscribedEvent;

	[SerializeField]
	private PlayMakerFSM targetFsm;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string setFsmBool;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	private bool setFsmBoolValue;

	[Space]
	[SerializeField]
	private bool enableFsmBeforeSend;

	[Space]
	[SerializeField]
	private AliasEventMode aliasEventMode;

	[Tooltip("Alias event not added to register.")]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAlias")]
	private string aliasEvent;

	private new bool didAwake;

	private int subscribedEventHash;

	private static readonly Dictionary<int, List<EventRegister>> _eventRegister = new Dictionary<int, List<EventRegister>>();

	private static bool auditListsOnUnload;

	public override string InspectorInfo => subscribedEvent;

	public string SubscribedEvent
	{
		get
		{
			return subscribedEvent;
		}
		set
		{
			SwitchEvent(value);
		}
	}

	private bool? CheckAlias(string eventName)
	{
		if (aliasEventMode == AliasEventMode.None && string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		return IsEventValid(eventName);
	}

	private bool? CheckSubscribedEvent(string eventName)
	{
		if (aliasEventMode != 0)
		{
			return null;
		}
		return IsEventValid(eventName);
	}

	private bool? IsEventValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(setFsmBool))
		{
			if (targetFsm.IsEventValid(eventName, isRequired: true) == true)
			{
				return true;
			}
			return null;
		}
		return targetFsm.IsEventValid(eventName, isRequired: true);
	}

	private bool? IsFsmBoolValid(string boolName)
	{
		if (!targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return targetFsm.FsmVariables.FindFsmBool(boolName) != null;
	}

	private void Reset()
	{
		targetFsm = GetComponent<PlayMakerFSM>();
	}

	protected override void Awake()
	{
		base.Awake();
		didAwake = true;
		UpdateEventHash();
		SubscribeEvent(this);
	}

	private void OnDestroy()
	{
		UnsubscribeEvent(this);
	}

	private void UpdateEventHash()
	{
		subscribedEventHash = GetEventHashCode(subscribedEvent);
	}

	public static int GetEventHashCode(string eventName)
	{
		if (!string.IsNullOrWhiteSpace(eventName))
		{
			return eventName.GetHashCode();
		}
		return 0;
	}

	[ContextMenu("Test Receive", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Test Receive", false)]
	public void ReceiveEvent()
	{
		if (string.IsNullOrEmpty(subscribedEvent))
		{
			return;
		}
		string eventName = subscribedEvent;
		if (aliasEventMode == AliasEventMode.SendAlias && !string.IsNullOrEmpty(aliasEvent))
		{
			eventName = aliasEvent;
		}
		if ((bool)targetFsm)
		{
			bool flag = targetFsm.enabled;
			if (!flag && enableFsmBeforeSend)
			{
				targetFsm.enabled = true;
			}
			bool flag2 = targetFsm.SendEventRecursive(eventName);
			if (enableFsmBeforeSend && !flag && !flag2)
			{
				targetFsm.enabled = false;
			}
			if (!string.IsNullOrEmpty(setFsmBool))
			{
				targetFsm.FsmVariables.FindFsmBool(setFsmBool).Value = setFsmBoolValue;
			}
		}
		else
		{
			PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SendEventRecursive(eventName);
			}
		}
		CallReceivedEvent();
	}

	public void SwitchEvent(string eventName)
	{
		if (!didAwake && !auditListsOnUnload)
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				auditListsOnUnload = true;
				instance.NextSceneWillActivate += AuditEventRegisterLists;
			}
		}
		UnsubscribeEvent(this);
		subscribedEvent = eventName;
		UpdateEventHash();
		SubscribeEvent(this);
	}

	private static void AuditEventRegisterLists()
	{
		auditListsOnUnload = false;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.NextSceneWillActivate -= AuditEventRegisterLists;
		}
		foreach (List<EventRegister> value in _eventRegister.Values)
		{
			value.RemoveAll((EventRegister o) => o == null);
		}
	}

	public static void SendEvent(string eventName, GameObject excludeGameObject = null)
	{
		SendEvent(eventName.GetHashCode(), excludeGameObject);
	}

	public static void SendEvent(int eventNameHash, GameObject excludeGameObject = null)
	{
		if (!_eventRegister.TryGetValue(eventNameHash, out var value))
		{
			return;
		}
		if ((bool)excludeGameObject)
		{
			foreach (EventRegister item in value)
			{
				if (!(item.gameObject == excludeGameObject))
				{
					item.ReceiveEvent();
				}
			}
			return;
		}
		foreach (EventRegister item2 in value)
		{
			item2.ReceiveEvent();
		}
	}

	private static void SubscribeEvent(EventRegister register)
	{
		if (!register.didAwake)
		{
			return;
		}
		int num = register.subscribedEventHash;
		if (num != 0)
		{
			if (!_eventRegister.TryGetValue(num, out var value))
			{
				value = new List<EventRegister>();
				_eventRegister.Add(num, value);
			}
			value.Add(register);
		}
	}

	private static void UnsubscribeEvent(EventRegister register)
	{
		int num = register.subscribedEventHash;
		if (num != 0 && _eventRegister.TryGetValue(num, out var value) && value.Remove(register) && value.Count <= 0)
		{
			_eventRegister.Remove(num);
		}
	}

	public static EventRegister GetRegisterGuaranteed(GameObject gameObject, string eventName)
	{
		EventRegister[] components = gameObject.GetComponents<EventRegister>();
		foreach (EventRegister eventRegister in components)
		{
			if (eventRegister.subscribedEvent.Equals(eventName))
			{
				return eventRegister;
			}
		}
		EventRegister eventRegister2 = gameObject.AddComponent<EventRegister>();
		eventRegister2.SwitchEvent(eventName);
		return eventRegister2;
	}

	public static void RemoveRegister(GameObject gameObject, string eventName)
	{
		EventRegister[] components = gameObject.GetComponents<EventRegister>();
		foreach (EventRegister eventRegister in components)
		{
			if (eventRegister.subscribedEvent.Equals(eventName))
			{
				UnityEngine.Object.Destroy(eventRegister);
			}
		}
	}
}
