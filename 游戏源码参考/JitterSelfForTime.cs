using UnityEngine;

public class JitterSelfForTime : JitterSelf
{
	[Space]
	[SerializeField]
	private float duration;

	[SerializeField]
	private AnimationCurve decayCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	private Coroutine delayedRoutine;

	public void StartTimedJitter()
	{
		if (!(duration <= 0f))
		{
			if (delayedRoutine != null)
			{
				StopCoroutine(delayedRoutine);
			}
			InternalStopJitter(withDecay: false);
			StartJitter();
			delayedRoutine = this.StartTimerRoutine(0f, duration, delegate(float t)
			{
				base.Multiplier = decayCurve.Evaluate(t);
			}, null, base.StopJitter);
		}
	}

	protected override void OnStopJitter()
	{
		if (delayedRoutine != null)
		{
			StopCoroutine(delayedRoutine);
			delayedRoutine = null;
		}
	}

	public static JitterSelfForTime AddHandler(GameObject gameObject, Vector3 jitterAmount, float jitterDuration, float jitterFrequency)
	{
		JitterSelfForTime jitterSelfForTime = gameObject.AddComponent<JitterSelfForTime>();
		jitterSelfForTime.duration = jitterDuration;
		jitterSelfForTime.startInactive = true;
		JitterSelfConfig jitterSelfConfig = jitterSelfForTime.config;
		jitterSelfConfig.AmountMin = (jitterSelfConfig.AmountMax = jitterAmount);
		jitterSelfConfig.Frequency = jitterFrequency;
		jitterSelfConfig.UseCameraRenderHooks = true;
		jitterSelfForTime.config = jitterSelfConfig;
		return jitterSelfForTime;
	}
}
