using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;

public class CollectionGramaphone : MonoBehaviour
{
	[Serializable]
	public struct PlayingInfo
	{
		public string RelicName;

		public float StartTime;
	}

	[SerializeField]
	private AudioSource source;

	[SerializeField]
	private float sourceStartDelay;

	[SerializeField]
	private AudioSource eventSource;

	[SerializeField]
	private AudioEvent sourcePreStartAudio;

	[SerializeField]
	private AudioEvent sourceEndAudio;

	[SerializeField]
	private OverrideNeedolinLoop needolinLoop;

	[SerializeField]
	private NeedolinSyncedAudioPlayer needolinSyncedAudioPlayer;

	[SerializeField]
	private AudioClip defaultNeedolinClip;

	[SerializeField]
	private AudioMixerSnapshot snapshot;

	[SerializeField]
	private float alreadyFadingTransitionTime = 1f;

	[SerializeField]
	private PlayMakerFSM behaviourFsm;

	[SerializeField]
	private GameObject activeWhilePlaying;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(PlayingInfo), false)]
	private string playingPdField;

	private Coroutine reportDelayRoutine;

	private AudioMixerGroup defaultMixer;

	private CollectableRelic loadedRelic;

	public CollectableRelic PlayingRelic { get; private set; }

	private void Awake()
	{
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: false);
		}
		defaultMixer = source.outputAudioMixerGroup;
		TryLoadRelic();
	}

	private IEnumerator Start()
	{
		if (!TryLoadRelic())
		{
			yield break;
		}
		HeroController hc = HeroController.instance;
		if (hc != null && !hc.isHeroInPosition)
		{
			while (!hc.isHeroInPosition)
			{
				yield return null;
			}
		}
		while (loadedRelic.IsLoading)
		{
			yield return null;
		}
		Play(loadedRelic, alreadyPlaying: true, null);
	}

	private void OnDestroy()
	{
		if ((bool)loadedRelic)
		{
			loadedRelic.FreeClips();
			loadedRelic = null;
		}
	}

	private bool TryLoadRelic()
	{
		if ((bool)loadedRelic)
		{
			return true;
		}
		if (string.IsNullOrEmpty(playingPdField))
		{
			return false;
		}
		PlayingInfo variable = PlayerData.instance.GetVariable<PlayingInfo>(playingPdField);
		if (string.IsNullOrEmpty(variable.RelicName))
		{
			return false;
		}
		loadedRelic = CollectableRelicManager.GetRelic(variable.RelicName);
		if (!loadedRelic)
		{
			return false;
		}
		loadedRelic.LoadClips();
		return true;
	}

	public void Play(CollectableRelic playingRelicAudio, bool alreadyPlaying, RelicBoardOwner owner)
	{
		if ((bool)snapshot)
		{
			snapshot.TransitionTo(alreadyPlaying ? alreadyFadingTransitionTime : 0f);
		}
		if ((bool)behaviourFsm)
		{
			behaviourFsm.SendEventSafe(alreadyPlaying ? "ALREADY PLAYING" : "START PLAYING");
		}
		if (!alreadyPlaying)
		{
			sourcePreStartAudio.PlayOnSource(eventSource);
		}
		if ((bool)behaviourFsm)
		{
			AudioMixerGroup mixerOverride = playingRelicAudio.MixerOverride;
			source.outputAudioMixerGroup = (mixerOverride ? mixerOverride : defaultMixer);
		}
		if (source.clip != playingRelicAudio.GramaphoneClip)
		{
			source.clip = playingRelicAudio.GramaphoneClip;
			source.PlayDelayed(sourceStartDelay);
		}
		else if (!source.isPlaying)
		{
			source.PlayDelayed(sourceStartDelay);
		}
		PlayingRelic = playingRelicAudio;
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: true);
		}
		if (playingRelicAudio.PlaySyncedAudioSource && (bool)needolinSyncedAudioPlayer)
		{
			needolinSyncedAudioPlayer.Play();
		}
		if ((bool)needolinLoop)
		{
			AudioClip needolinClip = playingRelicAudio.NeedolinClip;
			needolinLoop.DoSync = needolinClip != null;
			if (!needolinLoop.DoSync)
			{
				needolinClip = defaultNeedolinClip;
			}
			needolinLoop.NeedolinClip = needolinClip;
		}
		if (reportDelayRoutine != null)
		{
			StopCoroutine(reportDelayRoutine);
		}
		if (playingRelicAudio.WillSendPlayEvent)
		{
			reportDelayRoutine = StartCoroutine(ReportPlayedDelayed(playingRelicAudio, owner));
		}
		if (!string.IsNullOrEmpty(playingPdField))
		{
			PlayerData.instance.SetVariable(playingPdField, new PlayingInfo
			{
				RelicName = playingRelicAudio.name,
				StartTime = GameManager.instance.PlayTime
			});
		}
	}

	private IEnumerator ReportPlayedDelayed(CollectableRelic playingRelic, RelicBoardOwner owner)
	{
		if ((bool)owner)
		{
			owner.RelicBoard.IsActionsBlocked = true;
		}
		yield return new WaitForSeconds(0.5f);
		playingRelic.OnPlayedEvent();
	}

	public void Stop()
	{
		if (reportDelayRoutine != null)
		{
			StopCoroutine(reportDelayRoutine);
			reportDelayRoutine = null;
		}
		if ((bool)behaviourFsm)
		{
			behaviourFsm.SendEventSafe("STOP PLAYING");
		}
		source.Stop();
		if (PlayingRelic != null && PlayingRelic.PlaySyncedAudioSource && (bool)needolinSyncedAudioPlayer)
		{
			needolinSyncedAudioPlayer.Stop();
		}
		PlayingRelic = null;
		sourceEndAudio.PlayOnSource(eventSource);
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: false);
		}
		if (!string.IsNullOrEmpty(playingPdField))
		{
			PlayerData.instance.SetVariable(playingPdField, default(PlayingInfo));
		}
	}
}
