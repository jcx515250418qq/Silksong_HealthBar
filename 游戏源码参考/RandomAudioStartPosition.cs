using UnityEngine;

public class RandomAudioStartPosition : MonoBehaviour
{
	[SerializeField]
	private float timeMin;

	[SerializeField]
	private float timeMax;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (!(audioSource == null) && !(audioSource.clip == null))
		{
			float time = ((timeMin == 0f && timeMax == 0f) ? Random.Range(0f, audioSource.clip.length) : Random.Range(timeMin, timeMax));
			audioSource.time = time;
		}
	}
}
