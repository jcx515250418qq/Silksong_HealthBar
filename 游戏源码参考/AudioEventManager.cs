using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioEventManager : MonoBehaviour
{
	private static AudioEventManager _instance;

	private readonly Dictionary<AudioClip, float> clipReleaseTimesLeft = new Dictionary<AudioClip, float>();

	private readonly List<AudioClip> temp = new List<AudioClip>();

	private static AudioEventManager Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = new GameObject("AudioEvent Manager", typeof(AudioEventManager)).GetComponent<AudioEventManager>();
			}
			return _instance;
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void Update()
	{
		try
		{
			temp.AddRange(clipReleaseTimesLeft.Keys);
			foreach (AudioClip item in temp)
			{
				float num = clipReleaseTimesLeft[item];
				num -= Time.unscaledDeltaTime;
				if (num <= 0f)
				{
					clipReleaseTimesLeft.Remove(item);
				}
				else
				{
					clipReleaseTimesLeft[item] = num;
				}
			}
		}
		finally
		{
			temp.Clear();
		}
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		clipReleaseTimesLeft.Clear();
		temp.Clear();
	}

	private static void RecordAudioClip(AudioClip clip)
	{
		Instance.InternalRecordAudioClip(clip);
	}

	private static bool CanPlayAudioClip(AudioClip clip)
	{
		return Instance.InternalCanPlayAudioClip(clip);
	}

	public static bool TryPlayAudioClip(AudioClip clip, AudioSource prefab, Vector3 position)
	{
		if (!CanPlayAudioClip(clip))
		{
			return false;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return false;
		}
		if (prefab.spatialBlend > 0.95f && Vector3.Distance(silentInstance.mainCamera.transform.position, position) > prefab.maxDistance)
		{
			return false;
		}
		RecordAudioClip(clip);
		return true;
	}

	private void InternalRecordAudioClip(AudioClip clip)
	{
		clipReleaseTimesLeft[clip] = Audio.AudioEventFrequencyLimit;
	}

	private bool InternalCanPlayAudioClip(AudioClip clip)
	{
		if (clipReleaseTimesLeft.TryGetValue(clip, out var value))
		{
			return value <= 0f;
		}
		return true;
	}
}
