using System.Collections;

public class Tk2dTalkAnimNPC : TalkAnimNPC
{
	private tk2dSpriteAnimator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	protected override void PlayAnim(string animName)
	{
		animator.Play(animName);
	}

	protected override IEnumerator PlayAnimWait(string animName)
	{
		return animator.PlayAnimWait(animName);
	}
}
