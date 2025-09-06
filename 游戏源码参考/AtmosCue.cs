using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAtmosCue", menuName = "Hollow Knight/Atmos Cue", order = 1000)]
public class AtmosCue : ScriptableObject
{
	[Serializable]
	public class AtmosChannelInfo
	{
		[SerializeField]
		private AudioClip clip;

		public AudioClip Clip => clip;

		public bool IsEnabled => clip != null;
	}

	[Serializable]
	private class Alternative
	{
		public AtmosCue Cue;

		public PlayerDataTest Condition;
	}

	[SerializeField]
	private AudioMixerSnapshot snapshot;

	[SerializeField]
	[ArrayForEnum(typeof(AtmosChannels))]
	private AtmosChannelInfo[] channelInfos;

	[SerializeField]
	private Alternative[] alternatives;

	public AudioMixerSnapshot Snapshot => snapshot;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref channelInfos, typeof(AtmosChannels));
	}

	public AtmosChannelInfo GetChannelInfo(AtmosChannels channel)
	{
		if (channel < AtmosChannels.Layer1 || (int)channel >= channelInfos.Length)
		{
			return null;
		}
		return channelInfos[(int)channel];
	}

	public AtmosCue ResolveAlternatives()
	{
		if (alternatives != null)
		{
			Alternative[] array = alternatives;
			foreach (Alternative alternative in array)
			{
				if (alternative.Condition.IsFulfilled)
				{
					AtmosCue cue = alternative.Cue;
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
}
