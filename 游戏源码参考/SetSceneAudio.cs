using UnityEngine;
using UnityEngine.Audio;

public class SetSceneAudio : MonoBehaviour
{
	public AudioMixerSnapshot atmosSnapshot;

	public AudioMixerSnapshot enviroSnapshot;

	public AudioMixerSnapshot actorSnapshot;

	public float transitionTime;

	private void Start()
	{
		atmosSnapshot.TransitionTo(transitionTime);
		enviroSnapshot.TransitionTo(transitionTime);
		actorSnapshot.TransitionTo(transitionTime);
	}
}
