using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ControllerButtonLabel : MonoBehaviour
{
	[Header("Button Text")]
	private Text buttonText;

	[Header("Button Label")]
	public string overrideLabelKey;

	[Header("Button Mapping")]
	public InputControlType controllerButton;

	private InputHandler ih;

	private UIManager ui;

	private void Awake()
	{
		ih = GameManager.instance.inputHandler;
		ui = UIManager.instance;
		buttonText = GetComponent<Text>();
	}

	private void OnEnable()
	{
		UpdateLanguage();
		Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
	}

	private void OnDisable()
	{
		Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			UpdateLanguage();
		}
	}

	private void UpdateLanguage()
	{
		if (!string.IsNullOrEmpty(overrideLabelKey))
		{
			buttonText.text = Language.Get(overrideLabelKey, "MainMenu");
		}
		else
		{
			ShowCurrentBinding();
		}
	}

	private void ShowCurrentBinding()
	{
		buttonText.text = "+";
		if (controllerButton != 0)
		{
			PlayerAction actionForMappableControllerButton = ih.GetActionForMappableControllerButton(controllerButton);
			if (actionForMappableControllerButton != null)
			{
				buttonText.text = Language.Get(ih.ActionButtonLocalizedKey(actionForMappableControllerButton), "MainMenu");
				return;
			}
			actionForMappableControllerButton = ih.GetActionForDefaultControllerButton(controllerButton);
			if (actionForMappableControllerButton != null)
			{
				buttonText.text = Language.Get(ih.ActionButtonLocalizedKey(actionForMappableControllerButton), "MainMenu");
			}
			else
			{
				buttonText.text = Language.Get("CTRL_UNMAPPED", "MainMenu");
			}
		}
		else
		{
			buttonText.text = Language.Get("CTRL_UNMAPPED", "MainMenu");
		}
	}
}
