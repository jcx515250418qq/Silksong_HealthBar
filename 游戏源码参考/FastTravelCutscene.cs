using System;
using System.Collections;
using GlobalEnums;
using TeamCherry.Cinematics;
using UnityEngine;

public class FastTravelCutscene : MonoBehaviour, GameManager.ISkippable
{
	private class FastTravelAsyncLoadInfo : GameManager.SceneLoadInfo
	{
		private readonly FastTravelCutscene fastTravel;

		public FastTravelAsyncLoadInfo(FastTravelCutscene fastTravel)
		{
			this.fastTravel = fastTravel;
		}

		public override void NotifyFetchComplete()
		{
			base.NotifyFetchComplete();
			fastTravel.NotifyFetchComplete();
		}

		public override bool IsReadyToActivate()
		{
			if (base.IsReadyToActivate() && fastTravel.IsReadyToActivate)
			{
				return fastTravel.IsCutsceneComplete;
			}
			return false;
		}

		public override void NotifyFinished()
		{
			base.NotifyFinished();
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE OUT INSTANT");
			GameCameras.instance.OnCinematicEnd();
		}
	}

	[Serializable]
	private class PlatformLoadSettings
	{
		public bool overrideBackgroundLoadPriority;

		public ThreadPriority backgroundLoadPriority;

		[Space]
		public bool overrideAsyncLoadPriority;

		public int asyncLoadPriority = 100;
	}

	[Serializable]
	private enum Platform
	{
		XBoxOne = 0
	}

	[SerializeField]
	private CinematicSequence cinematicSequence;

	[Header("Load Priority")]
	[SerializeField]
	private bool useLoadPriority;

	[SerializeField]
	private ThreadPriority backgroundLoadPriority = ThreadPriority.BelowNormal;

	[Header("Platform Specific")]
	[SerializeField]
	[ArrayForEnum(typeof(Platform))]
	private PlatformLoadSettings[] platformLoadSettings = new PlatformLoadSettings[0];

	private bool isAsync;

	private bool isFetchComplete;

	private bool isSkipping;

	private bool isSkipFadeComplete;

	private ThreadPriority oldThreadPriority;

	private bool appliedThreadPriority;

	private bool hasPlatformSettings;

	private PlatformLoadSettings loadSettings;

	private bool hasStartedTransition;

	protected virtual bool IsReadyToActivate => true;

	protected bool IsCutsceneComplete { get; private set; }

	protected virtual bool ShouldFlipX => false;

