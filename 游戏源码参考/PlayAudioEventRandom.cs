using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PlayAudioEventRandom : PlayAudioEventBase
{
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	private int clipHash;

	private AudioClip[] audioClipsArray;

	public AudioClip[] AudioClipsArray
	{
		get
		{
			UpdateClipsArray();
			return audioClipsArray;
		}
	}

	private void UpdateClipsArray()
	{
		int contentHash = audioClips.Values.GetContentHash();
		if (clipHash != contentHash)
		{
			clipHash = contentHash;
			if (audioClips.Values == null)
			{
				audioClipsArray = null;
			}
			else
			{
				audioClipsArray = audioClips.Values.SafeCastToArray<AudioClip>();
			}
		}
	}

	public override void Awake()
	{
		UpdateClipsArray();
	}

	public override void Reset()
	{
		base.Reset();
		audioClips = null;
	}

	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEventRandom audioEventRandom = default(AudioEventRandom);
		audioEventRandom.Clips = AudioClipsArray;
		audioEventRandom.PitchMin = pitchMin.Value;
		audioEventRandom.PitchMax = pitchMax.Value;
		audioEventRandom.Volume = volume.Value;
		AudioEventRandom audioEventRandom2 = audioEventRandom;
		if (audioPlayerPrefab.IsNone)
		{
			return audioEventRandom2.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEventRandom2.SpawnAndPlayOneShot(audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}
}
