using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class RestoreSavePointDisplay : MonoBehaviour
{
	[Serializable]
	private class Selector
	{
		[SerializeField]
		private RectTransform selector;

		private bool hasTarget;

		private Transform target;

		private bool hideCalled;

		private bool waitedFrame;

		public void SetTargetAndShow(Transform target)
		{
			SetTarget(target);
			Show();
		}

		public void SetTarget(Transform target)
		{
			this.target = target;
			hasTarget = target;
			if (!hasTarget)
			{
				Hide();
			}
			Update();
		}

		public void Update()
		{
			if (hideCalled)
			{
				if (!waitedFrame)
				{
					waitedFrame = true;
					return;
				}
				hideCalled = false;
				Hide();
			}
			if (hasTarget)
			{
				selector.transform.position = target.transform.position;
			}
		}

		public void Show()
		{
			selector.gameObject.SetActive(value: true);
		}

		public void Hide()
		{
			selector.gameObject.SetActive(value: false);
		}

		public void DelayedHide()
		{
			hideCalled = true;
			waitedFrame = false;
		}
	}

	[SerializeField]
	private RestoreSaveButton restoreSaveButton;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private PreselectOption selectionHighlight;

	[SerializeField]
	private VerticalScrollRectController scrollRectController;

	[Space]
	[SerializeField]
	private RestoreSaveLoadIcon loadIcon;

	[Space]
	[SerializeField]
	private RestoreSaveSelectionButton template;

	[SerializeField]
	private List<RestoreSaveSelectionButton> buttons = new List<RestoreSaveSelectionButton>();

	[SerializeField]
	private Transform scrollContent;

	[SerializeField]
	private Selector selector;

	[Space]
	[SerializeField]
	private float scrollAmount = 130f;

	private bool init;

	private RestoreSaveSelectionButton lastSelection;

	private RestorePointFileWrapper selectedData;

	private UIManager ui;

	private InputHandler ih;

	private bool isActive;

	private bool hasSelector;

	private Navigation backButtonNav;

	private bool hasSetBackSelectable;

	private int lastActive = -1;

	private int buttonCount;

	private EventSystem eventSystem;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		eventSystem = EventSystem.current;
	}

	private void OnDisable()
	{
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		RestoreBackButtonNavigation();
	}

	private void Update()
	{
		if (isActive && ih.acceptingInput)
		{
			if (eventSystem.currentSelectedGameObject == null && lastSelection != null)
			{
				eventSystem.SetSelectedGameObject(lastSelection.gameObject);
			}
			if (ih.inputActions.MenuCancel.WasPressed)
			{
				Cancel();
			}
		}
	}

	private void LateUpdate()
	{
		if (isActive)
		{
			selector.Update();
		}
	}

	public void Init()
	{
		if (init)
		{
			return;
		}
		init = true;
		ih = GameManager.instance.inputHandler;
		ui = UIManager.instance;
		buttons.RemoveAll((RestoreSaveSelectionButton o) => o == null);
		RestoreSaveSelectionButton restoreSaveSelectionButton = null;
		for (int i = 0; i < buttons.Count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton2 = buttons[i];
			restoreSaveSelectionButton2.transform.SetSiblingIndex(i);
			restoreSaveSelectionButton2.gameObject.SetActive(value: false);
			restoreSaveSelectionButton2.SetRestoreParent(this);
			if (i > 0)
			{
				Navigation navigation = restoreSaveSelectionButton.navigation;
				navigation.selectOnDown = restoreSaveSelectionButton2;
				restoreSaveSelectionButton.navigation = navigation;
				Navigation navigation2 = restoreSaveSelectionButton2.navigation;
				navigation2.selectOnUp = restoreSaveSelectionButton;
				restoreSaveSelectionButton2.navigation = navigation2;
			}
			restoreSaveSelectionButton = restoreSaveSelectionButton2;
		}
	}

	private RestoreSaveSelectionButton CreateButton()
	{
		if (!template)
		{
			Debug.LogError($"{this} is missing button template", this);
			return null;
		}
		RestoreSaveSelectionButton restoreSaveSelectionButton = UnityEngine.Object.Instantiate(template);
		restoreSaveSelectionButton.transform.SetParent(scrollContent, worldPositionStays: false);
		restoreSaveSelectionButton.SetRestoreParent(this);
		return restoreSaveSelectionButton;
	}

	private void CreateMore(int count)
	{
		if (count <= buttons.Count)
		{
			return;
		}
		count -= buttons.Count;
		for (int i = 0; i < count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = CreateButton();
			if ((bool)restoreSaveSelectionButton)
			{
				restoreSaveSelectionButton.name = restoreSaveSelectionButton.name + " (" + (i + 2) + ")";
				buttons.Add(restoreSaveSelectionButton);
				continue;
			}
			Debug.LogError("Failed to create buttons", this);
			break;
		}
	}

	public void SetHidden()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		selector.Hide();
		HideLoadIconInstant();
	}

	public void ToggleLoadIcon(bool show)
	{
		if ((bool)loadIcon)
		{
			if (show)
			{
				base.gameObject.SetActive(value: true);
				loadIcon.StartFadeIn();
			}
			else
			{
				loadIcon.StartFadeOut();
			}
		}
	}

	private void HideLoadIconInstant()
	{
		if ((bool)loadIcon)
		{
			loadIcon.HideInstant();
		}
	}

	public void SetRestorePoints(List<RestorePointData> restorePoints)
	{
		if (restorePoints == null || restorePoints.Count == 0)
		{
			ClearRestorePoints();
			return;
		}
		if (restorePoints.Count > buttons.Count)
		{
			CreateMore(restorePoints.Count);
		}
		buttonCount = restorePoints.Count;
		for (int i = 0; i < buttons.Count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = buttons[i];
			if (i < restorePoints.Count)
			{
				RestorePointData restorePoint = restorePoints[i];
				restoreSaveSelectionButton.SetRestorePoint(restorePoint);
				restoreSaveSelectionButton.gameObject.SetActive(value: true);
				lastActive = i;
			}
			else
			{
				restoreSaveSelectionButton.ClearRestorePoint();
				restoreSaveSelectionButton.gameObject.SetActive(value: false);
			}
		}
	}

	public void ClearRestorePoints()
	{
		buttonCount = 0;
		for (int i = 0; i < buttons.Count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = buttons[i];
			restoreSaveSelectionButton.ClearRestorePoint();
			restoreSaveSelectionButton.gameObject.SetActive(value: false);
		}
	}

	public bool IsEmpty()
	{
		return buttonCount == 0;
	}

	private Selectable GetFirstSelectable()
	{
		if (buttons.Count > 0)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = buttons[0];
			if (restoreSaveSelectionButton.ButtonIsActive)
			{
				return restoreSaveSelectionButton;
			}
		}
		return GetBackButton();
	}

	private void Cancel()
	{
		isActive = false;
	}

	public void OnSelectButton(RestoreSaveSelectionButton button, bool isMouse = false)
	{
		scrollRectController.SetScrollTarget(button.transform, isMouse);
		selector.SetTargetAndShow(button.transform);
		lastSelection = button;
	}

	public void OnDeselectButton(RestoreSaveSelectionButton button)
	{
		selector.Hide();
	}

	public void SetSelectedData(RestorePointData data)
	{
		isActive = false;
		restoreSaveButton.SaveSelected(data);
	}

	public Selectable GetBackButton()
	{
		return restoreSaveButton.GetBackButton();
	}

	public void DoCancel()
	{
		restoreSaveButton.CancelSelection();
	}

	public IEnumerator Show()
	{
		ToggleLoadIcon(show: false);
		SetBackButtonNavigation();
		scrollRectController.ResetScroll();
		yield return ui.FadeInCanvasGroup(canvasGroup);
		HideLoadIconInstant();
		ih.StartUIInput();
		canvasGroup.blocksRaycasts = true;
		yield return null;
		ih.StopMouseInput();
		selectionHighlight.itemToHighlight = GetFirstSelectable();
		selectionHighlight.HighlightDefault(deselect: true);
		isActive = true;
		yield return null;
		ih.EnableMouseInput();
	}

	public IEnumerator Hide()
	{
		isActive = false;
		lastSelection = null;
		RestoreBackButtonNavigation();
		yield return ui.FadeOutCanvasGroup(canvasGroup);
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	private void SetBackButtonNavigation()
	{
		if (hasSetBackSelectable)
		{
			return;
		}
		Selectable backButton = GetBackButton();
		if (!backButton)
		{
			return;
		}
		hasSetBackSelectable = true;
		Navigation navigation = (backButtonNav = backButton.navigation);
		navigation.selectOnUp = null;
		for (int num = lastActive; num >= 0; num--)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = buttons[num];
			if (restoreSaveSelectionButton.ButtonIsActive)
			{
				navigation.selectOnUp = restoreSaveSelectionButton;
				break;
			}
		}
		backButton.navigation = navigation;
	}

	private void RestoreBackButtonNavigation()
	{
		if (hasSetBackSelectable)
		{
			GetBackButton().navigation = backButtonNav;
			hasSetBackSelectable = false;
		}
	}
}
