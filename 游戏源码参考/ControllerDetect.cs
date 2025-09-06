using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ControllerDetect : MonoBehaviour
{
	private GameManager gm;

	private UIManager ui;

	private InputHandler ih;

	private Image controllerImage;

	[Header("Controller Menu Items")]
	public CanvasGroup controllerPrompt;

	public CanvasGroup remapDialog;

	public CanvasGroup menuControls;

	public CanvasGroup remapControls;

	[Header("Controller Menu Preselect")]
	public Selectable controllerMenuPreselect;

	public Selectable remapMenuPreselect;

	[Header("Remap Menu Controls")]
	public MenuSelectable remapApplyButton;

	public MenuSelectable defaultsButton;

	[Header("Controller Menu Controls")]
	public MenuButton applyButton;

	public MenuButton remapButton;

	public float remapHiddenOffsetY;

	[SerializeField]
	public ControllerImage[] controllerImages;

	private float profileYPos;

	private MenuButtonList menuButtonList;

	private void Awake()
	{
		gm = GameManager.instance;
		ih = gm.inputHandler;
		ui = UIManager.instance;
		controllerImage = GetComponent<Image>();
		profileYPos = GetComponent<RectTransform>().anchoredPosition.y;
		menuButtonList = GetComponentInParent<MenuButtonList>();
	}

	private void OnEnable()
	{
		LookForActiveController();
		InputManager.OnActiveDeviceChanged += ControllerActivated;
		InputManager.OnDeviceAttached += ControllerAttached;
		InputManager.OnDeviceDetached += ControllerDetached;
	}

	private void OnDisable()
	{
		InputManager.OnActiveDeviceChanged -= ControllerActivated;
		InputManager.OnDeviceAttached -= ControllerAttached;
		InputManager.OnDeviceDetached -= ControllerDetached;
	}

	private void ControllerActivated(InputDevice inputDevice)
	{
		LookForActiveController();
	}

	private void ControllerAttached(InputDevice inputDevice)
	{
		LookForActiveController();
	}

	private void ControllerDetached(InputDevice inputDevice)
	{
		LookForActiveController();
		if (EventSystem.current != applyButton)
		{
			applyButton.Select();
		}
	}

	private void ShowController(GamepadType gamepadType)
	{
		gamepadType = Platform.Current.OverrideGamepadDisplay(gamepadType);
		GamepadType gamepadType2 = ((gamepadType != GamepadType.PS3_WIN) ? gamepadType : GamepadType.PS4);
		ControllerImage[] array = controllerImages;
		foreach (ControllerImage controllerImage in array)
		{
			if (controllerImage.buttonPositions != null)
			{
				controllerImage.buttonPositions.gameObject.SetActive(value: false);
			}
		}
		float num = (DemoHelper.IsDemoMode ? remapHiddenOffsetY : 0f);
		array = controllerImages;
		foreach (ControllerImage controllerImage2 in array)
		{
			if (controllerImage2.gamepadType == gamepadType2)
			{
				this.controllerImage.sprite = controllerImage2.sprite;
				if (controllerImage2.buttonPositions != null)
				{
					controllerImage2.buttonPositions.gameObject.SetActive(value: true);
				}
				base.transform.localScale = new Vector3(controllerImage2.displayScale, controllerImage2.displayScale, 1f);
				RectTransform component = GetComponent<RectTransform>();
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.y = profileYPos + controllerImage2.offsetY + num;
				component.anchoredPosition = anchoredPosition;
				break;
			}
		}
	}

	private void HideButtonLabels()
	{
		ControllerImage[] array = controllerImages;
		foreach (ControllerImage controllerImage in array)
		{
			if (controllerImage.buttonPositions != null)
			{
				controllerImage.buttonPositions.gameObject.SetActive(value: false);
			}
		}
	}

	private void LookForActiveController()
	{
		if (ih.gamepadState == GamepadState.DETACHED)
		{
			HideButtonLabels();
			controllerImage.sprite = controllerImages[0].sprite;
			ui.ShowCanvasGroup(controllerPrompt);
			remapButton.gameObject.SetActive(value: false);
		}
		else if (ih.activeGamepadType != 0)
		{
			ui.HideCanvasGroup(controllerPrompt);
			remapButton.gameObject.SetActive(value: true);
			ShowController((ih.ActiveGamepadAlias != 0) ? ih.ActiveGamepadAlias : ih.activeGamepadType);
		}
		if ((bool)menuButtonList)
		{
			menuButtonList.SetupActive();
		}
	}
}
