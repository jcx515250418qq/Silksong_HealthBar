using System;
using UnityEngine;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Demo Settings")]
	public class Demo : GlobalSettingsBase<Demo>
	{
		[Serializable]
		public struct SaveFileData
		{
			[Tooltip("Insert an UNENCRYPTED .txt save file.")]
			public TextAsset SaveFile;

			[Tooltip("Is this save file for display purposes only? Will run regular game start when selected.")]
			public bool IsDummySave;
		}

		[SerializeField]
		private SaveFileData[] saveFileOverrides;

		[SerializeField]
		private int maxDeathCount;

		public static int MaxDeathCount => Get().maxDeathCount;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Demo>.StartPreloadAddressable("Global Demo Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<Demo>.StartUnload();
		}

		private static Demo Get()
		{
			return GlobalSettingsBase<Demo>.Get("Global Demo Settings");
		}

		private void OnValidate()
		{
			if (saveFileOverrides.Length != 4)
			{
				SaveFileData[] array = new SaveFileData[4];
				for (int i = 0; i < Mathf.Min(array.Length, saveFileOverrides.Length); i++)
				{
					array[i] = saveFileOverrides[i];
				}
				saveFileOverrides = array;
			}
		}

		public static SaveFileData GetSaveFileOverride(int index)
		{
			Demo demo = Get();
			if (index < 0 || demo.saveFileOverrides == null || index >= demo.saveFileOverrides.Length)
			{
				return default(SaveFileData);
			}
			return demo.saveFileOverrides[index];
		}
	}
}
