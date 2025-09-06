using System.Collections;
using UnityEngine;

public sealed class VibrationPlayerAdvanced : MonoBehaviour
{
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[Space]
	[SerializeField]
	private float strength = 1f;

	[Space]
	[SerializeField]
	private bool playOnEnable;

	[SerializeField]
	private bool stopOnDisable;

	[Space]
	[SerializeField]
	private bool isLooping;

	[SerializeField]
	private bool isRealtime;

	[SerializeField]
	private string vibrationTag;

	[Space]
	[SerializeField]
	private bool doTimedStrength;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Constant(0f, 1f, 1f);

	private VibrationEmission emission;

	private Coroutine coroutine;

	public bool IsLooping
	{
		get
		{
			return isLooping;
		}
		set
		{
			isLooping = value;
			if (emission != null)
			{
				emission.IsLooping = isLooping;
			}
		}
	}

	public string VibrationTag
	{
		get
		{
			return vibrationTag;
		}
		set
		{
			vibrationTag = value;
			if (emission != null)
			{
				emission.Tag = vibrationTag;
			}
		}
	}

	private void OnEnable()
	{
		if (playOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		if (stopOnDisable)
		{
			Stop();
		}
		else if (isLooping && emission != null)
		{
			emission.IsLooping = false;
			emission = null;
		}
	}

	public void Play()
	{
		if (emission != null)
		{
			emission.Stop();
		}
		VibrationData vibrationData = vibrationDataAsset;
		bool flag = isLooping;
		string text = base.tag;
		bool flag2 = isRealtime;
		emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, flag, text, flag2);
		if (doTimedStrength)
		{
			StartStrengthRoutine();
		}
		else
		{
			emission.SetStrength(strength);
		}
	}

	public void Stop()
	{
		if (emission != null)
		{
			emission.Stop();
			emission = null;
		}
	}

	private void StartStrengthRoutine()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
		emission?.SetStrength(curve.Evaluate(0f) * strength);
		coroutine = StartCoroutine(TimePlayRoutine());
	}

	private IEnumerator TimePlayRoutine()
	{
		if (duration > 0f)
		{
			float t = 0f;
			float inverse = 1f / duration;
			emission?.SetStrength(curve.Evaluate(t) * strength);
			while (t < 1f)
			{
				VibrationEmission vibrationEmission = emission;
				if (vibrationEmission == null || !vibrationEmission.IsPlaying)
				{
					break;
				}
				yield return null;
				float num = (isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
				t += inverse * num;
				emission?.SetStrength(curve.Evaluate(Mathf.Clamp01(t)) * strength);
			}
		}
		emission?.SetStrength(curve.Evaluate(duration) * strength);
		Stop();
		coroutine = null;
	}
}
