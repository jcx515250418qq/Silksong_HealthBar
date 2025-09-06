using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	[RequireComponent(typeof(ScrollRect))]
	public class DebugMenuAutoScrollRect : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		private RectTransform scrollRectTransform;

		private RectTransform contentPanel;

		private RectTransform selectedRectTransform;

		private GameObject lastSelected;

		private Vector2 targetPos;

		private bool _mouseHover;

		private void Start()
		{
			scrollRectTransform = GetComponent<RectTransform>();
			if (contentPanel == null)
			{
				contentPanel = GetComponent<ScrollRect>().content;
			}
			targetPos = contentPanel.anchoredPosition;
		}

		private void Update()
		{
			if (!_mouseHover)
			{
				Autoscroll();
			}
		}

		public void Autoscroll()
		{
			if (contentPanel == null)
			{
				contentPanel = GetComponent<ScrollRect>().content;
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (!(currentSelectedGameObject == null) && !(currentSelectedGameObject.transform.parent != contentPanel.transform) && !(currentSelectedGameObject == lastSelected))
			{
				selectedRectTransform = (RectTransform)currentSelectedGameObject.transform;
				targetPos.x = contentPanel.anchoredPosition.x;
				targetPos.y = 0f - selectedRectTransform.localPosition.y - selectedRectTransform.rect.height / 2f;
				targetPos.y = Mathf.Clamp(targetPos.y, 0f, contentPanel.sizeDelta.y - scrollRectTransform.sizeDelta.y);
				contentPanel.anchoredPosition = targetPos;
				lastSelected = currentSelectedGameObject;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_mouseHover = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_mouseHover = false;
		}
	}
}
