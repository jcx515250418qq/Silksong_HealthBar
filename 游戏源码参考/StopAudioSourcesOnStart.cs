using UnityEngine;

public class StopAudioSourcesOnStart : MonoBehaviour, IBeginStopper
{
	private AudioSource[] sources;

	private bool hasStopped;

	private void Awake()
	{
		sources = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	private void OnEnable()
	{
		hasStopped = false;
	}

	private void LateUpdate()
	{
		if (!hasStopped)
		{
			hasStopped = true;
			AudioSource[] array = sources;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}
	}

	public void DoBeginStop()
	{
		hasStopped = false;
	}
}
