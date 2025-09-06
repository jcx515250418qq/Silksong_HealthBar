using TeamCherry.SharedUtils;
using UnityEngine;

public class PlayChainSound : MonoBehaviour
{
	[SerializeField]
	private AudioSource source;

	[Space]
	[SerializeField]
	private AudioClip[] hitSounds;

	[SerializeField]
	private MinMaxFloat hitSoundPitch = new MinMaxFloat(1f, 1f);

	[SerializeField]
	private AudioClip[] brokenHitSounds;

	[SerializeField]
	private MinMaxFloat brokenHitSoundPitch = new MinMaxFloat(1f, 1f);

	[SerializeField]
	private AudioClip[] touchSounds;

	[SerializeField]
	private MinMaxFloat touchSoundPitch = new MinMaxFloat(1f, 1f);

	private void Awake()
	{
		if (!source)
		{
			source = GetComponent<AudioSource>();
		}
	}

	public void PlayHitSound(Vector3 position)
	{
		if (hitSounds.Length != 0)
		{
			AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
			PlaySound(position, clip, hitSoundPitch);
		}
	}

	public void PlayBrokenHitSound(Vector3 position)
	{
		if (brokenHitSounds.Length != 0)
		{
			AudioClip clip = brokenHitSounds[Random.Range(0, brokenHitSounds.Length)];
			PlaySound(position, clip, brokenHitSoundPitch);
		}
	}

	public void PlayTouchSound(Vector3 position)
	{
		if (touchSounds.Length != 0)
		{
			AudioClip clip = touchSounds[Random.Range(0, touchSounds.Length)];
			PlaySound(position, clip, touchSoundPitch);
		}
	}

	private void PlaySound(Vector3 position, AudioClip clip, MinMaxFloat pitch)
	{
		if ((bool)source)
		{
			source.pitch = pitch.GetRandomValue();
			source.PlayOneShot(clip);
			return;
		}
		AudioEvent audioEvent = default(AudioEvent);
		audioEvent.Clip = clip;
		audioEvent.Volume = 1f;
		audioEvent.PitchMin = pitch.Start;
		audioEvent.PitchMax = pitch.End;
		AudioEvent audioEvent2 = audioEvent;
		audioEvent2.SpawnAndPlayOneShot(position);
	}
}
