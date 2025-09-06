using System.Collections.Generic;
using UnityEngine;

public sealed class AudioPreloader : MonoBehaviour
{
	private static HashSet<AudioClip> preloadedClips = new HashSet<AudioClip>();

	private static AudioPreloader _instance;

	private static bool initialised;

	public static AudioPreloader Instance
	{
		get
		{
			if (!initialised)
			{
				_instance = new GameObject("AudioPreloader").AddComponent<AudioPreloader>();
				if (!initialised)
				{
					initialised = true;
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			if (!initialised)
			{
				initialised = true;
			}
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			initialised = false;
			_instance = null;
			preloadedClips.Clear();
		}
	}

	private static AudioSource GetAudioSource()
	{
		AudioSource audioSource = Instance.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		return audioSource;
	}

	public static void PreloadClip(AudioClip clip)
	{
		if (preloadedClips.Add(clip))
		{
			GetAudioSource().clip = clip;
			clip.LoadAudioData();
		}
	}
}
