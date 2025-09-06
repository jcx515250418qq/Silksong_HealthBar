using UnityEngine;

public sealed class FadeAudioOnPause : MonoBehaviour
{
	private enum FadeState
	{
		None = 0,
		FadingUp = 1,
		FadingDown = 2
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float fadeRate = 1f;

	[SerializeField]
	private float pausedVolume = 0.25f;

	private float volume;

	private bool hasAudioSource;

	private bool hasModifier;

	private VolumeModifier volumeModifier;

	private FadeState fadeState;

	private float targetVolume;

	private float originalVolume = 1f;

	private GameManager gm;

	private void Awake()
	{
		hasAudioSource = audioSource != null;
		if (!hasAudioSource)
		{
			audioSource = GetComponent<AudioSource>();
			hasModifier = audioSource != null;
		}
		VolumeBlendController component = GetComponent<VolumeBlendController>();
		if (component != null)
		{
			volumeModifier = component.GetModifier("PAUSE_FADE");
			hasModifier = true;
		}
		if (hasModifier)
		{
			volume = 1f;
		}
		else if (hasAudioSource)
		{
			originalVolume = audioSource.volume;
			volume = audioSource.volume;
		}
	}

	private void OnEnable()
	{
		gm = GameManager.instance;
		if ((bool)gm)
		{
			gm.GamePausedChange += GamePausedChange;
		}
	}

	private void OnDisable()
	{
		if ((bool)gm)
		{
			gm.GamePausedChange -= GamePausedChange;
		}
	}

	private void Update()
	{
		if (fadeState > FadeState.None)
		{
			volume = Mathf.MoveTowards(volume, targetVolume, fadeRate * Time.unscaledDeltaTime);
			SetVolume(volume);
			if (Mathf.Approximately(volume, targetVolume))
			{
				fadeState = FadeState.None;
			}
		}
	}

	private void SetVolume(float volume)
	{
		this.volume = volume;
		if (hasModifier)
		{
			volumeModifier.Volume = volume;
		}
		else if (hasAudioSource)
		{
			audioSource.volume = volume;
		}
	}

	private void GamePausedChange(bool ispaused)
	{
		if (ispaused)
		{
			targetVolume = pausedVolume;
			if (fadeState != FadeState.FadingDown)
			{
				fadeState = FadeState.FadingDown;
				if (hasModifier)
				{
					originalVolume = 1f;
				}
				else if (hasAudioSource)
				{
					originalVolume = audioSource.volume;
					volume = audioSource.volume;
				}
			}
		}
		else if (fadeState != FadeState.FadingUp)
		{
			fadeState = FadeState.FadingUp;
			targetVolume = originalVolume;
		}
	}
}
