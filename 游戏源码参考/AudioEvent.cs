using System;
using GlobalSettings;
using UnityEngine;

[Serializable]
public struct AudioEvent
{
	public AudioClip Clip;

	public float PitchMin;

	public float PitchMax;

	public float Volume;

	public VibrationDataAsset vibrationDataAsset;

	public static readonly AudioEvent Default = new AudioEvent
	{
		PitchMin = 1f,
		PitchMax = 1f,
		Volume = 1f
	};

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

	public AudioSource SpawnAndPlayOneShot(Vector3 position, Action onRecycled = null)
	{
		return SpawnAndPlayOneShot(null, position, onRecycled);
	}

	public AudioSource SpawnAndPlayOneShot(Vector3 position, bool vibrate, Action onRecycled = null)
	{
		return SpawnAndPlayOneShot(null, position, vibrate, onRecycled);
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action onRecycled = null)
	{
		if (Volume < Mathf.Epsilon)
		{
			return null;
		}
		if (Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = Volume;
		audioSource.pitch = SelectPitch();
		audioSource.PlayOneShot(Clip);
		PlayVibration(audioSource);
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, bool vibrate, Action onRecycled = null)
	{
		if (Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = Volume;
		audioSource.pitch = SelectPitch();
		audioSource.PlayOneShot(Clip);
		if (vibrate)
		{
			PlayVibration(audioSource);
		}
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, float delay, Action onRecycled = null)
	{
		if (Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = Volume;
		audioSource.pitch = SelectPitch();
		if (delay > 0f)
		{
			audioSource.clip = Clip;
			audioSource.Play((ulong)(delay * (float)Clip.frequency));
			onRecycled = (Action)Delegate.Combine(onRecycled, (Action)delegate
			{
				audioSource.clip = null;
			});
		}
		else
		{
			audioSource.PlayOneShot(Clip);
		}
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	public AudioSource SpawnAndPlayLooped(AudioSource prefab, Vector3 position, float delay, Action onRecycled = null)
	{
		if (Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = Volume;
		audioSource.pitch = SelectPitch();
		audioSource.loop = true;
		audioSource.clip = Clip;
		if (delay > 0f)
		{
			audioSource.Play((ulong)(delay * (float)Clip.frequency));
		}
		else
		{
			audioSource.Play();
		}
		onRecycled = (Action)Delegate.Combine(onRecycled, (Action)delegate
		{
			audioSource.clip = null;
			audioSource.loop = false;
		});
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	public void PlayVibration(AudioSource audioSource)
	{
		if (!ObjectPool.IsCreatingPool && (bool)vibrationDataAsset)
		{
			VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset, null);
		}
	}

	public void PlayOnSource(AudioSource source)
	{
		if ((bool)source)
		{
			source.pitch = SelectPitch();
			source.clip = Clip;
			source.Play();
		}
	}
}
