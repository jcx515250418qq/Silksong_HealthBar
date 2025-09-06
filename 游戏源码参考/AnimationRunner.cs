using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationRunner : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string idleState = "Idle";

	[SerializeField]
	private string playState;

	[SerializeField]
	private float endDelay;

	[SerializeField]
	[Space]
	private UnityEvent animationFinished;

	private Coroutine invokeRoutine;

	private void OnEnable()
	{
		PlayAnim(idleState, invokeEvent: false);
	}

	public void Play()
	{
		PlayAnim(playState, invokeEvent: true);
	}

	private void PlayAnim(string stateName, bool invokeEvent)
	{
		if ((bool)animator && base.gameObject.activeInHierarchy)
		{
			if (invokeRoutine != null)
			{
				StopCoroutine(invokeRoutine);
				invokeRoutine = null;
				animationFinished.Invoke();
			}
			animator.Play(stateName);
			if (invokeEvent)
			{
				invokeRoutine = StartCoroutine(InvokeAnimEndEvent());
			}
		}
	}

	private IEnumerator InvokeAnimEndEvent()
	{
		AnimatorStateInfo beforeState = animator.GetCurrentAnimatorStateInfo(0);
		yield return null;
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (beforeState.GetHashCode() != currentAnimatorStateInfo.GetHashCode())
		{
			yield return new WaitForSeconds(currentAnimatorStateInfo.length + endDelay);
			animationFinished.Invoke();
		}
		invokeRoutine = null;
	}
}
