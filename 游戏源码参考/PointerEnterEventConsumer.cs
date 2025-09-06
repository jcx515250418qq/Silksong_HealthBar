using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PointerEnterEventConsumer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		eventData.Use();
	}
}
