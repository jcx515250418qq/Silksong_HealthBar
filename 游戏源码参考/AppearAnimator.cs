using System.Collections;
using UnityEngine;

public class AppearAnimator : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private static readonly int _appearAnimId = Animator.StringToHash("Appear");

	private void Reset()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Disappear()
	{
		StopAllCoroutines();
		if ((bool)animator)
		{
			animator.enabled = false;
		}
		if ((bool)spriteRenderer)
		{
			spriteRenderer.enabled = false;
		}
	}

	public void Appear()
	{
		StopAllCoroutines();
		StartCoroutine(AppearRoutine());
	}

	private IEnumerator AppearRoutine()
	{
		if ((bool)spriteRenderer)
		{
			spriteRenderer.enabled = true;
			spriteRenderer.sprite = null;
		}
		if ((bool)animator)
		{
			AnimatorCullingMode culling = animator.cullingMode;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.enabled = true;
			animator.Play(_appearAnimId);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			animator.cullingMode = culling;
		}
	}
}
