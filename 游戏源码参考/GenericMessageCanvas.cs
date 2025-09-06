using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericMessageCanvas : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup rootGroup;

	[SerializeField]
	private Text labelText;

	[SerializeField]
	private Text descText;

	[SerializeField]
	private GameObject okButton;

	private Action okButtonCallback;

	private bool isActive;

	private static GenericMessageCanvas _instance;

	public static bool IsActive
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance.isActive;
			}
			return false;
		}
	}

	protected void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		rootGroup.alpha = 0f;
		rootGroup.interactable = false;
		rootGroup.blocksRaycasts = false;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void Show(string errorKey, Action onOk)
	{
		_instance.ShowInternal(errorKey, onOk);
	}

	private void ShowInternal(string errorKey, Action onOk)
	{
		isActive = true;
		HeroController hc = HeroController.SilentInstance;
		if ((bool)hc)
		{
			hc.AddInputBlocker(this);
		}
		InputHandler ih = ManagerSingleton<InputHandler>.Instance;
		bool wasInputActive = ih.acceptingInput;
		GameObject previouslySelected = EventSystem.current.currentSelectedGameObject;
		okButtonCallback = delegate
		{
			EventSystem.current.SetSelectedGameObject(null);
			Hide();
			if (!wasInputActive)
			{
				ih.StopUIInput();
			}
			if ((bool)previouslySelected)
			{
				EventSystem.current.SetSelectedGameObject(previouslySelected);
			}
			onOk?.Invoke();
			if ((bool)hc)
			{
				hc.RemoveInputBlocker(this);
			}
			isActive = false;
		};
		labelText.text = new LocalisedString("Error", errorKey + "_TITLE");
		descText.text = new LocalisedString("Error", errorKey + "_DESC");
		rootGroup.alpha = 1f;
		rootGroup.interactable = true;
		rootGroup.blocksRaycasts = true;
		ih.StartUIInput();
		EventSystem.current.SetSelectedGameObject(okButton);
	}

	private void Hide()
	{
		rootGroup.alpha = 0f;
		rootGroup.interactable = false;
		rootGroup.blocksRaycasts = false;
	}

	public void OkButtonClicked()
	{
		if (okButtonCallback != null)
		{
			okButtonCallback();
			okButtonCallback = null;
		}
	}
}
