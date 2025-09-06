using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomAudioClipTable", menuName = "Hornet/Random Audio Clip Table", order = 1000)]
public class RandomAudioClipTable : ScriptableObject
{
	[Serializable]
	private class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
	{
		public AudioClip Clip;

		public override AudioClip Item => Clip;
	}

	private enum AudioTypes
	{
		Normal = 0,
		Player = 1
	}

	private struct LastPlayedInfo
	{
		public double EndTime;

		public int Priority;

		public AudioSource PlayedOnSource;
	}

	[SerializeField]
	private float pitchMin = 1f;

	[SerializeField]
	private float pitchMax = 1f;

	[SerializeField]
	private float volumeMin = 1f;

	[SerializeField]
	private float volumeMax = 1f;

	[SerializeField]
	private ProbabilityAudioClip[] clips;

	[SerializeField]
	[Range(0f, 1f)]
	private float totalProbability = 1f;

	[SerializeField]
	private bool disallowRepeatingPrevious;

	[SerializeField]
	private float unselectedMultiplier = 1f;

	[SerializeField]
	private float cooldownDuration;

	[SerializeField]
	private RandomAudioClipTable[] addCooldownTo;

	[NonSerialized]
	private AudioClip previousClip;

	[NonSerialized]
	private float[] currentProbabilities;

	[NonSerialized]
	private double nextPlayTime;

	[Space]
	[SerializeField]
	private AudioTypes type;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingPriority", true, true, false)]
	private int priority;

	[SerializeField]
	private AudioPitchEffects pitchEffects;

	private bool[] conditions;

	private static readonly Dictionary<AudioTypes, LastPlayedInfo> _lastPlayedInfos = new Dictionary<AudioTypes, LastPlayedInfo>();

	public int ClipCount
	{
		get
		{
			ProbabilityAudioClip[] array = clips;
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}
	}

	private bool IsUsingPriority()
	{
		return type != AudioTypes.Normal;
	}

	public AudioClip SelectClip(bool forcePlay = false)
	{
		if (!CanPlay(forcePlay))
		{
			return null;
		}
		if (!forcePlay && UnityEngine.Random.Range(0f, 1f) > totalProbability)
		{
			return null;
		}
		return previousClip = SelectRandomClip();
	}

	public AudioClip SelectClipIgnoreProbability(bool forcePlay = false)
	{
		if (!CanPlay(forcePlay))
		{
			return null;
		}
		return previousClip = SelectRandomClip();
	}

	private AudioClip SelectRandomClip()
	{
		if (ClipCount <= 0)
		{
			return null;
		}
		if (disallowRepeatingPrevious)
		{
			if (conditions == null || conditions.Length != clips.Length)
			{
				conditions = new bool[clips.Length];
			}
			for (int i = 0; i < conditions.Length; i++)
			{
				conditions[i] = clips[i].Clip != previousClip;
			}
		}
		else
		{
			conditions = null;
		}
		int chosenIndex;
		if (!(Math.Abs(unselectedMultiplier - 1f) < 0.001f))
		{
			return Probability.GetRandomItemByProbabilityFair<ProbabilityAudioClip, AudioClip>(clips, out chosenIndex, ref currentProbabilities, unselectedMultiplier, conditions);
		}
		return Probability.GetRandomItemByProbability<ProbabilityAudioClip, AudioClip>(clips, out chosenIndex, null, conditions);
	}

	public bool CanPlay(bool forcePlay)
	{
		if (!forcePlay)
		{
			if (Time.unscaledTimeAsDouble < nextPlayTime)
			{
				return false;
			}
			if (_lastPlayedInfos.TryGetValue(type, out var value) && priority < value.Priority && Time.unscaledTimeAsDouble < value.EndTime)
			{
				return false;
			}
		}
		if (type == AudioTypes.Player && !InteractManager.BlockingInteractable)
		{
			return GameManager.instance.gameSettings.playerVoiceEnabled;
		}
		return true;
	}

	public void ReportPlayed(AudioClip clip, AudioSource spawnedAudioSource)
	{
		SetCooldown(cooldownDuration);
		if (addCooldownTo != null)
		{
			RandomAudioClipTable[] array = addCooldownTo;
			foreach (RandomAudioClipTable randomAudioClipTable in array)
			{
				if (!(randomAudioClipTable == null))
				{
					randomAudioClipTable.SetCooldown(cooldownDuration);
				}
			}
		}
		if (type == AudioTypes.Normal || !clip || !spawnedAudioSource)
		{
			return;
		}
		if (_lastPlayedInfos.TryGetValue(type, out var value))
		{
			value.PlayedOnSource.Stop();
			value.PlayedOnSource.Recycle();
		}
		_lastPlayedInfos[type] = new LastPlayedInfo
		{
			Priority = priority,
			EndTime = Time.unscaledTimeAsDouble + (double)clip.length,
			PlayedOnSource = spawnedAudioSource
		};
		RecycleResetHandler.Add(spawnedAudioSource.gameObject, (Action)delegate
		{
			if (_lastPlayedInfos.TryGetValue(type, out var value2) && value2.PlayedOnSource == spawnedAudioSource)
			{
				_lastPlayedInfos.Remove(type);
			}
		});
	}

	private void SetCooldown(float cooldown)
	{
		nextPlayTime = Time.unscaledTimeAsDouble + (double)cooldown;
	}

	public float SelectPitch()
	{
		float num = 0f;
		if (pitchEffects.HasFlag(AudioPitchEffects.Quickening))
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance && instance.IsUsingQuickening)
			{
				num = 0.05f;
			}
		}
		if (Mathf.Approximately(pitchMin, pitchMax))
		{
			return pitchMax + num;
		}
		return UnityEngine.Random.Range(pitchMin, pitchMax) + num;
	}

	public float SelectVolume()
	{
		if (Mathf.Approximately(volumeMin, volumeMax))
		{
			return volumeMax;
		}
		return UnityEngine.Random.Range(volumeMin, volumeMax);
	}

	public void PlayOneShotUnsafe(AudioSource audioSource, float pitchOffset, bool forcePlay = false)
	{
		if (!(audioSource == null))
		{
			AudioClip audioClip = SelectClip(forcePlay);
			if (!(audioClip == null))
			{
				audioSource.pitch = SelectPitch() + pitchOffset;
				audioSource.volume = SelectVolume();
				audioSource.PlayOneShot(audioClip);
				ReportPlayed(audioClip, null);
			}
		}
	}

	public void PlayOneShotUnsafe(AudioSource audioSource, float pitchOffset, float volumeScale, bool forcePlay = false)
	{
		if (!(audioSource == null))
		{
			AudioClip audioClip = SelectClip(forcePlay);
			if (!(audioClip == null))
			{
				audioSource.pitch = SelectPitch() + pitchOffset;
				float num = SelectVolume();
				audioSource.PlayOneShot(audioClip, num * volumeScale);
				ReportPlayed(audioClip, null);
			}
		}
	}
}
