using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class ClearSaveButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler
	{
		[Header("Clear Save Button")]
		[SerializeField]
		private SaveSlotButton saveSlotButton;

		[SerializeField]
		private Animator selectIcon;

		private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");

		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
			case MoveDirection.Right:
				Navigate(eventData, FindSelectableOnRight());
				break;
			case MoveDirection.Left:
				Navigate(eventData, FindSelectableOnLeft());
				break;
			default:
				base.OnMove(eventData);
				break;
			}
		}

		private void Navigate(AxisEventData eventData, Selectable sel)
		{
			if (!(sel != null))
			{
				return;
			}
			if (sel.IsActive() && sel.IsInteractable())
			{
				eventData.selectedObject = sel.gameObject;
				return;
			}
			switch (eventData.moveDir)
			{
			case MoveDirection.Right:
				Navigate(eventData, sel.FindSelectableOnRight());
				break;
			case MoveDirection.Left:
				Navigate(eventData, sel.FindSelectableOnLeft());
				break;
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				base.OnSubmit(eventData);
				ForceDeselect();
				saveSlotButton.ClearSavePrompt();
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}

		public new void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (GetComponent<CanvasGroup>().interactable)
			{
				if ((bool)selectIcon)
				{
					selectIcon.SetBool(_isSelectedProp, value: true);
				}
			}
			else
			{
				StartCoroutine(SelectAfterFrame(base.navigation.selectOnUp.gameObject));
			}
		}

		protected override void OnDeselected(BaseEventData eventData)
		{
			if ((bool)selectIcon)
			{
				selectIcon.SetBool(_isSelectedProp, value: false);
			}
		}

		private IEnumerator SelectAfterFrame(GameObject obj)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}
}
