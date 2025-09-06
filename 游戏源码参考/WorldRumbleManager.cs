using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldRumbleManager : MonoBehaviour
{
	[Serializable]
	private class CameraShake : ICameraShake, ICameraShakeVibration
	{
		[SerializeField]
		[FormerlySerializedAs("Magnitude")]
		private float magnitude;

		public AnimationCurve Curve;

		public float CustomUpdateRate;

		[Space]
		public VibrationDataAsset vibration;

		public float vibrationStrength = 1f;

		private float length;

		private float inverseLength = 1f;

		private Func<float, float> processMagnitude;

		private float startUpdateTime;

		private float nextUpdateTime;

		private Vector2 startOffset;

		private Vector2 targetOffset;

		public bool CanFinish => true;

		public bool PersistThroughScenes => true;

		public float Magnitude => magnitude;

		public int FreezeFrames => 0;

		public ICameraShakeVibration CameraShakeVibration => this;

		public CameraShakeWorldForceIntensities WorldForceOnStart => CameraShakeWorldForceIntensities.None;

		public event Action<float> CurveEvaluated;

		public Vector2 GetOffset(float elapsedTime)
		{
			if (length <= 0f)
			{
				return Vector2.zero;
			}
			float num = Curve.Evaluate(elapsedTime / length);
			this.CurveEvaluated?.Invoke(num);
			if (CustomUpdateRate > 0f)
			{
				if (elapsedTime >= nextUpdateTime)
				{
					startOffset = targetOffset;
					targetOffset = GetTargetOffset(num);
					startUpdateTime = elapsedTime;
					nextUpdateTime = elapsedTime + 1f / CustomUpdateRate;
				}
				float num2 = nextUpdateTime - startUpdateTime;
				float num3 = elapsedTime - startUpdateTime;
				return Vector2.Lerp(startOffset, targetOffset, num3 / num2);
			}
			return GetTargetOffset(num);
		}

		private Vector2 GetTargetOffset(float t)
		{
			float num = processMagnitude(magnitude);
			return UnityEngine.Random.insideUnitCircle * (num * t);
		}

		public bool IsDone(float elapsedTime)
		{
			return elapsedTime >= length;
		}

		public void Setup(float newLength, Func<float, float> onProcessMagnitude)
		{
			length = newLength;
			if (length > 0f)
			{
				inverseLength = 1f / length;
			}
			else
			{
				inverseLength = 1f;
			}
			nextUpdateTime = 0f;
			processMagnitude = onProcessMagnitude;
		}

		public float GetInitialMagnitude()
		{
			return Curve.Evaluate(0f);
		}

		public VibrationEmission PlayVibration(bool isRealtime)
		{
			if (!vibration)
			{
				return null;
			}
			VibrationData vibrationData = vibration;
			bool isRealtime2 = isRealtime;
			return VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping: true, "", isRealtime2);
		}

		public float GetVibrationStrength(float timeElapsed)
		{
			return Curve.Evaluate(timeElapsed * inverseLength) * vibrationStrength;
		}
	}

	[Serializable]
	private class WorldRumble
	{
		public CameraShake CameraShake;

		public AudioClip[] Sounds;

		[SerializeField]
		private MinMaxFloat length;

		public float WindSpeedMultiplier = 1f;

		public float EventThreshold;

		public string SendEventOver;

		public string SendEventUnder;

		public GameObject ActiveWhileRumbling;

		public NestedFadeGroupBase FadeWithRumble;

		public float GetLength(AudioClip sound)
		{
			if (length.Start > Mathf.Epsilon || length.End > Mathf.Epsilon)
			{
				return length.GetRandomValue();
			}
			if ((bool)sound)
			{
				return sound.length;
			}
			return 0f;
		}
	}

	[Serializable]
	private class WorldRumbleGroup
	{
		public PlayerDataTest Condition;

		[EnumPickerBitmask(typeof(MapZone))]
		public long MapZoneMask;

		public MinMaxFloat WaitTime;

		public WorldRumble[] Rumbles;

		public string SendEventToRegister;
	}

	public interface IWorldRumblePreventer
	{
		bool AllowRumble { get; }
	}

	[SerializeField]
	private CameraManagerReference shakeCamera;

	[SerializeField]
	private AudioSource source;

	[SerializeField]
	private TransitionAudioFader transitionFader;

	[SerializeField]
	private ParticleEffectsLerpEmission particleController;

	[SerializeField]
	private NestedFadeGroupBase fadeUpGroup;

	[Space]
	[SerializeField]
	private bool isInScene;

	[SerializeField]
	private WorldRumbleGroup[] rumbleGroups;

	private WorldRumbleGroup sceneRumbleGroup;

	private bool hasStarted;

	private bool hasRumbled;

	private WorldRumble currentRumble;

	private float previousT;

	private bool isRumblesPrevented;

	private List<IWorldRumblePreventer> rumblePreventers;

	private float initialVolume;

	private Dictionary<UnityEngine.Object, float> magnitudeMultipliers;

	private bool hasTransitionAudioFader;

	private Coroutine fadeOutRoutine;

	public double WaitStartTime { get; private set; }

	public double WaitEndTime { get; private set; }

	public double SoundEndTime { get; private set; }

	private bool IsRumblesPrevented
	{
		get
		{
			if (isRumblesPrevented)
			{
				return true;
			}
			if (CheatManager.IsWorldRumbleDisabled)
			{
				return true;
			}
			if (rumblePreventers == null)
			{
				return false;
			}
			foreach (IWorldRumblePreventer rumblePreventer in rumblePreventers)
			{
				if (!rumblePreventer.AllowRumble)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void Awake()
	{
		initialVolume = source.volume;
		hasTransitionAudioFader = transitionFader != null;
		if (!hasTransitionAudioFader)
		{
			transitionFader = base.gameObject.GetComponentInChildren<TransitionAudioFader>();
			hasTransitionAudioFader = transitionFader != null;
		}
	}

	private void OnEnable()
	{
		WorldRumbleGroup[] array = rumbleGroups;
		for (int i = 0; i < array.Length; i++)
		{
			WorldRumble[] rumbles = array[i].Rumbles;
			foreach (WorldRumble worldRumble in rumbles)
			{
				if ((bool)worldRumble.ActiveWhileRumbling)
				{
					worldRumble.ActiveWhileRumbling.SetActive(value: false);
				}
			}
		}
		if (hasStarted)
		{
			OnSceneStarted();
		}
	}

	private void Start()
	{
		GameManager gm = GameManager.instance;
		if (isInScene)
		{
			if (!GameManager.IsWaitingForSceneReady)
			{
				OnSceneStarted();
				hasStarted = true;
			}
			else
			{
				gm.OnBeforeFinishedSceneTransition += SceneBeganCallback;
			}
			OnRumbleCurveEvaluated(0f);
			return;
		}
		if (!GameManager.IsWaitingForSceneReady)
		{
			OnSceneStarted();
		}
		hasStarted = true;
		gm.GamePausedChange += OnGamePauseChanged;
		gm.OnBeforeFinishedSceneTransition += OnSceneStarted;
		gm.GameStateChange += OnGameStateChanged;
		void SceneBeganCallback()
		{
			OnSceneStarted();
			hasStarted = true;
			gm.OnBeforeFinishedSceneTransition -= SceneBeganCallback;
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		CancelCurrentRumble();
	}

	private void OnDestroy()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (!isInScene && (bool)unsafeInstance)
		{
			unsafeInstance.GamePausedChange -= OnGamePauseChanged;
			unsafeInstance.OnBeforeFinishedSceneTransition -= OnSceneStarted;
			unsafeInstance.GameStateChange -= OnGameStateChanged;
		}
	}

	private void OnGamePauseChanged(bool isPaused)
	{
		if (currentRumble != null && !(source == null))
		{
			if (isPaused)
			{
				source.Pause();
			}
			else
			{
				source.UnPause();
			}
		}
	}

	private void OnGameStateChanged(GameState gameState)
	{
		if (gameState == GameState.CUTSCENE)
		{
			isRumblesPrevented = true;
			CancelCurrentRumble();
		}
	}

	public void OnSceneStarted()
	{
		isRumblesPrevented = false;
		rumblePreventers?.Clear();
		if (!source || rumbleGroups.Length == 0)
		{
			return;
		}
		sceneRumbleGroup = null;
		WorldRumbleGroup[] array = rumbleGroups;
		foreach (WorldRumbleGroup worldRumbleGroup in array)
		{
			if (worldRumbleGroup.Condition.IsFulfilled)
			{
				sceneRumbleGroup = worldRumbleGroup;
				break;
			}
		}
		bool flag = sceneRumbleGroup != null && CanRumbleInScene(sceneRumbleGroup);
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance && silentInstance.cState.dead)
		{
			flag = false;
		}
		if (flag)
		{
			if (currentRumble != null)
			{
				return;
			}
			CancelCurrentRumble();
			StopAllCoroutines();
			if (currentRumble != null)
			{
				currentRumble.CameraShake.CurveEvaluated -= OnRumbleCurveEvaluated;
				if ((bool)shakeCamera)
				{
					shakeCamera.CancelShake(currentRumble.CameraShake);
				}
			}
			currentRumble = null;
		}
		else
		{
			DoFadeOutAndEnd(0.25f);
		}
		WaitStartTime = 0.0;
		WaitEndTime = 0.0;
		SoundEndTime = 0.0;
		if (flag)
		{
			MinMaxFloat waitTime = sceneRumbleGroup.WaitTime;
			if (!(waitTime.Start < Mathf.Epsilon) || !(waitTime.End < Mathf.Epsilon))
			{
				StartCoroutine(DoRumbles());
			}
		}
	}

	public void DoRumbleNow()
	{
		if (!(Time.timeAsDouble < SoundEndTime))
		{
			OnSceneStarted();
			DoNewRumble();
		}
	}

	private IEnumerator DoRumbles()
	{
		yield return new WaitForSeconds(GetWaitTime(0f));
		while (true)
		{
			if (IsRumblesPrevented)
			{
				while (IsRumblesPrevented)
				{
					yield return null;
				}
				yield return new WaitForSeconds(GetWaitTime(0f));
			}
			float seconds = DoNewRumble();
			hasRumbled = true;
			yield return new WaitForSeconds(seconds);
		}
	}

	private void OnRumbleCurveEvaluated(float magnitude)
	{
		if (magnitude <= 0.001f)
		{
			magnitude = 0f;
		}
		if ((bool)fadeUpGroup)
		{
			fadeUpGroup.AlphaSelf = magnitude;
		}
		float speedMultiplier;
		if (currentRumble != null)
		{
			speedMultiplier = Mathf.Lerp(1f, currentRumble.WindSpeedMultiplier, magnitude);
			if (source.loop)
			{
				source.volume = initialVolume * magnitude * GetVolumeMultiplier();
			}
			else
			{
				source.volume = initialVolume * GetVolumeMultiplier();
			}
			if (previousT < currentRumble.EventThreshold)
			{
				if (magnitude >= currentRumble.EventThreshold)
				{
					EventRegister.SendEvent(currentRumble.SendEventOver);
				}
			}
			else if (magnitude < currentRumble.EventThreshold)
			{
				EventRegister.SendEvent(currentRumble.SendEventUnder);
			}
			if ((bool)currentRumble.FadeWithRumble)
			{
				currentRumble.FadeWithRumble.AlphaSelf = magnitude;
			}
			if (magnitude <= Mathf.Epsilon)
			{
				if ((bool)currentRumble.ActiveWhileRumbling && currentRumble.ActiveWhileRumbling.activeSelf)
				{
					currentRumble.ActiveWhileRumbling.SetActive(value: false);
				}
			}
			else if ((bool)currentRumble.ActiveWhileRumbling && !currentRumble.ActiveWhileRumbling.activeSelf)
			{
				currentRumble.ActiveWhileRumbling.SetActive(value: true);
			}
		}
		else
		{
			speedMultiplier = 1f;
		}
		foreach (UmbrellaWindRegion item in UmbrellaWindRegion.EnumerateActiveRegions())
		{
			item.SpeedMultiplier = speedMultiplier;
		}
		foreach (IdleForceAnimator item2 in IdleForceAnimator.EnumerateActiveAnimators())
		{
			item2.SpeedMultiplier = speedMultiplier;
		}
		previousT = magnitude;
	}

	private float DoNewRumble()
	{
		CancelFadeOut();
		CancelCurrentRumble();
		if (sceneRumbleGroup != null)
		{
			currentRumble = sceneRumbleGroup.Rumbles.GetRandomElement();
		}
		if (currentRumble == null)
		{
			return GetWaitTime(0f);
		}
		if (!string.IsNullOrEmpty(sceneRumbleGroup.SendEventToRegister))
		{
			EventRegister.SendEvent(sceneRumbleGroup.SendEventToRegister);
		}
		currentRumble.CameraShake.CurveEvaluated += OnRumbleCurveEvaluated;
		AudioClip randomElement = currentRumble.Sounds.GetRandomElement();
		source.clip = randomElement;
		float volume = source.volume;
		source.volume = 0f;
		source.Play();
		float length = currentRumble.GetLength(randomElement);
		if ((bool)shakeCamera)
		{
			source.volume = currentRumble.CameraShake.GetInitialMagnitude() * GetVolumeMultiplier();
			currentRumble.CameraShake.Setup(length, OnProcessMagnitude);
			shakeCamera.DoShake(currentRumble.CameraShake, this);
		}
		else
		{
			source.volume = volume * GetVolumeMultiplier();
		}
		if ((bool)particleController)
		{
			particleController.Play(length);
		}
		return GetWaitTime(length);
	}

	private float OnProcessMagnitude(float magnitude)
	{
		if (magnitudeMultipliers == null)
		{
			return magnitude;
		}
		foreach (KeyValuePair<UnityEngine.Object, float> magnitudeMultiplier in magnitudeMultipliers)
		{
			magnitude *= magnitudeMultiplier.Value;
		}
		return magnitude;
	}

	private float GetWaitTime(float clipTime)
	{
		MinMaxFloat waitTime = sceneRumbleGroup.WaitTime;
		float num = ((hasRumbled || !isInScene) ? (waitTime.GetRandomValue() + clipTime) : (UnityEngine.Random.Range(0f, waitTime.End) + clipTime));
		WaitStartTime = Time.timeAsDouble;
		WaitEndTime = WaitStartTime + (double)num;
		SoundEndTime = Time.timeAsDouble + (double)clipTime;
		return num;
	}

	private void CancelFadeOut()
	{
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
			fadeOutRoutine = null;
		}
	}

	private void CancelCurrentRumble()
	{
		OnRumbleCurveEvaluated(0f);
		if (currentRumble != null)
		{
			if ((bool)shakeCamera)
			{
				shakeCamera.CancelShake(currentRumble.CameraShake);
			}
			if ((bool)particleController)
			{
				particleController.Stop();
			}
			source.Stop();
			currentRumble.CameraShake.CurveEvaluated -= OnRumbleCurveEvaluated;
			currentRumble = null;
		}
	}

	private bool CanRumbleInScene(WorldRumbleGroup group)
	{
		GameManager instance = GameManager.instance;
		switch (instance.sm.WorldRumble)
		{
		case CustomSceneManager.WorldRumbleSettings.MapZone:
		{
			MapZone currentMapZoneEnum = instance.GetCurrentMapZoneEnum();
			if (!group.MapZoneMask.IsBitSet((int)currentMapZoneEnum))
			{
				return false;
			}
			if (instance.gameMap != null)
			{
				instance.gameMap.HasMapForScene(instance.GetSceneNameString(), out var sceneHasSprite);
				return sceneHasSprite;
			}
			return false;
		}
		case CustomSceneManager.WorldRumbleSettings.Enabled:
			return true;
		case CustomSceneManager.WorldRumbleSettings.Disabled:
			return false;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void ForceRumble()
	{
		DoNewRumble();
	}

	public void PreventRumbles()
	{
		isRumblesPrevented = true;
	}

	public void AllowRumbles()
	{
		isRumblesPrevented = false;
	}

	public void AddPreventer(IWorldRumblePreventer area)
	{
		if (rumblePreventers == null)
		{
			rumblePreventers = new List<IWorldRumblePreventer>();
		}
		rumblePreventers.AddIfNotPresent(area);
	}

	public void RemovePreventer(IWorldRumblePreventer area)
	{
		rumblePreventers?.Remove(area);
	}

	public void DoFadeOutAndDisable(float fadeTime)
	{
		StopAllCoroutines();
		if (currentRumble != null)
		{
			currentRumble.CameraShake.CurveEvaluated -= OnRumbleCurveEvaluated;
			if ((bool)shakeCamera)
			{
				shakeCamera.CancelShake(currentRumble.CameraShake);
			}
		}
		CancelFadeOut();
		fadeOutRoutine = StartCoroutine(FadeOutAndDisable(fadeTime));
	}

	private IEnumerator FadeOutAndDisable(float fadeTime)
	{
		float initialAlpha = (fadeUpGroup ? fadeUpGroup.AlphaSelf : previousT);
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			float magnitude = Mathf.Lerp(initialAlpha, 0f, elapsed / fadeTime);
			OnRumbleCurveEvaluated(magnitude);
			yield return null;
		}
		OnRumbleCurveEvaluated(0f);
		base.gameObject.SetActive(value: false);
		fadeOutRoutine = null;
	}

	public void DoFadeOutAndEnd(float fadeTime)
	{
		StopAllCoroutines();
		if (currentRumble != null)
		{
			CancelFadeOut();
			fadeOutRoutine = StartCoroutine(FadeOutAndEnd(fadeTime));
		}
	}

	private IEnumerator FadeOutAndEnd(float fadeTime)
	{
		float initialAlpha = (fadeUpGroup ? fadeUpGroup.AlphaSelf : previousT);
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			float magnitude = Mathf.Lerp(initialAlpha, 0f, elapsed / fadeTime);
			OnRumbleCurveEvaluated(magnitude);
			yield return null;
		}
		OnRumbleCurveEvaluated(0f);
		CancelCurrentRumble();
		fadeOutRoutine = null;
	}

	public void AddMagnitudeMultiplier(UnityEngine.Object from, float multiplier)
	{
		if (magnitudeMultipliers == null)
		{
			magnitudeMultipliers = new Dictionary<UnityEngine.Object, float>();
		}
		magnitudeMultipliers[from] = multiplier;
	}

	public void RemoveMagnitudeMultiplier(UnityEngine.Object from)
	{
		magnitudeMultipliers?.Remove(from);
	}

	private float GetVolumeMultiplier()
	{
		if (!hasTransitionAudioFader)
		{
			return 1f;
		}
		return transitionFader.Volume;
	}
}
