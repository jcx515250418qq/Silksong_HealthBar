using UnityEngine;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Audio Settings")]
	public class Audio : GlobalSettingsBase<Audio>
	{
		[SerializeField]
		private float audioEventFrequencyLimit = 0.02f;

		[SerializeField]
		private AudioSource defaultAudioSourcePrefab;

		[SerializeField]
		private AudioSource default2DAudioSourcePrefab;

		[SerializeField]
		private AudioSource defaultUIAudioSourcePrefab;

		[Space]
		[SerializeField]
		private AudioEvent inventorySelectionMoveSound;

		[SerializeField]
		private AudioEvent stopConfirmSound;

		[Space]
		[SerializeField]
		private RandomAudioClipTable corpseSpikeLandAudioTable;

		[SerializeField]
		private RandomAudioClipTable objectSpikeLandAudioTable;

		public static float AudioEventFrequencyLimit => Get().audioEventFrequencyLimit;

		public static AudioSource DefaultAudioSourcePrefab => Get().defaultAudioSourcePrefab;

		public static AudioSource Default2DAudioSourcePrefab => Get().default2DAudioSourcePrefab;

		public static AudioSource DefaultUIAudioSourcePrefab => Get().defaultUIAudioSourcePrefab;

		public static AudioEvent InventorySelectionMoveSound => Get().inventorySelectionMoveSound;

		public static AudioEvent StopConfirmSound => Get().stopConfirmSound;

		public static RandomAudioClipTable CorpseSpikeLandAudioTable => Get().corpseSpikeLandAudioTable;

		public static RandomAudioClipTable ObjectSpikeLandAudioTable => Get().objectSpikeLandAudioTable;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Audio>.StartPreloadAddressable("Global Audio Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<Audio>.StartUnload();
		}

		private static Audio Get()
		{
			return GlobalSettingsBase<Audio>.Get("Global Audio Settings");
		}
	}
}
