using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnableAudioSource : MonoBehaviour
{
	private AudioSource audioSource;

	private const int MinimumFrames = 5;

	private int framesPassed;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	protected void OnEnable()
	{
		framesPassed = 0;
	}

	protected void Update()
	{
		framesPassed++;
		if (framesPassed > 5 && !audioSource.isPlaying)
		{
			this.Recycle();
		}
	}

	public void Stop()
	{
		audioSource.Stop();
	}
}
