using System.Collections;
using UnityEngine;

public class AnimatorTalkAnimNPC : TalkAnimNPC
{
	private Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	protected override void PlayAnim(string animName)
	{
		animator.Play(animName);
	}

	protected override IEnumerator PlayAnimWait(string animName)
	{
		animator.Play(animName);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}
}
