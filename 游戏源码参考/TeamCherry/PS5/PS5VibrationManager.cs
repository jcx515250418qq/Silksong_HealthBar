using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry.PS5
{
	public static class PS5VibrationManager
	{
		public const string VIBRATION_LABEL = "ps5 vibration";

		public const string MANAGER_ASSET_NAME = "PS5 Vibration Manager Asset";

		public const string PS5_VIBRATION_ASSET_PATH = "Assets/Audio/Vibration Files/PS5";

		private static readonly Dictionary<AudioClip, PS5VibrationData> vibrations;

		private static Dictionary<AudioClip, PS5VibrationData> Vibrations => vibrations;

		static PS5VibrationManager()
		{
			Debug.Log("Initialising Vibration Manager");
			vibrations = new Dictionary<AudioClip, PS5VibrationData>();
		}

		public static AudioClip GetVibrationClip(AudioClip clip)
		{
			if (Vibrations.TryGetValue(clip, out var value))
			{
				Debug.Log($"Found vibration data for {clip} - {value}");
				if ((bool)value.VibrationClip)
				{
					Debug.Log($"Found {value.VibrationClip} vibration for {clip} audio", value);
					return value.VibrationClip;
				}
				Debug.LogError($"No vibration clip has been set for {clip}", clip);
			}
			else
			{
				Debug.LogError($"Failed to find vibration data for {clip}", clip);
			}
			return clip;
		}
	}
}
