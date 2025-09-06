using System;
using System.Collections;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private GameManager gm;

	private GameSettings gs;

	private InputHandler ih;

	public MenuAudioController uiAudioPlayer;

	public HollowKnightInputModule inputModule;

	[Space]
	[SerializeField]
	private CanvasGroup screenFader;

	[SerializeField]
	private float screenFadeTime = 1f;

	private Coroutine screenFadeRoutine;

	[Space]
	public float MENU_FADE_SPEED = 3.2f;

	private const float MENU_FADE_DELAY = 0.1f;

	private const float MENU_MODAL_DIMMER_ALPHA = 0.8f;

	public const float MENU_FADE_ALPHA_TOLERANCE = 0.05f;

	public const float MENU_FADE_FAILSAFE = 2f;

	[Header("State")]
	[Space(6f)]
	public UIState uiState;

	public MainMenuState menuState;

	[Header("Event System")]
	[Space(6f)]
	public EventSystem eventSystem;

	[Header("Main Elements")]
	[Space(6f)]
	public Canvas UICanvas;

	public CanvasGroup modalDimmer;

	public CanvasScaler canvasScaler;

	public Canvas GenericMessageCanvas;

	[Header("Menu Audio")]
	[Space(6f)]
	public AudioMixerSnapshot gameplaySnapshot;

	public AudioMixerSnapshot menuSilenceSnapshot;

	public AudioMixerSnapshot menuPauseSnapshot;

	[Space]
	public AudioMixerSnapshot defaultMusicSnapshot;

	public AudioMixerSnapshot blackThreadMusicSnapshot;

	public float musicSnapshotTransitionTime;

	[Header("Main Menu")]
	[Space(6f)]
	public CanvasGroup mainMenuScreen;

	public MainMenuOptions mainMenuButtons;

	public SpriteRenderer gameTitle;

	public PlayMakerFSM subtitleFSM;

	[Header("Save Profile Menu")]
	[Space(6f)]
	[SerializeField]
	private PreselectOption saveProfilePreselect;

	public CanvasGroup saveProfileScreen;

	public CanvasGroup saveProfileTitle;

	public CanvasGroup saveProfileControls;

	public Animator saveProfileTopFleur;

	public PreselectOption saveSlots;

	public SaveSlotButton slotOne;

	public SaveSlotButton slotTwo;

	public SaveSlotButton slotThree;

	public SaveSlotButton slotFour;

	public CheckpointSprite checkpointSprite;

	public GameObject blackThreadLoopAudio;

	[Header("Options Menu")]
	[Space(6f)]
	public MenuScreen optionsMenuScreen;

	[Header("Game Options Menu")]
	[Space(6f)]
	public MenuScreen gameOptionsMenuScreen;

	public GameMenuOptions gameMenuOptions;

	public MenuLanguageSetting languageSetting;

	public MenuSetting backerCreditsSetting;

	public MenuSetting nativeAchievementsSetting;

	public MenuSetting controllerRumbleSetting;

	public MenuSetting hudSetting;

	public MenuSetting cameraShakeSetting;

	public MenuSetting switchFrameCapSetting;

	[Header("Audio Menu")]
	[Space(6f)]
	public MenuScreen audioMenuScreen;

	public MenuAudioSlider masterSlider;

	public MenuAudioSlider musicSlider;

	public MenuAudioSlider soundSlider;

	public MenuSetting playerVoiceSetting;

	[Header("Video Menu")]
	[Space(6f)]
	public MenuScreen videoMenuScreen;

	public VideoMenuOptions videoMenuOptions;

	public MenuResolutionSetting resolutionOption;

	public ResolutionCountdownTimer countdownTimer;

	public MenuOptionHorizontal fullscreenMenuOption;

	public MenuSetting fullscreenOption;

	public MenuSetting vsyncOption;

	public MenuSetting particlesOption;

	public MenuSetting shadersOption;

	public MenuDisplaySetting displayOption;

	public MenuFrameCapSetting frameCapOption;

	[Header("Controller Options Menu")]
	[Space(6f)]
	public MenuScreen gamepadMenuScreen;

	public ControllerDetect controllerDetect;

	[Header("Controller Remap Menu")]
	[Space(6f)]
	public MenuScreen remapGamepadMenuScreen;

	[Header("Keyboard Options Menu")]
	[Space(6f)]
	public MenuScreen keyboardMenuScreen;

	[Header("Overscan Setting Menu")]
	[Space(6f)]
	public MenuScreen overscanMenuScreen;

	public OverscanSetting overscanSetting;

	[Header("Brightness Setting Menu")]
	[Space(6f)]
	public MenuScreen brightnessMenuScreen;

	public BrightnessSetting brightnessSetting;

	[Header("Achievements Menu")]
	[Space(6f)]
	public MenuScreen achievementsMenuScreen;

	public MenuAchievementsList menuAchievementsList;

	public GameObject achievementsPopupPanel;

	[Header("Extras Menu")]
	[Space(6f)]
	public MenuScreen extrasMenuScreen;

	public MenuScreen extrasContentMenuScreen;

	[Header("Play Mode Menu")]
	[Space(6f)]
	public MenuScreen playModeMenuScreen;

	[Header("Pause Menu")]
	[Space(6f)]
	public Animator pauseMenuAnimator;

	public MenuScreen pauseMenuScreen;

	[Header("Engage Menu")]
	[Space(6f)]
	public MenuScreen engageMenuScreen;

	public bool didLeaveEngageMenu;

	[Header("Prompts")]
	[Space(6f)]
	public MenuScreen quitGamePrompt;

	public MenuScreen returnMainMenuPrompt;

	public MenuScreen resolutionPrompt;

	[Header("Cinematics")]
	[SerializeField]
	private CinematicSkipPopup cinematicSkipPopup;

	[Header("Button Skins")]
	[Space(6f)]
	public UIButtonSkins uiButtonSkins;

	[Header("Menu Vibrations")]
	public VibrationDataAsset menuSubmitVibration;

	public VibrationDataAsset menuCancelVibration;

	private bool clearSaveFile;

	private Selectable lastSelected;

	private bool lastSubmitWasMouse;

	private bool ignoreUnpause;

	private bool isFadingMenu;

	private double startMenuTime;

	private MenuScreen fadingMenuScreen;

	private Coroutine fadingRoutine;

	private GraphicRaycaster graphicRaycaster;

	private bool permaDeath;

	private bool bossRush;

	private static readonly int _showProp = Animator.StringToHash("show");

	private static readonly int _hideProp = Animator.StringToHash("hide");

	private static readonly int _clearProp = Animator.StringToHash("clear");

	private bool isStartingNewGame;

	private bool isGoingBack;

	private int saveSlot;

	private AudioSourceFadeControl blackThreadAudioFader;

	private bool hasBlackThreadAudioFader;

	private bool initBlackThreadComponents;

	private bool registeredSaveStoreChangedEvent;

	private static UIManager _instance;

	private GameObject lastSelectionBeforeFocusLoss;

	public bool IsFadingMenu
	{
		get
		{
			if (!isFadingMenu)
			{
				return Time.timeAsDouble < startMenuTime;
			}
			return true;
		}
	}

	public static bool IsSelectingProfile { get; private set; }

	public static bool IsSaveProfileMenu { get; private set; }

	public static UIManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<UIManager>();
				if (_instance == null)
				{
					Debug.LogError("Couldn't find a UIManager, make sure one exists in the scene.");
					return null;
				}
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else if (this != _instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
		AudioGoToGameplay(0f);
		if ((bool)saveProfileScreen)
		{
			saveProfileScreen.gameObject.SetActive(value: true);
			saveProfileScreen.alpha = 0f;
			saveProfileScreen.blocksRaycasts = false;
			saveProfileScreen.interactable = false;
		}
		InitBlackThread();
	}

	public void SceneInit()
	{
		if (this == _instance)
		{
			SetupRefs();
		}
	}

	private void Start()
	{
		if (this != _instance)
		{
			return;
		}
		SetupRefs();
		if (gm.IsMenuScene())
		{
			SetScreenBlankerAlpha(1f);
			float num = FadeScreenIn();
			if (Platform.Current.IsSharedDataMounted)
			{
				LoadGameSettings();
			}
			startMenuTime = Time.timeAsDouble + (double)num;
			ConfigureMenu();
			if (DemoHelper.IsDemoMode)
			{
				slotOne.Prepare(gm);
				slotTwo.Prepare(gm);
				slotThree.Prepare(gm);
				slotFour.Prepare(gm);
			}
			else if (Platform.Current.WillPreloadSaveFiles && Platform.Current.IsSaveStoreMounted)
			{
				slotOne.PreloadSave(gm);
				slotTwo.PreloadSave(gm);
				slotThree.PreloadSave(gm);
				slotFour.PreloadSave(gm);
			}
			if (DemoHelper.IsDemoMode)
			{
				Navigation navigation = slotOne.navigation;
				Navigation navigation2 = slotTwo.navigation;
				Navigation navigation3 = slotThree.navigation;
				Navigation navigation4 = slotFour.navigation;
				navigation.selectOnDown = navigation.selectOnDown.navigation.selectOnDown;
				navigation2.selectOnDown = navigation2.selectOnDown.navigation.selectOnDown;
				navigation3.selectOnDown = navigation3.selectOnDown.navigation.selectOnDown;
				navigation4.selectOnDown = navigation4.selectOnDown.navigation.selectOnDown;
				navigation.selectOnRight = GetNavigationRightRecursive(navigation.selectOnRight);
				navigation2.selectOnLeft = GetNavigationLeftRecursive(navigation2.selectOnLeft);
				navigation2.selectOnRight = GetNavigationRightRecursive(navigation2.selectOnRight);
				navigation3.selectOnLeft = GetNavigationLeftRecursive(navigation3.selectOnLeft);
				navigation3.selectOnRight = GetNavigationRightRecursive(navigation3.selectOnRight);
				navigation4.selectOnLeft = GetNavigationLeftRecursive(navigation4.selectOnLeft);
				slotOne.navigation = navigation;
				slotTwo.navigation = navigation2;
				slotThree.navigation = navigation3;
				slotFour.navigation = navigation4;
				Selectable selectOnDown = navigation.selectOnDown;
				Navigation navigation5 = selectOnDown.navigation;
				navigation5.selectOnUp = slotOne;
				selectOnDown.navigation = navigation5;
			}
			if ((bool)menuAchievementsList)
			{
				if (Platform.Current.AreAchievementsFetched)
				{
					menuAchievementsList.PreInit();
				}
				else
				{
					Platform.AchievementsFetched += menuAchievementsList.PreInit;
				}
			}
		}
		else if (gm.startedOnThisScene && Platform.Current.IsSharedDataMounted)
		{
			LoadGameSettings();
		}
		RegisterSaveStoreChangedEvent();
		if ((bool)graphicRaycaster && (bool)ManagerSingleton<InputHandler>.Instance)
		{
			ManagerSingleton<InputHandler>.Instance.OnCursorVisibilityChange += delegate(bool isVisible)
			{
				graphicRaycaster.enabled = isVisible;
			};
		}
		int value = StaticVariableList.GetValue("ExhibitionModeProfileId", 0);
		if (value > 0)
		{
			StopAllCoroutines();
			uiState = UIState.MAIN_MENU_HOME;
			ih.StopUIInput();
			saveSlot = value;
			gm.LoadGameFromUI(value);
			StaticVariableList.SetValue("ExhibitionModeProfileId", 0);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if ((bool)menuAchievementsList && !Platform.Current.AreAchievementsFetched)
		{
			Platform.AchievementsFetched -= menuAchievementsList.PreInit;
		}
		UnregisterSaveStoreChangedEvent();
	}

	[ContextMenu("Test Save Reset")]
	private void TestSaveReset()
	{
		OnSaveStoreStateChanged(mounted: true);
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			LoadGameSettings();
			bool doAnimate = menuState == MainMenuState.SAVE_PROFILES || IsSaveProfileMenu;
			if ((bool)slotOne)
			{
				slotOne.ResetButton(gm, doAnimate);
			}
			if ((bool)slotTwo)
			{
				slotTwo.ResetButton(gm, doAnimate);
			}
			if ((bool)slotThree)
			{
				slotThree.ResetButton(gm, doAnimate);
			}
			if ((bool)slotFour)
			{
				slotFour.ResetButton(gm, doAnimate);
			}
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			lastSelectionBeforeFocusLoss = EventSystem.current.currentSelectedGameObject;
		}
		else if (lastSelectionBeforeFocusLoss != null)
		{
			EventSystem.current.SetSelectedGameObject(lastSelectionBeforeFocusLoss);
			lastSelectionBeforeFocusLoss = null;
		}
	}

	private Selectable GetNavigationRightRecursive(Selectable selectOnRight)
	{
		if (selectOnRight == null)
		{
			return null;
		}
		if (selectOnRight.gameObject.activeSelf)
		{
			return selectOnRight;
		}
		return GetNavigationRightRecursive(selectOnRight.navigation.selectOnRight);
	}

	private Selectable GetNavigationLeftRecursive(Selectable selectOnLeft)
	{
		if (selectOnLeft == null)
		{
			return null;
		}
		if (selectOnLeft.gameObject.activeSelf)
		{
			return selectOnLeft;
		}
		return GetNavigationLeftRecursive(selectOnLeft.navigation.selectOnLeft);
	}

	public void SetState(UIState newState)
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (newState != uiState)
		{
			if (uiState == UIState.PAUSED && newState == UIState.PLAYING)
			{
				UIClosePauseMenu();
			}
			else if (uiState == UIState.PLAYING && newState == UIState.PAUSED)
			{
				UIGoToPauseMenu();
			}
			else
			{
				switch (newState)
				{
				case UIState.INACTIVE:
					DisableScreens();
					break;
				case UIState.MAIN_MENU_HOME:
					if (Platform.Current.EngagementState == Platform.EngagementStates.Engaged)
					{
						didLeaveEngageMenu = true;
						UIGoToMainMenu();
					}
					else
					{
						UIGoToEngageMenu();
					}
					break;
				case UIState.LOADING:
					DisableScreens();
					break;
				case UIState.PLAYING:
					DisableScreens();
					break;
				case UIState.CUTSCENE:
					DisableScreens();
					break;
				}
			}
			uiState = newState;
		}
		else if (newState == UIState.MAIN_MENU_HOME)
		{
			UIGoToMainMenu();
		}
	}

	private void SetMenuState(MainMenuState newState)
	{
		menuState = newState;
	}

	private void SetupRefs()
	{
		gm = GameManager.instance;
		gs = gm.gameSettings;
		ih = gm.inputHandler;
		if (gm.IsMenuScene() && gameTitle == null)
		{
			gameTitle = GameObject.Find("LogoTitle").GetComponent<SpriteRenderer>();
		}
		if (UICanvas.worldCamera == null)
		{
			UICanvas.worldCamera = GameCameras.instance.mainCamera;
		}
	}

	public void SetUIStartState(GameState gameState)
	{
		switch (gameState)
		{
		case GameState.MAIN_MENU:
			SetState(UIState.MAIN_MENU_HOME);
			break;
		case GameState.LOADING:
			SetState(UIState.LOADING);
			break;
		case GameState.ENTERING_LEVEL:
			SetState(UIState.PLAYING);
			break;
		case GameState.PLAYING:
			SetState(UIState.PLAYING);
			break;
		case GameState.CUTSCENE:
			SetState(UIState.CUTSCENE);
			break;
		}
	}

	private void LoadGameSettings()
	{
		LanguageCode num = Language.CurrentLanguage();
		LoadGameOptionsSettings();
		LoadStoredSettings();
		Language.LoadLanguage();
		LanguageCode languageCode = Language.CurrentLanguage();
		if (num != languageCode)
		{
			gm.RefreshLocalization();
			ChangeFontByLanguage[] array = UnityEngine.Object.FindObjectsByType<ChangeFontByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFont();
			}
			ChangePositionByLanguage[] array2 = UnityEngine.Object.FindObjectsByType<ChangePositionByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].DoOffset();
			}
			ActivatePerLanguage[] array3 = UnityEngine.Object.FindObjectsByType<ActivatePerLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].UpdateLanguage();
			}
			ChangeByLanguageBase[] array4 = UnityEngine.Object.FindObjectsByType<ChangeByLanguageBase>(FindObjectsSortMode.None);
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].DoUpdate();
			}
		}
	}

	private void RegisterSaveStoreChangedEvent()
	{
		if (!registeredSaveStoreChangedEvent)
		{
			registeredSaveStoreChangedEvent = true;
			Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
		}
	}

	private void UnregisterSaveStoreChangedEvent()
	{
		if (registeredSaveStoreChangedEvent)
		{
			registeredSaveStoreChangedEvent = false;
			Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
		}
	}

	public bool UIGoBack()
	{
		if (IsSelectingProfile)
		{
			UIGoBackToSaveProfiles();
			return true;
		}
		return menuState switch
		{
			MainMenuState.OPTIONS_MENU => optionsMenuScreen.GoBack(), 
			MainMenuState.GAMEPAD_MENU => gamepadMenuScreen.GoBack(), 
			MainMenuState.KEYBOARD_MENU => keyboardMenuScreen.GoBack(), 
			MainMenuState.AUDIO_MENU => audioMenuScreen.GoBack(), 
			MainMenuState.VIDEO_MENU => videoMenuScreen.GoBack(), 
			MainMenuState.OVERSCAN_MENU => overscanMenuScreen.GoBack(), 
			MainMenuState.GAME_OPTIONS_MENU => gameOptionsMenuScreen.GoBack(), 
			MainMenuState.ACHIEVEMENTS_MENU => achievementsMenuScreen.GoBack(), 
			MainMenuState.RESOLUTION_PROMPT => resolutionPrompt.GoBack(), 
			MainMenuState.BRIGHTNESS_MENU => brightnessMenuScreen.GoBack(), 
			MainMenuState.PAUSE_MENU => pauseMenuScreen.GoBack(), 
			MainMenuState.PLAY_MODE_MENU => playModeMenuScreen.GoBack(), 
			MainMenuState.EXTRAS_MENU => extrasMenuScreen.GoBack(), 
			MainMenuState.REMAP_GAMEPAD_MENU => remapGamepadMenuScreen.GoBack(), 
			MainMenuState.EXTRAS_CONTENT_MENU => extrasContentMenuScreen.GoBack(), 
			MainMenuState.ENGAGE_MENU => engageMenuScreen.GoBack(), 
			_ => false, 
		};
	}

	public void UIGoToOptionsMenu()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void UILeaveOptionsMenu()
	{
		if (uiState == UIState.PAUSED)
		{
			UIGoToPauseMenu();
		}
		else
		{
			UIGoToMainMenu();
		}
	}

	public void ClearSaveCache()
	{
		slotOne.ClearCache();
		slotTwo.ClearCache();
		slotThree.ClearCache();
		slotFour.ClearCache();
	}

	public void UIExplicitSwitchUser()
	{
		ClearSaveCache();
		UIGoToEngageMenu();
	}

	public void UIGoToEngageMenu()
	{
		StartCoroutine(GoToEngageMenu());
	}

	public void UIGoToMainMenu()
	{
		StartCoroutine(GoToMainMenu());
	}

	public void UIGoToProfileMenu()
	{
		StartCoroutine(GoToProfileMenu());
	}

	public void UIGoToControllerMenu()
	{
		StartCoroutine(GoToControllerMenu());
	}

	public void UIGoToRemapControllerMenu()
	{
		StartCoroutine(GoToRemapControllerMenu());
	}

	public void UIGoToKeyboardMenu()
	{
		StartCoroutine(GoToKeyboardMenu());
	}

	public void UIGoToAudioMenu()
	{
		StartCoroutine(GoToAudioMenu());
	}

	public void UIGoToVideoMenu(bool rollbackRes = false)
	{
		StartCoroutine(GoToVideoMenu(rollbackRes));
	}

	public void UIGoToPauseMenu()
	{
		StartCoroutine(GoToPauseMenu());
	}

	public void UIClosePauseMenu()
	{
		ih.StopUIInput();
		StartCoroutine(HideCurrentMenu());
		StartCoroutine(FadeOutCanvasGroup(modalDimmer));
	}

	public void UIClearPauseMenu()
	{
		pauseMenuAnimator.SetBool(_clearProp, value: true);
	}

	public void UnClearPauseMenu()
	{
		pauseMenuAnimator.SetBool(_clearProp, value: false);
	}

	public void UIGoToOverscanMenu()
	{
		StartCoroutine(GoToOverscanMenu());
	}

	public void UIGoToBrightnessMenu()
	{
		StartCoroutine(GoToBrightnessMenu());
	}

	public void UIGoToGameOptionsMenu()
	{
		StartCoroutine(GoToGameOptionsMenu());
	}

	public void UIGoToAchievementsMenu()
	{
		StartCoroutine(GoToAchievementsMenu());
	}

	public void UIGoToExtrasMenu()
	{
		StartCoroutine(GoToExtrasMenu());
	}

	public void UIGoToExtrasContentMenu()
	{
		StartCoroutine(GoToExtrasContentMenu());
	}

	public void UIShowQuitGamePrompt()
	{
		StartCoroutine(GoToQuitGamePrompt());
	}

	public void UIShowReturnMenuPrompt()
	{
		StartCoroutine(GoToReturnMenuPrompt());
	}

	public void UIShowResolutionPrompt(bool startTimer = false)
	{
		StartCoroutine(GoToResolutionPrompt(startTimer));
	}

	public void UILeaveExitToMenuPrompt()
	{
		StartCoroutine(LeaveExitToMenuPrompt());
	}

	public void UIGoToPlayModeMenu()
	{
		StartCoroutine(GoToPlayModeMenu());
	}

	public void UIReturnToMainMenu()
	{
		StartCoroutine(ReturnToMainMenu());
	}

	public void UIGoToMenuCredits()
	{
		StartCoroutine(GoToMenuCredits());
	}

	public void UIStartNewGame()
	{
		StartNewGame();
	}

	public void UIStartNewGameContinue()
	{
		if (isStartingNewGame)
		{
			StartNewGame(permaDeath, bossRush);
		}
		else
		{
			UIContinueGame(saveSlot);
		}
	}

	public void UIGoBackToSaveProfiles()
	{
		StartCoroutine(GoBackToSaveProfiles());
	}

	public void StartNewGame(bool permaDeath = false, bool bossRush = false)
	{
		this.permaDeath = permaDeath;
		this.bossRush = bossRush;
		ih.StopUIInput();
		IsSelectingProfile = true;
		isStartingNewGame = true;
		if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 1)
		{
			IsSelectingProfile = false;
			isStartingNewGame = false;
			gm.EnsureSaveSlotSpace(delegate(bool hasSpace)
			{
				if (hasSpace)
				{
					if (menuState == MainMenuState.SAVE_PROFILES)
					{
						StartCoroutine(HideSaveProfileMenu(updateBlackThread: false));
					}
					else
					{
						StartCoroutine(HideCurrentMenu());
					}
					uiAudioPlayer.PlayStartGame();
					if ((bool)MenuStyles.Instance)
					{
						MenuStyles.Instance.StopAudio();
					}
					gm.StartNewGame(permaDeath, bossRush);
				}
				else
				{
					ih.StartUIInput();
					(gm.profileID switch
					{
						2 => slotTwo, 
						3 => slotThree, 
						4 => slotFour, 
						_ => slotOne, 
					}).Select();
					Debug.LogError("Insufficient space for new save profile", this);
				}
			});
		}
		else if (gs.overscanAdjusted == 0)
		{
			UIGoToOverscanMenu();
		}
		else if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 0)
		{
			UIGoToBrightnessMenu();
		}
	}

	public void UIContinueGame(int slot)
	{
		ih.StopUIInput();
		IsSelectingProfile = true;
		isStartingNewGame = false;
		saveSlot = slot;
		if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 1)
		{
			IsSelectingProfile = false;
			gm.LoadGameFromUI(slot);
		}
		else if (gs.overscanAdjusted == 0)
		{
			UIGoToOverscanMenu();
		}
		else if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 0)
		{
			UIGoToBrightnessMenu();
		}
	}

	public void UIContinueGame(int slot, SaveGameData saveGameData)
	{
		ih.StopUIInput();
		IsSelectingProfile = true;
		isStartingNewGame = false;
		saveSlot = slot;
		if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 1)
		{
			IsSelectingProfile = false;
			gm.LoadGameFromUI(slot, saveGameData);
		}
		else if (gs.overscanAdjusted == 0)
		{
			UIGoToOverscanMenu();
		}
		else if (gs.overscanAdjusted == 1 && gs.brightnessAdjusted == 0)
		{
			UIGoToBrightnessMenu();
		}
	}

	public void ContinueGame()
	{
		ih.StopUIInput();
		if ((bool)MenuStyles.Instance)
		{
			MenuStyles.Instance.StopAudio();
		}
		if (StaticVariableList.GetValue("ExhibitionModeProfileId", 0) <= 0)
		{
			uiAudioPlayer.PlayStartGame();
			if (menuState == MainMenuState.SAVE_PROFILES)
			{
				StartCoroutine(HideSaveProfileMenu(updateBlackThread: false));
			}
		}
	}

	public void PrepareContinueGame()
	{
		ih.StopUIInput();
		if ((bool)MenuStyles.Instance)
		{
			MenuStyles.Instance.StopAudio();
		}
		if (menuState == MainMenuState.SAVE_PROFILES)
		{
			StartCoroutine(HideSaveProfileMenu(updateBlackThread: false));
		}
	}

	public static void HighlightSelectableNoSound(Selectable selectable)
	{
		IPlaySelectSound component = selectable.GetComponent<IPlaySelectSound>();
		if (component != null)
		{
			component.DontPlaySelectSound = true;
			selectable.Select();
			component.DontPlaySelectSound = false;
		}
		else
		{
			selectable.Select();
		}
	}

	public IEnumerator GoToEngageMenu()
	{
		if (ih == null)
		{
			ih = gm.inputHandler;
		}
		ih.StopUIInput();
		didLeaveEngageMenu = false;
		Platform.Current.ClearEngagement();
		Platform.Current.BeginEngagement();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			mainMenuScreen.interactable = false;
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else if (menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return StartCoroutine(HideSaveProfileMenu(updateBlackThread: true));
		}
		else
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		ih.StopUIInput();
		gameTitle.gameObject.SetActive(value: true);
		gameTitle.GetComponent<LogoLanguage>().SetSprite();
		engageMenuScreen.gameObject.SetActive(value: true);
		StartCoroutine(FadeInSprite(gameTitle));
		subtitleFSM.SendEvent("FADE IN");
		engageMenuScreen.topFleur.ResetTrigger(_hideProp);
		engageMenuScreen.topFleur.SetTrigger(_showProp);
		engageMenuScreen.bottomFleur.ResetTrigger(_hideProp);
		engageMenuScreen.bottomFleur.SetTrigger(_showProp);
		yield return StartCoroutine(FadeInCanvasGroup(engageMenuScreen.GetComponent<CanvasGroup>()));
		yield return null;
		SetMenuState(MainMenuState.ENGAGE_MENU);
	}

	public IEnumerator GoToMainMenu()
	{
		if (ih == null)
		{
			ih = gm.inputHandler;
		}
		ih.StopUIInput();
		if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.ACHIEVEMENTS_MENU || menuState == MainMenuState.QUIT_GAME_PROMPT || menuState == MainMenuState.EXTRAS_MENU || menuState == MainMenuState.ENGAGE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		else if (menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return StartCoroutine(HideSaveProfileMenu(updateBlackThread: true));
		}
		else
		{
			yield return null;
		}
		ih.StopUIInput();
		gameTitle.gameObject.SetActive(value: true);
		gameTitle.GetComponent<LogoLanguage>().SetSprite();
		StartCoroutine(FadeInSprite(gameTitle));
		subtitleFSM.SendEvent("FADE IN");
		float num = (float)(startMenuTime - Time.timeAsDouble);
		if (num > 0f)
		{
			yield return new WaitForSeconds(num);
		}
		mainMenuScreen.gameObject.SetActive(value: true);
		yield return StartCoroutine(FadeInCanvasGroup(mainMenuScreen));
		yield return null;
		mainMenuScreen.interactable = true;
		ih.StartUIInput();
		mainMenuButtons.HighlightDefault();
		SetMenuState(MainMenuState.MAIN_MENU);
	}

	public IEnumerator GoToProfileMenu()
	{
		IsSelectingProfile = false;
		isStartingNewGame = false;
		IsSaveProfileMenu = true;
		ih.StopUIInput();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			StartCoroutine(FadeOutSprite(gameTitle));
			subtitleFSM.SendEvent("FADE OUT");
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else if (menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			ih.StopUIInput();
		}
		SaveSlotButton itemToHighlight = ((!DemoHelper.IsDemoMode) ? Platform.Current.LocalSharedData.GetInt("lastProfileIndex", 0) : 0) switch
		{
			2 => slotTwo, 
			3 => slotThree, 
			4 => slotFour, 
			_ => slotOne, 
		};
		saveProfilePreselect.itemToHighlight = itemToHighlight;
		StartCoroutine(FadeInCanvasGroup(saveProfileScreen, alwaysActive: true));
		StartCoroutine(FadeInCanvasGroup(saveProfileTitle));
		saveProfileTopFleur.ResetTrigger(_hideProp);
		saveProfileTopFleur.SetTrigger(_showProp);
		bool hasPreload = slotOne.HasPreloaded;
		if (hasPreload)
		{
			yield return new WaitForSeconds(0.165f);
		}
		StartCoroutine(PrepareSaveFilesInOrder());
		if (!hasPreload)
		{
			yield return new WaitForSeconds(0.165f);
		}
		uiAudioPlayer.PlayOpenProfileSelect();
		SaveSlotButton[] array = new SaveSlotButton[4] { slotOne, slotTwo, slotThree, slotFour };
		SaveSlotButton[] array2 = array;
		foreach (SaveSlotButton saveSlotButton in array2)
		{
			if ((bool)saveSlotButton && saveSlotButton.gameObject.activeSelf)
			{
				saveSlotButton.UpdateSaveFileState();
				saveSlotButton.ShowRelevantModeForSaveFileState();
				yield return new WaitForSeconds(0.165f);
			}
		}
		yield return new WaitForSeconds(0.165f);
		StartCoroutine(FadeInCanvasGroup(saveProfileControls));
		yield return null;
		ih.StartUIInput();
		saveSlots.HighlightDefault();
		SetMenuState(MainMenuState.SAVE_PROFILES);
		IsSaveProfileMenu = false;
	}

	private void InitBlackThread()
	{
		if (!initBlackThreadComponents)
		{
			initBlackThreadComponents = true;
			if (!(blackThreadLoopAudio == null))
			{
				blackThreadAudioFader = blackThreadLoopAudio.GetComponent<AudioSourceFadeControl>();
				hasBlackThreadAudioFader = blackThreadAudioFader != null;
			}
		}
	}

	public void UpdateBlackThreadAudio()
	{
		if (!blackThreadLoopAudio)
		{
			return;
		}
		InitBlackThread();
		if (slotOne.IsBlackThreaded || slotTwo.IsBlackThreaded || slotThree.IsBlackThreaded || slotFour.IsBlackThreaded)
		{
			if (hasBlackThreadAudioFader)
			{
				blackThreadAudioFader.FadeUp();
				if (!blackThreadLoopAudio.activeSelf)
				{
					blackThreadLoopAudio.SetActive(value: true);
				}
			}
			else
			{
				blackThreadLoopAudio.SetActive(value: true);
			}
			blackThreadMusicSnapshot.TransitionTo(musicSnapshotTransitionTime);
		}
		else
		{
			if (hasBlackThreadAudioFader)
			{
				blackThreadAudioFader.FadeDown();
			}
			else
			{
				blackThreadLoopAudio.SetActive(value: false);
			}
			defaultMusicSnapshot.TransitionTo(musicSnapshotTransitionTime);
		}
	}

	public void FadeOutBlackThreadLoop()
	{
		if ((bool)blackThreadLoopAudio)
		{
			if (hasBlackThreadAudioFader)
			{
				blackThreadAudioFader.FadeDown();
			}
			else
			{
				blackThreadLoopAudio.SetActive(value: false);
			}
		}
	}

	protected IEnumerator PrepareSaveFilesInOrder()
	{
		SaveSlotButton[] slotButtons = new SaveSlotButton[4] { slotOne, slotTwo, slotThree, slotFour };
		foreach (SaveSlotButton saveSlotButton in slotButtons)
		{
			if ((bool)saveSlotButton && saveSlotButton.saveFileState == SaveSlotButton.SaveFileStates.NotStarted)
			{
				saveSlotButton.PreloadSave(gm);
			}
		}
		int i2 = 0;
		while (i2 < slotButtons.Length)
		{
			SaveSlotButton slotButton = slotButtons[i2];
			if ((bool)slotButton && slotButton.saveFileState == SaveSlotButton.SaveFileStates.NotStarted)
			{
				slotButton.Prepare(gm);
				float waitTime = 0.165f;
				while (slotButton.saveFileState == SaveSlotButton.SaveFileStates.OperationInProgress)
				{
					yield return null;
					waitTime -= Time.deltaTime;
				}
				if (waitTime > 0f)
				{
					yield return new WaitForSeconds(waitTime);
				}
			}
			int num = i2 + 1;
			i2 = num;
		}
	}

	public IEnumerator GoToOptionsMenu()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			StartCoroutine(FadeOutSprite(gameTitle));
			subtitleFSM.SendEvent("FADE OUT");
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else if (menuState == MainMenuState.AUDIO_MENU || menuState == MainMenuState.VIDEO_MENU || menuState == MainMenuState.GAMEPAD_MENU || menuState == MainMenuState.GAME_OPTIONS_MENU || menuState == MainMenuState.PAUSE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		else if (menuState == MainMenuState.KEYBOARD_MENU)
		{
			if (uiButtonSkins.listeningKey != null)
			{
				uiButtonSkins.listeningKey.StopActionListening();
				uiButtonSkins.listeningKey.AbortRebind();
			}
			yield return StartCoroutine(HideCurrentMenu());
		}
		yield return StartCoroutine(ShowMenu(optionsMenuScreen));
		SetMenuState(MainMenuState.OPTIONS_MENU);
		ih.StartUIInput();
	}

	public IEnumerator GoToControllerMenu()
	{
		if (menuState == MainMenuState.OPTIONS_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		else if (menuState == MainMenuState.REMAP_GAMEPAD_MENU)
		{
			if (uiButtonSkins.listeningButton != null)
			{
				uiButtonSkins.listeningButton.StopActionListening();
				uiButtonSkins.listeningButton.AbortRebind();
			}
			yield return StartCoroutine(HideCurrentMenu());
		}
		yield return StartCoroutine(ShowMenu(gamepadMenuScreen));
		SetMenuState(MainMenuState.GAMEPAD_MENU);
	}

	public IEnumerator GoToRemapControllerMenu()
	{
		yield return StartCoroutine(HideCurrentMenu());
		StartCoroutine(ShowMenu(remapGamepadMenuScreen));
		yield return StartCoroutine(uiButtonSkins.ShowCurrentButtonMappings());
		SetMenuState(MainMenuState.REMAP_GAMEPAD_MENU);
	}

	public IEnumerator GoToKeyboardMenu()
	{
		yield return StartCoroutine(HideCurrentMenu());
		StartCoroutine(ShowMenu(keyboardMenuScreen));
		uiButtonSkins.ShowCurrentKeyboardMappings();
		SetMenuState(MainMenuState.KEYBOARD_MENU);
	}

	public IEnumerator GoToAudioMenu()
	{
		yield return StartCoroutine(HideCurrentMenu());
		yield return StartCoroutine(ShowMenu(audioMenuScreen));
		SetMenuState(MainMenuState.AUDIO_MENU);
	}

	public IEnumerator GoToVideoMenu(bool rollbackRes = false)
	{
		if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.OVERSCAN_MENU || menuState == MainMenuState.BRIGHTNESS_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		else if (menuState == MainMenuState.RESOLUTION_PROMPT)
		{
			if (rollbackRes)
			{
				HideMenuInstant(resolutionPrompt);
				videoMenuScreen.gameObject.SetActive(value: true);
				eventSystem.SetSelectedGameObject(null);
				resolutionOption.RollbackResolution();
			}
			else
			{
				yield return StartCoroutine(HideCurrentMenu());
			}
		}
		yield return StartCoroutine(ShowMenu(videoMenuScreen));
		SetMenuState(MainMenuState.VIDEO_MENU);
	}

	public IEnumerator GoToOverscanMenu()
	{
		if (menuState == MainMenuState.VIDEO_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			overscanSetting.NormalMode();
		}
		else if (menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return StartCoroutine(HideSaveProfileMenu(updateBlackThread: true));
			overscanSetting.DoneMode();
		}
		else if (menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			overscanSetting.DoneMode();
		}
		yield return StartCoroutine(ShowMenu(overscanMenuScreen));
		SetMenuState(MainMenuState.OVERSCAN_MENU);
	}

	public IEnumerator GoToBrightnessMenu()
	{
		if (menuState == MainMenuState.VIDEO_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			brightnessSetting.NormalMode();
		}
		else if (menuState == MainMenuState.OVERSCAN_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			brightnessSetting.DoneMode();
		}
		else if (menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return StartCoroutine(HideSaveProfileMenu(updateBlackThread: true));
			brightnessSetting.DoneMode();
		}
		else if (menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
			brightnessSetting.DoneMode();
		}
		yield return StartCoroutine(ShowMenu(brightnessMenuScreen));
		SetMenuState(MainMenuState.BRIGHTNESS_MENU);
	}

	public IEnumerator GoToGameOptionsMenu()
	{
		yield return StartCoroutine(HideCurrentMenu());
		yield return StartCoroutine(ShowMenu(gameOptionsMenuScreen));
		SetMenuState(MainMenuState.GAME_OPTIONS_MENU);
	}

	public IEnumerator GoToAchievementsMenu()
	{
		if (Platform.Current.HasNativeAchievementsDialog)
		{
			Platform.Current.ShowNativeAchievementsDialog();
			yield return null;
			mainMenuButtons.achievementsButton.Select();
			yield break;
		}
		ih.StopUIInput();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			StartCoroutine(FadeOutSprite(gameTitle));
			subtitleFSM.SendEvent("FADE OUT");
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return StartCoroutine(ShowMenu(achievementsMenuScreen));
		SetMenuState(MainMenuState.ACHIEVEMENTS_MENU);
		ih.StartUIInput();
	}

	public IEnumerator GoToExtrasMenu()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			StartCoroutine(FadeOutSprite(gameTitle));
			subtitleFSM.SendEvent("FADE OUT");
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else if (menuState == MainMenuState.EXTRAS_CONTENT_MENU)
		{
			yield return StartCoroutine(HideMenu(extrasContentMenuScreen));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return StartCoroutine(ShowMenu(extrasMenuScreen));
		SetMenuState(MainMenuState.EXTRAS_MENU);
		ih.StartUIInput();
	}

	public IEnumerator GoToExtrasContentMenu()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.EXTRAS_MENU)
		{
			yield return StartCoroutine(HideMenu(extrasMenuScreen));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return StartCoroutine(ShowMenu(extrasContentMenuScreen));
		SetMenuState(MainMenuState.EXTRAS_CONTENT_MENU);
		ih.StartUIInput();
	}

	public IEnumerator GoToQuitGamePrompt()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.MAIN_MENU)
		{
			StartCoroutine(FadeOutSprite(gameTitle));
			subtitleFSM.SendEvent("FADE OUT");
			yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return StartCoroutine(ShowMenu(quitGamePrompt));
		SetMenuState(MainMenuState.QUIT_GAME_PROMPT);
		ih.StartUIInput();
	}

	public IEnumerator GoToReturnMenuPrompt()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.PAUSE_MENU)
		{
			yield return StartCoroutine(HideCurrentMenu());
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return StartCoroutine(ShowMenu(returnMainMenuPrompt));
		SetMenuState(MainMenuState.EXIT_PROMPT);
		ih.StartUIInput();
	}

	public IEnumerator GoToResolutionPrompt(bool startTimer = false)
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.VIDEO_MENU)
		{
			yield return StartCoroutine(HideMenu(videoMenuScreen));
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return StartCoroutine(ShowMenu(resolutionPrompt));
		SetMenuState(MainMenuState.RESOLUTION_PROMPT);
		if (startTimer)
		{
			countdownTimer.StartTimer();
		}
		ih.StartUIInput();
	}

	public IEnumerator LeaveExitToMenuPrompt()
	{
		yield return StartCoroutine(HideCurrentMenu());
		if (uiState == UIState.PAUSED)
		{
			UnClearPauseMenu();
		}
	}

	public IEnumerator GoToPlayModeMenu()
	{
		ih.StopUIInput();
		if (menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return StartCoroutine(HideSaveProfileMenu(updateBlackThread: true));
		}
		yield return StartCoroutine(ShowMenu(playModeMenuScreen));
		SetMenuState(MainMenuState.PLAY_MODE_MENU);
		ih.StartUIInput();
	}

	public IEnumerator ReturnToMainMenu()
	{
		ih.StopUIInput();
		bool calledBack = false;
		StartCoroutine(gm.ReturnToMainMenu(willSave: true, delegate(bool willComplete)
		{
			calledBack = true;
			if (!willComplete)
			{
				ih.StartUIInput();
				returnMainMenuPrompt.HighlightDefault();
			}
			else
			{
				StartCoroutine(HideCurrentMenu());
			}
		}));
		while (!calledBack)
		{
			yield return null;
		}
	}

	public IEnumerator EmergencyReturnToMainMenu()
	{
		ih.StopUIInput();
		bool calledBack = false;
		PlayerData playerData = gm.playerData;
		int num;
		if ((!InteractManager.BlockingInteractable || playerData.atBench) && !playerData.disablePause && !playerData.disableSaveQuit && ih.PauseAllowed)
		{
			GameState gameState = gm.GameState;
			if (gameState == GameState.PLAYING || gameState == GameState.PAUSED)
			{
				num = ((!ScenePreloader.HasPendingOperations) ? 1 : 0);
				goto IL_00b9;
			}
		}
		num = 0;
		goto IL_00b9;
		IL_00b9:
		bool willSave = (byte)num != 0;
		StartCoroutine(gm.ReturnToMainMenu(willSave, delegate(bool willComplete)
		{
			calledBack = true;
			if (!willComplete)
			{
				ih.StartUIInput();
				returnMainMenuPrompt.HighlightDefault();
			}
			else
			{
				ih.StartUIInput();
				StartCoroutine(HideCurrentMenu());
			}
		}, isEndGame: false, forceMainMenu: true));
		while (!calledBack)
		{
			yield return null;
		}
	}

	public IEnumerator GoToPauseMenu()
	{
		ih.StopUIInput();
		ignoreUnpause = true;
		if (uiState == UIState.PAUSED)
		{
			if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.EXIT_PROMPT)
			{
				yield return StartCoroutine(HideCurrentMenu());
			}
		}
		else
		{
			StartCoroutine(FadeInCanvasGroupAlpha(modalDimmer, 0.8f));
		}
		yield return StartCoroutine(ShowMenu(pauseMenuScreen));
		SetMenuState(MainMenuState.PAUSE_MENU);
		ih.StartUIInput();
		ignoreUnpause = false;
	}

	public IEnumerator GoToMenuCredits()
	{
		ih.StopUIInput();
		yield return StartCoroutine(HideCurrentMenu());
		float num = FadeScreenOut();
		if (num > 0.25f)
		{
			yield return new WaitForSeconds(num - 0.25f);
			TransitionAudioFader.FadeOutAllFaders();
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			TransitionAudioFader.FadeOutAllFaders();
			yield return new WaitForSeconds(num);
		}
		gm.LoadScene("Menu_Credits");
	}

	private IEnumerator GoBackToSaveProfiles()
	{
		ih.StopAcceptingInput();
		isGoingBack = true;
		yield return HideCurrentMenu();
		yield return GoToProfileMenu();
		isGoingBack = false;
	}

	public void ReloadSaves()
	{
		bool doAnimate = menuState == MainMenuState.SAVE_PROFILES;
		slotOne.ResetButton(gm, doAnimate);
		slotTwo.ResetButton(gm, doAnimate);
		slotThree.ResetButton(gm, doAnimate);
		slotFour.ResetButton(gm, doAnimate);
	}

	public void ShowCutscenePrompt(CinematicSkipPopup.Texts text)
	{
		cinematicSkipPopup.Show(text);
	}

	public void HideCutscenePrompt(bool isInstant, Action onEnd = null)
	{
		cinematicSkipPopup.Hide(isInstant, onEnd);
	}

	public void ApplyAudioMenuSettings()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void ApplyVideoMenuSettings()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void ApplyControllerMenuSettings()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void ApplyRemapGamepadMenuSettings()
	{
		StartCoroutine(GoToControllerMenu());
	}

	public void ApplyKeyboardMenuSettings()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void ApplyOverscanSettings(bool goToBrightness = false)
	{
		Debug.LogError("This function is now deprecated");
	}

	public void ApplyBrightnessSettings()
	{
		StartCoroutine(GoToVideoMenu());
	}

	public void ApplyGameMenuSettings()
	{
		StartCoroutine(GoToOptionsMenu());
	}

	public void SaveOverscanSettings()
	{
		gs.SaveOverscanSettings();
	}

	public void SaveBrightnessSettings()
	{
		gs.SaveBrightnessSettings();
	}

	public void DefaultAudioMenuSettings()
	{
		gs.ResetAudioSettings();
		RefreshAudioControls();
	}

	public void DefaultVideoMenuSettings()
	{
		gs.ResetVideoSettings();
		Platform.Current.AdjustGraphicsSettings(gs);
		resolutionOption.ResetToDefaultResolution();
		fullscreenOption.UpdateSetting(gs.fullScreen);
		if (fullscreenMenuOption != null)
		{
			fullscreenMenuOption.RefreshMenuControls();
			fullscreenMenuOption.UpdateApplyButton();
		}
		vsyncOption.UpdateSetting(gs.vSync);
		shadersOption.UpdateSetting((int)gs.shaderQuality);
		RefreshVideoControls();
	}

	public void DefaultGamepadMenuSettings()
	{
		ih.ResetDefaultControllerButtonBindings();
		uiButtonSkins.RefreshButtonMappings();
	}

	public void DefaultKeyboardMenuSettings()
	{
		ih.ResetDefaultKeyBindings();
		uiButtonSkins.RefreshKeyMappings();
	}

	public void DefaultGameMenuSettings()
	{
		gs.ResetGameOptionsSettings();
		Platform.Current.AdjustGameSettings(gs);
		RefreshGameOptionsControls();
	}

	public void LoadStoredSettings()
	{
		gs.LoadOverscanConfigured();
		gs.LoadBrightnessConfigured();
		LoadAudioSettings();
		LoadVideoSettings();
		LoadGameOptionsSettings();
	}

	private void LoadAudioSettings()
	{
		gs.LoadAudioSettings();
		RefreshAudioControls();
	}

	private void LoadVideoSettings()
	{
		gs.LoadVideoSettings();
		gs.LoadOverscanSettings();
		gs.LoadBrightnessSettings();
		Platform.Current.AdjustGraphicsSettings(gs);
		RefreshVideoControls();
	}

	private void LoadGameOptionsSettings()
	{
		gs.LoadGameOptionsSettings();
		Platform.Current.AdjustGameSettings(gs);
		RefreshGameOptionsControls();
	}

	private void LoadControllerSettings()
	{
		Debug.LogError("Not yet implemented.");
	}

	private void RefreshAudioControls()
	{
		masterSlider.RefreshValueFromSettings();
		musicSlider.RefreshValueFromSettings();
		soundSlider.RefreshValueFromSettings();
		playerVoiceSetting.RefreshValueFromGameSettings();
	}

	private void RefreshVideoControls()
	{
		resolutionOption.RefreshControls();
		fullscreenOption.RefreshValueFromGameSettings();
		vsyncOption.RefreshValueFromGameSettings(alsoApplySetting: true);
		overscanSetting.RefreshValueFromSettings();
		brightnessSetting.RefreshValueFromSettings();
		displayOption.RefreshControls();
		frameCapOption.RefreshValueFromGameSettings();
		particlesOption.RefreshValueFromGameSettings();
		shadersOption.RefreshValueFromGameSettings();
	}

	public void DisableFrameCapSetting()
	{
		if ((bool)frameCapOption)
		{
			frameCapOption.DisableFrameCapSetting();
		}
	}

	public void DisableVsyncSetting()
	{
		if ((bool)vsyncOption)
		{
			vsyncOption.UpdateSetting(0);
			vsyncOption.RefreshValueFromGameSettings();
		}
	}

	private void RefreshKeyboardControls()
	{
		uiButtonSkins.RefreshKeyMappings();
	}

	private void RefreshGameOptionsControls()
	{
		languageSetting.RefreshControls();
		backerCreditsSetting.RefreshValueFromGameSettings();
		nativeAchievementsSetting.RefreshValueFromGameSettings();
		controllerRumbleSetting.RefreshValueFromGameSettings(alsoApplySetting: true);
		cameraShakeSetting.RefreshValueFromGameSettings(alsoApplySetting: true);
		hudSetting.RefreshValueFromGameSettings(alsoApplySetting: true);
		switchFrameCapSetting.RefreshValueFromGameSettings(alsoApplySetting: true);
	}

	public void TogglePauseGame()
	{
		if (!ignoreUnpause)
		{
			StartCoroutine(gm.PauseGameToggleByMenu());
		}
	}

	public void QuitGame()
	{
		ih.StopUIInput();
		StartCoroutine(gm.QuitGame());
	}

	public void FadeOutMenuAudio(float duration)
	{
		menuSilenceSnapshot.TransitionToSafe(duration);
	}

	public void AudioGoToPauseMenu(float duration)
	{
		menuPauseSnapshot.TransitionToSafe(duration);
	}

	public void AudioGoToGameplay(float duration)
	{
		gameplaySnapshot.TransitionToSafe(duration);
	}

	public void ConfigureMenu()
	{
		if (mainMenuButtons != null)
		{
			mainMenuButtons.ConfigureNavigation();
		}
		if (gameMenuOptions != null)
		{
			gameMenuOptions.ConfigureNavigation();
		}
		if (videoMenuOptions != null)
		{
			videoMenuOptions.ConfigureNavigation();
		}
		_ = uiState;
		_ = 1;
	}

	public IEnumerator HideCurrentMenu()
	{
		isFadingMenu = true;
		float disableAfterDelay = 0f;
		MenuScreen menu;
		switch (menuState)
		{
		default:
			yield break;
		case MainMenuState.OPTIONS_MENU:
			menu = optionsMenuScreen;
			break;
		case MainMenuState.AUDIO_MENU:
			menu = audioMenuScreen;
			gs.SaveAudioSettings();
			break;
		case MainMenuState.VIDEO_MENU:
			menu = videoMenuScreen;
			gs.SaveVideoSettings();
			break;
		case MainMenuState.GAMEPAD_MENU:
			menu = gamepadMenuScreen;
			gs.SaveGameOptionsSettings();
			break;
		case MainMenuState.KEYBOARD_MENU:
			menu = keyboardMenuScreen;
			ih.SendKeyBindingsToGameSettings();
			gs.SaveKeyboardSettings();
			break;
		case MainMenuState.OVERSCAN_MENU:
			menu = overscanMenuScreen;
			break;
		case MainMenuState.GAME_OPTIONS_MENU:
			menu = gameOptionsMenuScreen;
			gs.SaveGameOptionsSettings();
			break;
		case MainMenuState.ACHIEVEMENTS_MENU:
			menu = achievementsMenuScreen;
			break;
		case MainMenuState.QUIT_GAME_PROMPT:
			menu = quitGamePrompt;
			break;
		case MainMenuState.RESOLUTION_PROMPT:
			menu = resolutionPrompt;
			break;
		case MainMenuState.EXIT_PROMPT:
			menu = returnMainMenuPrompt;
			break;
		case MainMenuState.BRIGHTNESS_MENU:
			menu = brightnessMenuScreen;
			if (!isGoingBack)
			{
				gs.SaveBrightnessSettings();
			}
			break;
		case MainMenuState.PAUSE_MENU:
			menu = pauseMenuScreen;
			break;
		case MainMenuState.PLAY_MODE_MENU:
			menu = playModeMenuScreen;
			disableAfterDelay = 0.5f;
			break;
		case MainMenuState.EXTRAS_MENU:
			menu = extrasMenuScreen;
			break;
		case MainMenuState.REMAP_GAMEPAD_MENU:
			menu = remapGamepadMenuScreen;
			if (uiButtonSkins.listeningButton != null)
			{
				uiButtonSkins.listeningButton.StopActionListening();
				uiButtonSkins.listeningButton.AbortRebind();
			}
			ih.SendButtonBindingsToGameSettings();
			gs.SaveGamepadSettings(ih.activeGamepadType);
			break;
		case MainMenuState.ENGAGE_MENU:
			menu = engageMenuScreen;
			break;
		}
		yield return StartCoroutine(HideMenu(menu, disableAfterDelay <= 0f));
		if (disableAfterDelay > 0f)
		{
			yield return new WaitForSeconds(disableAfterDelay);
			menu.gameObject.SetActive(value: false);
		}
		isFadingMenu = false;
	}

	public IEnumerator ShowMenu(MenuScreen menu)
	{
		isFadingMenu = true;
		ih.StopUIInput();
		StopCurrentFade(menu);
		fadingMenuScreen = menu;
		if (menu.ScreenCanvasGroup != null)
		{
			fadingRoutine = StartCoroutine(FadeInCanvasGroup(menu.ScreenCanvasGroup));
		}
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(_hideProp);
			menu.topFleur.SetTrigger(_showProp);
		}
		if (menu.bottomFleur != null)
		{
			menu.bottomFleur.ResetTrigger(_hideProp);
			menu.bottomFleur.SetTrigger(_showProp);
		}
		yield return null;
		MenuButtonList component = menu.GetComponent<MenuButtonList>();
		if ((bool)component)
		{
			component.SetupActive();
		}
		if (menu.HighlightBehaviour == MenuScreen.HighlightDefaultBehaviours.BeforeFade)
		{
			menu.HighlightDefault();
		}
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
		ih.StartUIInput();
		yield return null;
		if (menu.HighlightBehaviour == MenuScreen.HighlightDefaultBehaviours.AfterFade)
		{
			menu.HighlightDefault();
		}
		isFadingMenu = false;
	}

	private void StopCurrentFade(MenuScreen menu)
	{
		if (fadingMenuScreen == menu && fadingRoutine != null)
		{
			StopCoroutine(fadingRoutine);
			fadingRoutine = null;
		}
	}

	public IEnumerator HideMenu(MenuScreen menu, bool disable = true)
	{
		isFadingMenu = true;
		ih.StopUIInput();
		StopCurrentFade(menu);
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(_showProp);
			menu.topFleur.SetTrigger(_hideProp);
		}
		if (menu.bottomFleur != null)
		{
			menu.bottomFleur.ResetTrigger(_showProp);
			menu.bottomFleur.SetTrigger(_hideProp);
		}
		if (menu.ScreenCanvasGroup != null)
		{
			yield return StartCoroutine(FadeOutCanvasGroup(menu.ScreenCanvasGroup, disable));
		}
		ih.StartUIInput();
		isFadingMenu = false;
	}

	public void HideMenuInstant(MenuScreen menu)
	{
		ih.StopUIInput();
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(_showProp);
			menu.topFleur.SetTrigger(_hideProp);
		}
		HideCanvasGroup(menu.ScreenCanvasGroup);
		ih.StartUIInput();
	}

	public IEnumerator HideSaveProfileMenu(bool updateBlackThread)
	{
		StartCoroutine(FadeOutCanvasGroup(saveProfileTitle));
		saveProfileTopFleur.ResetTrigger(_showProp);
		saveProfileTopFleur.SetTrigger(_hideProp);
		StartCoroutine(FadeOutCanvasGroup(saveProfileControls));
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		slotOne.HideSaveSlot(updateBlackThread);
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		slotTwo.HideSaveSlot(updateBlackThread);
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		slotThree.HideSaveSlot(updateBlackThread);
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		slotFour.HideSaveSlot(updateBlackThread);
		yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.33f));
		yield return StartCoroutine(FadeOutCanvasGroup(saveProfileScreen, disable: false, stopBlocking: true));
	}

	private void DisableScreens()
	{
		for (int i = 0; i < UICanvas.transform.childCount; i++)
		{
			if (!(UICanvas.transform.GetChild(i).name == "PauseMenuScreen"))
			{
				UICanvas.transform.GetChild(i).gameObject.SetActive(value: false);
			}
		}
		if ((bool)achievementsPopupPanel)
		{
			achievementsPopupPanel.SetActive(value: true);
		}
	}

	private void ShowCanvas(Canvas canvas)
	{
		canvas.gameObject.SetActive(value: true);
	}

	private void HideCanvas(Canvas canvas)
	{
		canvas.gameObject.SetActive(value: false);
	}

	public void ShowCanvasGroup(CanvasGroup cg)
	{
		cg.gameObject.SetActive(value: true);
		cg.interactable = true;
		cg.alpha = 1f;
	}

	public void HideCanvasGroup(CanvasGroup cg)
	{
		cg.interactable = false;
		cg.alpha = 0f;
		cg.gameObject.SetActive(value: false);
	}

	public IEnumerator FadeInCanvasGroup(CanvasGroup cg, bool alwaysActive = false)
	{
		if (cg == mainMenuScreen)
		{
			MenuStyles.Instance.SetInSubMenu(value: false);
		}
		float loopFailsafe = 0f;
		cg.alpha = 0f;
		if (alwaysActive)
		{
			cg.blocksRaycasts = false;
		}
		else
		{
			cg.gameObject.SetActive(value: true);
		}
		while (cg.alpha < 1f)
		{
			cg.alpha += Time.unscaledDeltaTime * MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha >= 0.95f)
			{
				cg.alpha = 1f;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = 1f;
		cg.interactable = true;
		if (alwaysActive)
		{
			cg.blocksRaycasts = true;
		}
		yield return null;
	}

	public IEnumerator FadeInCanvasGroupAlpha(CanvasGroup cg, float endAlpha)
	{
		float loopFailsafe = 0f;
		if (endAlpha > 1f)
		{
			endAlpha = 1f;
		}
		cg.alpha = 0f;
		cg.gameObject.SetActive(value: true);
		while (cg.alpha < endAlpha - 0.05f)
		{
			cg.alpha += Time.unscaledDeltaTime * MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha >= endAlpha - 0.05f)
			{
				cg.alpha = endAlpha;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = endAlpha;
		cg.interactable = true;
		cg.gameObject.SetActive(value: true);
		yield return null;
	}

	public IEnumerator FadeOutCanvasGroup(CanvasGroup cg, bool disable = true, bool stopBlocking = false)
	{
		if (cg == mainMenuScreen)
		{
			MenuStyles.Instance.SetInSubMenu(value: true);
		}
		float loopFailsafe = 0f;
		cg.interactable = false;
		while (cg.alpha > 0.05f)
		{
			cg.alpha -= Time.unscaledDeltaTime * MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha <= 0.05f || loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = 0f;
		if (disable)
		{
			cg.gameObject.SetActive(value: false);
		}
		if (stopBlocking)
		{
			cg.blocksRaycasts = false;
		}
		yield return null;
	}

	public static void FadeOutCanvasGroupInstant(CanvasGroup cg, bool disable = true, bool stopBlocking = false)
	{
		cg.interactable = false;
		cg.alpha = 0f;
		if (disable)
		{
			cg.gameObject.SetActive(value: false);
		}
		if (stopBlocking)
		{
			cg.blocksRaycasts = false;
		}
	}

	private IEnumerator FadeInSprite(SpriteRenderer sprite)
	{
		while (sprite.color.a < 1f)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + Time.unscaledDeltaTime * MENU_FADE_SPEED);
			yield return null;
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
		yield return null;
	}

	private IEnumerator FadeOutSprite(SpriteRenderer sprite)
	{
		while (sprite.color.a > 0f)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.unscaledDeltaTime * MENU_FADE_SPEED);
			yield return null;
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
		yield return null;
	}

	public void MakeMenuLean()
	{
		Debug.Log("Making UI menu lean.");
		if ((bool)saveProfileScreen)
		{
			UnityEngine.Object.Destroy(saveProfileScreen.gameObject);
			saveProfileScreen = null;
			saveProfilePreselect = null;
			saveProfileTitle = null;
			saveProfileControls = null;
			saveProfileTopFleur = null;
			saveSlots = null;
			slotOne = null;
			slotTwo = null;
			slotThree = null;
			slotFour = null;
			blackThreadLoopAudio = null;
		}
		if ((bool)achievementsMenuScreen)
		{
			UnityEngine.Object.Destroy(achievementsMenuScreen.gameObject);
			achievementsMenuScreen = null;
		}
		if (!Platform.Current.WillDisplayGraphicsSettings)
		{
			if ((bool)videoMenuScreen)
			{
				UnityEngine.Object.Destroy(videoMenuScreen.gameObject);
				videoMenuScreen = null;
			}
			if ((bool)brightnessMenuScreen)
			{
				UnityEngine.Object.Destroy(brightnessMenuScreen.gameObject);
				brightnessMenuScreen = null;
			}
			if ((bool)overscanMenuScreen)
			{
				UnityEngine.Object.Destroy(overscanMenuScreen.gameObject);
				overscanMenuScreen = null;
			}
		}
	}

	public float FadeScreenIn()
	{
		float num = Platform.Current.EnterSceneWait;
		if (CheatManager.SceneEntryWait >= 0f)
		{
			num = CheatManager.SceneEntryWait;
		}
		FadeScreenBlankerTo(0f, screenFadeTime, num);
		return screenFadeTime + num;
	}

	public float FadeScreenOut()
	{
		FadeScreenBlankerTo(1f, screenFadeTime, 0f);
		return screenFadeTime;
	}

	private void FadeScreenBlankerTo(float alpha, float duration, float delay)
	{
		if ((bool)screenFader)
		{
			if (screenFadeRoutine != null)
			{
				StopCoroutine(screenFadeRoutine);
			}
			float startAlpha = screenFader.alpha;
			screenFadeRoutine = this.StartTimerRoutine(delay, duration, delegate(float time)
			{
				screenFader.alpha = Mathf.Lerp(startAlpha, alpha, time);
			});
		}
	}

	private void SetScreenBlankerAlpha(float alpha)
	{
		if ((bool)screenFader)
		{
			screenFader.alpha = alpha;
		}
	}

	public void BlankScreen(bool value)
	{
		if ((bool)screenFader)
		{
			screenFader.gameObject.SetActive(value: true);
			screenFader.alpha = (value ? 1f : 0f);
		}
	}
}
