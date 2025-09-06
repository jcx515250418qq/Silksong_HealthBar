using System.Collections.Generic;
using UnityEngine;

public static class AudioGroupManager
{
	public sealed class AudioGroup
	{
		public int maxActive;

		public int maxPerFrame;

		public int startedThisFrame;

		public List<AudioSource> activeSources = new List<AudioSource>();

		public List<QueuedClip> clipQueue = new List<QueuedClip>();

		public AudioGroup(int maxActive, int maxPerFrame)
		{
			this.maxActive = ((maxActive <= 0) ? 5 : maxActive);
			this.maxPerFrame = ((maxPerFrame <= 0) ? 2 : maxPerFrame);
		}

		public void AddSource(AudioSource source)
		{
			startedThisFrame++;
			activeSources.Add(source);
		}
	}

	public sealed class QueuedClip
	{
		public AudioSource source;

		public AudioClip clip;

		public float volume;
	}

	private static readonly Dictionary<string, AudioGroup> audioGroups = new Dictionary<string, AudioGroup>();

	public const int MAX_PER_FRAME = 2;

	public const int MAX_ACTIVE = 5;

	public static void ClearAudioGroups()
	{
		audioGroups.Clear();
	}

	public static AudioGroup EnsureGroupExists(string groupId, int maxActive = 5, int maxPerFrame = 2)
	{
		if (!audioGroups.TryGetValue(groupId, out var value))
		{
			value = new AudioGroup(maxActive, maxPerFrame);
			audioGroups.Add(groupId, value);
		}
		return value;
	}

	public static bool CanPlay(string groupId, out AudioGroup group)
	{
		if (!audioGroups.TryGetValue(groupId, out group))
		{
			group = EnsureGroupExists(groupId);
		}
		if (group.startedThisFrame >= group.maxPerFrame || group.activeSources.Count >= group.maxActive)
		{
			return false;
		}
		return true;
	}

	public static bool CanPlayLoop(string groupId, out AudioGroup group)
	{
		if (!audioGroups.TryGetValue(groupId, out group))
		{
			group = EnsureGroupExists(groupId);
		}
		if (group.activeSources.Count >= group.maxActive)
		{
			return false;
		}
		return true;
	}

	public static bool PlayClip(string groupId, AudioSource source, AudioClip clip)
	{
		if (!CanPlay(groupId, out var group))
		{
			return false;
		}
		source.clip = clip;
		source.Play();
		group.startedThisFrame++;
		group.activeSources.Add(source);
		return true;
	}

	public static bool PlayOneShotClip(string groupId, AudioSource source, AudioClip clip)
	{
		if (!CanPlay(groupId, out var group))
		{
			return false;
		}
		source.PlayOneShot(clip);
		group.startedThisFrame++;
		group.activeSources.Add(source);
		return true;
	}

	public static void PlayLoopClip(string groupId, AudioSource source, AudioClip clip, float volume)
	{
		if (CanPlayLoop(groupId, out var group))
		{
			if ((bool)clip)
			{
				source.clip = clip;
			}
			source.volume = volume;
			source.Play();
			group.activeSources.Add(source);
		}
		else
		{
			group.clipQueue.Add(new QueuedClip
			{
				source = source,
				clip = clip,
				volume = volume
			});
		}
	}

	public static void RemoveLoopClip(string groupId, AudioSource source)
	{
		if (audioGroups.TryGetValue(groupId, out var value))
		{
			value.activeSources.RemoveAll((AudioSource s) => s == source);
			value.clipQueue.RemoveAll((QueuedClip q) => q.source == source);
		}
	}

	public static void UpdateAudioGroups()
	{
		foreach (AudioGroup value in audioGroups.Values)
		{
			value.startedThisFrame = 0;
			value.activeSources.RemoveAll((AudioSource source) => source == null || !source.isPlaying);
			int num = 0;
			while (num < value.clipQueue.Count && value.activeSources.Count < value.maxActive)
			{
				QueuedClip queuedClip = value.clipQueue[num];
				value.clipQueue.RemoveAt(num);
				if (queuedClip.clip != null)
				{
					queuedClip.source.clip = queuedClip.clip;
				}
				if (!(queuedClip.source.clip == null))
				{
					queuedClip.source.Play();
					queuedClip.source.volume = queuedClip.volume;
					value.activeSources.Add(queuedClip.source);
				}
			}
		}
	}
}
