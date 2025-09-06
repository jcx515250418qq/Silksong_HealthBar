using JetBrains.Annotations;
using UnityEngine;

public class AudioEventAnimationEvents : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("DisablePlayOnSource", false, true, false)]
	private AudioSource playOnSource;

	[SerializeField]
	private AudioEventRandom[] audioEvents;

	[UsedImplicitly]
	private bool DisablePlayOnSource()
	{
		return audioSourcePrefab;
	}

	public void PlayAudioEvent(int index)
	{
		AudioEventRandom audioEventRandom = audioEvents[index];
		if ((bool)audioSourcePrefab || !playOnSource)
		{
			audioEventRandom.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
			return;
		}
		playOnSource.pitch = audioEventRandom.SelectPitch();
		playOnSource.PlayOneShot(audioEventRandom.GetClip(), audioEventRandom.Volume);
	}
}
