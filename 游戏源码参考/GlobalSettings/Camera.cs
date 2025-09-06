using UnityEngine;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Camera Settings")]
	public class Camera : GlobalSettingsBase<Camera>
	{
		[SerializeField]
		private CameraManagerReference mainCameraShakeManager;

		[Header("Legacy Shake Replacements")]
		[SerializeField]
		private CameraShakeProfile bigShake;

		[SerializeField]
		private CameraShakeProfile bigShakeQuick;

		[SerializeField]
		private CameraShakeProfile tinyShake;

		[SerializeField]
		private CameraShakeProfile smallShake;

		[SerializeField]
		private CameraShakeProfile averageShake;

		[SerializeField]
		private CameraShakeProfile averageShakeQuick;

		[SerializeField]
		private CameraShakeProfile enemyKillShake;

		[SerializeField]
		private CameraShakeProfile tinyRumble;

		[SerializeField]
		private CameraShakeProfile smallRumble;

		[SerializeField]
		private CameraShakeProfile medRumble;

		[SerializeField]
		private CameraShakeProfile bigRumble;

		public static CameraManagerReference MainCameraShakeManager => Get().mainCameraShakeManager;

		public static CameraShakeProfile BigShake => Get().bigShake;

		public static CameraShakeProfile BigShakeQuick => Get().bigShakeQuick;

		public static CameraShakeProfile TinyShake => Get().tinyShake;

		public static CameraShakeProfile SmallShake => Get().smallShake;

		public static CameraShakeProfile AverageShake => Get().averageShake;

		public static CameraShakeProfile AverageShakeQuick => Get().averageShakeQuick;

		public static CameraShakeProfile EnemyKillShake => Get().enemyKillShake;

		public static CameraShakeProfile TinyRumble => Get().tinyRumble;

		public static CameraShakeProfile SmallRumble => Get().smallRumble;

		public static CameraShakeProfile MedRumble => Get().medRumble;

		public static CameraShakeProfile BigRumble => Get().bigRumble;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Camera>.StartPreloadAddressable("Global Camera Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<Camera>.StartUnload();
		}

		private static Camera Get()
		{
			return GlobalSettingsBase<Camera>.Get("Global Camera Settings");
		}
	}
}
