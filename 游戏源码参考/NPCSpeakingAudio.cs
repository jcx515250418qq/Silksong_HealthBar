using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSpeakingAudio : MonoBehaviour
{
	[Serializable]
	private class SpeakerAudio
	{
		public RandomAudioClipTable AudioTable;

		public AudioSource PlayOnSource;

		public string SpeakerEvent;
	}

	public const int RE_SPEAK_AFTER_LINES = 2;

	[SerializeField]
	private NPCControlBase npc;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private RandomAudioClipTable audioTable;

	[SerializeField]
	private List<SpeakerAudio> speakers;

	[Space]
	public UnityEvent OnPlayedVoiceClip;

	public UnityEvent OnEndedVoiceClip;

	private bool? wasPlayer;

	private bool hasSpokenOnce;

	private bool skipNextSpeak;

	private int linesSinceLastSpeak;

	private RandomAudioClipTable lastSpeakerTable;

	private DialogueBox.DialogueLine lastDialogueLine;

	private Dictionary<string, AudioClip> spokenRecord;

	private static AudioSource _currentPlayingSource;

	public RandomAudioClipTable Table
	{
		get
		{
			AudioSource playOnSource;
			return GetTableForSpeaker(null, out playOnSource);
		}
		set
		{
			SetTableForSpeaker(null, value);
		}
	}

	private void Reset()
	{
		npc = GetComponent<NPCControlBase>();
	}

	private void OnValidate()
	{
		if ((bool)audioTable)
		{
			speakers = new List<SpeakerAudio>
			{
				new SpeakerAudio
				{
					SpeakerEvent = null,
					AudioTable = audioTable
				}
			};
			audioTable = null;
		}
	}

	private void Awake()
	{
		if (!npc)
		{
			npc = GetComponent<NPCControlBase>();
		}
		if ((bool)npc)
		{
			npc.StartedDialogue += OnDialogueStarted;
			npc.StartedNewLine += OnNewLineStarted;
			npc.EndingDialogue += OnDialogueEnding;
		}
	}

	private void OnDialogueStarted()
	{
		wasPlayer = null;
		hasSpokenOnce = false;
	}

	private void OnDialogueEnding()
	{
		wasPlayer = null;
	}

	public RandomAudioClipTable GetTableForSpeaker(string speakerEvent, out AudioSource playOnSource)
	{
		if (speakers == null)
		{
			playOnSource = null;
			return null;
		}
		SpeakerAudio speakerAudio = null;
		foreach (SpeakerAudio speaker in speakers)
		{
			if (speakerAudio == null || speaker.SpeakerEvent == speakerEvent || (string.IsNullOrEmpty(speaker.SpeakerEvent) && string.IsNullOrEmpty(speakerEvent)))
			{
				speakerAudio = speaker;
			}
		}
		if (speakerAudio != null)
		{
			playOnSource = speakerAudio.PlayOnSource;
			return speakerAudio.AudioTable;
		}
		playOnSource = null;
		Debug.LogError("No NPC dialogue table found for speaker event: " + speakerEvent, this);
		return null;
	}

	public void SetTableForSpeaker(string speakerEvent, RandomAudioClipTable table)
	{
		if (speakers == null)
		{
			speakers = new List<SpeakerAudio>();
		}
		foreach (SpeakerAudio speaker in speakers)
		{
			if (speaker.SpeakerEvent == speakerEvent || (string.IsNullOrEmpty(speaker.SpeakerEvent) && string.IsNullOrEmpty(speakerEvent)))
			{
				speaker.AudioTable = table;
				return;
			}
		}
		speakers.Add(new SpeakerAudio
		{
			AudioTable = table
		});
	}

	private void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
		if (!line.IsPlayer)
		{
			Speak(line);
		}
		else
		{
			linesSinceLastSpeak = 0;
		}
		wasPlayer = line.IsPlayer;
	}

	private void Speak(DialogueBox.DialogueLine line)
	{
		AudioSource playOnSource;
		RandomAudioClipTable tableForSpeaker = GetTableForSpeaker(line.Event, out playOnSource);
		if (linesSinceLastSpeak < 1 && wasPlayer.HasValue && !wasPlayer.Value && tableForSpeaker == lastSpeakerTable)
		{
			linesSinceLastSpeak++;
			return;
		}
		linesSinceLastSpeak = 0;
		if (skipNextSpeak)
		{
			skipNextSpeak = false;
			return;
		}
		lastSpeakerTable = tableForSpeaker;
		lastDialogueLine = line;
		PlayVoice(tableForSpeaker, base.transform.position, playOnSource, this);
		hasSpokenOnce = true;
	}

	public static void PlayVoice(RandomAudioClipTable audioTable, Vector3 position)
	{
		PlayVoice(audioTable, position, null, null);
	}

	private static void PlayVoice(RandomAudioClipTable audioTable, Vector3 position, AudioSource playOnSource, NPCSpeakingAudio runner)
	{
		if ((bool)_currentPlayingSource)
		{
			_currentPlayingSource.Stop();
			OnRecycle();
		}
		if (!audioTable)
		{
			return;
		}
		if ((bool)runner)
		{
			runner.OnPlayedVoiceClip?.Invoke();
		}
		AudioClip value;
		if ((bool)runner && !string.IsNullOrEmpty(runner.lastDialogueLine.Text))
		{
			NPCSpeakingAudio nPCSpeakingAudio = runner;
			if (nPCSpeakingAudio.spokenRecord == null)
			{
				nPCSpeakingAudio.spokenRecord = new Dictionary<string, AudioClip>(audioTable.ClipCount);
			}
			if (runner.spokenRecord.TryGetValue(runner.lastDialogueLine.Text, out value))
			{
				if (!audioTable.CanPlay(forcePlay: true))
				{
					value = null;
				}
			}
			else
			{
				value = audioTable.SelectClip(forcePlay: true);
				runner.spokenRecord[runner.lastDialogueLine.Text] = value;
			}
		}
		else
		{
			value = audioTable.SelectClip(forcePlay: true);
		}
		if ((bool)playOnSource)
		{
			_currentPlayingSource = playOnSource;
			playOnSource.clip = value;
			playOnSource.pitch = audioTable.SelectPitch();
			playOnSource.volume = audioTable.SelectVolume();
			playOnSource.Play();
		}
		else
		{
			float num = audioTable.SelectPitch();
			AudioEvent audioEvent = default(AudioEvent);
			audioEvent.Clip = value;
			audioEvent.PitchMin = num;
			audioEvent.PitchMax = num;
			audioEvent.Volume = audioTable.SelectVolume();
			AudioEvent audioEvent2 = audioEvent;
			_currentPlayingSource = audioEvent2.SpawnAndPlayOneShot(position, OnRecycle);
			audioTable.ReportPlayed(value, _currentPlayingSource);
		}
		void OnRecycle()
		{
			_currentPlayingSource = null;
			if ((bool)runner)
			{
				runner.OnEndedVoiceClip?.Invoke();
			}
		}
	}

	public void TriggerFirstSpeak()
	{
		if (!hasSpokenOnce)
		{
			Speak(default(DialogueBox.DialogueLine));
		}
	}

	public void SkipNextSpeak()
	{
		skipNextSpeak = true;
	}
}
