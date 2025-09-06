using UnityEngine;

public sealed class LoopedAudioStateBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private AudioEvent loopedAudioClip;

	[SerializeField]
	private bool tryFindAudioSourceOnAnimator;

	[Tooltip("Will attempt to find an audio source on animator if null.")]
	[SerializeField]
	private AudioSource prefab;

	[SerializeField]
	private bool stopWhenAnimationStops;

	private bool hasAudioSource;

	private AudioSource audioSource;

	private void OnDisable()
	{
		if (hasAudioSource)
		{
			hasAudioSource = false;
			if (audioSource != null)
			{
				audioSource.Stop();
				audioSource = null;
			}
		}
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (hasAudioSource)
		{
			hasAudioSource = false;
			if (audioSource != null)
			{
				audioSource.Stop();
				audioSource = null;
			}
		}
		if (tryFindAudioSourceOnAnimator)
		{
			audioSource = animator.GetComponent<AudioSource>();
			hasAudioSource = audioSource != null;
			if (hasAudioSource)
			{
				audioSource.loop = true;
				loopedAudioClip.PlayOnSource(audioSource);
				return;
			}
		}
		audioSource = loopedAudioClip.SpawnAndPlayLooped(prefab, animator.transform.position, 0f, OnRecycled);
		hasAudioSource = audioSource != null;
		if (!hasAudioSource)
		{
			audioSource = animator.GetComponent<AudioSource>();
			hasAudioSource = audioSource != null;
			if (hasAudioSource)
			{
				audioSource.loop = true;
				loopedAudioClip.PlayOnSource(audioSource);
			}
		}
	}

	private void OnRecycled()
	{
		audioSource = null;
		hasAudioSource = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stopWhenAnimationStops && hasAudioSource && !animator.IsInTransition(layerIndex) && stateInfo.normalizedTime >= 1f)
		{
			audioSource.Stop();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (hasAudioSource && audioSource.isPlaying)
		{
			audioSource.Stop();
			audioSource = null;
			hasAudioSource = false;
		}
	}
}
