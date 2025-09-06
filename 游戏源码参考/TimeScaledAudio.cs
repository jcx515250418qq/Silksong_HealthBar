using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class TimeScaledAudio : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private MinMaxFloat pitchLimits = new MinMaxFloat(0f, 1f);

	private bool registeredEvents;

	private void Awake()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				base.enabled = false;
			}
		}
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvents)
		{
			registeredEvents = true;
			TimeManager.OnTimeScaleUpdated += OnTimeScaleUpdated;
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvents)
		{
			registeredEvents = false;
			TimeManager.OnTimeScaleUpdated -= OnTimeScaleUpdated;
		}
	}

	private void OnTimeScaleUpdated(float timeScale)
	{
		audioSource.pitch = pitchLimits.GetClampedBetween(timeScale);
	}
}
