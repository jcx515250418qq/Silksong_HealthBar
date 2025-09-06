using GlobalEnums;
using InControl;

public static class Constants
{
	public const string GAME_VERSION = "1.0.28324";

	public const int SILK_PARTS_COUNT = 3;

	public const int BIND_SILK_COST = 9;

	public const int BIND_SILK_COST_WITCH = 9;

	public const int MAX_SILK = 18;

	public const int SPOOL_PIECES_PER_SILK = 2;

	public const int MAX_SILK_SPOOLS = 18;

	public const int SILK_SKILL_COST = 4;

	public const int SILK_SKILL_COST_FLEACHARM = 3;

	public const int STARTING_HEALTH = 5;

	public const int MAX_HEALTH = 10;

	public const int HEART_PIECES_PER_HEALTH = 4;

	public const int MAX_HEART_PIECES = 20;

	public const int BLUE_HEALTH_MID = 5;

	public const int BLUE_HEALTH_FINAL = 7;

	public const int MAX_BLUE_HEALTH = 9;

	public const int MAX_SILK_REGEN = 3;

	public const int MAP_MARKER_COUNT = 9;

	public const int FLEA_FESTIVAL_CHAMP_JUGGLE = 30;

	public const int FLEA_FESTIVAL_SETH_JUGGLE = 55;

	public const int FLEA_FESTIVAL_CHAMP_DODGE = 65;

	public const int FLEA_FESTIVAL_SETH_DODGE = 95;

	public const int FLEA_FESTIVAL_CHAMP_BOUNCE = 42;

	public const int FLEA_FESTIVAL_SETH_BOUNCE = 68;

	public const float CORAL_SPEAR_SPAWN_TIME = 2f;

	public const int MAGGOT_CHARM_MAX_HITS = 3;

	public const float DEFAULT_TIMESCALE = 1f;

	public const float PAUSED_TIMESCALE = 0f;

	public const float FRAME_WAIT = 0.165f;

	public const float TIME_SCALE_CHANGE_RATE = 1E-05f;

	public const float SCENE_TRANSITION_WAIT = 0.34f;

	public const float SCENE_TRANSITION_FADE = 0.25f;

	public const float HERO_DEFAULT_GRAVITY = 0.79f;

	public const float HERO_UNDERWATER_GRAVITY = 0.225f;

	public const float RAYCAST_EXTENTS = 0.16f;

	public const float MIN_WALL_HEIGHT = 0.2f;

	public const float INPUT_LOWER_SNAP_V = 0.5f;

	public const float INPUT_LOWER_SNAP_H = 0.3f;

	public const float INPUT_UPPER_SNAP = 0.9f;

	public const float INPUT_DEADZONE_L = 0.15f;

	public const float INPUT_DEADZONE_U = 0.95f;

	public const float CAM_Z_DEFAULT = -38.1f;

	public const float CAM_BOUND_X = 14.6f;

	public const float CAM_BOUND_Y = 8.3f;

	public const float CAM_HOR_OFFSET_AMOUNT = 1f;

	public const float CAM_FALL_VELOCITY = -20f;

	public const float CAM_FALL_OFFSET = -4f;

	public const float CAM_LOOK_OFFSET = 6f;

	public const float CAM_START_LOCKED_TIMER = 0.65f;

	public const float CAM_HAZARD_RESPAWN_FROZEN = 0.5f;

	public const float CAM_MENU_X = 14.6f;

	public const float CAM_MENU_Y = 8.5f;

	public const float CAM_CIN_X = 14.6f;

	public const float CAM_CIN_Y = 8.5f;

	public const float CAM_CUT_X = 14.6f;

	public const float CAM_CUT_Y = 8.5f;

	public const float CAM_STAG_PRE_FADEOUT = 0.6f;

	public const float CAM_FADE_TIME_START_FADE = 2.3f;

	public const float CAM_DEFAULT_BLUR_DEPTH = 6.62f;

	public const float CAM_DEFAULT_SATURATION = 0.7f;

	public const float CAM_DEFAULT_INTENSITY = 0.7f;

	public const float MIN_VIEW_DEPTH = 10f;

	public const float MAX_VIEW_DEPTH = 1000f;

	public const float CAM_OVERLAP = 1E-05f;

