using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class ControllerProfileButton : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
	{
		public Animator leftCursor;

		public Animator rightCursor;

		public Image highlightCursor;

		public MenuAudioController uiAudioPlayer;

		private GameObject prevSelectedObject;

		private bool dontPlaySelectSound;

		public void OnSelect(BaseEventData eventData)
		{
			leftCursor.ResetTrigger("hide");
			rightCursor.ResetTrigger("hide");
			leftCursor.SetTrigger("show");
			rightCursor.SetTrigger("show");
			if (!dontPlaySelectSound)
			{
				uiAudioPlayer.PlaySelect();
			}
			else
			{
				dontPlaySelectSound = false;
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StartCoroutine(ValidateDeselect());
		}

		public void OnSubmit(BaseEventData eventData)
		{
			highlightCursor.gameObject.SetActive(value: true);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}

		private IEnumerator ValidateDeselect()
		{
			prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				leftCursor.ResetTrigger("show");
				rightCursor.ResetTrigger("show");
				leftCursor.SetTrigger("hide");
				rightCursor.SetTrigger("hide");
				yield break;
			}
			if (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
			{
				while (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
				{
					yield return null;
				}
			}
			yield return null;
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				leftCursor.ResetTrigger("show");
				rightCursor.ResetTrigger("show");
				leftCursor.SetTrigger("hide");
				rightCursor.SetTrigger("hide");
			}
			else if (prevSelectedObject != null && prevSelectedObject.activeInHierarchy)
			{
				dontPlaySelectSound = true;
				EventSystem.current.SetSelectedGameObject(prevSelectedObject);
			}
		}
	}
}
