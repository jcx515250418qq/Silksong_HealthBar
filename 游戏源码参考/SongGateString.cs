using System.Collections;
using UnityEngine;

public class SongGateString : MonoBehaviour
{
	[SerializeField]
	private Transform doorMark;

	private tk2dSpriteAnimator animator;

	private float distance;

	private Coroutine timerRoutine;

	private void Start()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		distance = Mathf.Abs(base.transform.position.x - doorMark.position.x);
	}

	public void StrumStart()
	{
		float delay = 2f - distance * 0.35f;
		StartTimerRoutine(delay, "Strum", 0f);
	}

	public void StrumEnd()
	{
		float delay = Random.Range(0.1f, 0.25f);
		StartTimerRoutine(delay, "Idle", 0f);
	}

	private void StartTimerRoutine(float delay, string anim, float stopDelay)
	{
		if (timerRoutine != null)
		{
			StopCoroutine(timerRoutine);
		}
		timerRoutine = StartCoroutine(PlayAnimDelayed(delay, anim, stopDelay));
	}

	private IEnumerator PlayAnimDelayed(float delay, string anim, float stopDelay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		animator.Play(anim);
		if (!(stopDelay <= 0f))
		{
			yield return new WaitForSeconds(stopDelay);
			animator.Play("Idle");
		}
	}

	public void QuickStrum()
	{
		StartTimerRoutine(0f, "Strum", 0.5f + Random.Range(0f, 0.15f));
	}
}
