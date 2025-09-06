using System.Collections;
using UnityEngine;

public class RandomLooptk2dAnimator : MonoBehaviour
{
	[SerializeField]
	private string defaultAnim;

	[SerializeField]
	private string variantAnim;

	[SerializeField]
	private float minDelay = 1f;

	[SerializeField]
	private float maxDelay = 2f;

	private tk2dSpriteAnimator animator;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	private void OnEnable()
	{
		if ((bool)animator)
		{
			StartCoroutine(Animate());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator Animate()
	{
		animator.Play(defaultAnim);
		yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
		while (true)
		{
			yield return StartCoroutine(animator.PlayAnimWait(defaultAnim));
			yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
			yield return StartCoroutine(animator.PlayAnimWait(variantAnim));
		}
	}
}
