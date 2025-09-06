using UnityEngine;

public sealed class AudioPlayStateSync : MonoBehaviour
{
	[SerializeField]
	private AudioSource otherSource;

	[SerializeField]
	private AudioSource selfSource;

	[SerializeField]
	private bool disableScriptOnStop;

	private bool didPlay;

	private void Awake()
	{
		if (selfSource == null)
		{
			selfSource = GetComponent<AudioSource>();
		}
	}

	private void OnEnable()
	{
		if (selfSource == null)
		{
			base.enabled = false;
		}
		if (otherSource == null)
		{
			base.enabled = false;
		}
		didPlay = false;
	}

	private void Update()
	{
		if (otherSource.isPlaying)
		{
			didPlay = true;
			if (!selfSource.isPlaying)
			{
				selfSource.Play();
			}
			return;
		}
		if (selfSource.isPlaying)
		{
			selfSource.Stop();
		}
		if (didPlay && disableScriptOnStop)
		{
			selfSource.Stop();
			base.enabled = false;
		}
	}

	public void SetTarget(AudioSource target)
	{
		otherSource = target;
		base.enabled = true;
	}
}
