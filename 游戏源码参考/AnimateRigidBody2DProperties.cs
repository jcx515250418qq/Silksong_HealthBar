using TeamCherry.SharedUtils;
using UnityEngine;

public class AnimateRigidBody2DProperties : BaseAnimator
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private MinMaxFloat linearDragRange;

	[SerializeField]
	private MinMaxFloat angularDragRange;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float duration;

	private Coroutine animationRoutine;

	public override void StartAnimation()
	{
		if (animationRoutine != null)
		{
			StopCoroutine(animationRoutine);
		}
		if ((bool)body)
		{
			animationRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				time = curve.Evaluate(time);
				body.linearDamping = Mathf.Lerp(linearDragRange.Start, linearDragRange.End, time);
				body.angularDamping = Mathf.Lerp(angularDragRange.Start, angularDragRange.End, time);
			});
		}
	}
}