	protected IEnumerator Start()
	{
		CheckPlatformOverrides();
		GameManager gm = GameManager.instance;
		gm.RegisterSkippable(this);
		while (gm.IsInSceneTransition)
		{
			yield return null;
		}
		if (StaticVariableList.GetValue("SkipCutscene", defaultValue: false))
		{
			StaticVariableList.SetValue("SkipCutscene", false);
			DoTransition();
			OnSkipped();
			IsCutsceneComplete = true;
			yield break;
		}
		isAsync = global::Platform.Current.FetchScenesBeforeFade;
		IsCutsceneComplete = false;
		CinematicVideoReference videoReference = GetVideoReference();
		if ((bool)videoReference)
		{
			cinematicSequence.VideoReference = videoReference;
		}
		cinematicSequence.VibrationDataAsset = GetVibrationData();
		GameCameras.instance.OnCinematicBegin();
		if (ShouldFlipX)
		{
			cinematicSequence.FlipScaleX();
		}
		if (!isAsync)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
			GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
			if (cinematicSequence.HasVideoReference)
			{
				bool flag = false;
				try
				{
					cinematicSequence.Begin();
					flag = true;
				}
				catch (Exception)
				{
				}
				if (flag)
				{
					while (cinematicSequence.IsPlaying && !isSkipFadeComplete)
					{
						yield return null;
					}
				}
			}
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE OUT INSTANT");
			yield return null;
			OnFadedOut();
			while (!IsReadyToActivate)
			{
				yield return null;
			}
			IsCutsceneComplete = true;
			DoTransition();
			yield break;
		}
		StartCoroutine("FadeInRoutine");
		GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		if (cinematicSequence.HasVideoReference)
		{
			if (useLoadPriority)
			{
				ApplyStreamingPriority();
			}
			cinematicSequence.Begin();
		}
		DoTransition();
	}

	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	private void OnValidate()
	{
		if (platformLoadSettings == null)
		{
			platformLoadSettings = new PlatformLoadSettings[1]
			{
				new PlatformLoadSettings()
			};
		}
		ArrayForEnumAttribute.EnsureArraySize(ref platformLoadSettings, typeof(Platform));
	}

	private void OnDisable()
	{
		RestoreLoadingPriority();
	}

	private void DoTransition()
	{
		if (!hasStartedTransition)
		{
			hasStartedTransition = true;
			FastTravelAsyncLoadInfo fastTravelAsyncLoadInfo = new FastTravelAsyncLoadInfo(this)
			{
				EntryGateName = "door_fastTravelExit",
				SceneName = GameManager.instance.playerData.nextScene,
				PreventCameraFadeOut = true,
				Visualization = GameManager.SceneLoadVisualizations.Custom,
				AsyncPriority = -10,
				AlwaysUnloadUnusedAssets = true
			};
			if (CheatManager.OverrideAsyncLoadPriority)
			{
				fastTravelAsyncLoadInfo.AsyncPriority = CheatManager.AsyncLoadPriority;
			}
			else if (hasPlatformSettings && loadSettings.overrideAsyncLoadPriority)
			{
				fastTravelAsyncLoadInfo.AsyncPriority = loadSettings.asyncLoadPriority;
			}
			GameManager.instance.BeginSceneTransition(fastTravelAsyncLoadInfo);
		}
	}

	private IEnumerator FadeInRoutine()
	{
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE OUT INSTANT");
		yield return new WaitForSeconds(1.5f);
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
	}

	protected void Update()
	{
		if (cinematicSequence.DidPlay)
		{
			CinematicVideoPlayer videoPlayer = cinematicSequence.VideoPlayer;
			if (isAsync && !isSkipping && isFetchComplete && (videoPlayer == null || videoPlayer.CurrentTime >= cinematicSequence.VideoLength))
			{
				StartCoroutine(Skip());
			}
		}
	}

	protected void NotifyFetchComplete()
	{
		isFetchComplete = true;
		GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
	}

	public IEnumerator Skip()
	{
		if (!isSkipping)
		{
			StopCoroutine("FadeInRoutine");
			isSkipping = true;
			GameCameras.instance.cameraFadeFSM.SendEventSafe("CINEMATIC SKIP FADE");
			yield return new WaitForSecondsRealtime(0.3f);
			RestoreLoadingPriority();
			isSkipFadeComplete = true;
			IsCutsceneComplete = true;
			OnSkipped();
		}
	}

	protected virtual CinematicVideoReference GetVideoReference()
	{
		return null;
	}

	protected virtual void OnFadedOut()
	{
	}

	protected virtual void OnSkipped()
	{
	}

	protected virtual VibrationDataAsset GetVibrationData()
	{
		return null;
	}

	private void ApplyStreamingPriority()
	{
		if (!appliedThreadPriority)
		{
			appliedThreadPriority = true;
			oldThreadPriority = Application.backgroundLoadingPriority;
			if (CheatManager.OverrideFastTravelBackgroundLoadPriority)
			{
				global::Platform.Current.SetBackgroundLoadingPriority(CheatManager.BackgroundLoadPriority);
			}
			else
			{
				global::Platform.Current.SetBackgroundLoadingPriority(backgroundLoadPriority);
			}
		}
	}

	private void RestoreLoadingPriority()
	{
		if (appliedThreadPriority)
		{
			appliedThreadPriority = false;
			global::Platform.Current.RestoreBackgroundLoadingPriority();
		}
	}

	private void CheckPlatformOverrides()
	{
	}
}
