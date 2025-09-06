using UnityEngine;

namespace TeamCherry.PS5
{
	[CreateAssetMenu(menuName = "Platform/PS5/Vibration Asset")]
	public sealed class PS5VibrationData : ScriptableObject
	{
		[SerializeField]
		private AudioClip vibrationClip;

		[SerializeField]
		private AssetLinker<AudioClip> audioClipSource = new AssetLinker<AudioClip>();

		[SerializeField]
		private bool preventOverride;

		public AudioClip VibrationClip => vibrationClip;

		public AudioClip ClipSource => audioClipSource.Asset;

		public bool PreventOverride => preventOverride;

		public static implicit operator AudioClip(PS5VibrationData vibrationData)
		{
			if ((bool)vibrationData.VibrationClip)
			{
				return vibrationData.VibrationClip;
			}
			return vibrationData.ClipSource;
		}
	}
}
