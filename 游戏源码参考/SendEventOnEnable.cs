using UnityEngine;

public sealed class SendEventOnEnable : MonoBehaviour
{
	[SerializeField]
	private EventRelay target;

	[SerializeField]
	private string eventName;

	private void Awake()
	{
		if (target == null)
		{
			target = GetComponentInParent<EventRelay>();
		}
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = GetComponentInParent<EventRelay>();
		}
	}

	private void OnEnable()
	{
		SendEvent();
	}

	public void SendEvent()
	{
		if (!string.IsNullOrEmpty(eventName) && (bool)target)
		{
			target.SendEvent(eventName);
		}
	}
}
