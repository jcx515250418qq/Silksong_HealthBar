using UnityEngine;
using UnityEngine.Audio;

public class SnapshotOnActivation : MonoBehaviour
{
	public AudioMixerSnapshot activationSnapshot;

	public AudioMixerSnapshot deactivationSnapshot;

	public float transitionTime;

	private void OnEnable()
	{
		activationSnapshot.TransitionTo(transitionTime);
	}

	private void OnDisable()
	{
		deactivationSnapshot.TransitionTo(transitionTime);
	}
}
