using TeamCherry.SharedUtils;
using UnityEngine;

public class PlayRandomAudioClip : MonoBehaviour
{
	[SerializeField]
	private AudioSource source;

	[SerializeField]
	private AudioClip[] clips;

	[SerializeField]
	private MinMaxFloat pitchRange = new MinMaxFloat(1f, 1f);

	[SerializeField]
	private MinMaxFloat volumeRange = new MinMaxFloat(1f, 1f);

	[SerializeField]
	private bool onEnable;

	private void Reset()
	{
		source = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			Play();
		}
	}

	public void Play()
	{
		source.clip = clips[Random.Range(0, clips.Length)];
		source.pitch = pitchRange.GetRandomValue();
		source.volume = volumeRange.GetRandomValue();
		source.Play();
	}
}
