using UnityEngine;

public class GamePadAudio : MonoBehaviour
{
	private AudioSource audioSource;

	private AudioSource vibrationSource;

	private void Awake()
	{
	}

	private void Init()
	{
		audioSource = base.gameObject.AddComponent<AudioSource>();
		vibrationSource = base.gameObject.AddComponent<AudioSource>();
	}
}
