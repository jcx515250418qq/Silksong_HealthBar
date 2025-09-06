using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
	[SerializeField]
	protected RandomAudioClipTable footstepsTable;

	[SerializeField]
	protected AudioSource audioSource;

	protected virtual void Awake()
	{
		_ = footstepsTable == null;
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			_ = audioSource == null;
		}
	}

	protected virtual void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	public void PlayFootstep()
	{
		footstepsTable.PlayOneShot(audioSource);
	}
}