	public const float CAM_ORTHOSIZE = 8.710664f;

	public const float CAM_CANVAS_MOVE_WAIT = 0.5f;

	public const float CINEMATIC_SKIP_FADE_TIME = 0.3f;

	public const float CAM_GAME_ASPECT_REF = 1.7777778f;

	public const float CAM_GAME_ASPECT_WIDE = 2.3916667f;

	public const float CAM_GAME_ASPECT_TALL = 1.6f;

	public const string CAM_SHAKE_ENEMYKILL = "EnemyKillShake";

	public const float SCENE_POSITION_LIMIT = 60f;

	public const string MENU_SCENE = "Menu_Title";

	public const string FIRST_LEVEL_NAME = "Tut_01";

	public const string FIRST_LEVEL_RESPAWN_POINT = "Death Respawn Marker Init";

	public const string STARTING_SCENE = "Opening_Sequence";

	public const string INTRO_PROLOGUE = "Intro_Cutscene_Prologue";

	public const string OPENING_CUTSCENE = "Intro_Cutscene";

	public const string STAG_CINEMATIC = "Cinematic_Stag_travel";

	public const string PERMADEATH_LEVEL = "PermaDeath";

	public const string PERMADEATH_UNLOCK = "PermaDeath_Unlock";

	public const string MRMUSHROOM_CINEMATIC = "Cinematic_MrMushroom";

	public const string ENDING_A_CINEMATIC = "Cinematic_Ending_A";

	public const string ENDING_B_CINEMATIC = "Cinematic_Ending_B";

	public const string ENDING_C_CINEMATIC = "Cinematic_Ending_C";

	public const string ENDING_D_CINEMATIC = "Cinematic_Ending_D";

	public const string ENDING_E_CINEMATIC = "Cinematic_Ending_E";

	public const string END_CREDITS = "End_Credits";

	public const string END_CREDITS_SCROLL = "End_Credits_Scroll";

	public const string MENU_CREDITS = "Menu_Credits";

	public const string TITLE_SCREEN_LEVEL = "Title_Screens";

	public const string TUTORIAL_LEVEL = "Tutorial_01";

	public const string BOSS_DOOR_CUTSCENE = "Cutscene_Boss_Door";

	public const string GAME_COMPLETION_SCREEN = "End_Game_Completion";

	public const string BOSSRUSH_END_SCENE = "GG_End_Sequence";

	public const string GG_ENTRANCE_SCENE = "GG_Entrance_Cutscene";

	public const string GG_DOOR_ENTRANCE_SCENE = "GG_Boss_Door_Entrance";

	public const string GG_RETURN_SCENE = "GG_Waterways";

	public const string DUST_MAZE_ENTRANCE_SCENE = "Dust_Maze_09_entrance";

	public const string SAVE_ICON_START_EVENT = "GAME SAVING";

	public const string SAVE_ICON_END_EVENT = "GAME SAVED";

	public const float HERO_Z = 0.004f;

	public const float HAZARD_DEATH_WAIT = 0f;

	public const float RESPAWN_FADEOUT_WAIT = 0.65f;

	public const float HAZ_RESPAWN_FADEIN_WAIT = 0.1f;

	public const float SCENE_ENTER_WAIT = 0.33f;

	public const float QUICKENING_PITCH_OFFSET = 0.05f;

	public const int MID_SHARD_VALUE = 5;

	public const float CAMERA_MARGIN_X = 15f;

	public const float CAMERA_MARGIN_Y = 9f;

	public const float CUTSCENE_PROMPT_TIMEOUT = 3f;

	public const float CUTSCENE_PROMPT_SKIP_COOLDOWN = 0.3f;

	public const float SAVE_FLEUR_PAUSE = 0.1f;

	public const float AREA_TITLE_UI_MSG_Y = -4f;

	public const string RECORD_PERMADEATH_MODE = "RecPermadeathMode";

	public const string RECORD_BOSSRUSH_MODE = "RecBossRushMode";

	public const SupportedLanguages DEFAULT_LANGUAGE = SupportedLanguages.EN;

	public const int DEFAULT_BACKERCREDITS = 0;

	public const int DEFAULT_NATIVEPOPUPS = 0;

	public const float MM_AUDIO_MASTER_VOL = 10f;

