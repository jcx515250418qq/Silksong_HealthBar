using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorControlSequence : MonoBehaviour
{
	[SerializeField]
	private string stateName;

	private Animator animator;

	private bool alreadyPlayed;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private IEnumerator Start()
	{
		if (!alreadyPlayed)
		{
			animator.enabled = true;
			animator.Play(stateName, 0, 0f);
			yield return null;
			animator.enabled = false;
		}
	}

	[ContextMenu("Play From Start", true)]
	[ContextMenu("Play From End", true)]
	private bool CanPlay()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Play From Start")]
	public void PlayAnimatorFromStart()
	{
		animator.enabled = true;
		animator.Play(stateName, 0, 0f);
		alreadyPlayed = true;
	}

	[ContextMenu("Play From End")]
	public void PlayAnimatorFromEnd()
	{
		animator.enabled = true;
		animator.Play(stateName, 0, 1f);
		alreadyPlayed = true;
	}
}
