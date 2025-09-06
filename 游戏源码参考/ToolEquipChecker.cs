using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ToolEquipChecker : MonoBehaviour
{
	[SerializeField]
	private ToolBase tool;

	[Space]
	[SerializeField]
	private EventRegister activateEvent;

	[Space]
	public UnityEvent<bool> ToolEquippedDynamic;

	public UnityEvent<bool> ToolEquippedDynamicReversed;

	[Space]
	[SerializeField]
	private float toolEquippedDelay;

	public UnityEvent ToolEquipped;

	[SerializeField]
	private float toolNotEquippedDelay;

	public UnityEvent ToolNotEquipped;

	private Coroutine invokeDelayedRoutine;

	private bool isActive;

	private EventRegister eventRegister;

	private void Awake()
	{
		eventRegister = EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED");
		if ((bool)activateEvent)
		{
			activateEvent.ReceivedEvent += OnActivate;
		}
		else
		{
			isActive = true;
		}
	}

	private void OnEnable()
	{
		eventRegister.ReceivedEvent += OnToolEquipsChanged;
		OnToolEquipsChanged();
	}

	private void OnDisable()
	{
		eventRegister.ReceivedEvent -= OnToolEquipsChanged;
	}

	private void OnActivate()
	{
		activateEvent.ReceivedEvent -= OnActivate;
		isActive = true;
		OnToolEquipsChanged();
	}

	private void OnToolEquipsChanged()
	{
		if (isActive)
		{
			bool flag = tool != null && ((tool is ToolItem toolItem) ? ToolItemManager.IsToolEquipped(toolItem, (base.gameObject.layer == 5) ? ToolEquippedReadSource.Hud : ToolEquippedReadSource.Active) : tool.IsEquipped);
			float num = (flag ? toolEquippedDelay : toolNotEquippedDelay);
			if (invokeDelayedRoutine != null)
			{
				StopCoroutine(invokeDelayedRoutine);
				invokeDelayedRoutine = null;
			}
			if (num > 0f)
			{
				invokeDelayedRoutine = StartCoroutine(InvokeDelayed(flag, num));
			}
			else
			{
				SendEvents(flag);
			}
		}
	}

	private void SendEvents(bool isEquipped)
	{
		ToolEquippedDynamic?.Invoke(isEquipped);
		ToolEquippedDynamicReversed?.Invoke(!isEquipped);
		if (isEquipped)
		{
			ToolEquipped?.Invoke();
		}
		else
		{
			ToolNotEquipped?.Invoke();
		}
	}

	private IEnumerator InvokeDelayed(bool isEquipped, float delay)
	{
		yield return new WaitForSeconds(delay);
		SendEvents(isEquipped);
	}
}
