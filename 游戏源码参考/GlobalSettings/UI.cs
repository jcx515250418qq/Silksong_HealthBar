using TeamCherry.Localization;
using UnityEngine;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global UI Settings")]
	public class UI : GlobalSettingsBase<UI>
	{
		[SerializeField]
		private Color disabledUiTextColor;

		[Space]
		[SerializeField]
		private float newDotScaleTime = 0.1f;

		[SerializeField]
		private float newDotScaleDelay = 0.2f;

		[Space]
		[SerializeField]
		private Vector3 uiMsgPopupStartPosition;

		[SerializeField]
		private Vector3 uiMsgPopupStackOffset;

		[SerializeField]
		private CollectableUIMsg collectableUIMsgPrefab;

		[SerializeField]
		private CrestSlotUnlockMsg crestSlotUnlockMsgPrefab;

		[Space]
		[SerializeField]
		private ToolTutorialMsg toolTutorialMsgPrefab;

		[Space]
		[SerializeField]
		[ArrayForEnum(typeof(ToolItemType))]
		private Color[] toolTypeColors;

		[SerializeField]
		private LocalisedString maxItemsPopup;

		[SerializeField]
		private Color maxItemsTextColor;

		[SerializeField]
		private LocalisedString itemTakenPopup;

		[SerializeField]
		private LocalisedString itemDepositedPopup;

		[SerializeField]
		private LocalisedString itemGivenPopup;

		[SerializeField]
		private float itemGivenPopupDelay;

		[SerializeField]
		private LocalisedString destroyedPopup;

		[Space]
		[SerializeField]
		private LocalisedString questContinuePopup;

		[SerializeField]
		private AudioEvent questContinuePopupSound;

		[Space]
		[SerializeField]
		private LocalisedString mainQuestBeginPopup;

		[SerializeField]
		private LocalisedString mainQuestProgressPopup;

		[SerializeField]
		private LocalisedString mainQuestCompletePopup;

		[Space]
		[SerializeField]
		private float itemQuestMaxPopupDelay;

		[SerializeField]
		private LocalisedString itemQuestMaxPopup;

		[SerializeField]
		private AudioEvent itemQuestMaxPopupSound;

		[SerializeField]
		private GameObject mapUpdatedPopupPrefab;

		private bool validated;

		public static Color DisabledUiTextColor => Get().disabledUiTextColor;

		public static float NewDotScaleTime => Get().newDotScaleTime;

		public static float NewDotScaleDelay => Get().newDotScaleDelay;

		public static Vector3 UIMsgPopupStartPosition => Get().uiMsgPopupStartPosition;

		public static Vector3 UIMsgPopupStackOffset => Get().uiMsgPopupStackOffset;

		public static CollectableUIMsg CollectableUIMsgPrefab => Get().collectableUIMsgPrefab;

		public static CrestSlotUnlockMsg CrestSlotUnlockMsgPrefab => Get().crestSlotUnlockMsgPrefab;

		public static ToolTutorialMsg ToolTutorialMsgPrefab => Get().toolTutorialMsgPrefab;

		public static LocalisedString MaxItemsPopup => Get().maxItemsPopup;

		public static Color MaxItemsTextColor => Get().maxItemsTextColor;

		public static LocalisedString ItemTakenPopup => Get().itemTakenPopup;

		public static LocalisedString ItemDepositedPopup => Get().itemDepositedPopup;

		public static LocalisedString ItemGivenPopup => Get().itemGivenPopup;

		public static float ItemGivenPopupDelay => Get().itemGivenPopupDelay;

		public static LocalisedString DestroyedPopup => Get().destroyedPopup;

		public static LocalisedString QuestContinuePopup => Get().questContinuePopup;

		public static AudioEvent QuestContinuePopupSound => Get().questContinuePopupSound;

		public static LocalisedString MainQuestBeginPopup => Get().mainQuestBeginPopup;

		public static LocalisedString MainQuestProgressPopup => Get().mainQuestProgressPopup;

		public static LocalisedString MainQuestCompletePopup => Get().mainQuestCompletePopup;

		public static float ItemQuestMaxPopupDelay => Get().itemQuestMaxPopupDelay;

		public static LocalisedString ItemQuestMaxPopup => Get().itemQuestMaxPopup;

		public static AudioEvent ItemQuestMaxPopupSound => Get().itemQuestMaxPopupSound;

		public static GameObject MapUpdatedPopupPrefab => Get().mapUpdatedPopupPrefab;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<UI>.StartPreloadAddressable("Global UI Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<UI>.StartUnload();
		}

		private static UI Get()
		{
			return GlobalSettingsBase<UI>.Get("Global UI Settings");
		}

		public static Color GetToolTypeColor(ToolItemType type)
		{
			UI uI = Get();
			uI.EnsureColorsArray();
			return uI.toolTypeColors[(int)type];
		}

		private void OnValidate()
		{
			ArrayForEnumAttribute.EnsureArraySize(ref toolTypeColors, typeof(ToolItemType));
		}

		private void EnsureColorsArray()
		{
			if (!validated)
			{
				validated = true;
				ArrayForEnumAttribute.EnsureArraySize(ref toolTypeColors, typeof(ToolItemType));
			}
		}
	}
}
