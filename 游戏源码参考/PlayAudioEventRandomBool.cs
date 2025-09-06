using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlayAudioEventRandomBool : PlayAudioEventBase
{
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	public FsmBool doPlay;

	public override void Reset()
	{
		base.Reset();
		audioClips = null;
		doPlay = true;
	}

	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		if (doPlay.Value)
		{
			AudioEventRandom audioEventRandom = default(AudioEventRandom);
			audioEventRandom.Clips = audioClips.Values.Cast<AudioClip>().ToArray();
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
		return null;
	}
}
