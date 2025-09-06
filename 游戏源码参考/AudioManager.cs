using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AudioManager : ManagerSingleton<AudioManager>
{
	[SerializeField]
	[ArrayForEnum(typeof(AtmosChannels))]
	private AudioSource[] atmosSources;

	[SerializeField]
	private AudioMixer atmosMixer;

	[Space]
	[SerializeField]
	private AudioLoopMaster audioLoopMaster;

	[SerializeField]
	[ArrayForEnum(typeof(MusicChannels))]
	private AudioSource[] musicSources;

	[SerializeField]
	private AudioMixer musicMixer;

	public AudioSource FanfareEnemyBattleClear;

	private AudioMixerSnapshot atmosSnapshotOverride;

	private AudioMixerSnapshot currentMusicSnapshot;

	private readonly List<AtmosSnapshotMarker> atmosMarkers = new List<AtmosSnapshotMarker>();

	private readonly AudioMixerSnapshot[] atmosSnapshots = new AudioMixerSnapshot[2];

	private readonly float[] atmosWeights = new float[2];

	private readonly List<MusicSnapshotMarker> musicMarkers = new List<MusicSnapshotMarker>();

	private readonly AudioMixerSnapshot[] musicSnapshots = new AudioMixerSnapshot[2];

	private readonly float[] musicWeights = new float[2];

	private double markerBlendUpdateBlockedAtmos;

	private double markerBlendUpdateBlockedMusic;

	private Transform camera;

	private Vector2 previousCameraPosition;

	private bool forceUpdate;

	private bool forceMusicUpdate;

	private bool waitingForCameraPosition;

	private GameManager gm;

	private HeroController hc;

	private CameraController cameraCtrl;

	private Coroutine applyMusicCueRoutine;

	private Coroutine applyAtmosCueRoutine;

	private Coroutine applyMusicSnapshotRoutine;

	private AtmosCue currentAtmosCue;

	private AsyncOperationHandle<SceneInstance> currentAtmosCueSceneHandle;

	private float previousAtmosMarkerTime;

	private MusicCue currentMusicCue;

	private AsyncOperationHandle<SceneInstance> currentMusicCueSceneHandle;

	private float previousMusicMarkerTime;

	private bool waitingForAtmosBlock;

	private bool waitingForMusicBlock;

	private bool isWaitingForAtmosCue;

	private bool hasQueuedAtmosOverride;

	private AudioMixerSnapshot queuedAtmosOverrideSnapshot;

	private float queuedAtmosTransitionTime;

	private static bool waitingToUnpause;

	private static bool levelIsReady;

	private static bool customSceneManagerIsReady;

	private static float waitForCustomSceneManager;

	private static int actorCallbackWaitFrames = -1;

	private const int ACTOR_CALLBACK_WAIT = 5;

	private const float CUSTOM_SCENE_MANAGER_WAIT_TIME = 3f;

	private static Action actorSnapshotCallback;

	public AudioSource[] MusicSources => musicSources;

	public static bool BlockAudioChange { get; set; }

	public AtmosCue CurrentAtmosCue
	{
		get
		{
			return currentAtmosCue;
		}
		private set
		{
			SetSoundCue(ref currentAtmosCue, value, ref currentAtmosCueSceneHandle);
		}
	}

	public MusicCue CurrentMusicCue
	{
		get
		{
			return currentMusicCue;
		}
		private set
		{
			SetSoundCue(ref currentMusicCue, value, ref currentMusicCueSceneHandle);
		}
	}

	public static event Action OnAppliedActorSnapshot;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref atmosSources, typeof(AtmosChannels));
		ArrayForEnumAttribute.EnsureArraySize(ref musicSources, typeof(MusicChannels));
	}

	protected override void Awake()
	{
		OnValidate();
		base.Awake();
	}

	private void OnEnable()
	{
		cameraCtrl = GameCameras.instance.cameraController;
		cameraCtrl.PositionedAtHero += OnCameraPositionedAtHero;
	}

	private void OnDisable()
	{
		if ((bool)cameraCtrl)
		{
			cameraCtrl.PositionedAtHero -= OnCameraPositionedAtHero;
			cameraCtrl = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (CurrentAtmosCue != null)
		{
			CurrentAtmosCue = null;
		}
		if (CurrentMusicCue != null)
		{
			CurrentMusicCue = null;
		}
		if (ManagerSingleton<AudioManager>.UnsafeInstance == this)
		{
			PersistentAudioManager.ClearAndReset();
		}
		HeroController.OnHeroInstanceSet -= OnSetHeroInstance;
	}

	private void OnCameraPositionedAtHero()
	{
		waitingForCameraPosition = false;
	}

	private void Start()
	{
		gm = GameManager.instance;
		hc = HeroController.SilentInstance;
		HeroController.OnHeroInstanceSet += OnSetHeroInstance;
	}

	private void Update()
	{
		PersistentAudioManager.Update();
		if ((bool)hc && !hc.isHeroInPosition)
		{
			waitingForCameraPosition = true;
			return;
		}
		UpdateSnapshotCallback();
		if (!camera)
		{
			return;
		}
		if (waitingForAtmosBlock && Time.timeAsDouble >= markerBlendUpdateBlockedAtmos)
		{
			waitingForAtmosBlock = false;
			forceUpdate = true;
		}
		if (waitingForMusicBlock && Time.timeAsDouble >= markerBlendUpdateBlockedMusic)
		{
			waitingForMusicBlock = false;
			forceUpdate = true;
		}
		if ((!ShouldUpdate() && !(Vector2.SqrMagnitude((Vector2)camera.position - previousCameraPosition) > 1f)) || gm.IsInSceneTransition || waitingForCameraPosition)
		{
			return;
		}
		forceUpdate = false;
		previousCameraPosition = camera.position;
		if (atmosMarkers.Count > 0)
		{
			if (Time.timeAsDouble >= markerBlendUpdateBlockedAtmos)
			{
				TransitionToCurrentAtmos(0f);
			}
			else
			{
				waitingForAtmosBlock = true;
			}
		}
		if (musicMarkers.Count > 0 || forceMusicUpdate)
		{
			if (Time.timeAsDouble >= markerBlendUpdateBlockedMusic)
			{
				TransitionToCurrentMusic(0f);
			}
			else
			{
				waitingForMusicBlock = true;
			}
		}
	}

	private bool ShouldUpdate()
	{
		if (!forceUpdate)
		{
			return forceMusicUpdate;
		}
		return true;
	}

	private void UpdateSnapshotCallback()
	{
		if (actorCallbackWaitFrames <= 0)
		{
			return;
		}
		if (waitForCustomSceneManager > 0f)
		{
			waitForCustomSceneManager -= Time.deltaTime;
		}
		else if ((bool)gm && gm.HasFinishedEnteringScene)
		{
			actorCallbackWaitFrames--;
			if (actorCallbackWaitFrames <= 0)
			{
				RunActorSnapshotCallback();
			}
		}
	}

	private void OnSetHeroInstance(HeroController controller)
	{
		hc = controller;
	}

	public void ApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
	{
		ApplyAtmosCue(atmosCue, transitionTime, markWaitForAtmos: false);
	}

	public void ApplyAtmosCue(AtmosCue atmosCue, float transitionTime, bool markWaitForAtmos)
	{
		atmosCue = (atmosCue ? atmosCue.ResolveAlternatives() : null);
		if (!(atmosCue == null) && !(currentAtmosCue == atmosCue))
		{
			if (markWaitForAtmos)
			{
				isWaitingForAtmosCue = true;
			}
			BeginApplyAtmosCue(atmosCue, transitionTime);
		}
	}

	public void StopAndClearMusic()
	{
		CurrentMusicCue = null;
		for (int i = 0; i < musicSources.Length; i++)
		{
			AudioSource obj = musicSources[i];
			obj.Stop();
			obj.clip = null;
		}
	}

	public void StopAndClearAtmos()
	{
		CurrentAtmosCue = null;
		for (int i = 0; i < atmosSources.Length; i++)
		{
			AudioSource obj = atmosSources[i];
			obj.Stop();
			obj.clip = null;
		}
	}

	private void OnFinishedWaitingForAtmosCue()
	{
		if (isWaitingForAtmosCue)
		{
			isWaitingForAtmosCue = false;
			if (hasQueuedAtmosOverride)
			{
				hasQueuedAtmosOverride = false;
				TransitionToAtmosOverride(queuedAtmosOverrideSnapshot, queuedAtmosTransitionTime);
				queuedAtmosOverrideSnapshot = null;
			}
		}
	}

	public void ClearAtmosOverrides()
	{
		atmosSnapshotOverride = null;
	}

	private void BeginApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
	{
		CurrentAtmosCue = atmosCue;
		if (atmosCue == null)
		{
			OnFinishedWaitingForAtmosCue();
			return;
		}
		if ((bool)atmosCue.Snapshot)
		{
			atmosSnapshotOverride = null;
		}
		for (int i = 0; i < atmosSources.Length; i++)
		{
			AtmosCue.AtmosChannelInfo channelInfo = atmosCue.GetChannelInfo((AtmosChannels)i);
			AudioSource audioSource = atmosSources[i];
			if (channelInfo == null || !channelInfo.IsEnabled)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
					audioSource.clip = null;
				}
				continue;
			}
			if (audioSource.clip != channelInfo.Clip)
			{
				audioSource.Stop();
			}
			audioSource.clip = channelInfo.Clip;
			if (!audioSource.isPlaying)
			{
				if ((bool)channelInfo.Clip)
				{
					float time = UnityEngine.Random.Range(0f, channelInfo.Clip.length);
					audioSource.time = time;
				}
				audioSource.Play();
			}
		}
		TransitionToCurrentAtmos(transitionTime);
		markerBlendUpdateBlockedAtmos = Time.timeAsDouble + (double)transitionTime;
		OnFinishedWaitingForAtmosCue();
	}

	private void TransitionToCurrentAtmos(float transitionTime)
	{
		TransitionToCurrentSnapshots(atmosMixer, transitionTime, atmosMarkers, currentAtmosCue ? currentAtmosCue.Snapshot : null, atmosSnapshotOverride, atmosSnapshots, atmosWeights, ref previousAtmosMarkerTime);
	}

	private void TransitionToCurrentMusic(float transitionTime)
	{
		forceMusicUpdate = false;
		TransitionToCurrentSnapshots(musicMixer, transitionTime, musicMarkers, currentMusicSnapshot, null, musicSnapshots, musicWeights, ref previousMusicMarkerTime);
	}

	private void TransitionToCurrentSnapshots(AudioMixer mixer, float transitionTime, IEnumerable<SnapshotMarker> markers, AudioMixerSnapshot cueSnapshot, AudioMixerSnapshot snapshotOverride, AudioMixerSnapshot[] snapshots, float[] weights, ref float previousMarkerTime)
	{
		float num = 0f;
		SnapshotMarker snapshotMarker = null;
		foreach (SnapshotMarker marker in markers)
		{
			float blendAmount = marker.GetBlendAmount();
			if (!(blendAmount <= num))
			{
				num = blendAmount;
				snapshotMarker = marker;
				previousMarkerTime = marker.TransitionTime;
			}
		}
		bool flag = cueSnapshot;
		bool flag2 = flag || (bool)snapshotOverride;
		bool flag3 = snapshotMarker != null && (bool)snapshotMarker.Snapshot;
		if (transitionTime <= Mathf.Epsilon)
		{
			transitionTime = previousMarkerTime;
		}
		if (num >= 1f)
		{
			flag2 = false;
		}
		if (flag2 && flag3)
		{
			snapshots[0] = (flag ? cueSnapshot : snapshotOverride);
			weights[0] = 1f - num;
			snapshots[1] = snapshotMarker.Snapshot;
			weights[1] = num;
			mixer.TransitionToSnapshots(snapshots, weights, transitionTime);
		}
		else if (flag2)
		{
			(flag ? cueSnapshot : snapshotOverride).TransitionTo(transitionTime);
		}
		else if (flag3)
		{
			snapshotMarker.Snapshot.TransitionTo(transitionTime);
		}
	}

	public void ApplyMusicCue(MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
	{
		if (BlockAudioChange)
		{
			return;
		}
		musicCue = (musicCue ? musicCue.ResolveAlternatives() : null);
		if (musicCue == null)
		{
			return;
		}
		if (CurrentMusicCue == musicCue)
		{
			CurrentMusicCue = musicCue;
			return;
		}
		if (applyMusicCueRoutine != null)
		{
			StopCoroutine(applyMusicCueRoutine);
			applyMusicCueRoutine = null;
		}
		applyMusicCueRoutine = StartCoroutine(BeginApplyMusicCue(musicCue, delayTime));
	}

	private IEnumerator BeginApplyMusicCue(MusicCue musicCue, float delayTime)
	{
		CurrentMusicCue = musicCue;
		yield return new WaitForSecondsRealtime(delayTime);
		AudioSource[] array = musicSources;
		foreach (AudioSource obj in array)
		{
			obj.Stop();
			obj.clip = null;
		}
		double time = AudioSettings.dspTime + 0.1;
		for (int j = 0; j < musicSources.Length; j++)
		{
			AudioSource audioSource = musicSources[j];
			MusicCue.MusicChannelInfo channelInfo = musicCue.GetChannelInfo((MusicChannels)j);
			if (channelInfo != null && channelInfo.IsEnabled)
			{
				audioSource.clip = channelInfo.Clip;
				audioSource.time = 0f;
				audioSource.volume = 1f;
				audioSource.PlayScheduled(time);
			}
			UpdateMusicSync((MusicChannels)j, channelInfo?.IsSyncRequired ?? false);
		}
		yield return new WaitForSeconds(0.1f);
		audioLoopMaster.AllowSync();
	}

	private void UpdateMusicSync(MusicChannels musicChannel, bool isSyncRequired)
	{
		switch (musicChannel)
		{
		case MusicChannels.MainAlt:
			audioLoopMaster.SetSyncMainAlt(isSyncRequired);
			break;
		case MusicChannels.Action:
			audioLoopMaster.SetSyncAction(isSyncRequired);
			break;
		case MusicChannels.Sub:
			audioLoopMaster.SetSyncSub(isSyncRequired);
			break;
		case MusicChannels.Tension:
			audioLoopMaster.SetSyncTension(isSyncRequired);
			break;
		case MusicChannels.Extra:
			audioLoopMaster.SetSyncExtra(isSyncRequired);
			break;
		default:
			throw new ArgumentOutOfRangeException("musicChannel", musicChannel, null);
		case MusicChannels.Main:
			break;
		}
	}

	public void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime)
	{
		ApplyMusicSnapshot(snapshot, delayTime, transitionTime, blockMusicMarker: true);
	}

	public void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime, bool blockMusicMarker)
	{
		if (applyMusicSnapshotRoutine != null)
		{
			StopCoroutine(applyMusicSnapshotRoutine);
			applyMusicSnapshotRoutine = null;
		}
		if (snapshot != null)
		{
			applyMusicSnapshotRoutine = StartCoroutine(BeginApplyMusicSnapshot(snapshot, delayTime, transitionTime, blockMusicMarker));
		}
	}

	private void SetSoundCue<T>(ref T cueRef, T newCue, ref AsyncOperationHandle<SceneInstance> sceneHandle) where T : ScriptableObject
	{
		if (sceneHandle.IsValid())
		{
			Addressables.ResourceManager.Release(sceneHandle);
			sceneHandle = default(AsyncOperationHandle<SceneInstance>);
		}
		cueRef = newCue;
		if (newCue != null && gm != null)
		{
			SceneLoad lastSceneLoad = gm.LastSceneLoad;
			if (lastSceneLoad != null && lastSceneLoad.OperationHandle.IsValid())
			{
				sceneHandle = gm.LastSceneLoad.OperationHandle;
				Addressables.ResourceManager.Acquire(sceneHandle);
			}
		}
	}

	private static IEnumerator BeginApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime, bool blockMusicMarker)
	{
		if (delayTime > Mathf.Epsilon)
		{
			yield return new WaitForSecondsRealtime(delayTime);
		}
		if (snapshot != null)
		{
			snapshot.TransitionTo(transitionTime);
		}
		AudioManager instance = ManagerSingleton<AudioManager>.Instance;
		instance.currentMusicSnapshot = snapshot;
		instance.TransitionToCurrentMusic(transitionTime);
		if (blockMusicMarker)
		{
			instance.markerBlendUpdateBlockedMusic = Time.timeAsDouble + (double)transitionTime;
		}
		else
		{
			instance.markerBlendUpdateBlockedMusic = 0.0;
		}
	}

	public static void AddAtmosMarker(AtmosSnapshotMarker marker)
	{
		if ((bool)ManagerSingleton<AudioManager>.Instance)
		{
			ManagerSingleton<AudioManager>.Instance.atmosMarkers.AddIfNotPresent(marker);
			if (!ManagerSingleton<AudioManager>.Instance.camera && (bool)GameCameras.instance)
			{
				ManagerSingleton<AudioManager>.Instance.camera = GameCameras.instance.mainCamera.transform;
				ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
			}
		}
	}

	public static void RemoveAtmosMarker(AtmosSnapshotMarker marker)
	{
		if ((bool)ManagerSingleton<AudioManager>.Instance && ManagerSingleton<AudioManager>.Instance.atmosMarkers.Remove(marker))
		{
			ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
		}
	}

	public static void TransitionToAtmosOverride(AudioMixerSnapshot snapshot, float transitionTime)
	{
		AudioManager instance = ManagerSingleton<AudioManager>.Instance;
		if ((bool)instance)
		{
			if (instance.isWaitingForAtmosCue)
			{
				instance.hasQueuedAtmosOverride = true;
				instance.queuedAtmosOverrideSnapshot = snapshot;
				instance.queuedAtmosTransitionTime = transitionTime;
			}
			else
			{
				instance.atmosSnapshotOverride = snapshot;
				instance.TransitionToCurrentAtmos(transitionTime);
				instance.markerBlendUpdateBlockedAtmos = Time.timeAsDouble + (double)transitionTime;
			}
		}
	}

	public static void AddMusicMarker(MusicSnapshotMarker marker)
	{
		if ((bool)ManagerSingleton<AudioManager>.Instance)
		{
			ManagerSingleton<AudioManager>.Instance.musicMarkers.AddIfNotPresent(marker);
			if (!ManagerSingleton<AudioManager>.Instance.camera && (bool)GameCameras.instance)
			{
				ManagerSingleton<AudioManager>.Instance.camera = GameCameras.instance.mainCamera.transform;
				ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
			}
		}
	}

	public static void ForceMarkerUpdate()
	{
		ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
	}

	public static void RemoveMusicMarker(MusicSnapshotMarker marker)
	{
		if ((bool)ManagerSingleton<AudioManager>.Instance && ManagerSingleton<AudioManager>.Instance.musicMarkers.Remove(marker))
		{
			ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
			ManagerSingleton<AudioManager>.Instance.forceMusicUpdate = true;
		}
	}

	public static void PauseActorSnapshot()
	{
		waitingToUnpause = true;
		levelIsReady = false;
		customSceneManagerIsReady = false;
		actorSnapshotCallback = null;
	}

	public static void UnpauseActorSnapshot(Action callback = null)
	{
		if (!waitingToUnpause)
		{
			callback?.Invoke();
			return;
		}
		levelIsReady = true;
		if (customSceneManagerIsReady)
		{
			if (actorSnapshotCallback != null)
			{
				actorSnapshotCallback();
			}
			else
			{
				callback?.Invoke();
			}
			actorSnapshotCallback = null;
			waitingToUnpause = false;
			actorCallbackWaitFrames = -1;
			OnDidAppliedActorSnapshot();
		}
		else
		{
			actorSnapshotCallback = callback;
			actorCallbackWaitFrames = 5;
		}
	}

	public static void CustomSceneManagerSnapshotReady(Action callback = null)
	{
		if (!waitingToUnpause)
		{
			callback?.Invoke();
			return;
		}
		customSceneManagerIsReady = true;
		if (levelIsReady)
		{
			if (callback != null)
			{
				callback();
			}
			else
			{
				actorSnapshotCallback?.Invoke();
			}
			actorSnapshotCallback = null;
			waitingToUnpause = false;
			actorCallbackWaitFrames = -1;
			OnDidAppliedActorSnapshot();
		}
		else
		{
			actorSnapshotCallback = callback;
			actorCallbackWaitFrames = 5;
		}
	}

	public static void SetIsWaitingForCustomSceneManager()
	{
		waitForCustomSceneManager = 3f;
	}

	public static void CustomSceneManagerReady()
	{
		waitForCustomSceneManager = 0f;
	}

	private static void RunActorSnapshotCallback()
	{
		if (waitingToUnpause)
		{
			waitingToUnpause = false;
			actorSnapshotCallback?.Invoke();
			actorCallbackWaitFrames = -1;
			OnDidAppliedActorSnapshot();
		}
	}

	private static void OnDidAppliedActorSnapshot()
	{
		AudioManager.OnAppliedActorSnapshot?.Invoke();
	}
}
