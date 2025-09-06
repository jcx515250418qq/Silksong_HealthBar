using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PlayAudioEvent : PlayAudioEventBase
{
	[ObjectType(typeof(AudioClip))]
	public FsmObject audioClip;

	public override void Reset()
	{
		base.Reset();
		audioClip = null;
	}

	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEvent audioEvent = default(AudioEvent);
		audioEvent.Clip = audioClip.Value as AudioClip;
		audioEvent.PitchMin = pitchMin.Value;
		audioEvent.PitchMax = pitchMax.Value;
		audioEvent.Volume = volume.Value;
		AudioEvent audioEvent2 = audioEvent;
		if (audioPlayerPrefab.IsNone)
		{
			return audioEvent2.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEvent2.SpawnAndPlayOneShot(audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}
}
