using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using SharpDX.DirectInput;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GameManager))]
public class InputHandler : ManagerSingleton<InputHandler>
{
	public readonly struct KeyOrMouseBinding
	{
		public readonly InControl.Key Key;

		public readonly InControl.Mouse Mouse;

		public KeyOrMouseBinding(InControl.Key key)
		{
			Key = key;
			Mouse = InControl.Mouse.None;
		}

		public KeyOrMouseBinding(InControl.Mouse mouse)
		{
			Key = InControl.Key.None;
			Mouse = mouse;
		}

		public static bool IsNone(KeyOrMouseBinding val)
		{
			if (val.Key == InControl.Key.None)
			{
				return val.Mouse == InControl.Mouse.None;
			}
			return false;
		}

		public override string ToString()
		{
			if (Mouse == InControl.Mouse.None)
			{
				return Key.ToString();
			}
			return Mouse.ToString();
		}
	}

	public delegate void CursorVisibilityChange(bool isVisible);

	public delegate void ActiveControllerSwitch();

	private const float BUTTON_QUEUE_TIME = 0.1f;

	private const float STAG_LOCKOUT_DURATION = 1.2f;

	private GameManager gm;

	private GameSettings gs;

	public InputDevice gameController;

	public HeroActions inputActions;

	public BindingSourceType lastActiveController;

	public InputDeviceStyle lastInputDeviceStyle;

	public GamepadType activeGamepadType;

	public GamepadState gamepadState;

	private HeroController heroCtrl;

	private PlayerData playerData;

	public bool acceptingInput;

	public bool skippingCutscene;

	private bool readyToSkipCutscene;

	private bool controllerDetected;

	private ControllerProfile currentControllerProfile;

	private bool isTitleScreenScene;

	private bool isMenuScene;

	private bool isStagTravelScene;

	private bool stagLockoutActive;

	private double skipCooldownTime;

	private bool controllerPressed;

	private float[] buttonQueueTimers;

	private bool hasSetup;

	private bool hasAwaken;

	private bool hasStarted;

	[NonSerialized]
	private bool doingSoftReset;

	public GamepadType ActiveGamepadAlias { get; set; }

	public List<PlayerAction> MappableControllerActions { get; set; }

	public List<PlayerAction> MappableKeyboardActions { get; set; }

	public bool PauseAllowed { get; private set; }

	public SkipPromptMode SkipMode { get; private set; }

	public bool WasSkipButtonPressed
	{
		get
		{
			HeroActions heroActions = inputActions;
			if (Platform.Current.GetMenuAction(heroActions) == Platform.MenuActions.None && !heroActions.Pause.WasPressed && !InventoryPaneInput.IsInventoryButtonPressed(heroActions) && !heroActions.QuickCast.WasPressed && !heroActions.SuperDash.WasPressed && !heroActions.Dash.WasPressed)
			{
				return heroActions.QuickMap.WasPressed;
			}
			return true;
		}
	}

	public bool ForceDreamNailRePress { get; set; }

	public static event Action<HeroActions> OnUpdateHeroActions;

	public event CursorVisibilityChange OnCursorVisibilityChange;

