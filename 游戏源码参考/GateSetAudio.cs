using UnityEngine;
using UnityEngine.Audio;

public class GateSetAudio : MonoBehaviour
{
	public AudioMixerSnapshot atmosSnapshot;

	public AudioMixerSnapshot enviroSnapshot;

	public float transitionTime;

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player")
		{
			atmosSnapshot.TransitionTo(transitionTime);
			enviroSnapshot.TransitionTo(transitionTime);
		}
	}
}
