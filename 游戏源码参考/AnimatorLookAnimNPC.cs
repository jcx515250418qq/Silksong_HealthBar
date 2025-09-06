using UnityEngine;

public class AnimatorLookAnimNPC : LookAnimNPC
{
	[Header("Animator")]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer sprite;

	protected override void FlipSprite()
	{
		if ((bool)sprite)
		{
			sprite.flipX = !sprite.flipX;
		}
	}

	protected override bool GetIsSpriteFlipped()
	{
		if ((bool)sprite)
		{
			return sprite.flipX;
		}
		return false;
	}

	protected override void PlayAnim(string animName)
	{
		if ((bool)animator)
		{
			animator.Play(animName);
		}
	}

	protected override bool IsAnimPlaying(string animName)
	{
		if (!animator)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == Animator.StringToHash(animName) && currentAnimatorStateInfo.normalizedTime < 1f)
		{
			return true;
		}
		return false;
	}
}
