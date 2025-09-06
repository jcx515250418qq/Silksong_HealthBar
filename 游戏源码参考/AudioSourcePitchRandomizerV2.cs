using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizerV2 : MonoBehaviour
{
	[Header("Randomize Pitch")]
	[Range(0.1f, 1f)]
	public float pitchLower = 1f;

	[Range(1f, 10f)]
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
