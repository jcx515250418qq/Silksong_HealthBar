using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizerV3 : MonoBehaviour
{
	[Header("Randomize Pitch")]
	public float pitchLower = 1f;

	public float pitchUpper = 1f;

	public bool playAfterPitchSet;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(pitchLower, pitchUpper);
		if (playAfterPitchSet)
		{
			audioSource.Play();
		}
	}

	private void OnEnable()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(pitchLower, pitchUpper);
		if (playAfterPitchSet)
		{
			audioSource.Play();
		}
	}
}
