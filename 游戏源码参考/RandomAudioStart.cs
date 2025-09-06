using UnityEngine;

public class RandomAudioStart : MonoBehaviour
{
	public AudioSource audioSource;

	public float timeMin;

	public float timeMax = 1f;

	public float pitchMin = 1f;

	public float pitchMax = 1f;

	[SerializeField]
	private bool waitForHeroInPosition;

	private bool started;

	private bool registeredHeroEvent;

	private HeroController hc;

	private void Start()
	{
		started = true;
		SafeStart();
	}

	private void OnEnable()
	{
		if (started)
		{
			SafeStart();
		}
	}

	private void OnDisable()
	{
		if (registeredHeroEvent && hc != null)
		{
			hc.heroInPosition -= OnHeroInPosition;
		}
		hc = null;
	}

	private void SafeStart()
	{
		if (!waitForHeroInPosition)
		{
			DoRandomAudioStart();
			return;
		}
		hc = HeroController.instance;
		if (hc != null && !hc.isHeroInPosition)
		{
			hc.heroInPosition += OnHeroInPosition;
			registeredHeroEvent = true;
		}
		else
		{
			DoRandomAudioStart();
		}
	}

	private void OnHeroInPosition(bool forceDirect)
	{
		DoRandomAudioStart();
		if (hc != null)
		{
			hc.heroInPosition -= OnHeroInPosition;
		}
		registeredHeroEvent = false;
	}

	private void DoRandomAudioStart()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				return;
			}
		}
		float num = Random.Range(timeMin, timeMax);
		if (audioSource.clip != null && num > audioSource.clip.length)
		{
			num = 0f;
		}
		audioSource.time = num;
		audioSource.pitch = Random.Range(pitchMin, pitchMax);
		audioSource.Play();
	}
}