	public const float MM_AUDIO_MUSIC_VOL = 10f;

	public const float MM_AUDIO_SOUND_VOL = 10f;

	public const int MM_VIDEO_RESX = 1920;

	public const int MM_VIDEO_RESY = 1080;

	public const int MM_VIDEO_FULLSCREEN = 2;

	public const int MM_VIDEO_VSYNC = 1;

	public const int DEFAULT_TARGET_FRAME_RATE = 60;

	public const int DEFAULT_DISPLAY = 0;

	public const int DEFAULT_VIDEO_PARTICLES = 1;

	public const ShaderQualities DEFAULT_VIDEO_SHADER_QUALITY = ShaderQualities.High;

	public const float MM_OS_MAINCAM = 1f;

	public const float MM_OS_HUDCAM = 8.710664f;

	public const float MM_OS_DEFAULT = 0f;

	public const float DEFAULT_BRIGHTNESS = 20f;

	public const GamepadType MM_INPUTTYPE = GamepadType.NONE;

	public const ControllerProfile MM_INPUTPROFILE = ControllerProfile.Default;

	public const Key DEFAULT_KEY_JUMP = Key.Z;

	public const Key DEFAULT_KEY_ATTACK = Key.X;

	public const Key DEFAULT_KEY_DASH = Key.C;

	public const Key DEFAULT_KEY_CAST = Key.A;

	public const Key DEFAULT_KEY_SUPERDASH = Key.S;

	public const Key DEFAULT_KEY_DREAMNAIL = Key.D;

	public const Key DEFAULT_KEY_QUICKMAP = Key.Tab;

	public const Key DEFAULT_KEY_QUICKCAST = Key.F;

	public const Key DEFAULT_KEY_INVENTORY = Key.I;

	public const Key DEFAULT_KEY_INVENTORY_MAP = Key.M;

	public const Key DEFAULT_KEY_INVENTORY_JOURNAL = Key.J;

	public const Key DEFAULT_KEY_INVENTORY_TOOLS = Key.Q;

	public const Key DEFAULT_KEY_INVENTORY_QUESTS = Key.T;

	public const Key DEFAULT_KEY_TAUNT = Key.V;

	public const Key DEFAULT_KEY_UP = Key.UpArrow;

	public const Key DEFAULT_KEY_DOWN = Key.DownArrow;

	public const Key DEFAULT_KEY_LEFT = Key.LeftArrow;

	public const Key DEFAULT_KEY_RIGHT = Key.RightArrow;

	public const InputControlType BUTTON_DEFAULT_JUMP = InputControlType.Action1;

	public const InputControlType BUTTON_DEFAULT_ATTACK = InputControlType.Action3;

	public const InputControlType BUTTON_DEFAULT_CAST = InputControlType.Action2;

	public const InputControlType BUTTON_DEFAULT_DASH = InputControlType.RightTrigger;

	public const InputControlType BUTTON_DEFAULT_SUPERDASH = InputControlType.LeftTrigger;

	public const InputControlType BUTTON_DEFAULT_DREAMNAIL = InputControlType.Action4;

	public const InputControlType BUTTON_DEFAULT_QUICKMAP = InputControlType.LeftBumper;

	public const InputControlType BUTTON_DEFAULT_QUICKCAST = InputControlType.RightBumper;

	public const InputControlType BUTTON_DEFAULT_TAUNT = InputControlType.RightStickButton;

	public const InputControlType BUTTON_DEFAULT_INVENTORY = InputControlType.Back;

	public const InputControlType BUTTON_DEFAULT_PS4_INVENTORY = InputControlType.TouchPadButton;

	public const InputControlType BUTTON_DEFAULT_PS4_PAUSE = InputControlType.Options;

	public const InputControlType BUTTON_DEFAULT_PS5_INVENTORY = InputControlType.TouchPadButton;

	public const InputControlType BUTTON_DEFAULT_PS5_PAUSE = InputControlType.Options;

	public const InputControlType BUTTON_DEFAULT_XBONE_INVENTORY = InputControlType.View;

	public const InputControlType BUTTON_DEFAULT_XBONE_PAUSE = InputControlType.Menu;

	public const InputControlType BUTTON_DEFAULT_PS3_WIN_INVENTORY = InputControlType.Select;

