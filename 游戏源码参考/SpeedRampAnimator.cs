using TeamCherry.SharedUtils;
using UnityEngine;

public class SpeedRampAnimator : SpeedChanger
{
	[SerializeField]
	private MinMaxFloat speedRange;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float duration;

	private Coroutine animationRoutine;

	public void StartSpeedRamp()
	{
		StopSpeedRamp();
		animationRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
		{
			time = curve.Evaluate(time);
			float lerpedValue = speedRange.GetLerpedValue(time);
			CallSpeedChangedEvent(lerpedValue);
		});
	}

	public void StopSpeedRamp()
	{
		if (animationRoutine != null)
		{
			StopCoroutine(animationRoutine);
		}
	}
}
