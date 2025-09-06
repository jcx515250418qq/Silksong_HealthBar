using System.Collections;
using UnityEngine;

public sealed class AudioSyncedVibration : MonoBehaviour
{
	[SerializeField]
	private AudioSource syncSource;

	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private float syncThreshold = 1f;

	[SerializeField]
	private bool loop;

	[SerializeField]
	private bool isRealTime;

	[SerializeField]
	private bool stopOnDisable;

	private VibrationEmission emission;

	private bool hasSyncSource;

	private float previousTime;

	private Coroutine fadeRoutine;

	private void Start()
	{
		hasSyncSource = syncSource;
	}

	private void OnDisable()
	{
		if (stopOnDisable)
		{
			StopVibration();
		}
		else if (loop && emission != null)
		{
			emission.IsLooping = false;
			emission = null;
		}
	}

	private void Update()
	{
		if (hasSyncSource && emission != null)
		{
			float time = syncSource.time;
			if (time < previousTime)
			{
				emission.SetPlaybackTime(time);
			}
			else if (Mathf.Abs(syncSource.time - emission.Time) >= syncThreshold)
			{
				emission.SetPlaybackTime(time);
			}
			previousTime = time;
		}
	}

	public void PlayVibration()
	{
		PlayVibration(0f);
	}

	public void PlayVibration(float fadeDuration)
	{
		StopVibration();
		VibrationData vibrationData = vibrationDataAsset;
		bool isLooping = loop;
		bool isRealtime = isRealTime;
		emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, "", isRealtime);
		if (hasSyncSource && emission != null)
		{
			float time = syncSource.time;
			emission.SetPlaybackTime(time);
			previousTime = time;
		}
		FadeInEmission(fadeDuration);
	}

	private void FadeInEmission(float duration)
	{
		if (!(duration <= 0f) && emission != null && base.gameObject.activeInHierarchy)
		{
			emission.SetStrength(0f);
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeRoutine(1f, duration));
		}
	}

	public void FadeOut(float duration)
	{
		if (duration <= 0f || emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			emission.Stop();
			emission = null;
			return;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeRoutine(0f, duration));
	}

	private IEnumerator FadeRoutine(float targetStrength, float fade)
	{
		float inverse = 1f / fade;
		float start = emission.Strength;
		float t = 0f;
		while (t < 1f)
		{
			yield return null;
			float deltaTime = Time.deltaTime;
			t += deltaTime * inverse;
			emission.SetStrength(Mathf.Lerp(start, targetStrength, t));
		}
		emission.SetStrength(targetStrength);
		fadeRoutine = null;
	}

	public void StopVibration()
	{
		emission?.Stop();
		emission = null;
	}
}