	public const InputControlType BUTTON_DEFAULT_SWITCH_INVENTORY = InputControlType.Minus;

	public const InputControlType BUTTON_DEFAULT_SWITCH_PAUSE = InputControlType.Plus;

	public const InputControlType BUTTON_DEFAULT_UNKNOWN_INVENTORY = InputControlType.Select;

	public const string GSKEY_GAME_LANGUAGE = "GameLang";

	public const string GSKEY_GAME_BACKERS = "GameBackers";

	public const string GSKEY_GAME_POPUPS = "GameNativePopups";

	public const string GSKEY_RUMBLE_SETTING = "RumbleSetting";

	public const string GSKEY_CAMSHAKE_SETTING = "CameraShakeSetting";

	public const string GSKEY_HUDSCALE_SETTING = "HudScaleSetting";

	public const string GSKEY_VIDEO_RESX = "VidResX";

	public const string GSKEY_VIDEO_RESY = "VidResY";

	public const string GSKEY_VIDEO_REFRESH = "VidResRefresh";

	public const string GSKEY_VIDEO_FULLSCREEN = "VidFullscreen";

	public const string GSKEY_VIDEO_VSYNC = "VidVSync";

	public const string GSKEY_VIDEO_DISPLAY = "VidDisplay";

	public const string GSKEY_VIDEO_FRAME_CAP = "VidTFR";

	public const string GSKEY_VIDEO_PARTICLES = "VidParticles";

	public const string GSKEY_VIDEO_SHADER_QUALITY = "ShaderQuality";

	public const string GSKEY_OS_VALUE = "VidOSValue";

	public const string GSKEY_OS_SET = "VidOSSet";

	public const string GSKEY_BRIGHT_VALUE = "VidBrightValue";

	public const string GSKEY_BRIGHT_SET = "VidBrightSet";

	public const string GSKEY_AUDIO_MASTER = "MasterVolume";

	public const string GSKEY_AUDIO_MUSIC = "MusicVolume";

	public const string GSKEY_AUDIO_SOUND = "SoundVolume";

	public const string GSKEY_AUDIO_PLAYERVOICE = "PlayerVoiceEnabled";

	public const string GSKEY_KEY_JUMP = "KeyJump";

	public const string GSKEY_KEY_ATTACK = "KeyAttack";

	public const string GSKEY_KEY_DASH = "KeyDash";

	public const string GSKEY_KEY_CAST = "KeyCast";

	public const string GSKEY_KEY_SUPERDASH = "KeySupDash";

	public const string GSKEY_KEY_DREAMNAIL = "KeyDreamnail";

	public const string GSKEY_KEY_QUICKMAP = "KeyQuickMap";

	public const string GSKEY_KEY_QUICKCAST = "KeyQuickCast";

	public const string GSKEY_KEY_TAUNT = "KeyTaunt";

	public const string GSKEY_KEY_INVENTORY = "KeyInventory";

	public const string GSKEY_KEY_INVENTORY_MAP = "KeyInventoryMap";

	public const string GSKEY_KEY_INVENTORY_JOURNAL = "KeyInventoryJournal";

	public const string GSKEY_KEY_INVENTORY_TOOLS = "KeyInventoryTools";

	public const string GSKEY_KEY_INVENTORY_QUESTS = "KeyInventoryQuests";

	public const string GSKEY_KEY_UP = "KeyUp";

	public const string GSKEY_KEY_DOWN = "KeyDown";

	public const string GSKEY_KEY_LEFT = "KeyLeft";

	public const string GSKEY_KEY_RIGHT = "KeyRight";

	public const string GSKEY_CONTROLLER_PREFIX = "Controller";

	public const string GSKEY_LANG_SET = "GameLangSet";

	public const string COMM_ARG_RESETALL = "-resetall";

	public const string COMM_ARG_RESETRES = "-resetres";

	public const string COMM_ARG_ALLOWLANG = "-forcelang";

	public const string COMM_ARG_DEBUGKEYS = "-allowdebug";

	public const string COMM_ARG_NATIVEINPUT = "-nativeinput";

	private static readonly FieldCache fieldCache = new FieldCache(typeof(Constants));

	public static T GetConstantValue<T>(string variableName)
	{
		return fieldCache.GetValue<T>(variableName);
	}
}
