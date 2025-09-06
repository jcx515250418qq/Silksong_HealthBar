using UnityEngine;

public class tk2dLookAnimNPC : LookAnimNPC
{
	private tk2dSpriteAnimator anim;

	private tk2dSprite sprite;

	private bool hasSprite;

	protected override void Awake()
	{
		base.Awake();
		anim = GetComponent<tk2dSpriteAnimator>();
		_ = (bool)anim;
		sprite = (anim ? (anim.Sprite as tk2dSprite) : GetComponent<tk2dSprite>());
		hasSprite = sprite;
	}

	protected override float GetXScale()
	{
		if (hasSprite)
		{
			return sprite.scale.x * base.transform.lossyScale.x;
		}
		return base.GetXScale();
	}

	protected override void FlipSprite()
	{
		if (hasSprite)
		{
			Vector3 scale = sprite.scale;
			scale.x *= -1f;
			sprite.scale = scale;
		}
	}

	protected override bool GetIsSpriteFlipped()
	{
		if (hasSprite)
		{
			return sprite.scale.x * base.transform.lossyScale.x < 0f;
		}
		return false;
	}

	protected override bool GetWasFacingLeft()
	{
		if (turnFlipType != 0)
		{
			float num = (hasSprite ? (sprite.scale.x * base.transform.lossyScale.x) : base.transform.lossyScale.x);
			if (!base.DefaultLeft)
			{
				return num < 0f;
			}
			return num > 0f;
		}
		return base.GetWasFacingLeft();
	}

	protected override void EnsureCorrectFacing()
	{
		if (turnFlipType == TurnFlipTypes.NoFlip)
		{
			return;
		}
		if (GetWasFacingLeft())
		{
			if (base.State == AnimState.Right)
			{
				base.State = AnimState.Left;
			}
		}
		else if (base.State == AnimState.Left)
		{
			base.State = AnimState.Right;
		}
	}

	protected override void PlayAnim(string animName)
	{
		if ((bool)anim && !string.IsNullOrEmpty(animName))
		{
			anim.Play(animName);
		}
	}

	protected override bool IsAnimPlaying(string animName)
	{
		if (!anim || string.IsNullOrEmpty(animName))
		{
			return false;
		}
		if (anim.IsPlaying(animName))
		{
			return anim.ClipTimeSeconds <= anim.CurrentClip.Duration;
		}
		return false;
	}
}
