using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceGamePause : MonoBehaviour
{
	private AudioSource source;

	public bool IsPaused { get; private set; }

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		GameManager.instance.GamePausedChange += OnGamePausedChanged;
	}

	private void OnDestroy()
	{
		if ((bool)GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.GamePausedChange -= OnGamePausedChanged;
		}
	}

	private void OnGamePausedChanged(bool isPaused)
	{
		if (base.isActiveAndEnabled)
		{
			if (isPaused)
			{
				source.Pause();
			}
			else
			{
				source.UnPause();
			}
			IsPaused = isPaused;
		}
	}
}
