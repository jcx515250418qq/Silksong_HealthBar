using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicCue", menuName = "Hollow Knight/Music Cue", order = 1000)]
public class MusicCue : ScriptableObject
{
	[Serializable]
	public class MusicChannelInfo
	{
		[SerializeField]
		private AudioClip clip;

		[SerializeField]
		private MusicChannelSync sync;

		public AudioClip Clip => clip;

		public bool IsEnabled => clip != null;

		public bool IsSyncRequired
		{
			get
			{
				if (sync == MusicChannelSync.Implicit)
				{
					return clip != null;
				}
				if (sync == MusicChannelSync.ExplicitOn)
				{
					return true;
				}
				return false;
			}
		}
	}

	[Serializable]
	private class Alternative
	{
		public MusicCue Cue;

		public PlayerDataTest Condition;

		public void Preload(GameObject gameObject)
		{
			if (!(Cue == null))
			{
				Cue.InternalPreload(gameObject, preloadAlts: false);
			}
		}
	}

	[SerializeField]
	private string originalMusicEventName;

	[SerializeField]
	private int originalMusicTrackNumber;

	[SerializeField]
	private AudioMixerSnapshot snapshot;

	[SerializeField]
	[ArrayForEnum(typeof(MusicChannels))]
	private MusicChannelInfo[] channelInfos;

	[SerializeField]
	private Alternative[] alternatives;

	public string OriginalMusicEventName => originalMusicEventName;

	public int OriginalMusicTrackNumber => originalMusicTrackNumber;

	public AudioMixerSnapshot Snapshot => snapshot;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref channelInfos, typeof(MusicChannels));
	}

	public MusicChannelInfo GetChannelInfo(MusicChannels channel)
	{
		if (channel < MusicChannels.Main || (int)channel >= channelInfos.Length)
		{
			return null;
		}
		return channelInfos[(int)channel];
	}

	public MusicCue ResolveAlternatives()
	{
		if (alternatives != null)
		{
			Alternative[] array = alternatives;
			foreach (Alternative alternative in array)
			{
				if (alternative.Condition.IsFulfilled)
				{
					MusicCue cue = alternative.Cue;
					if (!(cue != null))
					{
						return null;
					}
					return cue.ResolveAlternatives();
				}
			}
		}
		return this;
	}

	public void Preload(GameObject gameObject)
	{
		InternalPreload(gameObject, preloadAlts: true);
	}

	private void InternalPreload(GameObject gameObject, bool preloadAlts)
	{
		if (channelInfos != null)
		{
			MusicChannelInfo[] array = channelInfos;
			foreach (MusicChannelInfo musicChannelInfo in array)
			{
				if (musicChannelInfo != null && !(musicChannelInfo.Clip == null))
				{
					PreloadClip(gameObject, musicChannelInfo.Clip);
				}
			}
		}
		if (alternatives != null)
		{
			Alternative[] array2 = alternatives;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Preload(gameObject);
			}
		}
	}

	private void PreloadClip(GameObject gameObject, AudioClip clip)
	{
		AudioPreloader.PreloadClip(clip);
	}
}
