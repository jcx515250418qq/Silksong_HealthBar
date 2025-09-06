using System;
using GlobalSettings;
using UnityEngine;

[Serializable]
public struct AudioEventRandom
{
	public AudioClip[] Clips;

	public float PitchMin;

	public float PitchMax;

	public float Volume;

	[Space]
	public VibrationDataAsset[] vibrations;

	public void Reset()
	{
		PitchMin = 0.75f;
		PitchMax = 1.25f;
		Volume = 1f;
	}

	public float SelectPitch()
	{
		if (Mathf.Approximately(PitchMin, PitchMax))
		{
			return PitchMax;
		}
		return UnityEngine.Random.Range(PitchMin, PitchMax);
	}

	public bool HasClips()
	{
		if (Clips != null)
		{
			return Clips.Length != 0;
		}
		return false;
	}

	public AudioClip GetClip()
	{
		if (!HasClips())
		{
			return null;
		}
		return Clips[UnityEngine.Random.Range(0, Clips.Length)];
	}

	public AudioSource SpawnAndPlayOneShot(Vector3 position, Action onRecycled = null)
	{
		return SpawnAndPlayOneShot(null, position, onRecycled);
	}

	public AudioSource SpawnAndPlayOneShot(Vector3 position, bool vibrate, Action onRecycled = null)
	{
		return SpawnAndPlayOneShot(null, position, vibrate, onRecycled);
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, float volume)
	{
		return SpawnAndPlayOneShotInternal(prefab, position, volume);
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action onRecycled = null)
	{
		AudioSource audioSource = SpawnAndPlayOneShotInternal(prefab, position, 1f);
		if ((bool)audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, bool vibrate, Action onRecycled = null)
	{
		AudioSource audioSource = SpawnAndPlayOneShotInternal(prefab, position, 1f, vibrate);
		if ((bool)audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action<AudioSource> onRecycled)
	{
		AudioSource audioSource = SpawnAndPlayOneShotInternal(prefab, position, 1f);
		if ((bool)audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, (Action)delegate
			{
				onRecycled(audioSource);
			});
		}
		return audioSource;
	}

	private AudioSource SpawnAndPlayOneShotInternal(AudioSource prefab, Vector3 position, float volume, bool vibrate = true)
	{
		if (Clips == null || Clips.Length == 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, Clips.Length);
		AudioClip audioClip = Clips[num];
		if (audioClip == null)
		{
			return null;
		}
		if (Volume < Mathf.Epsilon)
		{
			return null;
		}
		if (prefab == null)
		{
			prefab = Audio.DefaultAudioSourcePrefab;
			if (prefab == null)
			{
				return null;
			}
		}
		if (!AudioEventManager.TryPlayAudioClip(audioClip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = Volume * volume;
		audioSource.pitch = SelectPitch();
		audioSource.PlayOneShot(audioClip);
		if (vibrate)
		{
			PlayVibration(num, audioSource);
		}
		return audioSource;
	}

	public void PlayVibrationRandom()
	{
		if (vibrations != null && vibrations.Length != 0)
		{
			PlayVibration(UnityEngine.Random.Range(0, vibrations.Length), null);
		}
	}

	public void PlayVibration(AudioClip clip, AudioSource source)
	{
		if (ObjectPool.IsCreatingPool || Clips.Length == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < Clips.Length; i++)
		{
			if (Clips[i] == clip)
			{
				num = i;
				break;
			}
		}
		if (vibrations != null && vibrations.Length != 0)
		{
			VibrationManager.PlayVibrationClipOneShot(vibrations[num % vibrations.Length], null);
		}
	}

	public void PlayVibration(int index, AudioSource source)
	{
		if (!ObjectPool.IsCreatingPool && vibrations != null && vibrations.Length != 0)
		{
			VibrationManager.PlayVibrationClipOneShot(vibrations[index % vibrations.Length], null);
		}
	}

	public void PlayOnSource(AudioSource source)
	{
		if ((bool)source)
		{
			source.pitch = SelectPitch();
			source.clip = GetClip();
			source.Play();
		}
	}
}
