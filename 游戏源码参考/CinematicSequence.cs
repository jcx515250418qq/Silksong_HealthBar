using System;
using TeamCherry.Cinematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Video;

public class CinematicSequence : SkippableSequence
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private bool waitForAudioFinish;

	[SerializeField]
	private AudioMixerSnapshot atmosSnapshot;

	[SerializeField]
	private float atmosSnapshotTransitionDuration;

	[SerializeField]
	private CinematicVideoReference videoReference;

	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private bool isLooping;

	[SerializeField]
	private MeshRenderer targetRenderer;

	[SerializeField]
	private MeshRenderer blankerRenderer;

	private float fadeRate;

	private float lastAlpha = -1f;

	[SerializeField]
	private bool autoScaleToHUDCamera = true;

	[Space]
	[SerializeField]
	private AudioSource extraAudio;

	[SerializeField]
	private float extraAudioEarlyPadding;

	[Space]
	public UnityEvent OnVideoLoaded;

	private VideoPlayer unityVideoPlayer;

	private bool isSkipQueued;

	private bool isSkipped;

	private int startFramesWaited;

	private int revealFramesWaited;

	private int framesSinceBegan;

	private float fadeByController;

	private Platform.ResolutionModes previousResMode;

	private VibrationEmission vibrationEmission;

	private bool hasChangedResolution;

	private bool wasPlaying;

	private bool isStarted;

	private float videoStartDelayLeft;

	public CinematicVideoPlayer VideoPlayer { get; private set; }

	public override bool IsSkipped => isSkipped;

	public bool HasVideoReference => videoReference;

	public float VideoLength
	{
		get
		{
			if (!videoReference)
			{
				return 0f;
			}
			return videoReference.VideoFileLength;
		}
	}

	public bool DidPlay { get; private set; }

	public bool IsLooping
	{
		get
		{
			return VideoPlayer?.IsLooping ?? isLooping;
		}
		set
		{
			if (VideoPlayer != null)
			{
				VideoPlayer.IsLooping = value;
			}
			isLooping = value;
		}
	}

	public CinematicVideoReference VideoReference
	{
		get
		{
			return videoReference;
		}
		set
		{
			videoReference = value;
		}
	}

	public VibrationDataAsset VibrationDataAsset
	{
		get
		{
			return vibrationDataAsset;
		}
		set
		{
			vibrationDataAsset = value;
		}
	}

	public override bool IsPlaying
	{
		get
		{
			if (waitForAudioFinish && (bool)audioSource && audioSource.isPlaying)
			{
				return true;
			}
			bool flag = framesSinceBegan < 10 || !DidPlay;
			if (!isSkipped)
			{
				if (!flag)
				{
					if (VideoPlayer != null)
					{
						return VideoPlayer.IsPlaying;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public override float FadeByController
	{
		get
		{
			return fadeByController;
		}
		set
		{
			fadeByController = value;
			UpdateBlanker(1f - fadeByController);
		}
	}

	public event Action OnPlaybackEnded;

	protected void Awake()
	{
		if (!audioSource)
		{
			audioSource = GetComponent<AudioSource>();
		}
		targetRenderer.enabled = DidPlay;
		fadeByController = 1f;
	}

	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
		End();
	}

	protected void Update()
	{
		if (videoStartDelayLeft > 0f)
		{
			videoStartDelayLeft -= Time.deltaTime;
			if (videoStartDelayLeft > 0f)
			{
				return;
			}
			videoStartDelayLeft = 0f;
		}
		if (VideoPlayer == null)
		{
			if (wasPlaying)
			{
				OnEndPlayback();
			}
			return;
		}
		framesSinceBegan++;
		VideoPlayer.Update();
		if (!VideoPlayer.IsLoading && !DidPlay)
		{
			DidPlay = true;
			if (atmosSnapshot != null)
			{
				atmosSnapshot.TransitionTo(atmosSnapshotTransitionDuration);
			}
			if (CheatManager.OverrideSkipFrameOnDrop)
			{
				VideoPlayer.SkipFrameOnDrop = CheatManager.SkipVideoFrameOnDrop;
			}
			else
			{
				VideoPlayer.SkipFrameOnDrop = false;
			}
			OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
			previousResMode = Platform.Current.ResolutionMode;
			if (!hasChangedResolution)
			{
				hasChangedResolution = true;
				int targetFrameRate = Mathf.RoundToInt(VideoReference.VideoFileFrameRate);
				Platform.Current.SetTargetFrameRate(targetFrameRate);
				if (previousResMode != 0)
				{
					Platform.Current.ResolutionMode = Platform.ResolutionModes.Native;
					Screen.SetResolution(videoReference.VideoFileWidth, videoReference.VideoFileHeight, fullscreen: true);
				}
			}
			VideoPlayer.Play();
			wasPlaying = true;
			UpdateBlanker(1f - fadeByController);
			OnVideoLoaded?.Invoke();
			vibrationEmission?.Stop();
			if ((bool)vibrationDataAsset)
			{
				vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset, null, isLooping: false, "", isRealtime: true);
			}
		}
		if (!VideoPlayer.IsPlaying && !VideoPlayer.IsLoading && framesSinceBegan >= 10)
		{
			targetRenderer.enabled = false;
			End();
		}
		else if (isSkipQueued)
		{
			if (VideoPlayer != null)
			{
				VideoPlayer.Stop();
			}
			if ((bool)audioSource)
			{
				audioSource.Stop();
			}
			vibrationEmission?.Stop();
			vibrationEmission = null;
			isSkipped = true;
			OnEndPlayback();
			isSkipQueued = false;
		}
	}

	private void OnCameraAspectChanged(float _)
	{
		if (!autoScaleToHUDCamera || !targetRenderer)
		{
			return;
		}
		bool num = targetRenderer.gameObject.layer == 5;
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
		Transform transform = targetRenderer.transform;
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
		Vector3 self = new Vector3(x, 1f, num2);
		transform.localScale = self.MultiplyElements(transform.localScale.GetSign());
		if ((bool)blankerRenderer)
		{
			Transform obj = blankerRenderer.transform;
			obj.localPosition = transform.localPosition;
			obj.localRotation = transform.localRotation;
			obj.localScale = transform.localScale;
		}
	}

	private void End()
	{
		if (VideoPlayer != null)
		{
			VideoPlayer.Dispose();
			VideoPlayer = null;
		}
		vibrationEmission?.Stop();
		vibrationEmission = null;
		OnEndPlayback();
	}

	private void RestoreResolution()
	{
		Platform.Current.RestoreFrameRate();
		if (hasChangedResolution)
		{
			hasChangedResolution = false;
			if (previousResMode != 0)
			{
				Platform.Current.ResolutionMode = previousResMode;
				Platform.Current.RefreshGraphicsTier();
			}
		}
	}

	public override void Begin()
	{
		int extendedWaitFrames;
		bool wasSendingEvent;
		if (!isStarted && (VideoPlayer == null || !VideoPlayer.IsPlaying))
		{
			OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
			if (VideoPlayer != null)
			{
				VideoPlayer.Dispose();
				VideoPlayer = null;
			}
			vibrationEmission?.Stop();
			vibrationEmission = null;
			VideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(videoReference, targetRenderer, audioSource, CinematicVideoFaderStyles.Black, GameManager.instance.GetImplicitCinematicVolume()));
			VideoPlayer.IsLooping = isLooping;
			unityVideoPlayer = targetRenderer.GetComponent<VideoPlayer>();
			if ((bool)unityVideoPlayer)
			{
				extendedWaitFrames = (CheatManager.OverrideReadyWaitFrames ? CheatManager.ReadyWaitFrames : Platform.Current.ExtendedVideoBlankFrames);
				wasSendingEvent = unityVideoPlayer.sendFrameReadyEvents;
				unityVideoPlayer.sendFrameReadyEvents = true;
				unityVideoPlayer.frameReady += UnityVideoOnframeReady;
			}
			UpdateBlanker(1f - fadeByController);
			targetRenderer.enabled = DidPlay;
			isStarted = true;
			isSkipped = false;
			isSkipQueued = false;
			framesSinceBegan = 0;
			if ((bool)extraAudio)
			{
				extraAudio.Play();
			}
			videoStartDelayLeft = extraAudioEarlyPadding;
		}
		void UnityVideoOnframeReady(VideoPlayer source, long frameIdx)
		{
			if (frameIdx >= extendedWaitFrames)
			{
				if ((bool)targetRenderer)
				{
					targetRenderer.enabled = true;
				}
				unityVideoPlayer.frameReady -= UnityVideoOnframeReady;
				unityVideoPlayer.sendFrameReadyEvents = wasSendingEvent;
			}
		}
	}

	private void UpdateBlanker(float alpha)
	{
		if (VideoPlayer != null)
		{
			VideoPlayer.Volume = 1f - Mathf.Clamp01(alpha);
		}
		if (alpha > Mathf.Epsilon)
		{
			if (!blankerRenderer.enabled)
			{
				blankerRenderer.enabled = true;
			}
			if (lastAlpha != alpha)
			{
				lastAlpha = alpha;
				blankerRenderer.material.color = new Color(0f, 0f, 0f, alpha);
			}
		}
		else if (blankerRenderer.enabled)
		{
			blankerRenderer.enabled = false;
		}
	}

	public override void Skip()
	{
		isSkipQueued = true;
	}

	public void FlipScaleX()
	{
		if ((bool)targetRenderer)
		{
			targetRenderer.transform.FlipLocalScale(x: true);
		}
	}

	private void OnEndPlayback()
	{
		RestoreResolution();
		if (isStarted)
		{
			isStarted = false;
			wasPlaying = false;
			this.OnPlaybackEnded?.Invoke();
			if (isSkipped && (bool)extraAudio)
			{
				extraAudio.Stop();
			}
		}
	}
}
