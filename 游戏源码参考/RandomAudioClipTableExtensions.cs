using System;
using GlobalSettings;
using UnityEngine;

public static class RandomAudioClipTableExtensions
{
	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, bool forcePlay = false)
	{
		if (!(table == null))
		{
			table.PlayOneShotUnsafe(audioSource, 0f, forcePlay);
		}
	}

	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, float pitchOffset, bool forcePlay = false)
	{
		if (!(table == null))
		{
			table.PlayOneShotUnsafe(audioSource, pitchOffset, forcePlay);
		}
	}

	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, float pitchOffset, float volumeScale, bool forcePlay = false)
	{
		if (!(table == null))
		{
			table.PlayOneShotUnsafe(audioSource, pitchOffset, volumeScale, forcePlay);
		}
	}

	public static AudioSource SpawnAndPlayOneShot(this RandomAudioClipTable table, Vector3 position, bool forcePlay = false)
	{
		return table.SpawnAndPlayOneShot(Audio.DefaultAudioSourcePrefab, position, forcePlay);
	}

	public static AudioSource SpawnAndPlayOneShot(this RandomAudioClipTable table, AudioSource prefab, Vector3 position, bool forcePlay = false, float volume = 1f, Action onRecycled = null)
	{
		if (table == null)
		{
			return null;
		}
		if (prefab == null)
		{
			prefab = Audio.DefaultAudioSourcePrefab;
		}
		if (prefab == null)
		{
			return null;
		}
		AudioClip audioClip = table.SelectClip(forcePlay);
		if (audioClip == null)
		{
			return null;
		}
		if (!AudioEventManager.TryPlayAudioClip(audioClip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn();
		audioSource.transform.position = position;
		audioSource.pitch = table.SelectPitch();
		audioSource.volume = table.SelectVolume() * volume;
		audioSource.PlayOneShot(audioClip);
		table.ReportPlayed(audioClip, audioSource);
		if ((bool)audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	public static void SpawnAndPlayOneShot2D(this RandomAudioClipTable table, Vector3 position, bool forcePlay = false)
	{
		table.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, position, forcePlay);
	}
}
