using System.Collections.Generic;
using UnityEngine;

public static class PersistentAudioManager
{
	public sealed class PersistentAudioData
	{
		public List<PersistentAudioInstance> instances = new List<PersistentAudioInstance>();

		private Vector3 lastAddedPosition;

		public void AddInstance(PersistentAudioInstance instance)
		{
			instances.RemoveAll((PersistentAudioInstance o) => o == null);
			lastAddedPosition = instance.transform.position;
			if (instances.Count > 0)
			{
				foreach (PersistentAudioInstance instance2 in instances)
				{
					if (instance.AlsoSetOtherChangeRate)
					{
						instance2.SetChangeRate(instance.FadeInRate);
					}
					instance2.MarkForDestroy();
				}
				bool adoptPreviousPlayingState = instance.AdoptPreviousPlayingState;
				foreach (PersistentAudioInstance instance3 in instances)
				{
					if (!instance3.AudioSource.isPlaying)
					{
						continue;
					}
					AudioSource audioSource = instance3.AudioSource;
					instance.QueueFadeUp();
					if (adoptPreviousPlayingState)
					{
						instance.AudioSource.clip = audioSource.clip;
						if (audioSource.isPlaying)
						{
							instance.AudioSource.Play();
						}
					}
					if (adoptPreviousPlayingState || (instance.AudioSource != null && audioSource.clip == instance.AudioSource.clip))
					{
						instance.AudioSource.timeSamples = audioSource.timeSamples;
						instance.SetSyncTarget(audioSource);
					}
					break;
				}
			}
			instances.Add(instance);
		}

		public void UpdatePositions()
		{
			foreach (PersistentAudioInstance instance in instances)
			{
				if (instance.AdoptNewInstancePosition && instance.IsFromPreviousScene)
				{
					instance.transform.position = lastAddedPosition;
				}
			}
		}

		public void AttachToObject(Transform transform)
		{
			foreach (PersistentAudioInstance instance in instances)
			{
				if (instance.KeepRelativePositionInNewScene && instance.IsFromPreviousScene)
				{
					instance.transform.SetParent(transform);
				}
			}
		}
	}

	private static Dictionary<string, PersistentAudioData> dictionary = new Dictionary<string, PersistentAudioData>();

	private static List<PersistentAudioInstance> activeInstances = new List<PersistentAudioInstance>();

	private static int queuedEntryState;

	public static bool Paused { get; set; }

	public static void AddInstance(PersistentAudioInstance instance)
	{
		if (!(instance == null) && !string.IsNullOrEmpty(instance.Key))
		{
			if (!dictionary.TryGetValue(instance.Key, out var value))
			{
				PersistentAudioData persistentAudioData2 = (dictionary[instance.Key] = new PersistentAudioData());
				value = persistentAudioData2;
			}
			value.AddInstance(instance);
			activeInstances.Add(instance);
		}
	}

	public static void RemoveInstance(PersistentAudioInstance instance)
	{
		if (!(instance == null) && dictionary.TryGetValue(instance.Key, out var value))
		{
			value.instances.Remove(instance);
			activeInstances.Remove(instance);
			if (value.instances.Count == 0)
			{
				dictionary.Remove(instance.Key);
			}
		}
	}

	public static void Update()
	{
		if (queuedEntryState >= 2 && queuedEntryState++ > 4)
		{
			OnEnteredNextScene();
			queuedEntryState = 0;
		}
		if (!Paused)
		{
			for (int num = activeInstances.Count - 1; num >= 0; num--)
			{
				activeInstances[num].UpdateVolume();
			}
		}
	}

	public static void MarkOldInstancesForRemoval()
	{
		foreach (PersistentAudioInstance activeInstance in activeInstances)
		{
			activeInstance.MarkForDestroy();
		}
	}

	public static void AttachToObject(Transform parent)
	{
		foreach (PersistentAudioData value in dictionary.Values)
		{
			value.AttachToObject(parent);
		}
	}

	public static void UpdateInstancePositions()
	{
		foreach (PersistentAudioData value in dictionary.Values)
		{
			value.UpdatePositions();
		}
	}

	public static void MarkAsPreviousScene()
	{
		foreach (PersistentAudioInstance activeInstance in activeInstances)
		{
			activeInstance.IsFromPreviousScene = true;
		}
	}

	public static void QueueSceneEntry()
	{
		if (queuedEntryState <= 0)
		{
			queuedEntryState = 1;
		}
	}

	public static void OnLevelLoaded()
	{
		if (queuedEntryState == 1)
		{
			queuedEntryState = 2;
		}
	}

	public static void OnLeaveScene()
	{
		Paused = true;
		MarkAsPreviousScene();
		MarkOldInstancesForRemoval();
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			AttachToObject(instance.cameraCtrl.transform);
		}
	}

	public static void OnEnteredNextScene()
	{
		Paused = false;
		AttachToObject(null);
		UpdateInstancePositions();
		queuedEntryState = 0;
	}

	public static void ClearAndReset()
	{
		Paused = false;
		for (int num = activeInstances.Count - 1; num >= 0; num--)
		{
			Object.Destroy(activeInstances[num].gameObject);
		}
		activeInstances.Clear();
		dictionary.Clear();
	}
}
