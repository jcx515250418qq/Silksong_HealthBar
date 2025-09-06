using System;
using UnityEngine;

public sealed class AudioSourcePriority : MonoBehaviour
{
	[Serializable]
	public enum SourceType
	{
		Music = 0,
		Atmos = 1,
		Hero = 2,
		BackgroundLoop = 3,
		SpawnedActor = 4
	}

	private const int DEFAULT_PRIORITY = 128;

	private const int MIN_PRIORITY = 0;

	private const int MAX_PRIORITY = 256;

	private static readonly int[] PRIORITY_TABLE;

	public static readonly int SPAWNED_ACTOR_PRIORITY;

	[Header("Audio Source Priority Settings")]
	[SerializeField]
	private SourceType sourceType;

	[SerializeField]
	private int offset;

	[SerializeField]
	private AudioSource audioSource;

	static AudioSourcePriority()
	{
		PRIORITY_TABLE = new int[5] { 0, 10, 100, 128, 118 };
		SPAWNED_ACTOR_PRIORITY = GetPriority(SourceType.SpawnedActor);
	}

	private void Awake()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				Debug.LogError("AudioSourcePriorityAssigner requires an AudioSource component on " + base.gameObject.name + ".", this);
				base.enabled = false;
				return;
			}
		}
		UpdatePriority();
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		if (audioSource != null)
		{
			UpdatePriority();
		}
	}

	private void UpdatePriority()
	{
		audioSource.priority = InternalGetPriority(sourceType);
	}

	private int InternalGetPriority(SourceType sourceType)
	{
		int num = 128;
		if (sourceType >= SourceType.Music && (int)sourceType < PRIORITY_TABLE.Length)
		{
			num = PRIORITY_TABLE[(int)sourceType];
		}
		else
		{
			Debug.LogWarning($"Priority level {sourceType} is out of range for the priority table on {base.gameObject.name}.", this);
		}
		return Mathf.Clamp(num + offset, 0, 256);
	}

	public static int GetPriority(SourceType sourceType)
	{
		int value = 128;
		if (sourceType >= SourceType.Music && (int)sourceType < PRIORITY_TABLE.Length)
		{
			value = PRIORITY_TABLE[(int)sourceType];
		}
		else
		{
			Debug.LogWarning($"Priority level {sourceType} is out of range for the priority table.");
		}
		return Mathf.Clamp(value, 0, 256);
	}
}
