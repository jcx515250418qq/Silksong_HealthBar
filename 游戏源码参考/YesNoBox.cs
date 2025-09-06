using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class YesNoBox : MonoBehaviour
{
	[SerializeField]
	protected InventoryPaneStandalone pane;

	[SerializeField]
	private PlayMakerFSM hudFSM;

	[SerializeField]
	private UISelectionList uiList;

	[Space]
	[SerializeField]
	private UISelectionListItem yesButton;

	[Space]
	[SerializeField]
	private LayoutGroup refreshLayoutGroup;

	private Action currentYes;

	private Action currentNo;

	private bool currentReturnHud;

	private bool selectedState;

	protected virtual string InactiveYesText => string.Empty;

	protected virtual bool ShouldHideHud => true;

	protected virtual void Awake()
	{
		pane.PaneClosedAnimEnd += delegate
		{
			Action obj = (selectedState ? currentYes : currentNo);
			Clear();
			if (currentReturnHud)
			{
				hudFSM.SendEventSafe("IN");
			}
			obj?.Invoke();
		};
		if ((bool)yesButton)
		{
			yesButton.InactiveConditionText = () => InactiveYesText;
		}
	}

	public void SelectYes()
	{
		if (string.IsNullOrEmpty(InactiveYesText))
		{
			selectedState = true;
			DoEnd();
		}
	}

	public void SelectNo()
	{
		selectedState = false;
		DoEnd();
	}

	protected void DoEnd()
	{
		if ((bool)uiList)
		{
			uiList.SetActive(value: false);
		}
		pane.PaneEnd();
	}

	protected void OnAppearing()
	{
		if ((bool)uiList)
		{
			uiList.SetActive(value: false);
		}
	}

	protected void OnAppeared()
	{
		if ((bool)uiList)
		{
			uiList.SetActive(value: true);
		}
	}

	private void Clear()
	{
		currentYes = null;
		currentNo = null;
	}

	protected void InternalOpen(Action yes, Action no, bool returnHud)
	{
		currentYes = yes;
		currentNo = no;
		currentReturnHud = returnHud;
		if (ShouldHideHud)
		{
			hudFSM.SendEventSafe("OUT");
		}
		DialogueBox.EndConversation(returnHud: false, DoOpen);
	}

	private void DoOpen()
	{
		pane.PaneStart();
		if ((bool)refreshLayoutGroup)
		{
			refreshLayoutGroup.ForceUpdateLayoutNoCanvas();
		}
	}
}
