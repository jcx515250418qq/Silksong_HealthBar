using System.Collections;
using UnityEngine;

public class TimeScaleIndependentUpdate : MonoBehaviour
{
	private float previousTimeSinceStartup;

	public float deltaTime { get; private set; }

	protected virtual void Awake()
	{
		previousTimeSinceStartup = Time.realtimeSinceStartup;
	}

	protected virtual void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		deltaTime = realtimeSinceStartup - previousTimeSinceStartup;
		previousTimeSinceStartup = realtimeSinceStartup;
		if (deltaTime < 0f)
		{
			deltaTime = 0f;
		}
	}

	public IEnumerator TimeScaleIndependentWaitForSeconds(float seconds)
	{
		for (float elapsedTime = 0f; elapsedTime < seconds; elapsedTime += deltaTime)
		{
			yield return null;
		}
	}
}
