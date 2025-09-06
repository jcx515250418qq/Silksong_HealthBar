using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePitchRandomizer : MonoBehaviour
{
	[Header("Randomize Pitch")]
	[Range(0.75f, 1f)]
	public float pitchLower = 1f;

	[Range(1f, 1.25f)]
	public float pitchUpper = 1f;

	private AudioSource audioSource;

	private void Awake()
	{
		RandomisePitch();
	}

	private void OnEnable()
	{
		RandomisePitch();
	}

	public void RandomisePitch()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(pitchLower, pitchUpper);
	}
}
