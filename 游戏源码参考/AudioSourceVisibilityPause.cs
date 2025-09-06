using UnityEngine;

public class AudioSourceVisibilityPause : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	private bool hasAudioSource;

	private void Awake()
	{
		if (!(audioSource == null))
		{
			return;
		}
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			Debug.LogError($"{this} is missing it's audio source. Destroying script to prevent null ref.", base.gameObject);
			if (Application.isPlaying)
			{
				Object.Destroy(this);
			}
		}
	}

	private void Reset()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnBecameVisible()
	{
		audioSource.UnPause();
	}

	private void OnBecameInvisible()
	{
		audioSource.Pause();
	}
}
