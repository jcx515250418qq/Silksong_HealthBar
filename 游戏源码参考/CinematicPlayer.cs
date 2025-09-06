using System;
using System.Collections;
using GlobalEnums;
using JetBrains.Annotations;
using TeamCherry.Cinematics;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class CinematicPlayer : MonoBehaviour, GameManager.ISkippable
{
	public enum MovieTrigger
	{
		OnStart = 0,
		ManualTrigger = 1,
		ManualPlayPreloaded = 2
	}

	public enum VideoType
	{
		OpeningCutscene = 0,
		StagTravel = 1,
		InGameVideo = 2,
		OpeningPrologue = 3,
		Ending = 4,
		EndingE = 5
	}

	public enum Blanker
	{
		Default = 0,
		SkipBlankFrame = 1,
		DoNotBlank = 2
	}

	[SerializeField]
	private CinematicVideoReference videoClip;

	[SerializeField]
	private float startAtTime;

	private CinematicVideoPlayer cinematicVideoPlayer;

	[SerializeField]
	private AudioSource additionalAudio;

	[SerializeField]
	private bool additionalAudioContinuesPastVideo;

	[SerializeField]
	private NestedFadeGroupBase selfBlanker;

	[SerializeField]
	private GameObject activeWhilePlaying;

	[SerializeField]
	private bool autoScaleToHUDCamera;

	[Header("Cinematic Settings")]
	[Tooltip("Determines what will trigger the video playing.")]
	public MovieTrigger playTrigger;

	[SerializeField]
	private float startDelay;

	[SerializeField]
	private float endDelay;

	[SerializeField]
	private bool fireQueuedAchievements;

	[SerializeField]
	private float achievementDelay = 1f;

	[Space]
	[Tooltip("Allows the player to skip the video.")]
	public SkipPromptMode skipMode;

	[Tooltip("Prevents the skip action from taking place until the lock is released. Useful for animators delaying skip feature.")]
	public bool startSkipLocked;

	[Tooltip("Video keeps looping until the player is explicitly told to stop.")]
	public bool loopVideo;

	public float startBlankTime;

	public Blanker blankerMode;

	public int prePlayFrames;

	[Space]
	[Tooltip("The name of the scene to load when the video ends. Leaving this blank will load the \"next scene\" as set in PlayerData.")]
	public VideoType videoType;

	public CinematicVideoFaderStyles faderStyle;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInGameVideo", true, true, false)]
	private bool disableGameCam;

	[Space]
	[SerializeField]
	private AudioMixerSnapshot masterOff;

	[SerializeField]
	private float masterOffFadeTime;

	[SerializeField]
	private AudioMixerSnapshot masterResume;

	[SerializeField]
	private float masterResumeFadeTime;

	[Space]
	[SerializeField]
	private float actCardDelay;

	[SerializeField]
	private Animator actCard;

	[Space]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[Space]
	public UnityEvent OnPlay;

	public UnityEvent OnSkip;

	private AudioSource audioSource;

	private MeshRenderer myRenderer;

	private GameManager gm;

	private UIManager ui;

	private PlayerData pd;

	private PlayMakerFSM cameraFsm;

	private bool videoTriggered;

	private bool loadingLevel;

	private bool isWaiting;

	private bool allowPlay;

	private bool hasPlayed;

	private bool isWaitingToPlay;

	private Platform.ResolutionModes previousResMode;

	private Coroutine finishRoutine;

	private bool appliedVsync;

	private VibrationEmission emission;

	private bool videoIsPlaying;

	private bool isSkipped;

	public CinematicVideoReference VideoClip
	{
		get
		{
			return videoClip;
		}
		set
		{
			videoClip = value;
		}
	}

	public float Duration
	{
		get
		{
			if (!videoClip)
			{
				return 0f;
			}
			return videoClip.VideoFileLength;
		}
	}

	[UsedImplicitly]
	private bool IsInGameVideo()
	{
		return videoType == VideoType.InGameVideo;
	}

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		myRenderer = GetComponent<MeshRenderer>();
		if (videoType == VideoType.InGameVideo || Platform.Current.ExtendedVideoBlankFrames > 0)
		{
			myRenderer.enabled = false;
		}
		if ((bool)actCard)
		{
			actCard.gameObject.SetActive(value: false);
		}
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
		if ((bool)gm)
		{
			gm.RegisterSkippable(this);
		}
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
		CameraRenderScaled.RemoveForceFullResolution(this);
		if ((bool)gm)
		{
			gm.DeregisterSkippable(this);
		}
	}

	protected void OnDestroy()
	{
		if (cinematicVideoPlayer != null)
		{
			cinematicVideoPlayer.Dispose();
			cinematicVideoPlayer = null;
		}
		RestoreScreen();
		RestoreVsync();
	}

	private void Start()
	{
		gm = GameManager.instance;
		ui = UIManager.instance;
		pd = PlayerData.instance;
		if (playTrigger != MovieTrigger.ManualTrigger)
		{
			StartCoroutine(StartVideo());
		}
		if ((bool)gm)
		{
			gm.RegisterSkippable(this);
		}
	}

	private void Update()
	{
		if (cinematicVideoPlayer != null)
		{
			cinematicVideoPlayer.Update();
		}
		if (videoIsPlaying)
		{
			TestFinish();
		}
		else if (Time.frameCount % 10 == 0)
		{
			Update10();
		}
	}

	private void Update10()
	{
		TestFinish();
	}

	private bool TestFinish()
	{
		if ((cinematicVideoPlayer == null || (!isWaitingToPlay && !cinematicVideoPlayer.IsLoading && !cinematicVideoPlayer.IsPlaying && !isWaiting)) && !loadingLevel && videoTriggered)
		{
			videoIsPlaying = false;
			if (videoType == VideoType.InGameVideo)
			{
				if (finishRoutine == null)
				{
					finishRoutine = StartCoroutine(FinishInGameVideo());
				}
			}
			else
			{
				FinishVideo();
			}
			return true;
		}
		return false;
	}

	private void OnCameraAspectChanged(float _)
	{
		if (!autoScaleToHUDCamera)
		{
			return;
		}
		bool num = base.gameObject.layer == 5;
		Camera camera = null;
		if (num)
		{
			camera = GameCameras.instance.hudCamera;
		}
		if (camera == null || !camera.isActiveAndEnabled)
		{
			camera = GameCameras.instance.mainCamera;
		}
		if (!camera)
		{
			return;
		}
		Transform transform = base.transform;
		Transform transform2 = camera.transform;
		Vector3 position = transform.position;
		position.x = transform2.position.x;
		position.y = transform2.position.y;
		transform.position = position;
		float num2;
		if (camera.orthographic)
		{
			num2 = camera.orthographicSize * 2f / 10f;
		}
		else
		{
			tk2dCamera component = camera.GetComponent<tk2dCamera>();
			if ((bool)component)
			{
				component.UpdateCameraMatrix();
			}
			float num3 = Mathf.Abs(transform2.position.z - transform.position.z);
			float num4 = MathF.PI / 180f * camera.fieldOfView;
			num2 = 2f * num3 * Mathf.Tan(num4 / 2f) / 10f;
		}
		float x = num2 * 1.7777778f;
		Vector3 localScale = new Vector3(x, 1f, num2);
		transform.localScale = localScale;
	}

	public IEnumerator Skip()
	{
		if (isSkipped || !videoTriggered)
		{
			yield break;
		}
		isSkipped = true;
		selfBlanker.AlphaSelf = 0f;
		selfBlanker.gameObject.SetActive(value: true);
		OnSkip.Invoke();
		for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
		{
			float num = elapsed / 0.3f;
			selfBlanker.AlphaSelf = num;
			if ((bool)audioSource)
			{
				audioSource.volume = Mathf.Clamp01(1f - num);
			}
			yield return null;
		}
		selfBlanker.AlphaSelf = 1f;
		yield return null;
		if (cinematicVideoPlayer != null)
		{
			cinematicVideoPlayer.Stop();
		}
		if ((bool)audioSource)
		{
			audioSource.Stop();
		}
		if ((bool)additionalAudio)
		{
			additionalAudio.Stop();
		}
		emission?.Stop();
		while (!TestFinish())
		{
			yield return null;
		}
	}

	public void TriggerStartVideo()
	{
		isSkipped = false;
		if (playTrigger == MovieTrigger.ManualPlayPreloaded)
		{
			allowPlay = true;
		}
		else
		{
			StartCoroutine(StartVideo());
		}
	}

	public void TriggerStopVideo()
	{
		if (videoType == VideoType.InGameVideo)
		{
			StartCoroutine(Skip());
		}
	}

	public void UnlockSkip()
	{
		gm.inputHandler.SetSkipMode(skipMode);
	}

	private IEnumerator StartVideo()
	{
		isSkipped = false;
		if (finishRoutine != null)
		{
			StopCoroutine(finishRoutine);
			finishRoutine = null;
		}
		if (cinematicVideoPlayer == null)
		{
			cinematicVideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(videoClip, myRenderer, audioSource, faderStyle, GameManager.instance.GetImplicitCinematicVolume()));
			if (cinematicVideoPlayer == null)
			{
				OnVideoBecameNull();
				yield break;
			}
		}
		bool shouldEnable = false;
		bool shouldDisableBlanker = false;
		if ((bool)myRenderer)
		{
			shouldEnable = !myRenderer.enabled;
		}
		VideoPlayer unityVideo = myRenderer.GetComponent<VideoPlayer>();
		int videoBlankFrames;
		bool wasSendingEvent;
		if ((bool)unityVideo)
		{
			videoBlankFrames = (CheatManager.OverrideReadyWaitFrames ? CheatManager.ReadyWaitFrames : Platform.Current.ExtendedVideoBlankFrames);
			videoBlankFrames = Mathf.Max(prePlayFrames, videoBlankFrames);
			wasSendingEvent = unityVideo.sendFrameReadyEvents;
			unityVideo.sendFrameReadyEvents = true;
			unityVideo.frameReady += UnityVideoOnframeReady;
			shouldEnable = false;
		}
		if (CheatManager.OverrideSkipFrameOnDrop)
		{
			cinematicVideoPlayer.SkipFrameOnDrop = CheatManager.SkipVideoFrameOnDrop;
		}
		else
		{
			cinematicVideoPlayer.SkipFrameOnDrop = false;
		}
		cinematicVideoPlayer.IsLooping = loopVideo;
		cinematicVideoPlayer.CurrentTime = startAtTime;
		if (playTrigger == MovieTrigger.ManualPlayPreloaded)
		{
			while (!allowPlay)
			{
				yield return null;
			}
		}
		if (blankerMode == Blanker.DoNotBlank)
		{
			selfBlanker.gameObject.SetActive(value: false);
		}
		else
		{
			selfBlanker.AlphaSelf = 1f;
			selfBlanker.gameObject.SetActive(value: true);
		}
		if (masterOff != null)
		{
			masterOff.TransitionTo(masterOffFadeTime);
		}
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		videoTriggered = true;
		isWaitingToPlay = true;
		OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		if (videoType == VideoType.InGameVideo)
		{
			gm.SetState(GameState.CUTSCENE);
			if (startDelay > 0f)
			{
				isWaiting = true;
				yield return new WaitForSeconds(startDelay);
				isWaiting = false;
			}
			while (cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (cinematicVideoPlayer == null)
			{
				OnVideoBecameNull();
				yield break;
			}
			OnPlay.Invoke();
			cinematicVideoPlayer.Play();
			SetScreenToMatchVideo();
			if (additionalAudio != null)
			{
				additionalAudio.Play();
			}
			if (vibrationDataAsset != null)
			{
				emission?.Stop();
				emission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset, null, isLooping: false, "", isRealtime: true);
			}
			for (int i = 0; i < prePlayFrames; i++)
			{
				yield return null;
			}
			if (shouldEnable)
			{
				myRenderer.enabled = true;
			}
		}
		else if (videoType == VideoType.StagTravel)
		{
			GameCameras.instance.OnCinematicBegin();
			if (startDelay > 0f)
			{
				isWaiting = true;
				yield return new WaitForSeconds(startDelay);
				isWaiting = false;
			}
			while (cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (cinematicVideoPlayer == null)
			{
				OnVideoBecameNull();
				yield break;
			}
			cinematicVideoPlayer.Play();
			if (shouldEnable)
			{
				myRenderer.enabled = true;
			}
			StartCoroutine(WaitForStagFadeOut());
			pd.disablePause = true;
		}
		else
		{
			GameCameras.instance.OnCinematicBegin();
			if (startDelay > 0f)
			{
				isWaiting = true;
				yield return new WaitForSeconds(startDelay);
				isWaiting = false;
			}
			while (cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (cinematicVideoPlayer == null)
			{
				OnVideoBecameNull();
				yield break;
			}
			cinematicVideoPlayer.Play();
			SetScreenToMatchVideo();
			if (additionalAudio != null)
			{
				additionalAudio.Play();
			}
			for (int i = 0; i < prePlayFrames; i++)
			{
				yield return null;
			}
			if (shouldEnable)
			{
				myRenderer.enabled = true;
			}
		}
		isWaitingToPlay = false;
		while (!cinematicVideoPlayer.IsPlaying)
		{
			yield return null;
		}
		if (blankerMode == Blanker.Default)
		{
			yield return null;
		}
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: true);
		}
		EventRegister.SendEvent(EventRegisterEvents.CinematicStart);
		float blankTime = startBlankTime;
		if (blankTime > 0f)
		{
			while (blankTime > 0f)
			{
				yield return null;
				blankTime -= Time.deltaTime;
			}
		}
		videoIsPlaying = true;
		shouldDisableBlanker = true;
		if (myRenderer.enabled)
		{
			selfBlanker.gameObject.SetActive(value: false);
		}
		if (disableGameCam && shouldEnable)
		{
			GameCameras.instance.SetMainCameraActive(value: false);
		}
		if (!startSkipLocked)
		{
			yield return new WaitForSeconds(1f);
			gm.inputHandler.SetSkipMode(skipMode);
		}
		void UnityVideoOnframeReady(VideoPlayer source, long frameIdx)
		{
			if (frameIdx >= videoBlankFrames)
			{
				if ((bool)myRenderer)
				{
					myRenderer.enabled = true;
				}
				if (shouldDisableBlanker)
				{
					selfBlanker.gameObject.SetActive(value: false);
				}
				if (disableGameCam)
				{
					GameCameras.instance.SetMainCameraActive(value: false);
				}
				unityVideo.frameReady -= UnityVideoOnframeReady;
				unityVideo.sendFrameReadyEvents = wasSendingEvent;
			}
		}
	}

	private void OnVideoBecameNull()
	{
		isWaitingToPlay = false;
		TestFinish();
	}

	private void SetScreenToMatchVideo()
	{
		Platform.ResolutionModes resolutionMode = Platform.Current.ResolutionMode;
		ApplyVsync();
		if (resolutionMode != 0)
		{
			previousResMode = resolutionMode;
			Platform.Current.ResolutionMode = Platform.ResolutionModes.Native;
			Screen.SetResolution(videoClip.VideoFileWidth, videoClip.VideoFileHeight, fullscreen: true);
		}
	}

	private void RestoreScreen()
	{
		RestoreVsync();
		if (previousResMode != 0)
		{
			Platform.Current.ResolutionMode = previousResMode;
			Platform.Current.RefreshGraphicsTier();
		}
	}

	private void ApplyVsync()
	{
		if (!appliedVsync)
		{
			appliedVsync = true;
			int targetFrameRate = Mathf.RoundToInt(videoClip.VideoFileFrameRate);
			Platform.Current.SetTargetFrameRate(targetFrameRate);
		}
	}

	private void RestoreVsync()
	{
		if (appliedVsync)
		{
			appliedVsync = false;
			Platform.Current.RestoreFrameRate();
		}
	}

	private void FinishVideo()
	{
		selfBlanker.AlphaSelf = 0f;
		GameCameras.instance.OnCinematicEnd();
		videoTriggered = false;
		videoIsPlaying = false;
		if (fireQueuedAchievements && (bool)gm)
		{
			this.ExecuteDelayed(achievementDelay, gm.AwardQueuedAchievements);
			endDelay = Mathf.Max(achievementDelay + 4f, endDelay);
		}
		if (endDelay > 0f)
		{
			this.ExecuteDelayed(endDelay, End);
		}
		else
		{
			End();
		}
		void End()
		{
			switch (videoType)
			{
			case VideoType.OpeningCutscene:
				GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
				ui.SetState(UIState.INACTIVE);
				loadingLevel = true;
				StartCoroutine(gm.LoadFirstScene());
				break;
			case VideoType.OpeningPrologue:
				GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
				ui.SetState(UIState.INACTIVE);
				loadingLevel = true;
				gm.LoadOpeningCinematic();
				break;
			case VideoType.StagTravel:
				ui.SetState(UIState.INACTIVE);
				loadingLevel = true;
				gm.ChangeToScene(pd.nextScene, "door_fastTravelExit", 0f);
				break;
			case VideoType.Ending:
				GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
				ui.SetState(UIState.INACTIVE);
				loadingLevel = true;
				gm.LoadScene(gm.playerData.bossRushMode ? "GG_End_Sequence" : "End_Credits");
				break;
			case VideoType.EndingE:
				GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
				ui.SetState(UIState.INACTIVE);
				loadingLevel = true;
				gm.LoadScene(pd.MushroomQuestCompleted ? "Cinematic_MrMushroom" : "End_Credits_Scroll");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case VideoType.InGameVideo:
				break;
			}
		}
	}

	private IEnumerator FinishInGameVideo()
	{
		if ((bool)activeWhilePlaying)
		{
			activeWhilePlaying.SetActive(value: false);
		}
		selfBlanker.AlphaSelf = 0f;
		myRenderer.enabled = false;
		gm.ui.HideCutscenePrompt(isInstant: false);
		RestoreScreen();
		if ((bool)actCard)
		{
			gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
			if (actCardDelay > 0f)
			{
				yield return new WaitForSeconds(actCardDelay);
			}
			selfBlanker.gameObject.SetActive(value: false);
			CameraRenderScaled.AddForceFullResolution(this);
			actCard.gameObject.SetActive(value: true);
			yield return null;
			yield return new WaitForSeconds(actCard.GetCurrentAnimatorStateInfo(0).length);
			CameraRenderScaled.RemoveForceFullResolution(this);
		}
		if (disableGameCam)
		{
			GameCameras.instance.SetMainCameraActive(value: true);
		}
		selfBlanker.gameObject.SetActive(value: false);
		if (masterResume != null)
		{
			masterResume.TransitionTo(masterResumeFadeTime);
		}
		if (!additionalAudioContinuesPastVideo && additionalAudio != null)
		{
			additionalAudio.Stop();
		}
		if (cinematicVideoPlayer != null)
		{
			cinematicVideoPlayer.Stop();
			cinematicVideoPlayer.Dispose();
			cinematicVideoPlayer = null;
		}
		emission?.Stop();
		emission = null;
		videoTriggered = false;
		gm.SetState(GameState.PLAYING);
		gm.inputHandler.StartAcceptingInput();
		if (isSkipped)
		{
			EventRegister.SendEvent(EventRegisterEvents.CinematicSkipped);
		}
		EventRegister.SendEvent(EventRegisterEvents.CinematicEnd);
		GameCameras.instance.OnCinematicEnd();
	}

	private IEnumerator WaitForStagFadeOut()
	{
		yield return new WaitForSeconds(2.6f);
		GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
	}

	public void SetAdditionalAudio(AudioSource newAudioSource, bool continuesPastVideo)
	{
		additionalAudio = newAudioSource;
		additionalAudioContinuesPastVideo = continuesPastVideo;
	}
}
