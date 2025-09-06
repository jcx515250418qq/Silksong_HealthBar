using System.Collections.Generic;
using UnityEngine;

public sealed class AudioVibrationSyncer : ManagerSingleton<AudioVibrationSyncer>
{
	public sealed class SyncedEmission
	{
		public readonly VibrationEmission emission;

		public readonly AudioSource audioSource;

		private readonly float syncThreshold;

		private float previousTime;

		public SyncedEmission(VibrationEmission emission, AudioSource audioSource, float syncThreshold)
		{
			this.emission = emission;
			this.audioSource = audioSource;
			this.syncThreshold = syncThreshold;
			float time = audioSource.time;
			emission.SetPlaybackTime(time);
			previousTime = time;
		}

		public bool Update()
		{
			if (!emission.IsPlaying)
			{
				return false;
			}
			float time = audioSource.time;
			if (time < previousTime)
			{
				emission.SetPlaybackTime(time);
			}
			else if (Mathf.Abs(time - emission.Time) >= syncThreshold)
			{
				emission.SetPlaybackTime(time);
			}
			previousTime = time;
			return true;
		}
	}

	private static bool isInitialised;

	private static readonly List<SyncedEmission> SYNCED_EMISSIONS = new List<SyncedEmission>();

	private const float syncThreshold = 0.1f;

	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this)
		{
			isInitialised = true;
		}
	}

	private void Start()
	{
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this && SYNCED_EMISSIONS.Count == 0)
		{
			base.enabled = false;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this)
		{
			isInitialised = false;
		}
	}

	private void Update()
	{
		for (int num = SYNCED_EMISSIONS.Count - 1; num >= 0; num--)
		{
			if (!SYNCED_EMISSIONS[num].Update())
			{
				SYNCED_EMISSIONS.RemoveAt(num);
			}
		}
		if (SYNCED_EMISSIONS.Count == 0)
		{
			base.enabled = false;
		}
	}

	public static VibrationEmission StartSyncedEmission(VibrationDataAsset vibrationDataAsset, AudioSource audioSource, bool isLooping, bool isRealTime)
	{
		return StartSyncedEmission(vibrationDataAsset, audioSource, isLooping, isRealTime, 0.1f);
	}

	private static VibrationEmission StartSyncedEmission(VibrationDataAsset vibrationDataAsset, AudioSource audioSource, bool isLooping, bool isRealTime, float syncThreshold)
	{
		VibrationData vibrationData = vibrationDataAsset;
		bool isLooping2 = isLooping;
		bool isRealtime = isRealTime;
		VibrationEmission vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping2, "", isRealtime);
		StartSyncedEmission(vibrationEmission, audioSource, syncThreshold);
		return vibrationEmission;
	}

	public static void StartSyncedEmission(VibrationEmission emission, AudioSource audioSource)
	{
		StartSyncedEmission(emission, audioSource, 0.1f);
	}

	private static void StartSyncedEmission(VibrationEmission emission, AudioSource audioSource, float syncThreshold)
	{
		if (emission != null && emission.IsPlaying && !(audioSource == null))
		{
			SYNCED_EMISSIONS.Add(new SyncedEmission(emission, audioSource, syncThreshold));
			if (isInitialised)
			{
				ManagerSingleton<AudioVibrationSyncer>.Instance.enabled = true;
			}
		}
	}
}
