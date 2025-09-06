using HutongGames.PlayMaker;
using UnityEngine;

public class AnimSpeedLerp : FsmStateAction
{
	public FsmOwnerDefault target;

	public FsmFloat duration;

	public FsmFloat toSpeed;

	private float elapsed;

	private float fromSpeed;

	private Animator animator;

	public override void Reset()
	{
		target = null;
		duration = null;
		toSpeed = null;
	}

	public override void OnEnter()
	{
		elapsed = 0f;
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			animator = safe.GetComponent<Animator>();
			if ((bool)animator)
			{
				fromSpeed = animator.speed;
				return;
			}
		}
		Finish();
	}

	public override void OnUpdate()
	{
		if ((bool)animator)
		{
			animator.speed = Mathf.Lerp(fromSpeed, toSpeed.Value, Mathf.Min(1f, elapsed / duration.Value));
			elapsed += Time.deltaTime;
		}
	}

	public override void OnExit()
	{
		if ((bool)animator)
		{
			animator.speed = 0f;
		}
	}
}