	public event ActiveControllerSwitch RefreshActiveControllerEvent;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		gm = GetComponent<GameManager>();
		gs = gm.gameSettings;
		inputActions = new HeroActions();
		acceptingInput = true;
		PauseAllowed = true;
		SkipMode = SkipPromptMode.NOT_SKIPPABLE;
		SaveDataUpgradeHandler.UpgradeSystemData(this);
		buttonQueueTimers = new float[ArrayForEnumAttribute.GetArrayLength(typeof(HeroActionButton))];
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		playerData = gm.playerData;
		if (!InputManager.IsSetup)
		{
			InputManager.OnSetupCompleted += Setup;
		}
		else
		{
			Setup();
		}
		InputManager.OnDeviceAttached += ControllerAttached;
		InputManager.OnActiveDeviceChanged += ControllerActivated;
		InputManager.OnDeviceDetached += ControllerDetached;
		Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
		return true;
		void Setup()
		{
			InputManager.OnSetupCompleted -= Setup;
			doingSoftReset = false;
			SetupNonMappableBindings();
			gs.LoadKeyboardSettings();
			MapKeyboardLayoutFromGameSettings();
			if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.IsAttached)
			{
				ControllerActivated(InputManager.ActiveDevice);
			}
			else
			{
				gameController = InputDevice.Null;
			}
			lastActiveController = BindingSourceType.None;
			SetupMappableKeyboardBindingsList();
			InputHandler.OnUpdateHeroActions?.Invoke(inputActions);
			hasSetup = true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnAwake();
	}

	public void Start()
	{
		OnStart();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		InputManager.OnDeviceAttached -= ControllerAttached;
		InputManager.OnActiveDeviceChanged -= ControllerActivated;
		InputManager.OnDeviceDetached -= ControllerDetached;
		Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
		inputActions.Destroy();
	}

	public void SceneInit()
	{
		isTitleScreenScene = gm.IsTitleScreenScene();
		isMenuScene = gm.IsMenuScene();
		if (gm.IsStagTravelScene())
		{
			isStagTravelScene = true;
			stagLockoutActive = true;
			Invoke("UnlockStagInput", 1.2f);
		}
		else
		{
			isStagTravelScene = false;
		}
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			LoadSavedInputBindings();
		}
	}

	private void LoadSavedInputBindings()
	{
		if (!hasSetup)
		{
			return;
		}
		try
		{
			doingSoftReset = true;
			foreach (PlayerAction action in inputActions.Actions)
			{
				action.ClearBindings();
			}
			SetupNonMappableBindings();
			gs.LoadKeyboardSettings();
			MapKeyboardLayoutFromGameSettings();
			if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.IsAttached)
			{
				ControllerActivated(InputManager.ActiveDevice);
			}
			SetupMappableKeyboardBindingsList();
		}
		finally
		{
			doingSoftReset = false;
			InputHandler.OnUpdateHeroActions?.Invoke(inputActions);
		}
	}

	private void SetCursorVisible(bool value)
	{
		SetCursorEnabled(value);
		if (this.OnCursorVisibilityChange != null)
		{
			this.OnCursorVisibilityChange(value);
		}
	}

	private static void SetCursorEnabled(bool isEnabled)
	{
		if (isEnabled && Platform.Current.IsMouseSupported && !DemoHelper.IsExhibitionMode)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void Update()
	{
		if (isTitleScreenScene)
		{
			SetCursorVisible(value: false);
		}
		else if (!isMenuScene)
		{
			if (!gm.isPaused)
			{
				SetCursorVisible(value: false);
			}
			else
			{
				SetCursorVisible(!controllerPressed);
			}
		}
		else
		{
			SetCursorVisible(!controllerPressed);
		}
		UpdateActiveController();
		UpdateButtonQueueing();
		if (acceptingInput)
		{
			if (gm.GameState == GameState.PLAYING)
			{
				PlayingInput();
			}
			else if (gm.GameState == GameState.CUTSCENE)
			{
				if (isStagTravelScene)
				{
					if (!stagLockoutActive)
					{
						StagCutsceneInput();
					}
				}
				else
				{
					CutsceneInput();
				}
			}
			if (inputActions.Pause.WasPressed && PauseAllowed && !playerData.disablePause)
			{
				GameState gameState = gm.GameState;
				if (gameState == GameState.PLAYING || gameState == GameState.PAUSED)
				{
					StartCoroutine(gm.PauseGameToggle(playSound: true));
				}
			}
		}
		if (controllerPressed)
		{
			if (Mathf.Abs(Input.GetAxisRaw("mouse x")) > 0.1f)
			{
				controllerPressed = false;
			}
		}
		else if (inputActions.ActiveDevice.AnyButtonIsPressed || inputActions.MoveVector.WasPressed)
		{
			controllerPressed = true;
		}
	}

	private void UpdateButtonQueueing()
	{
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < buttonQueueTimers.Length; i++)
		{
			HeroActionButton actionButtonType = (HeroActionButton)i;
			if (ActionButtonToPlayerAction(actionButtonType).WasPressed)
			{
				buttonQueueTimers[i] = 0.1f;
				continue;
			}
			float num = buttonQueueTimers[i];
			if (!(num <= 0f))
			{
				num -= Time.unscaledDeltaTime;
				buttonQueueTimers[i] = num;
			}
		}
	}

	public bool GetWasButtonPressedQueued(HeroActionButton heroAction, bool consume)
	{
		if (buttonQueueTimers[(int)heroAction] <= 0f)
		{
			return false;
		}
		if (consume)
		{
			buttonQueueTimers[(int)heroAction] = 0f;
		}
		return true;
	}

	private void ControllerAttached(InputDevice inputDevice)
	{
		if (!inputDevice.IsUnknown)
		{
			gamepadState = GamepadState.ATTACHED;
			gameController = inputDevice;
			Debug.LogFormat("Game controller {0} attached", inputDevice.Name);
			SetActiveGamepadType(inputDevice);
		}
	}

	private void ControllerActivated(InputDevice inputDevice)
	{
		if (!inputDevice.IsUnknown)
		{
			gamepadState = GamepadState.ACTIVATED;
			gameController = inputDevice;
			SetActiveGamepadType(inputDevice);
		}
	}

	private void ControllerDetached(InputDevice inputDevice)
	{
		if (gameController == inputDevice)
		{
			gamepadState = GamepadState.DETACHED;
			activeGamepadType = GamepadType.NONE;
			ActiveGamepadAlias = GamepadType.NONE;
			gameController = InputDevice.Null;
			Debug.LogFormat("Game controller {0} detached.", inputDevice.Name);
			UIManager instance = UIManager.instance;
			if (instance.uiButtonSkins.listeningButton != null)
			{
				instance.uiButtonSkins.listeningButton.StopActionListening();
				instance.uiButtonSkins.listeningButton.AbortRebind();
				instance.uiButtonSkins.RefreshButtonMappings();
			}
		}
	}

	private void PlayingInput()
	{
		if (!CheatManager.IsOpen && ForceDreamNailRePress && !inputActions.DreamNail.IsPressed)
		{
			ForceDreamNailRePress = false;
		}
	}

	private void CutsceneInput()
	{
		if (skippingCutscene || (!Input.anyKeyDown && (gameController == null || !gameController.AnyButton.WasPressed)))
		{
			return;
		}
		switch (SkipMode)
		{
		case SkipPromptMode.SKIP_INSTANT:
			skippingCutscene = true;
			gm.SkipCutscene();
			break;
		case SkipPromptMode.SKIP_PROMPT:
			if (!readyToSkipCutscene)
			{
				gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Skip);
				readyToSkipCutscene = true;
				CancelInvoke("StopCutsceneInput");
				Invoke("StopCutsceneInput", 3f * Time.timeScale);
				skipCooldownTime = Time.timeAsDouble + 0.30000001192092896;
			}
			else if (!(Time.timeAsDouble < skipCooldownTime))
			{
				CancelInvoke("StopCutsceneInput");
				readyToSkipCutscene = false;
				skippingCutscene = true;
				gm.SkipCutscene();
			}
			break;
		case SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING:
			gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Loading);
			CancelInvoke("StopCutsceneInput");
			Invoke("StopCutsceneInput", 3f * Time.timeScale);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case SkipPromptMode.NOT_SKIPPABLE:
			break;
		}
	}

	private void StagCutsceneInput()
	{
		if (Input.anyKeyDown || gameController.AnyButton.WasPressed)
		{
			gm.SkipCutscene();
		}
	}

	public void AttachHeroController(HeroController heroController)
	{
		heroCtrl = heroController;
	}

	public void StopAcceptingInput()
	{
		acceptingInput = false;
	}

	public void StartAcceptingInput()
	{
		acceptingInput = true;
	}

	public void PreventPause()
	{
		PauseAllowed = false;
	}

	public void AllowPause()
	{
		PauseAllowed = true;
	}

	public void UpdateActiveController()
	{
		if (lastActiveController != inputActions.LastInputType || lastInputDeviceStyle != inputActions.LastDeviceStyle)
		{
			lastActiveController = inputActions.LastInputType;
			lastInputDeviceStyle = inputActions.LastDeviceStyle;
			if (this.RefreshActiveControllerEvent != null)
			{
				this.RefreshActiveControllerEvent();
			}
		}
	}

	public void StopUIInput()
	{
		acceptingInput = false;
		EventSystem.current.sendNavigationEvents = false;
		UIManager.instance.inputModule.allowMouseInput = false;
	}

	public void StartUIInput()
	{
		acceptingInput = true;
		EventSystem.current.sendNavigationEvents = true;
		UIManager.instance.inputModule.allowMouseInput = true;
	}

	public void StopMouseInput()
	{
		UIManager.instance.inputModule.allowMouseInput = false;
	}

	public void EnableMouseInput()
	{
		UIManager.instance.inputModule.allowMouseInput = true;
	}

	public void SetSkipMode(SkipPromptMode newMode)
	{
		switch (newMode)
		{
		case SkipPromptMode.NOT_SKIPPABLE:
			StopAcceptingInput();
			break;
		case SkipPromptMode.SKIP_PROMPT:
			readyToSkipCutscene = false;
			StartAcceptingInput();
			break;
		case SkipPromptMode.SKIP_INSTANT:
			StartAcceptingInput();
			break;
		case SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING:
			readyToSkipCutscene = false;
			StartAcceptingInput();
			break;
		}
		SkipMode = newMode;
	}

	public void RefreshPlayerData()
	{
		playerData = PlayerData.instance;
	}

	public void ResetDefaultKeyBindings()
	{
		inputActions.Jump.ClearBindings();
		inputActions.Attack.ClearBindings();
		inputActions.Dash.ClearBindings();
		inputActions.Cast.ClearBindings();
		inputActions.SuperDash.ClearBindings();
		inputActions.DreamNail.ClearBindings();
		inputActions.QuickMap.ClearBindings();
		inputActions.OpenInventory.ClearBindings();
		inputActions.OpenInventoryMap.ClearBindings();
		inputActions.OpenInventoryJournal.ClearBindings();
		inputActions.OpenInventoryTools.ClearBindings();
		inputActions.OpenInventoryQuests.ClearBindings();
		inputActions.QuickCast.ClearBindings();
		inputActions.Up.ClearBindings();
		inputActions.Down.ClearBindings();
		inputActions.Left.ClearBindings();
		inputActions.Right.ClearBindings();
		inputActions.Taunt.ClearBindings();
		MapDefaultKeyboardLayout();
		gs.jumpKey = InControl.Key.Z.ToString();
		gs.attackKey = InControl.Key.X.ToString();
		gs.dashKey = InControl.Key.C.ToString();
		gs.castKey = InControl.Key.A.ToString();
		gs.superDashKey = InControl.Key.S.ToString();
		gs.dreamNailKey = InControl.Key.D.ToString();
		gs.quickMapKey = InControl.Key.Tab.ToString();
		gs.inventoryKey = InControl.Key.I.ToString();
		gs.inventoryMapKey = InControl.Key.M.ToString();
		gs.inventoryToolsKey = InControl.Key.Q.ToString();
		gs.inventoryJournalKey = InControl.Key.J.ToString();
		gs.inventoryQuestsKey = InControl.Key.T.ToString();
		gs.quickCastKey = InControl.Key.F.ToString();
		gs.tauntKey = InControl.Key.V.ToString();
		gs.upKey = InControl.Key.UpArrow.ToString();
		gs.downKey = InControl.Key.DownArrow.ToString();
		gs.leftKey = InControl.Key.LeftArrow.ToString();
		gs.rightKey = InControl.Key.RightArrow.ToString();
		gs.SaveKeyboardSettings();
		if (gameController != InputDevice.Null)
		{
			SetActiveGamepadType(gameController);
		}
	}

	public void ResetDefaultControllerButtonBindings()
	{
		inputActions.Jump.ClearBindings();
		inputActions.Attack.ClearBindings();
		inputActions.Dash.ClearBindings();
		inputActions.Cast.ClearBindings();
		inputActions.SuperDash.ClearBindings();
		inputActions.DreamNail.ClearBindings();
		inputActions.QuickMap.ClearBindings();
		inputActions.QuickCast.ClearBindings();
		inputActions.Taunt.ClearBindings();
		MapKeyboardLayoutFromGameSettings();
		gs.ResetGamepadSettings(activeGamepadType);
		gs.SaveGamepadSettings(activeGamepadType);
		MapControllerButtons(activeGamepadType);
	}

	public void ResetAllControllerButtonBindings()
	{
		int num = Enum.GetNames(typeof(GamepadType)).Length;
		for (int i = 0; i < num; i++)
		{
			GamepadType gamepadType = (GamepadType)i;
			if (gs.LoadGamepadSettings(gamepadType))
			{
				gs.ResetGamepadSettings(gamepadType);
				gs.SaveGamepadSettings(gamepadType);
			}
		}
	}

	public void SendKeyBindingsToGameSettings()
	{
		gs.jumpKey = GetKeyBindingForAction(inputActions.Jump).ToString();
		gs.attackKey = GetKeyBindingForAction(inputActions.Attack).ToString();
		gs.dashKey = GetKeyBindingForAction(inputActions.Dash).ToString();
		gs.castKey = GetKeyBindingForAction(inputActions.Cast).ToString();
		gs.superDashKey = GetKeyBindingForAction(inputActions.SuperDash).ToString();
		gs.dreamNailKey = GetKeyBindingForAction(inputActions.DreamNail).ToString();
		gs.quickMapKey = GetKeyBindingForAction(inputActions.QuickMap).ToString();
		gs.inventoryKey = GetKeyBindingForAction(inputActions.OpenInventory).ToString();
		gs.inventoryMapKey = GetKeyBindingForAction(inputActions.OpenInventoryMap).ToString();
		gs.inventoryJournalKey = GetKeyBindingForAction(inputActions.OpenInventoryJournal).ToString();
		gs.inventoryToolsKey = GetKeyBindingForAction(inputActions.OpenInventoryTools).ToString();
		gs.inventoryQuestsKey = GetKeyBindingForAction(inputActions.OpenInventoryQuests).ToString();
		gs.upKey = GetKeyBindingForAction(inputActions.Up).ToString();
		gs.downKey = GetKeyBindingForAction(inputActions.Down).ToString();
		gs.leftKey = GetKeyBindingForAction(inputActions.Left).ToString();
		gs.rightKey = GetKeyBindingForAction(inputActions.Right).ToString();
		gs.quickCastKey = GetKeyBindingForAction(inputActions.QuickCast).ToString();
		gs.tauntKey = GetKeyBindingForAction(inputActions.Taunt).ToString();
	}

	public void SendButtonBindingsToGameSettings()
	{
		gs.controllerMapping.jump = GetButtonBindingForAction(inputActions.Jump);
		gs.controllerMapping.attack = GetButtonBindingForAction(inputActions.Attack);
		gs.controllerMapping.dash = GetButtonBindingForAction(inputActions.Dash);
		gs.controllerMapping.cast = GetButtonBindingForAction(inputActions.Cast);
		gs.controllerMapping.superDash = GetButtonBindingForAction(inputActions.SuperDash);
		gs.controllerMapping.dreamNail = GetButtonBindingForAction(inputActions.DreamNail);
		gs.controllerMapping.quickMap = GetButtonBindingForAction(inputActions.QuickMap);
		gs.controllerMapping.quickCast = GetButtonBindingForAction(inputActions.QuickCast);
		gs.controllerMapping.taunt = GetButtonBindingForAction(inputActions.Taunt);
	}

	public void MapControllerButtons(GamepadType gamePadType)
	{
		inputActions.Reset();
		MapKeyboardLayoutFromGameSettings();
		if (!gs.LoadGamepadSettings(gamePadType))
		{
			gs.ResetGamepadSettings(gamePadType);
		}
		inputActions.Jump.AddBinding(new DeviceBindingSource(gs.controllerMapping.jump));
		inputActions.Attack.AddBinding(new DeviceBindingSource(gs.controllerMapping.attack));
		inputActions.Dash.AddBinding(new DeviceBindingSource(gs.controllerMapping.dash));
		inputActions.Cast.AddBinding(new DeviceBindingSource(gs.controllerMapping.cast));
		inputActions.SuperDash.AddBinding(new DeviceBindingSource(gs.controllerMapping.superDash));
		inputActions.DreamNail.AddBinding(new DeviceBindingSource(gs.controllerMapping.dreamNail));
		inputActions.QuickMap.AddBinding(new DeviceBindingSource(gs.controllerMapping.quickMap));
		inputActions.QuickCast.AddBinding(new DeviceBindingSource(gs.controllerMapping.quickCast));
		inputActions.Taunt.AddBinding(new DeviceBindingSource(gs.controllerMapping.taunt));
		switch (gamePadType)
		{
		case GamepadType.XBOX_360:
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Back));
			return;
		case GamepadType.PS4:
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.TouchPadButton));
			inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Options));
			inputActions.SwipeInventoryMap.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Up));
			inputActions.SwipeInventoryJournal.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Down));
			inputActions.SwipeInventoryTools.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Left));
			inputActions.SwipeInventoryQuests.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Right));
			return;
		case GamepadType.PS5:
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.TouchPadButton));
			inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Options));
			inputActions.SwipeInventoryMap.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Up));
			inputActions.SwipeInventoryJournal.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Down));
			inputActions.SwipeInventoryTools.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Left));
			inputActions.SwipeInventoryQuests.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Right));
			return;
		}
		GamepadType gamepadType = activeGamepadType;
		if (gamepadType == GamepadType.XBOX_ONE || gamepadType == GamepadType.XBOX_SERIES_X)
		{
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.View));
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Back));
			inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Menu));
			return;
		}
		if (gamePadType == GamepadType.PS3_WIN)
		{
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Select));
			return;
		}
		gamepadType = activeGamepadType;
		if (gamepadType == GamepadType.SWITCH_JOYCON_DUAL || gamepadType == GamepadType.SWITCH_PRO_CONTROLLER || gamepadType == GamepadType.SWITCH2_JOYCON_DUAL || gamepadType == GamepadType.SWITCH2_PRO_CONTROLLER)
		{
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Minus));
			inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Plus));
		}
		else if (gamePadType == GamepadType.UNKNOWN)
		{
			inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Select));
		}
	}

	public void RemapUiButtons()
	{
		inputActions.MenuSubmit.ResetBindings();
		inputActions.MenuCancel.ResetBindings();
		inputActions.MenuExtra.ResetBindings();
		inputActions.MenuSuper.ResetBindings();
		inputActions.PaneLeft.ResetBindings();
		inputActions.PaneRight.ResetBindings();
	}

	public PlayerAction ActionButtonToPlayerAction(HeroActionButton actionButtonType)
	{
		OnStart();
		switch (actionButtonType)
		{
		case HeroActionButton.JUMP:
			return inputActions.Jump;
		case HeroActionButton.ATTACK:
			return inputActions.Attack;
		case HeroActionButton.DASH:
			return inputActions.Dash;
		case HeroActionButton.CAST:
			return inputActions.Cast;
		case HeroActionButton.SUPER_DASH:
			return inputActions.SuperDash;
		case HeroActionButton.QUICK_MAP:
			return inputActions.QuickMap;
		case HeroActionButton.QUICK_CAST:
			return inputActions.QuickCast;
		case HeroActionButton.TAUNT:
			return inputActions.Taunt;
		case HeroActionButton.INVENTORY:
			return inputActions.OpenInventory;
		case HeroActionButton.INVENTORY_MAP:
			return inputActions.OpenInventoryMap;
		case HeroActionButton.INVENTORY_JOURNAL:
			return inputActions.OpenInventoryJournal;
		case HeroActionButton.INVENTORY_TOOLS:
			return inputActions.OpenInventoryTools;
		case HeroActionButton.INVENTORY_QUESTS:
			return inputActions.OpenInventoryQuests;
		case HeroActionButton.DREAM_NAIL:
			return inputActions.DreamNail;
		case HeroActionButton.UP:
			return inputActions.Up;
		case HeroActionButton.DOWN:
			return inputActions.Down;
		case HeroActionButton.LEFT:
			return inputActions.Left;
		case HeroActionButton.RIGHT:
			return inputActions.Right;
		case HeroActionButton.MENU_SUBMIT:
			return inputActions.MenuSubmit;
		case HeroActionButton.MENU_CANCEL:
			return inputActions.MenuCancel;
		case HeroActionButton.MENU_PANE_LEFT:
			return inputActions.PaneLeft;
		case HeroActionButton.MENU_PANE_RIGHT:
			return inputActions.PaneRight;
		case HeroActionButton.MENU_EXTRA:
			return inputActions.MenuExtra;
		case HeroActionButton.MENU_SUPER:
			return inputActions.MenuSuper;
		default:
			Debug.Log("No PlayerAction could be matched to HeroActionButton: " + actionButtonType);
			return null;
		}
	}

	public KeyOrMouseBinding GetKeyBindingForAction(PlayerAction action)
	{
		if (!inputActions.Actions.Contains(action))
		{
			return new KeyOrMouseBinding(InControl.Key.None);
		}
		int count = action.Bindings.Count;
		if (count == 0)
		{
			return new KeyOrMouseBinding(InControl.Key.None);
		}
		if (count == 1)
		{
			BindingSource bindingSource = action.Bindings[0];
			if (bindingSource.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource.BindingSourceType == BindingSourceType.MouseBindingSource)
			{
				return GetKeyBindingForActionBinding(action, action.Bindings[0]);
			}
			return new KeyOrMouseBinding(InControl.Key.None);
		}
		if (count > 1)
		{
			foreach (BindingSource binding in action.Bindings)
			{
				if (binding.BindingSourceType == BindingSourceType.KeyBindingSource || binding.BindingSourceType == BindingSourceType.MouseBindingSource)
				{
					KeyOrMouseBinding keyBindingForActionBinding = GetKeyBindingForActionBinding(action, binding);
					if (!KeyOrMouseBinding.IsNone(keyBindingForActionBinding))
					{
						return keyBindingForActionBinding;
					}
				}
			}
			return new KeyOrMouseBinding(InControl.Key.None);
		}
		return new KeyOrMouseBinding(InControl.Key.None);
	}

	private KeyOrMouseBinding GetKeyBindingForActionBinding(PlayerAction action, BindingSource bindingSource)
	{
		KeyBindingSource keyBindingSource = bindingSource as KeyBindingSource;
		if (keyBindingSource != null)
		{
			if (keyBindingSource.Control.IncludeCount == 0)
			{
				Debug.LogErrorFormat("This action has no key mapped but registered a key binding. ({0})", action.Name);
				return new KeyOrMouseBinding(InControl.Key.None);
			}
			if (keyBindingSource.Control.IncludeCount == 1)
			{
				return new KeyOrMouseBinding(keyBindingSource.Control.GetInclude(0));
			}
			_ = keyBindingSource.Control.IncludeCount;
			_ = 1;
			return new KeyOrMouseBinding(InControl.Key.None);
		}
		MouseBindingSource mouseBindingSource = bindingSource as MouseBindingSource;
		if (mouseBindingSource != null)
		{
			return new KeyOrMouseBinding(mouseBindingSource.Control);
		}
		Debug.LogErrorFormat("Keybinding Error - Action: {0} returned a null binding.", action.Name);
		return new KeyOrMouseBinding(InControl.Key.None);
	}

	public InputControlType GetButtonBindingForAction(PlayerAction action)
	{
		if (inputActions.Actions.Contains(action))
		{
			if (action.Bindings.Count > 0)
			{
				foreach (BindingSource binding in action.Bindings)
				{
					if (binding.BindingSourceType == BindingSourceType.DeviceBindingSource)
					{
						DeviceBindingSource deviceBindingSource = binding as DeviceBindingSource;
						if (deviceBindingSource != null)
						{
							return deviceBindingSource.Control;
						}
					}
				}
				return InputControlType.None;
			}
			return InputControlType.None;
		}
		return InputControlType.None;
	}

	public PlayerAction GetActionForMappableControllerButton(InputControlType button)
	{
		foreach (PlayerAction mappableControllerAction in MappableControllerActions)
		{
			if (GetButtonBindingForAction(mappableControllerAction) == button)
			{
				return mappableControllerAction;
			}
		}
		return null;
	}

	public PlayerAction GetActionForDefaultControllerButton(InputControlType button)
	{
		InputManager.ActiveDevice?.GetControl(button);
		return null;
	}

	public void PrintMappings(PlayerAction action)
	{
		if (inputActions.Actions.Contains(action))
		{
			foreach (BindingSource binding in action.Bindings)
			{
				if (binding.BindingSourceType == BindingSourceType.DeviceBindingSource)
				{
					DeviceBindingSource deviceBindingSource = (DeviceBindingSource)binding;
					Debug.LogFormat("{0} : {1} of type {2}", action.Name, deviceBindingSource.Control, binding.BindingSourceType);
				}
				else
				{
					Debug.LogFormat("{0} : {1} of type {2}", action.Name, binding.Name, binding.BindingSourceType);
				}
			}
			return;
		}
		Debug.Log("Action Not Found");
	}

	public string ActionButtonLocalizedKey(PlayerAction action)
	{
		return ActionButtonLocalizedKey(action.Name);
	}

	public string ActionButtonLocalizedKey(string actionName)
	{
		switch (actionName)
		{
		case "Jump":
			return "BUTTON_JUMP";
		case "Attack":
			return "BUTTON_ATTACK";
		case "Dash":
			return "BUTTON_DASH";
		case "Cast":
			return "BUTTON_CAST";
		case "Super Dash":
			return "BUTTON_SUPER_DASH";
		case "Quick Map":
			return "BUTTON_MAP";
		case "Quick Cast":
			return "BUTTON_QCAST";
		case "Inventory":
			return "BUTTON_INVENTORY";
		case "Dream Nail":
			return "BUTTON_DREAM_NAIL";
		case "Move":
			return "BUTTON_MOVE";
		case "Look":
			return "BUTTON_LOOK";
		case "Pause":
			return "BUTTON_PAUSE";
		default:
			Debug.Log("IH Unknown Key for action: " + actionName);
			return "unknownkey";
		}
	}

	private void StopCutsceneInput()
	{
		gm.ui.HideCutscenePrompt(isInstant: false, delegate
		{
			readyToSkipCutscene = false;
		});
	}

	private void UnlockStagInput()
	{
		stagLockoutActive = false;
	}

	private IEnumerator SetupGamepadUIInputActions()
	{
		if (gm.ui.menuState == MainMenuState.GAMEPAD_MENU)
		{
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			yield return new WaitForEndOfFrame();
		}
		switch (Platform.Current.AcceptRejectInputStyle)
		{
		case Platform.AcceptRejectInputStyles.NonJapaneseStyle:
			inputActions.MenuSubmit.AddDefaultBinding(InputControlType.Action1);
			inputActions.MenuCancel.AddDefaultBinding(InputControlType.Action2);
			break;
		case Platform.AcceptRejectInputStyles.JapaneseStyle:
			inputActions.MenuSubmit.AddDefaultBinding(InputControlType.Action2);
			inputActions.MenuCancel.AddDefaultBinding(InputControlType.Action1);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		inputActions.MenuExtra.AddDefaultBinding(InputControlType.Action3);
		inputActions.MenuSuper.AddDefaultBinding(InputControlType.Action4);
	}

	private void RemoveGamepadUiInputActions()
	{
		inputActions.MenuSubmit.RemoveBinding(new DeviceBindingSource(InputControlType.Action1));
		inputActions.MenuSubmit.RemoveBinding(new DeviceBindingSource(InputControlType.Action2));
		inputActions.MenuCancel.RemoveBinding(new DeviceBindingSource(InputControlType.Action1));
		inputActions.MenuCancel.RemoveBinding(new DeviceBindingSource(InputControlType.Action2));
		inputActions.MenuExtra.RemoveBinding(new DeviceBindingSource(InputControlType.Action3));
		inputActions.MenuSuper.RemoveBinding(new DeviceBindingSource(InputControlType.Action4));
	}

	private void DestroyCurrentActionSet()
	{
		inputActions.Destroy();
	}

	public void SetActiveGamepadType(InputDevice inputDevice)
	{
		ActiveGamepadAlias = GamepadType.NONE;
		if (gamepadState == GamepadState.DETACHED)
		{
			return;
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
		{
			switch (inputDevice.DeviceStyle)
			{
			case InputDeviceStyle.Xbox360:
				activeGamepadType = GamepadType.XBOX_360;
				break;
			case InputDeviceStyle.XboxOne:
			{
				GamepadType gamepadType = GamepadType.XBOX_ONE;
				if (InputManager.EnableXInput || InputManager.NativeInputEnableXInput)
				{
					int num = InputManager.Devices.IndexOf(inputDevice);
					List<DeviceInstance> devices = DirectInputManager.GetDevices();
					if (devices.Count > 0 && devices[num % devices.Count].InstanceName.Contains("xbox 360", StringComparison.InvariantCultureIgnoreCase))
					{
						ActiveGamepadAlias = GamepadType.XBOX_360;
					}
				}
				activeGamepadType = gamepadType;
				break;
			}
			case InputDeviceStyle.XboxSeriesX:
				activeGamepadType = GamepadType.XBOX_SERIES_X;
				break;
			case InputDeviceStyle.PlayStation3:
				activeGamepadType = GamepadType.PS3_WIN;
				break;
			case InputDeviceStyle.PlayStation4:
				activeGamepadType = GamepadType.PS4;
				break;
			case InputDeviceStyle.PlayStation5:
				activeGamepadType = GamepadType.PS5;
				break;
			case InputDeviceStyle.NintendoSwitch:
				activeGamepadType = GamepadType.SWITCH_PRO_CONTROLLER;
				break;
			default:
				Debug.LogError("Unable to match controller of name (" + inputDevice.Name + "), will attempt default mapping set." + inputDevice.DeviceStyle);
				activeGamepadType = GamepadType.XBOX_360;
				break;
			}
		}
		else
		{
			Debug.LogError("Unsupported platform for InputHander " + Application.platform);
			activeGamepadType = GamepadType.XBOX_360;
		}
		MapControllerButtons(activeGamepadType);
		UpdateActiveController();
		SetupMappableControllerBindingsList();
		StartCoroutine(SetupGamepadUIInputActions());
	}

	private void MapDefaultKeyboardLayout()
	{
		inputActions.Jump.AddBinding(new KeyBindingSource(InControl.Key.Z));
		inputActions.Attack.AddBinding(new KeyBindingSource(InControl.Key.X));
		inputActions.Dash.AddBinding(new KeyBindingSource(InControl.Key.C));
		inputActions.Cast.AddBinding(new KeyBindingSource(InControl.Key.A));
		inputActions.SuperDash.AddBinding(new KeyBindingSource(InControl.Key.S));
		inputActions.DreamNail.AddBinding(new KeyBindingSource(InControl.Key.D));
		inputActions.QuickMap.AddBinding(new KeyBindingSource(InControl.Key.Tab));
		inputActions.OpenInventory.AddBinding(new KeyBindingSource(InControl.Key.I));
		inputActions.OpenInventoryMap.AddBinding(new KeyBindingSource(InControl.Key.M));
		inputActions.OpenInventoryJournal.AddBinding(new KeyBindingSource(InControl.Key.J));
		inputActions.OpenInventoryTools.AddBinding(new KeyBindingSource(InControl.Key.Q));
		inputActions.OpenInventoryQuests.AddBinding(new KeyBindingSource(InControl.Key.T));
		inputActions.QuickCast.AddBinding(new KeyBindingSource(InControl.Key.F));
		inputActions.Taunt.AddBinding(new KeyBindingSource(InControl.Key.V));
		inputActions.Up.AddBinding(new KeyBindingSource(InControl.Key.UpArrow));
		inputActions.Down.AddBinding(new KeyBindingSource(InControl.Key.DownArrow));
		inputActions.Left.AddBinding(new KeyBindingSource(InControl.Key.LeftArrow));
		inputActions.Right.AddBinding(new KeyBindingSource(InControl.Key.RightArrow));
	}

	private void MapKeyboardLayoutFromGameSettings()
	{
		AddKeyBinding(inputActions.Jump, gs.jumpKey);
		AddKeyBinding(inputActions.Attack, gs.attackKey);
		AddKeyBinding(inputActions.Dash, gs.dashKey);
		AddKeyBinding(inputActions.Cast, gs.castKey);
		AddKeyBinding(inputActions.SuperDash, gs.superDashKey);
		AddKeyBinding(inputActions.DreamNail, gs.dreamNailKey);
		AddKeyBinding(inputActions.QuickMap, gs.quickMapKey);
		AddKeyBinding(inputActions.OpenInventory, gs.inventoryKey);
		AddKeyBinding(inputActions.OpenInventoryMap, gs.inventoryMapKey);
		AddKeyBinding(inputActions.OpenInventoryJournal, gs.inventoryJournalKey);
		AddKeyBinding(inputActions.OpenInventoryTools, gs.inventoryToolsKey);
		AddKeyBinding(inputActions.OpenInventoryQuests, gs.inventoryQuestsKey);
		AddKeyBinding(inputActions.QuickCast, gs.quickCastKey);
		AddKeyBinding(inputActions.Taunt, gs.tauntKey);
		AddKeyBinding(inputActions.Up, gs.upKey);
		AddKeyBinding(inputActions.Down, gs.downKey);
		AddKeyBinding(inputActions.Left, gs.leftKey);
		AddKeyBinding(inputActions.Right, gs.rightKey);
	}

	private static void AddKeyBinding(PlayerAction action, string savedBinding)
	{
		InControl.Mouse result = InControl.Mouse.None;
		if (Enum.TryParse<InControl.Key>(savedBinding, out var result2) || Enum.TryParse<InControl.Mouse>(savedBinding, out result))
		{
			if (result != 0)
			{
				action.AddBinding(new MouseBindingSource(result));
				return;
			}
			action.AddBinding(new KeyBindingSource(result2));
		}
	}

	private void SetupNonMappableBindings()
	{
		if (!doingSoftReset)
		{
			inputActions = new HeroActions();
		}
		inputActions.MenuSubmit.AddDefaultBinding(InControl.Key.Return);
		inputActions.MenuCancel.AddDefaultBinding(InControl.Key.Escape);
		inputActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		inputActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		inputActions.Right.AddDefaultBinding(InputControlType.DPadRight);
		inputActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		inputActions.Up.AddDefaultBinding(InputControlType.DPadUp);
		inputActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		inputActions.Down.AddDefaultBinding(InputControlType.DPadDown);
		inputActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		inputActions.RsUp.AddDefaultBinding(InputControlType.RightStickUp);
		inputActions.RsDown.AddDefaultBinding(InputControlType.RightStickDown);
		inputActions.RsLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		inputActions.RsRight.AddDefaultBinding(InputControlType.RightStickRight);
		inputActions.Pause.AddDefaultBinding(InControl.Key.Escape);
		inputActions.Pause.AddDefaultBinding(InputControlType.Start);
		inputActions.PaneRight.AddDefaultBinding(InControl.Key.RightBracket);
		inputActions.PaneRight.AddDefaultBinding(InputControlType.RightBumper);
		inputActions.PaneRight.AddDefaultBinding(InputControlType.RightTrigger);
		inputActions.PaneLeft.AddDefaultBinding(InControl.Key.LeftBracket);
		inputActions.PaneLeft.AddDefaultBinding(InputControlType.LeftBumper);
		inputActions.PaneLeft.AddDefaultBinding(InputControlType.LeftTrigger);
	}

	private void SetupMappableControllerBindingsList()
	{
		MappableControllerActions = new List<PlayerAction>
		{
			inputActions.Jump, inputActions.Attack, inputActions.Dash, inputActions.Cast, inputActions.SuperDash, inputActions.DreamNail, inputActions.QuickMap, inputActions.QuickCast, inputActions.Taunt, inputActions.OpenInventory,
			inputActions.Up, inputActions.Down, inputActions.Left, inputActions.Right
		};
	}

	private void SetupMappableKeyboardBindingsList()
	{
		MappableKeyboardActions = new List<PlayerAction>
		{
			inputActions.Jump, inputActions.Attack, inputActions.Dash, inputActions.Cast, inputActions.SuperDash, inputActions.DreamNail, inputActions.QuickMap, inputActions.QuickCast, inputActions.Taunt, inputActions.OpenInventory,
			inputActions.Up, inputActions.Down, inputActions.Left, inputActions.Right, inputActions.Up, inputActions.Down, inputActions.Left, inputActions.Right, inputActions.Taunt, inputActions.OpenInventoryJournal,
			inputActions.OpenInventoryMap, inputActions.OpenInventoryTools, inputActions.OpenInventoryQuests
		};
	}

	public Vector2 GetSticksInput(out bool isRightStick)
	{
		Vector2 value = inputActions.MoveVector.Value;
		Vector2 value2 = inputActions.RightStick.Value;
		Vector2 result;
		if (value2.magnitude > value.magnitude)
		{
			result = value2;
			isRightStick = true;
		}
		else
		{
			result = value;
			isRightStick = false;
		}
		if (result.magnitude > 1f)
		{
			result.Normalize();
		}
		return result;
	}
}
