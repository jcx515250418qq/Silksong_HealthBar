using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestoreSaveSelectionButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler, ICancelHandler
{
	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text dateTimeText;

	private RestorePointData restorePointData;

	private RestoreSavePointDisplay parentDisplay;

	private bool isValid;

	public bool ButtonIsActive { get; private set; }

	public new void OnSubmit(BaseEventData eventData)
	{
		if (base.interactable && ButtonIsActive && isValid)
		{
			base.OnSubmit(eventData);
			if (SubmitEventToParent())
			{
				ForceDeselect();
			}
		}
	}

	public new void OnCancel(BaseEventData eventData)
	{
		if (base.interactable)
		{
			base.OnCancel(eventData);
			parentDisplay.DoCancel();
			PlayCancelSound();
		}
	}

	public new void OnPointerClick(PointerEventData eventData)
	{
		OnSubmit(eventData);
	}

	public new void OnSelect(BaseEventData eventData)
	{
		if (!base.interactable)
		{
			eventData.selectedObject = GetBackButton();
			return;
		}
		if (!ButtonIsActive || !base.gameObject.activeSelf)
		{
			eventData.selectedObject = GetBackButton();
			return;
		}
		base.OnSelect(eventData);
		parentDisplay.OnSelectButton(this, eventData is PointerEventData);
		eventData.Use();
	}

	public override void OnMove(AxisEventData eventData)
	{
		if (eventData.moveDir == MoveDirection.Down && (base.navigation.selectOnDown == null || !base.navigation.selectOnDown.gameObject.activeInHierarchy))
		{
			eventData.selectedObject = GetBackButton();
			parentDisplay.OnDeselectButton(this);
		}
		else
		{
			base.OnMove(eventData);
		}
	}

	public void SetRestorePoint(RestorePointData restorePointData)
	{
		this.restorePointData = restorePointData;
		ButtonIsActive = restorePointData != null;
		UpdateDisplay();
	}

	public void ClearRestorePoint()
	{
		ButtonIsActive = false;
		restorePointData = null;
	}

	private void UpdateDisplay()
	{
		if (ButtonIsActive)
		{
			nameText.text = restorePointData.GetName();
			dateTimeText.text = restorePointData.GetDateTime();
			isValid = restorePointData.IsValid();
		}
	}

	public void PrependNumber(int number)
	{
		nameText.text = $"{number}. {nameText.text}";
	}

	public void SetRestoreParent(RestoreSavePointDisplay restoreSavePointDisplay)
	{
		parentDisplay = restoreSavePointDisplay;
	}

	private GameObject GetBackButton()
	{
		if ((bool)parentDisplay)
		{
			Selectable backButton = parentDisplay.GetBackButton();
			if ((bool)backButton)
			{
				return backButton.gameObject;
			}
		}
		return null;
	}

	private bool SubmitEventToParent()
	{
		if ((bool)parentDisplay)
		{
			parentDisplay.SetSelectedData(restorePointData);
			return true;
		}
		Debug.LogError($"{this} is missing parent display", this);
		return false;
	}
}
