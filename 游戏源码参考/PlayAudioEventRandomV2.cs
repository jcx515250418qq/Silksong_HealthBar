using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public sealed class PlayAudioEventRandomV2 : PlayAudioEventBase
{
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	[ArrayEditor(typeof(VibrationDataAsset), "", 0, 0, 65536)]
	public FsmArray vibrations;

	private int clipHash;

	private AudioClip[] audioClipsArray;

	private int vibrationHash;

	private VibrationDataAsset[] vibrationDataAssetsArray;

	public AudioClip[] AudioClipsArray
	{
		get
		{
			UpdateClipsArray();
			return audioClipsArray;
		}
	}

	public VibrationDataAsset[] VibrationDataAssetsArray
	{
		get
		{
			UpdateVibrationsArray();
			return vibrationDataAssetsArray;
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

	private void UpdateVibrationsArray()
	{
		int contentHash = vibrations.Values.GetContentHash();
		if (vibrationHash != contentHash)
		{
			vibrationHash = contentHash;
			if (vibrations.Values == null)
			{
				vibrationDataAssetsArray = null;
			}
			else
			{
				vibrationDataAssetsArray = vibrations.Values.SafeCastToArray<VibrationDataAsset>();
			}
		}
	}

	public override void Awake()
	{
		UpdateClipsArray();
		UpdateVibrationsArray();
	}

	public override void Reset()
	{
		base.Reset();
		audioClips = null;
		vibrations = null;
	}

	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEventRandom audioEventRandom = default(AudioEventRandom);
		audioEventRandom.Clips = AudioClipsArray;
		audioEventRandom.PitchMin = pitchMin.Value;
		audioEventRandom.PitchMax = pitchMax.Value;
		audioEventRandom.Volume = volume.Value;
		audioEventRandom.vibrations = VibrationDataAssetsArray;
		AudioEventRandom audioEventRandom2 = audioEventRandom;
		if (audioPlayerPrefab.IsNone)
		{
			return audioEventRandom2.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEventRandom2.SpawnAndPlayOneShot(audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}
}
