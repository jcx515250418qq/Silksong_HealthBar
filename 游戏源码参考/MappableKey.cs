using System.Collections.Generic;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MappableKey : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ICancelHandler
{
	private GameManager gm;

	private InputHandler ih;

	private UIManager ui;

	private UIButtonSkins uibs;

	private PlayerAction playerAction;

	private bool active;

	private bool isListening;

	private int oldFontSize;

	private TextAnchor oldAlignment;

	private Sprite oldSprite;

	private string oldText;

	private InputHandler.KeyOrMouseBinding currentBinding;

	private PlayerAction actionToSwap;

	private BindingSource bindingToSwap;

	private List<KeyBindingSource> unmappableKeys;

	private const float sqrX = 32f;

	private const float sqrWidth = 65f;

	private const bool sqrBestFit = true;

	private const int sqrFontSize = 46;

	private const int sqrMinFont = 20;

	private const int sqrMaxFont = 46;

	private const HorizontalWrapMode sqrHOverflow = HorizontalWrapMode.Wrap;

	private const TextAnchor sqrAlignment = TextAnchor.MiddleCenter;

	private const float wideX = 4f;

	private const float wideWidth = 137f;

	private const bool wideBestFit = false;

	private const int wideFontSize = 40;

	private const HorizontalWrapMode wideHOverflow = HorizontalWrapMode.Wrap;

	private const TextAnchor wideAlignment = TextAnchor.MiddleCenter;

	private const bool blankBestFit = false;

	private const int blankFontSize = 46;

	private const HorizontalWrapMode blankOverflow = HorizontalWrapMode.Overflow;

	private const TextAnchor blankAlignment = TextAnchor.MiddleRight;

	[Space(6f)]
	[Header("Button Mapping")]
	public HeroActionButton actionButtonType;

	public Text keymapText;

	public Image keymapSprite;

	private List<BindingSource> oldBindings = new List<BindingSource>();

	private new void Start()
	{
		if (Application.isPlaying)
		{
			active = true;
			SetupRefs();
		}
	}

	private new void OnEnable()
	{
		if (Application.isPlaying)
		{
			if (!active)
			{
				Start();
			}
			GetBinding();
			Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		ClearSwapCache();
		Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			ShowCurrentBinding();
		}
	}

	public void GetBinding()
	{
		currentBinding = ih.GetKeyBindingForAction(playerAction);
	}

	private void ClearSwapCache()
	{
		bindingToSwap = null;
		actionToSwap = null;
	}

	public void ListenForNewButton()
	{
		oldBindings.Clear();
		oldBindings.AddRange(playerAction.Bindings);
		playerAction.ClearBindings();
		oldFontSize = keymapText.fontSize;
		oldAlignment = keymapText.alignment;
		oldSprite = keymapSprite.sprite;
		oldText = keymapText.text;
		keymapSprite.sprite = uibs.blankKey;
		keymapText.text = Language.Get("KEYBOARD_PRESSKEY", "MainMenu");
		keymapText.fontSize = 46;
		keymapText.alignment = TextAnchor.MiddleRight;
		keymapText.horizontalOverflow = HorizontalWrapMode.Overflow;
		base.interactable = false;
		ClearSwapCache();
		SetupBindingListenOptions();
		isListening = true;
		uibs.ListeningForKeyRebind(this);
		playerAction.ListenForBinding();
	}

	public void ShowCurrentBinding()
	{
		if (!active)
		{
			Start();
		}
		if (InputHandler.KeyOrMouseBinding.IsNone(currentBinding))
		{
			keymapSprite.sprite = uibs.blankKey;
			keymapText.text = Language.Get("KEYBOARD_UNMAPPED", "MainMenu");
			keymapText.fontSize = 46;
			keymapText.alignment = TextAnchor.MiddleRight;
			keymapText.resizeTextForBestFit = false;
			keymapText.horizontalOverflow = HorizontalWrapMode.Overflow;
			keymapText.GetComponent<FixVerticalAlign>().AlignText();
		}
		else
		{
			ButtonSkin keyboardSkinFor = uibs.GetKeyboardSkinFor(playerAction);
			keymapSprite.sprite = keyboardSkinFor.sprite;
			keymapText.text = keyboardSkinFor.symbol;
			if (keyboardSkinFor.skinType == ButtonSkinType.SQUARE)
			{
				keymapText.fontSize = 46;
				keymapText.alignment = TextAnchor.MiddleCenter;
				keymapText.rectTransform.anchoredPosition = new Vector2(32f, keymapText.rectTransform.anchoredPosition.y);
				keymapText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 65f);
				keymapText.resizeTextForBestFit = true;
				keymapText.resizeTextMinSize = 20;
				keymapText.resizeTextMaxSize = 46;
				keymapText.horizontalOverflow = HorizontalWrapMode.Wrap;
			}
			else if (keyboardSkinFor.skinType == ButtonSkinType.WIDE)
			{
				keymapText.fontSize = 40;
				keymapText.alignment = TextAnchor.MiddleCenter;
				keymapText.rectTransform.anchoredPosition = new Vector2(4f, keymapText.rectTransform.anchoredPosition.y);
				keymapText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 137f);
				keymapText.resizeTextForBestFit = false;
				keymapText.horizontalOverflow = HorizontalWrapMode.Wrap;
			}
			else
			{
				keymapText.alignment = uibs.labelAlignment;
			}
			if (keymapSprite.sprite == null)
			{
				keymapSprite.sprite = uibs.blankKey;
			}
			keymapText.GetComponent<FixVerticalAlign>().AlignTextKeymap();
		}
		base.interactable = true;
	}

	public void AbortRebind()
	{
		if (!isListening)
		{
			return;
		}
		foreach (BindingSource oldBinding in oldBindings)
		{
			playerAction.AddBinding(oldBinding);
		}
		oldBindings.Clear();
		keymapText.text = oldText;
		keymapText.fontSize = oldFontSize;
		keymapText.alignment = oldAlignment;
		keymapSprite.sprite = oldSprite;
		keymapText.GetComponent<FixVerticalAlign>().AlignTextKeymap();
		base.interactable = true;
		isListening = false;
		ClearSwapCache();
	}

	public void StopActionListening()
	{
		playerAction.StopListeningForBinding();
		ClearSwapCache();
	}

	public bool OnBindingFound(PlayerAction action, BindingSource binding)
	{
		if (!(binding is MouseBindingSource) && (!(binding is KeyBindingSource item) || unmappableKeys.Contains(item)))
		{
			uibs.FinishedListeningForKey();
			action.StopListeningForBinding();
			AbortRebind();
			return false;
		}
		if (binding != null)
		{
			foreach (PlayerAction mappableKeyboardAction in ih.MappableKeyboardActions)
			{
				if (mappableKeyboardAction != playerAction && mappableKeyboardAction.Bindings.Contains(binding))
				{
					actionToSwap = mappableKeyboardAction;
					break;
				}
			}
			if (actionToSwap != null)
			{
				foreach (BindingSource oldBinding in oldBindings)
				{
					if (oldBinding.BindingSourceType == binding.BindingSourceType || oldBinding.BindingSourceType == BindingSourceType.KeyBindingSource || oldBinding.BindingSourceType == BindingSourceType.MouseBindingSource)
					{
						bindingToSwap = oldBinding;
						break;
					}
				}
				if (bindingToSwap == null)
				{
					foreach (BindingSource binding2 in action.Bindings)
					{
						if (binding2.BindingSourceType == binding.BindingSourceType || binding2.BindingSourceType == BindingSourceType.KeyBindingSource || binding2.BindingSourceType == BindingSourceType.MouseBindingSource)
						{
							bindingToSwap = binding2;
							break;
						}
					}
				}
			}
		}
		return true;
	}

	public void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		oldBindings.RemoveAll((BindingSource o) => o == null || o.DeviceClass == InputDeviceClass.Keyboard || o.DeviceClass == InputDeviceClass.Mouse);
		foreach (BindingSource oldBinding in oldBindings)
		{
			playerAction.AddBinding(oldBinding);
		}
		oldBindings.Clear();
		if (actionToSwap != null && bindingToSwap != null)
		{
			actionToSwap.AddBinding(bindingToSwap);
		}
		isListening = false;
		base.interactable = true;
		uibs.FinishedListeningForKey();
		ClearSwapCache();
	}

	public void OnBindingRejected(PlayerAction action, BindingSource binding, BindingSourceRejectionType rejection)
	{
		switch (rejection)
		{
		case BindingSourceRejectionType.DuplicateBindingOnAction:
			uibs.FinishedListeningForKey();
			AbortRebind();
			action.StopListeningForBinding();
			isListening = false;
			break;
		default:
			uibs.FinishedListeningForKey();
			AbortRebind();
			action.StopListeningForBinding();
			isListening = false;
			break;
		case BindingSourceRejectionType.DuplicateBindingOnActionSet:
			break;
		}
		ClearSwapCache();
	}

	public new void OnSubmit(BaseEventData eventData)
	{
		if (!isListening)
		{
			ListenForNewButton();
		}
	}

	public new void OnPointerClick(PointerEventData eventData)
	{
		OnSubmit(eventData);
	}

	public new void OnCancel(BaseEventData eventData)
	{
		if (isListening)
		{
			StopListeningForNewKey();
		}
		else
		{
			base.OnCancel(eventData);
		}
	}

	private void StopListeningForNewKey()
	{
		uibs.FinishedListeningForKey();
		StopActionListening();
		AbortRebind();
	}

	private void SetupUnmappableKeys()
	{
		unmappableKeys = new List<KeyBindingSource>();
		unmappableKeys.Add(new KeyBindingSource(Key.Escape));
		unmappableKeys.Add(new KeyBindingSource(Key.Return));
		unmappableKeys.Add(new KeyBindingSource(Key.Numlock));
		unmappableKeys.Add(new KeyBindingSource(Key.LeftCommand));
		unmappableKeys.Add(new KeyBindingSource(Key.RightCommand));
	}

	private void SetupBindingListenOptions()
	{
		BindingListenOptions bindingListenOptions = new BindingListenOptions();
		bindingListenOptions.IncludeControllers = true;
		bindingListenOptions.IncludeNonStandardControls = false;
		bindingListenOptions.IncludeMouseButtons = true;
		bindingListenOptions.IncludeKeys = true;
		bindingListenOptions.IncludeModifiersAsFirstClassKeys = true;
		bindingListenOptions.IncludeUnknownControllers = false;
		bindingListenOptions.MaxAllowedBindingsPerType = 1u;
		bindingListenOptions.OnBindingFound = OnBindingFound;
		bindingListenOptions.OnBindingAdded = OnBindingAdded;
		bindingListenOptions.OnBindingRejected = OnBindingRejected;
		bindingListenOptions.UnsetDuplicateBindingsOnSet = true;
		ih.inputActions.ListenOptions = bindingListenOptions;
	}

	private void SetupRefs()
	{
		gm = GameManager.instance;
		ui = gm.ui;
		uibs = ui.uiButtonSkins;
		ih = gm.inputHandler;
		playerAction = ih.ActionButtonToPlayerAction(actionButtonType);
		HookUpAudioPlayer();
		HookUpEventTrigger();
		SetupUnmappableKeys();
	}
}
