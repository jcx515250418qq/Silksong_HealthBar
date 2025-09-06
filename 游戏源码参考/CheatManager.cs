using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class CheatManager : MonoBehaviour, IOnGUI
{
	private enum MenuStates
	{
		None = -1,
		Main = 0,
		Abilities = 1,
		System = 2,
		Teleport = 3,
		PlayerData = 4,
		Collectables = 5,
		Tools = 6,
		Quests = 7,
		SaveManagement = 8,
		Achievements = 9,
		GCPressure = 10,
		SubMenu = 11
	}

	private enum TouchKeyboardStates
	{
		Closed = 0,
		OpenPdSearch = 1,
		OpenPdSet = 2,
		OpenObjectSearch = 3
	}

	private enum ButtonInputState
	{
		None = 0,
		Confirm = 1,
		Left = 2,
		Right = 3
	}

	public enum InvincibilityStates
	{
		Off = 0,
		TestInvincible = 1,
		FullInvincible = 2,
		PreventDeath = 3
	}

	public enum NailDamageStates
	{
		Normal = 0,
		InstaKill = 1,
		NoDamage = 2
	}

	public enum DamageSelfStates
	{
		None = 0,
		SingleHit = 1,
		DoubleHit = 2,
		Death = 3
	}

	private const float BUTTON_HEIGHT = 20f;

	private const float BUTTON_WIDTH = 350f;

	private const float SPACE_HEIGHT = 5f;

	private const float BASE_SCREEN_HEIGHT = 1080f;

	private const float X_INDENT = 15f;

	private const float Y_INDENT = 15f;

	private const int FONT_SIZE = 12;

	private const float MAX_SCALE_INCREASE = 5f;

	private const float MIN_SCALE = 0.5f;

	private static GUIStyle advanceButtonStyle;

	private static float lastMaxWidth;

	private static CheatManager _instance;

	private bool wasEverOpened;

	private int selectedButtonIndex;

	private int nextSelectedButtonIndex = -1;

	private int guiOffsetButtons;

	private int spaces;

	private int indentLevel;

	private bool isQuickHealEnabled;

	private bool isRegenerating;

	private static bool forceStun;

	private bool isFastTravelTeleportCheckActive;

	private int safetyCounter;

	private const int SAFETY_AMOUNT = 10;

	private const string TOGGLE_BUTTON_CHECKMARK = "■";

	private int selectDelta;

	private MenuStates menuState;

	private MenuStates queuedMenuState = MenuStates.None;

	private bool disableAlphaInput;

	private string textInputString;

	private string playerDataSearchString;

	private TouchScreenKeyboard touchKeyboard;

	private TouchKeyboardStates touchKeyboardState;

	private string pdSetFieldName;

	private string pdSetString;

	private string objectSearchString;

	private int invPaneHiddenBitmask;

	private readonly Dictionary<string, bool> tpMapGroupFoldout = new Dictionary<string, bool>();

	private readonly Dictionary<string, bool> tpMapSceneFoldout = new Dictionary<string, bool>();

	private readonly Dictionary<string, bool> crestFoldout = new Dictionary<string, bool>();

	private readonly Dictionary<string, bool> toolFoldout = new Dictionary<string, bool>();

	private readonly Dictionary<string, bool> questFoldout = new Dictionary<string, bool>();

	private bool isDebugHeatEffect;

	private static readonly int _heatEffectMultProp = Shader.PropertyToID("_HeatEffectMult");

	private float slowOpenLeftStickTimer;

	private float slowOpenRightStickTimer;

	private float fastOpenTimer;

	private float holdTime;

	private float tickTime;

	private bool isCrosshairEnabled;

	private Material lineMaterial;

	private readonly Vector2[] currentLine = new Vector2[2];

	private InputCapture inputCapture;

	private int updateFrames = -1;

	private int mainMenuIndex = -1;

	public static float Multiplier
	{
		get
		{
			if ((float)Screen.height > 1080f)
			{
				return (float)Screen.height / 1080f * 1.25f + MultiplierChange;
			}
			return 1f + MultiplierChange;
		}
	}

	public static float MultiplierChange { get; private set; }

	public static float ButtonHeight => 20f * Multiplier;

	public static float ButtonWidth => Mathf.Max(350f, lastMaxWidth) * Multiplier;

	public static float SpaceHeight => 5f * Multiplier;

	public static float XIndent => 15f * Multiplier;

	public static float YIndent => 15f * Multiplier;

	public static int FontSize => Mathf.RoundToInt(12f * Multiplier);

	public static GUIStyle LabelStyle
	{
		get
		{
			if (advanceButtonStyle == null)
			{
				advanceButtonStyle = new GUIStyle(GUI.skin.label);
			}
			advanceButtonStyle.fontSize = FontSize;
			return advanceButtonStyle;
		}
	}

	public static bool IsCheatsEnabled => false;

	public static bool isQuickSilkEnabled { get; set; }

	public static DamageSelfStates DamageSelfState { get; set; }

	public static HazardType HazardType { get; set; } = HazardType.NON_HAZARD;

	public static NailDamageStates NailDamage { get; set; }

	public static InvincibilityStates Invincibility { get; set; }

	public static bool ForceNextHitStun { get; set; }

	public static bool ForceStun
	{
		get
		{
			return forceStun;
		}
		set
		{
			forceStun = value;
		}
	}

	public static bool IsTextPrintSkipEnabled { get; set; }

	public static bool IsFrostDisabled { get; set; }

	public static bool CanChangeEquipsAnywhere { get; set; }

	public static bool AllowSaving => true;

	public static bool UseFieldAccessOptimisers { get; set; } = true;

	public static bool ColliderCastForGroundCheck { get; set; } = false;

	public static bool DisableAsyncSceneLoad { get; set; }

	public static bool UseAsyncSaveFileLoad { get; set; }

	public static bool UseTasksForJsonConversion { get; set; } = true;

	public static bool ForceCurrencyCountersAppear { get; set; }

	public static bool DisableMusicSync { get; private set; }

	public static bool SuperLuckyDice { get; private set; }

	public static bool ForceLanguageComponentUpdates { get; set; }

	public static bool OverrideFrameSkip { get; private set; } = false;

	public static bool OverrideReadyWaitFrames { get; set; }

	public static int ReadyWaitFrames { get; set; } = 1;

	public static bool OverrideFastTravelBackgroundLoadPriority { get; private set; }

	public static ThreadPriority BackgroundLoadPriority { get; private set; }

	public static bool OverrideAsyncLoadPriority { get; private set; }

	public static int AsyncLoadPriority { get; private set; } = 50;

	public static bool OverrideSkipFrameOnDrop { get; private set; }

	public static bool SkipVideoFrameOnDrop { get; private set; } = true;

	public static bool FakeCorruptedState { get; private set; }

	public static bool FakeIncompatibleState { get; private set; }

	public static bool AlwaysAwardAchievement { get; set; }

	public static bool ShowAllQuestBoardQuest { get; set; }

	public static float SceneEntryWait { get; set; } = -0.1f;

	public static bool BoostModeActive { get; set; }

	public static bool IsOpen { get; private set; }

	public static bool IsStackTracesEnabled { get; private set; }

	public static bool IsDialogueDebugEnabled { get; set; }

	public static bool IsWorldRumbleDisabled { get; set; }

	public static bool IsSilkDrainDisabled { get; set; }

	public static string LastErrorText { get; set; }

	public static bool IsForcingUnloads { get; private set; }

	public static bool OverrideSceneLoadPriority { get; private set; }

	public static int SceneLoadPriority { get; private set; }

	public static bool ShowAllCompletionIcons { get; private set; } = false;

	public static bool PS5BlockRefreshModeChange { get; set; }

	public static bool ForceChosenBlackThreadAttack { get; set; }

	public static BlackThreadAttack ChosenBlackThreadAttack { get; set; }

	public int GUIDepth => 10;

	public static bool AddVibrationAmplitudes { get; private set; }

	public static bool EnableLogMessages { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		if (IsCheatsEnabled)
		{
			Object.DontDestroyOnLoad(new GameObject("CheatManager", typeof(CheatManager)));
			PerformanceHud.Init();
		}
	}

	public static void ReInit()
	{
		if ((bool)_instance)
		{
			Object.Destroy(_instance.gameObject);
		}
		Init();
	}

	protected IEnumerator Start()
	{
		_instance = this;
		lineMaterial = new Material(Shader.Find("Sprites/Default"));
		CreateSubMenus();
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (!isRegenerating)
			{
				continue;
			}
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (!(unsafeInstance == null))
			{
				HeroController hero_ctrl = unsafeInstance.hero_ctrl;
				if (!(hero_ctrl == null))
				{
					hero_ctrl.AddHealth(Mathf.Clamp(unsafeInstance.playerData.maxHealth - unsafeInstance.playerData.health, 0, 1));
					hero_ctrl.AddSilk(Mathf.Clamp(unsafeInstance.playerData.CurrentSilkMax - unsafeInstance.playerData.silk, 0, 3), heroEffect: false);
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			IsOpen = false;
		}
		GUIDrawer.RemoveDrawer(this);
		if (lineMaterial != null)
		{
			Object.Destroy(lineMaterial);
			lineMaterial = null;
		}
	}

	public static bool IsInventoryPaneHidden(InventoryPaneList.PaneTypes paneType)
	{
		if ((bool)_instance)
		{
			return _instance.invPaneHiddenBitmask.IsBitSet((int)paneType);
		}
		return false;
	}

	public void DrawGUI()
	{
	}

	private static string ColorTextRed(string text)
	{
		return "<color=red>" + text + "</color>";
	}

	private static string ColorTextGreen(string text)
	{
		return "<color=green>" + text + "</color>";
	}

	private void CreateSubMenus()
	{
	}
}
