using UnityEngine;

public sealed class AudioEffectTag : MonoBehaviour, IInitialisable
{
	public enum AudioEffectType
	{
		BlackThreadVoice = 0
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioEffectType audioEffectType;

	private bool hasAwaken;

	private bool hasStarted;

	public AudioSource AudioSource
	{
		get
		{
			OnAwake();
			return audioSource;
		}
	}

	public AudioEffectType EffectType => audioEffectType;

	GameObject IInitialisable.gameObject => base.gameObject;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}
}
