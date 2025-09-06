using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BellShrineTuningFork : MonoBehaviour
{
	public enum States
	{
		Dormant = 0,
		Activated = 1,
		AllComplete = 2
	}

	private static readonly int _activatedId = Animator.StringToHash("Activated");

	private static readonly int _completedId = Animator.StringToHash("Completed");

	private static readonly int _singingId = Animator.StringToHash("Singing");

	private static readonly int _pulseId = Animator.StringToHash("Pulse");

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string bellShrineBool;

	[Space]
	[SerializeField]
	private Animator animator;

	private bool endWait;

	public bool IsBellShrineCompleted => PlayerData.instance.GetVariable<bool>(bellShrineBool);

	public void SetInitialState(States state)
	{
		switch (state)
		{
		case States.Dormant:
			animator.SetBool(_activatedId, value: false);
			animator.SetBool(_completedId, value: false);
			break;
		case States.Activated:
			animator.SetBool(_activatedId, value: true);
			animator.Play(_activatedId, 0, UnityEngine.Random.Range(0f, 1f));
			break;
		case States.AllComplete:
			animator.SetBool(_completedId, value: true);
			animator.Play(_completedId, 0, UnityEngine.Random.Range(0f, 1f));
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	public IEnumerator DoActivate()
	{
		endWait = false;
		animator.SetBool(_activatedId, value: true);
		yield return null;
		yield return new WaitForSecondsInterruptable(animator.GetCurrentAnimatorStateInfo(0).length, () => endWait);
	}

	public void FinishActivate()
	{
		endWait = true;
	}

	public void DoComplete(float delay)
	{
		StartCoroutine(CompleteRoutine(delay));
	}

	public void DoPulse()
	{
		animator.SetTrigger(_pulseId);
	}

	private IEnumerator CompleteRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		animator.SetBool(_completedId, value: true);
	}

	public void SetSinging(bool value)
	{
		animator.SetBool(_singingId, value);
	}
}
