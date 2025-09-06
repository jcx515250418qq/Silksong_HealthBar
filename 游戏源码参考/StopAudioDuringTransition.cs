using UnityEngine;

public class StopAudioDuringTransition : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void OnEnable()
	{
		if (!(audioSource == null))
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance && instance.cState.transitioning && !audioSource.loop)
			{
				audioSource.Stop();
			}
		}
	}
}
