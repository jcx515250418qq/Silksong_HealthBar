using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWindowBase : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	private RectTransform m_transform;

	private void Start()
	{
		m_transform = GetComponent<RectTransform>();
	}

	public void OnDrag(PointerEventData eventData)
	{
		m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
	}

	public void ChangeStrength(float value)
	{
		GetComponent<Image>().material.SetFloat("_Size", value);
	}

	public void ChangeVibrancy(float value)
	{
		GetComponent<Image>().material.SetFloat("_Vibrancy", value);
	}
}
