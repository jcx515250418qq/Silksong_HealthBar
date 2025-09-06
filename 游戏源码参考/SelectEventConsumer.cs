using UnityEngine;
using UnityEngine.EventSystems;

public sealed class SelectEventConsumer : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public void OnSelect(BaseEventData eventData)
	{
		eventData.Use();
	}
}
