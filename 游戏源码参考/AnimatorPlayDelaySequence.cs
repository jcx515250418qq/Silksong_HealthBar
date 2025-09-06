using System;
using System.Collections;
using UnityEngine;

public class AnimatorPlayDelaySequence : MonoBehaviour
{
	[Serializable]
	private class AnimatorGroup
	{
		public Animator[] Animators;
	}

	[SerializeField]
	private AnimatorGroup[] animatorGroups;

	[SerializeField]
	private float startDelayForward;

	[SerializeField]
	private float startDelayReverse;

	[SerializeField]
	private float delayBetweenGroups;

	[Space]
	[SerializeField]
	private string startAtEndAnim;

	private Coroutine playRoutine;

	private void Start()
	{
		SetAtEnd();
	}

	private void SetAtEnd()
	{
		if (string.IsNullOrEmpty(startAtEndAnim))
		{
			return;
		}
		AnimatorGroup[] array = animatorGroups;
		for (int i = 0; i < array.Length; i++)
		{
			Animator[] animators = array[i].Animators;
			foreach (Animator animator in animators)
			{
				if ((bool)animator)
				{
					animator.Play(startAtEndAnim, 0, 1f);
				}
			}
		}
	}

	public void Play(string animName)
	{
		if (playRoutine != null)
		{
			StopCoroutine(playRoutine);
		}
		playRoutine = StartCoroutine(Play(animName, isReversed: false));
	}

	public void PlayReversed(string animName)
	{
		if (playRoutine != null)
		{
			StopCoroutine(playRoutine);
		}
		playRoutine = StartCoroutine(Play(animName, isReversed: true));
	}

	private IEnumerator Play(string animName, bool isReversed)
	{
		yield return new WaitForSeconds(isReversed ? startDelayReverse : startDelayForward);
		bool doneFirst = false;
		for (int i = (isReversed ? (animatorGroups.Length - 1) : 0); isReversed ? (i >= 0) : (i < animatorGroups.Length); i += ((!isReversed) ? 1 : (-1)))
		{
			if (doneFirst)
			{
				yield return new WaitForSeconds(delayBetweenGroups);
			}
			else
			{
				doneFirst = true;
			}
			Animator[] animators = animatorGroups[i].Animators;
			foreach (Animator animator in animators)
			{
				if ((bool)animator)
				{
					animator.Play(animName);
				}
			}
		}
	}
}
